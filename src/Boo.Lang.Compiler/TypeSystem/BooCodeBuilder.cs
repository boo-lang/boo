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

namespace Boo.Lang.Compiler.TypeSystem
{
	using System;
	using System.Collections;
	using System.Diagnostics;
	using System.Reflection;
	using Boo.Lang.Compiler.Ast;
	using Attribute = Boo.Lang.Compiler.Ast.Attribute;

	public class BooCodeBuilder
	{
		protected TypeSystemServices _tss;
		
		public BooCodeBuilder(TypeSystemServices tss)
		{
			if (null == tss) throw new ArgumentNullException("tss");
			_tss = tss;
		}
		
		public TypeSystemServices TypeSystemServices
		{
			get { return _tss; }
		}
		
		public CompilerContext Context
		{
			get { return _tss.Context; }
		}

		public int GetFirstParameterIndex(TypeMember member)
		{
			return member.IsStatic ? 0 : 1;
		}

		public Statement CreateFieldAssignment(Field node, Expression initializer)
		{
			InternalField fieldEntity = (InternalField)TypeSystem.TypeSystemServices.GetEntity(node);
			return CreateFieldAssignment(node.LexicalInfo, fieldEntity, initializer);
		}
		
		public Statement CreateFieldAssignment(LexicalInfo lexicalInfo, IField fieldEntity, Expression initializer)
		{
			return new ExpressionStatement(lexicalInfo,
				CreateFieldAssignmentExpression(fieldEntity, initializer));
		}

		public Expression CreateFieldAssignmentExpression(IField fieldEntity, Expression initializer)
		{
			return CreateAssignment(initializer.LexicalInfo, CreateReference(fieldEntity), initializer);
		}

		public Attribute CreateAttribute(System.Type type)
		{
			return CreateAttribute(_tss.Map(type));
		}
		
		public Attribute CreateAttribute(IType type)
		{
			// TODO: check for the existence of a default constructor
			return CreateAttribute(_tss.GetDefaultConstructor(type));
		}
		
		public Attribute CreateAttribute(IConstructor constructor)
		{
			Attribute attribute = new Attribute();
			attribute.Name = constructor.DeclaringType.FullName;
			attribute.Entity = constructor;
			return attribute;
		}
		
		public Attribute CreateAttribute(IConstructor constructor, Expression arg)
		{
			Attribute attribute = CreateAttribute(constructor);
			attribute.Arguments.Add(arg);
			return attribute;
		}
		
		public BooClassBuilder CreateClass(string name)
		{
			return new BooClassBuilder(this, name);
		}
		
		public BooClassBuilder CreateClass(string name, TypeMemberModifiers modifiers)
		{
			BooClassBuilder builder = CreateClass(name);
			builder.Modifiers = modifiers;
			return builder;
		}

		public Expression CreateDefaultInitializer(LexicalInfo li, InternalLocal local)
		{
			if (local.Type.IsValueType)
			{
				return CreateInitValueType(li, local);
			}
			return CreateAssignment(
					li,
					CreateReference(local),
					CreateNullLiteral());
		}

		public Expression CreateInitValueType(LexicalInfo li, InternalLocal local)
		{
			MethodInvocationExpression mie = CreateBuiltinInvocation(li, BuiltinFunction.InitValueType);
			mie.Arguments.Add(CreateReference(local));
			return mie;
		}

		public Expression CreateCast(IType type, Expression target)
		{
			if (type == target.ExpressionType)
			{
				return target;
			}
			
			CastExpression expression = new CastExpression(target.LexicalInfo);
			expression.Type = CreateTypeReference(type);
			expression.Target = target;
			expression.ExpressionType = type;
			return expression;
		}
		
		public Expression CreateTypeofExpression(IType type)
		{
			TypeofExpression expression = new TypeofExpression();
			expression.Type = CreateTypeReference(type);
			expression.ExpressionType = _tss.TypeType;
			expression.Entity = type;
			return expression;
		}

		public Expression CreateTypeofExpression(System.Type type)
		{
			return CreateTypeofExpression(_tss.Map(type));
		}
		
		public ReferenceExpression CreateLabelReference(LabelStatement label)
		{
			ReferenceExpression reference = new ReferenceExpression(label.LexicalInfo, label.Name);
			reference.Entity = label.Entity;
			return reference;
		}
		
