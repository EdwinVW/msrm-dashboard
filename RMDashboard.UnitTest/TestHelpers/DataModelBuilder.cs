using RMDashboard.Models;
using System;
using System.Collections.Generic;

namespace RMDashboard.UnitTest.TestHelpers
{
    class DataModelBuilder
    {
        private List<Models.Environment> _environments;
        private DateTime _lastRefresh;
        private List<Component> _releaseComponents;
        private List<Release> _releases;
        private List<Step> _releaseSteps;
        private List<Stage> _stages;
        private List<StageWorkflow> _stageWorkflows;
        private List<DeploymentStep> _deploymentSteps;

        public DataModelBuilder()
        {
            _environments = new List<Models.Environment>();
            _lastRefresh = DateTime.Today;
            _releaseComponents = new List<Component>();
            _releases = new List<Release>();
            _releaseSteps = new List<Step>();
            _stages = new List<Stage>();
            _deploymentSteps = new List<DeploymentStep>();
            _stageWorkflows = new List<StageWorkflow>();
        }

        public DataModelBuilder WithDefaultDataSet()
        {
            WithEnvironment(new EnvironmentBuilder().Build());
            WithLastRefresh(DateTime.Today);
            WithComponent(new ComponentBuilder().Build());
            WithRelease(new ReleaseBuilder().Build());
            WithStep(new StepBuilder().Build());
            WithStage(new StageBuilder().Build());
            WithStageWorkflow(new StageWorkflowBuilder().Build());

            return this;
        }

        public DataModelBuilder WithEnvironment(Models.Environment environment)
        {
            _environments.Add(environment);
            return this;
        }

        public DataModelBuilder WithLastRefresh(DateTime lastRefresh)
        {
            _lastRefresh = lastRefresh;
            return this;
        }

        public DataModelBuilder WithComponent(Component component)
        {
            _releaseComponents.Add(component);
            return this;
        }

        public DataModelBuilder WithRelease(Release release)
        {
            _releases.Add(release);
            return this;
        }

        public DataModelBuilder WithStep(Step step)
        {
            _releaseSteps.Add(step);
            return this;
        }

        public DataModelBuilder WithStage(Stage stage)
        {
            _stages.Add(stage);
            return this;
        }

        public DataModelBuilder WithStageWorkflowFor(Release release, Stage stage)
        {
            var stageWorkflow = new StageWorkflowBuilder()
                .ForRelease(release)
                .ForStage(stage)
                .Build();

            _stageWorkflows.Add(stageWorkflow);

            return this;
        }

        public DataModelBuilder WithStageWorkflow(StageWorkflow stageWorkflow)
        {
            _stageWorkflows.Add(stageWorkflow);
            return this;
        }

        public DataModelBuilder WithDeploymentStep(DeploymentStep deploymentStep)
        {
            _deploymentSteps.Add(deploymentStep);
            return this;
        }

        public DataModel Build()
        {
            return new DataModel()
            {
                Environments = _environments,
                LastRefresh = _lastRefresh,
                ReleaseComponents = _releaseComponents,
                Releases = _releases,
                ReleaseSteps = _releaseSteps,
                Stages = _stages,
                StageWorkflows = _stageWorkflows,
                DeploymentSteps = _deploymentSteps
            };
        }
    }
}
