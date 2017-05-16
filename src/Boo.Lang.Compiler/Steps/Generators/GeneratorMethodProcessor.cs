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

using System.Collections.Generic;
using System.Linq;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Steps.StateMachine;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Builders;
using Boo.Lang.Compiler.TypeSystem.Generics;
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.Steps.Generators
{
    internal class GeneratorMethodProcessor : MethodToStateMachineTransformer
    {
        private readonly BooClassBuilder _enumerable;

        private readonly IType _generatorItemType;

        private readonly BooMethodBuilder _getEnumeratorBuilder;

        private readonly GeneratorTypeReplacer _methodToEnumerableMapper;

        private BooMethodBuilder _enumerableConstructor;

        private IMethod _yield;

        private IMethod _yieldDefault;

        public GeneratorMethodProcessor(CompilerContext context, InternalMethod method)
            : base(context, method)
        {
            var skeleton = My<GeneratorSkeletonBuilder>.Instance.SkeletonFor(method);
            _generatorItemType = skeleton.GeneratorItemType;
            _enumerable = skeleton.GeneratorClassBuilder;
            _getEnumeratorBuilder = skeleton.GetEnumeratorBuilder;
            _methodToEnumerableMapper = skeleton.GeneratorClassTypeReplacer;
        }

        private MethodInvocationExpression _enumerableConstructorInvocation;

        public override void Run()
        {
            base.Run();
            CreateGetEnumeratorBody(_stateMachineConstructorInvocation);
            FixGeneratorMethodBody(_enumerableConstructorInvocation);
        }

        protected override void PrepareConstructorCalls()
        {
            base.PrepareConstructorCalls();
            _enumerableConstructorInvocation = CodeBuilder.CreateGenericConstructorInvocation(
                (IType)_enumerable.ClassDefinition.Entity,
                 _genericParams);            
        }

        protected override void PropagateReferences()
        {
            // propagate the necessary parameters from
            // the enumerable to the enumerator
            foreach (ParameterDeclaration parameter in _method.Method.Parameters)
            {
                var myParam = MapParamType(parameter);

                var entity = (InternalParameter)myParam.Entity;
                if (entity.IsUsed)
                {
                    _enumerableConstructorInvocation.Arguments.Add(CodeBuilder.CreateReference(myParam));

                    PropagateFromEnumerableToEnumerator(_stateMachineConstructorInvocation,
                                                        entity.Name,
                                                        entity.Type);
                }
            }

            // propagate the external self reference if necessary
            if (_externalSelfField != null)
            {
                var type = (IType)_externalSelfField.Type.Entity;
                _enumerableConstructorInvocation.Arguments.Add(
                    CodeBuilder.CreateSelfReference(_methodToEnumerableMapper.MapType(type)));

                PropagateFromEnumerableToEnumerator(_stateMachineConstructorInvocation,
                                                    "self_",
                                                    _methodToStateMachineMapper.MapType(type));
            }

        }

        private void PropagateFromEnumerableToEnumerator(MethodInvocationExpression enumeratorConstructorInvocation,
                                                 string parameterName,
                                                 IType parameterType)
        {
            Field field = DeclareFieldInitializedFromConstructorParameter(
                _enumerable,
                _enumerableConstructor,
                parameterName,
                parameterType,
                _methodToEnumerableMapper);
            enumeratorConstructorInvocation.Arguments.Add(CodeBuilder.CreateReference(field));
        }

        protected override void CreateStateMachine()
        {
            _enumerableConstructor = CreateConstructor(_enumerable);
            base.CreateStateMachine();
        }

        private void CreateGetEnumeratorBody(Expression enumeratorExpression)
        {
            _getEnumeratorBuilder.Body.Add(
                new ReturnStatement(enumeratorExpression));
        }

        private void FixGeneratorMethodBody(MethodInvocationExpression enumerableConstructorInvocation)
        {
            var body = _method.Method.Body;
            body.Clear();

            body.Add(
                new ReturnStatement(
                    _method.Method.LexicalInfo,
                    GeneratorReturnsIEnumerator()
                        ? CreateGetEnumeratorInvocation(enumerableConstructorInvocation)
                        : enumerableConstructorInvocation));
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
                var baseType = (IConstructedTypeInfo)new GenericConstructedType(enumeratorEntity.DeclaringType, argList.ToArray());
                enumeratorEntity = (IMethod)baseType.Map(enumeratorEntity);
            }
            return CodeBuilder.CreateMethodInvocation(enumerableConstructorInvocation, enumeratorEntity);
        }

        private InternalMethod GetGetEnumeratorEntity()
        {
            return _getEnumeratorBuilder.Entity;
        }

        private bool GeneratorReturnsIEnumerator()
        {
            bool returnsEnumerator = _method.ReturnType == TypeSystemServices.IEnumeratorType;
            returnsEnumerator |=
                _method.ReturnType.ConstructedInfo != null &&
                _method.ReturnType.ConstructedInfo.GenericDefinition == TypeSystemServices.IEnumeratorGenericType;

            return returnsEnumerator;
        }

        protected override void SetupStateMachine()
        {
            _stateMachineClass.Modifiers |= _enumerable.Modifiers;
            var abstractEnumeratorType =
                TypeSystemServices.Map(typeof(GenericGeneratorEnumerator<>)).
                    GenericInfo.ConstructType(_methodToStateMachineMapper.MapType(_generatorItemType));

            _state = NameResolutionService.ResolveField(abstractEnumeratorType, "_state");
            _yield = NameResolutionService.ResolveMethod(abstractEnumeratorType, "Yield");
            _yieldDefault = NameResolutionService.ResolveMethod(abstractEnumeratorType, "YieldDefault");
            _stateMachineClass.AddBaseType(abstractEnumeratorType);
            _stateMachineClass.AddBaseType(TypeSystemServices.IEnumeratorType);

        }

        protected override string StateMachineClassName
        {
            get { return "$Enumerator"; }
        }

        protected override void SaveStateMachineClass(ClassDefinition cd)
        {
            _enumerable.ClassDefinition.Members.Add(cd);
        }

        private MethodInvocationExpression CreateYieldInvocation(LexicalInfo sourceLocation, int newState, Expression value)
        {
            MethodInvocationExpression invocation = CodeBuilder.CreateMethodInvocation(
                CodeBuilder.CreateSelfReference(_stateMachineClass.Entity),
                value != null ? _yield : _yieldDefault,
                CodeBuilder.CreateIntegerLiteral(newState));
            if (value != null) invocation.Arguments.Add(value);
            invocation.LexicalInfo = sourceLocation;
            return invocation;
        }

        protected override void CreateMoveNext()
        {
            Method generator = _method.Method;

            BooMethodBuilder methodBuilder = _stateMachineClass.AddVirtualMethod("MoveNext", TypeSystemServices.BoolType);
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

        public override void LeaveYieldStatement(YieldStatement node)
        {
            TryStatementInfo currentTry = _tryStatementStack.Count > 0 ? _tryStatementStack.Peek() : null;
            if (currentTry != null)
            {
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

        private IMethod CreateDisposeMethod()
        {
            BooMethodBuilder mn = _stateMachineClass.AddVirtualMethod("Dispose", TypeSystemServices.VoidType);
            mn.Method.LexicalInfo = this.LexicalInfo;

            var noEnsure = CodeBuilder.CreateLabel(_method.Method, "noEnsure").LabelStatement;
            mn.Body.Add(noEnsure);
            mn.Body.Add(SetStateTo(_finishedStateNumber));
            mn.Body.Add(new ReturnStatement());

            // Create a section calling all ensure methods for each converted try block
            var disposeLabels = new LabelStatement[_labels.Count];
            foreach (var t in _convertedTryStatements)
            {
                var info = t;
                disposeLabels[info._stateNumber] = CodeBuilder.CreateLabel(_method.Method, "$ensure_" + info._stateNumber).LabelStatement;
                mn.Body.Add(disposeLabels[info._stateNumber]);
                mn.Body.Add(SetStateTo(_finishedStateNumber));
                var block = mn.Body;
                while (info._parent != null)
                {
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
            for (var i = 0; i < _labels.Count; i++)
            {
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

        protected override IEnumerable<GenericParameterDeclaration> GetStateMachineGenericParams()
        {
            return _enumerable.ClassDefinition.GenericParameters;
        }

    }
}