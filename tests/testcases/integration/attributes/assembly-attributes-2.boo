import System
import System.IO
import System.Reflection
import BooCompiler.Tests
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines
import NUnit.Framework

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
Assert.Fail(result.Errors.ToString(true)) if len(result.Errors)

name = result.GeneratedAssembly.GetName()
Assert.AreEqual("test", name.Name)
Assert.AreEqual("1.0.0.0", name.Version.ToString())
Assert.IsNotNull(name.GetPublicKeyToken(), "AssemblyKeyFileAttribute must result in strongly named assembly")

