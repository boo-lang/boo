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

