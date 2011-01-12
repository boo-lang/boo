using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.TypeSystem.Builders
{
	public class StandardTypeReferenceFactory : ITypeReferenceFactory
	{
		private readonly ICodeBuilder _codeBuilder;

		public StandardTypeReferenceFactory(ICodeBuilder codeBuilder)
		{
			_codeBuilder = codeBuilder;
		}

		public TypeReference TypeReferenceFor(IType type)
		{
			if (type.IsArray)
			{
				IArrayType arrayType = (IArrayType)type;
				return new ArrayTypeReference(TypeReferenceFor(arrayType.ElementType), CreateIntegerLiteral(arrayType.Rank))
				{ Entity = type };
			}
			// TODO: support for generic types
			return new SimpleTypeReference(DisplayNameFor(type)) { Entity = type };
		}

		private static string DisplayNameFor(IType type)
		{
			if (type is IGenericParameter)
				return type.Name;
			return type.FullName;
		}

		private IntegerLiteralExpression CreateIntegerLiteral(int value)
		{
			return _codeBuilder.CreateIntegerLiteral(value);
		}
	}
}
