#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
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

namespace Boo.Lang.Compiler.TypeSystem.Internal
{
	public class InternalTypeSystemProvider
	{
		public ICompileUnit EntityFor(CompileUnit unit)
		{
			ICompileUnit compileUnit = (ICompileUnit) unit.Entity;
			if (null != compileUnit)
				return compileUnit;

			return Bind(unit, new InternalCompileUnit(unit));
		}

		public INamespace EntityFor(Module module)
		{
			INamespace entity = (INamespace) module.Entity;
			if (null != entity)
				return entity;
			return Bind(module, new InternalModule(this, module));
		}

		public IEntity EntityFor(TypeMember member)
		{
			IEntity entity = member.Entity;
			if (entity != null)
				return entity;
			return Bind(member, CreateEntityForMember(member));
		}

		private IEntity CreateEntityForMember(TypeMember member)
		{
			switch (member.NodeType)
			{
				case NodeType.Module:
					return EntityFor((Module) member);
				case NodeType.InterfaceDefinition:
					return new InternalInterface(this, (TypeDefinition) member);
				case NodeType.ClassDefinition:
					return new InternalClass(this, (ClassDefinition) member);
				case NodeType.Field:
					return new InternalField((Field)member);
				case NodeType.EnumDefinition:
					return new InternalEnum(this, (EnumDefinition) member);
				case NodeType.EnumMember:
					return new InternalEnumMember((EnumMember)member);
				case NodeType.Method:
					return CreateEntityFor((Method)member);
				case NodeType.Constructor:
					return new InternalConstructor(this, (Constructor)member);
				case NodeType.Property:
					return new InternalProperty(this, (Property)member);
				case NodeType.Event:
					return new InternalEvent(this, (Event)member);
			}
			throw new ArgumentException("Member type not supported: " + member.GetType());
		}

		private IEntity CreateEntityFor(Method node)
		{
			return (node.GenericParameters.Count == 0)
				? new InternalMethod(this, node)
				: new InternalGenericMethod(this, node);
		}

		private static TEntity Bind<TNode, TEntity>(TNode node, TEntity entity)
			where TNode : Node
			where TEntity : IEntity
		{
			node.Entity = entity;
			return entity;
		}

		public IParameter[] Map(ParameterDeclarationCollection parameters)
		{
			IParameter[] mapped = new IParameter[parameters.Count];
			for (int i = 0; i < mapped.Length; ++i)
			{
				mapped[i] = (IParameter) parameters[i].Entity;
			}
			return mapped;
		}

		public IType VoidType
		{
			get { return CoreTypeSystemServices().VoidType; }
		}

		private TypeSystemServices CoreTypeSystemServices()
		{
			return My<TypeSystemServices>.Instance;
		}

		public IType ValueTypeType
		{
			get { return CoreTypeSystemServices().ValueTypeType; }
		}

		public IType DuckType
		{
			get { return CoreTypeSystemServices().DuckType; }
		}

		public bool IsSystemObject(IType type)
		{
			return CoreTypeSystemServices().IsSystemObject(type);
		}

		public bool IsCallableTypeAssignableFrom(InternalCallableType type, IType other)
		{
			return CoreTypeSystemServices().IsCallableTypeAssignableFrom(type, other);
		}

		public IType Map(Type type)
		{
			return CoreTypeSystemServices().Map(type);
		}
	}
}
