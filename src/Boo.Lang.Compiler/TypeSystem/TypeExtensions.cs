namespace Boo.Lang.Compiler.TypeSystem
{
	public static class TypeExtensions
	{
		public static string QualifiedName(this IType type)
		{
			if (type is IGenericParameter)
				return type.Name;
			return type.FullName;
		}

		public static bool IsNull(this IType other)
		{
			return other == Null.Default;
		}
	}
}