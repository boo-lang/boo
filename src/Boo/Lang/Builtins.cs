using System;
using System.Collections;

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
		
		//[EnumeratorItemType(Type.GetType("System.Object[]"))]
		public static IEnumerable enumerate(object enumerable)
		{
			if (null == enumerable)
			{
				throw new ArgumentNullException("enumerable");
			}
			
			IEnumerable iterator = enumerable as IEnumerable;
			if (null == iterator)
			{
				throw new ArgumentException(GetString("ArgumentNotEnumerable"), "enumerable");
			}
			return new EnumerateEnumerator(iterator.GetEnumerator());
		}
		
		public static void assert(string message, bool condition)
		{
			throw new System.NotImplementedException();
		}
		
		public static void assert(bool condition)
		{
			throw new System.NotImplementedException();
		}
		
		private class EnumerateEnumerator : IEnumerator, IEnumerable
		{
			int _index = -1;
			
			IEnumerator _enumerator;
			
			public EnumerateEnumerator(IEnumerator enumerator)
			{
				if (null == enumerator)
				{
					throw new ArgumentNullException("enumerator");
				}
				_enumerator = enumerator;
			}
			
			public void Reset()
			{
				_index = -1;
				_enumerator.Reset();
			}
			
			public bool MoveNext()
			{				
				if (_enumerator.MoveNext())
				{
					++_index;
					return true;
				}
				return false;
			}
			
			public object Current
			{
				get
				{
					return new object[2] { _index, _enumerator.Current };
				}
			}
			
			public IEnumerator GetEnumerator()
			{
				return this;
			}
		}
		
		private static string GetString(string name)
		{
			return Boo.ResourceManager.GetString(name);
		}
	}
}
