using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Dapper;

namespace RMDashboard.Controllers
{
    public class ReleasesController : ApiController
    {
        // GET: api/release
        public object Get(HttpRequestMessage message)
        {
            dynamic result = new ExpandoObject();
            result.lastRefresh = DateTime.Now;
            result.releases = new List<dynamic>();

            // determine ReleasePathIds to include based on HTTP header
            string includedReleasePathIds = null;
            if (message.Headers.Contains("includedReleasePathIds"))
            {
                includedReleasePathIds = message.Headers.GetValues("includedReleasePathIds").First();
            }

            var data = GetData(includedReleasePathIds);
            var releases = data.Releases;
            foreach (var releaseData in releases)
            {
                // release
                dynamic release = new ExpandoObject();
                result.releases.Add(release);
                release.name = releaseData.Name;
                release.status = releaseData.Status;
                release.createdOn = releaseData.CreatedOn;
                release.targetStageId = releaseData.TargetStageId;
                release.releasePathName = releaseData.ReleasePathName;
                release.stages = new List<dynamic>();

                // stages
                var stages = data.Stages
                    .Where(stage => data.StageWorkflows.Any(wf => wf.StageId == stage.Id && wf.ReleaseId == releaseData.Id))
                    .OrderBy(stage => stage.Rank);
                foreach (var stageData in stages)
                {
                    dynamic stage = new ExpandoObject();
                    release.stages.Add(stage);
                    stage.id = stageData.Id;
                    stage.name = stageData.Name;
                    stage.rank = stageData.Rank;
                    var environmentData = data.Environments.First(env => env.Id == stageData.EnvironmentId);
                    stage.environment = environmentData.Name;

                    // Steps
                    stage.steps = new List<dynamic>();
                    var steps = data.ReleaseSteps
                        .Where(step => step.StageId == stageData.Id && step.ReleaseId == releaseData.Id)
                        .OrderBy(step => step.Attempt)
                        .ThenBy(step => step.StepRank);
                    foreach (var stepData in steps)
                    {
                        dynamic step = new ExpandoObject();
                        stage.steps.Add(step);
                        step.id = stepData.Id;
                        step.name = stepData.Name;
                        step.status = stepData.Status;
                        step.rank = stepData.StepRank;
                        step.createdOn = stepData.CreatedOn;
                    }
                }
            }

            return result;
        }

        private DataModel GetData(string includedReleasePathIds)
        {
            var data = new DataModel { LastRefresh = DateTime.Now };
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ReleaseManagement"].ConnectionString))
            {
                var sql = @"
                    -- releases
                    select	top 5 
                            Id = release.Id,
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
                    {0}
                    order by CreatedOn desc

                    -- releaseworkflows
                    select	Id, ReleaseId, StageId
                    from	ReleaseV2StageWorkflow

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
                    on		status.Id = step.StatusId";

                // add where clause for filtering on ReleasePathId
                string whereClause = null;
                if (!string.IsNullOrEmpty(includedReleasePathIds))
                {
                    whereClause = string.Format("where  release.ReleasePathId in ({0})", includedReleasePathIds);
                }
                sql = string.Format(sql, whereClause);

                using (var multi = connection.QueryMultiple(sql))
                {
                    data.Releases = multi.Read<Release>().ToList();
                    data.StageWorkflows = multi.Read<StageWorkflow>().ToList();
                    data.Stages = multi.Read<Stage>().ToList();
                    data.Environments = multi.Read<Environment>().ToList();
                    data.ReleaseSteps = multi.Read<Step>().ToList();
                }
            }
            return data;
        }
    }
}
