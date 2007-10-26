namespace Boo.Lang.Compiler.TypeSystem
{
	public interface IExternalEntity : IEntity
	{
		System.Reflection.MemberInfo MemberInfo
		{
			get;
		}
	}
}