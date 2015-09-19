"""
class AbstractInterpreter:

	def Eval():
		return "input\$((++_inputId))"

	def EvalCompileUnit(cu as CompileUnit):
		if ((not hasStatements) and (not hasMembers)) and (0 == len(module.Imports)):
			return CompilerContext(cu)

"""
class AbstractInterpreter:

	def Eval():
		return "input${++_inputId}"
		
	def EvalCompileUnit(cu as CompileUnit):
		if ((not hasStatements) and
			(not hasMembers) and
			0 == len(module.Imports)):
			return CompilerContext(cu)
		
