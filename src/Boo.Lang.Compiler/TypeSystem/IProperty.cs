namespace Boo.Lang.Compiler.TypeSystem
{
	public interface IProperty : IAccessibleMember, IEntityWithParameters, IExtensionEnabled
	{	
		IMethod GetGetMethod();
		
		IMethod GetSetMethod();
	}
}