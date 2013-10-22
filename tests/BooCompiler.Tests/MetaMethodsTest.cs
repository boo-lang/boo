using System.Reflection;
using Boo.Lang;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.MetaProgramming;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Environments;
using Boo.Lang.Parser;
using NUnit.Framework;

namespace BooCompiler.Tests
{
	[TestFixture]
	public class MetaMethodsTest
	{
		[Test]
		public void CanSeeArgumentTypes()
		{
			var code = string.Format(@"
			import {0}
			def test():
				assert typesOf(42, 42L, '42') == (int, long, string)
			", typeof(MetaMethods).FullName.Replace('+', '.'));
			var assembly = Compile("Test", code);
			assembly.GetType("TestModule").GetMethod("test").Invoke(null, null);
		}

		public static class MetaMethods
		{
			[SomeOtherAttribute]
			[Meta(ResolveArgs = true)]
			public static Expression typesOf(params Expression[] args)
			{
				var result = new ArrayLiteralExpression();
				foreach (var arg in args)
					result.Items.Add(CodeBuilder.CreateTypeofExpression(arg.ExpressionType));
				return result;
			}

			private static BooCodeBuilder CodeBuilder
			{
				get { return My<BooCodeBuilder>.Instance; }
			}
		}

		public class SomeOtherAttribute : System.Attribute {}

		private static Assembly Compile(string fileName, string code)
		{
			return Compilation.compile(Parse(fileName, code), typeof(MetaMethods).Assembly);
		}

		private static CompileUnit Parse(string fileName, string code)
		{
			return BooParser.ParseString(fileName, code.ReIndent());
		}
	}
}
