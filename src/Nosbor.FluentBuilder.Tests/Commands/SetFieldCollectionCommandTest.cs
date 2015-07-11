using Nosbor.FluentBuilder.Commands;
using NUnit.Framework;
using System.Collections.Generic;

namespace Nosbor.FluentBuilder.Tests.Commands
{
    [TestFixture]
    public class SetFieldCollectionCommandTest
    {
        [Test]
        public void Should_set_field_collection()
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
    }

    public class SampleTypeWithCollectionField
    {
        private IList<int> collectionField = new List<int>();

        public IList<int> PropertyForTestingPurpose { get { return collectionField; } }
    }
}
