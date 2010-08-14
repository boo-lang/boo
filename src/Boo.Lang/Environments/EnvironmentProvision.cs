namespace Boo.Lang.Environments
{
	public struct EnvironmentProvision<T> where T: class
	{
		private T _instance;

		public T Instance
		{
			get
			{
				if (_instance != null) return _instance;
				return (_instance = My<T>.Instance); 
			}
		}

		public static implicit operator T(EnvironmentProvision<T> provision)
		{
			return provision.Instance;
		}
	}
}
