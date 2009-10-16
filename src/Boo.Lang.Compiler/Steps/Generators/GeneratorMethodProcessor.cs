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

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Builders;
using Boo.Lang.Compiler.TypeSystem.Internal;

namespace Boo.Lang.Compiler.Steps.Generators
{
	internal class GeneratorMethodProcessor : AbstractTransformerCompilerStep
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
		System.Collections.Generic.List<TryStatementInfo> _tryStatementInfoForLabels = new System.Collections.Generic.List<TryStatementInfo>();
		
		Hashtable _mapping;
		
		IType _generatorItemType;

		BooMethodBuilder _getEnumeratorBuilder;

		/// <summary>
		/// used for expressionless yield statements when the generator type
		/// is a value type (and thus 'null' is not an appropriate value)
		/// </summary>
		Field _nullValueField;

		public GeneratorMethodProcessor(CompilerContext context, InternalMethod method)
		{
			_labels = new List();
			_mapping = new Hashtable();
			_generator = method;
			_generatorItemType = (IType)_generator.Method["GeneratorItemType"];
			_enumerable = (BooClassBuilder)_generator.Method["GeneratorClassBuilder"];
			_getEnumeratorBuilder = (BooMethodBuilder) _generator.Method["GetEnumeratorBuilder"];
			Debug.Assert(null != _generatorItemType);
			Debug.Assert(null != _enumerable);
			Initialize(context);
		}
		
		public LexicalInfo LexicalInfo
		{
			get { return _generator.Method.LexicalInfo; }
		}
		
		public InternalMethod MoveNextMethod
		{
			get { return _moveNext; }
		}
		
		override public void Run()
		{
			CreateEnumerableConstructor();
			CreateEnumerator();
			MethodInvocationExpression enumerableConstructorInvocation = CodeBuilder.CreateConstructorInvocation(_enumerable.ClassDefinition);
			MethodInvocationExpression enumeratorConstructorInvocation = CodeBuilder.CreateConstructorInvocation(_enumerator.ClassDefinition);
			PropagateReferences(enumerableConstructorInvocation, enumeratorConstructorInvocation);
			CreateGetEnumeratorBody(enumeratorConstructorInvocation);
			FixGeneratorMethodBody(enumerableConstructorInvocation);
		}
		
		void FixGeneratorMethodBody(MethodInvocationExpression enumerableConstructorInvocation)
		{
			Block body = _generator.Method.Body;
			body.Clear();

			body.Add(
				new ReturnStatement(
					_generator.Method.LexicalInfo,
					GeneratorReturnsIEnumerator()
						? CreateGetEnumeratorInvocation(enumerableConstructorInvocation)
						: enumerableConstructorInvocation));
		}
		
