using System;

namespace BooCompiler.Tests.SupportingClasses
{
	public class Disposable : System.IDisposable
	{
		public Disposable()
		{
			Console.WriteLine("Disposable.constructor");
		}
		
		public void foo()
		{
			Console.WriteLine("Disposable.foo");
		}
		
		void System.IDisposable.Dispose()
		{
			Console.WriteLine("Disposable.Dispose");
		}
	}
}