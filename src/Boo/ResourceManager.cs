using System;

namespace Boo
{
	/// <summary>
	/// Resource manager.
	/// </summary>
	public sealed class ResourceManager
	{
		static System.Resources.ResourceManager _rm = new System.Resources.ResourceManager("strings", typeof(ResourceManager).Assembly);

		private ResourceManager()
		{
		}

		public static string GetString(string name)
		{
			return _rm.GetString(name);
		}

		public static string Format(string name, params object[] args)
		{
			return string.Format(GetString(name), args);
		}
	}
}
