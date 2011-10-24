import System
import System.Reflection
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines

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
assert len(result.Errors) == 0, result.Errors.ToString(true)

asm = result.GeneratedAssembly
title as AssemblyTitleAttribute = Attribute.GetCustomAttribute(asm, AssemblyTitleAttribute)
assert title is not null, "AssemblyTitleAttribute"
assert "foo" == title.Title

description as AssemblyDescriptionAttribute = Attribute.GetCustomAttribute(asm, AssemblyDescriptionAttribute)
assert description is not null, "AssemblyDescriptionAttribute"
assert "bar" == description.Description

key = asm.GetName().GetPublicKeyToken()
assert key is null or 0 == len(key)


