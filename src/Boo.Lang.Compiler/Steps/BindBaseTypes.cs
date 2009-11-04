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

using System.Collections.Generic;
using Boo.Lang.Compiler.Steps.Inheritance;
using Boo.Lang.Compiler.TypeSystem.Internal;

namespace Boo.Lang.Compiler.Steps
{
	using System;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.TypeSystem;

	public class BindBaseTypes : AbstractVisitorCompilerStep
	{
		public BindBaseTypes()
		{
		}
		
		override public void Run()
		{
			Visit(CompileUnit.Modules);
		}
		
		override public void OnEnumDefinition(EnumDefinition node)
		{
		}
		
		override public void OnClassDefinition(ClassDefinition node)
		{
			// Visit type definition's members to resolve base types on nested types
			base.OnClassDefinition(node);

			// Resolve and check base types
			ResolveBaseTypes(new Boo.Lang.List(), node);
			CheckBaseTypes(node);
			
			if (node.IsFinal)
				return;

			if (((IType)node.Entity).IsFinal)
				node.Modifiers |= TypeMemberModifiers.Final;
		}
		
		override public void OnInterfaceDefinition(InterfaceDefinition node)
		{
			ResolveBaseTypes(new Boo.Lang.List(), node);
			CheckInterfaceBaseTypes(node);
		}
		
		void CheckBaseTypes(ClassDefinition node)
		{
			IType baseClass = null;
			foreach (TypeReference baseTypeRef in node.BaseTypes)
			{
				IType baseType = GetType(baseTypeRef);
				if (baseType.IsInterface)
					continue;

				if (null != baseClass)
				{
					Error(
					    CompilerErrorFactory.ClassAlreadyHasBaseType(baseTypeRef,
							node.Name,
							baseClass.FullName)
						);
					continue;
				}
				
				baseClass = baseType;
				if (baseClass.IsFinal && !TypeSystemServices.IsError(baseClass))
				{
					Error(
						CompilerErrorFactory.CannotExtendFinalType(
							baseTypeRef,
							baseClass.FullName));
				}
			}
			
			if (null == baseClass)
				node.BaseTypes.Insert(0, CodeBuilder.CreateTypeReference(TypeSystemServices.ObjectType)	);
		}
		
		void CheckInterfaceBaseTypes(InterfaceDefinition node)
		{
			foreach (TypeReference baseType in node.BaseTypes)
			{
				IType tag = GetType(baseType);
				if (!tag.IsInterface)
				{
					Error(CompilerErrorFactory.InterfaceCanOnlyInheritFromInterface(baseType, node.FullName, tag.FullName));
				}
			}
		}

		void ResolveBaseTypes(Boo.Lang.List visited, TypeDefinition node)
		{
			new BaseTypeResolution(Context, node, visited);
		}
	}

	/// <summary>
	/// Provides a quasi-namespace that can resolve a type's generic parameters before its base types are bound.
	/// </summary>
	internal class GenericParametersNamespaceExtender : AbstractNamespace
	{
		IType _type;
		INamespace _parent;

		public GenericParametersNamespaceExtender(IType type, INamespace currentNamespace)
		{
			_type = type;
			_parent = currentNamespace;
		}

		public override INamespace ParentNamespace
		{
			get 
			{ 
				return _parent; 
			}
		}

		public override IEnumerable<IEntity> GetMembers()
		{
			if (_type.GenericInfo != null)
			{
				return _type.GenericInfo.GenericParameters;
			}
			return NullNamespace.EmptyEntityArray;
		}
	}
}
