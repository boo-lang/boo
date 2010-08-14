using System;

namespace Boo.Lang.Environments
{
	/// <summary>
	/// Holds the active dynamically scoped <see cref="IEnvironment">environment</see> instance.
	/// 
	/// The active environment is responsible for providing code with any <see cref="My&lt;TNeed&gt.Instance">needs</see>.
	/// 
	/// A particular <see cref="IEnvironment">environment</see> can be made active for the execution of a specific piece of code through <see cref="ActiveEnvironment.With" />.
	/// 
	/// The environment design pattern has been previously described <see cref="http://bamboo.github.com/2009/02/16/environment-based-programming.html">in this article</see>.
	/// </summary>
	public static class ActiveEnvironment
	{
		/// <summary>
		/// The active environment.
		/// </summary>
		public static IEnvironment Instance { get { return _instance; } }

		/// <summary>
		/// Executes <paramref name="code"/> in the specified <paramref name="environment"/>.
		/// </summary>
		/// <param name="environment">environment that should be made active during the execution of <paramref name="code"/></param>
		/// <param name="code">code to execute</param>
		public static void With(IEnvironment environment, Action code)
		{
			var previous = _instance;
			try
			{
				_instance = environment;
				code(); 
			}
			finally
			{
				_instance = previous;
			}
		}

		[ThreadStatic]
		private static IEnvironment _instance;
	}
}