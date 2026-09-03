namespace BooCompiler.Tests.SupportingClasses

import System

struct ValueTypeOverrideBoolOperator:
	public static def op_Implicit(instance as ValueTypeOverrideBoolOperator) as bool:
		Console.WriteLine("ValueTypeOverrideBoolOperator.operator bool")
		return false
