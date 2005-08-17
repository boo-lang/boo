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
	using System;
	using Boo.Lang.Compiler.Ast;
	using Attribute = Boo.Lang.Compiler.Ast.Attribute;

	public class BooClassBuilder
	{
		BooCodeBuilder _codeBuilder;
		ClassDefinition _cd;
		
		public BooClassBuilder(BooCodeBuilder codeBuilder, string name)
		{
			if (null == codeBuilder)
			{
				throw new ArgumentNullException("codeBuilder");
			}
			if (null == name)
			{
				throw new ArgumentNullException("name");
			}
			
			_codeBuilder = codeBuilder;
			_cd = new ClassDefinition();
			_cd.Name = name;
			_cd.Entity = new InternalClass(_codeBuilder.TypeSystemServices, _cd);
		}
		
		public BooCodeBuilder CodeBuilder
		{
			get
			{
				return _codeBuilder;
			}
		}
		
		public ClassDefinition ClassDefinition
		{
			get
			{
				return _cd;
			}
		}
		
		public IType Entity
		{
			get
			{
				return (IType)_cd.Entity;
			}
		}
		
		public TypeMemberModifiers Modifiers
		{
			get
			{
				return _cd.Modifiers;
			}
			
			set
			{
				_cd.Modifiers = value;
			}
		}
		
		public LexicalInfo LexicalInfo
		{
			get
			{
				return _cd.LexicalInfo;
			}
			
			set
			{
				_cd.LexicalInfo = value;
			}
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
			Constructor constructor = new Constructor();
			constructor.Modifiers = TypeMemberModifiers.Public;
			constructor.Entity = new InternalConstructor(_codeBuilder.TypeSystemServices, constructor);
			_cd.Members.Add(constructor);
			
			return new BooMethodBuilder(_codeBuilder, constructor);
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
			TypeMemberModifiers modifiers = TypeMemberModifiers.Public;
			Property property = new Property(name);
			property.Modifiers = modifiers;
			property.Type = _codeBuilder.CreateTypeReference(type);
			property.Entity = new InternalProperty(_codeBuilder.TypeSystemServices, property);
			property.Getter = _codeBuilder.CreateMethod("get_" + name, type, modifiers);

			_cd.Members.Add(property);
			return property;			
		}
		
		public Field AddField(string name, IType type)
		{
			Field field = _codeBuilder.CreateField(name, type);			
			_cd.Members.Add(field);
			return field;
		}
	}
}
