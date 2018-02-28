using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Collections.Immutable;

namespace Boo.Lang.Compiler.TypeSystem.ReflectionMetadata
{
	internal class MetadataGenericContext
	{
		public MetadataGenericContext(List<IType> typeParameters, List<IType> methodParameters)
		{
			MethodParameters = methodParameters;
			TypeParameters = typeParameters;
		}

		public List<IType> MethodParameters { get; }
		public List<IType> TypeParameters { get; }
	}

	internal class MetadataSignatureDecoder : ISignatureTypeProvider<IType, MetadataGenericContext>
	{
		private readonly MetadataTypeSystemProvider _tss;
		private readonly MetadataReader _reader;

		public MetadataSignatureDecoder(MetadataTypeSystemProvider provider, MetadataReader reader)
		{
			_tss = provider;
			_reader = reader;
		}

		public IType GetArrayType(IType elementType, ArrayShape shape)
		{
			return elementType.MakeArrayType(shape.Rank);
		}

		public IType GetByReferenceType(IType elementType)
		{
			throw new NotImplementedException();
		}

		public IType GetFunctionPointerType(MethodSignature<IType> signature)
		{
			throw new NotImplementedException();
		}

		public IType GetGenericInstantiation(IType genericType, ImmutableArray<IType> typeArguments)
		{
			return ((IGenericTypeInfo)genericType).ConstructType(typeArguments.ToArray());
		}

		public IType GetGenericMethodParameter(MetadataGenericContext context, int index)
		{
			if (context == null)
				return null;
			return context.MethodParameters[index];
		}

		public IType GetGenericTypeParameter(MetadataGenericContext context, int index)
		{
			if (context == null)
				return null;
			return context.TypeParameters[index];
		}

		public IType GetModifiedType(IType modifier, IType unmodifiedType, bool isRequired)
		{
			throw new NotImplementedException();
		}

		public IType GetPinnedType(IType elementType)
		{
			throw new NotImplementedException();
		}

		public IType GetPointerType(IType elementType)
		{
			return elementType.MakePointerType();
		}

		public IType GetPrimitiveType(PrimitiveTypeCode typeCode)
		{
			throw new NotImplementedException();
		}

		public IType GetSZArrayType(IType elementType)
		{
			return elementType.MakeArrayType(1);
		}

		public IType GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
		{
			var definition = reader.GetTypeDefinition(handle);
			return _tss.GetTypeFromDefinition(definition, reader);
		}

		public IType GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
		{
			var reference = reader.GetTypeReference(handle);
			return _tss.GetTypeFromReference(reference, _reader);
		}
		
		public IType GetTypeFromSpecification(MetadataReader reader, MetadataGenericContext genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
		{
			return reader.GetTypeSpecification(handle).DecodeSignature(this, genericContext);
		}
	}
}
