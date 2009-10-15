using System;

namespace BooCompiler.Tests.SupportingClasses
{
	public class OverrideEqualityOperators
	{
		public static bool operator==(OverrideEqualityOperators lhs, OverrideEqualityOperators rhs)
		{
			if (Object.Equals(null, lhs))
			{
				Console.WriteLine("lhs is null");
			}
			
			if (Object.Equals(null, rhs))
			{
				Console.WriteLine("rhs is null");
			}
			return true;
		}
		
		public static bool operator!=(OverrideEqualityOperators lhs, OverrideEqualityOperators rhs)
		{
			if (Object.Equals(null, lhs))
			{
				Console.WriteLine("lhs is null");
			}
			
			if (Object.Equals(null, rhs))
			{
				Console.WriteLine("rhs is null");
			}
			return false;
		}
		
		// Just to remove the warnings; non-functional
		public override bool Equals(object obj)
		{
			throw new NotImplementedException("This override is for testing purposes only.");
		}
		
		// Just to remove the warnings; non-functional
		public override int GetHashCode()
		{
			throw new NotImplementedException("This override is for testing purposes only.");
		}
	}
}