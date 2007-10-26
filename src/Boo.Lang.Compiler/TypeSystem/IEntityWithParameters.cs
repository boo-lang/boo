namespace Boo.Lang.Compiler.TypeSystem
{
	public interface IEntityWithParameters : IEntity
	{
		IParameter[] GetParameters();

		bool AcceptVarArgs
		{
			get;
		}
	}
}