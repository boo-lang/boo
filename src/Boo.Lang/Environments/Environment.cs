using System;

namespace Boo.Lang.Environments
{
	public static class Environment
	{
		public static IEnvironment CurrentEnvironment { get { return _currentEnvironment; } }

		public static void With(IEnvironment environment, Action action)
		{
			var previous = _currentEnvironment;
			try
			{
				_currentEnvironment = environment;
				action(); 
			}
			finally
			{
				_currentEnvironment = previous;
			}
		}

		[ThreadStatic]
		private static IEnvironment _currentEnvironment;
	}
}