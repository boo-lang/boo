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

using System;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Environments;
using Attribute=Boo.Lang.Compiler.Ast.Attribute;

namespace Boo.Lang.Compiler.TypeSystem.Builders
{
	public class BooClassBuilder
	{
		private readonly BooCodeBuilder _codeBuilder;
		private readonly ClassDefinition _cd;
		private InternalTypeSystemProvider _internalTypeSystemProvider;

		public BooClassBuilder(string name)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			
			_internalTypeSystemProvider = My<InternalTypeSystemProvider>.Instance;
			_codeBuilder = My<BooCodeBuilder>.Instance;
			_cd = new ClassDefinition { Name = name, IsSynthetic = true };
			EnsureEntityFor(_cd);
		}
		
		public ClassDefinition ClassDefinition
		{
			get { return _cd; }
		}
		
		public IType Entity
		{
			get { return (IType)_cd.Entity; }
		}
		
		public TypeMemberModifiers Modifiers
		{
			get { return _cd.Modifiers; }
			
			set { _cd.Modifiers = value; }
		}
		
		public LexicalInfo LexicalInfo
		{
			get { return _cd.LexicalInfo; }
			
			set { _cd.LexicalInfo = value; }
		}
		
		public void AddAttribute(Attribute attribute)
		{
			_cd.Attributes.Add(attribute);
		}
		
		public void AddBaseType(IType type)
		{			
			_cd.BaseTypes.Add(_codeBuilder.CreateTypeReference(type));
		}
		
		public BooMethodBuilder AddConstructor()
		{
			var constructor = _codeBuilder.CreateConstructor(TypeMemberModifiers.Public);
			_cd.Members.Add(constructor);
			return new BooMethodBuilder(_codeBuilder, constructor);
		}

		private void EnsureEntityFor(TypeMember constructor)
		{
			_internalTypeSystemProvider.EntityFor(constructor);
		}

		public BooMethodBuilder AddMethod(string name, IType returnType)
		{
			return AddMethod(name, returnType, TypeMemberModifiers.Public);
		}
		
		public BooMethodBuilder AddVirtualMethod(string name, IType returnType)
		{
			return AddMethod(name, returnType, TypeMemberModifiers.Public|TypeMemberModifiers.Virtual);
		}
		
		public BooMethodBuilder AddMethod(string name, IType returnType, TypeMemberModifiers modifiers)
		{
			BooMethodBuilder builder = new BooMethodBuilder(_codeBuilder, name, returnType, modifiers);
			_cd.Members.Add(builder.Method);
			return builder;
		}
		
		public Property AddReadOnlyProperty(string name, IType type)
		{
			var property = _codeBuilder.CreateProperty(name, type);
			property.Getter = _codeBuilder.CreateMethod("get_" + name, type, TypeMemberModifiers.Public);
			_cd.Members.Add(property);
			return property;			
		}
		
		public Field AddField(string name, IType type)
		{
			Field field = _codeBuilder.CreateField(name, type);
			_cd.Members.Add(field);
			return field;
		}

		public Field AddPublicField(string name, IType type)
		{
			return AddField(name, type, TypeMemberModifiers.Public);
		}

		public Field AddInternalField(string name, IType type)
		{
			return AddField(name, type, TypeMemberModifiers.Internal);
		}

		public Field AddField(string name, IType type, TypeMemberModifiers modifiers)
		{
			Field field = AddField(name, type);
			field.Modifiers = modifiers;
			return field;
		}

		public GenericParameterDeclaration AddGenericParameter(string name)
		{
			var genericParameters = ClassDefinition.GenericParameters;
			var declaration = _codeBuilder.CreateGenericParameterDeclaration(genericParameters.Count, name);
			genericParameters.Add(declaration);
			return declaration;
		}

		public GenericParameterDeclaration AddGenericParameter(GenericParameterDeclaration source)
		{
			var result = AddGenericParameter(source.Name);
			result.Constraints = source.Constraints;
			foreach (var baseType in source.BaseTypes)
			{
				result.BaseTypes.Add(baseType.CloneNode());
			}
			return result;
		}
	}
}