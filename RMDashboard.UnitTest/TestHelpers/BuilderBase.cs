using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Dsl;

namespace RMDashboard.UnitTest.TestHelpers
{
    class BuilderBase<T>
    {
        public BuilderBase()
        {
            Fixture = new Fixture().Build<T>();
        }

        protected IPostprocessComposer<T> Fixture { get; set; }

        public T Build()
        {
            return Fixture.Create<T>();
        }
    }
}
