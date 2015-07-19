namespace Nosbor.FluentBuilder.Tests.Lib.SampleClasses
{
    internal class SampleServiceWithDependency
    {
        private readonly IDependency _dependency;

        public SampleServiceWithDependency(IDependency dependency)
        {
            _dependency = dependency;
        }

        public IDependency PropertyOnlyForTestingPurpose { get { return _dependency; } }
    }

    internal interface IDependency
    {
        int GetSomething();
        string GetOtherThing();
    }

    internal class SampleConcreteDependency : IDependency
    {
        public int GetSomething()
        {
            return 0;
        }

        public string GetOtherThing()
        {
            return string.Empty;
        }
    }
}
