using RMDashboard.Models;
using System;

namespace RMDashboard.UnitTest.TestHelpers
{
    class ComponentBuilder : BuilderBase<Component>
    {
        public ComponentBuilder ForRelease(Release release)
        {
            if (release == null) throw new ArgumentNullException("release");

            Fixture = Fixture.With((component) => component.ReleaseId, release.Id);
            return this;
        }

        public ComponentBuilder WithBuildDefinition(string buildDefinition)
        {
            Fixture = Fixture.With((component) => component.BuildDefinition, buildDefinition);
            return this;
        }
    }
}
