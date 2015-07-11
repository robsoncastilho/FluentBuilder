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
        [TestCase("field", "string", Description = "When field type is different from value type")]
        [TestCase(null, 10, Description = "When field name is null")]
        public void Should_not_create_invalid_command(string fieldName, object newValue)
        {
            var @object = new SampleTypeWithFields();

            TestDelegate testAction = () => new SetFieldCommand(@object, fieldName, newValue);

            Assert.Throws<FluentBuilderException>(testAction);
        }
    }

    public class SampleTypeWithFields
    {
        public int field;
        private int privateField;

        public int PropertyOnlyForTestingPurpose { get { return privateField; } }
    }
}
