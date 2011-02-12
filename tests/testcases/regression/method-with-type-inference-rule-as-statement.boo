"""
GetComponent(System.String)
"""
import Boo.Lang.Compiler.MetaProgramming

[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
def GetComponent(type as System.Type):
	print "GetComponent($type)"
	return null
	
code = [|
	import System
	GetComponent(String)
|]

compile(code, System.Reflection.Assembly.GetExecutingAssembly()).EntryPoint.Invoke(null, (null,))
