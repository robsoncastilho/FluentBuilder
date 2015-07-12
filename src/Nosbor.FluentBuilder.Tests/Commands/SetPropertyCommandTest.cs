using Nosbor.FluentBuilder.Commands;
using Nosbor.FluentBuilder.Exceptions;
using NUnit.Framework;

namespace Nosbor.FluentBuilder.Tests.Commands
{
    [TestFixture]
    public class SetPropertyCommandTest
    {
        [Test]
        public void Should_set_a_writable_property()
        {
            var @object = new SampleTypeWithProperties();
            var propertyName = "WritableProperty";
            var newValue = 10;
            var command = new SetPropertyCommand(@object, propertyName, newValue);

            command.Execute();

            Assert.AreEqual(newValue, @object.WritableProperty);
        }

        [TestCase("ReadOnlyProperty", 10, Description = "When property is read-only")]
        [TestCase("WritableProperty", null, Description = "When value is null")]
        [TestCase("WritableProperty", "invalidType", Description = "When property type is different from value type")]
        [TestCase("NonExistentProperty", "dummyValue", Description = "When property was not found")]
        [TestCase(null, 10, Description = "When property name is null")]
        public void Should_not_create_invalid_command_when(string propertyName, object newValue)
        {
            var @object = new SampleTypeWithProperties();

            TestDelegate testAction = () => new SetPropertyCommand(@object, propertyName, newValue);

            Assert.Throws<FluentBuilderException>(testAction);
        }

        [Test]
        public void Should_not_create_invalid_command_when_destination_object_is_null()
        {
            SampleTypeWithProperties @object = null;

            TestDelegate testAction = () => new SetPropertyCommand(@object, "dummyProperty", "dummyValue");

            Assert.Throws<FluentBuilderException>(testAction);
        }
    }

    internal class SampleTypeWithProperties
    {
        public int WritableProperty { get; set; }
        public int ReadOnlyProperty { get { return 0; } }
    }
}
