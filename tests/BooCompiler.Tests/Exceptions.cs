using System;
using NUnit.Framework;

namespace BooCompiler.Tests
{
	static class Exceptions
	{
		public static void Expecting<T>(Action action) where T:Exception
		{
			try
			{
				action();
			}
			catch (T)
			{
				return;
			}
			Assert.Fail("{0} expected!", typeof(T).Name);
		}
	}
}
