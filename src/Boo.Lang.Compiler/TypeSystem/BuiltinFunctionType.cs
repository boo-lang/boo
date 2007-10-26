namespace Boo.Lang.Compiler.TypeSystem
{
	public enum BuiltinFunctionType
	{
		Len,
		AddressOf,
		Eval,
		Quack, // duck typing support,
		Switch, // switch IL opcode
		InitValueType, // initobj IL opcode
		Custom // custom builtin function
	}
}