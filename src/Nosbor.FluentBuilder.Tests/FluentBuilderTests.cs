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
        public void Should_build_object_setting_dependency()
        {
            var concreteService = new SampleConcreteDependency();

            var createdObject = FluentBuilder<SampleServiceWithDependency>
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

        /// <summary>
        /// Not a "real" test. Just a performance test of building thousands of objects
        /// </summary>
        [Test]
        public void Tests_builder_performance()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var numberOfObjects = 100000;
            for (var i = 1; i <= numberOfObjects; i++)
            {
                var obj = FluentBuilder<SampleEntity>
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

            Assert.Pass(string.Format("Finish building {0:n0} objects in {1}", numberOfObjects, formattedElapsedTime));
        }

        [Test]
        public void Should_build_object_setting_dummies_for_all_service_interfaces_not_specified()
        {
            Assert.Ignore();
        }

        [Test]
        public void Should_build_object_setting_default_values_to_properties_not_specified()
        {
            Assert.Ignore();
        }
    }
}
