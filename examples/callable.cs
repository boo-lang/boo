#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
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

using System;
using System.Collections;

public interface ICallable
{
	object Call(object[] args);
}

public class App
{
	public static void eachInterface(ICallable callable, IEnumerable enumerable)
	{
		object[] args = new object[1];
		foreach (object item in enumerable)
		{
			args[0] = item;
			callable.Call(args);
		}
	}
	
	public static void eachDynamicDelegate(Delegate function, IEnumerable enumerable)
	{		
		object[] args = new object[1];
		foreach (object item in enumerable)
		{
			args[0] = item;
			function.DynamicInvoke(args);
		}
	}
	
	public static void eachDelegate(_long_long_delegate function, IEnumerable enumerable)
	{		
		foreach (long l in enumerable)
		{
			function(l);
		}
	}
	
	public static void eachDelegateObject(_object_object_delegate function, IEnumerable enumerable)
	{		
		foreach (object item in enumerable)
		{
			function(item);
		}		
	}
	
	public static long square(long value)
	{
		return value*value;
	}
	
	public static object _square_adaptor(object value)
	{
		return square((long)value);
	}
	
	public delegate long _long_long_delegate(long value);
	
	public delegate object _object_object_delegate(object value);
	
	class _square_Callable : ICallable
	{
		public object Call(object[] args)
		{
			return App.square((long)args[0]);
		}
	}
	
	class _Callable_long_long_delegate
	{
		ICallable _callable;
		object[] _args;
		
		public _Callable_long_long_delegate(ICallable callable)
		{
			_callable = callable;
			_args = new object[1];
		}
		
		public long Call(long value)
		{
			_args[0] = value;
			return (long)_callable.Call(_args);
		}
	}
	
	class _long_long_delegate_Callable : ICallable
	{
		_long_long_delegate _delegate;
		
		public _long_long_delegate_Callable(_long_long_delegate d)
		{
			_delegate = d;
		}
		
		public object Call(object[] args)
		{
			return _delegate((long)args[0]);
		}
	}
	
	public static void Main()
	{
		const int count = 1000000;
		ArrayList l = new ArrayList(count);
		for (long i=0; i<count; ++i)
		{
			l.Add(i);
		}
		
		for (int i=0; i<3; ++i)
		{
			DateTime start = DateTime.Now;
			eachDelegate(new _long_long_delegate(square), l);
			TimeSpan elapsed = DateTime.Now - start;
			Console.WriteLine("static delegate version took {0}ms", elapsed.TotalMilliseconds);
			
			start = DateTime.Now;
			eachDynamicDelegate(new _long_long_delegate(square), l);
			elapsed = DateTime.Now - start;
			Console.WriteLine("dynamic delegate version took {0}ms", elapsed.TotalMilliseconds);
			
			start = DateTime.Now;
			eachDelegateObject(new _object_object_delegate(_square_adaptor), l);
			elapsed = DateTime.Now - start;
			Console.WriteLine("delegate to delegate adaptation version took {0}ms", elapsed.TotalMilliseconds);
			
			start = DateTime.Now;
			eachInterface(new _square_Callable(), l);
			elapsed = DateTime.Now - start;
			Console.WriteLine("interface version took {0}ms", elapsed.TotalMilliseconds);
			
			start = DateTime.Now;
			eachInterface(new _long_long_delegate_Callable(new _long_long_delegate(square)), l);
			elapsed = DateTime.Now - start;
			Console.WriteLine("interface adaptation for delegate version took {0}ms", elapsed.TotalMilliseconds);
			
			start = DateTime.Now;
			eachDelegate(new _long_long_delegate(new _Callable_long_long_delegate(new _square_Callable()).Call), l);
			elapsed = DateTime.Now - start;
			Console.WriteLine("delegate adaptation for interface version took {0}ms", elapsed.TotalMilliseconds);
			
			Console.WriteLine("---------------------------------------------------------------");
		}
	}
}
