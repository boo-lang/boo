"""
regular method
generic method
"""
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines
import System.Reflection

def compile(name as string, code as string, reference as Assembly):	
	compiler = BooCompiler()
	compiler.Parameters.OutputType = CompilerOutputType.Library
	compiler.Parameters.References.Add(reference) if reference
	compiler.Parameters.Input.Add(StringInput(name, code))
	compiler.Parameters.Pipeline = CompileToMemory()
	return compiler.Run()

code1 = """	
class Class:
  static def Method(arg as int):
    return "regular method"

  static def Method[of T](arg as int):
    return "generic method"
"""

compiled1 = compile("<lib>", code1, null)

print compiled1.Errors.ToString() if compiled1.Errors.Count
print compiled1.Warnings.ToString() if compiled1.Warnings.Count

code2 = """
class Default:
  static def Main():
    print Class.Method(42)
    print Class.Method[of single](42)
"""
compiled2 = compile("<code>", code2, compiled1.GeneratedAssembly)

print compiled2.Errors.ToString() if compiled2.Errors.Count
print compiled2.Warnings.ToString() if compiled2.Warnings.Count

compiled2.GeneratedAssembly.GetType('Default').GetMethod('Main').Invoke(null, (,))