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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Services;
using Boo.Lang.Compiler.Steps;
using Boo.Lang.Compiler.TypeSystem.Builders;
using Boo.Lang.Compiler.TypeSystem.Generics;
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Environments;
using Attribute = Boo.Lang.Compiler.Ast.Attribute;

namespace Boo.Lang.Compiler.TypeSystem
{
	public class BooCodeBuilder : ICodeBuilder
	{
		private readonly EnvironmentProvision<TypeSystemServices> _tss = new EnvironmentProvision<TypeSystemServices>();
		private readonly EnvironmentProvision<InternalTypeSystemProvider> _internalTypeSystemProvider = new EnvironmentProvision<InternalTypeSystemProvider>();

		private readonly DynamicVariable<ITypeReferenceFactory> _typeReferenceFactory;

		public BooCodeBuilder()
		{
			_typeReferenceFactory = new DynamicVariable<ITypeReferenceFactory>(new StandardTypeReferenceFactory(this));
		}

		public TypeSystemServices TypeSystemServices
		{
			get { return _tss; }
		}

		public int GetFirstParameterIndex(TypeMember member)
		{
			return member.IsStatic ? 0 : 1;
		}

		public Statement CreateFieldAssignment(Field node, Expression initializer)
		{
			var fieldEntity = (InternalField)TypeSystem.TypeSystemServices.GetEntity(node);
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
			return CreateAttribute(TypeSystemServices.Map(type));
		}

		public Attribute CreateAttribute(IType type)
		{
			return CreateAttribute(DefaultConstructorFor(type));
		}

		public Attribute CreateAttribute(IConstructor constructor)
		{
			return new Attribute { Name = constructor.DeclaringType.FullName, Entity = constructor };
		}

		public Attribute CreateAttribute(IConstructor constructor, Expression arg)
		{
			var attribute = CreateAttribute(constructor);
			attribute.Arguments.Add(arg);
			return attribute;
		}

		public Ast.Module CreateModule(string moduleName, string nameSpace)
		{
			var module = new Ast.Module { Name = moduleName };
			if (!string.IsNullOrEmpty(nameSpace)) module.Namespace = new NamespaceDeclaration(nameSpace);
			InternalTypeSystemProvider.EntityFor(module); // ensures the module is bound
			return module;
		}

		private InternalTypeSystemProvider InternalTypeSystemProvider
		{
			get { return _internalTypeSystemProvider.Instance; }
		}

		public BooClassBuilder CreateClass(string name)
		{
			return new BooClassBuilder(name);
		}

		public BooClassBuilder CreateClass(string name, TypeMemberModifiers modifiers)
		{
			var builder = CreateClass(name);
			builder.Modifiers = modifiers;
			return builder;
		}

		public Expression CreateDefaultInitializer(LexicalInfo li, ReferenceExpression target, IType type)
		{
			return type.IsValueType
				? CreateInitValueType(li, target)
				: CreateAssignment(li, target, CreateNullLiteral());
		}

		public Expression CreateDefaultInitializer(LexicalInfo li, InternalLocal local)
		{
			return CreateDefaultInitializer(li, CreateReference(local), local.Type);
		}

		public Expression CreateInitValueType(LexicalInfo li, ReferenceExpression target)
		{
			var mie = CreateBuiltinInvocation(li, BuiltinFunction.InitValueType);
			mie.Arguments.Add(target);
			return mie;
		}

		public Expression CreateInitValueType(LexicalInfo li, InternalLocal local)
		{
			return CreateInitValueType(li, CreateReference(local));
		}

		public Expression CreateCast(IType type, Expression target)
		{
			if (type == target.ExpressionType)
				return target;

			var expression = new CastExpression(target.LexicalInfo);
			expression.Type = CreateTypeReference(type);
			expression.Target = target;
			expression.ExpressionType = type;
			return expression;
		}

        public Expression CreateAsCast(IType type, Expression target)
        {
            if (type == target.ExpressionType)
                return target;

            var expression = new TryCastExpression(target.LexicalInfo)
            {
                Type = CreateTypeReference(type),
                Target = target,
                ExpressionType = type
            };
            return expression;
        }

