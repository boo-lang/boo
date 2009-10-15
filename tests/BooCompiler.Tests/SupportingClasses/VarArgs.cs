using System;

namespace BooCompiler.Tests.SupportingClasses
{
	public class VarArgs
	{
		public void Method()
		{
			Console.WriteLine("VarArgs.Method");
		}
		
		public void Method(params object[] args)
		{
			Console.WriteLine("VarArgs.Method({0})", Boo.Lang.Builtins.join(args, ", "));
		}
	}
}