namespace Boo.Lang.Compiler.TypeSystem
{
	using System;
	
	[Flags]
	public enum EntityType
	{
		CompileUnit = 0x00,
		Module = 0x01,
		Type = 0x02,
		Method = 0x08,		
		Constructor = 0x10,
		Field = 0x20,
		Property = 0x40,
		Event = 0x80,
		Local = 0x100,		
		Parameter = 0x200,
		Assembly = 0x400,
		Namespace = 0x800,
		Ambiguous = 0x1000,
		Array = 0x2000,
		BuiltinFunction = 0x4000,
		Unknown,
		Null,
		Error,
		Any = 0xFFFF
	}
}
