using Nosbor.FluentBuilder.Internals.Queries;
using NUnit.Framework;

namespace Nosbor.FluentBuilder.Tests.Internals.Queries
{
    [TestFixture]
    public class GetMemberQueryTest
    {
        [TestCase("ReadOnlyPropertyWithUnderLyingField", Result = "readOnlyPropertyWithUnderLyingField")]
        [TestCase("ReadOnlyPropertyWithUnderLyingFieldWithUnderscore", Result = "_readOnlyPropertyWithUnderLyingFieldWithUnderscore")]
        [TestCase("NonExistentMember", Result = "")]
        public string Should_get_underlying_field_name_for_member(string memberName)
        {
            return GetMemberQuery.GetFieldNameFor(new SampleTypeWithFieldAndProperties(), memberName);
        }

        [Test]
        public void Should_get_property_name_for_expression()
        {
            var propertyName = GetMemberQuery.GetMemberNameFor<SampleTypeWithFieldAndProperties, int>(sampleType => sampleType.ReadOnlyPropertyWithUnderLyingField);

            Assert.AreEqual("ReadOnlyPropertyWithUnderLyingField", propertyName);
        }
    }

    internal class SampleTypeWithFieldAndProperties
    {
        private int readOnlyPropertyWithUnderLyingField;
        public int ReadOnlyPropertyWithUnderLyingField { get { return readOnlyPropertyWithUnderLyingField; } }

        private int _readOnlyPropertyWithUnderLyingFieldWithUnderscore;
        public int ReadOnlyPropertyWithUnderLyingFieldWithUnderscore { get { return _readOnlyPropertyWithUnderLyingFieldWithUnderscore; } }
    }
}
