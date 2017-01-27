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

using System.Linq;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Services;
using Boo.Lang.Compiler.Steps.Generators;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Builders;
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.Steps
{
    using System.Collections.Generic;

    public class ForeignReferenceCollector : FastDepthFirstVisitor
	{
        private IType _currentType;

        private readonly List<Expression> _references;

        private readonly List<MemberReferenceExpression> _recursiveReferences;
		
		private readonly Dictionary<IEntity, InternalField> _referencedEntities;
		
		private SelfEntity _selfEntity;
		
		public ForeignReferenceCollector()
		{
            _references = new List<Expression>();
            _recursiveReferences = new List<MemberReferenceExpression>();
			_referencedEntities = new Dictionary<IEntity, InternalField>();
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

        public List<Expression> References
		{
			get { return _references; }
		}
		
		public Dictionary<IEntity, InternalField> ReferencedEntities
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

		private readonly EnvironmentProvision<BooCodeBuilder> _codeBuilder = new EnvironmentProvision<BooCodeBuilder>();
		
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
		    var keys = _referencedEntities.Keys.Cast<ITypedEntity>().ToArray();
		    foreach (var entity in keys)
		        _collector.Visit(entity.Type);
		    if (_collector.Matches.Any())
		        BuildTypeMap(builder.ClassDefinition);

			// referenced entities turn into fields
            foreach (var entity in keys)
			{
				Field field = builder.AddInternalField(GetUniqueName(entity.Name), _mapper.MapType(entity.Type));
				_referencedEntities[entity] = (InternalField) field.Entity;
			}

			// single constructor taking all referenced entities
			BooMethodBuilder constructor = builder.AddConstructor();
			constructor.Modifiers = TypeMemberModifiers.Public;			
			constructor.Body.Add(CodeBuilder.CreateSuperConstructorInvocation(builder.Entity.BaseType));
			foreach (var entity in _referencedEntities.Keys)
			{
				InternalField field = _referencedEntities[entity];
				ParameterDeclaration parameter = constructor.AddParameter(field.Name, ((ITypedEntity)entity).Type);
				constructor.Body.Add(
					CodeBuilder.CreateAssignment(CodeBuilder.CreateReference(field),
									CodeBuilder.CreateReference(parameter)));
			}
		}

        private readonly TypeCollector _collector = new TypeCollector(type => type is IGenericParameter);

        private readonly GeneratorTypeReplacer _mapper = new GeneratorTypeReplacer();

        private void BuildTypeMap(ClassDefinition newClass)
        {
            string lastName = null;
            IType lastType = null;
            int i = 0;
	        foreach (var newParam in _collector.Matches.Cast<IGenericParameter>().OrderBy(t => t.GenericParameterPosition))
	        {
	            if (!newParam.Name.Equals(lastName))
	            {
	                lastName = newParam.Name;
	                var genParam = CodeBuilder.CreateGenericParameterDeclaration(i, newParam.Name);
	                newClass.GenericParameters.Add(genParam);
	                lastType = (IType) genParam.Entity;
	                ++i;
	            }
                _mapper.Replace(newParam, lastType);
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
				InternalField entity;
                _referencedEntities.TryGetValue(reference.Entity, out entity);
			    if (null != entity)
			    {
			        reference.ParentNode.Replace(reference, CodeBuilder.CreateReference(entity));
			        if (reference.ParentNode.NodeType == NodeType.MemberReferenceExpression)
                        Remap((MemberReferenceExpression)reference.ParentNode);
			    }
			}

			foreach (var reference in _recursiveReferences)
			{
				reference.ParentNode.Replace(
					reference,
					CodeBuilder.MemberReferenceForEntity(
						CodeBuilder.CreateSelfReference((IType)CurrentMethod.DeclaringType.Entity), 
						CurrentMethod.Entity));
			}
		}

	    private static void Remap(MemberReferenceExpression mre)
	    {
            var parentType = ((ITypedEntity)mre.Target.Entity).Type as TypeSystem.Generics.GenericConstructedType;
            if (parentType == null) return;
            var entity = (IMember)mre.Entity;
            var gmm = entity as TypeSystem.Generics.IGenericMappedMember;
            if (gmm != null)
                entity = gmm.SourceMember;
	        mre.Entity = parentType.ConstructedInfo.Map(entity);
            mre.ExpressionType = ((ITypedEntity)mre.Entity).Type;
	    }

	    public MethodInvocationExpression CreateConstructorInvocationWithReferencedEntities(IType type, Method containingMethod)
	    {
            GeneratorTypeReplacer mapper;
            type = GeneratorTypeReplacer.MapTypeInMethodContext(type, containingMethod, out mapper);
	        MethodInvocationExpression mie = CodeBuilder.CreateConstructorInvocation(type.GetConstructors().First());
			foreach (var entity in _referencedEntities.Keys)
				mie.Arguments.Add(CreateForeignReference(entity));
            if (mapper != null)
                mie.Accept(new GenericTypeMapper(mapper));
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

		public override void OnReferenceExpression(ReferenceExpression node)
		{
			if (IsForeignReference(node))
			{
				_references.Add(node);
				_referencedEntities[node.Entity] = null;
			}
		}
		
		public override void OnSelfLiteralExpression(SelfLiteralExpression node)
		{
			var entity = GetSelfEntity();
			node.Entity = entity;
			_references.Add(node);
			_referencedEntities[entity] = null;
		}

        private bool IsRecursiveReference(MemberReferenceExpression node)
		{
			return CurrentMethod != null && node.Entity == CurrentMethod.Entity;
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
