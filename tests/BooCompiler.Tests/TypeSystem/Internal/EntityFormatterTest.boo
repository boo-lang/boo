namespace BooCompiler.Tests.TypeSystem.Internal

import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Compiler.TypeSystem.Builders
import Boo.Lang.Environments
import BooCompiler.Tests.TypeSystem
import NUnit.Framework

[TestFixture]
class EntityFormatterTest(EntityFormatterTestBase):
	protected override def SimpleType() as IType:
		return InvokeInCompilerContextEnvironment[of IType]({ DefineInternalClass("Foo", "Bar") })

	protected override def CallableType() as IType:
		return InvokeInCompilerContextEnvironment[of IType](
			{ My[of CallableTypeBuilder].Instance.ForCallableDefinition(CallableDefinition(Name: "Foo")).Entity })

	protected override def GenericType() as IType:
		return InvokeInCompilerContextEnvironment[of IType]({ DefineGenericType() })

	private def DefineGenericType() as IType:
		type = BuildInternalClass("Foo", "Bar")
		type.AddGenericParameter("T")
		return type.Entity

	protected override def ArrayType() as IType:
		return SimpleType().MakeArrayType(1)
