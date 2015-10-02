using Nosbor.FluentBuilder.Lib;
using Nosbor.FluentBuilder.Tests.Lib.SampleClasses;
using NUnit.Framework;
using System;
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
        public void Should_build_object_setting_field_just_passing_the_value()
        {
            var newValueForACorrespondingField = new OneMoreComplexType();

            var createdObject = FluentBuilder<ComplexType>
                .New()
                .WithFieldValue(newValueForACorrespondingField)
                .Build();

            Assert.AreEqual(newValueForACorrespondingField, createdObject.oneMoreComplexType);
        }

        [Test]
        public void Should_build_object_setting_collection_element_by_element()
        {
            var anObject = new AnotherComplexType("Robson");
            var otherObject = new AnotherComplexType("Nosbor");

            var createdObject = FluentBuilder<ComplexType>
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

            var createdObject = FluentBuilder<ComplexType>
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

            var createdObject = FluentBuilder<ComplexType>
                .New()
                .With(newObject => newObject.CollectionWithFieldFollowingNameConvention, anObject, otherObject)
                .Build();

            var expected = new List<AnotherComplexType> { anObject, otherObject };
            CollectionAssert.AreEqual(expected, createdObject.CollectionWithFieldFollowingNameConvention);
        }

        [Test]
        public void Should_build_object_using_implicit_conversion_when_not_calling_build_method()
        {
            const string newValue = "Robson";

            ComplexType createdObject = FluentBuilder<ComplexType>.New().With(newObject => newObject.PropertyWithSetter, newValue);

            Assert.AreEqual(newValue, createdObject.PropertyWithSetter);
        }

        [Test]
        public void Should_build_object_setting_default_values()
        {
            var createdObject = FluentBuilder<ComplexType>.New().EnableDefaultValues().Build();

            Assert.IsNotNull(createdObject.PropertyOnlyForTestingPurpose);
            Assert.IsNotNull(createdObject.PublicField);
            Assert.IsNotNull(createdObject.AnotherComplexType);
            Assert.IsNotNull(createdObject.CollectionWithFieldFollowingNameConvention);
            Assert.IsNotNull(createdObject.PropertyWithSetter);
            Assert.IsNotNull(createdObject.ReadOnlyPropertyWithBackingField);
        }

        [Test]
        public void Should_build_object_setting_no_default_value_for_member_of_same_type()
        {
            var createdObject = FluentBuilder<ComplexType>.New().Build();

            Assert.IsNull(createdObject.SameTypeEntityIsNotInitialized);
        }

        [Test]
        public void Should_build_object_setting_no_default_value_for_abstract_member()
        {
            var createdObject = FluentBuilder<ComplexType>.New().Build();

            Assert.IsNull(createdObject.AbstractComplexType);
        }

        [Test]
        public void Should_build_object_within_a_list()
        {
            var collectionWithOneComplexType = FluentBuilder<ComplexType>
                .New()
                .With(newObject => newObject.PropertyWithSetter, "Some value")
                .AsList();

            Assert.AreEqual(1, collectionWithOneComplexType.Count());
            Assert.AreEqual("Some value", collectionWithOneComplexType.First().PropertyWithSetter);
        }

        [Test]
        public void Should_build_many_objects()
        {
            const int amountOfObjects = 3;

            var collection = FluentBuilder<ComplexType>.Many(amountOfObjects);

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

        [Test, Ignore]
        public void Should_build_object_setting_association_to_an_association()
        {


        }

        [Test, Ignore]
        public void Should_build_object_creating_a_stub()
        {
            const int returnValue = 10;
            Func<IDependency, int> setupAction = dependency => dependency.GetSomething();

            //var builder = FluentBuilder<SampleServiceWithDependency>
            //    .New()
            //    .WithStub<IDependency, int>(setupAction, returnValue);
        }

        [Test, Ignore]
        public void Should_build_object_creating_a_stub_and_configuring_more_than_one_method_for_the_same_stub()
        {
            const int returnValue = 10;
            const string otherReturnValue = "value";

            Func<IDependency, int> setupAction = dependency => dependency.GetSomething();
            Func<IDependency, string> otherSetupAction = dependency => dependency.GetOtherThing();

            //var builder = FluentBuilder<SampleServiceWithDependency>
            //    .New()
            //    .WithStub<IDependency>()
            //    .Setting(setupAction, returnValue)
            //    .Setting(otherSetupAction, otherReturnValue);
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
    }
}
