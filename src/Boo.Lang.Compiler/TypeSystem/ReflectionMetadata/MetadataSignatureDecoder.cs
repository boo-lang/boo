using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;

namespace Boo.Lang.Compiler.TypeSystem.ReflectionMetadata
{
	using System.Collections.Generic;
	internal class MetadataGenericContext
	{
		public MetadataGenericContext(IEnumerable<IType> typeParameters, IEnumerable<IType> methodParameters)
		{
			MethodParameters = methodParameters == null ? new List<IType>() : methodParameters.ToList();
			TypeParameters = typeParameters == null ? new List<IType>() : typeParameters.ToList();
		}

		public List<IType> MethodParameters { get; }
		public List<IType> TypeParameters { get; }
	}

	internal class MetadataSignatureDecoder : ISignatureTypeProvider<IType, MetadataGenericContext>, ICustomAttributeTypeProvider<IType>
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
			return genericType.GenericInfo.ConstructType(typeArguments.ToArray());
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
			switch (typeCode)
			{
				case PrimitiveTypeCode.Boolean:
					return _tss.Map(typeof(bool));
				case PrimitiveTypeCode.Byte:
					return _tss.Map(typeof(byte));
				case PrimitiveTypeCode.SByte:
					return _tss.Map(typeof(SByte));
				case PrimitiveTypeCode.Char:
					return _tss.Map(typeof(char));
				case PrimitiveTypeCode.Int16:
					return _tss.Map(typeof(Int16));
				case PrimitiveTypeCode.UInt16:
					return _tss.Map(typeof(UInt16));
				case PrimitiveTypeCode.Int32:
					return _tss.Map(typeof(Int32));
				case PrimitiveTypeCode.UInt32:
					return _tss.Map(typeof(UInt32));
				case PrimitiveTypeCode.Int64:
					return _tss.Map(typeof(Int64));
				case PrimitiveTypeCode.UInt64:
					return _tss.Map(typeof(UInt64));
				case PrimitiveTypeCode.Single:
					return _tss.Map(typeof(Single));
				case PrimitiveTypeCode.Double:
					return _tss.Map(typeof(Double));
				case PrimitiveTypeCode.IntPtr:
					return _tss.Map(typeof(IntPtr));
				case PrimitiveTypeCode.UIntPtr:
					return _tss.Map(typeof(UIntPtr));
				case PrimitiveTypeCode.Object:
					return _tss.Map(typeof(object));
				case PrimitiveTypeCode.String:
					return _tss.Map(typeof(string));
				case PrimitiveTypeCode.TypedReference:
					return _tss.Map(typeof(TypedReference));
				case PrimitiveTypeCode.Void:
					return _tss.Map(typeof(void));
				default:
					throw new ArgumentException(string.Format("Unknown primitive type code: {0}", typeCode));
			}
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

		public IType GetSystemType()
		{
			return _tss.Map(typeof(System.Type));
		}

		public IType GetTypeFromSerializedName(string name)
		{
			throw new NotImplementedException();
		}

		public PrimitiveTypeCode GetUnderlyingEnumType(IType type)
		{
			if (!type.IsEnum)
				throw new ArgumentException("Not an enum type");
			var fieldType = type.GetMembers().OfType<IField>().Single(f => !f.IsStatic).Type;
			switch (((MetadataExternalType)fieldType).PrimitiveName)
			{
				case "sbyte":
					return PrimitiveTypeCode.SByte;
				case "byte":
					return PrimitiveTypeCode.Byte;
				case "short":
					return PrimitiveTypeCode.Int16;
				case "ushort":
					return PrimitiveTypeCode.UInt16;
				case "int":
					return PrimitiveTypeCode.Int32;
				case "uint":
					return PrimitiveTypeCode.UInt32;
				case "long":
					return PrimitiveTypeCode.Int64; 
				case "ulong":
					return PrimitiveTypeCode.UInt64;
				default:
					throw new ArgumentException("Invalid enum type");
			}
		}

		public bool IsSystemType(IType type)
		{
			return type.Equals(GetSystemType());
		}
	}
}
