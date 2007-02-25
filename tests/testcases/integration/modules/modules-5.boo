import System
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines

code = """
import System

[System.Runtime.CompilerServices.CompilerGlobalScope]
class MyModule:
	
	static def constructor():
		pass
		
	public static Version = "0.5"
	
	static def HeyBooWhatYouReDoing():
		return "I'm standing on the verge of getting it on"		
	
	[STAThread]
	static def Main(argv as (string)):
		print("it works!")
		
def Spam():
	return "eggs"
"""

compiler = BooCompiler()
compiler.Parameters.Input.Add(StringInput("code", code))
compiler.Parameters.Pipeline = CompileToMemory()

result = compiler.Run()
assert 0 == len(result.Errors), "\n" + result.Errors.ToString(true)

types = result.GeneratedAssembly.GetTypes()
assert 1 == len(types)

type = types[0]
assert "MyModule" == type.Name

entry = result.GeneratedAssembly.EntryPoint
assert entry is not null
assert "Main" == entry.Name
assert Attribute.GetCustomAttribute(entry, STAThreadAttribute) is not null

members = Hash((member.Name, member) for member in type.GetMembers())
assert "Version" in members
assert "HeyBooWhatYouReDoing" in members
assert "Main" in members
assert "Spam" in members


