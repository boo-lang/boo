namespace Boo.Lang.Compiler.TypeSystem
{
	public interface IMethodBase : IAccessibleMember, IEntityWithParameters
	{
		ICallableType CallableType
		{
			get;
		}
	}
}