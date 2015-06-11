using System.Collections.Generic;

namespace Nosbor.FluentBuilder.Tests.SampleClasses
{
    internal class ClassWithReadOnlyProperty
    {
        private readonly IList<string> _addresses = new List<string>();
        public IEnumerable<string> Addresses { get { return _addresses; } }

        private readonly List<AnotherSampleClass> _sampleObjects = new List<AnotherSampleClass>();
        public IEnumerable<AnotherSampleClass> SampleObjects { get { return _sampleObjects; } }
    }
}