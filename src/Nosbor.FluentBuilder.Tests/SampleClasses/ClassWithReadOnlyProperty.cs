using System.Collections.Generic;

namespace Nosbor.FluentBuilder.Tests.SampleClasses
{
    internal class ClassWithReadOnlyProperty
    {
        private readonly IList<string> _addresses = new List<string>();
        public IEnumerable<string> Addresses { get { return _addresses; } }

        private readonly List<AnotherSampleClass> _anotherName = new List<AnotherSampleClass>();
        public IEnumerable<AnotherSampleClass> CollectionWithFieldNotFollowingNameConvention { get { return _anotherName; } }
    }
}