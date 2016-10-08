using Nosbor.FluentBuilder.Exceptions;
using Nosbor.FluentBuilder.Lib;
using Nosbor.FluentBuilder.Tests.Lib.SampleClasses;
using NUnit.Framework;

namespace Nosbor.FluentBuilder.Tests.Lib
{
    [TestFixture]
    public class FluentBuilderExceptionsTest
    {
        [Test]
        public void With_throws_exception_when_property_is_read_only()
        {
            Assert.Throws<FluentBuilderException>(() => FluentBuilder<SampleClass>.New()
                .With(newObject => newObject.ReadOnlyProperty, "dummy"));
        }

        [Test]
        public void With_throws_exception_when_property_is_not_informed()
        {
            Assert.Throws<FluentBuilderException>(() => FluentBuilder<SampleClass>.New()
                .With(justAnObjectWithNoPropInformed => justAnObjectWithNoPropInformed, null));
        }

        [Test]
        public void With_throws_exception_when_property_belongs_to_a_child_object()
        {
            Assert.Throws<FluentBuilderException>(() => FluentBuilder<SampleClass>.New()
                .With(newObject => newObject.AnotherComplexType.Name, "dummy"));
        }

        [Test]
        public void With_throws_exception_when_public_field_informed_instead_a_property()
        {
            Assert.Throws<FluentBuilderException>(() => FluentBuilder<SampleClass>.New()
               .With(newObject => newObject.PublicField, "dummy"));
        }

        [Test]
        public void WithFieldValue_throws_exception_when_member_was_not_found()
        {
            var newValue = new StandAloneComplexType();

            Assert.Throws<FluentBuilderException>(() => FluentBuilder<SampleClass>.New()
                .WithFieldValue(newValue));
        }

        [Test]
        public void AddingTo_throws_exception_when_underlying_field_for_collection_is_not_found()
        {
            Assert.Throws<FluentBuilderException>(() => FluentBuilder<SampleClass>.New()
                .AddingTo(newObject => newObject.CollectionWithFieldNotFollowingNameConvention, newElement: 5));
        }
    }
}