		public Statement CreateSwitch(LexicalInfo li, Expression offset, IEnumerable labels)
		{
			offset.LexicalInfo = li;
			return CreateSwitch(offset, labels);
		}
		
		public Statement CreateSwitch(Expression offset, IEnumerable labels)
		{
			MethodInvocationExpression sw = CreateBuiltinInvocation(offset.LexicalInfo, BuiltinFunction.Switch);
			sw.Arguments.Add(offset);
			foreach (LabelStatement label in labels)
			{
				sw.Arguments.Add(CreateLabelReference(label));
			}
			sw.ExpressionType = _tss.VoidType;
			return new ExpressionStatement(sw);
		}
		
		public Expression CreateAddressOfExpression(IMethod method)
		{
			MethodInvocationExpression mie = new MethodInvocationExpression();
			mie.Target = CreateBuiltinReference(BuiltinFunction.AddressOf);
			mie.Arguments.Add(CreateMethodReference(method));
			mie.ExpressionType = _tss.IntPtrType;
			return mie;
		}
		
		public MethodInvocationExpression CreateMethodInvocation(Expression target, MethodInfo method)
		{
			return CreateMethodInvocation(target, _tss.Map(method));
		}
		
		public MethodInvocationExpression CreatePropertyGet(Expression target, IProperty property)
		{
			return property.IsExtension
				? CreateMethodInvocation(property.GetGetMethod(), target)
				: CreateMethodInvocation(target, property.GetGetMethod());
		}
		
		public MethodInvocationExpression CreatePropertySet(Expression target, IProperty property, Expression value)
		{
			return CreateMethodInvocation(target, property.GetSetMethod(), value);
		}
		
		public MethodInvocationExpression CreateMethodInvocation(MethodInfo staticMethod, Expression arg)
		{
			return CreateMethodInvocation(_tss.Map(staticMethod), arg);
		}

		public MethodInvocationExpression CreateMethodInvocation(MethodInfo staticMethod, Expression arg0, Expression arg1)
		{
			return CreateMethodInvocation(_tss.Map(staticMethod), arg0, arg1);
		}

		public MethodInvocationExpression CreateMethodInvocation(LexicalInfo li, Expression target, IMethod tag, Expression arg)
		{
			MethodInvocationExpression mie = CreateMethodInvocation(target, tag, arg);
			mie.LexicalInfo = li;
			return mie;
		}
		
		public MethodInvocationExpression CreateMethodInvocation(Expression target, IMethod tag, Expression arg)
		{
			MethodInvocationExpression mie = CreateMethodInvocation(target, tag);
			mie.Arguments.Add(arg);
			return mie;
		}
		
		public MethodInvocationExpression CreateMethodInvocation(Expression target, IMethod entity, Expression arg1, Expression arg2)
		{
			MethodInvocationExpression mie = CreateMethodInvocation(target, entity, arg1);
			mie.Arguments.Add(arg2);
			return mie;
		}

		public MethodInvocationExpression CreateMethodInvocation(LexicalInfo li, IMethod staticMethod, Expression arg0, Expression arg1)
		{
			MethodInvocationExpression expression = CreateMethodInvocation(staticMethod, arg0, arg1);
			expression.LexicalInfo = li;
			return expression;
		}
		
		public MethodInvocationExpression CreateMethodInvocation(IMethod staticMethod, Expression arg0, Expression arg1)
		{
			MethodInvocationExpression mie = CreateMethodInvocation(staticMethod, arg0);
			mie.Arguments.Add(arg1);
			return mie;
		}

		public MethodInvocationExpression CreateMethodInvocation(LexicalInfo li, IMethod staticMethod, Expression arg0, Expression arg1, Expression arg2)
		{
			MethodInvocationExpression expression = CreateMethodInvocation(staticMethod, arg0, arg1, arg2);
			expression.LexicalInfo = li;
			return expression;
		}

		public MethodInvocationExpression CreateMethodInvocation(IMethod staticMethod, Expression arg0, Expression arg1, Expression arg2)
		{
			MethodInvocationExpression mie = CreateMethodInvocation(staticMethod, arg0, arg1);
			mie.Arguments.Add(arg2);
			return mie;
		}
		
		public MethodInvocationExpression CreateMethodInvocation(IMethod staticMethod, Expression arg)
		{
			MethodInvocationExpression mie = CreateMethodInvocation(staticMethod);
			mie.LexicalInfo = arg.LexicalInfo;
			mie.Arguments.Add(arg);
			return mie;
		}
		
