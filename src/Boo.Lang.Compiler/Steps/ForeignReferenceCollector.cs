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
using System.Linq;
using Boo.Lang;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Services;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Builders;
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.Steps
{
	public class ForeignReferenceCollector : FastDepthFirstVisitor
	{
		IType _currentType;
		
		List _references;

		List _recursiveReferences;
		
		Hash _referencedEntities;
		
		SelfEntity _selfEntity;
		
		public ForeignReferenceCollector()
		{
			_references = new List();
			_recursiveReferences = new List();
			_referencedEntities = new Hash();
		}
		
		public Node SourceNode { get; set; }

		public Method CurrentMethod { get; set; }
		
		public IType CurrentType
		{
			get { return _currentType; }
			
			set
			{
				_currentType = value;
				if (null != _selfEntity)
					_selfEntity.Type = value;
			}
		}
		
		public List References
		{
			get { return _references; }
		}
		
		public Hash ReferencedEntities
		{
			get { return _referencedEntities; }
		}
		
		public bool ContainsForeignLocalReferences
		{
			get
			{
				foreach (IEntity entity in _referencedEntities.Keys)
				{
					var entityType = entity.EntityType;
					if (entityType == EntityType.Local || entityType == EntityType.Parameter)
						return true;
				}
				return false;
			}
		}
		
		protected IEntity GetSelfEntity()
		{
			return _selfEntity ?? (_selfEntity = new SelfEntity("this", CurrentType));
		}

		protected BooCodeBuilder CodeBuilder
		{
			get { return _codeBuilder; }
		}

		private EnvironmentProvision<BooCodeBuilder> _codeBuilder = new EnvironmentProvision<BooCodeBuilder>();
		
		public BooClassBuilder CreateSkeletonClass(string name, LexicalInfo lexicalInfo)
		{
			var builder = CodeBuilder.CreateClass(name);
			builder.Modifiers |= TypeMemberModifiers.Internal;
			builder.LexicalInfo = lexicalInfo;
			
			builder.AddBaseType(CodeBuilder.TypeSystemServices.ObjectType);
			DeclareFieldsAndConstructor(builder);
			return builder;
		}
		
		public void DeclareFieldsAndConstructor(BooClassBuilder builder)
		{
			// referenced entities turn into fields
			foreach (ITypedEntity entity in Builtins.array(_referencedEntities.Keys))
			{
				Field field = builder.AddInternalField(GetUniqueName(entity.Name), entity.Type);
				_referencedEntities[entity] = field.Entity;
			}

			// single constructor taking all referenced entities
			BooMethodBuilder constructor = builder.AddConstructor();
			constructor.Modifiers = TypeMemberModifiers.Public;			
			constructor.Body.Add(CodeBuilder.CreateSuperConstructorInvocation(builder.Entity.BaseType));
			foreach (ITypedEntity entity in _referencedEntities.Keys)
			{
				InternalField field = (InternalField)_referencedEntities[entity];
				ParameterDeclaration parameter = constructor.AddParameter(field.Name, entity.Type);
				constructor.Body.Add(
					CodeBuilder.CreateAssignment(CodeBuilder.CreateReference(field),
									CodeBuilder.CreateReference(parameter)));
			}
		}

		private string GetUniqueName(string name)
		{
			return _uniqueNameProvider.Instance.GetUniqueName(name);
		}

		private EnvironmentProvision<UniqueNameProvider> _uniqueNameProvider;

		public void AdjustReferences()
		{
			foreach (Expression reference in _references)
			{
				var entity = (InternalField)_referencedEntities[reference.Entity];
				if (null != entity)
					reference.ParentNode.Replace(reference, CodeBuilder.CreateReference(entity));
			}

			foreach (ReferenceExpression reference in _recursiveReferences)
			{
				reference.ParentNode.Replace(
					reference,
					CodeBuilder.MemberReferenceForEntity(
						CodeBuilder.CreateSelfReference((IType)CurrentMethod.DeclaringType.Entity), 
						CurrentMethod.Entity));
			}
		}
		
		public MethodInvocationExpression CreateConstructorInvocationWithReferencedEntities(IType type)
		{
			MethodInvocationExpression mie = CodeBuilder.CreateConstructorInvocation(type.GetConstructors().First());
			foreach (ITypedEntity entity in _referencedEntities.Keys)
				mie.Arguments.Add(CreateForeignReference(entity));
			return mie;
		}
		
		public Expression CreateForeignReference(IEntity entity)
		{
			if (_selfEntity == entity)
				return CodeBuilder.CreateSelfReference(CurrentType);
			return CodeBuilder.CreateReference(entity);
		}

		public override void OnMemberReferenceExpression(MemberReferenceExpression node)
		{
			if (IsRecursiveReference(node))
				_recursiveReferences.Add(node);
			else
				Visit(node.Target);
		}

		override public void OnReferenceExpression(ReferenceExpression node)
		{
			if (IsForeignReference(node))
			{
				_references.Add(node);
				_referencedEntities[node.Entity] = null;
			}
		}
		
		override public void OnSelfLiteralExpression(SelfLiteralExpression node)
		{
			var entity = GetSelfEntity();
			node.Entity = entity;
			_references.Add(node);
			_referencedEntities[entity] = null;
		}

		private bool IsRecursiveReference(Node node)
		{
			return (CurrentMethod != null && node.Entity == CurrentMethod.Entity);
		}
		
		bool IsForeignReference(ReferenceExpression node)
		{
			var entity = node.Entity;
			if (null != entity)
			{
				var type = entity.EntityType;
				if (type == EntityType.Local)
					return null == CurrentMethod || !CurrentMethod.Locals.ContainsEntity(entity);
				if (type == EntityType.Parameter)
					return null == CurrentMethod || !CurrentMethod.Parameters.ContainsEntity(entity);
			}
			return false;
		}
	}
}
