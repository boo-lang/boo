#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
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
