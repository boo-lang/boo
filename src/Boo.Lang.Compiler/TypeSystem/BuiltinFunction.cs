#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
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
