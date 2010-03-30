using System.Linq;

namespace Boo.Lang.Compiler.TypeSystem.Services
{
	public static class MemberCollector
	{
		public static IEntity[] CollectAllMembers(INamespace entity)
		{
			List members = new List();
			CollectAllMembers(members, entity);
			return (IEntity[])members.ToArray(new IEntity[members.Count]);
		}

		private static void CollectAllMembers(List members, INamespace entity)
		{
			IType type = entity as IType;
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

		private static void CollectBaseTypeMembers(List members, IType baseType)
		{
			if (null == baseType) return;

			members.Extend(baseType.GetMembers().Where(m => !(m is IConstructor) && !IsHiddenBy(m, members)));

			CollectBaseTypeMembers(members, baseType.BaseType);
		}

		private static bool IsHiddenBy(IEntity entity, List members)
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
