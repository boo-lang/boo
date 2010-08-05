using System.Runtime.CompilerServices;

namespace Boo.Lang.Environments
{
	[CompilerGlobalScope]
	public static class EnvironmentExtensions
	{
		/// <summary>
		/// Runs the given action in this environment.
		/// </summary>
		public static void Run(this IEnvironment @this, System.Action action)
		{
			Environment.With(@this, action);
		}

		/// <summary>
		/// Invokes the given function in this environment.
		/// </summary>
		public static TResult Invoke<TResult>(this IEnvironment @this, System.Func<TResult> function)
		{
			TResult result = default(TResult);
			Environment.With(@this, () => result = function());
			return result;
		}
	}
}