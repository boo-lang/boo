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

namespace Boo.Lang.Compiler.Taxonomy
{
	[Flags]
	public enum ElementType
	{
		CompileUnit = 0x00,
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
		Array = 0x2000,
		BuiltinFunction = 0x4000,
		MethodReference,
		Unknown,
		Null,
		Error,
		Any = 0xFFFF
	}
	
	public interface IElement
	{	
		string Name
		{
			get;
		}
		
		string FullName
		{
			get;
		}
		
		ElementType ElementType
		{
			get;
		}
	}
	
	public interface IInternalElement : IElement
	{
		Boo.Lang.Compiler.Ast.Node Node
		{
			get;
		}
	}
	
	public interface ITypedElement : IElement
	{
		IType Type
		{
			get;			
		}
	}
	
	public interface IMember : ITypedElement
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
	
	public interface IType : ITypedElement, INamespace
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
		
		int GetArrayRank();
		
		IType GetElementType();
		
		IType BaseType
		{
			get;
		}
		
		IElement GetDefaultMember();
		
		IElement[] GetMembers();
		
		IConstructor[] GetConstructors();
		
		IType[] GetInterfaces();
		
		bool IsSubclassOf(IType other);
		
		bool IsAssignableFrom(IType other);
	}
	
	public interface IParameter : ITypedElement
	{		
	}
	
	public interface IMethod : IMember
	{
		IParameter[] GetParameters();		
		
		IType ReturnType
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
