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

        private class SampleTypeWithFieldAndProperties
        {
            private string field;
            public AnotherSampleType sampleType;
            public IList<AnotherSampleType> sampleTypes;

            public int WritableProperty { get; private set; }

            private int _readOnlyPropertyWithUnderLyingField;
            public int ReadOnlyPropertyWithUnderLyingField { get { return _readOnlyPropertyWithUnderLyingField; } }

            public string PropertyOnlyForTestingPurpose { get { return field; } }

            public void MemberNotPropertyNorField() { }
        }

        private class AnotherSampleType
        {
            public string stringField;
        }

        [SetUp]
        public void SetUp()
        {
            _object = new SampleTypeWithFieldAndProperties();
        }

        [Test]
        public void Should_set_default_values_for_string_fields()
        {
            var command = new SetDefaultValuesCommand(_object, new DefaultValueGeneratorFactory());

            command.Execute();

            Assert.IsNotNull(_object.PropertyOnlyForTestingPurpose);
        }

        [Test]
        public void Should_set_default_values_for_complextype_fields()
        {
            var command = new SetDefaultValuesCommand(_object, new DefaultValueGeneratorFactory());

            command.Execute();

            Assert.IsNotNull(_object.sampleType);
        }
    }
}
