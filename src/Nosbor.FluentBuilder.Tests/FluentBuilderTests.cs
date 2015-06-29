using Nosbor.FluentBuilder.Exceptions;
using Nosbor.FluentBuilder.Tests.SampleClasses;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;

namespace Nosbor.FluentBuilder.Tests
{
    [TestFixture]
    public class FluentBuilderTests
    {
        [Test]
        public void Should_build_object_setting_writable_properties()
        {
            const string newValue = "Robson";
            var newElements = new List<string> { "20th Street", "1st Avenue" };

            var createdObject = FluentBuilder<ComplexType>
                .New()
                .With(newObject => newObject.PropertyWithSetter, newValue)
                .With(newObject => newObject.CollectionWithSetter, newElements)
                .Build();

            Assert.AreEqual(newValue, createdObject.PropertyWithSetter);
            Assert.AreEqual(newElements, createdObject.CollectionWithSetter);
        }

        [Test]
        public void Should_build_object_setting_an_element_from_a_collection()
        {
            var anObject = new AnotherComplexType("Robson");
            var otherObject = new AnotherComplexType("Nosbor");

            var createdObject = FluentBuilder<ComplexType>
                .New()
                .AddingTo(newObject => newObject.CollectionWithFieldFollowingNameConvention, anObject)
                .AddingTo(newObject => newObject.CollectionWithFieldFollowingNameConvention, otherObject)
                .Build();

            var expectedAddresses = new List<AnotherComplexType> { anObject, otherObject };
            CollectionAssert.AreEqual(expectedAddresses, createdObject.CollectionWithFieldFollowingNameConvention);
        }

        [Test]
        public void Should_build_object_using_implicit_conversion_when_not_calling_build_method()
        {
            const string newValue = "Robson";

            ComplexType createdObject = FluentBuilder<ComplexType>.New().With(newObject => newObject.PropertyWithSetter, newValue);

            Assert.AreEqual(newValue, createdObject.PropertyWithSetter);
        }

        [Test]
        public void Should_build_object_setting_default_values_for_members_required_from_constructor()
        {
            var createdObject = FluentBuilder<ComplexType>.New().Build();

            Assert.AreEqual("publicField", createdObject.PublicField);
            Assert.AreEqual("propertyWithSetter", createdObject.PropertyWithSetter);
            Assert.AreEqual("propertyWithBackingField", createdObject.PropertyWithBackingField);
            Assert.AreEqual("name", createdObject.AnotherComplexType.Name);
            Assert.AreEqual("name", createdObject.AnotherComplexTypeInsensitiveCaseTest.Name);

            // TODO: assert private field
        }

        [Test]
        public void Should_build_object_setting_no_default_value_for_member_of_same_type()
        {
            var createdObject = FluentBuilder<ComplexType>.New().Build();

            Assert.IsNull(createdObject.SameTypeEntityIsNotInitialized);
        }

        [Test]
        public void Should_build_object_setting_dependency()
        {
            var concreteService = new SampleConcreteDependency();

            FluentBuilder<SampleServiceWithDependency>
                .New()
                .WithDependency<IDependency, SampleConcreteDependency>(concreteService)
                .Build();

            // TODO: assert private field
        }

        [Test]
        public void Should_build_many_objects()
        {
            const int amountOfObjects = 3;

            var collection = FluentBuilder<ComplexType>.Many(amountOfObjects);

            CollectionAssert.AllItemsAreInstancesOfType(collection, typeof(ComplexType));
            Assert.AreEqual(amountOfObjects, collection.Count);
        }

        [Test]
        public void Throws_exception_when_property_is_read_only()
        {
            const string dummy = "";

            Assert.Throws<FluentBuilderException>(() => FluentBuilder<ComplexType>
                .New()
                .With(newObject => newObject.PropertyWithBackingField, dummy)
                .Build());
        }

        [Test]
        public void Throws_exception_when_property_is_not_informed()
        {
            var dummy = (ComplexType)null;

            Assert.Throws<FluentBuilderException>(() => FluentBuilder<ComplexType>
                .New()
                .With(justAnObjectWithNoPropInformed => justAnObjectWithNoPropInformed, dummy)
                .Build());
        }

        [Test]
        public void Throws_exception_when_underlying_field_for_collection_is_not_found()
        {
            const int dummy = 0;

            Assert.Throws<FluentBuilderException>(() => FluentBuilder<ComplexType>
                .New()
                .AddingTo(newObject => newObject.CollectionWithFieldNotFollowingNameConvention, dummy)
                .Build());
        }

        [Test]
        public void Throws_exception_when_trying_to_set_property_of_child_object()
        {
            Assert.Throws<FluentBuilderException>(() => FluentBuilder<ComplexType>
                .New()
                .With(newObject => newObject.AnotherComplexType.Name, "dummy")
                .Build());
        }

        [TestCase(100000, 30), Ignore]
        public void Should_build_large_number_of_objects_in_acceptable_time(int numberOfObjects, int expectedMaxTime)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            for (var i = 1; i <= numberOfObjects; i++)
            {
                FluentBuilder<ComplexType>
                    .New()
                    .With(newObject => newObject.PropertyWithSetter, "Name")
                    .AddingTo(newObject => newObject.CollectionWithFieldFollowingNameConvention, new AnotherComplexType(i.ToString()))
                    .AddingTo(newObject => newObject.CollectionWithFieldFollowingNameConvention, new AnotherComplexType(i.ToString()))
                    .AddingTo(newObject => newObject.CollectionWithFieldFollowingNameConvention, new AnotherComplexType(i.ToString()))
                    .AddingTo(newObject => newObject.CollectionWithFieldFollowingNameConvention, new AnotherComplexType(i.ToString()))
                    .AddingTo(newObject => newObject.CollectionWithFieldFollowingNameConvention, new AnotherComplexType(i.ToString()))
                    .AddingTo(newObject => newObject.CollectionWithFieldFollowingNameConvention, new AnotherComplexType(i.ToString()))
                    .AddingTo(newObject => newObject.CollectionWithFieldFollowingNameConvention, new AnotherComplexType(i.ToString()))
                    .Build();
            }
            stopWatch.Stop();

            Assert.LessOrEqual(stopWatch.Elapsed.Seconds, expectedMaxTime);
        }

        [Test, Ignore]
        public void Should_build_object_setting_public_fields()
        {

        }
    }
}