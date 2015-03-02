using RMDashboard.Models;
using System.Collections.Generic;

namespace RMDashboard.UnitTest.TestHelpers
{
    class ReleasePathsBuilder
    {
        private List<ReleasePath> _releasePaths;

        public ReleasePathsBuilder()
        {
            _releasePaths = new List<ReleasePath>();
        }

        public ReleasePathsBuilder WithMany()
        {
            _releasePaths.Add(new ReleasePathBuilder().Build());
            _releasePaths.Add(new ReleasePathBuilder().Build());
            _releasePaths.Add(new ReleasePathBuilder().Build());

            return this;
        }

        public ReleasePathsBuilder Empty()
        {
            _releasePaths = new List<ReleasePath>();
            return this;
        }

        public List<ReleasePath> Build()
        {
            return _releasePaths;
        }
    }
}