		public MethodInvocationExpression CreateMethodInvocation(IMethod method)
		{
			MethodInvocationExpression mie = new MethodInvocationExpression();
			mie.Target = CreateMemberReference(method);
			mie.ExpressionType = method.ReturnType;
			return mie;
		}
		
		public TypeReference CreateTypeReference(Type type)
		{
			return CreateTypeReference(_tss.Map(type));
		}

		public TypeReference CreateTypeReference(LexicalInfo li, Type type)
		{
			return CreateTypeReference(li, _tss.Map(type));
		}

		public TypeReference CreateTypeReference(LexicalInfo li, IType type)
		{
			TypeReference reference = CreateTypeReference(type);
			reference.LexicalInfo = li;
			return reference;
		}

		public TypeReference CreateTypeReference(IType tag)
		{
			TypeReference typeReference = null;
			
			if (tag.IsArray)
			{
				IType elementType = ((IArrayType)tag).GetElementType();
				//typeReference = new ArrayTypeReference();
				//((ArrayTypeReference)typeReference).ElementType = CreateTypeReference(elementType);
				// FIXME: This is what it *should* be, but it causes major breakage. ??
				typeReference = new ArrayTypeReference(CreateTypeReference(elementType), CreateIntegerLiteral(((IArrayType)tag).GetArrayRank()));
			}
			else
			{				
				typeReference = new SimpleTypeReference(tag.FullName);				
			}
			
			typeReference.Entity = tag;
			return typeReference;
		}
		
		public SuperLiteralExpression CreateSuperReference(IType super)
		{
			SuperLiteralExpression expression = new SuperLiteralExpression();
			expression.ExpressionType = super;
			return expression;
		}
		
		public SelfLiteralExpression CreateSelfReference(IType self)
		{
			SelfLiteralExpression expression = new SelfLiteralExpression();
			expression.ExpressionType = self;
			return expression;
		}
		
		public ReferenceExpression CreateLocalReference(string name, InternalLocal entity)
		{
			return CreateTypedReference(name, entity);
		}
		
		public ReferenceExpression CreateTypedReference(string name, ITypedEntity entity)
		{
			ReferenceExpression expression = new ReferenceExpression(name);
			expression.Entity = entity;
			expression.ExpressionType = entity.Type;
			return expression;
		}
		
		public ReferenceExpression CreateReference(IEntity entity)
		{
			switch (entity.EntityType)
			{
				case EntityType.Local: return CreateReference((InternalLocal)entity);
				case EntityType.Field: return CreateReference((IField)entity);
				case EntityType.Parameter: return CreateReference((InternalParameter)entity);
				case EntityType.Custom: return CreateTypedReference(entity.Name, (ITypedEntity)entity);
				case EntityType.Property: return CreateReference((IProperty)entity);
            }
            return CreateTypedReference(entity.Name, (ITypedEntity)entity);
        }
		
		public ReferenceExpression CreateReference(InternalLocal local)
		{
			return CreateLocalReference(local.Name, local);
		}
		
		public MemberReferenceExpression CreateReference(Field field)
		{
			return CreateReference((IField)field.Entity);
		}
		
		public MemberReferenceExpression CreateReference(IField field)
		{
			return CreateMemberReference(field);
		}

        public MemberReferenceExpression CreateReference(Property property)
        {
            return CreateReference((IProperty)property.Entity);
        }
        public MemberReferenceExpression CreateReference(IProperty property)
        {
            return CreateMemberReference(property);
        }

        public MemberReferenceExpression CreateMemberReference(IMember member)
		{
        	Expression target = member.IsStatic
				? (Expression)CreateReference(member.DeclaringType)
				: (Expression)CreateSelfReference(member.DeclaringType);
			return CreateMemberReference(target, member);
		}

		public MemberReferenceExpression CreateMemberReference(LexicalInfo li, Expression target, IMember member)
		{
			MemberReferenceExpression expression = CreateMemberReference(target, member);
			expression.LexicalInfo = li;
			return expression;
		}

		public MemberReferenceExpression CreateMemberReference(Expression target, IMember member)
		{
			MemberReferenceExpression reference = new MemberReferenceExpression(target.LexicalInfo);
			reference.Target = target;
			reference.Name = member.Name;
			reference.Entity = member;
			reference.ExpressionType = member.Type;
			return reference;
		}

