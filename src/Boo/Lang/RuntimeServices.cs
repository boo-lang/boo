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
	using System.IO;
	using System.Text;

	public class RuntimeServices
	{
		public static object MoveNext(IEnumerator enumerator)
		{
			if (null == enumerator)
			{
				Error("CantUnpackNull");
			}
			if (!enumerator.MoveNext())
			{
				Error("UnpackListOfWrongSize");
			}
			return enumerator.Current;
		}
		
		public static int Len(object obj)
		{
			if (null != obj)
			{
				ICollection collection = obj as ICollection;
				if (null != collection)
				{
					return collection.Count;
				}
				string s = obj as string;
				if (null != s)
				{
					return s.Length;
				}
			}
			return 0;
		}
		
		public static string Mid(string s, int begin, int end)
		{
			if (begin < 0)
			{
				begin += s.Length;
			}
			if (end < 0)
			{
				end += s.Length;
			}
			return s.Substring(begin, end-begin);
		}
		
		public static Array GetRange(Array source, int begin, int end)
		{
			if (begin < 0)
			{
				begin += source.Length;
			}
			if (end < 0)
			{
				end += source.Length;
			}
			
			int targetLen = end-begin;
			Array target = Array.CreateInstance(source.GetType().GetElementType(), targetLen);
			Array.Copy(source, begin, target, 0, targetLen);
			return target;
		}
		
		public static void CheckArrayUnpack(Array array, int expected)
		{
			if (null == array)
			{
				Error("CantUnpackNull");
			}			
			if (expected > array.Length)
			{
				Error("UnpackArrayOfWrongSize", expected, array.Length);
			}
		}
		
		public static int NormalizeArrayIndex(Array array, int index)
		{
			return index < 0 ? array.Length + index : index;
		}
		
		public static IEnumerable GetEnumerable(object enumerable)
		{
			if (null == enumerable)
			{
				Error("CantEnumerateNull");
			}
			
			IEnumerable iterator = enumerable as IEnumerable;
			if (null == iterator)
			{
				TextReader reader = enumerable as TextReader;
				if (null != reader)
				{
					iterator = new Boo.IO.TextReaderEnumerator(reader);
				}
				else
				{
					Error("ArgumentNotEnumerable");
				}
			}
			return iterator;
		}
		
		#region global operators
		
		public static string op_Addition(string lhs, object rhs)
		{
			return string.Concat(lhs, rhs);
		}
		
		public static string op_Addition(object lhs, string rhs)
		{
			return string.Concat(lhs, rhs);
		}
		
		public static Array op_Multiply(Array lhs, int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			
			Type type = lhs.GetType();
			if (1 != type.GetArrayRank())
			{
				throw new ArgumentException("lhs");
			}
			
			int length = lhs.Length;
			Array result = Array.CreateInstance(type.GetElementType(), length*count);
			int destinationIndex = 0;
			for (int i=0; i<count; ++i)
			{
				Array.Copy(lhs, 0, result, destinationIndex, length);
				destinationIndex += length;
			}
			return result;
		}
		
		public static Array op_Multiply(int count, Array rhs)
		{
			return op_Multiply(rhs, count);
		}
		
		public static string op_Multiply(string lhs, int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			
			string result = null;
			if (null != lhs)
			{
				StringBuilder builder = new StringBuilder(lhs.Length * count);
				for (int i=0; i<count; ++i)
				{
					builder.Append(lhs);
				}
				result = builder.ToString();
			}
			return result;
		}
		
		public static string op_Multiply(int count, string rhs)
		{
			return op_Multiply(rhs, count);
		}
		
		public static bool op_NotMember(string lhs, string rhs)
		{
			return !op_Member(lhs, rhs);
		}
		
		public static bool op_Member(string lhs, string rhs)
		{			
			if (null == lhs || null == rhs)
			{
				return false;
			}
			return rhs.IndexOf(lhs) > -1;
		}
		
		public static bool op_Match(string input, string pattern)
		{			
			return System.Text.RegularExpressions.Regex.IsMatch(input, pattern);
		}
		
		public static bool op_NotMatch(string input, string pattern)
		{
			return !op_Match(input, pattern);
		}
		
		public static bool op_Member(object lhs, IList rhs)
		{
			if (null == rhs)
			{
				return false;
			}
			return rhs.Contains(lhs);
		}
		
		public static bool op_NotMember(object lhs, IList rhs)
		{
			return !op_Member(lhs, rhs);
		}
		
		public static bool op_Member(object lhs, IDictionary rhs)
		{
			if (null == rhs)
			{
				return false;
			}
			return rhs.Contains(lhs);
		}
		
		public static bool op_NotMember(object lhs, IDictionary rhs)
		{
			return !op_Member(lhs, rhs);
		}
		
		public static bool op_Equality(Array lhs, Array rhs)
		{
			if (lhs == rhs)
			{
				return true;
			}
			
			if (null == lhs || null == rhs)
			{
				return false;
			}
			
			if (1 != lhs.Rank || 1 != rhs.Rank)
			{
				throw new ArgumentException("array rank must be 1"); 
			}
			
			if (lhs.Length != rhs.Length)
			{
				return false;
			}
			
			for (int i=0; i<lhs.Length; ++i)
			{
				if (!Object.Equals(lhs.GetValue(i), rhs.GetValue(i)))
				{
					return false;
				}
			}
			return true;
		}
		#endregion
		
		public static bool ToBool(object value)
		{
			if (null == value)
			{
				return false;
			}
			
			if (value is ValueType)
			{		
				if (value is bool)
				{
					return ((bool)value);
				}
				if (value is int)
				{
					return 0 != ((int)value);
				}
				if (value is long)
				{
					return 0 != ((long)value);
				}
			}
			
			return true;
		}
		
		static void Error(string name, params object[] args)
		{
			throw new ApplicationException(Boo.ResourceManager.Format(name, args));
		}
		
		static void Error(string name)
		{
			throw new ApplicationException(Boo.ResourceManager.GetString(name));
		}
	}
}
