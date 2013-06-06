using System;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.TypeSystem
{
	public static class EntityExtensions
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

		public static bool IsAmbiguous(this IEntity entity)
		{
			return entity != null && entity.EntityType == EntityType.Ambiguous;
		}

		public static bool IsIndexedProperty(this IEntity entity)
		{
			var property = entity as IProperty;
			return property != null && property.GetParameters().Length > 0;
		}
	}
}