		public MethodInvocationExpression CreateMethodInvocation(LexicalInfo li, Expression target, IMethod entity)
		{
			MethodInvocationExpression expression = CreateMethodInvocation(target, entity);
			expression.LexicalInfo = li;
			return expression;
		}

		public MethodInvocationExpression CreateMethodInvocation(Expression target, IMethod entity)
		{
			MethodInvocationExpression mie = new MethodInvocationExpression(target.LexicalInfo);
			mie.Target = CreateMemberReference(target, entity);			
			mie.ExpressionType = entity.ReturnType;
			return mie;			
		}
		
		public ReferenceExpression CreateReference(LexicalInfo info, IType type)
		{
			ReferenceExpression expression = CreateReference(type);
			expression.LexicalInfo = info;
			return expression;
		}

		public ReferenceExpression CreateReference(LexicalInfo li, System.Type type)
		{
			return CreateReference(li, _tss.Map(type));
		}
		
		public ReferenceExpression CreateReference(IType type)
		{
			ReferenceExpression reference = new ReferenceExpression(type.FullName);
			reference.Entity = type;
			reference.IsSynthetic = true;
			return reference;
		}
		
		public MethodInvocationExpression CreateEvalInvocation(LexicalInfo li)
		{
			return CreateBuiltinInvocation(li, BuiltinFunction.Eval);
		}

		private static MethodInvocationExpression CreateBuiltinInvocation(LexicalInfo li, BuiltinFunction builtin)
		{
			MethodInvocationExpression eval = new MethodInvocationExpression(li);
			eval.Target = CreateBuiltinReference(builtin);
			return eval;
		}

		public static ReferenceExpression CreateBuiltinReference(BuiltinFunction builtin)
		{
			ReferenceExpression target = new ReferenceExpression(builtin.Name);
			target.Entity = builtin;
			return target;
		}

		public MethodInvocationExpression CreateEvalInvocation(LexicalInfo li, Expression arg, Expression value)
		{
			MethodInvocationExpression eval = CreateEvalInvocation(li);
			eval.Arguments.Add(arg);
			eval.Arguments.Add(value);
			eval.ExpressionType = value.ExpressionType;
			return eval;
		}
		
		public UnpackStatement CreateUnpackStatement(DeclarationCollection declarations, Expression expression)
		{
			UnpackStatement unpack = new UnpackStatement(expression.LexicalInfo);
			unpack.Declarations.Extend(declarations);
			unpack.Expression = expression;
			return unpack;
		}
		
		public BinaryExpression CreateAssignment(LexicalInfo li, Expression lhs, Expression rhs)
		{
			BinaryExpression assignment = CreateAssignment(lhs, rhs);
			assignment.LexicalInfo = li;
			return assignment;
		}
		
		public BinaryExpression CreateAssignment(Expression lhs, Expression rhs)
		{
			BinaryExpression assignment = new BinaryExpression(
											BinaryOperatorType.Assign,
											lhs,
											rhs);
			assignment.ExpressionType = _tss.GetExpressionType(lhs);
			return assignment;
		}
		
		public Expression CreateMethodReference(IMethod method)
		{			
			return CreateMemberReference(method);
		}

		public Expression CreateMethodReference(LexicalInfo lexicalInfo, IMethod method)
		{
			Expression e = CreateMethodReference(method);
			e.LexicalInfo = lexicalInfo;
			return e;
		}
		
		public BinaryExpression CreateBoundBinaryExpression(IType expressionType,
												BinaryOperatorType op,
												Expression lhs,
												Expression rhs)
		{
			BinaryExpression expression = new BinaryExpression(op, lhs, rhs);
			expression.ExpressionType = expressionType;
			return expression;
		}
		
		public BoolLiteralExpression CreateBoolLiteral(bool value)
		{
			BoolLiteralExpression expression = new BoolLiteralExpression(value);
			expression.ExpressionType = _tss.BoolType;
			return expression;
		}
		
		public StringLiteralExpression CreateStringLiteral(string value)
		{
			StringLiteralExpression expression = new StringLiteralExpression(value);
			expression.ExpressionType = _tss.StringType;
			return expression;
		}
		
		public NullLiteralExpression CreateNullLiteral()
		{
			NullLiteralExpression expression = new NullLiteralExpression();
			expression.ExpressionType = Null.Default;
			return expression;
		}
		
		public ArrayLiteralExpression CreateObjectArray(ExpressionCollection items)
		{
			return CreateArray(_tss.ObjectArrayType, items);
		}

