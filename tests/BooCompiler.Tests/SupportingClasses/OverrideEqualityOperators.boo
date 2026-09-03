namespace BooCompiler.Tests.SupportingClasses

import System

class OverrideEqualityOperators:
	public static def op_Equality(lhs as OverrideEqualityOperators, rhs as OverrideEqualityOperators) as bool:
		if Object.Equals(null, lhs):
			Console.WriteLine("lhs is null")

		if Object.Equals(null, rhs):
			Console.WriteLine("rhs is null")
		return true

	public static def op_Inequality(lhs as OverrideEqualityOperators, rhs as OverrideEqualityOperators) as bool:
		if Object.Equals(null, lhs):
			Console.WriteLine("lhs is null")

		if Object.Equals(null, rhs):
			Console.WriteLine("rhs is null")
		return false

	# Just to remove the warnings; non-functional
	public override def Equals(obj as object) as bool:
		raise NotImplementedException("This override is for testing purposes only.")

	# Just to remove the warnings; non-functional
	public override def GetHashCode() as int:
		raise NotImplementedException("This override is for testing purposes only.")
