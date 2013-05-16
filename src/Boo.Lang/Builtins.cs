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
using System.Collections.Generic;
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
				return new System.Version("0.9.7.0");
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
			var sb = new StringBuilder();
			var enumerator = enumerable.GetEnumerator();
			using (enumerator as IDisposable)
			{
				if (enumerator.MoveNext())
				{
					sb.Append(enumerator.Current);
					while (enumerator.MoveNext())
					{
						sb.Append(separator);
						sb.Append(enumerator.Current);
					}
				}
			}
			return sb.ToString();
		}

		public static string join(IEnumerable enumerable, char separator)
		{
		    return join(enumerable, separator.ToString());
		}

		public static string join(IEnumerable enumerable)
		{
			return join(enumerable, " ");
		}

		public static IEnumerable map(object enumerable, ICallable function)
		{
			if (null == enumerable) throw new ArgumentNullException("enumerable");
			if (null == function) throw new ArgumentNullException("function");

			object[] args = new object[1];
			foreach (object item in iterator(enumerable))
			{
				args[0] = item;
				yield return function.Call(args);
			}
		}

		public static object[] array(IEnumerable enumerable)
		{
			return new List(enumerable).ToArray();
		}

		//fast path, that implementation detail does not really need to be API
		private static Array ArrayFromCollection(Type elementType, ICollection collection)
		{
			if (null == elementType)
				throw new ArgumentNullException("elementType");
			if (null == collection)
				throw new ArgumentNullException("collection");

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

		[TypeInferenceRule(TypeInferenceRules.ArrayOfTypeReferencedByFirstArgument)]
		public static Array array(Type elementType, IEnumerable enumerable)
		{
			if (null == elementType)
				throw new ArgumentNullException("elementType");
			if (null == enumerable)
				throw new ArgumentNullException("enumerable");

			#pragma warning disable 618 //obsolete
			ICollection collection = enumerable as ICollection;
			if (null != collection) //fast path
				return ArrayFromCollection(elementType, collection);
			#pragma warning restore 618

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

		[TypeInferenceRule(TypeInferenceRules.ArrayOfTypeReferencedByFirstArgument)]
		public static Array array(Type elementType, int length)
		{
			if (length < 0)
				throw new ArgumentException("`length' cannot be negative", "length");

			return matrix(elementType, length);
		}

		public static Array matrix(Type elementType, params int[] lengths)
		{
			if (null == elementType)
				throw new ArgumentNullException("elementType");
			if (null == lengths || 0 == lengths.Length)
				throw new ArgumentException("A matrix must have at least one dimension", "lengths");

			return Array.CreateInstance(elementType, lengths);
		}


		#region generic array/matrix builtins (v0.9.2+)
		public static T[] array<T>(int length)
		{
			throw new NotSupportedException("Operation should have been optimized away by the compiler!");
		}

		public static T[,] matrix<T>(int length0, int length1)
		{
			throw new NotSupportedException("Operation should have been optimized away by the compiler!");
		}

		public static T[,,] matrix<T>(int length0, int length1, int length2)
		{
			throw new NotSupportedException("Operation should have been optimized away by the compiler!");
		}

		public static T[,,,] matrix<T>(int length0, int length1, int length2, int length3)
		{
			throw new NotSupportedException("Operation should have been optimized away by the compiler!");
		}
		#endregion


		public static IEnumerable iterator(object enumerable)
		{
			return RuntimeServices.GetEnumerable(enumerable);
		}

#if !NO_SYSTEM_PROCESS
		public static System.Diagnostics.Process shellp(string filename, string arguments)
		{
			var p = new System.Diagnostics.Process();
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
			var p = shellp(filename, arguments);
			var output = p.StandardOutput.ReadToEnd();
			p.WaitForExit();
			return output;
		}
#endif

		public static IEnumerable<object[]> enumerate(object enumerable)
		{
			int i = 0;
			foreach (object item in iterator(enumerable))
			{
				yield return new object[] { i++, item };
			}
		}

		public static IEnumerable<int> range(int max)
		{
			if (max < 0) /* added for coherence with behavior of compiler-optimized
						  * for-in-range() loops, should compiler loops automatically
						  * inverse iteration in this case? */
			{
				throw new ArgumentOutOfRangeException("max < 0");
			}
			return range(0, max);
		}

		public static IEnumerable<int> range(int begin, int end)
		{
			if (begin < end)
			{
				for (int i = begin; i < end; ++i) yield return i;
			}
			else if (begin > end)
			{
				for (int i = begin; i > end; --i) yield return i;
			}
		}

		public static IEnumerable<int> range(int begin, int end, int step)
		{
			if (0 ==step)
			{
				throw new ArgumentOutOfRangeException("step == 0");
			}
			if (step < 0)
			{
				if (begin < end)
				{
					throw new ArgumentOutOfRangeException("begin < end && step < 0");
				}
				for (int i = begin; i > end; i += step) yield return i;
			}
			else
			{
				if (begin > end)
				{
					throw new ArgumentOutOfRangeException("begin > end && step > 0");
				}
				for (int i = begin; i < end; i += step) yield return i;
			}
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

		public static IEnumerable<object> cat(params object[] args)
		{
			foreach (object e in args)
			{
				foreach (object item in iterator(e))
				{
					yield return item;
				}
			}
		}

		[EnumeratorItemType(typeof(object[]))]
		public class ZipEnumerator : IEnumerator, IEnumerable, IDisposable
		{
			IEnumerator[] _enumerators;

			internal ZipEnumerator(params IEnumerator[] enumerators)
			{
				_enumerators = enumerators;
			}

			public void Dispose()
			{
				for (int i=0; i<_enumerators.Length; ++i)
				{
					IDisposable d = _enumerators[i] as IDisposable;
					if (d != null)
						d.Dispose();
				}
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

		private static IEnumerator GetEnumerator(object enumerable)
		{
			return RuntimeServices.GetEnumerable(enumerable).GetEnumerator();
		}
	}
}

