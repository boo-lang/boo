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
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Builders;
using Boo.Lang.Compiler.TypeSystem.Reflection;
using Boo.Lang.Compiler.Util;

namespace Boo.Lang.Compiler.Steps.Generators
{
	class GeneratorExpressionProcessor : AbstractCompilerComponent
	{
		GeneratorExpression _generator;
		
		BooClassBuilder _enumerable;
		
		BooClassBuilder _enumerator;

		BooMethodBuilder _getEnumeratorBuilder;
		
		Field _current;
		
		Field _enumeratorField;
		
		ForeignReferenceCollector _collector;
		
		IType _sourceItemType;
		IType _sourceEnumeratorType; 
		IType _sourceEnumerableType;

		IType _resultItemType;
		IType _resultEnumeratorType; 
		
		public GeneratorExpressionProcessor(CompilerContext context,
		                                    ForeignReferenceCollector collector,
		                                    GeneratorExpression node)
		{
			_collector = collector;
			_generator = node;
			_resultItemType = (IType)_generator["GeneratorItemType"];
			_enumerable = (BooClassBuilder)_generator["GeneratorClassBuilder"];
			_getEnumeratorBuilder = (BooMethodBuilder) _generator["GetEnumeratorBuilder"];
			Initialize(context);
		}
		
		public void Run()
		{
			RemoveReferencedDeclarations();
			CreateAnonymousGeneratorType();
		}
		
		void RemoveReferencedDeclarations()
		{
			Hash referencedEntities = _collector.ReferencedEntities;
			foreach (Declaration d in _generator.Declarations)
			{
				referencedEntities.Remove(d.Entity);
			}
		}
		
		void CreateAnonymousGeneratorType()
		{
			// Set up some important types
			_sourceItemType = TypeSystemServices.ObjectType;
			_sourceEnumeratorType = TypeSystemServices.IEnumeratorType;
			_sourceEnumerableType = TypeSystemServices.IEnumerableType;
			
			_resultEnumeratorType = TypeSystemServices.IEnumeratorGenericType.GenericInfo.ConstructType(_resultItemType);
			
			_enumerator = _collector.CreateSkeletonClass("Enumerator",_generator.LexicalInfo);

			// use a generic enumerator for the source type if possible
			_sourceItemType = TypeSystemServices.GetGenericEnumerableItemType(_generator.Iterator.ExpressionType);
			if (_sourceItemType != null && _sourceItemType != TypeSystemServices.ObjectType)
			{
				_sourceEnumerableType = TypeSystemServices.IEnumerableGenericType.GenericInfo.ConstructType(_sourceItemType);
				_sourceEnumeratorType = TypeSystemServices.IEnumeratorGenericType.GenericInfo.ConstructType(_sourceItemType);
			}
			else
			{
				_sourceItemType = TypeSystemServices.ObjectType;
			}
			
			// Add base types
			_enumerator.AddBaseType(_resultEnumeratorType);
			_enumerator.AddBaseType(TypeSystemServices.Map(typeof(ICloneable)));
			_enumerator.AddBaseType(TypeSystemServices.IDisposableType);
			
			// Add fields
			_enumeratorField = _enumerator.AddField("$$enumerator", _sourceEnumeratorType);
			_current = _enumerator.AddField("$$current", _resultItemType);
			
			// Add methods 
			CreateReset();
			CreateCurrent();
			CreateMoveNext();
			CreateClone();
			CreateDispose();
			
			EnumeratorConstructorMustCallReset();
			
			_collector.AdjustReferences();
			
			_collector.DeclareFieldsAndConstructor(_enumerable);
			
			CreateGetEnumerator();
			_enumerable.ClassDefinition.Members.Add(_enumerator.ClassDefinition);
		}
		
		public MethodInvocationExpression CreateEnumerableConstructorInvocation()
		{
			return _collector.CreateConstructorInvocationWithReferencedEntities(
				_enumerable.Entity);
		}
		
