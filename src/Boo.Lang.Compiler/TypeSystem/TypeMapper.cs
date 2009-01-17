using System;
using System.Collections.Generic;
using System.Text;

namespace Boo.Lang.Compiler.TypeSystem
{
	public abstract class TypeMapper
	{
		private TypeSystemServices _tss;
		protected IDictionary<IEntity, IEntity> _cache = new Dictionary<IEntity, IEntity>();

		public TypeMapper(TypeSystemServices tss)
		{
			_tss = tss;
		}

		protected TypeSystemServices TypeSystemServices
		{
			get { return _tss; } 
		}

		protected TEntity Cache<TEntity>(TEntity source, TEntity mapped) where TEntity : IEntity
		{
			_cache[source] = mapped;
			return mapped;
		}

		public virtual IType MapType(IType sourceType)
		{
			if (sourceType == null) return null;

			if (_cache.ContainsKey(sourceType)) return (IType)_cache[sourceType];

			if (sourceType.IsByRef)
			{
				return MapByRefType(sourceType);
			}

			if (sourceType.ConstructedInfo != null)
			{
				return MapConstructedType(sourceType);
			}

			// TODO: Map nested types
			// GenericType[of T].NestedType => GenericType[of int].NestedType

			IArrayType array = (sourceType as IArrayType);
			if (array != null) return MapArrayType(array);

			AnonymousCallableType anonymousCallableType = sourceType as AnonymousCallableType;
			if (anonymousCallableType != null)
			{
				return MapCallableType(anonymousCallableType);
			}

			return sourceType;
		}

		public virtual IType MapByRefType(IType sourceType)
		{
			IType elementType = MapType(sourceType.GetElementType());
			return elementType;
		}

		public virtual IType MapArrayType(IArrayType sourceType)
		{
			IType elementType = MapType(sourceType.GetElementType());
			return _tss.GetArrayType(elementType, sourceType.GetArrayRank());
		}

		public virtual IType MapCallableType(AnonymousCallableType sourceType)
		{
			CallableSignature signature = sourceType.GetSignature();

			IType returnType = MapType(signature.ReturnType);
			IParameter[] parameters = MapParameters(signature.Parameters);

			CallableSignature mappedSignature = new CallableSignature(
				parameters, returnType, signature.AcceptVarArgs);

			return TypeSystemServices.GetCallableType(mappedSignature);
		}

		public virtual IType MapConstructedType(IType sourceType)
		{
			IType mappedDefinition = MapType(sourceType.ConstructedInfo.GenericDefinition);

			IType[] mappedArguments = Array.ConvertAll<IType, IType>(
				sourceType.ConstructedInfo.GenericArguments,
				MapType);

			IType mapped = mappedDefinition.GenericInfo.ConstructType(mappedArguments);

			return mapped;
		}

		internal IParameter[] MapParameters(IParameter[] parameters)
		{
			return Array.ConvertAll<IParameter, IParameter>(parameters, MapParameter);
		}

		internal IParameter MapParameter(IParameter parameter)
		{
			if (_cache.ContainsKey(parameter)) return _cache[parameter] as IParameter;
			return Cache<IParameter>(parameter, new MappedParameter(parameter, MapType(parameter.Type)));
		}
	}

	public class TypeReplacer : TypeMapper
	{
		IDictionary<IType, IType> _map;

		public TypeReplacer(TypeSystemServices tss) : base(tss)
		{
			_map = new Dictionary<IType, IType>();
		}
		
		protected IDictionary<IType, IType> TypeMap
		{
			get { return _map; }
		}

		public void Replace(IType source, IType replacement)
		{
			_map[source] = replacement;
		}

		public override IType MapType(IType sourceType)
		{
			if (TypeMap.ContainsKey(sourceType)) return TypeMap[sourceType];
			
			return base.MapType(sourceType);
		}
	}
}
