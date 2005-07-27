#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

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
		IMethod GetRaiseMethod();

		bool IsAbstract
		{
			get;
		}

		bool IsVirtual
		{
			get;
		}
	}
	
	public interface IField : IAccessibleMember
	{	
		bool IsInitOnly
		{
			get;
		}
		
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
		
		bool IsAbstract
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
		
		bool IsFinal
		{
			get;
		}
		
		bool IsArray
		{
			get;
		}
		
		int GetTypeDepth();
		
		IType GetElementType();
		
		IType BaseType
		{
			get;
		}
		
		IEntity GetDefaultMember();
		
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
	}
	
	public interface ILocalEntity : ITypedEntity
	{
		bool IsPrivateScope
		{
			get;
		}
		
		/// <summary>
		/// Is this variable shared among closures?
		/// </summary>
		bool IsShared
		{
			get;
			set;
		}
		
		/// <summary>
		/// Is this variable ever used in the body of the method?
		/// </summary>
		bool IsUsed
		{
			get;
			set;
		}
	}
	
	public interface IParameter : ITypedEntity
	{		
	}
	
	public interface IAccessibleMember : IMember
	{
		bool IsProtected
		{
			get;
		}

		bool IsInternal
		{
			get;
		}

		bool IsPrivate
		{
			get;
		}
	}
	
	public interface IMethod : IAccessibleMember
	{		
		IParameter[] GetParameters();		
		
		IType ReturnType
		{
			get;
		}

		bool AcceptVarArgs
		{
			get;
		}
		
		ICallableType CallableType
		{
			get;
		}
		
		bool IsAbstract
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

	public interface IDestructor : IMethod
	{		
	}
}