		void EnumeratorConstructorMustCallReset()
		{
			Constructor constructor = _enumerator.ClassDefinition.GetConstructor(0);
			constructor.Body.Add(CreateMethodInvocation(_enumerator.ClassDefinition, "Reset"));
		}
		
		IMethod GetMemberwiseCloneMethod()
		{
			return TypeSystemServices.Map(
				typeof(object).GetMethod("MemberwiseClone",
				                         System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance));
		}
		
		MethodInvocationExpression CreateMethodInvocation(ClassDefinition cd, string name)
		{
			IMethod method = (IMethod)((Method)cd.Members[name]).Entity;
			return CodeBuilder.CreateMethodInvocation(
				CodeBuilder.CreateSelfReference(method.DeclaringType),
				method);
		}
		
		void CreateCurrent()
		{
			Property property = _enumerator.AddReadOnlyProperty("Current", TypeSystemServices.ObjectType);
			property.Getter.Modifiers |= TypeMemberModifiers.Virtual;
			property.Getter.Body.Add(
				new ReturnStatement(
					CodeBuilder.CreateReference(_current)));

			// If item type is object, we're done
			if (_resultItemType == TypeSystemServices.ObjectType) return;
				
			// Since enumerator is generic, this object-typed property should be the 
			// explicit interface implementation for the non-generic IEnumerator interface
			property.ExplicitInfo = new ExplicitMemberInfo();
			property.ExplicitInfo.InterfaceType =
				(SimpleTypeReference)CodeBuilder.CreateTypeReference(TypeSystemServices.IEnumeratorType);
			
			// ...and now we create a typed property for the generic IEnumerator<> interface
			Property typedProperty = _enumerator.AddReadOnlyProperty("Current", _resultItemType);
			typedProperty.Getter.Modifiers |= TypeMemberModifiers.Virtual;
			typedProperty.Getter.Body.Add(
				new ReturnStatement(
					CodeBuilder.CreateReference(_current)));		
		}
		
		void CreateGetEnumerator()
		{
			BooMethodBuilder method = _getEnumeratorBuilder;
			
			MethodInvocationExpression mie = CodeBuilder.CreateConstructorInvocation(_enumerator.ClassDefinition);
			foreach (TypeMember member in _enumerable.ClassDefinition.Members)
			{
				if (NodeType.Field == member.NodeType)
				{
					IField field = (IField)member.Entity;
					mie.Arguments.Add(CodeBuilder.CreateMemberReference(field));
				}
			}
			
			method.Body.Add(new ReturnStatement(mie));
		}
		
		void CreateClone()
		{
			BooMethodBuilder method = _enumerator.AddVirtualMethod("Clone", TypeSystemServices.ObjectType);
			method.Body.Add(
				new ReturnStatement(
					CodeBuilder.CreateMethodInvocation(
						CodeBuilder.CreateSelfReference(_enumerator.Entity),
						GetMemberwiseCloneMethod())));
		}
		
		void CreateReset()
		{
			// Find GetEnumerator method on the source type
			IMethod getEnumerator = (IMethod)GetMember(_sourceEnumerableType, "GetEnumerator", EntityType.Method);

			// Build Reset method that calls GetEnumerator on the source            
			BooMethodBuilder method = _enumerator.AddVirtualMethod("Reset", TypeSystemServices.VoidType);
			method.Body.Add(
				CodeBuilder.CreateAssignment(
					CodeBuilder.CreateReference((InternalField)_enumeratorField.Entity),
					CodeBuilder.CreateMethodInvocation(_generator.Iterator, getEnumerator)));
		}
		
