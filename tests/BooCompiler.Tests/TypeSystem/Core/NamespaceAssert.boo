namespace BooCompiler.Tests.TypeSystem.Core

import Boo.Lang
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Compiler.Util
import NUnit.Framework

class NamespaceAssert:
	public static def ResolveSingle(root as INamespace, name as string) as IEntity:
		resolved = List[of IEntity]()
		Assert.IsTrue(root.Resolve(resolved, name, EntityType.Any), "Failed to resolve '{0}' against '{1}'", name, root)
		Assert.AreEqual(1, resolved.Count)
		return resolved[0]

	public static def ResolveQualifiedNameToSingle(root as INamespace, qualifiedName as string) as IEntity:
		current as IEntity = root
		for part in qualifiedName.Split(char('.')):
			current = ResolveSingle(current as INamespace, part)
		return current

	public static def ResolveQualifiedName(root as INamespace, qualifiedName as string) as Set[of IEntity]:
		current as INamespace = root
		parts = qualifiedName.Split(char('.'))
		for i in range(0, parts.Length - 1):
			current = ResolveSingle(current, parts[i]) as INamespace
		result = Set[of IEntity]()
		current.Resolve(result, parts[parts.Length - 1], EntityType.Any)
		return result
