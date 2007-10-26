namespace Boo.Lang.Compiler.TypeSystem
{
	public interface IParameter : ITypedEntity
	{
		/// <summary>
		/// Is the parameter out or ref?
		/// </summary>
		bool IsByRef
		{
			get;
		}
	}
}