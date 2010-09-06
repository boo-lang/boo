using System.Collections.Generic;
using System.Linq;

namespace Boo.Lang.Compiler.TypeSystem.Services
{
	public static class MemberCollector
	{
		public static IEntity[] CollectAllMembers(INamespace entity)
		{
			var members = new List<IEntity>();
			CollectAllMembers(members, entity);
			return members.ToArray();
		}

		private static void CollectAllMembers(List<IEntity> members, INamespace entity)
		{
			var type = entity as IType;
			if (null != type)
			{
				members.ExtendUnique(type.GetMembers());
				CollectBaseTypeMembers(members, type.BaseType);
			}
			else
			{
				members.Extend(entity.GetMembers());
			}
		}

		private static void CollectBaseTypeMembers(List<IEntity> members, IType baseType)
		{
			if (null == baseType) return;

			members.Extend(baseType.GetMembers().Where(m => !(m is IConstructor) && !IsHiddenBy(m, members)));

			CollectBaseTypeMembers(members, baseType.BaseType);
		}

		private static bool IsHiddenBy(IEntity entity, IEnumerable<IEntity> members)
		{
			var m = entity as IMethod;
			if (m != null)
				return members.OfType<IMethod>().Any(existing => SameNameAndSignature(m, existing));
			return members.OfType<IEntity>().Any(existing => existing.Name == entity.Name);
		}

		private static bool SameNameAndSignature(IMethod method, IMethod existing)
		{
			if (method.Name != existing.Name)
				return false;
			return method.CallableType == existing.CallableType;
		}
	}
}
