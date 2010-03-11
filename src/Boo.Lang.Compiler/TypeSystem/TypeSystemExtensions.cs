using System.Collections.Generic;
using System.Linq;

namespace Boo.Lang.Compiler.TypeSystem
{
	public static class TypeSystemExtensions
	{
		public static IEnumerable<IConstructor> GetConstructors(this IType self)
		{
			foreach (var member in self.GetMembers())
			{
				var ctor = member as IConstructor;
				if (ctor == null || ctor.IsStatic)
					continue;
				yield return ctor;
			}
		}
	}
}
