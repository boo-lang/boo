using System;
using System.Diagnostics;

public class Person
{
	string _name;
	
	public string Name
	{
		get
		{
			return _name;
		}
		
		set
		{
			_name = value;
		}
	}
}

public class Application
{
	public static void use(Person[] p)
	{
		Debug.Assert(5 == p.Length);
	}
	
	public static void run()
	{
		foreach (object item in Boo.Lang.Builtins.range(100000))
		{
			Person[] array = new Person[5];
			array[0] = new Person(); array[0].Name = "a name";
			array[1] = new Person(); array[1].Name = "a name";
			array[2] = new Person(); array[2].Name = "a name";
			array[3] = new Person(); array[3].Name = "a name";
			array[4] = new Person(); array[4].Name = "a name";
			use(array);
		}
	}
	
	public static void Main()
	{	
		DateTime start = DateTime.Now;
		
		foreach (object item in Boo.Lang.Builtins.range(10))
		{
			run();
		}
		
		Console.WriteLine("elapsed: {0}", DateTime.Now-start);
	}
}
