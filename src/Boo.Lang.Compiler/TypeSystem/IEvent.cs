namespace Boo.Lang.Compiler.TypeSystem
{
	public interface IEvent : IMember
	{		
		IMethod GetAddMethod();
		IMethod GetRemoveMethod();
		IMethod GetRaiseMethod();

		bool IsAbstract
		{
			get;
		}

		bool IsVirtual
		{
			get;
		}
	}
}