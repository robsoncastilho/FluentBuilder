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
            const string newName = "Robson";
            var newAddresses = new List<string> { "20th Street", "1st Avenue" };

            var createdObject = FluentBuilder<SampleEntity>
                .New()
                .With(newObject => newObject.Name, newName)
                .With(newObject => newObject.AddressesWithSetter, newAddresses)
                .Build();

            Assert.AreEqual(newName, createdObject.Name);
            Assert.AreEqual(newAddresses, createdObject.AddressesWithSetter);
        }

        [Test]
        public void Should_build_object_using_implicit_conversion_when_not_calling_build_method()
        {
            const string newName = "Robson";

            SampleEntity createdObject = FluentBuilder<SampleEntity>.New().With(newObject => newObject.Name, newName);

            Assert.AreEqual(newName, createdObject.Name);
        }

        [Test]
        public void Should_build_object_with_no_public_parameterless_constructor()
        {
            const string newName = "Robson";

            var createdObject = FluentBuilder<SampleEntity>
                .New()
                .With(newObject => newObject.Name, newName)
                .Build();

            Assert.AreEqual(newName, createdObject.Name);
        }

        [Test]
        public void Should_build_object_setting_an_element_from_a_collection()
        {
            var createdObject = FluentBuilder<SampleEntity>
                .New()
                .AddingTo(newObject => newObject.Addresses, "20th Street")
                .AddingTo(newObject => newObject.Addresses, "1st Avenue")
                .Build();

            var expectedAddresses = new List<string> { "20th Street", "1st Avenue" };
            CollectionAssert.AreEqual(expectedAddresses, createdObject.Addresses);
        }

        [Test]
        public void Should_build_object_setting_default_values_for_members_required_from_constructor_but_not_specified_in_builder()
        {
            var createdObject = FluentBuilder<SampleEntityWithNoParameterlessCtor>.New().Build();

            Assert.AreEqual("Name", createdObject.Name);
            Assert.AreEqual("Name", createdObject.SampleEntity.Name);
        }

        [Test]
        public void Should_build_object_setting_no_value_for_member_of_same_type()
        {
            var createdObject = FluentBuilder<SampleEntityWithNoParameterlessCtor>.New().Build();

            Assert.IsNull(createdObject.SameTypeEntity);
        }

        [Test]
        public void Should_build_object_setting_dependency()
        {
            var concreteService = new SampleConcreteDependency();

            FluentBuilder<SampleServiceWithDependency>
                .New()
                .WithDependency<IDependency, SampleConcreteDependency>(concreteService)
                .Build();

            // TODO: assertion
        }

        [Test]
        public void Throws_exception_when_property_is_read_only()
        {
            var dummy = new List<string> { "20th Street", "1st Avenue" };

            Assert.Throws<FluentBuilderException>(() => FluentBuilder<SampleEntity>
                .New()
                .With(newObject => newObject.Addresses, dummy)
                .Build());
        }

        [Test]
        public void Throws_exception_when_property_is_not_informed()
        {
            var dummy = new SampleEntity();

            Assert.Throws<FluentBuilderException>(() => FluentBuilder<SampleEntity>
                .New()
                .With(justAnObjectWithNoPropInformed => justAnObjectWithNoPropInformed, dummy)
                .Build());
        }

        [Test]
        public void Throws_exception_when_underlying_field_for_collection_is_not_found()
        {
            int dummy = 0;

            Assert.Throws<FluentBuilderException>(() => FluentBuilder<SampleEntity>
                .New()
                .AddingTo(newObject => newObject.CollectionWithFieldNotFollowingNameConvention, dummy)
                .Build());
        }

        [TestCase(1000000, 30)]
        public void Should_build_a_large_number_of_objects_in_acceptable_time(int numberOfObjects, int expectedMaxTime)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            for (var i = 1; i <= numberOfObjects; i++)
            {
                FluentBuilder<SampleEntity>
                    .New()
                    .With(newObject => newObject.Name, "Name")
                    .AddingTo(newObject => newObject.Addresses, "Address")
                    .AddingTo(newObject => newObject.Addresses, "Address")
                    .AddingTo(newObject => newObject.Addresses, "Address")
                    .AddingTo(newObject => newObject.Addresses, "Address")
                    .AddingTo(newObject => newObject.Addresses, "Address")
                    .AddingTo(newObject => newObject.Addresses, "Address")
                    .AddingTo(newObject => newObject.Addresses, "Address")
                    .Build();
            }

            stopWatch.Stop();
            var elapsedTime = stopWatch.Elapsed;
            var formattedElapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}", elapsedTime.Hours, elapsedTime.Minutes, elapsedTime.Seconds, elapsedTime.Milliseconds / 10);

            Assert.LessOrEqual(elapsedTime.Seconds, expectedMaxTime);
        }
    }
}
