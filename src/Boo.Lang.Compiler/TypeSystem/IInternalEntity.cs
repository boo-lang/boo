namespace Boo.Lang.Compiler.TypeSystem
{
	public interface IInternalEntity : IEntity
	{
		Boo.Lang.Compiler.Ast.Node Node
		{
			get;
		}
	}
}