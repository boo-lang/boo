import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO

def run(pipelineName, code):
	compiler = BooCompiler()
	compiler.Parameters.Input.Add(StringInput("<code>", code))
	compiler.Parameters.Pipeline.Load(pipelineName)
	result = compiler.Run()
	print(join(result.Errors, "\n")) if len(result.Errors)

code = "print('Hello!')"

run("boo", code)
run("booi", code)
			
