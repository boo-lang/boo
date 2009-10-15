using System;

namespace BooCompiler.Tests.SupportingClasses
{
	public class OverrideBoolOperator
	{
		public static implicit operator bool(OverrideBoolOperator instance)
		{
			Console.WriteLine("OverrideBoolOperator.operator bool");
			return false;
		}
	}
}