using System.Collections.Generic;

namespace Nosbor.FluentBuilder.Tests.SampleClasses
{
    internal class ComplexType
    {
        private string privateField;
        public string PublicField;
        public string PropertyWithSetter { get; private set; }
        public List<string> CollectionWithSetter { get; protected set; }
        public AnotherComplexType AnotherComplexType { get; private set; }
        public AnotherComplexType AnotherComplexTypeInsensitiveCaseTest { get; private set; }
        public ComplexType SameTypeEntityIsNotInitialized { get; private set; }

        private string _propertyWithBackingField;
        public string PropertyWithBackingField { get { return _propertyWithBackingField; } }

        private readonly IList<AnotherComplexType> _collectionWithFieldFollowingNameConvention = new List<AnotherComplexType>();
        public IEnumerable<AnotherComplexType> CollectionWithFieldFollowingNameConvention { get { return _collectionWithFieldFollowingNameConvention; } }

        private readonly List<int> _anotherName = new List<int>();
        public IEnumerable<int> CollectionWithFieldNotFollowingNameConvention { get { return _anotherName; } }

        public ComplexType(string privateField, string publicField, string propertyWithSetter, string propertyWithBackingField,
            AnotherComplexType anotherComplexType, AnotherComplexType anotherComplexTypeINSENSITIVECASETest, ComplexType sameTypeEntityIsNotInitialized,
            List<AnotherComplexType> collectionWithFieldFollowingNameConvention)
        {
            // initialize members... 
        }
    }

    internal class AnotherComplexType
    {
        public string Name { get; private set; }

        public AnotherComplexType(string name)
        {
            Name = name;
        }
    }
}