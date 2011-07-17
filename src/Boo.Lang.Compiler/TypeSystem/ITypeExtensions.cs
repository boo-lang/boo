using System;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.TypeSystem
{
	public static class IEntityExtensions
	{
		public static string DisplayName(this IEntity entity)
		{
			var formatter = My<EntityFormatter>.Instance;

			var type = entity as IType;
			if (type != null)
				return formatter.FormatType(type);

			var method = entity as IMember;
			if (method != null)
				return formatter.FormatTypeMember(method);

			var @namespace = entity as INamespace;
			if (@namespace != null)
				return @namespace.FullName;

			throw new NotSupportedException(entity.GetType().ToString());
		}
	}

	public static class ITypeExtensions
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
