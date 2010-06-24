using System;
using System.Text.RegularExpressions;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;

namespace Nih
{
	/// <summary>
	/// 
	/// Simple recursive descent parser for the nih language which has only a single statement:
	/// 
	///     say (times)?
	/// 
	/// </summary>
	public class Parser
	{
		public static Module ParseModule(string code)
		{
			var parser = new Parser(code);
			return parser.Parse();
		}

		private string _code;
		private int _position;

		private Parser(string code)
		{
			_code = code;
		}

		private Module Parse()
		{
			var module = new Module();
			var globals = module.Globals;
			while (_position < _code.Length)
			{
				var expression = ParseNextExpression();
				if (expression == null)
					break;
				globals.Add(expression);
			}
			return module;
		}

		private Expression ParseNextExpression()
		{
			var sayMatch = Match(SayPattern);
			var integerMatch = Match(IntegerPattern);
			return new MethodInvocationExpression(
				new ReferenceExpression(sayMatch.Value.Trim()),
				new IntegerLiteralExpression(long.Parse(integerMatch.Value.Trim())));
		}

		private Match Match(Regex pattern)
		{
			var m = pattern.Match(_code, _position);
			if (!m.Success)
				throw new CompilerError(LexicalInfo.Empty, "Expecting '" + pattern + "'");
			_position += m.Length;
			return m;
		}

		static Regex SayPattern = new Regex(@"\s*say");
		static Regex IntegerPattern = new Regex(@"\s*\d+");
	}
}