        public TypeofExpression CreateTypeofExpression(IType type)
		{
			return new TypeofExpression
					{
						Type = CreateTypeReference(type),
						ExpressionType = TypeSystemServices.TypeType
					};
		}

		public Expression CreateTypeofExpression(System.Type type)
		{
			return CreateTypeofExpression(TypeSystemServices.Map(type));
		}

		public ReferenceExpression CreateLabelReference(LabelStatement label)
		{
			var reference = new ReferenceExpression(label.LexicalInfo, label.Name);
			reference.Entity = label.Entity;
			return reference;
		}

		public Statement CreateSwitch(LexicalInfo li, Expression offset, IEnumerable<LabelStatement> labels)
		{
			offset.LexicalInfo = li;
			return CreateSwitch(offset, labels);
		}

		public Statement CreateSwitch(Expression offset, IEnumerable<LabelStatement> labels)
		{
			MethodInvocationExpression sw = CreateBuiltinInvocation(offset.LexicalInfo, BuiltinFunction.Switch);
			sw.Arguments.Add(offset);
			foreach (LabelStatement label in labels)
			{
				sw.Arguments.Add(CreateLabelReference(label));
			}
			sw.ExpressionType = TypeSystemServices.VoidType;
			return new ExpressionStatement(sw);
		}

		public Expression CreateAddressOfExpression(IMethod method)
		{
			MethodInvocationExpression mie = new MethodInvocationExpression();
			mie.Target = CreateBuiltinReference(BuiltinFunction.AddressOf);
			mie.Arguments.Add(CreateMethodReference(method));
			mie.ExpressionType = TypeSystemServices.IntPtrType;
			return mie;
		}

		public MethodInvocationExpression CreateMethodInvocation(Expression target, MethodInfo method)
		{
			return CreateMethodInvocation(target, TypeSystemServices.Map(method));
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
			return CreateMethodInvocation(TypeSystemServices.Map(staticMethod), arg);
		}

		public MethodInvocationExpression CreateMethodInvocation(MethodInfo staticMethod, Expression arg0, Expression arg1)
		{
			return CreateMethodInvocation(TypeSystemServices.Map(staticMethod), arg0, arg1);
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
			return CreateTypeReference(TypeSystemServices.Map(type));
		}

		public TypeReference CreateTypeReference(LexicalInfo li, Type type)
		{
			return CreateTypeReference(li, TypeSystemServices.Map(type));
		}

		public TypeReference CreateTypeReference(LexicalInfo li, IType type)
		{
			TypeReference reference = CreateTypeReference(type);
			reference.LexicalInfo = li;
			return reference;
		}

		public TypeReference CreateTypeReference(IType type)
		{
			return TypeReferenceFactory.TypeReferenceFor(type);
		}

		private ITypeReferenceFactory TypeReferenceFactory
		{
			get { return _typeReferenceFactory.Value; }
		}

		public SuperLiteralExpression CreateSuperReference(IType expressionType)
		{
			return new SuperLiteralExpression { ExpressionType = expressionType };
		}
		
		public SelfLiteralExpression CreateSelfReference(LexicalInfo location, IType expressionType)
		{
			var reference = CreateSelfReference(expressionType);
			reference.LexicalInfo = location;
			return reference;
		}

		public SelfLiteralExpression CreateSelfReference(IType expressionType)
		{
			return new SelfLiteralExpression { ExpressionType = expressionType };
		}

        public SelfLiteralExpression CreateSelfReference(IMethod method, IType expressionType)
        {
            return new SelfLiteralExpression { ExpressionType = expressionType, Entity = method };
        }

        public ReferenceExpression CreateLocalReference(string name, InternalLocal entity)
		{
			return CreateTypedReference(name, entity);
		}

        public ReferenceExpression CreateLocalReference(InternalLocal entity)
        {
            return CreateTypedReference(entity.Name, entity);
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
				//case EntityType.Custom: return CreateTypedReference(entity.Name, (ITypedEntity)entity);
				case EntityType.Property: return CreateReference((IProperty)entity);
            }
            return CreateTypedReference(entity.Name, (ITypedEntity)entity);
        }

