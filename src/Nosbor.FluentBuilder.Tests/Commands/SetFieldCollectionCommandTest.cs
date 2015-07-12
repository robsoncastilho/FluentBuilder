using Nosbor.FluentBuilder.Commands;
using Nosbor.FluentBuilder.Exceptions;
using NUnit.Framework;
using System.Collections.Generic;

namespace Nosbor.FluentBuilder.Tests.Commands
{
    [TestFixture]
    public class SetFieldCollectionCommandTest
    {
        [Test]
        public void Should_set_a_field_collection()
        {
            var collectionName = "collectionField";
            var @object = new SampleTypeWithCollectionField();
            var command = new SetFieldCollectionCommand(@object, collectionName);
            command.Add(1);
            command.Add(10);

            command.Execute();

            var newValues = new object[] { 1, 10 };
            Assert.AreEqual(newValues, @object.PropertyForTestingPurpose);
        }

        [TestCase(null, Description = "When collection name is null")]
        public void Should_not_create_invalid_command_when(string collectionName)
        {
            var @object = new SampleTypeWithCollectionField();

            TestDelegate testAction = () => new SetFieldCollectionCommand(@object, collectionName);

            Assert.Throws<FluentBuilderException>(testAction);
        }
    }

    internal class SampleTypeWithCollectionField
    {
        private IList<int> collectionField = new List<int>();

        public IList<int> PropertyForTestingPurpose { get { return collectionField; } }
    }
}
