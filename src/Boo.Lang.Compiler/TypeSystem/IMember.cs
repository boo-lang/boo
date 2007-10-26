namespace Boo.Lang.Compiler.TypeSystem
{
	public interface IMember : ITypedEntity
	{
		bool IsDuckTyped
		{
			get;
		}

		IType DeclaringType
		{
			get;
		}
		
		bool IsStatic
		{
			get;
		}
		
		bool IsPublic
		{
			get;
		}
	}
}