using FluentBuilderLib.Tests.SampleClasses;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace FluentBuilderLib.Tests
{
    [TestFixture]
    public class FluentBuilderTests
    {
        [Test]
        public void Should_build_object_setting_writable_properties()
        {
            const string newName = "Robson";
            var newAddresses = new List<string> { "20th Street", "1st Avenue" };

            var createdObject = FluentBuilder<ClassWithWritableProperties>
                .New()
                .With(newObject => newObject.Name, newName)
                .With(newObject => newObject.Addresses, newAddresses)
                .Build();

            Assert.AreEqual(newName, createdObject.Name);
            Assert.AreEqual(newAddresses, createdObject.Addresses);
        }

        [Test]
        public void Should_build_object_setting_underlying_fields()
        {
            var newAddresses = new List<string> { "20th Street", "1st Avenue" };

            var createdObject = FluentBuilder<ClassWithReadOnlyProperty>
                .New()
                .With(newObject => newObject.Addresses, newAddresses)
                .Build();

            Assert.AreEqual(newAddresses, createdObject.Addresses);
        }

        [Test]
        public void Should_build_object_using_implicit_conversion_when_not_calling_build_method()
        {
            const string newName = "Robson";

            ClassWithWritableProperties createdObject = FluentBuilder<ClassWithWritableProperties>.New().With(newObject => newObject.Name, newName);

            Assert.AreEqual(newName, createdObject.Name);
        }

        [Test]
        public void Should_throw_exception_when_underlying_field_not_found()
        {
            Assert.Throws<Exception>(() => FluentBuilder<ClassWithReadOnlyProperty>
                .New()
                .With(newObject => newObject.BadAddresses, new List<string> { "20th Street", "1st Avenue" })
                .Build());
        }

        [Test]
        public void Should_build_object_with_no_public_parameterless_constructor()
        {
            const string newName = "Robson";

            var createdObject = FluentBuilder<ClassWithoutParameterlessCtor>
                .New()
                .With(newObject => newObject.Name, newName)
                .Build();

            Assert.AreEqual(newName, createdObject.Name);
        }

        [Test, Ignore]
        public void Should_build_object_setting_an_element_from_a_collection()
        {
            var createdObject = FluentBuilder<ClassWithReadOnlyProperty>
                .New()
                .Adding(newObject => newObject.Addresses, "20th Street")
                .Adding(newObject => newObject.Addresses, "1st Avenue")
                .Build();

            var expectedAddresses = new List<string> { "20th Street", "1st Avenue" };
            CollectionAssert.AreEqual(expectedAddresses, createdObject.Addresses);
        }

        [Test]
        public void Should_build_object_setting_services()
        {
            Assert.Ignore();
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
