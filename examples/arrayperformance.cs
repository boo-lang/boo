using System;
using Boo.Lang;

public class App
{
	public static void Main()
	{
		Test();
		Test();
		Test();
	}
	
	private static void Test()
	{
		const int items = 2000000;

		object[] array = (object[])new List(Builtins.range(items)).ToArray(typeof(object));
		
		List collect = new List();

		DateTime start = DateTime.Now;
		foreach (int i in Builtins.range(items))
		{
			collect.Add(array[i]);
		}
		TimeSpan elapsed = DateTime.Now.Subtract(start);

		Console.WriteLine("{0} elapsed.", elapsed.TotalMilliseconds);
	}
}
