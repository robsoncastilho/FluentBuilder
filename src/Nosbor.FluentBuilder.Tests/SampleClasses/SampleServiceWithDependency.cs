namespace Nosbor.FluentBuilder.Tests.SampleClasses
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
    }

    internal class SampleConcreteDependency : IDependency
    {

    }
}
