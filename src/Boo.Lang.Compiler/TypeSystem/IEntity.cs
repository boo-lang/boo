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

using System;
using System.Collections;
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.TypeSystem
{
	public interface IEntity
	{	
		string Name
		{
			get;
		}
		
		string FullName
		{
			get;
		}
		
		EntityType EntityType
		{
			get;
		}
	}
	
	public interface IInternalEntity : IEntity
	{
		Boo.Lang.Compiler.Ast.Node Node
		{
			get;
		}
	}
	
	public interface ITypedEntity : IEntity
	{
		IType Type
		{
			get;			
		}
	}
	
	public interface IMember : ITypedEntity
	{
		IType DeclaringType
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
	
	public interface IEvent : IMember
	{		
		IMethod GetAddMethod();
		IMethod GetRemoveMethod();
	}
	
	public interface IField : IMember
	{	
		bool IsLiteral
		{
			get;
		}
		
		object StaticValue
		{
			get;
		}
	}
	
	public interface IProperty : IMember
	{
		IParameter[] GetParameters();
		
		IMethod GetGetMethod();
		
		IMethod GetSetMethod();
	}
	
	public interface IType : ITypedEntity, INamespace
	{	
		bool IsClass
		{
			get;
		}
		
		bool IsInterface
		{
			get;
		}
		
		bool IsEnum
		{
			get;
		}
		
		bool IsByRef
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
		
		int GetTypeDepth();
		
		IType BaseType
		{
			get;
		}
		
		IEntity GetDefaultMember();
		
		IEntity[] GetMembers();
		
		IConstructor[] GetConstructors();
		
		IType[] GetInterfaces();
		
		bool IsSubclassOf(IType other);
		
		bool IsAssignableFrom(IType other);
	}
	
	public interface ICallableType : IType
	{
		CallableSignature GetSignature();
	}
	
	public interface IArrayType : IType
	{
		int GetArrayRank();
		
		IType GetElementType();
	}
	
	public interface IParameter : ITypedEntity
	{		
	}
	
	public interface IMethod : IMember
	{		
		IParameter[] GetParameters();		
		
		IType ReturnType
		{
			get;
		}
		
		ICallableType CallableType
		{
			get;
		}
		
		bool IsVirtual
		{
			get;
		}
		
		bool IsSpecialName
		{
			get;
		}
	}
	
	public interface IConstructor : IMethod
	{		
	}
}
