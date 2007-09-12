using System;
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Parser
{
	public class OperatorParser
	{
		public static BinaryOperatorType ParseComparison(string op)
		{
			switch (op)
			{
				case "<=": return BinaryOperatorType.LessThanOrEqual;
				case ">=": return BinaryOperatorType.GreaterThanOrEqual;
				case "==": return BinaryOperatorType.Equality;
				case "!=": return BinaryOperatorType.Inequality;
				case "=~": return BinaryOperatorType.Match;
				case "!~": return BinaryOperatorType.NotMatch;
			}
			throw new ArgumentException(op, "op");
		}

		public static BinaryOperatorType ParseAssignment(string op)
		{
			switch (op)
			{
				case "=": return BinaryOperatorType.Assign;
				case "+=": return BinaryOperatorType.InPlaceAddition;
				case "-=": return BinaryOperatorType.InPlaceSubtraction;
				case "/=": return BinaryOperatorType.InPlaceDivision;
				case "*=": return BinaryOperatorType.InPlaceMultiply;
				case "^=": return BinaryOperatorType.InPlaceExclusiveOr;
			}
			throw new ArgumentException(op, "op");
		}
	}
}
