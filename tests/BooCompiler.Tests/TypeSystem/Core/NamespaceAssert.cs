using Boo.Lang;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.Util;
using NUnit.Framework;

namespace BooCompiler.Tests.TypeSystem.Core
{
	class NamespaceAssert
	{
		public static IEntity ResolveSingle(INamespace root, string name)
		{
			List<IEntity> resolved = new List<IEntity>();
			Assert.IsTrue(root.Resolve(resolved, name, EntityType.Any), "Failed to resolve '{0}' against '{1}'", name, root);
			Assert.AreEqual(1, resolved.Count, resolved.ToString());
			return resolved[0];
		}

		public static IEntity ResolveQualifiedNameToSingle(INamespace root, string qualifiedName)
		{
			IEntity current = root;
			foreach (string part in qualifiedName.Split('.'))
			{
				current = ResolveSingle((INamespace) current, part);
			}
			return current;
		}

		public static Set<IEntity> ResolveQualifiedName(INamespace root, string qualifiedName)
		{
			INamespace current = root;
			string[] parts = qualifiedName.Split('.');
			for (int i=0; i < parts.Length - 1; ++i)
			{
				current = (INamespace) ResolveSingle(current, parts[i]);
			}
			Set<IEntity> result = new Set<IEntity>();
			current.Resolve(result, parts[parts.Length - 1], EntityType.Any);
			return result;
		}
	}
}