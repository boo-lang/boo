using System.Collections.Generic;

namespace Boo.Lang.Compiler.TypeSystem.Core
{
    internal class ByRefType : IType
    {
        public ByRefType(IType baseType)
        {
            _type = baseType;
        }

        private readonly IType _type;

        public bool IsClass => _type.IsClass;

        public bool IsAbstract => _type.IsAbstract;

        public bool IsInterface => _type.IsInterface;

        public bool IsEnum => _type.IsEnum;

        public bool IsByRef => true;

        public bool IsValueType => _type.IsValueType;

        public bool IsFinal => _type.IsFinal;

        public bool IsArray => _type.IsArray;

        public bool IsPointer => _type.IsPointer;

        public IEntity DeclaringEntity => _type.DeclaringEntity;

        public IType ElementType => _type;

        public IType BaseType => _type.BaseType;

        public IGenericTypeInfo GenericInfo => _type.GenericInfo;

        public IConstructedTypeInfo ConstructedInfo => _type.ConstructedInfo;

        public IType Type => this;

        public INamespace ParentNamespace => _type.ParentNamespace;

        public string Name => _type.Name;

        public string FullName => _type.FullName;

        public EntityType EntityType => _type.EntityType;

        public IEntity GetDefaultMember()
        {
            return _type.GetDefaultMember();
        }

        public IType[] GetInterfaces()
        {
            return _type.GetInterfaces();
        }

        public IEnumerable<IEntity> GetMembers()
        {
            return _type.GetMembers();
        }

        public int GetTypeDepth()
        {
            return _type.GetTypeDepth();
        }

        public bool IsAssignableFrom(IType other)
        {
            return _type.IsAssignableFrom(other);
        }

        public bool IsDefined(IType attributeType)
        {
            return _type.IsDefined(attributeType);
        }

        public bool IsSubclassOf(IType other)
        {
            return _type.IsSubclassOf(other);
        }

        public IArrayType MakeArrayType(int rank)
        {
            return _type.MakeArrayType(rank);
        }

        public IType MakePointerType()
        {
            return _type.MakePointerType();
        }

        public bool Resolve(ICollection<IEntity> resultingSet, string name, EntityType typesToConsider)
        {
            return _type.Resolve(resultingSet, name, typesToConsider);
        }
    }
}
