//#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// 
// Contact Information
//
// mailto:rbo@acm.org
//#endregion

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

