using Nosbor.FluentBuilder.Internals.Commands;
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
        public void Should_set_default_values_for_all_string_complextypes_and_collection_fields_and_runtime_backing_fields()
        {
            var command = new SetDefaultValuesCommand(_object);

            command.Execute();

            Assert.IsNotNull(_object.PropertyOnlyForTestingPurpose);
            Assert.IsNotNull(_object.CollectionOnlyForTestingPurpose);

            Assert.IsNotNull(_object.BasePropertyOnlyForTestingPurpose);
            Assert.IsNotNull(_object.BaseCollectionOnlyForTestingPurpose);
        }

        private class BaseSampleType
        {
            private string baseField;

            private IList<AnotherSampleType> _baseCollectionField = new List<AnotherSampleType>();
            public IEnumerable<AnotherSampleType> BaseCollectionOnlyForTestingPurpose { get { return _baseCollectionField; } }
            public string BasePropertyOnlyForTestingPurpose { get { return baseField; } }
        }

        private class SampleTypeWithFieldAndProperties : BaseSampleType
        {
            private string field;
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
