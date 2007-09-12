using System;
using System.Globalization;
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Parser
{
	public class PrimitiveParser
	{
		public static TimeSpan ParseTimeSpan(string text)
		{
			if (text.EndsWith("ms"))
			{
				return TimeSpan.FromMilliseconds(
					ParseDouble(text.Substring(0, text.Length - 2)));
			}

			char last = text[text.Length - 1];
			double value = ParseDouble(text.Substring(0, text.Length - 1));
			switch (last)
			{
				case 's': return TimeSpan.FromSeconds(value);
				case 'h': return TimeSpan.FromHours(value);
				case 'm': return TimeSpan.FromMinutes(value);
				case 'd': return TimeSpan.FromDays(value);
			}
			throw new ArgumentException(text, "text");
		}

		public static double ParseDouble(string s)
		{
			return ParseDouble(s, false);
		}

		public static double ParseDouble(string s, bool isSingle)
		{
			double val;
			if (isSingle)
			{
				val = float.Parse(s, NumberStyles.Float, CultureInfo.InvariantCulture);
			}
			else
			{
				val = double.Parse(s, NumberStyles.Float, CultureInfo.InvariantCulture);
			}
			return val;
		}

		public static IntegerLiteralExpression ParseIntegerLiteralExpression(antlr.IToken token, string s, bool asLong)
		{
			const string HEX_PREFIX = "0x";
			
			NumberStyles style = NumberStyles.Integer | NumberStyles.AllowExponent;
			int hex_start = s.IndexOf(HEX_PREFIX);
			bool negative = false;

			if (hex_start >= 0)
			{
				if (s.StartsWith("-"))
				{
					negative = true;
				}
				s = s.Substring(hex_start + HEX_PREFIX.Length);
				style = NumberStyles.HexNumber;
			}

			long value = long.Parse(s, style, CultureInfo.InvariantCulture);
			if (negative) //negative hex number
			{
				value *= -1;
			}
			return new IntegerLiteralExpression(SourceLocationFactory.ToLexicalInfo(token), value, asLong || (value > int.MaxValue || value < int.MinValue));
		}

	}
}
