using System;
using System.Collections;

namespace Boo.Lang
{
	public class RuntimeServices
	{
		public static object MoveNext(IEnumerator enumerator)
		{
			if (!enumerator.MoveNext())
			{
				throw new ApplicationException(GetString("UnpackListOfWrongSize"));
			}
			return enumerator.Current;
		}
		
		public static bool IsMatch(string input, object pattern)
		{		
			return System.Text.RegularExpressions.Regex.IsMatch(input, (string)pattern);
		}
		
		static string GetString(string name)
		{
			return Boo.ResourceManager.GetString(name);
		}
	}
}
