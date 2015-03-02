using RMDashboard.Models;
using System;

namespace RMDashboard.UnitTest.TestHelpers
{
    class StageWorkflowBuilder : BuilderBase<StageWorkflow>
    {
        internal StageWorkflowBuilder ForRelease(Release release)
        {
            if (release == null) throw new ArgumentNullException("release");

            Fixture = Fixture.With((stageWorkflow) => stageWorkflow.ReleaseId, release.Id);
            return this;
        }

        internal StageWorkflowBuilder ForStage(Stage stage)
        {
            if (stage == null) throw new ArgumentNullException("stage");

            Fixture = Fixture.With((stageWorkflow) => stageWorkflow.StageId, stage.Id);
            return this;
        }
    }
}
