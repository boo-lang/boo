using System;
using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;

public class App
{
	const int Iterations = 100000;
	
	public static void Main()
	{
		Console.WriteLine(ParseInt("321"));
		Console.WriteLine(string.Format("{0}, {1}, {2}, {3}, {0}, {1}", "foo", "bar", "baz", "bag"));
		Console.WriteLine(Format("{0}, {1}, {2}, {3}, {0}, {1}", "foo", "bar", "baz", "bag"));
		
		DateTime start = DateTime.Now;
		for (int i=0; i<Iterations; ++i)
		{
			string.Format("{0}, {1}, {2}, {3}, {0}, {1}", "foo", "bar", "baz", "bag");
		}
		DateTime stop = DateTime.Now;
		Console.WriteLine("string.Format performance: {0}", stop-start);
		
		start = DateTime.Now;
		for (int i=0; i<Iterations; ++i)
		{
			Format("{0}, {1}, {2}, {3}, {0}, {1}", "foo", "bar", "baz", "bag");
		}
		stop = DateTime.Now;
		Console.WriteLine("regex performance: {0}", stop-start);
		
		start = DateTime.Now;
		for (int i=0; i<Iterations; ++i)
		{
			StringBuilder builder = new StringBuilder(56);
			builder.Append("foo");
			builder.Append(", ");
			builder.Append("bar");
			builder.Append(", ");
			builder.Append("baz");
			builder.Append(", ");
			builder.Append("bag");
			builder.Append(", ");
			builder.Append("foo");
			builder.Append(", ");
			builder.Append("bar");
			builder.ToString();
		}
		stop = DateTime.Now;
		Console.WriteLine("builder performance: {0}", stop-start);
	}
	
	static Regex _re = new Regex(@"\{(\d+)\}", RegexOptions.Compiled|RegexOptions.CultureInvariant);
	
	public static int ParseInt(string value)
	{
		int intValue=0;
		int multiplier=1;
		for (int i=value.Length; i>0; --i)
		{			
			int charValue = ((int)value[i-1]) - (int)'0';
			intValue += (charValue*multiplier);
			multiplier *= 10;
		}
		return intValue;
	}
	
	public static string Format(string template, params object[] args)
	{
		StringBuilder builder = new StringBuilder(template.Length*2);
		
		int current = 0;
		Match m = _re.Match(template);
		while (m.Success)
		{
			//Console.WriteLine("current: {0}, m.Index: {1}, m.Length: {2}", current, m.Index, m.Length);
			builder.Append(template, current, m.Index-current);
			
			int index = ParseInt(m.Groups[1].Value);
			builder.Append(args[index]);
			
			current = m.Index + m.Length;			
			m = m.NextMatch();
		}
		
		builder.Append(template, current, template.Length-current);
		
		return builder.ToString();
	}
}

