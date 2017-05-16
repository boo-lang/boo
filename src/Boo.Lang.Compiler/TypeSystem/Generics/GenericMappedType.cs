using System;
using System.Collections.Generic;
using System.Linq;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem.Core;
using Boo.Lang.Compiler.TypeSystem.Services;

namespace Boo.Lang.Compiler.TypeSystem.Generics
{
    public interface IGenericMappedType : IType
    {
        IType SourceType { get; }
    }

    public class GenericMappedType : IGenericMappedType
    {
        private readonly IType _sourceType;
        private readonly GenericConstructedType _containingType;

        private static Dictionary<KeyValuePair<IType, GenericConstructedType>, GenericMappedType> _cache =
            new Dictionary<KeyValuePair<IType, GenericConstructedType>, GenericMappedType>();

        public static GenericMappedType Create(IType sourceType, GenericConstructedType containingType)
        {
            var pair = new KeyValuePair<IType, GenericConstructedType>(sourceType, containingType);
            GenericMappedType result;
            if (!_cache.TryGetValue(pair, out result))
            {
                result = new GenericMappedType(sourceType, containingType);
                _cache[pair] = result;
            }
            return result;
        }

        protected GenericMappedType(IType sourceType, GenericConstructedType containingType)
		{
            if (sourceType.DeclaringEntity != ((IConstructedTypeInfo)containingType).GenericDefinition)
                throw new ArgumentException("Mapping type onto invalid container");

            _sourceType = sourceType;
            _containingType = containingType;
		}

        public IType SourceType { get { return _sourceType; } }

        public GenericMapping GenericMapping
        {
            get { return _containingType.GenericMapping; }
        }

        public string Name
        {
            get { return _sourceType.Name; }
        }

        public string FullName
        {
            get { return _sourceType.FullName; }
        }

        public EntityType EntityType
        {
            get { return EntityType.Type; }
        }

        public IGenericTypeInfo GenericInfo
        {
            get { return _sourceType.GenericInfo; }
        }

        public IConstructedTypeInfo ConstructedInfo
        {
            get { return null; }
        }

        public IType Type
        {
            get { return this; }
        }

        public IEntity DeclaringEntity
        {
            get { return _containingType; }
        }

        public bool IsClass
        {
            get { return _sourceType.IsClass; }
        }

        public bool IsAbstract
        {
            get { return _sourceType.IsAbstract; }
        }

        public bool IsInterface
        {
            get { return _sourceType.IsInterface; }
        }

        public bool IsEnum
        {
            get { return _sourceType.IsEnum; }
        }

        public bool IsByRef
        {
            get { return _sourceType.IsByRef; }
        }

        public bool IsValueType
        {
            get { return _sourceType.IsValueType; }
        }

        public bool IsFinal
        {
            get { return _sourceType.IsFinal; }
        }

        public bool IsArray
        {
            get { return _sourceType.IsArray; }
        }

        public bool IsPointer
        {
            get { return _sourceType.IsPointer; }
        }

        public int GetTypeDepth()
        {
            return _sourceType.GetTypeDepth();
        }

        public bool IsDefined(IType attributeType)
        {
            return _sourceType.IsDefined(GenericMapping.MapType(attributeType));
        }

        public IEnumerable<IEntity> GetMembers()
        {
            return _sourceType.GetMembers().Select(GenericMapping.Map);
        }

        public INamespace ParentNamespace
        {
            get
            {
                return GenericMapping.Map(_sourceType.ParentNamespace) as INamespace;
            }
        }

        public bool Resolve(ICollection<IEntity> resultingSet, string name, EntityType typesToConsider)
        {
            var definitionMatches = new HashSet<IEntity>();
            if (!_sourceType.Resolve(definitionMatches, name, typesToConsider))
                return false;
            foreach (var match in definitionMatches)
                resultingSet.Add(GenericMapping.Map(match));
            return true;
        }

        public IType BaseType
        {
            get { return GenericMapping.MapType(_sourceType.BaseType); }
        }

        public IType ElementType
        {
            get { return GenericMapping.MapType(_sourceType.ElementType); }
        }

        public IEntity GetDefaultMember()
        {
            IEntity definitionDefaultMember = _sourceType.GetDefaultMember();
            if (definitionDefaultMember != null) return GenericMapping.Map(definitionDefaultMember);
            return null;
        }

        public IType[] GetInterfaces()
        {
            return Array.ConvertAll(
                _sourceType.GetInterfaces(),
                GenericMapping.MapType);
        }

        public virtual bool IsAssignableFrom(IType other)
        {
            if (other == null)
                return false;

            if (other == this || other.IsSubclassOf(this) || (other.IsNull() && !IsValueType) || IsGenericAssignableFrom(other))
                return true;

            return false;
        }

        public bool IsGenericAssignableFrom(IType other)
        {
            var gmt = other as GenericMappedType;
            if (gmt == null) 
                return false;

            if (!this._containingType.IsGenericAssignableFrom(gmt._containingType))
                return false;

            var st = _sourceType;
            return (st != null && st.IsAssignableFrom(gmt._sourceType));
        }

        public bool IsSubclassOf(IType other)
        {
            if (null == other)
                return false;

            if (BaseType != null && (BaseType == other || BaseType.IsSubclassOf(other)))
            {
                return true;
            }

            return other.IsInterface && Array.Exists(
                        GetInterfaces(),
                        i => TypeCompatibilityRules.IsAssignableFrom(other, i));
        }

        private ArrayTypeCache _arrayTypes;

        public IArrayType MakeArrayType(int rank)
        {
            if (null == _arrayTypes)
                _arrayTypes = new ArrayTypeCache(this);
            return _arrayTypes.MakeArrayType(rank);
        }

        public IType MakePointerType()
        {
            return null;
        }
    }
}
