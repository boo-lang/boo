import System
import System.IO
import System.Reflection
import BooCompiler.Tests from BooCompiler.Tests
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines

assemblyKeyFile = Path.Combine(BooTestCaseUtil.TestCasesPath, "../test.snk").Replace("\\", "//")

code = """
import System.Reflection

[assembly: AssemblyVersion('1.0.0.0')]
[assembly: AssemblyKeyFile('${assemblyKeyFile}')]
"""

compiler = BooCompiler()
compiler.Parameters.Input.Add(StringInput("test", code))
compiler.Parameters.OutputType = CompilerOutputType.Library
compiler.Parameters.Pipeline = CompileToMemory()

result = compiler.Run()
assert len(result.Errors) == 0, result.Errors.ToString(true)

name = result.GeneratedAssembly.GetName()
assert "test" == name.Name
assert "1.0.0.0" == name.Version.ToString()
assert name.GetPublicKeyToken() is not null, "AssemblyKeyFileAttribute must result in strongly named assembly"

