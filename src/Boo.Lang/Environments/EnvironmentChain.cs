namespace Boo.Lang.Environments
{
	public class EnvironmentChain : IEnvironment
	{
		private readonly IEnvironment[] _chain;

		public EnvironmentChain(params IEnvironment[] chain)
		{
			_chain = chain;
		}

		public TNeed Provide<TNeed>() where TNeed : class
		{
			foreach (var environment in _chain)
			{
				var need = environment.Provide<TNeed>();
				if (need != null)
					return need;
			}
			return null;
		}
	}
}
