namespace Boo.Lang.Environments
{
	public interface IEnvironment
	{
		TNeed Provide<TNeed>() where TNeed : class;
	}
}