namespace Boo.Lang.Compiler.TypeSystem
{
	public interface IAccessibleMember : IMember
	{
		bool IsProtected
		{
			get;
		}

		bool IsInternal
		{
			get;
		}

		bool IsPrivate
		{
			get;
		}
	}
}