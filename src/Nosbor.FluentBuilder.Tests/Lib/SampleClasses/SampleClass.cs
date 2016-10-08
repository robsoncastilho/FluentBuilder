using System.Collections.Generic;

namespace Nosbor.FluentBuilder.Tests.Lib.SampleClasses
{
    internal class SampleClass
    {
        public string PublicField;
        public string String;
        public OneMoreComplexType OneMoreComplexType;

        public string PropertyWithSetter { get; private set; }
        public List<string> CollectionWithSetter { get; protected set; }
        public int[] ArrayWithSetter { get; set; }
        public string ReadOnlyProperty => "read-only";

        private readonly IList<AnotherComplexType> _collectionWithFieldFollowingNameConvention = new List<AnotherComplexType>();
        public IEnumerable<AnotherComplexType> CollectionWithFieldFollowingNameConvention => _collectionWithFieldFollowingNameConvention;

        // TODO: not working
        public string GetOnlyProperty { get; }

        public AnotherComplexType AnotherComplexType { get; private set; }
        public AnotherComplexType AnotherComplexTypeInsensitiveCaseTest { get; private set; }
        public AnotherComplexType OneMoreComplexType1 { get; protected set; }

        public AbstractComplexType AbstractComplexType { get; private set; }

        public SampleClass SameTypeEntityIsNotInitialized { get; private set; }
      
        private readonly IList<int> _anotherCollection = new List<int>();
        public IEnumerable<int> AnotherCollection { get { return _anotherCollection; } }

        private readonly List<int> _anotherName = new List<int>();
        public IEnumerable<int> CollectionWithFieldNotFollowingNameConvention { get { return _anotherName; } }

        public SampleClass(string privateField, string publicField, string propertyWithSetter, string readOnlyPropertyWithBackingField,
            AnotherComplexType anotherComplexType, AnotherComplexType anotherComplexTypeINSENSITIVECASETest, SampleClass sameTypeEntityIsNotInitialized,
            List<AnotherComplexType> collectionWithFieldFollowingNameConvention, AbstractComplexType abstractComplexType)
        {
            // initialize members... 
        }
    }

    internal class OneMoreComplexType
    {
        public string Name { get; set; }
    }

    internal abstract class AbstractComplexType
    {
    }

    internal class ConcreteComplexType : AbstractComplexType
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

    internal class StandAloneComplexType
    {

    }
}