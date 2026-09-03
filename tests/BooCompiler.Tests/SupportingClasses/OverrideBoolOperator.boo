namespace BooCompiler.Tests.SupportingClasses

import System

class OverrideBoolOperator:
	public static def op_Implicit(instance as OverrideBoolOperator) as bool:
		Console.WriteLine("OverrideBoolOperator.operator bool")
		return false
