#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

import java.util.ArrayList;
import java.util.Iterator;

class xrange implements Iterator
{
	int _max;
	int _current;
	
	public xrange(int max)
	{
		_max = max;
		_current = 0;
	}
	
	public boolean hasNext()
	{
		return _current < _max;
	}
	
	public Object next()
	{
		return new Integer(_current++);
	}
	
	public void remove()
	{
	}
}

public class arrayperformance
{
	public static final int items = 2000000;

	public static void main(String[] args)
	{
		test();
		test();
		test();
	}
	
	private static void test()
	{
		Object[] array = new Object[items];
		for (int i = 0; i < array.length; i++)
		{
			array[i] = new Object();
		}
		
		ArrayList collect = new ArrayList();
		
		Iterator i = new xrange(items);
		long start = System.currentTimeMillis();		
		while (i.hasNext())
		{
			int index = ((Integer)i.next()).intValue();
			collect.add(array[index]);
		}
		System.out.println((System.currentTimeMillis() - start) + " elapsed.");
	}
}

