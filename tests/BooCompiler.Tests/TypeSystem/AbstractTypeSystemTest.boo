namespace BooCompiler.Tests.TypeSystem

import System
import System.Collections.Generic
import Boo.Lang.Compiler
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Compiler.TypeSystem.Builders
import Boo.Lang.Environments
import NUnit.Framework

class AbstractTypeSystemTest:
	protected Context as CompilerContext

	protected Environment as IEnvironment:
		get: return Context.Environment

	protected CodeBuilder as BooCodeBuilder:
		get: return Context.CodeBuilder

	protected static TypeSystemServices as TypeSystemServices:
		get: return My[of TypeSystemServices].Instance

	[SetUp]
	public virtual def SetUp():
		Context = CompilerContext(false)

	protected def RunInCompilerContextEnvironment(action as System.Action):
		ActiveEnvironment.With(Environment):
			action()

	protected def InvokeInCompilerContextEnvironment[of T](function as System.Func[of T]) as T:
		container = List[of T]()
		ActiveEnvironment.With(Environment):
			container.Add(function())
		return container[0]

	protected def DefineInternalClass(namespace_ as string, typeName as string) as IType:
		return BuildInternalClass(namespace_, typeName).Entity

	protected def BuildInternalClass(namespace_ as string, typeName as string) as BooClassBuilder:
		classBuilder = CodeBuilder.CreateClass(typeName)
		classModule = CodeBuilder.CreateModule(typeName + "Module", namespace_)
		classModule.Members.Add(classBuilder.ClassDefinition)
		Context.CompileUnit.Modules.Add(classModule)
		return classBuilder
