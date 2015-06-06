using System.Collections.Generic;

namespace FluentBuilderLib.Tests.SampleClasses
{
    internal class ClassWithWritableProperties
    {
        public string Name { get; private set; }
        public List<string> Addresses { get; protected set; }
    }
}