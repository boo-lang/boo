using System.Collections.Generic;
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.TypeSystem.Builders
{
	public class MappedTypeReferenceFactory : ITypeReferenceFactory
	{
		private readonly ITypeReferenceFactory _typeReferenceFactory;
		private readonly IDictionary<IType, IType> _typeMappings;

		public MappedTypeReferenceFactory(ITypeReferenceFactory typeReferenceFactory, IDictionary<IType, IType> typeMappings)
		{
			_typeReferenceFactory = typeReferenceFactory;
			_typeMappings = typeMappings;
		}

		public TypeReference TypeReferenceFor(IType type)
		{
			return _typeReferenceFactory.TypeReferenceFor(Map(type));
		}

		private IType Map(IType type)
		{
			IType mappedType;
			if (_typeMappings.TryGetValue(type, out mappedType))
				return mappedType;

			if (type.IsArray)
			{
				var arrayType = ((IArrayType)type);
				var mappedElementType = Map(arrayType.ElementType);
				return mappedElementType.MakeArrayType(arrayType.Rank);
			}
			return type;
		}
	}
}