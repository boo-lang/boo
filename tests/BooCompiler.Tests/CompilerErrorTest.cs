using System;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using NUnit.Framework;

namespace BooCompiler.Tests
{
	[TestFixture]
	public class CompilerErrorTest
	{
		[Test]
		public void VerboseFlagTrueIsRespectedByNestedCompilerError()
		{
			var error = ProduceNestedCompilerError();
			StringAssert.Contains("Bar", error.ToString(true));
		}

		[Test]
		public void VerboseFlagFalseIsRespectedByNestedCompilerError()
		{
			var error = ProduceNestedCompilerError();
			Assert.IsFalse(error.ToString().Contains("Bar"));
		}

		private CompilerError ProduceNestedCompilerError()
		{
			try
			{
				ThrowNestedCompilerError();
			}
			catch (CompilerError e)
			{
				return e;
			}
			Assert.Fail("CompilerError exception expected.");
			return null;
		}

		private void ThrowNestedCompilerError()
		{
			WrappingInCompilerError(Foo);
		}

		private void Foo()
		{
			WrappingInCompilerError(Bar);
		}

		private void Bar()
		{
			throw new CompilerError("error");
		}

		private void WrappingInCompilerError(Action action)
		{
			try
			{
				action();
			}
			catch (Exception e)
			{
				throw new CompilerError(LexicalInfo.Empty, e);
			}
		}
	}
}
