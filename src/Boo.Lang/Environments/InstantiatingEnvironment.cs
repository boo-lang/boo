using System;

namespace Boo.Lang.Environments
{
	/// <summary>
	/// An <see cref="IEnvironment" /> implementation that simply instantiates requested types.
	/// </summary>
	public class InstantiatingEnvironment : IEnvironment
	{	
		public TNeed Provide<TNeed>() where TNeed : class
		{
			return Activator.CreateInstance<TNeed>();
		}
	}
}