"""
false == true
"""
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.MetaProgramming

macroModule = [|
	namespace Test
	
	import Test.Impl
	
	macro _assert:
		condition = conditionFor(_assert)
		yield [| raise $(condition.ToCodeString()) unless $condition |]
|]
globals = [|
	namespace Test.Impl
	
	import Boo.Lang.Compiler.Ast
	
	// internal version should be preferred over
	// external one
	def conditionFor(node as MacroStatement):
		condition, = node.Arguments
		return condition
|]

macroModule.Name = "Test" # forces the assembly name
macroAssembly = compile(CompileUnit(macroModule, globals.CloneNode()))

bootstrapped1 = [|
	namespace Test
	
	macro _assert:
		yield Test.Impl.Expander().Expand(_assert)
|]
bootstrapped2 = [|
	namespace Test.Impl
	
	import Test from Test
	import Boo.Lang.Compiler.Ast
	
	class Expander:
		def Expand(node as MacroStatement):
			_assert len(node.Arguments) == 1
			condition = conditionFor(node)
			return [| raise $(condition.ToCodeString()) unless $condition |]
|]

bootstrappedAssembly = compile(CompileUnit(bootstrapped1, bootstrapped2, globals), macroAssembly)

test = [|
	import Test
	
	_assert false == true
|]
try:
	compile(test, bootstrappedAssembly).EntryPoint.Invoke(null, (null,))
except x as System.Reflection.TargetInvocationException:
	print x.InnerException.Message

