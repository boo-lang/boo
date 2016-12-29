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
            catch (Boo.Lang.Compiler.CompilerError e)
            {
                if (e.InnerException is T)
                    return;
                throw;
            }
			Assert.Fail("{0} expected!", typeof(T).Name);
		}
	}
}
