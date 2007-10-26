namespace Boo.Lang.Compiler.TypeSystem
{
	public interface IGenericParameter: IType
	{
		IType DeclaringType { get; }
		IMethod DeclaringMethod { get; } 
		int GenericParameterPosition { get; }
		// TODO: Constraints { get; }
	}
}