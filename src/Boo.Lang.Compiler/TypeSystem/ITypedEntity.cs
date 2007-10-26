namespace Boo.Lang.Compiler.TypeSystem
{
	public interface ITypedEntity : IEntity
	{	
		IType Type
		{
			get;			
		}
	}
}