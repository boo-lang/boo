"""
module0(4,17): BCE0004: Ambiguous reference 'IFoo': NS1.IFoo, NS2.IFoo.
"""
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines

def compile(references, *modules as (string)):
	compiler = BooCompiler()
	compiler.Parameters.OutputType = CompilerOutputType.Library
	compiler.Parameters.Pipeline = CompileToMemory()
	compiler.Parameters.References.Extend(references)
	for i, m in enumerate(modules):
		compiler.Parameters.Input.Add(StringInput("module${i}", m))
	return compiler.Run()

context = compile([],
"""
namespace NS1

interface IFoo:
	pass
""",
"""
namespace NS2

interface IFoo:
	pass
""")


assert len(context.Errors) == 0, context.Errors.ToString()

context = compile([context.GeneratedAssembly],
"""
import NS1

partial class A(IFoo):
	pass
""",
"""
import NS2

partial class A(IFoo):
	pass
""")

print context.Errors.ToString()
