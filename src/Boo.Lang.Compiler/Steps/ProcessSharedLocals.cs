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

using Boo.Lang.Compiler.TypeSystem.Builders;

namespace Boo.Lang.Compiler.Steps
{
	using System.Collections;
	using Boo.Lang;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;
	
	public class ProcessSharedLocals : AbstractTransformerCompilerStep
	{
		Method _currentMethod;
		
		ClassDefinition _sharedLocalsClass;
		
		Hashtable _mappings = new Hashtable();
		
		List _references = new List();
		
		List _shared = new List();
		
		int _closureDepth;
		
		override public void Run()
		{
			Visit(CompileUnit);
		}
		
		override public void Dispose()
		{
			_shared.Clear();
			_references.Clear();
			_mappings.Clear();
			base.Dispose();
		}
		
		override public void OnField(Field node)
		{
		}
		
		override public void OnInterfaceDefinition(InterfaceDefinition node)
		{
		}
		
		override public void OnEnumDefinition(EnumDefinition node)
		{
		}
		
		override public void OnConstructor(Constructor node)
		{
			OnMethod(node);
		}
		
		override public void OnMethod(Method node)
		{
			_references.Clear();
			_mappings.Clear();
			_currentMethod = node;
			_sharedLocalsClass = null;
			_closureDepth = 0;
			
			Visit(node.Body);
			
			CreateSharedLocalsClass();
			if (null != _sharedLocalsClass)
			{
				node.DeclaringType.Members.Add(_sharedLocalsClass);
				Map();
			}
		}

		override public void OnBlockExpression(BlockExpression node)
		{
			++_closureDepth;
			Visit(node.Body);
			--_closureDepth;
		}
		
		override public void OnGeneratorExpression(GeneratorExpression node)
		{
			++_closureDepth;
			Visit(node.Iterator);
			Visit(node.Expression);
			Visit(node.Filter);
			--_closureDepth;
		}
		
		override public void OnReferenceExpression(ReferenceExpression node)
		{
			ILocalEntity local = node.Entity as ILocalEntity;
			if (null == local) return;
			if (local.IsPrivateScope) return;
			
			_references.Add(node);
			
			if (_closureDepth == 0) return;
			
			local.IsShared = _currentMethod.Locals.ContainsEntity(local)
							|| _currentMethod.Parameters.ContainsEntity(local);
			
		}
		
		void Map()
		{
			IType type = (IType)_sharedLocalsClass.Entity;
			InternalLocal locals = CodeBuilder.DeclareLocal(_currentMethod, "$locals", type);
			
			foreach (ReferenceExpression reference in _references)
			{
				IField mapped = (IField)_mappings[reference.Entity];
				if (null == mapped) continue;
				
				reference.ParentNode.Replace(
					reference,
					CodeBuilder.CreateMemberReference(
						CodeBuilder.CreateReference(locals),
						mapped));
			}
			
			Block initializationBlock = new Block();
			initializationBlock.Add(CodeBuilder.CreateAssignment(
						CodeBuilder.CreateReference(locals),
						CodeBuilder.CreateConstructorInvocation(type.GetConstructors()[0])));
			InitializeSharedParameters(initializationBlock, locals);
			_currentMethod.Body.Statements.Insert(0, initializationBlock);
						
			foreach (IEntity entity in _mappings.Keys)
			{
				_currentMethod.Locals.RemoveByEntity(entity);
			}
		}
		
		void InitializeSharedParameters(Block block, InternalLocal locals)
		{
			foreach (Node node in _currentMethod.Parameters)
			{
				InternalParameter param = (InternalParameter)node.Entity;
				if (param.IsShared)
				{
					block.Add(
						CodeBuilder.CreateAssignment(
							CodeBuilder.CreateMemberReference(
								CodeBuilder.CreateReference(locals),
								(IField)_mappings[param]),
							CodeBuilder.CreateReference(param)));
				}
			}
		}
		
		void CreateSharedLocalsClass()
		{
			_shared.Clear();
			
			CollectSharedLocalEntities(_currentMethod.Locals);
			CollectSharedLocalEntities(_currentMethod.Parameters);
			
			if (_shared.Count > 0)
			{
				BooClassBuilder builder = CodeBuilder.CreateClass(Context.GetUniqueName(_currentMethod.Name, "locals"));
				builder.Modifiers |= TypeMemberModifiers.Internal;
				builder.AddBaseType(TypeSystemServices.ObjectType);
				
				int i=0;
				foreach (ILocalEntity local in _shared)
				{
					Field field = builder.AddInternalField(
									string.Format("${0}", local.Name),
									local.Type);
					++i;
					
					_mappings[local] = field.Entity;
				}
				
				builder.AddConstructor().Body.Add(
					CodeBuilder.CreateSuperConstructorInvocation(TypeSystemServices.ObjectType));
					
				_sharedLocalsClass = builder.ClassDefinition;
			}
		}
		
		void CollectSharedLocalEntities<T>(System.Collections.Generic.IEnumerable<T> nodes) where T : Node
		{
			foreach (T node in nodes)
			{
				ILocalEntity local = (ILocalEntity)node.Entity;
				if (local.IsShared)
				{
					_shared.Add(local);
				}
			}
		}
	}
}
