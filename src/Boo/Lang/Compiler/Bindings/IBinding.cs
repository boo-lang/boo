#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;
using System.Collections;
using Boo.Lang.Ast;

namespace Boo.Lang.Compiler.Bindings
{
	[Flags]
	public enum BindingType
	{
		Module = 0x01,
		Type = 0x02,
		TypeReference = 0x04,
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
		Tuple = 0x2000,
		SpecialFunction = 0x4000,
		Unknown,
		Null,
		Error,
		Any = 0xFFFF
	}	
	
	public interface IInternalBinding
	{
		Boo.Lang.Ast.Node Node
		{
			get;
		}
		
		bool Visited
		{
			get;
			set;
		}
	}
	
	public interface IBinding
	{	
		string FullName
		{
			get;
		}
		
		string Name
		{
			get;
		}
		
		BindingType BindingType
		{
			get;
		}
	}
	
	public enum SpecialFunction
	{
		Typeof,
		Len
	}
	
	public class SpecialFunctionBinding : IBinding
	{
		SpecialFunction _function;
		
		public SpecialFunctionBinding(SpecialFunction f)
		{
			_function = f;
		}
		
		public string Name
		{
			get
			{
				return _function.ToString();
			}
		}
		
		public string FullName
		{
			get
			{
				return Name;
			}
		}
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.SpecialFunction;
			}
		}
		
		public SpecialFunction Function
		{
			get
			{
				return _function;
			}
		}
	}
	
	public interface ITypedBinding : IBinding
	{
		ITypeBinding BoundType
		{
			get;			
		}
	}
	
	public interface IMemberBinding : ITypedBinding
	{
		ITypeBinding DeclaringType
		{
			get;
		}
		
		bool IsStatic
		{
			get;
		}
		
		bool IsPublic
		{
			get;
		}
	}
	
	public interface IEventBinding : IMemberBinding
	{		
	}
	
	public interface IFieldBinding : IMemberBinding
	{		
		object StaticValue
		{
			get;
		}
	}
	
	public interface IPropertyBinding : IMemberBinding
	{
		ITypeBinding[] GetIndexParameters();
		
		IMethodBinding GetGetMethod();
		
		IMethodBinding GetSetMethod();
	}
	
	public interface ITypeBinding : ITypedBinding, INamespace
	{	
		bool IsClass
		{
			get;
		}
		
		bool IsEnum
		{
			get;
		}
		
		bool IsValueType
		{
			get;
		}
		
		bool IsArray
		{
			get;
		}
		
		ITypeBinding GetElementType();
		
		ITypeBinding BaseType
		{
			get;
		}
		
		IBinding GetDefaultMember();
		
		IConstructorBinding[] GetConstructors();
		
		bool IsSubclassOf(ITypeBinding other);
		
		bool IsAssignableFrom(ITypeBinding other);
	}
	
	public interface IMethodBinding : IMemberBinding
	{
		int ParameterCount
		{
			get;
		}
		
		ITypeBinding GetParameterType(int parameterIndex);		
		
		ITypeBinding ReturnType
		{
			get;
		}
		
		bool IsVirtual
		{
			get;
		}
	}
	
	public interface IConstructorBinding : IMethodBinding
	{		
	}
}
