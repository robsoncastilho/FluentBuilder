using Nosbor.FluentBuilder.Exceptions;
using Nosbor.FluentBuilder.Internals.Commands;
using NUnit.Framework;

namespace Nosbor.FluentBuilder.Tests.Internals.Commands
{
    [TestFixture]
    public class SetPropertyCommandTest
    {
        private SampleTypeWithProperties _object;

        [SetUp]
        public void SetUp()
        {
            _object = new SampleTypeWithProperties();
        }

        [Test]
        public void Should_set_a_writable_property()
        {
            var propertyName = "WritableProperty";
            var newValue = 10;
            var command = new SetPropertyCommand(_object, propertyName, newValue);

            command.Execute();

            Assert.AreEqual(newValue, _object.WritableProperty);
        }

        [Test]
        public void Should_set_a_writable_property_when_value_inherits_from_property_type()
        {
            var propertyName = "AbstractProperty";
            var newValue = new ConcreteSampleType();
            var command = new SetPropertyCommand(_object, propertyName, newValue);

            command.Execute();

            Assert.AreEqual(newValue, _object.AbstractProperty);
        }

        [TestCase("ReadOnlyProperty", 10, Description = "When property is read-only")]
        [TestCase("WritableProperty", null, Description = "When value is null")]
        [TestCase("WritableProperty", "invalidType", Description = "When property type is different from value type")]
        [TestCase("NonExistentProperty", "dummyValue", Description = "When property was not found")]
        [TestCase(null, 10, Description = "When property name is null")]
        public void Should_not_create_invalid_command_when(string propertyName, object newValue)
        {
            TestDelegate testAction = () => new SetPropertyCommand(_object, propertyName, newValue);

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

        public AbstractSampleType AbstractProperty { get; protected set; }
    }

    internal abstract class AbstractSampleType
    {
    }

    internal class ConcreteSampleType : AbstractSampleType
    {
    }
}
