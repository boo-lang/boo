namespace Boo.Microsoft.Build.Tasks

abstract class AbstractScript:
	[property(Task)]    _task as ExecBoo
	[property(Arguments)] _args = System.Collections.Generic.Dictionary[of string,string]()
	Output:
		get:
			return Task.ScriptResult
		set:
			Task.ScriptResult = value
	
	protected def PrepareArgumentDictionary():
		for arg in Task.Arguments:
			delimeter = arg.IndexOf(char(':'))
			param = arg.Substring(0, delimeter)
			val = arg.Substring(delimeter + 1)
			Arguments.Add(param, val)
	
	def print([default(string.Empty)] msg as string):
		_task.Log.LogMessage(msg)
	
	def print([default(string.Empty)] obj):
		print(obj.ToString())
	
	def warn([default(string.Empty)] msg as string):
		_task.Log.LogWarning(msg)
	
	def warn([default(string.Empty)] obj):
		warn(obj.ToString())
	
	def warn([required] ex as System.Exception):
		_task.Log.LogWarningFromException(ex)

	def error([default(string.Empty)] msg as string):
		_task.Log.LogError(msg)
	
	def error([default(string.Empty)] obj):
		error(obj.ToString())
	
	def error([required] ex as System.Exception):
		_task.Log.LogErrorFromException(ex)

	abstract def Run():
		pass