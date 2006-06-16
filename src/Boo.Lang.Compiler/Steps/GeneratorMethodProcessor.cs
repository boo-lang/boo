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


namespace Boo.Lang.Compiler.Steps
{
	using System.Collections;
	using System.Diagnostics;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;
	
	internal class ExternalMemberReferenceProcessor : DepthFirstTransformer
	{
		static readonly object AccessorsKey = new object();
		
		static readonly object SettersKey = new object();
		
		TypeDefinition _externalType;
		
		IType _currentType;
		
		CompilerContext _context;
		
		public ExternalMemberReferenceProcessor(CompilerContext context, TypeDefinition externalType)
		{
			_context = context;
			_externalType = externalType;
		}
		
		override public bool EnterClassDefinition(ClassDefinition node)
		{
			_currentType = (IType)node.Entity;
			return true;
		}
		
		bool IsCurrentType(IType type)
		{
			return type == _currentType || _currentType.IsSubclassOf(type);
		}
		
		override public void LeaveBinaryExpression(BinaryExpression node)
		{
			if (node.Operator != BinaryOperatorType.Assign) return;
			
			MemberReferenceExpression mre = node.Left as MemberReferenceExpression;
			if (null == mre) return;
						
			IAccessibleMember member = GetAccessibleMemberToReplace(mre);
			if (null == member) return;
			
			ReplaceCurrentNode(
				_context.CodeBuilder.CreateMethodInvocation(
					mre.Target,
					GetFieldSetter((IField)member),
					node.Right));
		}
		
		override public void LeaveMemberReferenceExpression(MemberReferenceExpression node)
		{
			if (AstUtil.IsLhsOfAssignment(node)) return;			
			IAccessibleMember member = GetAccessibleMemberToReplace(node);
			if (null == member) return;
			
			IMethod accessor = GetMemberAccessor(member);
			if (member.EntityType == EntityType.Field)
			{
				ReplaceCurrentNode(
					_context.CodeBuilder.CreateMethodInvocation(
						node.Target,
						accessor));
			}
			else
			{
				node.Entity = accessor;
			}
		}
		
		IAccessibleMember GetAccessibleMemberToReplace(MemberReferenceExpression node)
		{
			IAccessibleMember member = node.Entity as IAccessibleMember;
			if (member == null) return null;			
			if (member.IsPublic || member.IsInternal) return null;
			if (IsCurrentType(member.DeclaringType)) return null;
			return member;		
		}
		
		IDictionary GetDictionary(object key)
		{		
			IDictionary d = (IDictionary)_externalType[key];
			if (null != d) return d;			
			d = new Hashtable();
			_externalType[key] = d;
			return d;
		}
		
		IMethod GetFieldSetter(IField member)
		{
			IDictionary setters = GetDictionary(SettersKey);
			IMethod setter = (IMethod)setters[member];
			if (null != setter) return setter;
			
			return NewAccessor(setters, member, CreateFieldSetter(member));
		}
		
		IMethod GetMemberAccessor(IAccessibleMember member)
		{
			IDictionary accessors = GetDictionary(AccessorsKey);
			IMethod accessor = (IMethod)accessors[member];
			if (null != accessor) return accessor;
			
			return NewAccessor(accessors, member, CreateAccessorFor(member));
		}
		
		IMethod NewAccessor(IDictionary cache, IAccessibleMember member, Method accessor)
		{
			_externalType.Members.Add(accessor);			
			IMethod entity = (IMethod)accessor.Entity;
			cache.Add(member, entity);
			return entity;
		}
		
		Method CreateAccessorFor(IAccessibleMember member)
		{
			switch (member.EntityType)
			{
				case EntityType.Field:
					return CreateFieldAccessor((IField)member);
				case EntityType.Method:
					return CreateMethodAccessor((IMethod)member);
			}
			throw new System.ArgumentException("Unsupported member:" + member);
		}
		
		Method CreateFieldSetter(IField member)
		{
			BooCodeBuilder builder = _context.CodeBuilder;
			Method method = builder.CreateMethod("___" + member.Name, _context.TypeSystemServices.VoidType, TypeMemberModifiers.None);
			ParameterDeclaration value = builder.CreateParameterDeclaration(1, "value", member.Type);
			method.Parameters.Add(value);					
			method.Body.Add(
					builder.CreateFieldAssignment(
						LexicalInfo.Empty,
						member,
						builder.CreateReference(value)));
			return method;
		}
		
		Method CreateFieldAccessor(IField member)
		{
			BooCodeBuilder builder = _context.CodeBuilder;
			Method method = builder.CreateMethod("___" + member.Name, member.Type, TypeMemberModifiers.None);
			method.Body.Add(
				new ReturnStatement(
					builder.CreateReference(member)));
			return method;
		}
		
