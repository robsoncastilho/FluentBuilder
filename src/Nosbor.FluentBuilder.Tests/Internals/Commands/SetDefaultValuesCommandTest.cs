using Nosbor.FluentBuilder.Internals.Commands;
using Nosbor.FluentBuilder.Internals.DefaultValueGenerators;
using NUnit.Framework;
using System.Collections.Generic;

namespace Nosbor.FluentBuilder.Tests.Internals.Commands
{
    [TestFixture]
    public class SetDefaultValuesCommandTest
    {
        private SampleTypeWithFieldAndProperties _object;

        [SetUp]
        public void SetUp()
        {
            _object = new SampleTypeWithFieldAndProperties();
        }

        [Test]
        public void Should_set_default_values_for_all_string_complextypes_and_collection_members()
        {
            var command = new SetDefaultValuesCommand(_object, new DefaultValueGeneratorFactory());

            command.Execute();

            Assert.IsNotNull(_object.PropertyOnlyForTestingPurpose);
            Assert.IsNotNull(_object.sampleType);
            Assert.IsNotNull(_object.CollectionOnlyForTestingPurpose);
        }

        private class SampleTypeWithFieldAndProperties
        {
            private string field;
            public AnotherSampleType sampleType;
            private IList<AnotherSampleType> sampleTypes;

            public string PropertyOnlyForTestingPurpose { get { return field; } }
            public IEnumerable<AnotherSampleType> CollectionOnlyForTestingPurpose { get { return sampleTypes; } }

            public void MemberNotPropertyNorField() { }
        }

        private class AnotherSampleType
        {
            public string stringField;
        }
    }
}
