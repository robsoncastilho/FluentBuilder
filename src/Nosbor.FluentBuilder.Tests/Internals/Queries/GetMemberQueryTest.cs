using Nosbor.FluentBuilder.Internals.Queries;
using NUnit.Framework;

namespace Nosbor.FluentBuilder.Tests.Internals.Queries
{
    [TestFixture]
    public class GetMemberQueryTest
    {
        [TestCase("ReadOnlyPropertyWithUnderLyingField", Result = "readOnlyPropertyWithUnderLyingField")]
        [TestCase("BaseReadOnlyPropertyWithUnderLyingField", Result = "baseReadOnlyPropertyWithUnderLyingField")]
        [TestCase("ReadOnlyPropertyWithUnderLyingFieldWithUnderscore", Result = "_readOnlyPropertyWithUnderLyingFieldWithUnderscore")]
        [TestCase("BaseReadOnlyPropertyWithUnderLyingFieldWithUnderscore", Result = "_baseReadOnlyPropertyWithUnderLyingFieldWithUnderscore")]
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

    internal class SampleBaseTypeWithFieldAndProperties
    {
        private int baseReadOnlyPropertyWithUnderLyingField;
        public int BaseReadOnlyPropertyWithUnderLyingField { get { return baseReadOnlyPropertyWithUnderLyingField; } }

        private int _baseReadOnlyPropertyWithUnderLyingFieldWithUnderscore;
        public int BaseReadOnlyPropertyWithUnderLyingFieldWithUnderscore { get { return _baseReadOnlyPropertyWithUnderLyingFieldWithUnderscore; } }
    }

    internal class SampleTypeWithFieldAndProperties : SampleBaseTypeWithFieldAndProperties
    {
        private int readOnlyPropertyWithUnderLyingField;
        public int ReadOnlyPropertyWithUnderLyingField { get { return readOnlyPropertyWithUnderLyingField; } }

        private int _readOnlyPropertyWithUnderLyingFieldWithUnderscore;
        public int ReadOnlyPropertyWithUnderLyingFieldWithUnderscore { get { return _readOnlyPropertyWithUnderLyingFieldWithUnderscore; } }
    }
}
