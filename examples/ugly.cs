using System;
using System.Collections;

public class App
{
	public static long value(long x, long y, long z)
	{
		return (long)(Math.Pow(2, x)*Math.Pow(3, y)*Math.Pow(5, z));
	}
	
	public static long ugly(int max)
	{		
		ArrayList uglies = new ArrayList();
		long counter = 1;
		Hashtable dict = new Hashtable();
		dict[counter] = new long[] { 0, 0, 0 };		
		
		while (uglies.Count < max)
		{	
			uglies.Add(counter);
			long[] array = (long[])dict[counter];
			long x = array[0];
			long y = array[1];
			long z = array[2];			
			
			dict[value(x+1, y, z)] = new long[] { x+1, y, z };
			dict[value(x, y+1, z)] = new long[] { x, y+1, z };
			dict[value(x, y, z+1)] = new long[] { x, y, z+1 };
			
			dict.Remove(counter);
			
			ArrayList keys = new ArrayList(dict.Keys);
			keys.Sort();
			
			counter = (long)keys[0];
		}
	
		return (long)uglies[uglies.Count-1];
	}
	
	public static void Main()
	{
		int iter = 1500;
		DateTime start = DateTime.Now;
		long uvalue = 0;
		for (int i=0; i<10; ++i)
		{
			uvalue = ugly(iter);
		}
		DateTime end = DateTime.Now;
		Console.WriteLine("{0} ugly value = {1} in {2}ms", iter, uvalue, (end-start).TotalMilliseconds);
	}
}

