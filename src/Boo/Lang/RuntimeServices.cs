using System;
using System.Collections;
using System.IO;

namespace Boo.Lang
{
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
		
		public static void CheckArrayUnpack(Array array, int expected)
		{
			if (null == array)
			{
				Error("CantUnpackNull");
			}			
			if (expected != array.Length)
			{
				Error("UnpackArrayOfWrongSize", expected, array.Length);
			}
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
				StreamReader reader = enumerable as StreamReader;
				if (null != reader)
				{
					iterator = new Boo.IO.StreamReaderEnumerator(reader);
				}
				else
				{
					Error("ArgumentNotEnumerable");
				}
			}
			return iterator;
		}
		
		public static bool IsMatch(string input, object pattern)
		{		
			return System.Text.RegularExpressions.Regex.IsMatch(input, (string)pattern);
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
