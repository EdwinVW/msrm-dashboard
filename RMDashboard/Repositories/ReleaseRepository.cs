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
        public List<ReleasePath> GetReleasePaths()
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ReleaseManagement"].ConnectionString))
            {
                var sql = @"
                    select	Id, 
                            Name, 
                            Description
                    from    ReleasePath
                    where   IsDeleted = 0";
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
                    FROM ReleaseV2 release {1} 
                    Order By release.CreatedOn desc

                    -- releases
                    select	Id = release.Id,
                            Name = release.Name, 
		                    Status = status.Name,
                            CreatedOn = release.CreatedOn,
                            TargetStageId = release.TargetStageId,
							ReleasePathId = release.ReleasePathId,
							ReleasePathName = releasepath.Name
                    from	ReleaseV2 release
                    join	ReleasePath releasepath
					on		releasepath.Id = release.ReleasePathId
					join	ReleaseStatus status
                    on		status.Id = release.StatusId
                    where   release.Id in (SELECT Id FROM @ScopedReleases)
                    order by CreatedOn desc

                    -- releaseworkflows
                    select	Id, ReleaseId, StageId
                    from	ReleaseV2StageWorkflow
                    where   ReleaseId in (SELECT Id FROM @ScopedReleases)

                    -- stages
                    select	Id = stage.Id, 
                            Name = stagetype.Name, 
                            EnvironmentId = stage.EnvironmentId, 
                            Rank = stage.Rank, 
                            IsDeleted = stage.IsDeleted
                    from	Stage stage
                    join	StageType stagetype
                    on		stagetype.Id = stage.StageTypeId

                    -- environments / servers
                    select	Id = env.Id, 
		                    Name = env.Name
                    from	Environment env

                    -- releasesteps
                    select	Id = step.Id,
		                    Name = steptype.Name,
		                    Status = status.Name, 
		                    ReleaseId = step.ReleaseId,
		                    StageId = step.StageId,
                            StepRank = step.StepRank,
		                    StageRank = step.StageRank,
		                    EnvironmentId = step.EnvironmentId,
		                    Attempt = step.TrialNumber,
                            CreatedOn = step.CreatedOn,
                            ModifiedOn = step.ModifiedOn
                    from	ReleaseV2Step step
                    join	StageStepType steptype
                    on		steptype.Id = step.StepTypeId
                    join	ReleaseStepStatus status
                    on		status.Id = step.StatusId
                    where   step.ReleaseId in (SELECT Id FROM @ScopedReleases)
                    
                    -- components for selected releases
                    select  ReleaseId = releaseComponent.ReleaseId,
                            TeamProject = releaseComponent.TeamProject,
                            BuildDefinition = releaseComponent.BuildDefinition,
                            Build = releaseComponent.Build
                    from    ReleaseComponentV2 releaseComponent
                    where   releaseComponent.ReleaseId in (SELECT Id FROM @ScopedReleases)";

            // add where clause for filtering on ReleasePathId
            string whereClause = null;
            if (!string.IsNullOrEmpty(includedReleasePathIds))
            {
                whereClause = string.Format("where  release.ReleasePathId in ({0})", includedReleasePathIds);
            }
            sql = string.Format(sql, releaseCount, whereClause);
            return sql;
        }
    }
}