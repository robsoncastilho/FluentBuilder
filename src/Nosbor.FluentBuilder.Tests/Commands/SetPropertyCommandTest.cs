using Nosbor.FluentBuilder.Commands;
using Nosbor.FluentBuilder.Exceptions;
using NUnit.Framework;

namespace Nosbor.FluentBuilder.Tests.Commands
{
    [TestFixture]
    public class SetPropertyCommandTest
    {
        [Test]
        public void Should_set_writable_property()
        {
            var @object = new SampleTypeWithProperties();
            var propertyName = "WritableProperty";
            var newValue = 10;

            var cmd = new SetPropertyCommand(@object, propertyName, newValue);

            cmd.Execute();

            Assert.AreEqual(newValue, @object.WritableProperty);
        }

        [TestCase("ReadOnlyProperty", 10, Description = "When property is read-only")]
        [TestCase("WritableProperty", null, Description = "When value is null")]
        [TestCase("WritableProperty", "string", Description = "When property type is different from value type")]
        [TestCase(null, 10, Description = "When property name is null")]
        public void Should_not_create_invalid_command(string propertyName, object newValue)
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

        [Test]
        public void Should_not_create_invalid_command_when_property_was_not_found()
        {
            var @object = new SampleTypeWithProperties();

            TestDelegate testAction = () => new SetPropertyCommand(@object, "NonExistentProperty", 10);

            Assert.Throws<FluentBuilderException>(testAction);
        }
    }

    internal class SampleTypeWithProperties
    {
        public int WritableProperty { get; set; }
        public int ReadOnlyProperty { get { return 0; } }
    }
}
