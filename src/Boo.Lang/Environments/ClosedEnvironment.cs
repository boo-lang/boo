namespace Boo.Lang.Environments
{
	public class ClosedEnvironment : IEnvironment
	{
		private readonly object[] _bindings;

		public ClosedEnvironment(params object[] bindings)
		{
			_bindings = bindings;
		}

		public TNeed Provide<TNeed>() where TNeed : class
		{
			foreach (var binding in _bindings)
				if (binding is TNeed)
					return (TNeed)binding;
			return null;
		}
	}
}