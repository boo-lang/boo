using System;
using System.Collections;

namespace Boo.Lang
{
	public class RuntimeServices
	{
		public static object MoveNext(IEnumerator enumerator)
		{
			if (null == enumerator)
			{
				throw new ApplicationException(GetString("CantUnpackNull"));
			}
			if (!enumerator.MoveNext())
			{
				throw new ApplicationException(GetString("UnpackListOfWrongSize"));
			}
			return enumerator.Current;
		}
		
		public static void CheckArrayUnpack(Array array, int expected)
		{
			if (null == array)
			{
				throw new ApplicationException(GetString("CantUnpackNull"));
			}			
			if (expected != array.Length)
			{
				throw new ApplicationException(Format("UnpackArrayOfWrongSize", expected, array.Length));
			}
		}
		
		public static bool IsMatch(string input, object pattern)
		{		
			return System.Text.RegularExpressions.Regex.IsMatch(input, (string)pattern);
		}
		
		static string GetString(string name)
		{
			return Boo.ResourceManager.GetString(name);
		}
		
		static string Format(string name, params object[] args)
		{
			return Boo.ResourceManager.Format(name, args);
		}
	}
}
