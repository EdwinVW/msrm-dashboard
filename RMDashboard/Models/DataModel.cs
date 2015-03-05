using System;
using System.Collections.Generic;

namespace RMDashboard.Models
{
    public class DataModel
    {
        public DateTime LastRefresh;
        public List<Release> Releases;
        public List<StageWorkflow> StageWorkflows;
        public List<Stage> Stages;
        public List<Environment> Environments;
        public List<Step> ReleaseSteps;
        public List<Component> ReleaseComponents;
    }

    public class Release
    {
        public int Id;
        public string Name;
        public string Status;
        public DateTime CreatedOn;
        public int TargetStageId;
        public int ReleasePathId;
        public string ReleasePathName;
    }

    public class StageWorkflow
    {
        public int Id;
        public int ReleaseId;
        public int StageId;
    }

    public class Stage
    {
        public int Id;
        public string Name;
        public int EnvironmentId;
        public int Rank;
        public bool IsDeleted;
    }

    public class Environment
    {
        public int Id;
        public string Name;
    }

    public class Step
    {
        public int Id;
        public string Name;
        public string Status;
        public int ReleaseId;
        public int StageId;
        public int StepRank;
        public int StageRank;
        public int EnvironmentId;
        public int Attempt;
        public DateTime CreatedOn;
        public DateTime ModifiedOn;
    }

    public class ReleasePath
    {
        public int Id;
        public string Name;
        public string Description;
    }

    public class Component
    {
        public int ReleaseId;
        public string TeamProject;
        public string BuildDefinition;
        public string Build;
    }
}