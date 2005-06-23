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
	using Boo.Lang.Compiler.Ast;
	
	public class BooCodeBuilder
	{
		protected TypeSystemServices _tss;
		
		public BooCodeBuilder(TypeSystemServices tss)
		{
			if (null == tss)
			{
				throw new ArgumentNullException("tss");
			}
			
			_tss = tss;
		}
		
		public TypeSystemServices TypeSystemServices
		{
			get
			{
				return _tss;
			}
		}
		
		public CompilerContext Context
		{
			get
			{
				return _tss.Context;
			}
		}

		public int GetFirstParameterIndex(TypeMember member)
		{
			return member.IsStatic ? 0 : 1;
		}

		public Statement CreateFieldAssignment(Field node, Expression initializer)
		{
			InternalField fieldEntity = (InternalField)TypeSystemServices.GetEntity(node);
			
			ExpressionStatement stmt = new ExpressionStatement(initializer.LexicalInfo);
			Expression context = node.IsStatic
				? (Expression) CreateReference(node.LexicalInfo, fieldEntity.DeclaringType)
				: CreateSelfReference(fieldEntity.Type);
			stmt.Expression = this.CreateAssignment(initializer.LexicalInfo,
				CreateMemberReference(context, fieldEntity),
				initializer);
			return stmt;
		}
		
		public Boo.Lang.Compiler.Ast.Attribute CreateAttribute(IConstructor constructor, Expression arg)
		{
			Boo.Lang.Compiler.Ast.Attribute attribute = new Boo.Lang.Compiler.Ast.Attribute();
			attribute.Name = constructor.DeclaringType.FullName;
			attribute.Entity = constructor;
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
		
		public InternalLabel CreateLabelStatement(Node sourceNode, string name)
		{
			return new InternalLabel(new LabelStatement(sourceNode.LexicalInfo, name));
		}
		
		public ReferenceExpression CreateLabelReference(LabelStatement label)
		{
			ReferenceExpression reference = new ReferenceExpression(label.LexicalInfo, label.Name);
			reference.Entity = label.Entity;
			return reference;
		}
		
		public Statement CreateSwitch(Expression offset, System.Collections.IEnumerable labels)
		{
			MethodInvocationExpression sw = new MethodInvocationExpression();
			sw.Target = new ReferenceExpression("__switch__");
			sw.Target.Entity = BuiltinFunction.Switch;
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
			mie.Target = new ReferenceExpression("__addressof__");
			mie.Target.Entity = BuiltinFunction.AddressOf;
			mie.Arguments.Add(CreateMethodReference(method));
			mie.ExpressionType = _tss.IntPtrType;
			return mie;
		}
		
		public MethodInvocationExpression CreateMethodInvocation(Expression target, System.Reflection.MethodInfo method)
		{
			return CreateMethodInvocation(target, _tss.Map(method));
		}
		
		public MethodInvocationExpression CreatePropertyGet(Expression target, IProperty property)
		{
			return CreateMethodInvocation(target, property.GetGetMethod());
		}
		
		public MethodInvocationExpression CreatePropertySet(Expression target, IProperty property, Expression value)
		{
			return CreateMethodInvocation(target, property.GetSetMethod(), value);
		}
		
		public MethodInvocationExpression CreateMethodInvocation(System.Reflection.MethodInfo staticMethod, Expression arg)
		{
			return CreateMethodInvocation(_tss.Map(staticMethod), arg);
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
		
		public MethodInvocationExpression CreateMethodInvocation(IMethod staticMethod, Expression arg0, Expression arg1)
		{
			MethodInvocationExpression mie = CreateMethodInvocation(staticMethod, arg0);
			mie.Arguments.Add(arg1);
			return mie;
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
		
		public MethodInvocationExpression CreateMethodInvocation(IMethod staticMethod)
		{
			MethodInvocationExpression mie = new MethodInvocationExpression();
			mie.Target = new ReferenceExpression(staticMethod.FullName);
			mie.Target.Entity = staticMethod;
			mie.ExpressionType = staticMethod.ReturnType;
			return mie;
		}
		
		public Boo.Lang.Compiler.Ast.TypeReference CreateTypeReference(System.Type type)
		{
			return CreateTypeReference(_tss.Map(type));
		}
		
		public Boo.Lang.Compiler.Ast.TypeReference CreateTypeReference(IType tag)
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
			IType declaringType = member.DeclaringType;
			
			Expression target = null;
			if (member.IsStatic)
			{
				target = CreateReference(declaringType);
			}
			else
			{
				target = CreateSelfReference(declaringType);
			}
			return CreateMemberReference(target, member);
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
		
		public MethodInvocationExpression CreateMethodInvocation(Expression target, IMethod tag)
		{
			MethodInvocationExpression mie = new MethodInvocationExpression(target.LexicalInfo);
			mie.Target = CreateMemberReference(target, tag);			
			mie.ExpressionType = tag.ReturnType;			
			return mie;			
		}
		
		public ReferenceExpression CreateReference(LexicalInfo info, IType type)
		{
			ReferenceExpression expression = CreateReference(type);
			expression.LexicalInfo = info;
			return expression;
		}
		
		public ReferenceExpression CreateReference(IType type)
		{
			ReferenceExpression reference = new ReferenceExpression(type.FullName);
			reference.Entity = type;
			return reference;
		}
		
		public MethodInvocationExpression CreateEvalInvocation(LexicalInfo li)
		{
			MethodInvocationExpression eval = new MethodInvocationExpression(li);
			eval.Target = new ReferenceExpression("__eval__");
			eval.Target.Entity = BuiltinFunction.Eval;
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
			ArrayLiteralExpression array = new ArrayLiteralExpression();
			array.ExpressionType = _tss.ObjectArrayType;
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
			return CreateReference((InternalParameter)TypeSystemServices.GetEntity(parameter));
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
		
		public ParameterDeclaration CreateParameterDeclaration(int index, string name, IType type)
		{
			ParameterDeclaration parameter = new ParameterDeclaration(name, CreateTypeReference(type));
			parameter.Entity = new InternalParameter(parameter, index);
			return parameter;
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
		
		public MethodInvocationExpression CreateConstructorInvocation(IConstructor constructor)
		{
			MethodInvocationExpression mie = new MethodInvocationExpression();
			mie.Target = new ReferenceExpression(constructor.DeclaringType.FullName);			
			mie.Target.Entity = constructor;
			mie.ExpressionType = constructor.DeclaringType;
			return mie;
		}
		
		public Statement CreateSuperConstructorInvocation(IType baseType)
		{			
			IConstructor defaultConstructor = _tss.GetDefaultConstructor(baseType);
			System.Diagnostics.Debug.Assert(null != defaultConstructor);
			return CreateSuperConstructorInvocation(defaultConstructor);
		}
		
		public Statement CreateSuperConstructorInvocation(IConstructor defaultConstructor)
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
			return method;
		}
		
		public Property CreateProperty(string name, IType type)
		{
			Property property = new Property(name);
			property.Modifiers = TypeMemberModifiers.Public;
			property.Type = CreateTypeReference(type);
			property.Entity = new InternalProperty(_tss, property);
			return property;
		}
		
		public Field CreateField(string name, IType type)
		{
			Field field = new Field();
			field.Modifiers = TypeMemberModifiers.Protected;
			field.Name = name;
			field.Type = CreateTypeReference(type);
			field.Entity = new InternalField(field);
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
		
		public Method CreateRuntimeMethod(string name, IType returnType, IParameter[] parameters)
		{
			Method method = CreateRuntimeMethod(name, returnType);
			DeclareParameters(method, 0, parameters);
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
						p.Type));
			}
		}
		
		public Method CreateAbstractMethod(LexicalInfo lexicalInfo, IMethod baseMethod)
		{
			Method method = new Method(lexicalInfo);
			method.Name = baseMethod.Name;
			method.Modifiers = TypeMemberModifiers.Public | TypeMemberModifiers.Abstract;
			
			IParameter[] parameters = baseMethod.GetParameters();
			for (int i=0; i<parameters.Length; ++i)
			{
				method.Parameters.Add(CreateParameterDeclaration(i + 1, "arg" + i, parameters[i].Type));
			}
			method.ReturnType = CreateTypeReference(baseMethod.ReturnType);			
			method.Entity = new InternalMethod(_tss, method);
			return method;
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
		
		public InternalLocal DeclareTempLocal(Method node, IType type)
		{
			InternalLocal local = DeclareLocal(node, "___temp" + Context.AllocIndex(), type);
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
	}
}
