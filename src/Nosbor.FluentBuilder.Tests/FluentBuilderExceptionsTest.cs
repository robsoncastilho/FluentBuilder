using Nosbor.FluentBuilder.Exceptions;
using Nosbor.FluentBuilder.Lib;
using Nosbor.FluentBuilder.Tests.SampleClasses;
using NUnit.Framework;

namespace Nosbor.FluentBuilder.Tests
{
    [TestFixture]
    public class FluentBuilderExceptionsTest
    {
        [Test]
        public void With_throws_exception_when_property_is_read_only()
        {
            const string dummy = "";

            Assert.Throws<FluentBuilderException>(() => FluentBuilder<ComplexType>
                .New()
                .With(newObject => newObject.ReadOnlyPropertyWithBackingField, dummy));
        }

        [Test]
        public void With_throws_exception_when_property_is_not_informed()
        {
            var dummy = (ComplexType)null;

            Assert.Throws<FluentBuilderException>(() => FluentBuilder<ComplexType>
                .New()
                .With(justAnObjectWithNoPropInformed => justAnObjectWithNoPropInformed, dummy));
        }

        [Test]
        public void With_throws_exception_when_trying_to_set_property_of_child_object()
        {
            Assert.Throws<FluentBuilderException>(() => FluentBuilder<ComplexType>
                .New()
                .With(newObject => newObject.AnotherComplexType.Name, "dummy"));
        }

        [Test]
        public void WithValue_throws_exception_when_member_was_not_found()
        {
            var newValue = new StandAloneComplexType();

            Assert.Throws<FluentBuilderException>(() => FluentBuilder<ComplexType>
                .New()
                .WithValue(newValue));
        }

        [Test]
        public void AddingTo_throws_exception_when_underlying_field_for_collection_is_not_found()
        {
            const int dummy = 0;

            Assert.Throws<FluentBuilderException>(() => FluentBuilder<ComplexType>
                .New()
                .AddingTo(newObject => newObject.CollectionWithFieldNotFollowingNameConvention, dummy));
        }
    }
}
