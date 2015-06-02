using RMDashboard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMDashboard.UnitTest.TestHelpers
{
    class DeploymentStepBuilder : BuilderBase<Models.DeploymentStep>
    {
        internal DeploymentStepBuilder ForRelease(Release release)
        {
            if (release == null) throw new ArgumentNullException("release");

            Fixture = Fixture.With((deploymentStep) => deploymentStep.ReleaseId, release.Id);
            return this;
        }

        internal DeploymentStepBuilder ForStage(Stage stage)
        {
            if (stage == null) throw new ArgumentNullException("stage");

            Fixture = Fixture.With((deploymentStep) => deploymentStep.StageId, stage.Id);
            return this;
        }

        internal DeploymentStepBuilder ForStep(Step step)
        {
            if (step == null) throw new ArgumentNullException("step");

            Fixture = Fixture.With((deploymentStep) => deploymentStep.ReleaseStepId, step.Id);
            return this;
        }
    }
}
