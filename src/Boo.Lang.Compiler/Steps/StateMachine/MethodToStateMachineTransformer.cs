#region license
// Copyright (c) 2003-2017 Rodrigo B. de Oliveira (rbo@acm.org), Mason Wheeler
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
using Boo.Lang.Compiler.Steps.Generators;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Builders;
using Boo.Lang.Compiler.TypeSystem.Generics;
using Boo.Lang.Compiler.TypeSystem.Internal;

namespace Boo.Lang.Compiler.Steps.StateMachine
{
	using System.Collections.Generic;

	internal abstract class MethodToStateMachineTransformer : AbstractTransformerCompilerStep
    {
 
		protected readonly InternalMethod _method;

        protected InternalMethod _moveNext;

        protected IField _state;

        protected readonly GeneratorTypeReplacer _methodToStateMachineMapper = new GeneratorTypeReplacer();

        protected BooClassBuilder _stateMachineClass;

        protected BooMethodBuilder _stateMachineConstructor;

        protected Field _externalSelfField;

        protected readonly List<LabelStatement> _labels;

        protected readonly List<TryStatementInfo> _tryStatementInfoForLabels = new List<TryStatementInfo>();

        private readonly Dictionary<IEntity, InternalField> _mapping = new Dictionary<IEntity, InternalField>();

        private readonly Dictionary<IEntity, IEntity> _entityMapper = new Dictionary<IEntity, IEntity>();

        protected int _finishedStateNumber;

        protected MethodToStateMachineTransformer(CompilerContext context, InternalMethod method)
		{
			_labels = new List<LabelStatement>();
			_method = method;

			Initialize(context);
		}

		protected LexicalInfo LexicalInfo
		{
			get { return _method.Method.LexicalInfo; }
		}

        protected GenericParameterDeclaration[] _genericParams;

        protected MethodInvocationExpression _stateMachineConstructorInvocation;

		public override void Run()
		{
            _genericParams = _method.Method.DeclaringType.GenericParameters.Concat(_method.Method.GenericParameters).ToArray();
			CreateStateMachine();
		    PrepareConstructorCalls();
			PropagateReferences();
		}

        protected virtual IEnumerable<GenericParameterDeclaration> GetStateMachineGenericParams()
        {
            return _genericParams;
        }

        protected virtual void PrepareConstructorCalls()
        {
            _stateMachineConstructorInvocation = CodeBuilder.CreateGenericConstructorInvocation(
                (IType)_stateMachineClass.ClassDefinition.Entity,
                GetStateMachineGenericParams());            
        }

        protected ParameterDeclaration MapParamType(ParameterDeclaration parameter)
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
                    foreach (var genParam in _genericParams)
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

        protected abstract void PropagateReferences();

        private void CreateStateMachineConstructor()
		{
			_stateMachineConstructor = CreateConstructor(_stateMachineClass);
		}

        protected abstract void SetupStateMachine();

        protected abstract string StateMachineClassName {
            get;
        }

        protected virtual void CreateStateMachine()
		{
            _stateMachineClass = CodeBuilder.CreateClass(StateMachineClassName);
			_stateMachineClass.AddAttribute(CodeBuilder.CreateAttribute(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute)));
			_stateMachineClass.LexicalInfo = this.LexicalInfo;
            foreach (var param in _genericParams)
            {
                var replacement = _stateMachineClass.AddGenericParameter(param);
                _methodToStateMachineMapper.Replace((IType)param.Entity, (IType)replacement.Entity);
            }

		    SetupStateMachine();
            CreateStateMachineConstructor();

            SaveStateMachineClass(_stateMachineClass.ClassDefinition);
            CreateMoveNext();
		}

        protected abstract void SaveStateMachineClass(ClassDefinition cd);

        protected abstract void CreateMoveNext();

