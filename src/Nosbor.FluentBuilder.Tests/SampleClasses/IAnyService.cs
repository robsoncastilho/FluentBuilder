namespace Nosbor.FluentBuilder.Tests.SampleClasses
{
    public interface IAnyService
    {
    }

    public class ConcreteService : IAnyService
    {

    }

    public class ClassWithDependency
    {
        private readonly IAnyService _anyService;

        public ClassWithDependency(IAnyService anyService)
        {
            _anyService = anyService;
        }
    }
}
