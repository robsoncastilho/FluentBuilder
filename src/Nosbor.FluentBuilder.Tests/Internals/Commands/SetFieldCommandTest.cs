using Nosbor.FluentBuilder.Exceptions;
using Nosbor.FluentBuilder.Internals.Commands;
using NUnit.Framework;

namespace Nosbor.FluentBuilder.Tests.Internals.Commands
{
    [TestFixture]
    public class SetFieldCommandTest
    {
        private SampleTypeWithFields _object;

        [SetUp]
        public void SetUp()
        {
            _object = new SampleTypeWithFields();
        }

        [Test]
        public void Should_set_a_public_field()
        {
            const string fieldName = "field";
            const int newValue = 1;
            var command = new SetFieldCommand(_object, fieldName, newValue);

            command.Execute();

            Assert.AreEqual(newValue, _object.field);
        }

        [Test]
        public void Should_set_a_private_field()
        {
            const string fieldName = "privateField";
            const int newValue = 1;
            var command = new SetFieldCommand(_object, fieldName, newValue);

            command.Execute();

            Assert.AreEqual(newValue, _object.PropertyOnlyForTestingPurpose);
        }

        [Test]
        public void Should_set_a_field_when_value_inherits_from_field_type()
        {
            const string fieldName = "abstractField";
            var newValue = new ConcreteSampleType();
            var command = new SetFieldCommand(_object, fieldName, newValue);

            command.Execute();

            Assert.AreEqual(newValue, _object.abstractField);
        }

        [TestCase(null, 10, Description = "When field name is null")]
        [TestCase("field", null, Description = "When value is null")]
        [TestCase("NonExistentField", "dummyValue", Description = "When field was not found")]
        [TestCase("field", "invalidType", Description = "When field type is different from value type")]
        public void Should_not_create_invalid_set_field_command_when(string fieldName, object newValue)
        {
            TestDelegate testAction = () => new SetFieldCommand(_object, fieldName, newValue);

            Assert.Throws<FluentBuilderException>(testAction);
        }

        [Test]
        public void Should_not_create_invalid_set_field_command_when_destination_object_is_null()
        {
            SampleTypeWithFields @object = null;

            TestDelegate testAction = () => new SetFieldCommand(@object, "dummyField", "dummyValue");

            Assert.Throws<FluentBuilderException>(testAction);
        }
    }

    internal class SampleTypeWithFields
    {
        public int field;
        private int _privateField;

        public AbstractSampleType abstractField;

        public int PropertyOnlyForTestingPurpose { get { return _privateField; } }
    }
}
