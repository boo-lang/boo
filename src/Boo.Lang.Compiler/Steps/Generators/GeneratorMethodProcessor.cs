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
using System.Linq;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Builders;
using Boo.Lang.Compiler.TypeSystem.Generics;
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.Steps.Generators
{
	internal class GeneratorMethodProcessor : AbstractTransformerCompilerStep
	{
		private readonly InternalMethod _generator;

        private InternalMethod _moveNext;

        private readonly BooClassBuilder _enumerable;

        private BooClassBuilder _enumerator;

        private BooMethodBuilder _enumeratorConstructor;

        private BooMethodBuilder _enumerableConstructor;

        private IField _state;

        private IMethod _yield;

        private IMethod _yieldDefault;

        private Field _externalEnumeratorSelf;

        private readonly List<LabelStatement> _labels;

        private readonly System.Collections.Generic.List<TryStatementInfo> _tryStatementInfoForLabels = new System.Collections.Generic.List<TryStatementInfo>();

        private readonly Hashtable _mapping;

        private readonly IType _generatorItemType;

        private readonly BooMethodBuilder _getEnumeratorBuilder;

        private readonly TypeReplacer _methodToEnumerableMapper;

        private readonly TypeReplacer _methodToEnumeratorMapper = new TypeReplacer();

        private readonly Dictionary<IEntity, IEntity> _entityMapper = new Dictionary<IEntity, IEntity>();

        public GeneratorMethodProcessor(CompilerContext context, InternalMethod method)
		{
			_labels = new List<LabelStatement>();
			_mapping = new Hashtable();
			_generator = method;

			var skeleton = My<GeneratorSkeletonBuilder>.Instance.SkeletonFor(method);
			_generatorItemType = skeleton.GeneratorItemType;
			_enumerable = skeleton.GeneratorClassBuilder;
			_getEnumeratorBuilder = skeleton.GetEnumeratorBuilder;
            _methodToEnumerableMapper = skeleton.GeneratorClassTypeReplacer;

			Initialize(context);
		}

		private LexicalInfo LexicalInfo
		{
			get { return _generator.Method.LexicalInfo; }
		}

        private GenericParameterDeclaration[] _genericParams;

		public override void Run()
		{
            _genericParams = _generator.Method.DeclaringType.GenericParameters.Concat(_generator.Method.GenericParameters).ToArray();
            CreateEnumerableConstructor();
			CreateEnumerator();
		    var enumerableConstructorInvocation = CodeBuilder.CreateGenericConstructorInvocation(
                (IType)_enumerable.ClassDefinition.Entity,
                 _genericParams);
            var enumeratorConstructorInvocation = CodeBuilder.CreateGenericConstructorInvocation(
                (IType)_enumerator.ClassDefinition.Entity,
                _enumerable.ClassDefinition.GenericParameters);
			PropagateReferences(enumerableConstructorInvocation, enumeratorConstructorInvocation);
			CreateGetEnumeratorBody(enumeratorConstructorInvocation);
			FixGeneratorMethodBody(enumerableConstructorInvocation);
		}

        private void FixGeneratorMethodBody(MethodInvocationExpression enumerableConstructorInvocation)
		{
			var body = _generator.Method.Body;
			body.Clear();

			body.Add(
				new ReturnStatement(
					_generator.Method.LexicalInfo,
					GeneratorReturnsIEnumerator()
						? CreateGetEnumeratorInvocation(enumerableConstructorInvocation)
						: enumerableConstructorInvocation));
		}

        private ParameterDeclaration MapParamType(ParameterDeclaration parameter)
	    {
            if (parameter.Type.NodeType == NodeType.GenericTypeReference)
            {
                var gen = (GenericTypeReference)parameter.Type;
                var genEntityType = gen.Entity as IConstructedTypeInfo;
                if (genEntityType == null)
                    return parameter;
                var trc = new TypeReferenceCollection();
                foreach (var genArg in gen.GenericArguments)
                {
                    var replacement = genArg;
                    foreach (var genParam in _enumerable.ClassDefinition.GenericParameters)
                        if (genParam.Name.Equals(genArg.Entity.Name))
                        {
                            replacement = new SimpleTypeReference(genParam.Name) {Entity = genParam.Entity};
                            break;
                        }
                    trc.Add(replacement);
                }
                parameter = parameter.CloneNode();
                gen = (GenericTypeReference)parameter.Type;
                gen.GenericArguments = trc;
                gen.Entity = new GenericConstructedType(genEntityType.GenericDefinition, trc.Select(a => a.Entity).Cast<IType>().ToArray());
            }
	        return parameter;
	    }

        private void PropagateReferences(MethodInvocationExpression enumerableConstructorInvocation,
		                         MethodInvocationExpression enumeratorConstructorInvocation)
		{
			// propagate the necessary parameters from
			// the enumerable to the enumerator
			foreach (ParameterDeclaration parameter in _generator.Method.Parameters)
			{
			    var myParam = MapParamType(parameter);

                var entity = (InternalParameter)myParam.Entity;
				if (entity.IsUsed)
				{
                    enumerableConstructorInvocation.Arguments.Add(CodeBuilder.CreateReference(myParam));
					
					PropagateFromEnumerableToEnumerator(enumeratorConstructorInvocation,
					                                    entity.Name,
					                                    entity.Type);
				}
			}
			
			// propagate the external self reference if necessary
			if (null != _externalEnumeratorSelf)
			{
				var type = (IType)_externalEnumeratorSelf.Type.Entity;
				enumerableConstructorInvocation.Arguments.Add(CodeBuilder.CreateSelfReference(type));
				
				PropagateFromEnumerableToEnumerator(enumeratorConstructorInvocation,
				                                    "self_",
				                                    type);
			}
			
		}

		private MethodInvocationExpression CreateGetEnumeratorInvocation(MethodInvocationExpression enumerableConstructorInvocation)
		{
            IMethod enumeratorEntity = GetGetEnumeratorEntity();
            var enumeratorInfo = enumeratorEntity.DeclaringType.GenericInfo;
            if (enumeratorInfo != null && _genericParams.Length > 0)
            {
                var argList = new List<IType>();
                foreach (var param in enumeratorInfo.GenericParameters)
                {
                    var replacement = _genericParams.SingleOrDefault(gp => gp.Name.Equals(param.Name));
                    argList.Add(replacement == null ? param : (IType)replacement.Entity);
                }
                var baseType = (IConstructedTypeInfo) new GenericConstructedType(enumeratorEntity.DeclaringType, argList.ToArray());
                enumeratorEntity = (IMethod) baseType.Map(enumeratorEntity);
            }
			return CodeBuilder.CreateMethodInvocation(enumerableConstructorInvocation, enumeratorEntity);
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

        private void CreateGetEnumeratorBody(Expression enumeratorExpression)
		{
			_getEnumeratorBuilder.Body.Add(
				new ReturnStatement(enumeratorExpression));
		}

        private void CreateEnumerableConstructor()
		{
			_enumerableConstructor = CreateConstructor(_enumerable);
		}

        private void CreateEnumeratorConstructor()
		{
			_enumeratorConstructor = CreateConstructor(_enumerator);
		}

        private void CreateEnumerator()
		{
            _enumerator = CodeBuilder.CreateClass("$Enumerator");
			_enumerator.AddAttribute(CodeBuilder.CreateAttribute(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute)));
			_enumerator.Modifiers |= _enumerable.Modifiers;
			_enumerator.LexicalInfo = this.LexicalInfo;
            foreach (var param in _genericParams)
            {
                var replacement = _enumerator.AddGenericParameter(param.Name);
                _methodToEnumeratorMapper.Replace((IType)param.Entity, (IType)replacement.Entity);
            }
            var abstractEnumeratorType =
                TypeSystemServices.Map(typeof(GenericGeneratorEnumerator<>)).
                    GenericInfo.ConstructType(_methodToEnumeratorMapper.MapType(_generatorItemType));

            _state = NameResolutionService.ResolveField(abstractEnumeratorType, "_state");
            _yield = NameResolutionService.ResolveMethod(abstractEnumeratorType, "Yield");
            _yieldDefault = NameResolutionService.ResolveMethod(abstractEnumeratorType, "YieldDefault");
            _enumerator.AddBaseType(abstractEnumeratorType);
            _enumerator.AddBaseType(TypeSystemServices.IEnumeratorType);

			CreateEnumeratorConstructor();
			CreateMoveNext();
			
			_enumerable.ClassDefinition.Members.Add(_enumerator.ClassDefinition);
		}

        private void CreateMoveNext()
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
			
			methodBuilder.Body.Add(CreateYieldInvocation(LexicalInfo.Empty, _finishedStateNumber, null));
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
				
				var tryFailure = new TryStatement();
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
				var entity = (InternalParameter)parameter.Entity;
				if (entity.IsUsed)
				{
					var field = DeclareFieldInitializedFromConstructorParameter(_enumerator, _enumeratorConstructor,
					                                                              entity.Name,
					                                                              _methodToEnumeratorMapper.MapType(entity.Type));
					_mapping[entity] = field.Entity;
				}
			}
		}

		private void TransformLocalsIntoFields(Method generator)
		{
			foreach (var local in generator.Locals)
			{
				var entity = (InternalLocal)local.Entity;
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
            var newLocal = new InternalLocal(local, _methodToEnumerableMapper.MapType(((InternalLocal)local.Entity).Type));
		    _entityMapper.Add(local.Entity, newLocal);
		    local.Entity = newLocal;
			_moveNext.Method.Locals.Add(local);
		}

		private void AddInternalFieldFor(InternalLocal entity)
		{
            Field field = _enumerator.AddInternalField(UniqueName(entity.Name), _methodToEnumeratorMapper.MapType(entity.Type));
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
            var entity = _enumerator.Entity;
            var genParams = _enumerator.ClassDefinition.GenericParameters;
            if (!genParams.IsEmpty)
            {
                var args = genParams.Select(gpd => gpd.Entity).Cast<IType>().ToArray();
                entity = new GenericConstructedType(entity, args);
                var mapping = new InternalGenericMapping(entity, args);
                method = mapping.Map(method);
            }
			return CodeBuilder.CreateMethodInvocation(
				CodeBuilder.CreateSelfReference(entity),
				method);
		}

        private IMethod CreateDisposeMethod()
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

        private void PropagateFromEnumerableToEnumerator(MethodInvocationExpression enumeratorConstructorInvocation,
		                                         string parameterName,
		                                         IType parameterType)
        {
			Field field = DeclareFieldInitializedFromConstructorParameter(
                _enumerable,
                _enumerableConstructor,
                parameterName,
                _methodToEnumerableMapper.MapType(parameterType));
			enumeratorConstructorInvocation.Arguments.Add(CodeBuilder.CreateReference(field));
		}

        private Field DeclareFieldInitializedFromConstructorParameter(BooClassBuilder type,
		                                                      BooMethodBuilder constructor,
		                                                      string parameterName,
		                                                      IType parameterType)
        {
			Field field = type.AddInternalField(UniqueName(parameterName), parameterType);
			InitializeFieldFromConstructorParameter(constructor, field, parameterName, parameterType);
			return field;
		}

        private void InitializeFieldFromConstructorParameter(BooMethodBuilder constructor,
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
		
		public override void OnReferenceExpression(ReferenceExpression node)
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
		
		public override void OnSelfLiteralExpression(SelfLiteralExpression node)
		{
			ReplaceCurrentNode(CodeBuilder.CreateReference(node.LexicalInfo, ExternalEnumeratorSelf()));
		}

		public override void OnSuperLiteralExpression(SuperLiteralExpression node)
		{
			var externalSelf = CodeBuilder.CreateReference(node.LexicalInfo, ExternalEnumeratorSelf());
			if (AstUtil.IsTargetOfMethodInvocation(node)) // super(...)
				ReplaceCurrentNode(CodeBuilder.CreateMemberReference(externalSelf, (IMethod)GetEntity(node)));
			else // super.Method(...)
				ReplaceCurrentNode(externalSelf);
		}

	    private IMethod RemapMethod(Node node, GenericMappedMethod gmm, GenericParameterDeclarationCollection genParams)
	    {
            var sourceMethod = gmm.SourceMember;
	        if (sourceMethod.GenericInfo != null)
	            throw new CompilerError(node, "Mapping generic methods in generators is not implemented yet");

	        var baseType = sourceMethod.DeclaringType;
	        var genericInfo = baseType.GenericInfo;
	        if (genericInfo == null)
	            throw new CompilerError(node, "Mapping generic nested types in generators is not implemented yet");

	        var genericArgs = ((IGenericArgumentsProvider)gmm.DeclaringType).GenericArguments;
	        var mapList = new List<IType>();
	        foreach (var arg in genericArgs)
	        {
	            var mappedArg = genParams.SingleOrDefault(gp => gp.Name == arg.Name);
	            if (mappedArg != null)
	                mapList.Add((IType)mappedArg.Entity);
	            else mapList.Add(arg);
	        }
	        var newType = (IConstructedTypeInfo)new GenericConstructedType(baseType, mapList.ToArray());
	        return (IMethod)newType.Map(sourceMethod);
	    }

        public override void OnMemberReferenceExpression(MemberReferenceExpression node)
        {
            base.OnMemberReferenceExpression(node);
            var gmm = node.Entity as GenericMappedMethod;
            if (gmm != null)
            {
                var genParams = _enumerator.ClassDefinition.GenericParameters;
                if (genParams.IsEmpty)
                    return;
                node.Entity = RemapMethod(node, gmm, genParams);
            }
        }

        public override void OnDeclaration(Declaration node)
        {
            base.OnDeclaration(node);
            if (_entityMapper.ContainsKey(node.Entity))
                node.Entity = _entityMapper[node.Entity];
        }

		public override void OnMethodInvocationExpression(MethodInvocationExpression node)
		{
			var superInvocation = IsInvocationOnSuperMethod(node);
			base.OnMethodInvocationExpression(node);
			if (!superInvocation)
				return;

			var accessor = CreateAccessorForSuperMethod(node.Target);
			Bind(node.Target, accessor);
		}

		private IEntity CreateAccessorForSuperMethod(Expression target)
		{
			var superMethod = (IMethod)GetEntity(target);
			var accessor = CodeBuilder.CreateMethodFromPrototype(target.LexicalInfo, superMethod, TypeMemberModifiers.Internal, UniqueName(superMethod.Name));
			var accessorEntity = (IMethod)GetEntity(accessor);
			var superMethodInvocation = CodeBuilder.CreateSuperMethodInvocation(superMethod);
			foreach (var p in accessorEntity.GetParameters())
				superMethodInvocation.Arguments.Add(CodeBuilder.CreateReference(p));
			accessor.Body.Add(new ReturnStatement(superMethodInvocation));

			DeclaringTypeDefinition.Members.Add(accessor);
			return GetEntity(accessor);
		}

		private string UniqueName(string name)
		{
			return Context.GetUniqueName(name);
		}

		protected TypeDefinition DeclaringTypeDefinition
		{
			get { return _generator.Method.DeclaringType; }
		}

		private static bool IsInvocationOnSuperMethod(MethodInvocationExpression node)
		{
			if (node.Target is SuperLiteralExpression)
				return true;

			var target = node.Target as MemberReferenceExpression;
			return target != null && target.Target is SuperLiteralExpression;
		}

		private Field ExternalEnumeratorSelf()
		{
			if (null == _externalEnumeratorSelf)
			{
				_externalEnumeratorSelf = DeclareFieldInitializedFromConstructorParameter(
					_enumerator,
					_enumeratorConstructor,
					"self_",
					_generator.DeclaringType);
			}

			return _externalEnumeratorSelf;
		}

		private sealed class TryStatementInfo
		{
			internal TryStatement _statement;
			internal TryStatementInfo _parent;
			
			internal bool _containsYield;
			internal int _stateNumber = -1;
			internal Block _replacement;
			
			internal IMethod _ensureMethod;
		}

	    private readonly System.Collections.Generic.List<TryStatementInfo> _convertedTryStatements 
            = new System.Collections.Generic.List<TryStatementInfo>();
        private readonly Stack<TryStatementInfo> _tryStatementStack = new Stack<TryStatementInfo>();
		private int _finishedStateNumber;
		
		public override bool EnterTryStatement(TryStatement node)
		{
			var info = new TryStatementInfo();
			info._statement = node;
			if (_tryStatementStack.Count > 0)
				info._parent = _tryStatementStack.Peek();
			_tryStatementStack.Push(info);
			return true;
		}
		
		private BinaryExpression SetStateTo(int num)
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
		
		private void ConvertTryStatement(TryStatementInfo currentTry)
		{
			if (currentTry._containsYield)
				return;
			currentTry._containsYield = true;
			currentTry._stateNumber = _labels.Count;
			var tryReplacement = new Block();
			//tryReplacement.Add(CreateLabel(tryReplacement));
			// when the MoveNext() is called while the enumerator is still in running state, don't jump to the
			// try block, but handle it like MoveNext() calls when the enumerator is in the finished state.
			_labels.Add(_labels[_finishedStateNumber]);
			_tryStatementInfoForLabels.Add(currentTry);
			tryReplacement.Add(SetStateTo(currentTry._stateNumber));
			currentTry._replacement = tryReplacement;
		}
		
		public override void LeaveYieldStatement(YieldStatement node)
		{
			TryStatementInfo currentTry = _tryStatementStack.Count > 0 ? _tryStatementStack.Peek() : null;
			if (currentTry != null) {
				ConvertTryStatement(currentTry);
			}
			var block = new Block();
			block.Add(
				new ReturnStatement(
					node.LexicalInfo,
					CreateYieldInvocation(node.LexicalInfo, _labels.Count, node.Expression),
					null));
			block.Add(CreateLabel(node));
			// setting the state back to the "running" state not required, as that state has the same ensure blocks
			// as the state we are currently in.
//			if (currentTry != null) {
//				block.Add(SetStateTo(currentTry._stateNumber));
//			}
			ReplaceCurrentNode(block);
		}

		private MethodInvocationExpression CreateYieldInvocation(LexicalInfo sourceLocation, int newState, Expression value)
		{
			MethodInvocationExpression invocation = CodeBuilder.CreateMethodInvocation(
				CodeBuilder.CreateSelfReference(_enumerator.Entity),
				value != null ? _yield : _yieldDefault,
				CodeBuilder.CreateIntegerLiteral(newState));
			if (value != null) invocation.Arguments.Add(value);
			invocation.LexicalInfo = sourceLocation;
			return invocation;
		}

		private LabelStatement CreateLabel(Node sourceNode)
		{
			InternalLabel label = CodeBuilder.CreateLabel(sourceNode, "$state$" + _labels.Count);
			_labels.Add(label.LabelStatement);
			_tryStatementInfoForLabels.Add(_tryStatementStack.Count > 0 ? _tryStatementStack.Peek() : null);
			return label.LabelStatement;
		}
		
		private BooMethodBuilder CreateConstructor(BooClassBuilder builder)
		{
			BooMethodBuilder constructor = builder.AddConstructor();
			constructor.Body.Add(CodeBuilder.CreateSuperConstructorInvocation(builder.Entity.BaseType));
			return constructor;
		}
	}
}