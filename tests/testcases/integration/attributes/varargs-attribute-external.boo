"""
C0
C1
	foo
C2
	foo
	bar
"""
import System
import Boo.Lang.Compiler.MetaProgramming

class VarArgsAttribute(Attribute):
	public final Args as (string)
	def constructor(*args as (string)):
		Args = args
		
code = [|
	import System
	
	[VarArgs]
	class C0:
		pass
		
	[VarArgs("foo")]
	class C1:
		pass
			
	[VarArgs("foo", "bar")]
	class C2:
		pass
|]

assembly = compile(code, typeof(VarArgsAttribute).Assembly)
for type in assembly.GetTypes():
	print type.Name
	for arg in (Attribute.GetCustomAttribute(assembly.GetType(type.Name), VarArgsAttribute) as VarArgsAttribute).Args:
		print "\t$arg"
		
