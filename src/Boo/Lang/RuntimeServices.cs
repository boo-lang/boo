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
		
		static string GetString(string name)
		{
			return Boo.ResourceManager.GetString(name);
		}
	}
}
