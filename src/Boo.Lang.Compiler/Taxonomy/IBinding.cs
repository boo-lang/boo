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
	public enum InfoType
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
		SpecialFunction = 0x4000,
		MethodReference,
		Unknown,
		Null,
		Error,
		Any = 0xFFFF
	}	
	
	public interface IInternalInfo
	{
		Boo.Lang.Compiler.Ast.Node Node
		{
			get;
		}
		
		bool Visited
		{
			get;
			set;
		}
	}
	
	public interface IInfo
	{	
		string FullName
		{
			get;
		}
		
		string Name
		{
			get;
		}
		
		InfoType InfoType
		{
			get;
		}
	}
	
	public interface ITypedInfo : IInfo
	{
		ITypeInfo BoundType
		{
			get;			
		}
	}
	
	public interface IMemberInfo : ITypedInfo
	{
		ITypeInfo DeclaringType
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
	
	public interface IEventInfo : IMemberInfo
	{		
	}
	
	public interface IFieldInfo : IMemberInfo
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
	
	public interface IPropertyInfo : IMemberInfo
	{
		ITypeInfo[] GetIndexParameters();
		
		IMethodInfo GetGetMethod();
		
		IMethodInfo GetSetMethod();
	}
	
	public interface ITypeInfo : ITypedInfo, INamespace
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
		
		ITypeInfo GetElementType();
		
		ITypeInfo BaseType
		{
			get;
		}
		
		IInfo GetDefaultMember();
		
		IInfo[] GetMembers();
		
		IConstructorInfo[] GetConstructors();
		
		ITypeInfo[] GetInterfaces();
		
		bool IsSubclassOf(ITypeInfo other);
		
		bool IsAssignableFrom(ITypeInfo other);
	}
	
	public interface IParameterInfo : ITypedInfo
	{		
		int Position
		{
			get;
		}
	}
	
	public interface IMethodInfo : IMemberInfo
	{
		int ParameterCount
		{
			get;
		}
		
		ITypeInfo GetParameterType(int parameterIndex);		
		
		ITypeInfo ReturnType
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
	
	public interface IConstructorInfo : IMethodInfo
	{		
	}
}
