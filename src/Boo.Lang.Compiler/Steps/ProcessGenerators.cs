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
	using System.Collections;
	using Boo.Lang;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;
	
	public class ProcessGenerators : AbstractTransformerCompilerStep
	{
		static System.Reflection.ConstructorInfo List_IEnumerableConstructor = Types.List.GetConstructor(new Type[] { Types.IEnumerable });
		
		Method _current;
		
		override public void Run()
		{
			Visit(CompileUnit.Modules);
		}
		
		override public void OnInterfaceDefinition(InterfaceDefinition node)
		{
			// ignore
		}
		
		override public void OnEnumDefinition(EnumDefinition node)
		{
			// ignore
		}
		
		override public void OnField(Field node)
		{
			// ignore
		}
		
		override public void OnConstructor(Constructor method)
		{
			_current = method;
			Visit(_current.Body);
		}
		
		override public bool EnterMethod(Method method)
		{
			_current = method;
			return true;
		}
		
		override public void LeaveMethod(Method method)
		{
			InternalMethod entity = (InternalMethod)method.Entity;
			if (entity.IsGenerator)
			{
				GeneratorMethodProcessor processor = new GeneratorMethodProcessor(_context, entity);
				processor.Run();
			}
		}

		override public void OnListLiteralExpression(ListLiteralExpression node)
		{
			bool generator = AstUtil.IsListGenerator(node);
			Visit(node.Items);
			if (generator)
			{
				ReplaceCurrentNode(
					CodeBuilder.CreateConstructorInvocation(
						TypeSystemServices.Map(List_IEnumerableConstructor),
						node.Items[0]));
			}
		}
		
		override public void LeaveGeneratorExpression(GeneratorExpression node)
		{				
			using (ForeignReferenceCollector collector = new ForeignReferenceCollector())
			{
				collector.CurrentType = (IType)AstUtil.GetParentClass(node).Entity;				
				collector.Initialize(_context);
				collector.Visit(node);

				GeneratorExpressionProcessor processor = new GeneratorExpressionProcessor(_context, collector, node);
				processor.Run();
				ReplaceCurrentNode(processor.CreateEnumerableConstructorInvocation());
			}
		}
	}
	
	class GeneratorMethodProcessor : AbstractTransformerCompilerStep
	{
		InternalMethod _generator;
		
		InternalMethod _moveNext;
		
		BooClassBuilder _enumerable;
		
		BooClassBuilder _enumerator;
		
		BooMethodBuilder _enumeratorConstructor;
		
		BooMethodBuilder _enumerableConstructor;
		
		IField _state;
		
		IMethod _yield;
		
		Field _externalEnumeratorSelf;
		
		List _labels;
		
		Hashtable _mapping;
		
		public GeneratorMethodProcessor(CompilerContext context, InternalMethod method)
		{
			_labels = new List();
			_mapping = new Hashtable();
			_generator = method;			
			Initialize(context);
		}
		
		public InternalMethod MoveNextMethod
		{
			get
			{
				return _moveNext;
			}
		}
		
		override public void Run()
		{
			_enumerable = (BooClassBuilder)_generator.Method["GeneratorClassBuilder"];
			if (null == _enumerable)
			{
				Errors.Add(CompilerErrorFactory.InternalError(_generator.Method, null));
			}
			
			CreateEnumerableConstructor();
			CreateEnumerator();
			
			MethodInvocationExpression enumerableConstructorInvocation = CodeBuilder.CreateConstructorInvocation(_enumerable.ClassDefinition);
			MethodInvocationExpression enumeratorConstructorInvocation = CodeBuilder.CreateConstructorInvocation(_enumerator.ClassDefinition);
			
			// propagate the necessary parameters from
			// the enumerable to the enumerator
			foreach (ParameterDeclaration parameter in _generator.Method.Parameters)
			{
				InternalParameter entity = (InternalParameter)parameter.Entity;				
				if (entity.IsUsed)
				{
					enumerableConstructorInvocation.Arguments.Add(
						CodeBuilder.CreateReference(parameter));
						
					PropagateFromEnumerableToEnumerator(enumeratorConstructorInvocation,
														entity.Name,
														entity.Type);
				}
			}
			
			// propagate the external self reference if necessary
			if (null != _externalEnumeratorSelf)
			{				
				IType type = (IType)_externalEnumeratorSelf.Type.Entity;
				enumerableConstructorInvocation.Arguments.Add(
					CodeBuilder.CreateSelfReference(type));
					
				PropagateFromEnumerableToEnumerator(enumeratorConstructorInvocation,
														"self_",
														type);
			}

			
			CreateGetEnumerator(enumeratorConstructorInvocation);
			
			Block body = _generator.Method.Body;
			body.Clear();
			body.Add(new ReturnStatement(enumerableConstructorInvocation));
		}
		
		void CreateGetEnumerator(Expression enumeratorExpression)
		{	
			BooMethodBuilder method = (BooMethodBuilder)_generator.Method["GetEnumeratorBuilder"];
			method.Body.Add(new ReturnStatement(enumeratorExpression));
		}
		
		void CreateEnumerableConstructor()
		{
			_enumerableConstructor = CreateConstructor(_enumerable);
		}
		
		void CreateEnumeratorConstructor()
		{
			_enumeratorConstructor = CreateConstructor(_enumerator);
		}
		
		void CreateEnumerator()
		{
			IType abstractEnumeratorType = TypeSystemServices.Map(typeof(Boo.Lang.AbstractGeneratorEnumerator));
			
			_state = NameResolutionService.ResolveField(abstractEnumeratorType, "_state");
			_yield = NameResolutionService.ResolveMethod(abstractEnumeratorType, "Yield");
			
			_enumerator = CodeBuilder.CreateClass("Enumerator");
			_enumerator.AddBaseType(abstractEnumeratorType);
			_enumerator.AddBaseType(TypeSystemServices.IEnumeratorType);
			
			CreateEnumeratorConstructor();
			CreateMoveNext();			
			
			_enumerable.ClassDefinition.Members.Add(_enumerator.ClassDefinition);
		}
		
		void CreateMoveNext()
		{
			Method generator = _generator.Method;
			
			BooMethodBuilder mn = _enumerator.AddVirtualMethod("MoveNext", TypeSystemServices.BoolType);
			_moveNext = mn.Entity;
			
			foreach (Local local in generator.Locals)
			{
				InternalLocal entity = (InternalLocal)local.Entity;
				
				Field field = _enumerator.AddField("___" + entity.Name + _context.AllocIndex(), entity.Type);
				_mapping[entity] = field.Entity;
			}
			generator.Locals.Clear();
			
			foreach (ParameterDeclaration parameter in generator.Parameters)
			{
				InternalParameter entity = (InternalParameter)parameter.Entity;				
				if (entity.IsUsed)
				{
					Field field = DeclareFieldInitializedFromConstructorParameter(_enumerator, _enumeratorConstructor,
												entity.Name,
												entity.Type);
					_mapping[entity] = field.Entity;					
				}
			}
			
			mn.Body.Add(CreateLabel(generator));
			mn.Body.Add(generator.Body);
			generator.Body.Clear();
			
			Visit(mn.Body);
			
			mn.Body.Add(CreateYieldInvocation(CodeBuilder.CreateNullLiteral()));
			mn.Body.Add(CreateLabel(generator));
			
			mn.Body.Insert(0,
				CodeBuilder.CreateSwitch(
					CodeBuilder.CreateMemberReference(_state),
					_labels));
		}
		
		void PropagateFromEnumerableToEnumerator(MethodInvocationExpression enumeratorConstructorInvocation,
												string parameterName,
												IType parameterType)
		{
			Field field = DeclareFieldInitializedFromConstructorParameter(_enumerable, _enumerableConstructor, parameterName, parameterType);					
			enumeratorConstructorInvocation.Arguments.Add(
					CodeBuilder.CreateReference(field));
		}
		
		Field DeclareFieldInitializedFromConstructorParameter(BooClassBuilder type,
													BooMethodBuilder constructor,
													string parameterName,
													IType parameterType)
		{
			Field field = type.AddField("___" + parameterName + _context.AllocIndex(), parameterType);
			InitializeFieldFromConstructorParameter(constructor, field, parameterName, parameterType);
			return field;
		}
		
		void InitializeFieldFromConstructorParameter(BooMethodBuilder constructor,
											Field field,
											string parameterName,
											IType parameterType)
		{
			ParameterDeclaration parameter = constructor.AddParameter(parameterName, parameterType);
			constructor.Body.Add(
				CodeBuilder.CreateAssignment(
					CodeBuilder.CreateReference(field),
					CodeBuilder.CreateReference(parameter)));
		}
		
		override public void OnReferenceExpression(ReferenceExpression node)
		{
			InternalField mapped = (InternalField)_mapping[node.Entity];
			if (null != mapped)
			{
				ReplaceCurrentNode(
					CodeBuilder.CreateMemberReference(
						CodeBuilder.CreateSelfReference(_enumerator.Entity),
						mapped));
			}
		}
		
		override public void OnSelfLiteralExpression(SelfLiteralExpression node)
		{			
			if (null == _externalEnumeratorSelf)
			{
				IType type = (IType)node.ExpressionType;
				_externalEnumeratorSelf = DeclareFieldInitializedFromConstructorParameter(
													_enumerator,
													_enumeratorConstructor,													
													"self_",	 type);
			}
			
			ReplaceCurrentNode(CodeBuilder.CreateReference(_externalEnumeratorSelf));
		}
		
		override public void LeaveYieldStatement(YieldStatement node)
		{						
			Block block = new Block();			
			block.Add(
				new ReturnStatement(
					node.LexicalInfo,
					CreateYieldInvocation(node.Expression),
					null));				
			block.Add(CreateLabel(node));					
			ReplaceCurrentNode(block);
		}
		
		MethodInvocationExpression CreateYieldInvocation(Expression value)
		{
			return CodeBuilder.CreateMethodInvocation(
					CodeBuilder.CreateSelfReference(_enumerator.Entity),
					_yield,
					CodeBuilder.CreateIntegerLiteral(_labels.Count),
					value);
		}
		
		LabelStatement CreateLabel(Node sourceNode)
		{
			InternalLabel label = CodeBuilder.CreateLabelStatement(sourceNode,
									"___state" + _labels.Count);
			_labels.Add(label.LabelStatement);
			_moveNext.AddLabel(label);
			return label.LabelStatement;
		}
		
		BooMethodBuilder CreateConstructor(BooClassBuilder builder)
		{	
			BooMethodBuilder constructor = builder.AddConstructor();			
			constructor.Body.Add(CodeBuilder.CreateSuperConstructorInvocation(builder.Entity.BaseType));
			return constructor; 
		}
	}
	
	class GeneratorExpressionProcessor : AbstractCompilerComponent
	{		
		GeneratorExpression _generator;
		
		BooClassBuilder _enumerable;
		
		BooClassBuilder _enumerator;
		
		Field _current;
		
		Field _enumeratorField;
		
		ForeignReferenceCollector _collector;
		
		public GeneratorExpressionProcessor(CompilerContext context,
								ForeignReferenceCollector collector,
								GeneratorExpression node)
		{			
			_collector = collector;
			_generator = node;
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
			_enumerable = (BooClassBuilder)_generator["GeneratorClassBuilder"];
			
			_enumerator = _collector.CreateSkeletonClass("Enumerator");
			_enumerator.AddBaseType(TypeSystemServices.IEnumeratorType);
			_enumerator.AddBaseType(TypeSystemServices.Map(typeof(ICloneable)));
			
			_enumeratorField = _enumerator.AddField("____enumerator",
									TypeSystemServices.IEnumeratorType);
			_current = _enumerator.AddField("____current",
									TypeSystemServices.ObjectType);
			
			CreateReset();
			CreateCurrent();
			CreateMoveNext();
			CreateClone();
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
		}
		
		void CreateGetEnumerator()
		{	
			BooMethodBuilder method = (BooMethodBuilder)_generator["GetEnumeratorBuilder"];
			
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
						CodeBuilder.CreateSelfReference((IType)_enumerator.Entity),
						GetMemberwiseCloneMethod())));
		}
		
		void CreateReset()
		{
			BooMethodBuilder method = _enumerator.AddVirtualMethod("Reset", TypeSystemServices.VoidType);
			method.Body.Add(
				CodeBuilder.CreateAssignment(
					CodeBuilder.CreateReference((InternalField)_enumeratorField.Entity),
					CodeBuilder.CreateMethodInvocation(_generator.Iterator,
						TypeSystemServices.Map(Types.IEnumerable.GetMethod("GetEnumerator")))));
		}
		
		void CreateMoveNext()
		{
			BooMethodBuilder method = _enumerator.AddVirtualMethod("MoveNext", TypeSystemServices.BoolType);
			
			Expression moveNext = CodeBuilder.CreateMethodInvocation(
												CodeBuilder.CreateReference((InternalField)_enumeratorField.Entity),
												TypeSystemServices.Map(Types.IEnumerator.GetMethod("MoveNext")));
												
			Expression current = CodeBuilder.CreateMethodInvocation(
									CodeBuilder.CreateReference((InternalField)_enumeratorField.Entity),
									TypeSystemServices.Map(Types.IEnumerator.GetProperty("Current").GetGetMethod()));
			
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
	}
}
