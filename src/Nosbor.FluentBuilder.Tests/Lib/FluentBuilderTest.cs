using Nosbor.FluentBuilder.Lib;
using Nosbor.FluentBuilder.Tests.Lib.SampleClasses;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Nosbor.FluentBuilder.Tests.Lib
{
    [TestFixture]
    public class FluentBuilderTest
    {
        [Test]
        public void Should_build_object_setting_writable_properties()
        {
            const string newValue = "Robson";
            var newListValues = new List<string> { "20th Street", "1st Avenue" };
            var newArrayValues = new[] { 1, 2 };

            var createdObject = FluentBuilder<SampleClass>
                .New()
                .With(newObject => newObject.PropertyWithSetter, newValue)
                .With(newObject => newObject.CollectionWithSetter, newListValues)
                .With(newObject => newObject.ArrayWithSetter, newArrayValues)
                .Build();

            Assert.AreEqual(newValue, createdObject.PropertyWithSetter);
            Assert.AreEqual(newListValues, createdObject.CollectionWithSetter);
            Assert.AreEqual(newArrayValues, createdObject.ArrayWithSetter);
        }

        [Test]
        public void Should_build_object_setting_field_using_naming_convention()
        {
            var newValueForComplexType = new OneMoreComplexType();
            const string newValueForString = "Robson";

            var createdObject = FluentBuilder<SampleClass>
                .New()
                .WithFieldValue(newValueForComplexType)
                .WithFieldValue(newValueForString)
                .Build();

            Assert.AreEqual(newValueForComplexType, createdObject.OneMoreComplexType);
            Assert.AreEqual(newValueForString, createdObject.String);
        }

        [Test, Ignore("Issue #32")]
        public void Should_build_object_setting_get_only_property()
        {
            const string newValue = "Robson";

            var createdObject = FluentBuilder<SampleClass>
                .New()
                .With(newObject => newObject.GetOnlyProperty, newValue)
                .Build();

            Assert.AreEqual(newValue, createdObject.GetOnlyProperty);
        }

        [Test]
        public void Should_build_object_setting_collection_element_by_element()
        {
            var anObject = new AnotherComplexType("Robson");
            var otherObject = new AnotherComplexType("Nosbor");

            var createdObject = FluentBuilder<SampleClass>
                .New()
                .AddingTo(newObject => newObject.CollectionWithFieldFollowingNameConvention, anObject)
                .AddingTo(newObject => newObject.CollectionWithFieldFollowingNameConvention, otherObject)
                .Build();

            var expected = new List<AnotherComplexType> { anObject, otherObject };
            CollectionAssert.AreEqual(expected, createdObject.CollectionWithFieldFollowingNameConvention);
        }

        [Test]
        public void Should_build_object_setting_different_collections_element_by_element()
        {
            var anObject = new AnotherComplexType("Robson");
            var integerElement = 1000;

            var createdObject = FluentBuilder<SampleClass>
                .New()
                .AddingTo(newObject => newObject.CollectionWithFieldFollowingNameConvention, anObject)
                .AddingTo(newObject => newObject.AnotherCollection, integerElement)
                .AddingTo(newObject => newObject.AnotherCollection, integerElement)
                .Build();

            var expected = new List<AnotherComplexType> { anObject };
            var expectedIntCollection = new List<int> { integerElement, integerElement };
            CollectionAssert.AreEqual(expected, createdObject.CollectionWithFieldFollowingNameConvention);
            CollectionAssert.AreEqual(expectedIntCollection, createdObject.AnotherCollection);
        }

        [Test]
        public void Should_build_object_setting_collection_all_elements_at_once()
        {
            var anObject = new AnotherComplexType("Robson");
            var otherObject = new AnotherComplexType("Nosbor");

            var createdObject = FluentBuilder<SampleClass>
                .New()
                .WithCollection(newObject => newObject.CollectionWithFieldFollowingNameConvention, anObject, otherObject)
                .Build();

            var expected = new List<AnotherComplexType> { anObject, otherObject };
            CollectionAssert.AreEqual(expected, createdObject.CollectionWithFieldFollowingNameConvention);
        }

        [Test]
        public void Should_build_object_using_implicit_conversion_when_not_calling_build_method()
        {
            const string newValue = "Robson";

            SampleClass createdObject = FluentBuilder<SampleClass>.New().With(newObject => newObject.PropertyWithSetter, newValue);

            Assert.AreEqual(newValue, createdObject.PropertyWithSetter);
        }

        [Test]
        public void Should_build_object_setting_default_values()
        {
            var createdObject = FluentBuilder<SampleClass>.New().Build();

            Assert.IsNotNull(createdObject.PublicField);
            Assert.IsNotNull(createdObject.CollectionWithFieldFollowingNameConvention);
            Assert.IsNotNull(createdObject.PropertyWithSetter);
            Assert.IsNotNull(createdObject.ReadOnlyProperty);
        }

        [Test]
        public void Should_build_object_setting_no_default_value_for_member_of_same_type()
        {
            var createdObject = FluentBuilder<SampleClass>.New().Build();

            Assert.IsNull(createdObject.SameTypeEntityIsNotInitialized);
        }

        [Test]
        public void Should_build_object_setting_no_default_value_for_abstract_member()
        {
            var createdObject = FluentBuilder<SampleClass>.New().Build();

            Assert.IsNull(createdObject.AbstractComplexType);
        }

        [Test]
        public void Should_build_object_within_a_list()
        {
            var collectionWithOneComplexType = FluentBuilder<SampleClass>
                .New()
                .With(newObject => newObject.PropertyWithSetter, "Some value")
                .AsList();

            Assert.AreEqual(1, collectionWithOneComplexType.Count());
            Assert.AreEqual("Some value", collectionWithOneComplexType.First().PropertyWithSetter);
        }

        [Test]
        public void Should_build_a_collection_of_objects()
        {
            const int amountOfObjects = 3;

            var collection = FluentBuilder<SampleClass>.Many(amountOfObjects);

            Assert.AreEqual(amountOfObjects, collection.Count());
        }

        [Test]
        public void Should_build_object_setting_dependency()
        {
            var concreteService = new SampleConcreteDependency();

            var createdObject = FluentBuilder<SampleServiceWithDependency>
                .New()
                .WithDependency<IDependency, SampleConcreteDependency>(concreteService)
                .Build();

            Assert.AreEqual(createdObject.PropertyOnlyForTestingPurpose, concreteService);
        }


        [TestCase(100000, 30), Ignore("I don`t know")]
        public void Should_build_large_number_of_objects_in_acceptable_time(int numberOfObjects, int expectedMaxTime)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            for (var i = 1; i <= numberOfObjects; i++)
            {
                FluentBuilder<SampleClass>
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
    }
}
