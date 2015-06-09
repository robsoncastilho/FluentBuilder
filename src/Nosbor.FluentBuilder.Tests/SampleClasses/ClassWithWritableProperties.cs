using System.Collections.Generic;

namespace Nosbor.FluentBuilder.Tests.SampleClasses
{
    internal class ClassWithWritableProperties
    {
        public string Name { get; private set; }
        public List<string> Addresses { get; protected set; }
    }
}