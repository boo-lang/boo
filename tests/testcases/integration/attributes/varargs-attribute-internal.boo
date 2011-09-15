"""
C0
C1
	foo
C2
	foo
	bar
"""
import System

class VarArgsAttribute(Attribute):
	public final Args as (string)
	def constructor(*args as (string)):
		Args = args

[VarArgs]
class C0:
	pass
	
[VarArgs("foo")]
class C1:
	pass
		
[VarArgs("foo", "bar")]
class C2:
	pass
	
for type in C0, C1, C2:
	print type.Name
	for arg in (Attribute.GetCustomAttribute(type, VarArgsAttribute) as VarArgsAttribute).Args:
		print "\t$arg"
		