		void CreateMoveNext()
		{
			BooMethodBuilder method = _enumerator.AddVirtualMethod("MoveNext", TypeSystemServices.BoolType);
			
			Expression moveNext = CodeBuilder.CreateMethodInvocation(
				CodeBuilder.CreateReference((InternalField)_enumeratorField.Entity),
				TypeSystemServices.Map(Types.IEnumerator.GetMethod("MoveNext")));
						
			Expression current = CodeBuilder.CreateMethodInvocation(
				CodeBuilder.CreateReference((InternalField)_enumeratorField.Entity),
				((IProperty)GetMember(_sourceEnumeratorType, "Current", EntityType.Property)).GetGetMethod());
			
			Statement filter = null;
			Statement stmt = null;
			Block outerBlock = null;
			Block innerBlock = null;
			
			if (null == _generator.Filter)
			{
				IfStatement istmt = new IfStatement(moveNext, new Block(), null);
				outerBlock = innerBlock = istmt.TrueBlock;
				
				stmt = istmt;
			}
			else
			{
				WhileStatement wstmt = new WhileStatement(moveNext);
				outerBlock = wstmt.Block;
				
				if (StatementModifierType.If == _generator.Filter.Type)
				{
					IfStatement ifstmt = new IfStatement(_generator.Filter.Condition, new Block(), null);
					innerBlock = ifstmt.TrueBlock;
					filter = ifstmt;
				}
				else
				{
					UnlessStatement ustmt = new UnlessStatement(_generator.Filter.Condition);
					innerBlock = ustmt.Block;
					filter = ustmt;
				}
				
				stmt = wstmt;
			}
												
			DeclarationCollection declarations = _generator.Declarations;
			if (declarations.Count > 1)
			{
				NormalizeIterationStatements.UnpackExpression(CodeBuilder,
				                                              method.Method,
				                                              outerBlock,
				                                              current,
				                                              declarations);
												
				foreach (Declaration declaration in declarations)
				{
					method.Locals.Add(((InternalLocal)declaration.Entity).Local);
				}
			}
			else
			{
				InternalLocal local = (InternalLocal)declarations[0].Entity;
				method.Locals.Add(local.Local);
				outerBlock.Add(CodeBuilder.CreateAssignment(
				               	CodeBuilder.CreateReference(local),
				               	current));
			}
			
			if (null != filter)
			{
				outerBlock.Add(filter);
			}
			
			innerBlock.Add(CodeBuilder.CreateAssignment(
			               	CodeBuilder.CreateReference((InternalField)_current.Entity),
			               	_generator.Expression));
			innerBlock.Add(new ReturnStatement(new BoolLiteralExpression(true)));
			
			method.Body.Add(stmt);
			method.Body.Add(new ReturnStatement(new BoolLiteralExpression(false)));
		}
		
		private void CreateDispose()
		{
			BooMethodBuilder dispose = _enumerator.AddVirtualMethod("Dispose", TypeSystemServices.VoidType);
			if (TypeSystemServices.IDisposableType.IsAssignableFrom(_sourceEnumeratorType))
			{
				dispose.Body.Add(CodeBuilder.CreateMethodInvocation(
				                 	CodeBuilder.CreateReference(_enumeratorField),
				                 	Types.IDisposable.GetMethod("Dispose")));
			}
		}

		/// <summary>
		/// Gets the member of the specified type with the specified name, assuming there is only one.
		/// </summary>
		private IEntity GetMember(IType type, string name, EntityType entityType)
		{
			// For external types we can use GetMethod or GetProperty to optimize things a little
			ExternalType external = type as ExternalType;
			if (external != null)
			{
				if (entityType == EntityType.Property)
				{
					return TypeSystemServices.Map(
						external.ActualType.GetProperty(name));
				}
				if (entityType == EntityType.Method)
				{
					return TypeSystemServices.Map(
						external.ActualType.GetMethod(name));

				}
			}

			// For other cases we just scan through the members collection
			return Collections.FindFirst<IEntity>(
				type.GetMembers(), 
				delegate(IEntity e) {
					return entityType == e.EntityType && e.Name == name; 
				});
		}
	}
}