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

namespace Boo.Lang.Compiler.Steps
{
	using System;
	using Boo.Lang;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;	
	
	public class ForeignReferenceCollector : DepthFirstVisitor, IDisposable, ICompilerComponent
	{
		Method _foreignMethod;
		
		List _references;
		
		Hash _referencedEntities;
		
		IEntity _selfEntity;
		
		CompilerContext _context; 
		
		public ForeignReferenceCollector()
		{				
			_references = new List();
			_referencedEntities = new Hash();
		}		
		
		public Method ForeignMethod
		{
			get
			{
				return _foreignMethod;
			}
			
			set
			{
				_foreignMethod = value;
			}
		}
		
		public IType ForeignType
		{
			get
			{
				return (IType)_foreignMethod.DeclaringType.Entity;
			}
		}
		
		public List References
		{
			get
			{
				return _references;
			}
		}
		
		public Hash ReferencedEntities
		{
			get
			{
				return _referencedEntities;
			}
		}
		
		public bool ContainsForeignLocalReferences
		{
			get
			{
				foreach (IEntity entity in _referencedEntities.Keys)
				{
					EntityType type = entity.EntityType;
					if (EntityType.Local == type ||
						EntityType.Parameter == type)
					{
						return true;
					}
				}
				return false;
			}
		}
		
		protected IEntity GetSelfEntity()
		{
			if (null == _selfEntity)
			{
				_selfEntity = new SelfEntity("this", ForeignType);
			}
			return _selfEntity;
		}
		
		protected BooCodeBuilder CodeBuilder
		{
			get
			{
				return _context.CodeBuilder;
			}
		}
		
		public void Initialize(CompilerContext context)
		{			
			if (null == _foreignMethod)
			{
				throw new InvalidOperationException("ForeignMethod was not properly initialized!");
			}
			_context = context;
		}
		
		public void Dispose()
		{
			_context = null;
			_foreignMethod = null;
			_selfEntity = null;
			_references.Clear();
			_referencedEntities.Clear();
		}
		
		public BooClassBuilder CreateSkeletonClass(string name)
		{
			IType baseType = _context.TypeSystemServices.ObjectType;
			
			BooClassBuilder builder = CodeBuilder.CreateClass(name);
			builder.AddBaseType(baseType);
			
			// referenced entities turn into fields			
			foreach (ITypedEntity entity in Builtins.array(_referencedEntities.Keys))
			{
				Field field = builder.AddField("__" + entity.Name, entity.Type);				
				_referencedEntities[entity] = field.Entity;
			}
			
			// single constructor taking all referenced entities
			BooMethodBuilder constructor = builder.AddConstructor();			
			constructor.Body.Add(CodeBuilder.CreateSuperConstructorInvocation(baseType));			
			foreach (ITypedEntity entity in _referencedEntities.Keys)
			{
				ParameterDeclaration parameter = constructor.AddParameter(entity.Name, entity.Type);										
				InternalField field = (InternalField)_referencedEntities[entity];
				constructor.Body.Add(
					CodeBuilder.CreateAssignment(CodeBuilder.CreateReference(field),
									CodeBuilder.CreateReference(parameter)));
			}
			
			return builder;
		}
		
		public void AdjustReferences()
		{ 			
			foreach (Expression reference in _references)
			{				 
				InternalField entity = (InternalField)_referencedEntities[reference.Entity];
				if (null != entity)
				{
					reference.ParentNode.Replace(reference,
							CodeBuilder.CreateReference(entity));
				}
			}
		}
		
		public MethodInvocationExpression CreateConstructorInvocationWithReferencedEntities(IType type)
		{			
			MethodInvocationExpression mie = CodeBuilder.CreateConstructorInvocation(type.GetConstructors()[0]);
			foreach (ITypedEntity entity in _referencedEntities.Keys)
			{
				mie.Arguments.Add(CreateForeignReference(entity));
			}
			return mie;
		}
		
		public Expression CreateForeignReference(IEntity entity)
		{
			if (_selfEntity == entity)
			{
				return CodeBuilder.CreateSelfReference(ForeignType);
			}
			else
			{
				return CodeBuilder.CreateReference(entity);
			}		
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
			IEntity entity = GetSelfEntity();
			node.Entity = entity;
			_references.Add(node);			
			_referencedEntities[entity] = null;
		}
		
		bool IsForeignReference(ReferenceExpression node)
		{
			IEntity entity = node.Entity;
			if (null != entity)
			{
				EntityType type = entity.EntityType;
				if (type == EntityType.Local)
				{
					return _foreignMethod.Locals.ContainsEntity(entity);					
				}
				else if (type == EntityType.Parameter)
				{
					return _foreignMethod.Parameters.ContainsEntity(entity);
				}
			}
			return false;
		}
	}
	
	public class SelfEntity : ITypedEntity
	{
		string _name;
		IType _type;
		
		public SelfEntity(string name, IType type)
		{
			_name = name;
			_type = type;
		}
		
		public string Name
		{
			get
			{
				return _name;
			}
		}
		
		public string FullName
		{
			get
			{
				return _name;
			}
		}
		
		public EntityType EntityType
		{
			get
			{
				return EntityType.Unknown;
			}
		}
		
		public IType Type
		{
			get
			{
				return _type;
			}
		}
	}
}
