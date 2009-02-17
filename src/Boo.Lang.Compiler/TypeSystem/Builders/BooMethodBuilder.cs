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

using Boo.Lang.Compiler.TypeSystem.Internal;

namespace Boo.Lang.Compiler.TypeSystem
{
	using System;
	using Boo.Lang.Compiler.Ast;

	public class BooMethodBuilder
	{
		private BooCodeBuilder _codeBuilder;
		private Method _method;
		
		public BooMethodBuilder(BooCodeBuilder codeBuilder, string name, IType returnType) : this(codeBuilder, name, returnType, TypeMemberModifiers.Public)
		{
		}
		
		public BooMethodBuilder(BooCodeBuilder codeBuilder, string name, IType returnType, TypeMemberModifiers modifiers)
		{
			if (null == codeBuilder)
				throw new ArgumentNullException("codeBuilder");
			if (null == name)
				throw new ArgumentNullException("name");
			
			_codeBuilder = codeBuilder;			
			_method = _codeBuilder.CreateMethod(name, returnType, modifiers);
		}
		
		public BooMethodBuilder(BooCodeBuilder codeBuilder, Method method)
		{
			if (null == codeBuilder)
				throw new ArgumentNullException("codeBuilder");
			if (null == method)
				throw new ArgumentNullException("method");
			_codeBuilder = codeBuilder;
			_method = method;
		}
		
		public Method Method
		{
			get
			{
				return _method;
			}
		}
		
		public InternalMethod Entity
		{
			get
			{
				return (InternalMethod)_method.Entity;
			}
		}
		
		public Block Body
		{
			get
			{
				return _method.Body;
			}
		}
		
		public ParameterDeclarationCollection Parameters
		{
			get
			{
				return _method.Parameters;
			}
		}
		
		public LocalCollection Locals
		{
			get
			{
				return _method.Locals;
			}
		}
		
		public TypeMemberModifiers Modifiers
		{
			get
			{
				return _method.Modifiers;
			}
			
			set
			{
				_method.Modifiers = value;
			}
		}
		
		public ParameterDeclaration AddParameter(string name, IType type)
		{
			return AddParameter(name, type, false);
		}
		
		public ParameterDeclaration AddParameter(string name, IType type, bool byref)
		{
			ParameterDeclaration pd = _codeBuilder.CreateParameterDeclaration(GetNextParameterIndex(), name, type, byref);
			_method.Parameters.Add(pd);
			return pd;
		}
		
		int GetNextParameterIndex()
		{
			return (_method.IsStatic ? 0 : 1) + _method.Parameters.Count;
		}
	}
}
