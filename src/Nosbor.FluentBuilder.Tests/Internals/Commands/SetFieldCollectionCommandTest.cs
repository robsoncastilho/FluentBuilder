using Nosbor.FluentBuilder.Exceptions;
using Nosbor.FluentBuilder.Internals.Commands;
using NUnit.Framework;
using System.Collections.Generic;

namespace Nosbor.FluentBuilder.Tests.Internals.Commands
{
    [TestFixture]
    public class SetFieldCollectionCommandTest
    {
        private SampleTypeWithCollectionField _object;

        [SetUp]
        public void SetUp()
        {
            _object = new SampleTypeWithCollectionField();
        }

        [Test]
        public void Should_set_a_field_collection()
        {
            const string collectionName = "collectionField";
            var command = new SetFieldCollectionCommand(_object, collectionName);
            command.Add(1);
            command.Add(10);

            command.Execute();

            var expected = new object[] { 1, 10 };
            Assert.AreEqual(expected, _object.PropertyForTestingPurpose);
        }

        [TestCase(null, Description = "When collection name is null")]
        public void Should_not_create_invalid_command_when(string collectionName)
        {
            TestDelegate testAction = () => new SetFieldCollectionCommand(_object, collectionName);

            Assert.Throws<FluentBuilderException>(testAction);
        }
    }

    internal class SampleTypeWithCollectionField
    {
        private IList<int> collectionField = new List<int>();

        public IList<int> PropertyForTestingPurpose { get { return collectionField; } }
    }
}
