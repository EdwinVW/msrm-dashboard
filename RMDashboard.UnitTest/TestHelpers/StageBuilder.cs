using RMDashboard.Models;
using System;

namespace RMDashboard.UnitTest.TestHelpers
{
    class StageBuilder : BuilderBase<Stage>
    {
        internal StageBuilder ForEnvironment(Models.Environment environment)
        {
            if (environment == null) throw new ArgumentNullException("environment");

            Fixture = Fixture.With((stage) => stage.EnvironmentId, environment.Id);
            return this;
        }

        internal StageBuilder WithRank(int rank)
        {
            Fixture = Fixture.With((stage) => stage.Rank, rank);
            return this;
        }
    }
}
