using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.TypeSystem
{
	static class ITypeExtensions
	{
		public static string QualifiedName(this IType type)
		{
			if (type is IGenericParameter)
				return type.Name;
			return type.FullName;
		}

		public static string DisplayName(this IType arrayType)
		{
			return My<EntityFormatter>.Instance.FormatType(arrayType);
		}

		public static bool IsNull(this IType other)
		{
			return other == Null.Default;
		}
	}
}
