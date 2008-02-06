namespace Boo.Lang.Compiler.TypeSystem
{
	public interface IConstructedTypeInfo
	{
		IType[] GenericArguments { get; }
		IType GenericDefinition { get; }
		bool FullyConstructed { get; }

		IMethod GetMethodTemplate(IMethod method);
	}
}