		public ArrayLiteralExpression CreateArray(IType arrayType, ExpressionCollection items)
		{
			if (!arrayType.IsArray) throw new ArgumentException(string.Format("'{0}'  is not an array type!", arrayType), "arrayType");
			ArrayLiteralExpression array = new ArrayLiteralExpression();
			array.ExpressionType = arrayType;
			array.Items.Extend(items);
			_tss.MapToConcreteExpressionTypes(array.Items);
			return array;
		}
		
		public IntegerLiteralExpression CreateIntegerLiteral(int value)
		{
			IntegerLiteralExpression integer = new IntegerLiteralExpression(value);
			integer.ExpressionType = _tss.IntType;
			return integer;
		}

		public IntegerLiteralExpression CreateIntegerLiteral(long value)
		{
			IntegerLiteralExpression integer = new IntegerLiteralExpression(value);
			integer.ExpressionType = _tss.LongType;
			return integer;
		}
		
		public SlicingExpression CreateSlicing(Expression target, int begin)
		{
			SlicingExpression expression = new SlicingExpression(target,
												CreateIntegerLiteral(begin));
												
			IType expressionType = _tss.ObjectType;
			IArrayType arrayType = target.ExpressionType as IArrayType;
			if (null != arrayType)
			{
				expressionType = arrayType.GetElementType();
			}
			expression.ExpressionType = expressionType;			
			return expression;
		}
		
		public ReferenceExpression CreateReference(ParameterDeclaration parameter)
		{
			return CreateReference((InternalParameter)TypeSystem.TypeSystemServices.GetEntity(parameter));
		}
		
		public ReferenceExpression CreateReference(InternalParameter parameter)
		{
			ReferenceExpression reference = new ReferenceExpression(parameter.Name);
			reference.Entity = parameter;
			reference.ExpressionType = parameter.Type;
			return reference;
		}
		
		public UnaryExpression CreateNotExpression(Expression node)
		{
			UnaryExpression notNode = new UnaryExpression();
			notNode.LexicalInfo = node.LexicalInfo;
			notNode.Operand = node;
			notNode.Operator = UnaryOperatorType.LogicalNot;
			
			notNode.ExpressionType = _tss.BoolType;
			return notNode;
		}
		
		public ParameterDeclaration CreateParameterDeclaration(int index, string name, IType type, bool byref)
		{
			ParameterModifiers modifiers = ParameterModifiers.None;
			if (byref)
			{
				modifiers |= ParameterModifiers.Ref;
			}
			ParameterDeclaration parameter = new ParameterDeclaration(name, 
								CreateTypeReference(type),
								modifiers);
			parameter.Entity = new InternalParameter(parameter, index);
			return parameter;
		}
		
		public ParameterDeclaration CreateParameterDeclaration(int index, string name, IType type)
		{
			return CreateParameterDeclaration(index, name, type, false);
		}
		
		public Constructor CreateConstructor(TypeMemberModifiers modifiers)
		{
			Constructor constructor = new Constructor();
			constructor.Modifiers = modifiers;
			constructor.Entity = new InternalConstructor(_tss, constructor);
			constructor.IsSynthetic = true;
			return constructor;
		}
		
		public MethodInvocationExpression CreateConstructorInvocation(ClassDefinition cd)
		{
			IConstructor constructor = ((IType)cd.Entity).GetConstructors()[0];
			return CreateConstructorInvocation(constructor);
		}
		
		public MethodInvocationExpression CreateConstructorInvocation(IConstructor constructor, Expression arg1, Expression arg2)
		{
			MethodInvocationExpression mie = CreateConstructorInvocation(constructor, arg1);
			mie.Arguments.Add(arg2);
			return mie;
		}
		
		public MethodInvocationExpression CreateConstructorInvocation(IConstructor constructor, Expression arg)
		{
			MethodInvocationExpression mie = CreateConstructorInvocation(constructor);
			mie.LexicalInfo = arg.LexicalInfo;			
			mie.Arguments.Add(arg);
			return mie;
		}

		public MethodInvocationExpression CreateConstructorInvocation(LexicalInfo lexicalInfo, IConstructor constructor, params Expression[] args)
		{
			MethodInvocationExpression mie = CreateConstructorInvocation(constructor);
			mie.LexicalInfo = lexicalInfo;
			mie.Arguments.Extend(args);
			return mie;
		}
		
