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
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Reflection;
using Boo.Lang.Runtime;

namespace Boo.Lang
{
	/// <summary>
	/// boo language builtin functions.
	/// </summary>
	public class Builtins
	{
		public class duck
		{
		}
		
		public static System.Version BooVersion
		{
			get
			{
				return new System.Version("0.7.6.2111");
			}
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
			IEnumerator enumerator = enumerable.GetEnumerator();
			if (enumerator.MoveNext())
			{
				sb.Append(enumerator.Current);
				while (enumerator.MoveNext())
				{
					sb.Append(separator);
					sb.Append(enumerator.Current);
				}
			}
			return sb.ToString();
		}
		
		public static string join(IEnumerable enumerable, char separator)
		{
			StringBuilder sb = new StringBuilder();
			IEnumerator enumerator = enumerable.GetEnumerator();
			if (enumerator.MoveNext())
			{
				sb.Append(enumerator.Current);
				while (enumerator.MoveNext())
				{
					sb.Append(separator);
					sb.Append(enumerator.Current);
				}
			}
			return sb.ToString();
		}

		public static string join(IEnumerable enumerable)
		{
			return join(enumerable, ' ');
		}

		public static IEnumerable map(object enumerable, ICallable function)
		{
			if (null == enumerable)
			{
				throw new ArgumentNullException("enumerable");
			}
			if (null == function)
			{
				throw new ArgumentNullException("function");
			}
			return new MapEnumerable(RuntimeServices.GetEnumerable(enumerable), function);
		}

		public static object[] array(IEnumerable enumerable)
		{
			return new List(enumerable).ToArray();
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
			if (RuntimeServices.IsPromotableNumeric(Type.GetTypeCode(elementType)))
			{
				int i=0;
				foreach (object item in collection)
				{
					object value = RuntimeServices.CheckNumericPromotion(item).ToType(elementType, null);
					array.SetValue(value, i);
					++i;
				}
			}
			else
			{
				collection.CopyTo(array, 0);
			}
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
			
			// future optimization, check EnumeratorItemType of enumerable
			// and get the fast path whenever possible
			List l = null;
			if (RuntimeServices.IsPromotableNumeric(Type.GetTypeCode(elementType)))
			{
				l = new List();
				foreach (object item in enumerable)
				{
					object value = RuntimeServices.CheckNumericPromotion(item).ToType(elementType, null);
					l.Add(value);
				}
			}
			else
			{
				l = new List(enumerable);
			}
			return l.ToArray(elementType);
		}
		
		public static Array array(Type elementType, int length)
		{
			return matrix(elementType, length);
		}

		public static Array matrix(Type elementType, params int[] lengths)
		{
			if (null == elementType)
			{
				throw new ArgumentNullException("elementType");
			}
			return Array.CreateInstance(elementType, lengths);
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
			p.StartInfo.RedirectStandardInput = true;
			p.StartInfo.RedirectStandardError = true;
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
		
		internal class AssemblyExecutor : MarshalByRefObject
		{
			string _filename;
			string[] _arguments;
			string _capturedOutput = "";
			
			public AssemblyExecutor(string filename, string[] arguments)
			{
				_filename = filename;
				_arguments = arguments;
			}
			
			public string CapturedOutput
			{
				get
				{
					return _capturedOutput;
				}
			}
			
			public void Execute()
			{
				StringWriter output = new System.IO.StringWriter();
				TextWriter saved = Console.Out;
				try
				{
					Console.SetOut(output);
					//AppDomain.CurrentDomain.ExecuteAssembly(_filename, null, _arguments);
					Assembly.LoadFrom(_filename).EntryPoint.Invoke(null, new object[1] { _arguments });
				}
				finally
				{
					Console.SetOut(saved);
					_capturedOutput = output.ToString();
				}
			}
		}
		
		/// <summary>
		/// Execute the specified MANAGED application in a new AppDomain.
		///
		/// The base directory for the new application domain will be set to
		/// directory containing filename (Path.GetDirectoryName(Path.GetFullPath(filename))).
		/// </summary>
		public static string shellm(string filename, string[] arguments)
		{
			AppDomainSetup setup = new AppDomainSetup();
			setup.ApplicationBase = Path.GetDirectoryName(Path.GetFullPath(filename));
				
			AppDomain domain = AppDomain.CreateDomain("shellm", null, setup);
			try
			{
				AssemblyExecutor executor = new AssemblyExecutor(filename, arguments);
				domain.DoCallBack(new CrossAppDomainDelegate(executor.Execute));
				return executor.CapturedOutput;
			}
			finally
			{
				AppDomain.Unload(domain);
			}
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
		
		public static IEnumerable reversed(object enumerable)
		{
			return new List(iterator(enumerable)).Reversed;
		}
		
		public static ZipEnumerator zip(params object[] enumerables)
		{
			IEnumerator[] enumerators = new IEnumerator[enumerables.Length];
			for (int i=0; i<enumerables.Length; ++i)
			{
				enumerators[i] = GetEnumerator(enumerables[i]);
			}
			return new ZipEnumerator(enumerators);
		}
		
		public static ConcatEnumerator cat(params object[] args)
		{
			return new ConcatEnumerator(args);
		}
		
		private class MapEnumerable : IEnumerable
		{
			IEnumerable _enumerable;
			ICallable _function;
			
			public MapEnumerable(IEnumerable enumerable, ICallable function)
			{
				_enumerable = enumerable;
				_function = function;
			}
			
			public IEnumerator GetEnumerator()
			{
				return new MapEnumerator(_enumerable.GetEnumerator(), _function);
			}
		}

		private class MapEnumerator : IEnumerator
		{
			IEnumerator _enumerator;

			ICallable _function;

			object _current;

			object[] _arguments = new object[1];

			public MapEnumerator(IEnumerator enumerator, ICallable function)
			{
				_enumerator = enumerator;
				_function = function;
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
		
		public class ConcatEnumerator : IEnumerator, IEnumerable
		{
			int _index;
			object[] _enumerables;
			IEnumerator _current;
			
			internal ConcatEnumerator(params object[] args)
			{
				_enumerables = args;
				Reset();
			}
			
			public void Reset()
			{
				_index = 0;
				_current = iterator(_enumerables[_index]).GetEnumerator();
			}
			
			public bool MoveNext()
			{
				if (_current.MoveNext())
				{
					return true;
				}
				
				while (++_index < _enumerables.Length)
				{
					_current = iterator(_enumerables[_index]).GetEnumerator();
					if (_current.MoveNext()) 
						return true;
				}
				return false;
			}
			
			public IEnumerator GetEnumerator()
			{
				return this;
			}
			
			public object Current
			{
				get
				{
					return _current.Current;
				}
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

		private static IEnumerator GetEnumerator(object enumerable)
		{
			return RuntimeServices.GetEnumerable(enumerable).GetEnumerator();
		}
	}
}
