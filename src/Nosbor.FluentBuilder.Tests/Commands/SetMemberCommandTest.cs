using Nosbor.FluentBuilder.Commands;
using Nosbor.FluentBuilder.Exceptions;
using NUnit.Framework;

namespace Nosbor.FluentBuilder.Tests.Commands
{
    [TestFixture]
    public class SetMemberCommandTest
    {
        [Test]
        public void Should_set_a_member_when_it_is_a_field()
        {
            var @object = new SampleTypeWithFieldAndProperties();
            var memberName = "field";
            var newValue = 1;
            var command = new SetMemberCommand(@object, memberName, newValue);

            command.Execute();

            Assert.AreEqual(newValue, @object.PropertyOnlyForTestingPurpose);
        }

        [Test]
        public void Should_set_a_member_when_it_is_a_writable_property()
        {
            var @object = new SampleTypeWithFieldAndProperties();
            var memberName = "WritableProperty";
            var newValue = 1;
            var command = new SetMemberCommand(@object, memberName, newValue);

            command.Execute();

            Assert.AreEqual(newValue, @object.WritableProperty);
        }

        [Test]
        public void Should_set_a_member_when_it_is_a_readonly_property_finding_the_underlying_field()
        {
            var @object = new SampleTypeWithFieldAndProperties();
            var memberName = "ReadOnlyPropertyWithUnderLyingField";
            var newValue = 1;
            var command = new SetMemberCommand(@object, memberName, newValue);

            command.Execute();

            Assert.AreEqual(newValue, @object.ReadOnlyPropertyWithUnderLyingField);
        }

        [Test]
        public void Execute_throws_exception_when_member_type_is_different_from_the_type_of_the_new_value()
        {
            var @object = new SampleTypeWithFieldAndProperties();
            var memberName = "WritableProperty";
            var newValueOfInvalidType = "string";
            var command = new SetMemberCommand(@object, memberName, newValueOfInvalidType);

            TestDelegate testAction = () => command.Execute();

            Assert.Throws<FluentBuilderException>(testAction);
        }

        [TestCase("WritableProperty", null, Description = "When value is null")]
        [TestCase("NonExistentProperty", "dummyValue", Description = "When member was not found")]
        [TestCase(null, 10, Description = "When member name is null")]
        public void Should_not_create_invalid_command_when(string memberName, object newValue)
        {
            var @object = new SampleTypeWithFieldAndProperties();

            TestDelegate testAction = () => new SetMemberCommand(@object, memberName, newValue);

            Assert.Throws<FluentBuilderException>(testAction);
        }

        [Test]
        public void Should_not_create_invalid_command_when_destination_object_is_null()
        {
            SampleTypeWithFieldAndProperties @object = null;

            TestDelegate testAction = () => new SetMemberCommand(@object, "dummyProperty", "dummyValue");

            Assert.Throws<FluentBuilderException>(testAction);
        }
    }

    internal class SampleTypeWithFieldAndProperties
    {
        private int field;
        public int WritableProperty { get; private set; }

        private int _readOnlyPropertyWithUnderLyingField;
        public int ReadOnlyPropertyWithUnderLyingField { get { return _readOnlyPropertyWithUnderLyingField; } }

        public int PropertyOnlyForTestingPurpose { get { return field; } }
    }
}
