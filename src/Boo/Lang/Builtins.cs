#region license
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
#endregion

namespace Boo.Lang
{
	using System;
	using System.Collections;
	using System.Diagnostics;
	using System.Text;
	using System.Reflection;

	/// <summary>
	/// boo language builtin functions.
	/// </summary>
	public class Builtins
	{		
		public class duck
		{
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
				if (sb.Length>0)
				{
					sb.Append(separator);
				}
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

		public static object[] array(IEnumerable enumerable)
		{
			return (object[])array(typeof(object), enumerable);
		}

		public static Array array(Type elementType, ICollection collection)
		{
			if (null == collection)
			{
				throw new ArgumentNullException("collection");
			}
			if (null == elementType)
			{
				throw new ArgumentNullException("elementType");
			}

			Array array = Array.CreateInstance(elementType, collection.Count);
			collection.CopyTo(array, 0);
			return array;
		}

		public static Array array(Type elementType, IEnumerable enumerable)
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

		public static Array array(Type elementType, int length)
		{
			if (null == elementType)
			{
				throw new ArgumentNullException("elementType");
			}
			return Array.CreateInstance(elementType, length);
		}

		public static IEnumerable iterator(object enumerable)
		{
			return RuntimeServices.GetEnumerable(enumerable);
		}

		public static Process shellp(string filename, string arguments)
		{
			Process p = new Process();
			p.StartInfo.Arguments = arguments;
			p.StartInfo.CreateNoWindow = true;
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.FileName = filename;
			p.Start();
			return p;
		}

		public static string shell(string filename, string arguments)
		{
			Process p = shellp(filename, arguments);
			string output = p.StandardOutput.ReadToEnd();
			p.WaitForExit();
			return output;
		}

		public static EnumerateEnumerator enumerate(object enumerable)
		{
			return new EnumerateEnumerator(GetEnumerator(enumerable));
		}

		public static RangeEnumerator range(int max)
		{
			if (max < 0)
			{
				throw new ArgumentOutOfRangeException("max");
			}

			return new RangeEnumerator(0, max, 1);
		}

		public static RangeEnumerator range(int begin, int end)
		{
			int step = 1;
			if (begin > end)
			{
				step = -1;
			}
			return new RangeEnumerator(begin, end, step);
		}

		public static RangeEnumerator range(int begin, int end, int step)
		{
			if (step < 0)
			{
				if (begin < end)
				{
					throw new ArgumentOutOfRangeException("step");
				}
			}
			else
			{
				if (begin > end)
				{
					throw new ArgumentOutOfRangeException("step");
				}
			}
			return new RangeEnumerator(begin, end, step);
		}

		public static ZipEnumerator zip(object first, object second)
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

		[EnumeratorItemType(typeof(object[]))]
		public class ZipEnumerator : IEnumerator, IEnumerable
		{
			IEnumerator[] _enumerators;

			internal ZipEnumerator(params IEnumerator[] enumerators)
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

		[EnumeratorItemType(typeof(int))]
		public class RangeEnumerator : IEnumerator, IEnumerable
		{
			int _index;
			int _begin;
			int _end;
			int _step;

			internal RangeEnumerator(int begin, int end, int step)
			{
				if (step > 0)
				{
					_end = begin + (step * (int)Math.Ceiling(Math.Abs(end-begin)/((double)step)));
				}
				else
				{
					_end = begin + (step * (int)Math.Ceiling(Math.Abs(begin-end)/((double)Math.Abs(step))));
				}


				_end -= step;
				_begin = begin-step;
				_step = step;
				_index = _begin;
			}

			public void Reset()
			{
				_index = _begin;
			}

			public bool MoveNext()
			{
				if (_index != _end)
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

		[EnumeratorItemType(typeof(object[]))]
		public class EnumerateEnumerator : IEnumerator, IEnumerable
		{
			int _index = -1;

			IEnumerator _enumerator;

			internal EnumerateEnumerator(IEnumerator enumerator)
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
