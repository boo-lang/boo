using System;

namespace BooCompiler.Tests.SupportingClasses
{
	public class ConditionalClass
	{
		[System.Diagnostics.Conditional("BOO_COMPILER_TESTS_NOT_DEFINED_CONDITIONAL")]
		public static void PrintNothing(int i)
		{
			Console.WriteLine(i);
		}

		[System.Diagnostics.Conditional("BOO_COMPILER_TESTS_DEFINED_CONDITIONAL")]
		public static void PrintSomething(string s)
		{
			Console.WriteLine(s);
		}

		[System.Diagnostics.Conditional("BOO_COMPILER_TESTS_NOT_DEFINED_CONDITIONAL")]
		public static void PrintNoT<T>(T s)
		{
			Console.WriteLine(s);
		}

		[System.Diagnostics.Conditional("BOO_COMPILER_TESTS_DEFINED_CONDITIONAL")]
		public static void PrintSomeT<T>(T s)
		{
			Console.WriteLine(s);
		}
	}
}