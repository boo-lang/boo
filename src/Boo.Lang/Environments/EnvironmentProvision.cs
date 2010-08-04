namespace Boo.Lang.Environments
{
	public struct EnvironmentProvision<T> where T: class
	{
		private T _instance;

		public T Instance { get { return _instance ?? (_instance = My<T>.Instance);  } }

		public static implicit operator T(EnvironmentProvision<T> provision)
		{
			return provision.Instance;
		}
	}
}
