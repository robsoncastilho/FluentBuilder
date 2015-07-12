using System.Collections.Generic;

namespace Nosbor.FluentBuilder.Tests.SampleClasses
{
    internal class ComplexType
    {
        private string privateField;

        public string PublicField;

        public string PropertyWithSetter { get; private set; }

        public List<string> CollectionWithSetter { get; protected set; }

        public OneMoreComplexType oneMoreComplexType;

        public AnotherComplexType AnotherComplexType { get; private set; }
        public AnotherComplexType AnotherComplexTypeInsensitiveCaseTest { get; private set; }
        public AnotherComplexType OneMoreComplexType { get; protected set; }

        public AbstractComplexType AbstractComplexType { get; private set; }

        public ComplexType SameTypeEntityIsNotInitialized { get; private set; }

        private string _readOnlyPropertyWithBackingField;
        public string ReadOnlyPropertyWithBackingField { get { return _readOnlyPropertyWithBackingField; } }

        private readonly IList<AnotherComplexType> _collectionWithFieldFollowingNameConvention = new List<AnotherComplexType>();
        public IEnumerable<AnotherComplexType> CollectionWithFieldFollowingNameConvention { get { return _collectionWithFieldFollowingNameConvention; } }

        private readonly IList<int> _anotherCollection = new List<int>();
        public IEnumerable<int> AnotherCollection { get { return _anotherCollection; } }


        private readonly List<int> _anotherName = new List<int>();
        public IEnumerable<int> CollectionWithFieldNotFollowingNameConvention { get { return _anotherName; } }

        public ComplexType(string privateField, string publicField, string propertyWithSetter, string readOnlyPropertyWithBackingField,
            AnotherComplexType anotherComplexType, AnotherComplexType anotherComplexTypeINSENSITIVECASETest, ComplexType sameTypeEntityIsNotInitialized,
            List<AnotherComplexType> collectionWithFieldFollowingNameConvention, AbstractComplexType abstractComplexType)
        {
            // initialize members... 
        }

        public string PropertyOnlyForTestingPurpose { get { return privateField; } }
    }

    internal abstract class AbstractComplexType
    {
    }

    internal class AnotherComplexType
    {
        public string Name { get; private set; }

        public AnotherComplexType(string name)
        {
            Name = name;
        }
    }

    internal class OneMoreComplexType
    {
        public string Name { get; set; }
    }

    internal class StandAloneComplexType
    {

    }
}