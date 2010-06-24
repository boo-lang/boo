using System;
using System.IO;
using NUnit.Framework;

namespace Nih.Tests
{
	[TestFixture]
	public class CompilerTest
	{
		[Test]
		public void SayStatement()
		{
			var assembly = Nih.Compiler.CompileString("say 3");
			var output = CapturingStandardOutput(() => assembly.EntryPoint.Invoke(null, new object[] { null }));
			Assert.AreEqual("nih! nih! nih!", output.Trim());
		}

		private string CapturingStandardOutput(System.Action action)
		{
			var oldOut = Console.Out;
			var newOut = new StringWriter();
			Console.SetOut(newOut);
			try
			{
				action();
			}
			finally
			{
				Console.SetOut(oldOut);
			}
			return newOut.ToString();
		}
	}
}
