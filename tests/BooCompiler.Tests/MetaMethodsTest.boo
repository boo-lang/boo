namespace BooCompiler.Tests

import System.Reflection
import Boo.Lang
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.MetaProgramming
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Environments
import Boo.Lang.Parser
import NUnit.Framework
import BooCompiler.Tests.StringExtensions

[TestFixture]
class MetaMethodsTest:
	[Test]
	def CanSeeArgumentTypes():
		code = string.Format("""
			import {0}
			def test():
				assert typesOf(42, 42L, '42') == (int, long, string)
			""", typeof(MetaMethods).FullName.Replace(char('+'), char('.')))
		assembly = Compile("Test", code)
		assembly.GetType("TestModule").GetMethod("test").Invoke(null, null)

	public static class MetaMethods:
		[MetaMethodsTest.SomeOtherAttribute]
		[Meta(ResolveArgs: true)]
		public def typesOf(*args as (Expression)) as Expression:
			result = ArrayLiteralExpression()
			for arg in args:
				result.Items.Add(CodeBuilder.CreateTypeofExpression(arg.ExpressionType))
			return result

		private CodeBuilder as BooCodeBuilder:
			get: return My[of BooCodeBuilder].Instance

	public class SomeOtherAttribute(System.Attribute):
		pass

	private static def Compile(fileName as string, code as string) as Assembly:
		return Compilation.compile(Parse(fileName, code), typeof(MetaMethods).Assembly)

	private static def Parse(fileName as string, code as string) as CompileUnit:
		return BooParser.ParseString(fileName, code.ReIndent())
