#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// mailto:rbo@acm.org
#endregion

namespace Boo.Lang
{
	using System;	
	using System.Collections;
	using System.Text;

	/// <summary>
	/// boo language builtin functions.
	/// </summary>
	public class Builtins
	{	
		public static void print(string s)
		{
			Console.WriteLine(s);
		}
		
		public static void print(object o)
		{
			Console.WriteLine(o);
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
		
		public static string join(IEnumerable enumerable, string separator)
		{
			StringBuilder sb = new StringBuilder();			
			foreach (object item in enumerable)
			{
				if (sb.Length>0) { sb.Append(separator); }
				sb.Append(item);
			}
			return sb.ToString();
		}
		
		public static string join(IEnumerable enumerable)
		{
			return join(enumerable, " ");
		}
		
		public static IEnumerable map(ICallable function, object enumerable)
		{
			if (null == function)
			{
				throw new ArgumentNullException("function");
			}
			if (null == enumerable)
			{
				throw new ArgumentNullException("enumerable");
			}
			return new MapEnumerator(function, GetEnumerator(enumerable));
		}
		
		public static object[] tuple(IEnumerable enumerable)
		{
			return (object[])tuple(typeof(object), enumerable);
		}
		
		public static Array tuple(Type elementType, IEnumerable enumerable)
		{
			if (null == enumerable)
			{
				throw new ArgumentNullException("enumerable");
			}
			if (null == elementType)
			{
				throw new ArgumentNullException("elementType");
			}			
			return new List(enumerable).ToArray(elementType); 
		}
		
		public static Array tuple(Type elementType, int length)
		{
			if (null == elementType)
			{
				throw new ArgumentNullException("elementType");
			}
			return Array.CreateInstance(elementType, length);
		}
		
		//[EnumeratorItemType(Type.GetType("System.Object[]"))]
		public static IEnumerable enumerate(object enumerable)
		{			
			return new EnumerateEnumerator(GetEnumerator(enumerable));
		}
		
		public static IEnumerable range(int max)
		{
			if (max < 0)
			{
				throw new ArgumentOutOfRangeException("max");
			}
			
			return new RangeEnumerator(0, max, 1);
		}
		
		public static IEnumerable range(int min, int max)
		{
			if (max < min || max < 0)
			{
				throw new ArgumentOutOfRangeException("max");
			}
			return new RangeEnumerator(min, max, 1);
		}
		
		public static IEnumerable range(int min, int max, int step)
		{
			if (step < 0)
			{
				throw new ArgumentNullException("step");
			}
			
			if (max < min || max < 0)
			{
				throw new ArgumentNullException("max");
			}
			return new RangeEnumerator(min, max, step);
		}
		
		//[EnumeratorItemType(Type.GetType("System.Object[]"))]
		public static IEnumerable zip(object first, object second)
		{
			return new ZipEnumerator(GetEnumerator(first),
									GetEnumerator(second));
		}
		
		public static void assert(string message, bool condition)
		{
			throw new System.NotImplementedException();
		}
		
		public static void assert(bool condition)
		{
			throw new System.NotImplementedException();
		}
		
		private class MapEnumerator : IEnumerator, IEnumerable
		{
			IEnumerator _enumerator;
			
			ICallable _function;
			
			object _current;
			
			object[] _arguments = new object[1];
			
			public MapEnumerator(ICallable function, IEnumerator enumerator)
			{
				_function = function;
				_enumerator = enumerator;
			}
			
			public void Reset()
			{
				_enumerator.Reset();
			}
			
			public bool MoveNext()
			{
				if (_enumerator.MoveNext())
				{
					_arguments[0] = _enumerator.Current;
					_current = _function.Call(_arguments);
					return true;
				}
				return false;
			}
			
			public object Current
			{
				get
				{
					return _current;
				}
			}
			
			public IEnumerator GetEnumerator()
			{
				return this;
			}
		}
		
		private class ZipEnumerator : IEnumerator, IEnumerable
		{
			IEnumerator[] _enumerators;
			
			public ZipEnumerator(params IEnumerator[] enumerators)
			{
				_enumerators = enumerators;
			}
			
			public void Reset()
			{
				for (int i=0; i<_enumerators.Length; ++i)
				{
					_enumerators[i].Reset();
				}
			}
			
			public bool MoveNext()
			{
				for (int i=0; i<_enumerators.Length; ++i)
				{
					if (!_enumerators[i].MoveNext())
					{
						return false;
					}
				}
				return true;
			}
			
			public object Current
			{
				get
				{
					object[] current = new object[_enumerators.Length];
					for (int i=0; i<current.Length; ++i)
					{
						current[i] = _enumerators[i].Current;
					}
					return current;
				}
			}
			
			public IEnumerator GetEnumerator()
			{
				return this;
			}
		}
		
		private class RangeEnumerator : IEnumerator, IEnumerable
		{
			int _index;
			int _min;
			int _max;
			int _step;
			
			public RangeEnumerator(int min, int max, int step)
			{
				_min = min;
				_index = min-step;
				_max = max-step;
				_step = step;
			}
			
			public void Reset()
			{
				_index = _min-_step;
			}
			
			public bool MoveNext()
			{
				if (_index < _max)
				{	
					_index += _step;
					return true;
				}
				return false;
			}
			
			public object Current
			{
				get
				{
					return _index;
				}
			}
			
			public IEnumerator GetEnumerator()
			{
				return this;
			}
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
		
		private static IEnumerator GetEnumerator(object enumerable)
		{
			return RuntimeServices.GetEnumerable(enumerable).GetEnumerator();
		}
	}
}
