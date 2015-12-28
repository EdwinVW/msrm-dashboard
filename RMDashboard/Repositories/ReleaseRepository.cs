using Dapper;
using RMDashboard.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace RMDashboard.Repositories
{
    internal class ReleaseRepository : IReleaseRepository
    {
        private static bool _Initialized;
        private static Version _DatabaseVersion;
        private static string _TablePrefix = null;

        internal ReleaseRepository()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (!_Initialized)
            {
                _DatabaseVersion = DetermineVersion();
                _TablePrefix = DetermineTablePrefix();

                _Initialized = true;
            }
        }

        private Version DetermineVersion()
        {
            Version databaseVersion = new Version();
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ReleaseManagement"].ConnectionString))
            {
                if (connection.Query("select * from sys.tables where name = 'Version'").Count() > 0)
                {
                    var version = connection.Query("select top(1) Number from Version").First();
                    databaseVersion = new Version(version.Number);
                }
                else
                {
                    var version = connection.Query("select top(1) Number from RM.tbl_Version").First();
                    databaseVersion = new Version(version.Number);
                }
            }

            return databaseVersion;
        }

        private static string DetermineTablePrefix()
        {
            // In RM 2015 RC, the tables are placed in a schema 'RM' and are prefixed witg 'tbl_'.
            // To make sure the dashboard works with both the 2013 as the 2015 version, a table prefix 
            // is determined based on the version-number in te database.
            if (_DatabaseVersion.Major >= 14 && _DatabaseVersion.Build >= 22821)
            {
                return "RM.tbl_";
            }
            else
            {
                return string.Empty;
            }
        }

        public List<ReleasePath> GetReleasePaths()
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ReleaseManagement"].ConnectionString))
            {
                var sql = @"
                    select	Id, 
                            Name, 
                            Description
                    from    ##tableprefix##ReleasePath
                    where   IsDeleted = 0";

                // handle version-dependent table-prefix
                sql = sql.Replace("##tableprefix##", _TablePrefix);

                return connection.Query<ReleasePath>(sql).ToList();
            }
        }

        public DataModel GetReleaseData(string includedReleasePathIds, int releaseCount)
        {
            var data = new DataModel { LastRefresh = DateTime.Now };

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ReleaseManagement"].ConnectionString))
            {
                var sql = GenerateSQL(releaseCount, includedReleasePathIds);

                // query database
                using (var multi = connection.QueryMultiple(sql, new { numberOfReleasesToShow = releaseCount }))
                {
                    data.Releases = multi.Read<Release>().ToList();
                    data.StageWorkflows = multi.Read<StageWorkflow>().ToList();
                    data.Stages = multi.Read<Stage>().ToList();
                    data.Environments = multi.Read<Models.Environment>().ToList();
                    data.ReleaseSteps = multi.Read<Step>().ToList();
                    data.DeploymentSteps = multi.Read<DeploymentStep>().ToList();
                    data.ReleaseComponents = multi.Read<Component>().ToList();
                }
            }
            return data;
        }

        private string GenerateSQL(int releaseCount, string includedReleasePathIds)
        {
            var sql = @"
                    DECLARE @ScopedReleases TABLE
                    (
                       Id INT NOT NULL
                    )

                    INSERT INTO @ScopedReleases
                    SELECT TOP {0} Id 
                    FROM ##tableprefix##ReleaseV2 release {1}  
                    Order By release.CreatedOn desc

                    -- releases
                    select	Id = release.Id,
                            Name = release.Name, 
		                    Status = status.Name,
                            CreatedOn = release.CreatedOn,
                            TargetStageId = release.TargetStageId,
							ReleasePathId = release.ReleasePathId,
							ReleasePathName = releasepath.Name
                    from	##tableprefix##ReleaseV2 release
                    join	##tableprefix##ReleasePath releasepath
					on		releasepath.Id = release.ReleasePathId
					join	##tableprefix##ReleaseStatus status
                    on		status.Id = release.StatusId
                    where   release.Id in (SELECT Id FROM @ScopedReleases)
                    order by CreatedOn desc

                    -- releaseworkflows
                    select	Id, ReleaseId, StageId
                    from	##tableprefix##ReleaseV2StageWorkflow
                    where   ReleaseId in (SELECT Id FROM @ScopedReleases)

                    -- stages
                    select	Id = stage.Id, 
                            Name = stagetype.Name, 
                            EnvironmentId = stage.EnvironmentId, 
                            Rank = stage.Rank, 
                            IsDeleted = stage.IsDeleted
                    from	##tableprefix##Stage stage
                    join	##tableprefix##StageType stagetype
                    on		stagetype.Id = stage.StageTypeId

                    -- environments / servers
                    select	Id = env.Id, 
		                    Name = env.Name
                    from	##tableprefix##Environment env

                    -- releasesteps
                    select	    Id = step.Id,
		                        Name = steptype.Name,
		                        Status = status.Name, 
                                IsAutomated = step.IsAutomated,
                                CASE WHEN step.ApproverId IS NOT NULL
                                    THEN [user].DisplayName
                                    ELSE [group].Name
                                END AS Approver,
		                        ReleaseId = step.ReleaseId,
		                        StageId = step.StageId,
                                StepRank = step.StepRank,
		                        StageRank = step.StageRank,
		                        EnvironmentId = step.EnvironmentId,
		                        Attempt = step.TrialNumber,
                                CreatedOn = step.CreatedOn,
                                ModifiedOn = step.ModifiedOn
                    from	    ##tableprefix##ReleaseV2Step step
                    join	    ##tableprefix##StageStepType steptype
                    on		    steptype.Id = step.StepTypeId
                    join	    ##tableprefix##ReleaseStepStatus status
                    on		    status.Id = step.StatusId
                    left join   ##tableprefix##User [user]
                    on          step.ApproverId = [user].Id
                    left join   ##tableprefix##SecurityGroup [group]
                    on          step.ApproverGroupId = [group].Id
                    where       step.ReleaseId in (SELECT Id FROM @ScopedReleases)
                    
                    -- deployment steps
                    select      stageWorkflow.ReleaseId,
                                stageWorkflow.StageId,
                                releaseStep.Id AS releaseStepId,
                                x.value('@DisplayName', 'varchar(max)') as 'activityDisplayName',
                                CASE 
									WHEN activityLog.[Status] = 1 THEN 'Pending'
									WHEN activityLog.[Status] = 2 THEN 'In Progress'
									WHEN activityLog.[Status] = 3 THEN 'Succeeded'
									WHEN activityLog.[Status] = 4 THEN 'Failed'
									WHEN activityLog.[Status] = 5 THEN 'Cancelled'
                                    ELSE 'Unknown'
                                END AS [Status],
                                activityLog.DateStarted,
                                activityLog.DateEnded
                    from        ##tableprefix##ReleaseV2ActivityLog activityLog  
                    left join   ##tableprefix##ReleaseV2Step releaseStep 
                    on          activityLog.ReleaseStepId = releaseStep.Id
                    inner join  ##tableprefix##ReleaseV2StageWorkflow stageWorkflow
                    ON          (activityLog.ReleaseId = stageWorkflow.ReleaseId AND releaseStep.StageId = stageWorkflow.StageId)
                    cross apply stageWorkflow.[Workflow].nodes('{2}//ActionActivity') as aa(x)
                    where       stageWorkflow.ReleaseId in (SELECT Id FROM @ScopedReleases)
                    and         x.value('@WorkflowActivityId', 'varchar(max)') = activityLog.WorkflowActivityId	

                    -- components for selected releases
                    select  ReleaseId = releaseComponent.ReleaseId,
                            TeamProject = releaseComponent.TeamProject,
                            BuildDefinition = releaseComponent.BuildDefinition,
                            Build = releaseComponent.Build
                    from    ##tableprefix##ReleaseComponentV2 releaseComponent
                    where   releaseComponent.ReleaseId in (SELECT Id FROM @ScopedReleases)";

            // handle version-dependent table-prefix
            sql = sql.Replace("##tableprefix##", _TablePrefix);

            // add where clause for filtering on ReleasePathId
            string whereClause = null;
            if (!string.IsNullOrEmpty(includedReleasePathIds))
            {
                whereClause = string.Format("where release.ReleasePathId in ({0})", includedReleasePathIds);
            }
            const string defaultWorkflowXmlNamespace = "declare default element namespace \"clr-namespace:Microsoft.TeamFoundation.Release.Workflow.Activities;assembly=Microsoft.TeamFoundation.Release.Workflow\";";
            sql = string.Format(sql, releaseCount, whereClause, defaultWorkflowXmlNamespace);

            return sql;
        }
    }
}