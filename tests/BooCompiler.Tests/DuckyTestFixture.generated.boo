namespace BooCompiler.Tests

import NUnit.Framework
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Pipelines

[TestFixture]
class DuckyTestFixture(AbstractCompilerTestCase):
	protected override def CustomizeCompilerParameters():
		_parameters.Ducky = true

	[Test]
	def @BOO_827_1():
		RunCompilerTestCase("BOO-827-1.boo")

	[Test]
	def @callable_1():
		RunCompilerTestCase("callable-1.boo")

	[Test]
	def @duck_12():
		RunCompilerTestCase("duck-12.boo")

	[Test]
	def @duck_slice_1():
		RunCompilerTestCase("duck-slice-1.boo")

	[Test]
	def @ducky_1():
		RunCompilerTestCase("ducky-1.boo")

	[Test]
	def @ducky_10():
		RunCompilerTestCase("ducky-10.boo")

	[Test]
	def @ducky_11():
		RunCompilerTestCase("ducky-11.boo")

	[Test]
	def @ducky_2():
		RunCompilerTestCase("ducky-2.boo")

	[Test]
	def @ducky_3():
		RunCompilerTestCase("ducky-3.boo")

	[Test]
	def @ducky_4():
		RunCompilerTestCase("ducky-4.boo")

	[Test]
	def @ducky_5():
		RunCompilerTestCase("ducky-5.boo")

	[Test]
	def @ducky_6():
		RunCompilerTestCase("ducky-6.boo")

	[Test]
	def @ducky_7():
		RunCompilerTestCase("ducky-7.boo")

	[Test]
	def @ducky_8():
		RunCompilerTestCase("ducky-8.boo")

	[Test]
	def @ducky_9():
		RunCompilerTestCase("ducky-9.boo")

	[Test]
	def @implicit_1():
		RunCompilerTestCase("implicit-1.boo")

	[Test]
	def @implicit_2():
		RunCompilerTestCase("implicit-2.boo")

	[Test]
	def @implicit_3():
		RunCompilerTestCase("implicit-3.boo")

	[Test]
	def @method_dispatch_1():
		RunCompilerTestCase("method-dispatch-1.boo")

	[Test]
	def @method_dispatch_10():
		RunCompilerTestCase("method-dispatch-10.boo")

	[Test]
	def @method_dispatch_11():
		RunCompilerTestCase("method-dispatch-11.boo")

	[Test]
	def @method_dispatch_12():
		RunCompilerTestCase("method-dispatch-12.boo")

	[Test]
	def @method_dispatch_2():
		RunCompilerTestCase("method-dispatch-2.boo")

	[Test]
	def @method_dispatch_3():
		RunCompilerTestCase("method-dispatch-3.boo")

	[Test]
	def @method_dispatch_4():
		RunCompilerTestCase("method-dispatch-4.boo")

	[Test]
	def @method_dispatch_5():
		RunCompilerTestCase("method-dispatch-5.boo")

	[Test]
	def @method_dispatch_6():
		RunCompilerTestCase("method-dispatch-6.boo")

	[Test]
	def @method_dispatch_7():
		RunCompilerTestCase("method-dispatch-7.boo")

	[Test]
	def @method_dispatch_8():
		RunCompilerTestCase("method-dispatch-8.boo")

	[Test]
	def @method_dispatch_9():
		RunCompilerTestCase("method-dispatch-9.boo")

	[Test]
	def @null_initializer_1():
		RunCompilerTestCase("null-initializer-1.boo")

	[Test]
	def @object_construction_1():
		RunCompilerTestCase("object-construction-1.boo")

	[Test]
	def @object_overloads_1():
		RunCompilerTestCase("object-overloads-1.boo")

	override protected def GetRelativeTestCasesPath() as string:
		return "ducky"
