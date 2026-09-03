namespace BooCompiler.Tests

import System
import Boo.Lang.Compiler
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Compiler.TypeSystem.Services
import Boo.Lang.Environments
import NUnit.Framework

[TestFixture]
class MyTest:
	[Test]
	def MyOutsideContext():
		try:
			service = My[of NameResolutionService].Instance
			Assert.Fail("expected InvalidOperationException, got ${service}")
		except as InvalidOperationException:
			pass

	[Test]
	def MyExistingService():
		RunInCompilerContextEnvironment({ Assert.AreSame(My[of CompilerContext].Instance.CodeBuilder, My[of BooCodeBuilder].Instance) })

	public class DummyService:
		pass

	[Test]
	def AutomaticServiceRegistration():
		RunInCompilerContextEnvironment() do:
			Assert.IsNotNull(My[of DummyService].Instance)
			Assert.AreSame(My[of DummyService].Instance, My[of DummyService].Instance)

	class DummyServiceExtension(DummyService):
		pass

	[Test]
	def MyExistingServiceThroughSubTyping():
		RunInCompilerContextEnvironment() do:
			Assert.IsNotNull(My[of DummyServiceExtension].Instance)
			Assert.AreSame(My[of DummyService].Instance, My[of DummyServiceExtension].Instance)

	private def RunInCompilerContextEnvironment(action as Action):
		ActiveEnvironment.With(CompilerContext(false).Environment):
			action()
