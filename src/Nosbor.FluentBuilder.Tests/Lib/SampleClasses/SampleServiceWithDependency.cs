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
        int Count();
    }

    internal class SampleConcreteDependency : IDependency
    {
        public int Count()
        {
            return 0;
        }
    }
}
