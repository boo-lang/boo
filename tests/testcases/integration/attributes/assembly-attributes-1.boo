import System
import System.Reflection
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines
import NUnit.Framework

code = """
import System.Reflection

[assembly: AssemblyTitle('foo')]
[assembly: AssemblyDescription('bar')]
"""

compiler = BooCompiler()
compiler.Parameters.Input.Add(StringInput("test", code))
compiler.Parameters.OutputType = CompilerOutputType.Library
compiler.Parameters.Pipeline = CompileToMemory()

result = compiler.Run()
Assert.Fail(result.Errors.ToString(true)) if len(result.Errors)

asm = result.GeneratedAssembly
title as AssemblyTitleAttribute = Attribute.GetCustomAttribute(asm, AssemblyTitleAttribute)
Assert.IsNotNull(title, "AssemblyTitleAttribute")
Assert.AreEqual("foo", title.Title)

description as AssemblyDescriptionAttribute = Attribute.GetCustomAttribute(asm, AssemblyDescriptionAttribute)
Assert.IsNotNull(description, "AssemblyDescriptionAttribute")
Assert.AreEqual("bar", description.Description)

key = asm.GetName().GetPublicKeyToken()
assert key is null or 0 == len(key)


