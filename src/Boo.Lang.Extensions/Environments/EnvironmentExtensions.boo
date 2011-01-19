namespace Boo.Lang.Environments

import System.Runtime.CompilerServices

[Extension]
[CompilerGlobalScope]
static class EnvironmentExtensions:

	[Extension] def Run(this as IEnvironment, action as System.Action):
	"""
	Runs the given action in this environment.
	"""
		ActiveEnvironment.With(this, { action() })
		
	[Extension] def Invoke[of TResult](this as IEnvironment, function as System.Func of TResult):
	"""
	Invokes the given function in this environment and returns the result.
	"""
		invoker = FunctionInvoker of TResult(Function: function)
		ActiveEnvironment.With(this, invoker.Action)
		return invoker.Result
		
	internal class FunctionInvoker[of TResult]:
	"""workaround for compiler not supporting closures capturing generic parameters"""
		public Function as System.Func of TResult
		public Result as TResult
		def Action():
			Result = Function()
