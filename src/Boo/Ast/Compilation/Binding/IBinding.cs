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

namespace Boo.Ast.Compilation.Binding
{
	public enum BindingType
	{
		Type,
		TypeReference,
		Method,		
		Constructor,
		Field,
		Property,
		Event,
		Local,		
		Parameter,
		Assembly,
		Namespace,
		Ambiguous,
		Error
	}
	
	public interface IBinding
	{	
		string Name
		{
			get;
		}
		
		BindingType BindingType
		{
			get;
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
	}
	
	public interface IFieldBinding : IMemberBinding
	{		
	}
	
	public interface IPropertyBinding : IMemberBinding
	{
		
	}
	
	public interface ITypeBinding : ITypedBinding, INamespace
	{			
		string FullName
		{
			get;
		}
		
		bool IsValueType
		{
			get;
		}
		
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
	}
	
	public interface IConstructorBinding : IMethodBinding
	{		
	}	
}
