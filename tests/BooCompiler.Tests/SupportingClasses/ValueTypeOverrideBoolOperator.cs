using System;

namespace BooCompiler.Tests.SupportingClasses
{
	public struct ValueTypeOverrideBoolOperator
	{
		public static implicit operator bool(ValueTypeOverrideBoolOperator instance)
		{
			Console.WriteLine("ValueTypeOverrideBoolOperator.operator bool");
			return false;
		}
	}
}