"""
Init(42)
42
42
"""
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.MetaProgramming

macro deferred:
	case [| deferred $(ReferenceExpression(Name: name)) = $initializer |]:
		p = Property(Name: name)
		p.Getter = [|
			[OnceAttribute]
			def get():
				return $initializer
		|]
		yield p
		
class OnceAttribute(AbstractAstAttribute):
	override def Apply(node as Node):
		method = node as Method
		
		prototype = [|
			class _:
				private cache = null
				
				def OldMethod():
					$(method.Body)
		|]
		
		method.Body = [|
			if cache is not null: return cache
			return cache = OldMethod()
		|]
		
		method.DeclaringType.Members.Extend(prototype.Members)
		
code = [|
	import System
	
	class Foo:
		deferred Bar = Init(42)
		
	def Init(value):
		print "Init($value)"
		return value
		
	f = Foo()
	print f.Bar
	print f.Bar
|]

compile(code, typeof(OnceAttribute).Assembly).GetEntryPoint().Invoke(null, (null,))		

