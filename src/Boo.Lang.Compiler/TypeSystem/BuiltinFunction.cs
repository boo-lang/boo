#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

namespace Boo.Lang.Compiler.TypeSystem
{
	public enum BuiltinFunctionType
	{
		Len,
		AddressOf,
		Eval,
		Quack // duck typing support
	}
	
	public class BuiltinFunction : IEntity
	{
		public static BuiltinFunction Quack = new BuiltinFunction("quack", BuiltinFunctionType.Quack);
		
		public static BuiltinFunction Len = new BuiltinFunction("len", BuiltinFunctionType.Len);
		
		public static BuiltinFunction AddressOf = new BuiltinFunction("__addressof__", BuiltinFunctionType.AddressOf);
		
		public static BuiltinFunction Eval = new BuiltinFunction("__eval__", BuiltinFunctionType.Eval);
		
		BuiltinFunctionType _function;
		
		string _name;
		
		public BuiltinFunction(string name, BuiltinFunctionType type)
		{
			_name = name;
			_function = type;
		}
		
		public string Name
		{
			get
			{
				return _name;
			}
		}
		
		public string FullName
		{
			get
			{
				return Name;
			}
		}
		
		public EntityType EntityType
		{
			get
			{
				return EntityType.BuiltinFunction;
			}
		}
		
		public BuiltinFunctionType FunctionType
		{
			get
			{
				return _function;
			}
		}
	}
}