		public ReferenceExpression CreateReference(InternalLocal local)
		{
			return CreateLocalReference(local.Name, local);
		}

		public MemberReferenceExpression CreateReference(LexicalInfo li, Field field)
		{
			MemberReferenceExpression e = CreateReference(field);
			e.LexicalInfo = li;
			return e;
		}

		public MemberReferenceExpression CreateMappedReference(LexicalInfo nodeLexicalInfo,
			Field field,
			IType type)
		{
			if (type.GenericInfo != null && type.ConstructedInfo == null)
				type = TypeSystemServices.SelfMapGenericType(type);
			var entity = type.ConstructedInfo != null ?
				(IField)type.ConstructedInfo.Map((IField)field.Entity) :
				(IField)field.Entity;
			return CreateReference(entity);
		}

		public MemberReferenceExpression CreateReference(Field field)
		{
			return CreateReference((IField)field.Entity);
		}

		public MemberReferenceExpression CreateReference(IField field)
		{
			return CreateMemberReference(field);
		}

	    public GenericReferenceExpression CreateGenericReference(ReferenceExpression baseRef,
	        IEnumerable<TypeReference> typeArgs)
	    {
            var gre = new GenericReferenceExpression { Target = baseRef };
            foreach (var arg in typeArgs)
                gre.GenericArguments.Add(arg);
            var entity = My<NameResolutionService>.Instance.ResolveGenericReferenceExpression(gre, gre.Target.Entity);
            gre.Entity = entity;
	        return gre;
;	    }

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
			MemberReferenceExpression reference = MemberReferenceForEntity(target, member);
			reference.ExpressionType = member.Type;
			return reference;
		}

		public MemberReferenceExpression MemberReferenceForEntity(Expression target, IEntity entity)
		{
			MemberReferenceExpression reference = new MemberReferenceExpression(target.LexicalInfo);
			reference.Target = target;
			reference.Name = entity.Name;
            var genType = target.ExpressionType as IConstructedTypeInfo;
            if (genType != null && entity is IMember && !(entity is IGenericMappedMember))
            {
                var gcm = entity as GenericConstructedMethod;
                if (gcm != null)
                    entity = gcm.GenericDefinition;
                entity = genType.Map((IMember)entity);
                if (gcm != null)
                    entity = ((GenericMappedMethod)entity).GenericInfo.ConstructMethod(gcm.GenericArguments);
            }
			reference.Entity = entity;
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
			return new MethodInvocationExpression(target.LexicalInfo)
			       	{
			       		Target = CreateMemberReference(target, entity),
			       		ExpressionType = entity.ReturnType
			       	};
		}

		public ReferenceExpression CreateReference(LexicalInfo info, IType type)
		{
			var expression = CreateReference(type);
			expression.LexicalInfo = info;
			return expression;
		}

		public ReferenceExpression CreateReference(LexicalInfo li, System.Type type)
		{
			return CreateReference(li, TypeSystemServices.Map(type));
		}

	    private MemberReferenceExpression CreateNestedReference(IType type)
	    {
	        var baseType = CreateReference(type.DeclaringEntity);
            return new MemberReferenceExpression(baseType, type.Name) { Entity = type, IsSynthetic = true };
	    }

		public ReferenceExpression CreateReference(IType type)
		{
		    if (type.DeclaringEntity is GenericConstructedType)
		        return CreateNestedReference(type);
			if (type.GenericInfo != null)
				type = type.GenericInfo.ConstructType(type.GenericInfo.GenericParameters);
			return new ReferenceExpression(type.FullName) {Entity = type, ExpressionType = type, IsSynthetic = true};
		}

		public MethodInvocationExpression CreateEvalInvocation(LexicalInfo li)
		{
			return CreateBuiltinInvocation(li, BuiltinFunction.Eval);
		}

		private static MethodInvocationExpression CreateBuiltinInvocation(LexicalInfo li, BuiltinFunction builtin)
		{
			return new MethodInvocationExpression(li) { Target = CreateBuiltinReference(builtin) };
		}

		public static ReferenceExpression CreateBuiltinReference(BuiltinFunction builtin)
		{
			return new ReferenceExpression(builtin.Name) { Entity = builtin };
		}

		public MethodInvocationExpression CreateEvalInvocation(LexicalInfo li, Expression arg, Expression value)
		{
			MethodInvocationExpression eval = CreateEvalInvocation(li);
			eval.Arguments.Add(arg);
			eval.Arguments.Add(value);
			eval.ExpressionType = value.ExpressionType;
			return eval;
		}

        public MethodInvocationExpression CreateEvalInvocation(LexicalInfo li, params Expression[] args)
        {
            MethodInvocationExpression eval = CreateEvalInvocation(li);
            IType et = null;
            foreach (var arg in args)
            {
                eval.Arguments.Add(arg);
                et = arg.ExpressionType;
            }
            eval.ExpressionType = et;
            return eval;
        }

        public MethodInvocationExpression CreateDefaultInvocation(LexicalInfo li, IType type)
	    {
	        var result = CreateBuiltinInvocation(li, BuiltinFunction.Default);
            result.Arguments.Add(CreateTypeofExpression(type));
	        result.ExpressionType = type;
	        return result;
	    }

		public UnpackStatement CreateUnpackStatement(DeclarationCollection declarations, Expression expression)
		{
			UnpackStatement unpack = new UnpackStatement(expression.LexicalInfo);
			unpack.Declarations.AddRange(declarations);
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
			return CreateBoundBinaryExpression(TypeSystemServices.GetExpressionType(lhs), BinaryOperatorType.Assign, lhs, rhs);
		}

		public Expression CreateMethodReference(IMethod method)
		{
			return CreateMemberReference(method);
		}

		public Expression CreateMethodReference(LexicalInfo lexicalInfo, IMethod method)
		{
			var e = CreateMethodReference(method);
			e.LexicalInfo = lexicalInfo;
			return e;
		}

		public BinaryExpression CreateBoundBinaryExpression(IType expressionType, BinaryOperatorType op, Expression lhs, Expression rhs)
		{
			return new BinaryExpression(op, lhs, rhs) { ExpressionType = expressionType, IsSynthetic = true };
		}

		public BoolLiteralExpression CreateBoolLiteral(bool value)
		{
			return new BoolLiteralExpression(value) { ExpressionType = TypeSystemServices.BoolType };
		}

		public StringLiteralExpression CreateStringLiteral(string value)
		{
			return new StringLiteralExpression(value) { ExpressionType = TypeSystemServices.StringType };
		}

		public NullLiteralExpression CreateNullLiteral()
		{
			return new NullLiteralExpression { ExpressionType = Null.Default };
		}

		public ArrayLiteralExpression CreateObjectArray(ExpressionCollection items)
		{
			return CreateArray(TypeSystemServices.ObjectArrayType, items);
		}

		public ArrayLiteralExpression CreateArray(IType arrayType, ExpressionCollection items)
		{
			if (!arrayType.IsArray)
				throw new ArgumentException(string.Format("'{0}'  is not an array type!", arrayType), "arrayType");

			var array = new ArrayLiteralExpression();
			array.ExpressionType = arrayType;
			array.Items.AddRange(items);
			TypeSystemServices.MapToConcreteExpressionTypes(array.Items);
			return array;
		}

		public IntegerLiteralExpression CreateIntegerLiteral(int value)
		{
			return new IntegerLiteralExpression(value) { ExpressionType = TypeSystemServices.IntType };
		}

		public IntegerLiteralExpression CreateIntegerLiteral(long value)
		{
			return new IntegerLiteralExpression(value) { ExpressionType = TypeSystemServices.LongType };
		}

		public SlicingExpression CreateSlicing(Expression target, int begin)
		{
			var arrayType = target.ExpressionType as IArrayType;
			var expressionType = arrayType != null ? arrayType.ElementType : TypeSystemServices.ObjectType;
			return new SlicingExpression(target, CreateIntegerLiteral(begin)) { ExpressionType = expressionType };
		}

		public ReferenceExpression CreateReference(ParameterDeclaration parameter)
		{
			return CreateReference((InternalParameter)TypeSystemServices.GetEntity(parameter));
		}

		public ReferenceExpression CreateReference(InternalParameter parameter)
		{
			return new ReferenceExpression(parameter.Name)
			       	{
			       		Entity = parameter,
			       		ExpressionType = parameter.Type
			       	};
		}

		public UnaryExpression CreateNotExpression(Expression node)
		{
			return new UnaryExpression
			       	{
			       		LexicalInfo = node.LexicalInfo,
			       		Operand = node,
			       		Operator = UnaryOperatorType.LogicalNot,
			       		ExpressionType = TypeSystemServices.BoolType
			       	};
		}

		public ParameterDeclaration CreateParameterDeclaration(int index, string name, IType type, bool byref)
		{
			var modifiers = byref ? ParameterModifiers.Ref : ParameterModifiers.None;
			var parameter = new ParameterDeclaration(name, CreateTypeReference(type), modifiers);
			parameter.Entity = new InternalParameter(parameter, index);
			return parameter;
		}

		public ParameterDeclaration CreateParameterDeclaration(int index, string name, IType type)
		{
			return CreateParameterDeclaration(index, name, type, false);
		}

		//TODO: >=0.9, support constraints here
		public GenericParameterDeclaration CreateGenericParameterDeclaration(int index, string name)
		{
			GenericParameterDeclaration p = new GenericParameterDeclaration(name);
			p.Entity = new InternalGenericParameter(_tss, p, index);
			return p;
		}

		public Constructor CreateConstructor(TypeMemberModifiers modifiers)
		{
			Constructor constructor = new Constructor();
			constructor.Modifiers = modifiers;
			constructor.IsSynthetic = true;
			EnsureEntityFor(constructor);
			return constructor;
		}

		private void EnsureEntityFor(TypeMember member)
		{
			InternalTypeSystemProvider.EntityFor(member);
		}

		public Constructor CreateStaticConstructor(TypeDefinition type)
		{
			var constructor = new Constructor();
			constructor.IsSynthetic = true;
			constructor.Modifiers = TypeMemberModifiers.Private | TypeMemberModifiers.Static;
			EnsureEntityFor(constructor);
			type.Members.Add(constructor);
			return constructor;
		}

        public MethodInvocationExpression CreateGenericConstructorInvocation(IType classType,
	        IEnumerable<GenericParameterDeclaration> genericArgs)
	    {
	        var args = genericArgs.Select(gpd => new SimpleTypeReference(gpd.Name){Entity = gpd.Entity}).Cast<TypeReference>();
            return CreateGenericConstructorInvocation(classType, args);
	    }

	    public MethodInvocationExpression CreateGenericConstructorInvocation(IType classType,
	        IEnumerable<TypeReference> genericArgs)
	    {
            var gpp = classType as IGenericParametersProvider;
            IConstructor constructor;
            if (gpp == null || !genericArgs.Any())
            {
                constructor = classType.GetConstructors().First();
                return CreateConstructorInvocation(constructor);
            }

            classType = new GenericConstructedType(
                classType,
                genericArgs.Select(a => a.Entity).Cast<IType>().ToArray());
            constructor = classType.GetConstructors().First();

	        var result = new MethodInvocationExpression {Target = CreateReference(constructor.DeclaringType)};
	        result.Target.Entity = constructor;
            result.ExpressionType = constructor.DeclaringType;

	        return result;
	    }

		public MethodInvocationExpression CreateConstructorInvocation(ClassDefinition cd)
		{
			IConstructor constructor = ((IType)cd.Entity).GetConstructors().First();
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
			mie.Arguments.AddRange(args);
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
			return CreateSuperConstructorInvocation(DefaultConstructorFor(baseType));
		}

		private IConstructor DefaultConstructorFor(IType baseType)
		{
			IConstructor defaultConstructor = TypeSystemServices.GetDefaultConstructor(baseType);
			if (null == defaultConstructor)
				throw new ArgumentException("No default constructor for type '" + baseType + "'.");
			return defaultConstructor;
		}

		public ExpressionStatement CreateSuperConstructorInvocation(IConstructor defaultConstructor)
		{
			var call = new MethodInvocationExpression(new SuperLiteralExpression());
			call.Target.Entity = defaultConstructor;
			call.ExpressionType = TypeSystemServices.VoidType;
			return new ExpressionStatement(call);
		}

		public MethodInvocationExpression CreateSuperMethodInvocation(IMethod superMethod)
		{
			var mie = new MethodInvocationExpression(CreateMemberReference(CreateSuperReference(superMethod.DeclaringType), superMethod));
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
			return CreateMethod(name, returnType, TypeMemberModifiers.Public|TypeMemberModifiers.Virtual);
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
			method.IsSynthetic = true;
			EnsureEntityFor(method);
			return method;
		}

        public Method CreateGenericMethod(string name, TypeReference returnType, TypeMemberModifiers modifiers, GenericParameterDeclaration[] genParams)
        {
            Method method = new Method(name);
            method.Modifiers = modifiers;
            method.ReturnType = returnType;
            method.IsSynthetic = true;
            method.GenericParameters.AddRange(genParams);
            EnsureEntityFor(method);
            return method;
        }

        public Property CreateProperty(string name, IType type)
		{
			Property property = new Property(name);
			property.Modifiers = TypeMemberModifiers.Public;
			property.Type = CreateTypeReference(type);
			property.IsSynthetic = true;
			EnsureEntityFor(property);
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

		public Method CreateRuntimeMethod(string name, IType returnType, IParameter[] parameters, bool hasParamArray)
		{
			Method method = CreateRuntimeMethod(name, returnType);
			DeclareParameters(method, parameters);
			method.Parameters.HasParamArray = hasParamArray;
			return method;
		}

		public void DeclareParameters(INodeWithParameters method, IParameter[] parameters)
		{
			DeclareParameters(method, parameters, 0);
		}

		public void DeclareParameters(INodeWithParameters method, IParameter[] parameters, int parameterIndexDelta)
		{
			for (int i=0; i < parameters.Length; ++i)
			{
				IParameter p = parameters[i];
				int pIndex = parameterIndexDelta + i;
				method.Parameters.Add(
					CreateParameterDeclaration(pIndex,
						string.IsNullOrEmpty(p.Name) ? "arg"+pIndex : p.Name,
						p.Type,
						p.IsByRef));
			}
		}

		public void DeclareGenericParameters(INodeWithGenericParameters node, IGenericParameter[] parameters)
		{
			DeclareGenericParameters(node, parameters, 0);
		}

		public void DeclareGenericParameters(INodeWithGenericParameters node, IGenericParameter[] parameters, int parameterIndexDelta)
		{
			for (int i=0; i < parameters.Length; ++i)
			{
				var prototype = parameters[i];
				var newParameter = CreateGenericParameterDeclaration(parameterIndexDelta + i, prototype.Name);
				node.GenericParameters.Add(newParameter);
			}
		}

		public Method CreateAbstractMethod(LexicalInfo lexicalInfo, IMethod baseMethod)
		{
			TypeMemberModifiers visibility = VisibilityFrom(baseMethod);
			return CreateMethodFromPrototype(lexicalInfo, baseMethod, visibility | TypeMemberModifiers.Abstract);
		}

		private TypeMemberModifiers VisibilityFrom(IMethod baseMethod)
		{
			if (baseMethod.IsPublic) return TypeMemberModifiers.Public;
			if (baseMethod.IsInternal) return TypeMemberModifiers.Internal;
			return TypeMemberModifiers.Protected;
		}

		public Method CreateMethodFromPrototype(IMethod baseMethod, TypeMemberModifiers newModifiers)
		{
			return CreateMethodFromPrototype(LexicalInfo.Empty, baseMethod, newModifiers);
		}

		public Method CreateMethodFromPrototype(LexicalInfo lexicalInfo, IMethod baseMethod, TypeMemberModifiers newModifiers)
		{
			return CreateMethodFromPrototype(lexicalInfo, baseMethod, newModifiers, baseMethod.Name);
		}

		public Method CreateMethodFromPrototype(LexicalInfo location, IMethod baseMethod, TypeMemberModifiers newModifiers, string newMethodName)
		{
			var method = new Method(location);
			method.Name = newMethodName;
			method.Modifiers = newModifiers;
			method.IsSynthetic = true;

			var optionalTypeMappings = DeclareGenericParametersFromPrototype(method, baseMethod);
			var typeReferenceFactory = optionalTypeMappings != null
			                           	? new MappedTypeReferenceFactory(TypeReferenceFactory, optionalTypeMappings)
			                           	: TypeReferenceFactory;
			_typeReferenceFactory.With(typeReferenceFactory, ()=>
			{
				DeclareParameters(method, baseMethod.GetParameters(), baseMethod.IsStatic ? 0 : 1);
				method.ReturnType = CreateTypeReference(baseMethod.ReturnType);
			});
			EnsureEntityFor(method);
			return method;
		}

		private IDictionary<IType, IType> DeclareGenericParametersFromPrototype(Method method, IMethod baseMethod)
		{	
			var genericMethodInfo = baseMethod.GenericInfo;
			if (genericMethodInfo == null) return null;
			var prototypeParameters = genericMethodInfo.GenericParameters;
			DeclareGenericParameters(method, prototypeParameters);

			var newGenericParameters = method.GenericParameters.ToArray(p => (IGenericParameter)p.Entity);
			return CreateDictionaryMapping(prototypeParameters, newGenericParameters);
		}

		private static IDictionary<IType, IType> CreateDictionaryMapping(IGenericParameter[] from, IGenericParameter[] to)
		{
			var mappings = new Dictionary<IType, IType>(from.Length);
			for (int i=0; i<from.Length; ++i)
				mappings.Add(from[i], to[i]);
			return mappings;
		}

		public Event CreateAbstractEvent(LexicalInfo lexicalInfo, IEvent baseEvent)
		{
			Event ev = new Event(lexicalInfo);
			ev.Name = baseEvent.Name;
			ev.Type = CreateTypeReference(baseEvent.Type);
			ev.Add = CreateAbstractMethod(lexicalInfo, baseEvent.GetAddMethod());
			ev.Remove = CreateAbstractMethod(lexicalInfo, baseEvent.GetRemoveMethod());
			EnsureEntityFor(ev);
			return ev;
		}

		public Expression CreateNotNullTest(Expression target)
		{
			BinaryExpression test = new BinaryExpression(target.LexicalInfo,
											BinaryOperatorType.ReferenceInequality,
											target,
											CreateNullLiteral());
			test.ExpressionType = TypeSystemServices.BoolType;
			return test;
		}

		public RaiseStatement RaiseException(LexicalInfo lexicalInfo, IConstructor exceptionConstructor, params Expression[] args)
		{
			Debug.Assert(TypeSystemServices.IsValidException(exceptionConstructor.DeclaringType));
			return new RaiseStatement(lexicalInfo, CreateConstructorInvocation(lexicalInfo, exceptionConstructor, args));
		}

	    public TryStatement CreateTryExcept(LexicalInfo lexicalInfo, Block protecteBlock,
	        params ExceptionHandler[] handlers)
	    {
	        var result = new TryStatement(lexicalInfo) {ProtectedBlock = protecteBlock};
            result.ExceptionHandlers.AddRange(handlers);
	        return result;
	    }

	    public ExceptionHandler CreateExceptionHandler(LexicalInfo lexicalInfo, Declaration definition, Block body)
	    {
	        return new ExceptionHandler(lexicalInfo){ Declaration = definition, Block = body};
	    }

		public InternalLocal DeclareTempLocal(Method node, IType type)
		{
			var local = DeclareLocal(node, My<UniqueNameProvider>.Instance.GetUniqueName(), type);
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

        public Declaration CreateDeclaration(Method method, string name, IType type, out InternalLocal local)
        {
            var result = new Declaration(name, CreateTypeReference(type));
            local = this.DeclareLocal(method, name, type);
            method.Locals.Add(local.Local);
            result.Entity = local;
            return result;
        }

        public void BindParameterDeclarations(bool isStatic, INodeWithParameters node)
		{
			// arg0 is the this pointer when member is not static
			int delta = isStatic ? 0 : 1;
			ParameterDeclarationCollection parameters = node.Parameters;
			for (int i=0; i<parameters.Count; ++i)
			{
				ParameterDeclaration parameter = parameters[i];
				if (null == parameter.Type)
				{
					if (parameter.IsParamArray)
					{
						parameter.Type = CreateTypeReference(TypeSystemServices.ObjectArrayType);
					}
					else
					{
						parameter.Type = CreateTypeReference(TypeSystemServices.ObjectType);
					}
				}
				parameter.Entity = new InternalParameter(parameter, i + delta);
			}
		}

		public InternalLabel CreateLabel(Node sourceNode, string name)
		{
			return new InternalLabel(new LabelStatement(sourceNode.LexicalInfo, name));
		}

        public InternalLabel CreateLabel(Node sourceNode, string name, int depth)
        {
            var result = CreateLabel(sourceNode, name);
            AstAnnotations.SetTryBlockDepth(result.LabelStatement, depth);
            return result;
        }

        public GotoStatement CreateGoto(LexicalInfo li, InternalLabel target)
	    {
	        return new GotoStatement(li, CreateLabelReference(target.LabelStatement));
	    }

	    public GotoStatement CreateGoto(InternalLabel target)
	    {
	        return CreateGoto(LexicalInfo.Empty, target);
	    }

        public GotoStatement CreateGoto(InternalLabel target, int depth)
        {
            var result = CreateGoto(LexicalInfo.Empty, target);
            AstAnnotations.SetTryBlockDepth(result, depth);
            return result;
        }

        public TypeMember CreateStub(ClassDefinition node, IMember member)
		{
			IMethod baseMethod = member as IMethod;
			if (null != baseMethod)
				return CreateMethodStub(baseMethod);

			IProperty property = member as IProperty;
			if (null != property)
				return CreatePropertyStub(node, property);

			return null;
		}

		Method CreateMethodStub(IMethod baseMethod)
		{
			var stub = CreateMethodFromPrototype(baseMethod, TypeSystemServices.GetAccess(baseMethod) | TypeMemberModifiers.Virtual);

			var notImplementedException = new MethodInvocationExpression
			        	{
			        		Target = new MemberReferenceExpression(new ReferenceExpression("System"), "NotImplementedException")
			        	};
			stub.Body.Statements.Add(new RaiseStatement(notImplementedException) { LexicalInfo = LexicalInfo.Empty });

			return stub;
		}

		Property CreatePropertyStub(ClassDefinition node, IProperty baseProperty)
		{
			//try to complete partial implementation if any
			Property property = node.Members[baseProperty.Name] as Property;
			if (null == property) {
				property = new Property(LexicalInfo.Empty);
				property.Name = baseProperty.Name;
				property.Modifiers = TypeSystemServices.GetAccess(baseProperty) | TypeMemberModifiers.Virtual;
				property.IsSynthetic = true;
				DeclareParameters(property, baseProperty.GetParameters(), baseProperty.IsStatic ? 0 : 1);
				property.Type = CreateTypeReference(baseProperty.Type);
			}

			if (property.Getter == null && null != baseProperty.GetGetMethod())
				property.Getter = CreateMethodStub(baseProperty.GetGetMethod());

			if (property.Setter == null && null != baseProperty.GetSetMethod())
				property.Setter = CreateMethodStub(baseProperty.GetSetMethod());

			EnsureEntityFor(property);
			return property;
		}

		public Constructor GetOrCreateStaticConstructorFor(TypeDefinition type)
		{
			return type.GetStaticConstructor() ?? CreateStaticConstructor(type);
		}
	}
}

