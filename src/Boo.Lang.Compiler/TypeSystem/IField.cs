namespace Boo.Lang.Compiler.TypeSystem
{
	public interface IField : IAccessibleMember
	{	
		bool IsInitOnly
		{
			get;
		}
		
		bool IsLiteral
		{
			get;
		}
		
		object StaticValue
		{
			get;
		}
	}
}