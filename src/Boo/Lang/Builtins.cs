using System;

namespace Boo.Lang
{
	/// <summary>
	/// boo language builtin functions.
	/// </summary>
	public class Builtins
	{	
		public static void print(string s)
		{
			Console.WriteLine(s);
		}

		public static string gets()
		{
			return Console.ReadLine();
		}

		public static string prompt(string message)
		{
			Console.Write(message);
			return Console.ReadLine();
		}
		
		public static void assert(string message, bool condition)
		{
			throw new System.NotImplementedException();
		}
		
		public static void assert(bool condition)
		{
			throw new System.NotImplementedException();
		}
	}
}
