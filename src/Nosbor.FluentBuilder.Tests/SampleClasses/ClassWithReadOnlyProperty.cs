using System.Collections.Generic;

namespace Nosbor.FluentBuilder.Tests.SampleClasses
{
    internal class ClassWithReadOnlyProperty
    {
        private readonly IList<string> _addresses = new List<string>();
        public IEnumerable<string> Addresses { get { return _addresses; } }

        private readonly IList<string> _fieldWithNoMatchingName = new List<string>();
        public IEnumerable<string> BadAddresses { get { return _fieldWithNoMatchingName; } }
    }
}