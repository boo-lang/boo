namespace BooCompiler.Tests.TypeSystem.Reflection

import System
import Boo.Lang
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Compiler.TypeSystem.Reflection
import BooCompiler.Tests.TypeSystem
import NUnit.Framework

[TestFixture]
class EntityFormatterTest(EntityFormatterTestBase):
	private final _subject as IReflectionTypeSystemProvider = ReflectionTypeSystemProvider()

	protected override def SimpleType() as IType:
		return Map(typeof(object))

	protected override def ArrayType() as IType:
		return Map(typeof((object)))

	protected override def CallableType() as IType:
		return Map(typeof(System.Action))

	protected override def GenericType() as IType:
		return Map(typeof(List[of *]))

	private def Map(type as Type) as IType:
		return _subject.Map(type)