		public MethodInvocationExpression CreateConstructorInvocation(IConstructor constructor)
		{
			MethodInvocationExpression mie = new MethodInvocationExpression();
			mie.Target = new ReferenceExpression(constructor.DeclaringType.FullName);			
			mie.Target.Entity = constructor;
			mie.ExpressionType = constructor.DeclaringType;
			return mie;
		}
		
		public ExpressionStatement CreateSuperConstructorInvocation(IType baseType)
		{			
			IConstructor defaultConstructor = _tss.GetDefaultConstructor(baseType);
			Debug.Assert(null != defaultConstructor);
			return CreateSuperConstructorInvocation(defaultConstructor);
		}
		
		public ExpressionStatement CreateSuperConstructorInvocation(IConstructor defaultConstructor)
		{			
			MethodInvocationExpression call = new MethodInvocationExpression(new SuperLiteralExpression());			
			call.Target.Entity = defaultConstructor;
			call.ExpressionType = _tss.VoidType;			
			return new ExpressionStatement(call);
		}
		
		public MethodInvocationExpression CreateSuperMethodInvocation(IMethod superMethod)
		{
			MethodInvocationExpression mie = new MethodInvocationExpression(new SuperLiteralExpression());
			mie.Target.Entity = superMethod;
			mie.ExpressionType = superMethod.ReturnType;
			return mie;
		}
		
		public MethodInvocationExpression CreateSuperMethodInvocation(IMethod superMethod, Expression arg)
		{
			MethodInvocationExpression mie = CreateSuperMethodInvocation(superMethod);
			mie.Arguments.Add(arg);
			return mie;
		}
		
		public Method CreateVirtualMethod(string name, IType returnType)
		{
			return CreateVirtualMethod(name, CreateTypeReference(returnType));
		}
		
		public Method CreateVirtualMethod(string name, TypeReference returnType)
		{
			return CreateMethod(name,
							returnType,
							TypeMemberModifiers.Public|TypeMemberModifiers.Virtual);
		}
		
		public Method CreateMethod(string name, IType returnType, TypeMemberModifiers modifiers)
		{
			return CreateMethod(name, CreateTypeReference(returnType), modifiers);
		}
		
		public Method CreateMethod(string name, TypeReference returnType, TypeMemberModifiers modifiers)
		{
			Method method = new Method(name);
			method.Modifiers = modifiers;
			method.ReturnType = returnType;
			method.Entity = new InternalMethod(_tss, method);
			method.IsSynthetic = true;
			return method;
		}
		
		public Property CreateProperty(string name, IType type)
		{
			Property property = new Property(name);
			property.Modifiers = TypeMemberModifiers.Public;
			property.Type = CreateTypeReference(type);
			property.Entity = new InternalProperty(_tss, property);
			property.IsSynthetic = true;
			return property;
		}
		
		public Field CreateField(string name, IType type)
		{
			Field field = new Field();
			field.Modifiers = TypeMemberModifiers.Protected;
			field.Name = name;
			field.Type = CreateTypeReference(type);
			field.Entity = new InternalField(field);
			field.IsSynthetic = true;
			return field;
		}
		
		public Method CreateRuntimeMethod(string name, TypeReference returnType)
		{
			Method method = CreateVirtualMethod(name, returnType);
			method.ImplementationFlags = MethodImplementationFlags.Runtime;
			return method;
		}
		
		public Method CreateRuntimeMethod(string name, IType returnType)
		{
			return CreateRuntimeMethod(name, CreateTypeReference(returnType));
		}
		
		public Method CreateRuntimeMethod(string name, IType returnType, IParameter[] parameters, bool variableArguments)
		{
			Method method = CreateRuntimeMethod(name, returnType);
			DeclareParameters(method, 0, parameters);
			method.Parameters.VariableNumber = variableArguments;
			return method;
		}
		
		public void DeclareParameters(Method method, int parameterIndexDelta, IParameter[] parameters)
		{
			for (int i=0; i<parameters.Length; ++i)
			{
				IParameter p = parameters[i];
				method.Parameters.Add(
					CreateParameterDeclaration(parameterIndexDelta + i,
						p.Name,
						p.Type,
						p.IsByRef));
			}
		}
		
		public Method CreateAbstractMethod(LexicalInfo lexicalInfo, IMethod baseMethod)
		{
			return CreateMethodFromPrototype(lexicalInfo, baseMethod, TypeMemberModifiers.Public | TypeMemberModifiers.Abstract);
		}
		