		Method CreateMethodAccessor(IMethod member)
		{
			BooCodeBuilder builder = _context.CodeBuilder;
			Method method = builder.CreateMethodFromPrototype(LexicalInfo.Empty, member, TypeMemberModifiers.None);			
			method.Name = "___" + member.Name;
			MethodInvocationExpression mie = builder.CreateMethodInvocation(member);
			foreach (ParameterDeclaration p in method.Parameters)
			{
				mie.Arguments.Add(builder.CreateReference(p));
			}
			if (member.ReturnType == _context.TypeSystemServices.VoidType)
			{
				method.Body.Add(mie);
			}
			else
			{
				method.Body.Add(new ReturnStatement(mie));
			}
			return method;
		}
	}
	
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
		
		Hashtable _mapping;
		
		IType _generatorItemType;

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
			CreateGetEnumerator(enumeratorConstructorInvocation);			
			FixGeneratorMethodBody(enumerableConstructorInvocation);			
			FixExternalMemberReferences();
		}
		
		void FixExternalMemberReferences()
		{
			ExternalMemberReferenceProcessor processor = new ExternalMemberReferenceProcessor(_context, _generator.Method.DeclaringType);
			_enumerator.ClassDefinition.Accept(processor);
		}
		
		void FixGeneratorMethodBody(MethodInvocationExpression enumerableConstructorInvocation)
		{
			Block body = _generator.Method.Body;
			body.Clear();

			body.Add(
				new ReturnStatement(
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
				enumerableConstructorInvocation.Arguments.Add(
					CodeBuilder.CreateSelfReference(type));
					
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
			return GetGetEnumeratorBuilder().Entity;
		}

		private bool GeneratorReturnsIEnumerator()
		{
			return _generator.ReturnType == TypeSystemServices.IEnumeratorType;
		}

		void CreateGetEnumerator(Expression enumeratorExpression)
		{
			BooMethodBuilder method = GetGetEnumeratorBuilder();
			method.Body.Add(new ReturnStatement(enumeratorExpression));
		}

		private BooMethodBuilder GetGetEnumeratorBuilder()
		{
			return (BooMethodBuilder)_generator.Method["GetEnumeratorBuilder"];
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
			
			_enumerator = CodeBuilder.CreateClass(_enumerable.ClassDefinition.Name + "_Enumerator");
			_enumerator.LexicalInfo = this.LexicalInfo;
			_enumerator.AddBaseType(abstractEnumeratorType);
			_enumerator.AddBaseType(TypeSystemServices.IEnumeratorType);
			
			CreateEnumeratorConstructor();
			CreateMoveNext();
			
			//_enumerable.ClassDefinition.Members.Add(_enumerator.ClassDefinition);
			TypeSystemServices.AddCompilerGeneratedType(_enumerator.ClassDefinition);
		}
		
		void CreateMoveNext()
		{
			Method generator = _generator.Method;
			
			BooMethodBuilder mn = _enumerator.AddVirtualMethod("MoveNext", TypeSystemServices.BoolType);
			mn.Method.LexicalInfo = this.LexicalInfo;
			_moveNext = mn.Entity;
			// TODO: remove this hack by making InternalMethod.Labels a calculated property
			((InternalMethod)generator.Entity).MoveLabelsTo(_moveNext);
			
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
					this.LexicalInfo,
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
				IType type = node.ExpressionType;
				_externalEnumeratorSelf = DeclareFieldInitializedFromConstructorParameter(
													_enumerator,
													_enumeratorConstructor,
													"self_",
													type);
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
					value == null ? GetDefaultYieldValue() : value);
		}

		private Expression GetDefaultYieldValue()
		{	
			if (_generatorItemType.IsValueType)
			{
				if (null == _nullValueField)
				{
					_nullValueField = _enumerator.AddField("______empty", _generatorItemType);
				}
				return CodeBuilder.CreateReference(_nullValueField);
			}
			return new NullLiteralExpression();
		}

		LabelStatement CreateLabel(Node sourceNode)
		{
			LabelStatement label = new LabelStatement(sourceNode.LexicalInfo, "___state_" + _labels.Count);
			_labels.Add(label);
			_moveNext.AddLabel(new InternalLabel(label));
			return label;
		}
		
		BooMethodBuilder CreateConstructor(BooClassBuilder builder)
		{
			BooMethodBuilder constructor = builder.AddConstructor();
			constructor.Body.Add(CodeBuilder.CreateSuperConstructorInvocation(builder.Entity.BaseType));
			return constructor;
		}
	}
}