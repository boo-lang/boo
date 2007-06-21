import System
import System.Reflection
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines
import NUnit.Framework

code = """
print(sqrt(4))
print(SunShip.Ascent())
"""

compiler = BooCompiler()
compiler.Parameters.References.Add(Assembly.LoadWithPartialName("BooModules"))
compiler.Parameters.Input.Add(StringInput("test", code))
compiler.Parameters.Pipeline = Compile()

result = compiler.Run()
Assert.Fail(result.Errors.ToString(true)) if len(result.Errors)
