using Nosbor.FluentBuilder.Commands;
using Nosbor.FluentBuilder.Exceptions;
using NUnit.Framework;
namespace Nosbor.FluentBuilder.Tests.Commands
{
    [TestFixture]
    public class SetFieldCommandTest
    {
        [Test]
        public void Should_set_a_public_field()
        {
            var @object = new SampleTypeWithFields();
            var fieldName = "field";
            var newValue = 1;
            var command = new SetFieldCommand(@object, fieldName, newValue);

            command.Execute();

            Assert.AreEqual(newValue, @object.field);
        }

        [Test]
        public void Should_set_a_private_field()
        {
            var @object = new SampleTypeWithFields();
            var fieldName = "privateField";
            var newValue = 1;
            var command = new SetFieldCommand(@object, fieldName, newValue);

            command.Execute();

            Assert.AreEqual(newValue, @object.PropertyOnlyForTestingPurpose);
        }

        [TestCase("field", null, Description = "When value is null")]
        [TestCase("field", "invalidType", Description = "When field type is different from value type")]
        [TestCase("NonExistentField", "dummyValue", Description = "When field was not found")]
        [TestCase(null, 10, Description = "When field name is null")]
        public void Should_not_create_invalid_command_when(string fieldName, object newValue)
        {
            var @object = new SampleTypeWithFields();

            TestDelegate testAction = () => new SetFieldCommand(@object, fieldName, newValue);

            Assert.Throws<FluentBuilderException>(testAction);
        }

        [Test]
        public void Should_not_create_invalid_command_when_destination_object_is_null()
        {
            SampleTypeWithFields @object = null;

            TestDelegate testAction = () => new SetFieldCommand(@object, "dummyField", "dummyValue");

            Assert.Throws<FluentBuilderException>(testAction);
        }
    }

    internal class SampleTypeWithFields
    {
        public int field;
        private int privateField;

        public int PropertyOnlyForTestingPurpose { get { return privateField; } }
    }
}
