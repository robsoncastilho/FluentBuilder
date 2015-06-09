using System.Collections.Generic;

namespace Nosbor.FluentBuilder.Tests.SampleClasses
{
    internal class ClassWithReadOnlyProperty
    {
        private readonly List<string> _addresses = new List<string>();
        public List<string> Addresses { get { return _addresses; } }

        private readonly List<string> _fieldWithNoMatchingName = new List<string>();
        public List<string> BadAddresses { get { return _fieldWithNoMatchingName; } }
    }
}