		protected void TransformParametersIntoFieldsInitializedByConstructor(Method generator)
		{
			foreach (ParameterDeclaration parameter in generator.Parameters)
			{
				var entity = (InternalParameter)parameter.Entity;
				if (entity.IsUsed)
				{
					var field = DeclareFieldInitializedFromConstructorParameter(_stateMachineClass, 
                                                                                _stateMachineConstructor,
					                                                            entity.Name,
					                                                            entity.Type,
                                                                                _methodToStateMachineMapper);
					_mapping[entity] = (InternalField)field.Entity;
				}
			}
		}

		protected void TransformLocalsIntoFields(Method stateMachine)
		{
			foreach (var local in stateMachine.Locals)
			{
				var entity = (InternalLocal)local.Entity;
				if (IsExceptionHandlerVariable(entity))
				{
					AddToMoveNextMethod(local);
					continue;
				}

				AddInternalFieldFor(entity);
			}
			stateMachine.Locals.Clear();
		}

		private void AddToMoveNextMethod(Local local)
		{
            var newLocal = new InternalLocal(local, _methodToStateMachineMapper.MapType(((InternalLocal)local.Entity).Type));
		    _entityMapper.Add(local.Entity, newLocal);
		    local.Entity = newLocal;
			_moveNext.Method.Locals.Add(local);
		}

		private void AddInternalFieldFor(InternalLocal entity)
		{
            Field field = _stateMachineClass.AddInternalField(UniqueName(entity.Name), _methodToStateMachineMapper.MapType(entity.Type));
			_mapping[entity] = (InternalField)field.Entity;
		}

		private bool IsExceptionHandlerVariable(InternalLocal local)
		{
			Declaration originalDeclaration = local.OriginalDeclaration;
			if (originalDeclaration == null) return false;
			return originalDeclaration.ParentNode is ExceptionHandler;
		}

