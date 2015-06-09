namespace Nosbor.FluentBuilder.Tests.SampleClasses
{
    internal class ClassWithoutParameterlessCtor
    {
        public string Name { get; private set; }

        public ClassWithoutParameterlessCtor(string name)
        {
            Name = name;
        }
    }
}