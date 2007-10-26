namespace Boo.Lang.Compiler.TypeSystem
{
	public interface IConstructedMethodInfo
	{
		IType[] GenericArguments { get; }
		IMethod GenericDefinition { get; }
		bool FullyConstructed { get; }
	}
}