		protected MethodInvocationExpression CallMethodOnSelf(IMethod method)
		{
            var entity = _stateMachineClass.Entity;
            var genParams = _stateMachineClass.ClassDefinition.GenericParameters;
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

        protected Field DeclareFieldInitializedFromConstructorParameter(BooClassBuilder type,
		                                                      BooMethodBuilder constructor,
		                                                      string parameterName,
		                                                      IType parameterType,
                                                              TypeReplacer replacer)
        {
            var internalFieldType = replacer.MapType(parameterType);
			Field field = type.AddInternalField(UniqueName(parameterName), internalFieldType);
			InitializeFieldFromConstructorParameter(constructor, field, parameterName, internalFieldType);
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

	    private void OnTypeReference(TypeReference node)
	    {
            var type = (IType)node.Entity;
            node.Entity = _methodToStateMachineMapper.MapType(type);	        
	    }

	    public override void OnSimpleTypeReference(SimpleTypeReference node)
	    {
            OnTypeReference(node);
	    }

        public override void OnArrayTypeReference(ArrayTypeReference node)
        {
            base.OnArrayTypeReference(node);
            OnTypeReference(node);
        }

        public override void OnCallableTypeReference(CallableTypeReference node)
        {
            base.OnCallableTypeReference(node);
            OnTypeReference(node);
        }

        public override void OnGenericTypeReference(GenericTypeReference node)
	    {
            base.OnGenericTypeReference(node);
            OnTypeReference(node);
        }

        public override void OnGenericTypeDefinitionReference(GenericTypeDefinitionReference node)
        {
            base.OnGenericTypeDefinitionReference(node);
            OnTypeReference(node);
        }

        public override void OnReferenceExpression(ReferenceExpression node)
        {
            InternalField mapped;
            if (_mapping.TryGetValue(node.Entity, out mapped))
            {
                ReplaceCurrentNode(
                    CodeBuilder.CreateMemberReference(
                        node.LexicalInfo,
                        CodeBuilder.CreateSelfReference(_stateMachineClass.Entity),
                        mapped));
            }
			else if (node.Entity is IGenericMappedMember || node.Entity is IGenericParameter || node.Entity is InternalLocal)
			{
				node.Accept(new GenericTypeMapper(_methodToStateMachineMapper));
			}
        }

        public override void OnSelfLiteralExpression(SelfLiteralExpression node)
        {
			var newNode = CodeBuilder.CreateMappedReference(
				node.LexicalInfo, 
				ExternalEnumeratorSelf(), 
				_stateMachineClass.Entity);
			ReplaceCurrentNode(newNode);
		}

		public override void OnSuperLiteralExpression(SuperLiteralExpression node)
		{
			var externalSelf = CodeBuilder.CreateReference(node.LexicalInfo, ExternalEnumeratorSelf());
			if (AstUtil.IsTargetOfMethodInvocation(node)) // super(...)
				ReplaceCurrentNode(CodeBuilder.CreateMemberReference(externalSelf, (IMethod)GetEntity(node)));
			else // super.Method(...)
				ReplaceCurrentNode(externalSelf);
		}

	    private static IMethod RemapMethod(Node node, GenericMappedMethod gmm, IType[] genParams)
	    {
            var sourceMethod = gmm.SourceMember;
	        if (sourceMethod.GenericInfo != null)
	            throw new CompilerError(node, "Mapping generic methods in generators is not implemented yet");

	        var baseType = sourceMethod.DeclaringType;
	        var genericInfo = baseType.GenericInfo;
	        if (genericInfo == null)
	            throw new CompilerError(node, "Mapping generic nested types in generators is not implemented yet");

	        var genericArgs = ((IGenericArgumentsProvider)gmm.DeclaringType).GenericArguments;
            var collector = new TypeCollector(type => type is IGenericParameter);
	        foreach (var arg in genericArgs)
                collector.Visit(arg);
            var mapper = new GeneratorTypeReplacer();
            foreach (var genParam in collector.Matches)
            {
                var mappedArg = genParams.SingleOrDefault(gp => gp.Name == genParam.Name);
	            if (mappedArg != null)
	                mapper.Replace(genParam, mappedArg);
	        }
	        var newType = (IConstructedTypeInfo)new GenericConstructedType(
                baseType, 
                genericArgs.Select(mapper.MapType).ToArray());
	        return (IMethod)newType.Map(sourceMethod);
	    }

        public override void OnMemberReferenceExpression(MemberReferenceExpression node)
        {
            base.OnMemberReferenceExpression(node);

			var genParams = GetGenericParams(node);
			if (genParams != null)
			{
				var gmm = node.Entity as GenericMappedMethod;
				if (gmm != null)
				{
					node.Entity = RemapMethod(node, gmm, genParams);
					node.ExpressionType = ((IMethod)node.Entity).CallableType;
					return;
				}
			}
			var member = node.Entity as IMember;
			if (member != null)
				MapMember(node, member);
        }

	    private void MapMember(MemberReferenceExpression node, IMember member)
	    {
            if (!_methodToStateMachineMapper.Any)
                return;
            var baseType = member.DeclaringType;
		    var mapped = member as IGenericMappedMember;
		    if (mapped != null)
		    {
			    if (baseType == node.Target.ExpressionType)
				    return;
			    member = mapped.SourceMember;
		    }
			var didMap = false;
			if (node.Target.ExpressionType != null)
			{
				var newType = node.Target.ExpressionType.ConstructedInfo;
				if (newType != null)
				{
					member = newType.Map(member);
					didMap = true;
				}
				else if (node.Target.ExpressionType.GenericInfo != null)
					throw new System.InvalidOperationException("Bad target type");				
			}
			if (!didMap && member.EntityType == EntityType.Method)
			{
				var gen = member as IGenericMethodInfo;
				if (gen != null)
				{
					foreach (var gp in gen.GenericParameters)
						if (!_methodToStateMachineMapper.ContainsType(gp))
						{
							var replacement = this._genericParams.FirstOrDefault(p => p.Name == gp.Name);
							if (replacement != null)
								_methodToStateMachineMapper.Replace(gp, _methodToStateMachineMapper.MapType((IType)replacement.Entity));
						}
					member = gen.ConstructMethod(
						gen.GenericParameters.Select(_methodToStateMachineMapper.MapType).ToArray());
				}
				else
				{
					var con = member as IConstructedMethodInfo;
					if (con != null)
					{
						var gd = con.GenericDefinition.GenericInfo;
						member = gd.ConstructMethod(con.GenericArguments.Select(_methodToStateMachineMapper.MapType).ToArray());
					}
				}
			}
		    node.Entity = member;
		    node.ExpressionType = member.Type;
	    }

		private void MapMember(GenericReferenceExpression node, IMember member)
		{
			if (member.EntityType == EntityType.Constructor)
			{
				//if this is an External constructor, we don't care about mapping it here.
				var mappedCtor = member as GenericMappedConstructor;
				if (mappedCtor != null)
					MapConstructor(node, mappedCtor);
				return;
			}
			var genArgs = node.GenericArguments
				.Select(ga => _methodToStateMachineMapper.MapType((IType) ga.Entity))
				.ToArray();
			var mapped = member as IGenericMappedMember;
			if (mapped != null)
			{
				var source = mapped.SourceMember;
				member = source.DeclaringType.GenericInfo.ConstructType(genArgs).ConstructedInfo.Map(source);
			}
			else
			{
				var method = ((IMethod)member).ConstructedInfo;
				member = method.GenericDefinition.GenericInfo.ConstructMethod(genArgs);
			}

			node.Entity = member;
			node.ExpressionType = member.Type;
		}

	    private void MapConstructor(GenericReferenceExpression node, GenericMappedConstructor member)
	    {
		    var source = member.SourceMember;
		    var genArgs = node.GenericArguments
				.Select(ga => _methodToStateMachineMapper.MapType((IType) ga.Entity))
				.ToArray();
		    var result = source.DeclaringType.GenericInfo.ConstructType(genArgs).ConstructedInfo.Map(source);
		    node.Entity = result;
		    node.ExpressionType = result.Type;
	    }

	    private static IType[] GetGenericParams(MemberReferenceExpression node)
	    {
			var target = node.Target.Entity ?? node.Target.ExpressionType;			
		    IType targetType;
			if (target is IMember)
				targetType = ((IMember)target).DeclaringType;
			else if (target.EntityType == EntityType.Type)
				targetType = (IType)target;
			else return null;

		    IType[] genParams;
		    if (targetType.ConstructedInfo != null)
			    genParams = targetType.ConstructedInfo.GenericArguments;
		    else if (targetType.GenericInfo != null)
			    genParams = System.Array.ConvertAll(targetType.GenericInfo.GenericParameters, igp => (IType) igp);
		    else genParams = null;
		    return genParams;
	    }

	    public override void OnDeclaration(Declaration node)
        {
            base.OnDeclaration(node);
			if (node.Entity != null && _entityMapper.ContainsKey(node.Entity))
                node.Entity = _entityMapper[node.Entity];
        }

		public override void OnMethodInvocationExpression(MethodInvocationExpression node)
		{
			var superInvocation = IsInvocationOnSuperMethod(node);
			var et = node.ExpressionType;
			base.OnMethodInvocationExpression(node);
			if (node.Target.Entity.EntityType == EntityType.Field)
				ContextAnnotations.AddFieldInvocation(node);
			else if (node.Target.Entity.EntityType == EntityType.BuiltinFunction)
			{
				if (node.Target.Entity == BuiltinFunction.Default)
					node.ExpressionType = node.Arguments[0].ExpressionType;
			}
			if (et != null && 
				((et.GenericInfo != null || 
					(et.ConstructedInfo != null && !et.ConstructedInfo.FullyConstructed)) 
					&& node.Target.ExpressionType != null))
				node.ExpressionType = node.Target.Entity.EntityType == EntityType.Constructor ?
					((IConstructor)node.Target.Entity).DeclaringType:
					((ICallableType)node.Target.ExpressionType).GetSignature().ReturnType;
			if (!superInvocation)
				return;

			var accessor = CreateAccessorForSuperMethod(node.Target);
			Bind(node.Target, accessor);
		}

		public override void OnGenericReferenceExpression(GenericReferenceExpression node)
		{
			base.OnGenericReferenceExpression(node);
			node.ExpressionType = _methodToStateMachineMapper.MapType(node.ExpressionType);
			var member = node.Entity as IMember;
			if (member != null)
				MapMember(node, member);
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

		protected string UniqueName(string name)
		{
			return Context.GetUniqueName(name);
		}

		protected TypeDefinition DeclaringTypeDefinition
		{
			get { return _method.Method.DeclaringType; }
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
			if (null == _externalSelfField)
			{
				_externalSelfField = DeclareFieldInitializedFromConstructorParameter(
					_stateMachineClass,
					_stateMachineConstructor,
					"self_",
					TypeSystemServices.SelfMapGenericType(_method.DeclaringType),
                    _methodToStateMachineMapper);
			}

			return _externalSelfField;
		}

        protected sealed class TryStatementInfo
		{
			internal TryStatement _statement;
			internal TryStatementInfo _parent;
			
			internal bool _containsYield;
			internal int _stateNumber = -1;
			internal Block _replacement;
			
			internal IMethod _ensureMethod;
			internal ExceptionHandlerCollection _handlers;
		}

        protected readonly System.Collections.Generic.List<TryStatementInfo> _convertedTryStatements 
            = new System.Collections.Generic.List<TryStatementInfo>();
        protected readonly Stack<TryStatementInfo> _tryStatementStack = new Stack<TryStatementInfo>();
		
		public override bool EnterTryStatement(TryStatement node)
		{
			var info = new TryStatementInfo();
			info._statement = node;
			if (_tryStatementStack.Count > 0)
				info._parent = _tryStatementStack.Peek();
			_tryStatementStack.Push(info);
			return true;
		}
		
		protected virtual BinaryExpression SetStateTo(int num)
		{
			return CodeBuilder.CreateAssignment(CodeBuilder.CreateMemberReference(_state),
			                                    CodeBuilder.CreateIntegerLiteral(num));
		}
		
		public override void LeaveTryStatement(TryStatement node)
		{
			TryStatementInfo info = _tryStatementStack.Pop();
			if (info._containsYield) {
				ReplaceCurrentNode(info._replacement);
				info._handlers = node.ExceptionHandlers;
				TryStatementInfo currentTry = (_tryStatementStack.Count > 0) ? _tryStatementStack.Peek() : null;
				info._replacement.Add(node.ProtectedBlock);
				if (currentTry != null) {
					ConvertTryStatement(currentTry);
					info._replacement.Add(SetStateTo(currentTry._stateNumber));
				} else {
					// leave try block, reset state to prevent ensure block from being called again
					info._replacement.Add(SetStateTo(_finishedStateNumber));
				}
				if (info._statement.EnsureBlock != null)
				{
					BooMethodBuilder ensureMethod = _stateMachineClass.AddMethod("$ensure" + info._stateNumber,
						TypeSystemServices.VoidType, TypeMemberModifiers.Private);
					ensureMethod.Body.Add(info._statement.EnsureBlock);
					info._ensureMethod = ensureMethod.Entity;
					info._replacement.Add(CallMethodOnSelf(ensureMethod.Entity));
				}
				_convertedTryStatements.Add(info);
			}
		}
		
		protected void ConvertTryStatement(TryStatementInfo currentTry)
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
		
		protected LabelStatement CreateLabel(Node sourceNode)
		{
			InternalLabel label = CodeBuilder.CreateLabel(sourceNode, "$state$" + _labels.Count);
			_labels.Add(label.LabelStatement);
			_tryStatementInfoForLabels.Add(_tryStatementStack.Count > 0 ? _tryStatementStack.Peek() : null);
			return label.LabelStatement;
		}
		
		protected virtual BooMethodBuilder CreateConstructor(BooClassBuilder builder)
		{
			BooMethodBuilder constructor = builder.AddConstructor();
			constructor.Body.Add(CodeBuilder.CreateSuperConstructorInvocation(builder.Entity.BaseType));
			return constructor;
		}
   }
}
