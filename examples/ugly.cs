#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


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

