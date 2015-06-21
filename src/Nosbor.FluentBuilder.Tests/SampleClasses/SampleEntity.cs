using System.Collections.Generic;

namespace Nosbor.FluentBuilder.Tests.SampleClasses
{
    internal class SampleEntity
    {
        public string Name { get; private set; }
        public List<string> AddressesWithSetter { get; protected set; }

        private readonly IList<string> _addresses = new List<string>();
        public IEnumerable<string> Addresses { get { return _addresses; } }

        private readonly List<int> _anotherName = new List<int>();
        public IEnumerable<int> CollectionWithFieldNotFollowingNameConvention { get { return _anotherName; } }
    }

    internal class SampleEntityWithNoParameterlessCtor
    {
        public string Name { get; private set; }
        public SampleSimpleEntity SampleEntity { get; private set; }
        public SampleEntityWithNoParameterlessCtor SameTypeEntity { get; private set; }

        public SampleEntityWithNoParameterlessCtor(string name, SampleSimpleEntity sampleEntity, SampleEntityWithNoParameterlessCtor sameTypeEntity)
        {
            Name = name;
            SampleEntity = sampleEntity;
            SameTypeEntity = sameTypeEntity;
        }
    }

    internal class SampleSimpleEntity
    {
        public string Name { get; private set; }

        public SampleSimpleEntity(string name)
        {
            Name = name;
        }
    }
}