		void PropagateReferences(MethodInvocationExpression enumerableConstructorInvocation,
		                         MethodInvocationExpression enumeratorConstructorInvocation)
		{
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
				enumerableConstructorInvocation.Arguments.Add(CodeBuilder.CreateSelfReference(type));
				
				PropagateFromEnumerableToEnumerator(enumeratorConstructorInvocation,
				                                    "self_",
				                                    type);
			}
			
		}

		private MethodInvocationExpression CreateGetEnumeratorInvocation(MethodInvocationExpression enumerableConstructorInvocation)
		{
			return CodeBuilder.CreateMethodInvocation(
				enumerableConstructorInvocation,
				GetGetEnumeratorEntity());
		}

		private InternalMethod GetGetEnumeratorEntity()
		{
			return _getEnumeratorBuilder.Entity;
		}

		private bool GeneratorReturnsIEnumerator()
		{
			bool returnsEnumerator = _generator.ReturnType == TypeSystemServices.IEnumeratorType;
			returnsEnumerator |=
				_generator.ReturnType.ConstructedInfo != null &&
				_generator.ReturnType.ConstructedInfo.GenericDefinition == TypeSystemServices.IEnumeratorGenericType;
			
			return returnsEnumerator;
		}

		void CreateGetEnumeratorBody(Expression enumeratorExpression)
		{
			_getEnumeratorBuilder.Body.Add(
				new ReturnStatement(enumeratorExpression));
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
			IType abstractEnumeratorType =
				TypeSystemServices.Map(typeof(Boo.Lang.GenericGeneratorEnumerator<>)).
					GenericInfo.ConstructType(new IType[] {_generatorItemType});
			
			_state = NameResolutionService.ResolveField(abstractEnumeratorType, "_state");
			_yield = NameResolutionService.ResolveMethod(abstractEnumeratorType, "Yield");
			
			_enumerator = CodeBuilder.CreateClass("$");
			_enumerator.AddAttribute(CodeBuilder.CreateAttribute(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute)));
			_enumerator.Modifiers |= TypeMemberModifiers.Final;
			_enumerator.LexicalInfo = this.LexicalInfo;
			_enumerator.AddBaseType(abstractEnumeratorType);
			_enumerator.AddBaseType(TypeSystemServices.IEnumeratorType);
			
			CreateEnumeratorConstructor();
			CreateMoveNext();
			
			_enumerable.ClassDefinition.Members.Add(_enumerator.ClassDefinition);
			//new Boo.Lang.Compiler.Ast.Visitors.BooPrinterVisitor(System.Console.Out).Visit(_enumerator.ClassDefinition);
		}
		
		void CreateMoveNext()
		{
			Method generator = _generator.Method;
			
			BooMethodBuilder methodBuilder = _enumerator.AddVirtualMethod("MoveNext", TypeSystemServices.BoolType);
			methodBuilder.Method.LexicalInfo = generator.LexicalInfo;
			_moveNext = methodBuilder.Entity;
			
			TransformLocalsIntoFields(generator);

			TransformParametersIntoFieldsInitializedByConstructor(generator);
			
			methodBuilder.Body.Add(CreateLabel(generator));

			// Visit() needs to know the number of the finished state
			_finishedStateNumber = _labels.Count;
			LabelStatement finishedLabel = CreateLabel(generator);
			methodBuilder.Body.Add(generator.Body);
			generator.Body.Clear();
			
			Visit(methodBuilder.Body);
			
			methodBuilder.Body.Add(CreateYieldInvocation(null, _finishedStateNumber));
			methodBuilder.Body.Add(finishedLabel);
			
			methodBuilder.Body.Insert(0,
			                          CodeBuilder.CreateSwitch(
			                          	this.LexicalInfo,
			                          	CodeBuilder.CreateMemberReference(_state),
			                          	_labels));
			
			// if the method contains converted try statements, put it in a try/failure block
			if (_convertedTryStatements.Count > 0)
			{
				IMethod dispose = CreateDisposeMethod();
				
				TryStatement tryFailure = new TryStatement();
				tryFailure.ProtectedBlock.Add(methodBuilder.Body);
				tryFailure.FailureBlock = new Block();
				tryFailure.FailureBlock.Add(CallMethodOnSelf(dispose));
				methodBuilder.Body.Clear();
				methodBuilder.Body.Add(tryFailure);
			}
		}

		private void TransformParametersIntoFieldsInitializedByConstructor(Method generator)
		{
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
		}

		private void TransformLocalsIntoFields(Method generator)
		{
			foreach (Local local in generator.Locals)
			{
				InternalLocal entity = (InternalLocal)local.Entity;
				if (IsExceptionHandlerVariable(entity))
				{
					AddToMoveNextMethod(local);
					continue;
				}

				AddInternalFieldFor(entity);
			}
			generator.Locals.Clear();
		}

		private void AddToMoveNextMethod(Local local)
		{
			_moveNext.Method.Locals.Add(local);
		}

		private void AddInternalFieldFor(InternalLocal entity)
		{
			Field field = _enumerator.AddInternalField(Context.GetUniqueName(entity.Name), entity.Type);
			_mapping[entity] = field.Entity;
		}

		private bool IsExceptionHandlerVariable(InternalLocal local)
		{
			Declaration originalDeclaration = local.OriginalDeclaration;
			if (originalDeclaration == null) return false;
			return originalDeclaration.ParentNode is ExceptionHandler;
		}

		MethodInvocationExpression CallMethodOnSelf(IMethod method)
		{
			return CodeBuilder.CreateMethodInvocation(
				CodeBuilder.CreateSelfReference(_enumerator.Entity),
				method);
		}
		
		IMethod CreateDisposeMethod()
		{
			BooMethodBuilder mn = _enumerator.AddVirtualMethod("Dispose", TypeSystemServices.VoidType);
			mn.Method.LexicalInfo = this.LexicalInfo;
			
			LabelStatement noEnsure = CodeBuilder.CreateLabel(_generator.Method, "noEnsure").LabelStatement;
			mn.Body.Add(noEnsure);
			mn.Body.Add(SetStateTo(_finishedStateNumber));
			mn.Body.Add(new ReturnStatement());
			
			// Create a section calling all ensure methods for each converted try block
			LabelStatement[] disposeLabels = new LabelStatement[_labels.Count];
			for (int i = 0; i < _convertedTryStatements.Count; i++) {
				TryStatementInfo info = _convertedTryStatements[i];
				disposeLabels[info._stateNumber] = CodeBuilder.CreateLabel(_generator.Method, "$ensure_" + info._stateNumber).LabelStatement;
				mn.Body.Add(disposeLabels[info._stateNumber]);
				mn.Body.Add(SetStateTo(_finishedStateNumber));
				Block block = mn.Body;
				while (info._parent != null) {
					TryStatement ts = new TryStatement();
					block.Add(ts);
					ts.ProtectedBlock.Add(CallMethodOnSelf(info._ensureMethod));
					block = ts.EnsureBlock = new Block();
					info = info._parent;
				}
				block.Add(CallMethodOnSelf(info._ensureMethod));
				mn.Body.Add(new ReturnStatement());
			}
			
			// now map the labels of the suspended states to the labels we just created
			for (int i = 0; i < _labels.Count; i++) {
				if (_tryStatementInfoForLabels[i] == null)
					disposeLabels[i] = noEnsure;
				else
					disposeLabels[i] = disposeLabels[_tryStatementInfoForLabels[i]._stateNumber];
			}
			
			mn.Body.Insert(0, CodeBuilder.CreateSwitch(
			                  	this.LexicalInfo,
			                  	CodeBuilder.CreateMemberReference(_state),
			                  	disposeLabels));
			return mn.Entity;
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
			Field field = type.AddInternalField(Context.GetUniqueName(parameterName), parameterType);
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
						node.LexicalInfo,
						CodeBuilder.CreateSelfReference(_enumerator.Entity),
						mapped));
			}
		}
		
		override public void OnSelfLiteralExpression(SelfLiteralExpression node)
		{
			if (null == _externalEnumeratorSelf)
			{
				IType type = node.ExpressionType;
				_externalEnumeratorSelf = DeclareFieldInitializedFromConstructorParameter(
					_enumerator,
					_enumeratorConstructor,
					"self_",
					type);
			}
			
			ReplaceCurrentNode(CodeBuilder.CreateReference(node.LexicalInfo, _externalEnumeratorSelf));
		}
		
		sealed class TryStatementInfo
		{
			internal TryStatement _statement;
			internal TryStatementInfo _parent;
			
			internal bool _containsYield;
			internal int _stateNumber = -1;
			internal Block _replacement;
			
			internal IMethod _ensureMethod;
		}
		
		System.Collections.Generic.List<TryStatementInfo> _convertedTryStatements = new System.Collections.Generic.List<TryStatementInfo>();
		Stack<TryStatementInfo> _tryStatementStack = new Stack<TryStatementInfo>();
		int _finishedStateNumber;

		public override bool EnterTryStatement(TryStatement node)
		{
			TryStatementInfo info = new TryStatementInfo();
			info._statement = node;
			if (_tryStatementStack.Count > 0)
				info._parent = _tryStatementStack.Peek();
			_tryStatementStack.Push(info);
			return true;
		}
		
		BinaryExpression SetStateTo(int num)
		{
			return CodeBuilder.CreateAssignment(CodeBuilder.CreateMemberReference(_state),
			                                    CodeBuilder.CreateIntegerLiteral(num));
		}
		
		public override void LeaveTryStatement(TryStatement node)
		{
			TryStatementInfo info = _tryStatementStack.Pop();
			if (info._containsYield) {
				ReplaceCurrentNode(info._replacement);
				TryStatementInfo currentTry = (_tryStatementStack.Count > 0) ? _tryStatementStack.Peek() : null;
				info._replacement.Add(node.ProtectedBlock);
				if (currentTry != null) {
					ConvertTryStatement(currentTry);
					info._replacement.Add(SetStateTo(currentTry._stateNumber));
				} else {
					// leave try block, reset state to prevent ensure block from being called again
					info._replacement.Add(SetStateTo(_finishedStateNumber));
				}
				BooMethodBuilder ensureMethod = _enumerator.AddMethod("$ensure" + info._stateNumber, TypeSystemServices.VoidType, TypeMemberModifiers.Private);
				ensureMethod.Body.Add(info._statement.EnsureBlock);
				info._ensureMethod = ensureMethod.Entity;
				info._replacement.Add(CallMethodOnSelf(ensureMethod.Entity));
				_convertedTryStatements.Add(info);
			}
		}
		
		void ConvertTryStatement(TryStatementInfo currentTry)
		{
			if (currentTry._containsYield)
				return;
			currentTry._containsYield = true;
			currentTry._stateNumber = _labels.Count;
			Block tryReplacement = new Block();
			//tryReplacement.Add(CreateLabel(tryReplacement));
			// when the MoveNext() is called while the enumerator is still in running state, don't jump to the
			// try block, but handle it like MoveNext() calls when the enumerator is in the finished state.
			_labels.Add(_labels[_finishedStateNumber]);
			_tryStatementInfoForLabels.Add(currentTry);
			tryReplacement.Add(SetStateTo(currentTry._stateNumber));
			currentTry._replacement = tryReplacement;
		}
		
		override public void LeaveYieldStatement(YieldStatement node)
		{
			TryStatementInfo currentTry = (_tryStatementStack.Count > 0) ? _tryStatementStack.Peek() : null;
			if (currentTry != null) {
				ConvertTryStatement(currentTry);
			}
			Block block = new Block();
			block.Add(
				new ReturnStatement(
					node.LexicalInfo,
					CreateYieldInvocation(node.Expression, _labels.Count),
					null));
			block.Add(CreateLabel(node));
			// setting the state back to the "running" state not required, as that state has the same ensure blocks
			// as the state we are currently in.
//			if (currentTry != null) {
//				block.Add(SetStateTo(currentTry._stateNumber));
//			}
			ReplaceCurrentNode(block);
		}
		
		MethodInvocationExpression CreateYieldInvocation(Expression value, int newState)
		{
			MethodInvocationExpression invocation = CodeBuilder.CreateMethodInvocation(
				CodeBuilder.CreateSelfReference(_enumerator.Entity),
				_yield,
				CodeBuilder.CreateIntegerLiteral(newState),
				value == null ? GetDefaultYieldValue() : value);
			if (null != value) invocation.LexicalInfo = value.LexicalInfo;
			return invocation;
		}

		private Expression GetDefaultYieldValue()
		{
			if (_generatorItemType.IsValueType)
			{
				if (null == _nullValueField)
				{
					_nullValueField = _enumerator.AddField("$empty", _generatorItemType);
				}
				return CodeBuilder.CreateReference(_nullValueField);
			}
			return new NullLiteralExpression();
		}

		LabelStatement CreateLabel(Node sourceNode)
		{
			InternalLabel label = CodeBuilder.CreateLabel(sourceNode, "$state$" + _labels.Count);
			_labels.Add(label.LabelStatement);
			_tryStatementInfoForLabels.Add(_tryStatementStack.Count > 0 ? _tryStatementStack.Peek() : null);
			return label.LabelStatement;
		}
		
		BooMethodBuilder CreateConstructor(BooClassBuilder builder)
		{
			BooMethodBuilder constructor = builder.AddConstructor();
			constructor.Body.Add(CodeBuilder.CreateSuperConstructorInvocation(builder.Entity.BaseType));
			return constructor;
		}
	}
}