using RMDashboard.Models;
using System;

namespace RMDashboard.UnitTest.TestHelpers
{
    class StepBuilder : BuilderBase<Step>
    {
        internal StepBuilder ForRelease(Release release)
        {
            if (release == null) throw new ArgumentNullException("release");

            Fixture = Fixture.With((step) => step.ReleaseId, release.Id);
            return this;
        }

        internal StepBuilder ForStage(Stage stage)
        {
            if (stage == null) throw new ArgumentNullException("stage");

            Fixture = Fixture.With((step) => step.StageId, stage.Id);
            return this;
        }

        internal StepBuilder WithAttempt(int attempt)
        {
            Fixture = Fixture.With((step) => step.Attempt, attempt);
            return this;
        }

        internal StepBuilder WithRank(int rank)
        {
            Fixture = Fixture.With((step) => step.StepRank, rank);
            return this;
        }
    }
}