		public Method CreateMethodFromPrototype(LexicalInfo lexicalInfo, IMethod baseMethod, TypeMemberModifiers newModifiers)
		{
			Method method = new Method(lexicalInfo);
			method.Name = baseMethod.Name;
			method.Modifiers = newModifiers;
			method.IsSynthetic = true;
			
			IParameter[] parameters = baseMethod.GetParameters();
			for (int i=0; i<parameters.Length; ++i)
			{
				method.Parameters.Add(CreateParameterDeclaration(i + 1, 
								"arg" + i, 
								parameters[i].Type,
								parameters[i].IsByRef));
			}
			method.ReturnType = CreateTypeReference(baseMethod.ReturnType);			
			method.Entity = new InternalMethod(_tss, method);
			return method;
		}

		public Event CreateAbstractEvent(LexicalInfo lexicalInfo, IEvent baseEvent)
		{
			Event ev = new Event(lexicalInfo);
			ev.Name = baseEvent.Name;
			ev.Type = CreateTypeReference(baseEvent.Type);
			ev.Add = CreateAbstractMethod(lexicalInfo, baseEvent.GetAddMethod());
			ev.Remove = CreateAbstractMethod(lexicalInfo, baseEvent.GetRemoveMethod());
			ev.Entity = new InternalEvent (_tss, ev);
			return ev;
		}
		
		public Expression CreateNotNullTest(Expression target)
		{
			BinaryExpression test = new BinaryExpression(target.LexicalInfo,
											BinaryOperatorType.ReferenceInequality,
											target,
											CreateNullLiteral());
			test.ExpressionType = _tss.BoolType;
			return test;
		}

		public RaiseStatement RaiseException(LexicalInfo lexicalInfo, IConstructor exceptionConstructor, params Expression[] args)
		{
			Debug.Assert(exceptionConstructor.DeclaringType.IsSubclassOf(this._tss.ExceptionType));
			return new RaiseStatement(lexicalInfo, CreateConstructorInvocation(lexicalInfo, exceptionConstructor, args));
		}

		public string CreateTempName()
		{
			return "___temp" + Context.AllocIndex();
		}
		
		public InternalLocal DeclareTempLocal(Method node, IType type)
		{
			InternalLocal local = DeclareLocal(node, CreateTempName(), type);
			local.IsPrivateScope = true;
			return local;
		}
		
		public InternalLocal DeclareLocal(Method node, string name, IType type)
		{
			Local local = new Local(node.LexicalInfo, name);
			InternalLocal entity = new InternalLocal(local, type);
			local.Entity = entity;
			node.Locals.Add(local);
			return entity;
		}
		
		public void BindParameterDeclarations(bool isStatic, INodeWithParameters node)
		{
			// arg0 is the this pointer when member is not static			
			int delta = isStatic ? 0 : 1; 
			ParameterDeclarationCollection parameters = node.Parameters;
			int last = parameters.Count - 1;
			for (int i=0; i<parameters.Count; ++i)
			{
				ParameterDeclaration parameter = parameters[i];
				if (null == parameter.Type)
				{
					if (last == i && parameters.VariableNumber)
					{
						parameter.Type = CreateTypeReference(_tss.ObjectArrayType);
					}
					else
					{
						parameter.Type = CreateTypeReference(_tss.ObjectType);
					}
				}
				parameter.Entity = new InternalParameter(parameter, i + delta);
			}
		}

		public InternalLabel CreateLabel(Node sourceNode, string name)
		{
			return new InternalLabel(new LabelStatement(sourceNode.LexicalInfo, name));
		}
		
		
		public TypeMember CreateStub(IMember member)
		{
			IMethod md = (member as IMethod);
			if (null == md) return null;				
				
			Method m = CreateVirtualMethod(md.Name, md.ReturnType);
			int idx = 0;
			foreach (IParameter param in md.GetParameters()) {
				m.Parameters.Add(
					CreateParameterDeclaration(idx,
											param.Name,
											param.Type,
											param.IsByRef));
				idx++;
			}
			
			MethodInvocationExpression x = new MethodInvocationExpression();
			x.Target = new MemberReferenceExpression(
								new ReferenceExpression("System"),
								"NotImplementedException");
			RaiseStatement rs = new RaiseStatement(x);
			rs.LexicalInfo = LexicalInfo.Empty;			
			m.Body.Statements.Insert(0, rs);
			
			return m;
		}
		
	}
}
