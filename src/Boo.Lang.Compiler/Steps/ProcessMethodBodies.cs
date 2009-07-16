#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
//	   * Redistributions of source code must retain the above copyright notice,
//	   this list of conditions and the following disclaimer.
//	   * Redistributions in binary form must reproduce the above copyright notice,
//	   this list of conditions and the following disclaimer in the documentation
//	   and/or other materials provided with the distribution.
//	   * Neither the name of Rodrigo B. de Oliveira nor the names of its
//	   contributors may be used to endorse or promote products derived from this
//	   software without specific prior written permission.
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Ast.Visitors;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Builders;
using Boo.Lang.Compiler.TypeSystem.Core;
using Boo.Lang.Compiler.TypeSystem.Generics;
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Compiler.TypeSystem.Reflection;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Runtime;
using Attribute = Boo.Lang.Compiler.Ast.Attribute;
using Module=Boo.Lang.Compiler.Ast.Module;

namespace Boo.Lang.Compiler.Steps
{
	/// <summary>
	/// AST semantic evaluation.
	/// </summary>
	public class ProcessMethodBodies : AbstractNamespaceSensitiveVisitorCompilerStep
	{
		static readonly ExpressionCollection EmptyExpressionCollection = new ExpressionCollection();

		static readonly object OptionalReturnStatementAnnotation = new object();

		static readonly object ResolvedAsExtensionAnnotation = new object();

		protected Stack _methodStack;

		protected Stack _memberStack;
		// for accurate error reporting during type inference

		protected Module _currentModule;

		protected InternalMethod _currentMethod;

		protected bool _optimizeNullComparisons = true;

		const string TempInitializerName = "$___temp_initializer";

		override public void Run()
		{
			NameResolutionService.Reset();

			_currentModule = null;
			_currentMethod = null;
			_methodStack = new Stack();
			_memberStack = new Stack();

			InitializeMemberCache();

			Visit(CompileUnit);
		}

		protected CallableResolutionService CallableResolutionService
		{
			get { return Context.GetService<CallableResolutionService>(); }
		}

		protected IMethod ResolveMethod(IType type, string name)
		{
			return NameResolutionService.ResolveMethod(type, name);
		}

		protected IProperty ResolveProperty(IType type, string name)
		{
			return NameResolutionService.ResolveProperty(type, name);
		}

		virtual protected void InitializeMemberCache()
		{
			_methodCache = new Dictionary<string,IMethodBase>();
		}

		override public void Dispose()
		{
			base.Dispose();

			_currentModule = null;
			_currentMethod = null;
			_methodStack = null;
			_memberStack = null;

			_methodCache = null;
		}

		override public void OnModule(Module module)
		{
			if (WasVisited(module)) return;
			MarkVisited(module);
			_currentModule = module;

			EnterNamespace(InternalModule.ScopeFor(module));

			Visit(module.Members);
			Visit(module.AssemblyAttributes);

			LeaveNamespace();
		}

		override public void OnInterfaceDefinition(InterfaceDefinition node)
		{
			if (WasVisited(node)) return;
			MarkVisited(node);

			VisitTypeDefinition(node);
		}

		private void VisitBaseTypes(TypeDefinition node)
		{
			foreach (TypeReference typeRef in node.BaseTypes)
			{
				EnsureRelatedNodeWasVisited(typeRef, typeRef.Entity);
			}
		}

		private void VisitTypeDefinition(TypeDefinition node)
		{
			INamespace ns = (INamespace)GetEntity(node);
			EnterNamespace(ns);
			VisitBaseTypes(node);
			Visit(node.Attributes);
			Visit(node.Members);
			LeaveNamespace();
		}

		override public void OnClassDefinition(ClassDefinition node)
		{
			if (WasVisited(node)) return;
			MarkVisited(node);

			VisitTypeDefinition(node);
			ProcessInitializers(node);
		}

		void ProcessInitializers(ClassDefinition node)
		{
			foreach (TypeMember member in node.Members.ToArray())
			{
				switch (member.NodeType)
				{
					case NodeType.Field:
						ProcessFieldInitializer((Field) member);
						break;
					case NodeType.StatementTypeMember:
						ProcessStatementTypeMemberInitializer(node, ((StatementTypeMember)member));
						break;
				}
			}

			Method initializer = (Method) node["$initializer$"];
			if (null != initializer)
			{
				AddInitializerToInstanceConstructors(node, initializer);
				node.Members.Remove(initializer);
			}
		}

		private void ProcessStatementTypeMemberInitializer(ClassDefinition node, StatementTypeMember statementTypeMember)
		{
			Statement stmt = statementTypeMember.Statement;

			Method initializer = GetInitializerFor(node, node.IsStatic);
			initializer.Body.Add(stmt);

			InternalMethod entity = (InternalMethod) GetEntity(initializer);
			ProcessNodeInMethodContext(entity, entity, stmt);

			node.Members.Remove(statementTypeMember);
		}

		override public void OnAttribute(Attribute node)
		{
			IType tag = node.Entity as IType;
			if (null != tag && !TypeSystemServices.IsError(tag))
			{
				Visit(node.Arguments);
				ResolveNamedArguments(tag, node.NamedArguments);

				IConstructor constructor = GetCorrectConstructor(node, tag, node.Arguments);
				if (null != constructor)
				{
					Bind(node, constructor);
				}
			}
		}

		override public void OnProperty(Property node)
		{
			if (WasVisited(node))
			{
				return;
			}
			MarkVisited(node);

			Method setter = node.Setter;
			Method getter = node.Getter;

			Visit(node.Attributes);
			Visit(node.Type);
			Visit(node.Parameters);

			ResolvePropertyOverride(node);

			if (null != getter)
			{
				if (null != node.Type)
				{
					getter.ReturnType = node.Type.CloneNode();
				}
				getter.Parameters.ExtendWithClones(node.Parameters);

				Visit(getter);
			}

			IType typeInfo = null;
			if (null != node.Type)
			{
				typeInfo = GetType(node.Type);
			}
			else
			{
				if (null != getter)
				{
					typeInfo = GetEntity(node.Getter).ReturnType;
					if (typeInfo == TypeSystemServices.VoidType)
					{
						typeInfo = TypeSystemServices.ObjectType;
						node.Getter.ReturnType = CodeBuilder.CreateTypeReference(getter.LexicalInfo, typeInfo);
					}
				}
				else
				{
					typeInfo = TypeSystemServices.ObjectType;
				}
				node.Type = CodeBuilder.CreateTypeReference(node.LexicalInfo, typeInfo);
			}

			if (null != setter)
			{
				ParameterDeclaration parameter = new ParameterDeclaration();
				parameter.Type = CodeBuilder.CreateTypeReference(typeInfo);
				parameter.Name = "value";
				parameter.Entity = new InternalParameter(parameter, node.Parameters.Count+CodeBuilder.GetFirstParameterIndex(setter));
				setter.Parameters.ExtendWithClones(node.Parameters);
				setter.Parameters.Add(parameter);
				setter.Name = "set_" + node.Name;
				Visit(setter);
			}
		}

		override public void OnStatementTypeMember(StatementTypeMember node)
		{
			// statement type members are later
			// processed as initializers
		}

		override public void OnField(Field node)
		{
			if (WasVisited(node)) return;
			MarkVisited(node);

			InternalField entity = (InternalField)GetEntity(node);

			Visit(node.Attributes);
			Visit(node.Type);

			if (null != node.Initializer)
			{
				IType type = (null != node.Type) ? GetType(node.Type) : null;
				if (null != type && TypeSystemServices.IsNullable(type))
				{
					BindNullableInitializer(node, node.Initializer, type);
				}
				if (entity.DeclaringType.IsValueType && !node.IsStatic)
				{
					Error(
						CompilerErrorFactory.ValueTypeFieldsCannotHaveInitializers(
							node.Initializer));
				}
				try
				{
					PushMember(node);
					PreProcessFieldInitializer(node);
				}
				finally
				{
					PopMember();
				}
			}
			else
			{
				if (null == node.Type)
				{
					node.Type = CreateTypeReference(node.LexicalInfo, TypeSystemServices.ObjectType);
				}
			}
			CheckFieldType(node.Type);
		}

		bool IsValidLiteralInitializer(Expression e)
		{
			switch (e.NodeType)
			{
				case NodeType.BoolLiteralExpression:
				case NodeType.IntegerLiteralExpression:
				case NodeType.DoubleLiteralExpression:
				case NodeType.NullLiteralExpression:
				case NodeType.StringLiteralExpression:
					{
						return true;
					}
			}
			return false;
		}

		void ProcessLiteralField(Field node)
		{
			Visit(node.Initializer);
			ProcessFieldInitializerType(node, node.Initializer.ExpressionType);
			((InternalField)node.Entity).StaticValue = node.Initializer;
			node.Initializer = null;
		}

		void ProcessFieldInitializerType(Field node, IType initializerType)
		{
			if (null == node.Type)
			{
				node.Type = CreateTypeReference(node.LexicalInfo, MapNullToObject(initializerType));
			}
			else
			{
				AssertTypeCompatibility(node.Initializer, GetType(node.Type), initializerType);
			}
		}

		private TypeReference CreateTypeReference(LexicalInfo info, IType type)
		{
			TypeReference reference = CodeBuilder.CreateTypeReference(type);
			reference.LexicalInfo = info;
			return reference;
		}

		private void PreProcessFieldInitializer(Field node)
		{
			Expression initializer = node.Initializer;
			if (node.IsFinal && node.IsStatic)
			{
				if (IsValidLiteralInitializer(initializer))
				{
					ProcessLiteralField(node);
					return;
				}
			}

			BlockExpression closure = node.Initializer as BlockExpression;
			if (closure != null)
			{
				InferClosureSignature(closure);
			}

			Method method = GetInitializerMethod(node);
			InternalMethod entity = (InternalMethod)method.Entity;

			ReferenceExpression temp = new ReferenceExpression(TempInitializerName);

			BinaryExpression assignment = new BinaryExpression(
				node.LexicalInfo,
				BinaryOperatorType.Assign,
				temp,
				initializer);

			ProcessNodeInMethodContext(entity, entity, assignment);
			method.Locals.RemoveByEntity(temp.Entity);

			IType initializerType = GetExpressionType(assignment.Right);
			ProcessFieldInitializerType(node, initializerType);
			node.Initializer = assignment.Right;
		}

		void ProcessFieldInitializer(Field node)
		{
			Expression initializer = node.Initializer;
			if (null == initializer) return;

			//do not unnecessarily assign fields to default values
			switch (initializer.NodeType)
			{
				case NodeType.NullLiteralExpression:
					node.Initializer = null;
					return;
				case NodeType.IntegerLiteralExpression:
					if (0 == ((IntegerLiteralExpression) initializer).Value) {
						node.Initializer = null;
						return;
					}
					break;
				case NodeType.BoolLiteralExpression:
					if (false == ((BoolLiteralExpression) initializer).Value) {
						node.Initializer = null;
						return;
					}
					break;
				case NodeType.DoubleLiteralExpression:
					if (0.0f == ((DoubleLiteralExpression) initializer).Value) {
						node.Initializer = null;
						return;
					}
					break;
			}

			Method method = GetInitializerMethod(node);
			method.Body.Add(
				CodeBuilder.CreateAssignment(
					initializer.LexicalInfo,
					CodeBuilder.CreateReference(node),
					initializer));
			node.Initializer = null;
		}

		Method CreateInitializerMethod(TypeDefinition type, string name, TypeMemberModifiers modifiers)
		{
			Method method = CodeBuilder.CreateMethod(name, TypeSystemServices.VoidType, modifiers);
			type.Members.Add(method);
			MarkVisited(method);
			return method;
		}

		private Field GetFieldsInitializerInitializedField(TypeDefinition type)
		{
			string name = AstUtil.BuildUniqueTypeMemberName(type, "initialized");
			Field field= (Field) type.Members[name];

			if (null == field)
			{
				field = CodeBuilder.CreateField(name, TypeSystemServices.BoolType);
				field.Visibility = TypeMemberModifiers.Private;
				type.Members.Add(field);
				MarkVisited(field);
			}
			return field;
		}

		Method GetInitializerMethod(Field node)
		{
			return GetInitializerFor(node.DeclaringType, node.IsStatic);
		}

		private Method GetInitializerFor(TypeDefinition type, bool isStatic)
		{
			string methodName = isStatic ? "$static_initializer$" : "$initializer$";
			Method method = (Method)type[methodName];
			if (null == method)
			{
				if (isStatic)
				{
					if (!type.HasStaticConstructor)
					{
						// when the class doesnt have a static constructor
						// yet, create one and use it as the static
						// field initializer method
						method = CodeBuilder.CreateStaticConstructor(type);
					}
					else
					{
						method = CreateInitializerMethod(type, methodName, TypeMemberModifiers.Static);
						AddInitializerToStaticConstructor(type, (InternalMethod)method.Entity);
					}
				}
				else
				{
					method = CreateInitializerMethod(type, methodName, TypeMemberModifiers.None);
				}
				type[methodName] = method;
			}
			return method;
		}

		void AddInitializerToStaticConstructor(TypeDefinition type, InternalMethod initializer)
		{
			GetStaticConstructor(type).Body.Insert(0,
												   CodeBuilder.CreateMethodInvocation(initializer));
		}

		void AddInitializerToInstanceConstructors(TypeDefinition type, Method initializer)
		{
			int n = 0;

			//count number of non-static constructors
			foreach (TypeMember node in type.Members)
			{
				if (NodeType.Constructor == node.NodeType && !node.IsStatic)
					n++;
			}

			//if there is more than one we need $initialized$ guard check
			if (n > 1)
				AddInitializedGuardToInitializer(type, initializer);

			foreach (TypeMember node in type.Members)
			{
				if (NodeType.Constructor == node.NodeType && !node.IsStatic)
				{
					Constructor constructor = (Constructor) node;
					n = GetIndexAfterSuperInvocation(constructor.Body);
					foreach (Statement st in initializer.Body.Statements)
					{
						constructor.Body.Insert(n, (Statement) st.Clone());
						n++;
					}
					foreach (Local loc in initializer.Locals)
					{
						constructor.Locals.Add(loc);
					}
				}
			}
		}

		void AddInitializedGuardToInitializer(TypeDefinition type, Method initializer)
		{
			Field field = GetFieldsInitializerInitializedField(type);

			//run initializer code only if $initialized$ is false
			//hmm quasi-notation would be lovely here
			Block trueBlock = new Block();
			trueBlock.Add(new GotoStatement(LexicalInfo.Empty, new ReferenceExpression("___initialized___")));
			IfStatement cond = new IfStatement(CodeBuilder.CreateReference(field),
				trueBlock, null);
			initializer.Body.Insert(0, cond);

			//set $initialized$ field to true
			initializer.Body.Add(
				CodeBuilder.CreateFieldAssignment(field, new BoolLiteralExpression(true)));

			//label we're past the initializer
			initializer.Body.Add(
				new LabelStatement(LexicalInfo.Empty, "___initialized___"));
		}

		int GetIndexAfterSuperInvocation(Block body)
		{
			int index = 0;
			foreach (Statement s in body.Statements)
			{
				if (NodeType.ExpressionStatement == s.NodeType)
				{
					Expression expression = ((ExpressionStatement)s).Expression;
					if (NodeType.MethodInvocationExpression == expression.NodeType)
					{
						if (NodeType.SuperLiteralExpression == ((MethodInvocationExpression)expression).Target.NodeType)
						{
							return index + 1;
						}
					}
				}
				++index;
			}
			return 0;
		}

		Constructor GetStaticConstructor(TypeDefinition type)
		{
			Constructor constructor = type.GetStaticConstructor();
			if (null == constructor)
			{
				constructor = CodeBuilder.CreateStaticConstructor(type);
			}
			return constructor;
		}

		void AddFieldInitializerToStaticConstructor(int index, Field node)
		{
			Constructor constructor = GetStaticConstructor(node.DeclaringType);
			Statement stmt = CodeBuilder.CreateFieldAssignment(node, node.Initializer);
			constructor.Body.Statements.Insert(index, stmt);
			node.Initializer = null;
		}

		void CheckRuntimeMethod(Method method)
		{
			if (!method.Body.IsEmpty)
			{
				Error(CompilerErrorFactory.RuntimeMethodBodyMustBeEmpty(method, method.FullName));
			}
		}

		//ECMA-335 Partition III Section 1.8.1.4
		//cannot call an instance method before super/self.
		void CheckInstanceMethodInvocationsWithinConstructor(Constructor ctor)
		{
			if (ctor.Body.IsEmpty)
				return;

			foreach (Statement st in ctor.Body.Statements)
			{
				ExpressionStatement est = st as ExpressionStatement;
				if (null == est) continue;

				MethodInvocationExpression mie = est.Expression as MethodInvocationExpression;
				if (null == mie) continue;

				if (mie.Target is SelfLiteralExpression
					|| mie.Target is SuperLiteralExpression)
					break;//okay we're done checking

				if (mie.Target is MemberReferenceExpression)
				{
					MemberReferenceExpression mre = (MemberReferenceExpression) mie.Target;
					if (mre.Target is SelfLiteralExpression
						|| mre.Target is SuperLiteralExpression)
					{
						Error(CompilerErrorFactory.InstanceMethodInvocationBeforeInitialization(ctor, mre));
					}
				}
			}
		}

		override public void OnConstructor(Constructor node)
		{
			if (WasVisited(node))
			{
				return;
			}
			MarkVisited(node);

			Visit(node.Attributes);
			Visit(node.Parameters);

			InternalConstructor entity = (InternalConstructor)node.Entity;
			ProcessMethodBody(entity);

			if (node.IsRuntime)
			{
				CheckRuntimeMethod(node);
			}
			else
			{
				if (entity.DeclaringType.IsValueType)
				{
					if (0 == node.Parameters.Count
						&& !node.IsStatic
						&& !node.IsSynthetic)
					{
						Error(CompilerErrorFactory.ValueTypesCannotDeclareParameterlessConstructors(node));
					}
				}
				else if (
					!entity.HasSelfCall &&
					!entity.HasSuperCall &&
					!entity.IsStatic)
				{
					IType baseType = entity.DeclaringType.BaseType;
					IConstructor super = GetCorrectConstructor(node, baseType, EmptyExpressionCollection);
					if (null != super)
					{
						node.Body.Statements.Insert(0,
													CodeBuilder.CreateSuperConstructorInvocation(super));
					}
				}
				if (!entity.IsStatic)
					CheckInstanceMethodInvocationsWithinConstructor(node);
			}
		}

		override public void LeaveParameterDeclaration(ParameterDeclaration node)
		{
			AssertIdentifierName(node, node.Name);
			CheckParameterType(node.Type);
		}

		void CheckParameterType(TypeReference type)
		{
			if (type.Entity != TypeSystemServices.VoidType) return;
			Error(CompilerErrorFactory.InvalidParameterType(type, type.Entity.ToString()));
		}

		void CheckFieldType(TypeReference type)
		{
			if (type.Entity != TypeSystemServices.VoidType) return;
			Error(CompilerErrorFactory.InvalidFieldType(type, type.Entity.ToString()));
		}

		void CheckDeclarationType(TypeReference type)
		{
			if (type.Entity != TypeSystemServices.VoidType) return;
			Error(CompilerErrorFactory.InvalidDeclarationType(type, type.Entity.ToString()));
		}

		override public void OnBlockExpression(BlockExpression node)
		{
			if (WasVisited(node)) return;
			if (ShouldDeferClosureProcessing(node)) return;

			InferClosureSignature(node);
			ProcessClosureBody(node);
		}

		private void InferClosureSignature(BlockExpression node)
		{
			ClosureSignatureInferrer inferrer = new ClosureSignatureInferrer(node);
			ICallableType inferredCallableType = inferrer.InferCallableType();
			BindExpressionType(node, inferredCallableType);
			AddInferredClosureParameterTypes(node, inferredCallableType);
		}

		private bool ShouldDeferClosureProcessing(BlockExpression node)
		{
			// Defer closure processing if it's an argument in a generic method invocation
			MethodInvocationExpression methodInvocationContext = node.ParentNode as MethodInvocationExpression;
			if (methodInvocationContext == null) return false;
			if (!methodInvocationContext.Arguments.Contains(node)) return false;

			IMethod target = methodInvocationContext.Target.Entity as IMethod;
			return (target != null && GenericsServices.IsGenericMethod(target));
		}

		private void AddInferredClosureParameterTypes(BlockExpression node, ICallableType callableType)
		{
			IParameter[] parameters = (callableType == null ? null : callableType.GetSignature().Parameters);
			for (int i = 0; i < node.Parameters.Count; i++)
			{
				ParameterDeclaration pd = node.Parameters[i];
				if (pd.Type != null) continue;

				IType inferredType;
				if (parameters != null && i < parameters.Length)
				{
					inferredType = parameters[i].Type;
				}
				else if (node.Parameters.VariableNumber && i == node.Parameters.Count - 1)
				{
					inferredType = TypeSystemServices.ObjectArrayType;
				}
				else
				{
					inferredType = TypeSystemServices.ObjectType;
				}

				pd.Type = CodeBuilder.CreateTypeReference(inferredType);
			}
		}

		void ProcessClosureBody(BlockExpression node)
		{
			MarkVisited(node);

			string explicitClosureName = node["ClosureName"] as string;

			Method closure = CodeBuilder.CreateMethod(
				ClosureName(explicitClosureName),
				node.ReturnType ?? CodeBuilder.CreateTypeReference(Unknown.Default),
				ClosureModifiers());

			MarkVisited(closure);

			InternalMethod closureEntity = (InternalMethod)closure.Entity;
			closure.LexicalInfo = node.LexicalInfo;
			closure.Parameters = node.Parameters;
			closure.Body = node.Body;

			_currentMethod.Method.DeclaringType.Members.Add(closure);

			CodeBuilder.BindParameterDeclarations(_currentMethod.IsStatic, closure);

			// check for invalid names and
			// resolve parameter types
			Visit(closure.Parameters);

			if (node.ContainsAnnotation("inline"))
			{
				AddOptionalReturnStatement(node.Body);
			}

			// Inside the closure, connect the closure method namespace with the current namespace
			NamespaceDelegator ns = new NamespaceDelegator(CurrentNamespace, closureEntity);

			// Allow closure body to reference itself using its explicit name (BOO-1085)
			if (explicitClosureName != null)
			{
				ns.DelegateTo(new AliasedNamespace(explicitClosureName, closureEntity));
			}

			ProcessMethodBody(closureEntity, ns);

			if (closureEntity.ReturnType is Unknown)
			{
				TryToResolveReturnType(closureEntity);
			}

			node.ExpressionType = closureEntity.Type;
			node.Entity = closureEntity;
		}

		private void ProcessClosureInMethodInvocation(GenericParameterInferrer inferrer, BlockExpression closure, ICallableType formalType)
		{
			CallableSignature sig = formalType.GetSignature();

			TypeReplacer replacer = new TypeReplacer();
			TypeCollector collector = new TypeCollector(delegate(IType t)
			{
				IGenericParameter gp = t as IGenericParameter;
				if (gp == null) return false;
				return gp.DeclaringEntity == inferrer.GenericMethod;
			});

			collector.Visit(formalType);
			foreach (IType typeParameter in collector.Matches)
			{
				IType inferredType = inferrer.GetInferredType((IGenericParameter)typeParameter);
				if (inferredType != null)
				{
					replacer.Replace(typeParameter, inferredType);
				}
			}

			for (int i = 0; i < sig.Parameters.Length; i++)
			{
				ParameterDeclaration pd = closure.Parameters[i];
				if (pd.Type != null) continue;
				pd.Type = CodeBuilder.CreateTypeReference(replacer.MapType(sig.Parameters[i].Type));
			}

			ProcessClosureBody(closure);
		}

		private TypeMemberModifiers ClosureModifiers()
		{
			TypeMemberModifiers modifiers = TypeMemberModifiers.Internal;
			if (_currentMethod.IsStatic)
			{
				modifiers |= TypeMemberModifiers.Static;
			}
			return modifiers;
		}

		private string ClosureName(string explicitName)
		{
			string closureHint = explicitName ?? "closure";
			return Context.GetUniqueName(_currentMethod.Name, closureHint);
		}

		private void AddOptionalReturnStatement(Block body)
		{
			if (body.Statements.Count != 1) return;
			ExpressionStatement stmt = body.FirstStatement as ExpressionStatement;
			if (null == stmt) return;

			ReturnStatement rs = new ReturnStatement(stmt.LexicalInfo, stmt.Expression, null);
			rs.Annotate(OptionalReturnStatementAnnotation);
			body.Replace(stmt, rs);
		}

		override public void OnMethod(Method method)
		{
			if (WasVisited(method)) return;
			MarkVisited(method);

			Visit(method.Attributes);
			Visit(method.Parameters);
			Visit(method.ReturnType);
			Visit(method.ReturnTypeAttributes);

			bool ispinvoke = GetEntity(method).IsPInvoke;
			if (method.IsRuntime || ispinvoke)
			{
				CheckRuntimeMethod(method);
				if (ispinvoke)
				{
					method.Modifiers |= TypeMemberModifiers.Static;
				}
			}
			else
			{
				try
				{
					PushMember(method);
					ProcessRegularMethod(method);
				}
				finally
				{
					PopMember();
				}
			}
		}

		void CheckIfIsMethodOverride(InternalMethod entity)
		{
			if (entity.IsStatic) return;

			IMethod overriden = FindMethodOverride(entity);
			if (null == overriden) return;

			ProcessMethodOverride(entity, overriden);
		}

		IMethod FindPropertyAccessorOverride(Property property, Method accessor)
		{
			IProperty baseProperty = ((InternalProperty)property.Entity).Override;
			if (null == baseProperty) return null;

			IMethod baseMethod = null;
			if (property.Getter == accessor)
			{
				baseMethod = baseProperty.GetGetMethod();
			}
			else
			{
				baseMethod = baseProperty.GetSetMethod();
			}

			if (null != baseMethod)
			{
				IMethod accessorEntity = (IMethod)accessor.Entity;
				if (TypeSystemServices.CheckOverrideSignature(accessorEntity, baseMethod))
				{
					return baseMethod;
				}
			}

			return null;
		}

		IMethod FindMethodOverride(InternalMethod entity)
		{
			Method method = entity.Method;
			if (NodeType.Property == method.ParentNode.NodeType)
			{
				return FindPropertyAccessorOverride((Property)method.ParentNode, method);
			}

			IType baseType = entity.DeclaringType.BaseType;
			IEntity candidates = NameResolutionService.Resolve(baseType, entity.Name, EntityType.Method);
			if (null == candidates)
			{
				return null;
			}

			IMethod baseMethod = FindMethodOverride(entity, candidates);
			if (null != baseMethod)
			{
				EnsureRelatedNodeWasVisited(method, baseMethod);
			}
			return baseMethod;
		}

		private static IMethod FindMethodOverride(InternalMethod entity, IEntity candidates)
		{
			if (EntityType.Method == candidates.EntityType)
			{
				IMethod candidate = (IMethod)candidates;
				if (TypeSystemServices.CheckOverrideSignature(entity, candidate))
				{
					return candidate;
				}
			}

			if (EntityType.Ambiguous == candidates.EntityType)
			{
				IEntity[] entities = ((Ambiguous)candidates).Entities;
				foreach (IMethod candidate in entities)
				{
					if (TypeSystemServices.CheckOverrideSignature(entity, candidate))
					{
						return candidate;
					}
				}
			}

			return null;
		}

		void ResolveMethodOverride(InternalMethod entity)
		{
			IMethod baseMethod = FindMethodOverride(entity);
			if (null == baseMethod)
			{
				string suggestion = NameResolutionService.GetMostSimilarMemberName(
						entity.DeclaringType.BaseType, entity.Name, EntityType.Method);
				if (suggestion == entity.Name) //same name => incompatible signature
					Error(CompilerErrorFactory.NoMethodToOverride(entity.Method, entity.ToString(), true));
				else //suggestion (or null)
					Error(CompilerErrorFactory.NoMethodToOverride(entity.Method, entity.ToString(), suggestion));
			}
			else
			{
				if (!baseMethod.IsVirtual)
				{
					CantOverrideNonVirtual(entity.Method, baseMethod);
				}
				else
				{
					ProcessMethodOverride(entity, baseMethod);
				}
			}
		}

		void ProcessMethodOverride(InternalMethod entity, IMethod baseMethod)
		{
			CallableSignature baseSignature = TypeSystemServices.GetOverriddenSignature(baseMethod, entity);

			if (TypeSystemServices.IsUnknown(entity.ReturnType))
			{
				entity.Method.ReturnType = CodeBuilder.CreateTypeReference(entity.Method.LexicalInfo, baseSignature.ReturnType);
			}
			else if (baseSignature.ReturnType != entity.ReturnType)
			{
				Error(CompilerErrorFactory.InvalidOverrideReturnType(
					entity.Method.ReturnType,
					baseMethod.FullName,
					baseMethod.ReturnType.ToString(),
					entity.ReturnType.ToString()));
			}
			SetOverride(entity, baseMethod);
		}

		void CantOverrideNonVirtual(Method method, IMethod baseMethod)
		{
			Error(CompilerErrorFactory.CantOverrideNonVirtual(method, baseMethod.ToString()));
		}

		void SetPropertyAccessorOverride(Method accessor)
		{
			if (null != accessor)
			{
				accessor.Modifiers |= TypeMemberModifiers.Override;
			}
		}

		IProperty ResolvePropertyOverride(IProperty p, IEntity[] candidates)
		{
			foreach (IEntity candidate in candidates)
			{
				if (EntityType.Property != candidate.EntityType) continue;
				IProperty candidateProperty = (IProperty)candidate;
				if (CheckOverrideSignature(p, candidateProperty))
				{
					return candidateProperty;
				}
			}
			return null;
		}

		bool CheckOverrideSignature(IProperty p, IProperty candidate)
		{
			return TypeSystemServices.CheckOverrideSignature(p.GetParameters(), candidate.GetParameters());
		}

		void ResolvePropertyOverride(Property property)
		{
			InternalProperty entity = (InternalProperty)property.Entity;
			IType baseType = entity.DeclaringType.BaseType;
			IEntity baseProperties = NameResolutionService.Resolve(baseType, property.Name, EntityType.Property);
			if (null != baseProperties)
			{
				if (EntityType.Property == baseProperties.EntityType)
				{
					IProperty candidate = (IProperty)baseProperties;
					if (CheckOverrideSignature(entity, candidate))
					{
						entity.Override = candidate;
					}
				}
				else if (EntityType.Ambiguous == baseProperties.EntityType)
				{
					entity.Override = ResolvePropertyOverride(entity, ((Ambiguous)baseProperties).Entities);
				}
			}

			if (null != entity.Override)
			{
				EnsureRelatedNodeWasVisited(property, entity.Override);
				if (property.IsOverride)
				{
					SetPropertyAccessorOverride(property.Getter);
					SetPropertyAccessorOverride(property.Setter);
				}
				else
				{
					if (null != entity.Override.GetGetMethod())
					{
						SetPropertyAccessorOverride(property.Getter);
					}
					if (null != entity.Override.GetSetMethod())
					{
						SetPropertyAccessorOverride(property.Setter);
					}
				}

				if (null == property.Type)
				{
					property.Type = CodeBuilder.CreateTypeReference(entity.Override.Type);
				}
			}
		}

		void SetOverride(InternalMethod entity, IMethod baseMethod)
		{
			TraceOverride(entity.Method, baseMethod);

			entity.Overriden = baseMethod;
			entity.Method.Modifiers |= TypeMemberModifiers.Override;
		}


		void TraceOverride(Method method, IMethod baseMethod)
		{
			_context.TraceInfo("{0}: Method '{1}' overrides '{2}'", method.LexicalInfo, method.Name, baseMethod);
		}

		sealed class ReturnExpressionFinder : DepthFirstVisitor
		{
			bool _hasReturnStatements = false;

			bool _hasYieldStatements = false;

			public ReturnExpressionFinder(Method node)
			{
				Visit(node.Body);
			}

			public bool HasReturnStatements
			{
				get
				{
					return _hasReturnStatements;
				}
			}

			public bool HasYieldStatements
			{
				get
				{
					return _hasYieldStatements;
				}
			}

			public override void OnReturnStatement(ReturnStatement node)
			{
				_hasReturnStatements |= (null != node.Expression);
			}

			public override void OnYieldStatement(YieldStatement node)
			{
				_hasYieldStatements = true;
			}
		}

		bool DontHaveReturnExpressionsNorYield(Method node)
		{
			ReturnExpressionFinder finder = new ReturnExpressionFinder(node);
			return !(finder.HasReturnStatements || finder.HasYieldStatements);
		}

		void PreProcessMethod(Method node)
		{
			if (WasAlreadyPreProcessed(node)) return;
			MarkPreProcessed(node);

			InternalMethod entity = (InternalMethod)GetEntity(node);
			if (node.IsOverride)
			{
				ResolveMethodOverride(entity);
			}
			else
			{
				CheckIfIsMethodOverride(entity);
				if (TypeSystemServices.IsUnknown(entity.ReturnType))
				{
					if (DontHaveReturnExpressionsNorYield(node))
					{
						node.ReturnType = CodeBuilder.CreateTypeReference(node.LexicalInfo, TypeSystemServices.VoidType);
					}
				}
			}
		}

		static readonly object PreProcessedKey = new object();

		private bool WasAlreadyPreProcessed(Method node)
		{
			return node.ContainsAnnotation(PreProcessedKey);
		}

		private void MarkPreProcessed(Method node)
		{
			node[PreProcessedKey] = PreProcessedKey;
		}

		void ProcessRegularMethod(Method node)
		{
			PreProcessMethod(node);

			InternalMethod entity = (InternalMethod) GetEntity(node);
			ProcessMethodBody(entity);

			PostProcessMethod(node);
		}

		private void PostProcessMethod(Method node)
		{
			bool parentIsClass = node.DeclaringType.NodeType == NodeType.ClassDefinition;
			if (!parentIsClass) return;

			InternalMethod entity = (InternalMethod)GetEntity(node);
			if (TypeSystemServices.IsUnknown(entity.ReturnType))
			{
				TryToResolveReturnType(entity);
			}
			else
			{
				if (entity.IsGenerator)
				{
					CheckGeneratorReturnType(node, entity.ReturnType);
					CheckGeneratorYieldType(entity, entity.ReturnType);
				}
			}
			CheckGeneratorCantReturnValues(entity);
		}

		void CheckGeneratorCantReturnValues(InternalMethod entity)
		{
			if (!entity.IsGenerator) return;
			if (null == entity.ReturnExpressions) return;

			foreach (Expression e in entity.ReturnExpressions)
			{
				Error(CompilerErrorFactory.GeneratorCantReturnValue(e));
			}
		}

		void CheckGeneratorReturnType(Method method, IType returnType)
		{
			bool validReturnType =
				(TypeSystemServices.IEnumerableType == returnType ||
				 TypeSystemServices.IEnumeratorType == returnType ||
				 TypeSystemServices.IsSystemObject(returnType) ||
				 CheckGenericGeneratorReturnType(returnType));

			if (!validReturnType)
			{
				Error(CompilerErrorFactory.InvalidGeneratorReturnType(method.ReturnType));
			}
		}

		bool CheckGenericGeneratorReturnType(IType returnType)
		{
			return returnType.ConstructedInfo != null &&
				(returnType.ConstructedInfo.GenericDefinition == TypeSystemServices.IEnumerableGenericType ||
				 returnType.ConstructedInfo.GenericDefinition == TypeSystemServices.IEnumeratorGenericType);
		}

		void CheckGeneratorYieldType(InternalMethod method, IType returnType)
		{
			if (CheckGenericGeneratorReturnType(returnType))
			{
				IType returnElementType = returnType.ConstructedInfo.GenericArguments[0];

				foreach (Expression yieldExpression in method.YieldExpressions)
				{
					IType yieldType = yieldExpression.ExpressionType;
					if (!returnElementType.IsAssignableFrom(yieldType) &&
						!TypeSystemServices.CanBeReachedByDownCastOrPromotion(returnElementType, yieldType))
					{
						Error(CompilerErrorFactory.YieldTypeDoesNotMatchReturnType(
							yieldExpression, yieldType.ToString(), returnElementType.ToString()));
					}
				}
			}
		}

		void ProcessMethodBody(InternalMethod entity)
		{
			ProcessMethodBody(entity, entity);
		}

		void ProcessMethodBody(InternalMethod entity, INamespace ns)
		{
			ProcessNodeInMethodContext(entity, ns, entity.Method.Body);
			if (entity.IsGenerator) CreateGeneratorSkeleton(entity);
		}

		void ProcessNodeInMethodContext(InternalMethod entity, INamespace ns, Node node)
		{
			PushMethodInfo(entity);
			EnterNamespace(ns);
			try
			{
				Visit(node);
			}
			finally
			{
				LeaveNamespace();
				PopMethodInfo();
			}
		}

		void ResolveGeneratorReturnType(InternalMethod entity)
		{
			IType returnType = GetGeneratorReturnType(entity);
			entity.Method.ReturnType = CodeBuilder.CreateTypeReference(returnType);
		}

		private IType GeneratorItemTypeFor(InternalMethod entity)
		{
			return (IType)entity.Method["GeneratorItemType"];
		}

		/// <summary>
		/// Allows a different language to use custom rules for generator
		/// return types.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		protected virtual IType GetGeneratorReturnType(InternalMethod entity)
		{
			// Make method return a generic IEnumerable
			IType itemType = GeneratorItemTypeFor(entity);
			if (TypeSystemServices.VoidType == itemType)
			{
				// circunvent exception in MakeGenericType
				return TypeSystemServices.ErrorEntity;
			}
			IType enumerableType = TypeSystemServices.IEnumerableGenericType;
			return enumerableType.GenericInfo.ConstructType(itemType);
		}

		void TryToResolveReturnType(InternalMethod entity)
		{
			if (entity.IsGenerator)
			{
				ResolveGeneratorReturnType(entity);
			}
			else
			{
				if (CanResolveReturnType(entity))
				{
					ResolveReturnType(entity);
				}
			}
		}

		override public void OnSuperLiteralExpression(SuperLiteralExpression node)
		{
			if (!AstUtil.IsTargetOfMethodInvocation(node))
			{
			node.ExpressionType = _currentMethod.DeclaringType.BaseType;
				return;
			}

			if (EntityType.Constructor == _currentMethod.EntityType)
			{
				// TODO: point to super ctor
				node.Entity = _currentMethod;
				return;
			}

			if (null == _currentMethod.Overriden)
			{
				Error(node,
				CompilerErrorFactory.MethodIsNotOverride(node, _currentMethod.ToString()));
				return;
		}

			node.Entity = _currentMethod;
		}

		bool CanResolveReturnType(InternalMethod tag)
		{
			ExpressionCollection expressions = tag.ReturnExpressions;
			if (null != expressions)
			{
				foreach (Expression expression in expressions)
				{
					IType type = expression.ExpressionType;
					if (null == type || TypeSystemServices.IsUnknown(type))
					{
						return false;
					}
				}
			}
			return true;
		}

		TypeReference GetMostGenericTypeReference(ExpressionCollection expressions)
		{
			IType type = MapNullToObject(GetMostGenericType(expressions));
			return CodeBuilder.CreateTypeReference(type);
		}

		void ResolveReturnType(InternalMethod entity)
		{
			Method method = entity.Method;
			if (null == entity.ReturnExpressions)
			{
				method.ReturnType = CodeBuilder.CreateTypeReference(TypeSystemServices.VoidType);
			}
			else
			{
				method.ReturnType = GetMostGenericTypeReference(entity.ReturnExpressions);
			}
			TraceReturnType(method, entity);
		}

		IType MapNullToObject(IType type)
		{
			if (Null.Default == type)
			{
				return TypeSystemServices.ObjectType;
			}
			return type;
		}

		IType GetMostGenericType(IType lhs, IType rhs)
		{
			return TypeSystemServices.GetMostGenericType(lhs, rhs);
		}

		IType GetMostGenericType(ExpressionCollection args)
		{
			IType type = GetConcreteExpressionType(args[0]);
			for (int i=1; i<args.Count; ++i)
			{
				IType newType = GetConcreteExpressionType(args[i]);

				if (type == newType)
				{
					continue;
				}

				type = GetMostGenericType(type, newType);
				if (TypeSystemServices.IsSystemObject(type))
				{
					break;
				}
			}
			return type;
		}

		override public void OnBoolLiteralExpression(BoolLiteralExpression node)
		{
			BindExpressionType(node, TypeSystemServices.BoolType);
		}

		override public void OnTimeSpanLiteralExpression(TimeSpanLiteralExpression node)
		{
			BindExpressionType(node, TypeSystemServices.TimeSpanType);
		}

		override public void OnIntegerLiteralExpression(IntegerLiteralExpression node)
		{
			if (node.IsLong)
			{
				BindExpressionType(node, TypeSystemServices.LongType);
			}
			else
			{
				BindExpressionType(node, TypeSystemServices.IntType);
			}
		}

		override public void OnDoubleLiteralExpression(DoubleLiteralExpression node)
		{
			BindExpressionType(node, node.IsSingle ? TypeSystemServices.SingleType : TypeSystemServices.DoubleType);
		}

		override public void OnStringLiteralExpression(StringLiteralExpression node)
		{
			BindExpressionType(node, TypeSystemServices.StringType);
		}

		override public void OnCharLiteralExpression(CharLiteralExpression node)
		{
			string value = node.Value;
			if (null == value || 1 != value.Length)
			{
				Errors.Add(CompilerErrorFactory.InvalidCharLiteral(node, value));
			}
			BindExpressionType(node, TypeSystemServices.CharType);
		}

		IEntity[] GetSetMethods(IEntity[] entities)
		{
			List setMethods = new List();
			for (int i=0; i<entities.Length; ++i)
			{
				IProperty property = entities[i] as IProperty;
				if (null != property)
				{
					IMethod setter = property.GetSetMethod();
					if (null != setter)
					{
						setMethods.AddUnique(setter);
					}
				}
			}
			return ToEntityArray(setMethods);
		}

		IEntity[] GetGetMethods(IEntity[] entities)
		{
			List getMethods = new List();
			for (int i=0; i<entities.Length; ++i)
			{
				IProperty property = entities[i] as IProperty;
				if (null != property)
				{
					IMethod getter = property.GetGetMethod();
					if (null != getter)
					{
						getMethods.AddUnique(getter);
					}
				}
			}
			return ToEntityArray(getMethods);
		}

		private static IEntity[] ToEntityArray(List entities)
		{
			return (IEntity[])entities.ToArray(new IEntity[entities.Count]);
		}

		void CheckNoComplexSlicing(SlicingExpression node)
		{
			if (AstUtil.IsComplexSlicing(node))
			{
				NotImplemented(node, "complex slicing");
			}
		}

		protected MethodInvocationExpression CreateEquals(BinaryExpression node)
		{
			return CodeBuilder.CreateMethodInvocation(RuntimeServices_EqualityOperator, node.Left, node.Right);
		}

		IntegerLiteralExpression CreateIntegerLiteral(long value)
		{
			IntegerLiteralExpression expression = new IntegerLiteralExpression(value);
			Visit(expression);
			return expression;
		}

		bool CheckComplexSlicingParameters(SlicingExpression node)
		{
			foreach (Slice slice in node.Indices)
			{
				if (!CheckComplexSlicingParameters(slice))
				{
					return false;
				}
			}
			return true;
		}

		bool CheckComplexSlicingParameters(Slice node)
		{
			if (null != node.Step)
			{
				NotImplemented(node, "slicing step");
				return false;
			}

			if (OmittedExpression.Default == node.Begin)
			{
				node.Begin = CreateIntegerLiteral(0);
			}
			else
			{
				if (!AssertTypeCompatibility(node.Begin, TypeSystemServices.IntType, GetExpressionType(node.Begin)))
				{
					return false;
				}
			}

			if (null != node.End && OmittedExpression.Default != node.End)
			{
				if (!AssertTypeCompatibility(node.End, TypeSystemServices.IntType, GetExpressionType(node.End)))
				{
					return false;
				}
			}

			return true;
		}

		void BindComplexListSlicing(SlicingExpression node)
		{
			Slice slice = node.Indices[0];

			if (CheckComplexSlicingParameters(slice))
			{
				MethodInvocationExpression mie = null;

				if (null == slice.End || slice.End == OmittedExpression.Default)
				{
					mie = CodeBuilder.CreateMethodInvocation(node.Target, List_GetRange1);
					mie.Arguments.Add(slice.Begin);
				}
				else
				{
					mie = CodeBuilder.CreateMethodInvocation(node.Target, List_GetRange2);
					mie.Arguments.Add(slice.Begin);
					mie.Arguments.Add(slice.End);
				}
				node.ParentNode.Replace(node, mie);
			}
		}

		void BindComplexArraySlicing(SlicingExpression node)
		{
			if (AstUtil.IsLhsOfAssignment(node))
			{
				return;
			}

			if (CheckComplexSlicingParameters(node))
			{
				if (node.Indices.Count > 1)
				{
					IArrayType arrayType = (IArrayType)GetExpressionType(node.Target);
					MethodInvocationExpression mie = null;
					ArrayLiteralExpression collapse = new ArrayLiteralExpression();
					ArrayLiteralExpression ranges = new ArrayLiteralExpression();
					int collapseCount = 0;
					for (int i = 0; i < node.Indices.Count; i++)
					{
						ranges.Items.Add(node.Indices[i].Begin);
						if (node.Indices[i].End == null ||
							node.Indices[i].End == OmittedExpression.Default)
						{
							BinaryExpression end = new BinaryExpression(BinaryOperatorType.Addition,
																		node.Indices[i].Begin,
																		new IntegerLiteralExpression(1));
							ranges.Items.Add(end);
							BindExpressionType(end, GetExpressionType(node.Indices[i].Begin));
							collapse.Items.Add(new BoolLiteralExpression(true));
							collapseCount++;
						}
						else
						{
							ranges.Items.Add(node.Indices[i].End);
							collapse.Items.Add(new BoolLiteralExpression(false));
						}
					}
					mie = CodeBuilder.CreateMethodInvocation(RuntimeServices_GetMultiDimensionalRange1, node.Target, ranges);
					mie.Arguments.Add(collapse);

					BindExpressionType(ranges, TypeSystemServices.Map(typeof(int[])));
					BindExpressionType(collapse, TypeSystemServices.Map(typeof(bool[])));
					BindExpressionType(mie, arrayType.GetElementType().MakeArrayType(node.Indices.Count - collapseCount));
					node.ParentNode.Replace(node, mie);
				}
				else
				{
					Slice slice = node.Indices[0];

					if (CheckComplexSlicingParameters(slice))
					{
						MethodInvocationExpression mie = null;

						if (null == slice.End || slice.End == OmittedExpression.Default)
						{
							mie = CodeBuilder.CreateMethodInvocation(RuntimeServices_GetRange1, node.Target, slice.Begin);
						}
						else
						{
							mie = CodeBuilder.CreateMethodInvocation(RuntimeServices_GetRange2, node.Target, slice.Begin, slice.End);
						}

						BindExpressionType(mie, GetExpressionType(node.Target));
						node.ParentNode.Replace(node, mie);
					}
				}
			}
		}

		bool NeedsNormalization(Expression index)
		{
			if (NodeType.IntegerLiteralExpression == index.NodeType)
			{
				return ((IntegerLiteralExpression)index).Value < 0;
			}
			return true;
		}

		void BindComplexStringSlicing(SlicingExpression node)
		{
			Slice slice = node.Indices[0];

			if (CheckComplexSlicingParameters(slice))
			{
				MethodInvocationExpression mie = null;

				if (null == slice.End || slice.End == OmittedExpression.Default)
				{
					if (NeedsNormalization(slice.Begin))
					{
						mie = CodeBuilder.CreateEvalInvocation(node.LexicalInfo);
						mie.ExpressionType = TypeSystemServices.StringType;

						InternalLocal temp = DeclareTempLocal(TypeSystemServices.StringType);
						mie.Arguments.Add(
							CodeBuilder.CreateAssignment(
								CodeBuilder.CreateReference(temp),
								node.Target));

						mie.Arguments.Add(
							CodeBuilder.CreateMethodInvocation(
								CodeBuilder.CreateReference(temp),
								String_Substring_Int,
								CodeBuilder.CreateMethodInvocation(
									RuntimeServices_NormalizeStringIndex,
									CodeBuilder.CreateReference(temp),
									slice.Begin)));
					}
					else
					{
						mie = CodeBuilder.CreateMethodInvocation(node.Target, String_Substring_Int, slice.Begin);
					}
				}
				else
				{
					mie = CodeBuilder.CreateMethodInvocation(RuntimeServices_Mid, node.Target, slice.Begin, slice.End);
				}

				node.ParentNode.Replace(node, mie);
			}
		}

		protected bool IsIndexedProperty(Expression expression)
		{
			IEntity entity = expression.Entity;
			if (null != entity)
			{
				return IsIndexedProperty(entity);
			}
			return false;
		}

		override public void LeaveSlicingExpression(SlicingExpression node)
		{
			if (IsAmbiguous(node.Target.Entity))
			{
				BindIndexedPropertySlicing(node);
				return;
			}

			// target[indices]
			IType targetType = GetExpressionType(node.Target);
			if (TypeSystemServices.IsError(targetType))
			{
				Error(node);
				return;
			}

			if (IsIndexedProperty(node.Target))
			{
				BindIndexedPropertySlicing(node);
			}
			else
			{
				if (targetType.IsArray)
				{
					IArrayType arrayType = (IArrayType)targetType;
					if (arrayType.GetArrayRank() != node.Indices.Count)
					{
						Error(node, CompilerErrorFactory.InvalidArrayRank(node, node.Target.ToString(), arrayType.GetArrayRank(), node.Indices.Count));
					}

					if (AstUtil.IsComplexSlicing(node))
					{
						BindComplexArraySlicing(node);
					}
					else
					{
						if (arrayType.GetArrayRank() > 1)
						{
							BindMultiDimensionalArraySlicing(node);
						}
						else
						{
							BindExpressionType(node, arrayType.GetElementType());
						}
					}
				}
				else
				{
					if (AstUtil.IsComplexSlicing(node))
					{
						if (TypeSystemServices.StringType == targetType)
						{
							BindComplexStringSlicing(node);
						}
						else
						{
							if (TypeSystemServices.ListType.IsAssignableFrom(targetType))
							{
								BindComplexListSlicing(node);
							}
							else
							{
								NotImplemented(node, "complex slicing for anything but lists, arrays and strings");
							}
						}
					}
					else
					{
						IEntity member = TypeSystemServices.GetDefaultMember(targetType);
						if (null == member)
						{
							Error(node, CompilerErrorFactory.TypeDoesNotSupportSlicing(node.Target, targetType.ToString()));
						}
						else
						{
							node.Target = new MemberReferenceExpression(node.LexicalInfo, node.Target, member.Name);
							node.Target.Entity = member;
							// to be resolved later
							node.Target.ExpressionType = Null.Default;
							SliceMember(node, member);
						}
					}
				}
			}
		}

		private void BindIndexedPropertySlicing(SlicingExpression node)
		{
			CheckNoComplexSlicing(node);
			SliceMember(node, node.Target.Entity);
		}

		private bool IsAmbiguous(IEntity entity)
		{
			return null != entity && EntityType.Ambiguous == entity.EntityType;
		}

		bool IsIndexedProperty(IEntity tag)
		{
			return EntityType.Property == tag.EntityType &&
				((IProperty)tag).GetParameters().Length > 0;
		}

		void BindMultiDimensionalArraySlicing(SlicingExpression node)
		{
			if (AstUtil.IsLhsOfAssignment(node))
			{
				// leave it to LeaveBinaryExpression to resolve
				return;
			}

			MethodInvocationExpression mie = CodeBuilder.CreateMethodInvocation(
				node.Target,
				TypeSystemServices.Map(
					typeof(Array).GetMethod("GetValue", new Type[] { typeof(int[]) })));
			for (int i = 0; i < node.Indices.Count; i++)
			{
				mie.Arguments.Add(node.Indices[i].Begin);
			}

			IType elementType = node.Target.ExpressionType.GetElementType();
			node.ParentNode.Replace(node, CodeBuilder.CreateCast(elementType, mie));
		}

		void SliceMember(SlicingExpression node, IEntity member)
		{
			EnsureRelatedNodeWasVisited(node, member);
			if (AstUtil.IsLhsOfAssignment(node))
			{
				// leave it to LeaveBinaryExpression to resolve
				Bind(node, member);
				return;
			}

			MethodInvocationExpression mie = new MethodInvocationExpression(node.LexicalInfo);
			foreach (Slice index in node.Indices)
			{
				mie.Arguments.Add(index.Begin);
			}

			IMethod getter = null;
			if (EntityType.Ambiguous == member.EntityType)
			{
				IEntity result = ResolveAmbiguousPropertyReference((ReferenceExpression)node.Target, (Ambiguous)member, mie.Arguments);
				IProperty found = result as IProperty;
				if (null != found)
				{
					getter = found.GetGetMethod();
				}
				else if (EntityType.Ambiguous == result.EntityType)
				{
					Error(node);
					return;
				}
			}
			else if (EntityType.Property == member.EntityType)
			{
				getter = ((IProperty)member).GetGetMethod();
			}

			if (null != getter)
			{
				if (AssertParameters(node, getter, mie.Arguments))
				{
					Expression target = GetIndexedPropertySlicingTarget(node);

					mie.Target = CodeBuilder.CreateMemberReference(target, getter);
					BindExpressionType(mie, getter.ReturnType);

					node.ParentNode.Replace(node, mie);
				}
				else
				{
					Error(node);
				}
			}
			else
			{
				NotImplemented(node, "slice for anything but arrays and default properties");
			}
		}

		private Expression GetIndexedPropertySlicingTarget(SlicingExpression node)
		{
			Expression target = node.Target;
			MemberReferenceExpression mre = target as MemberReferenceExpression;
			if (null != mre) return mre.Target;
			return CreateSelfReference();
		}

		override public void LeaveExpressionInterpolationExpression(ExpressionInterpolationExpression node)
		{
			BindExpressionType(node, TypeSystemServices.StringType);
		}

		override public void LeaveListLiteralExpression(ListLiteralExpression node)
		{
			BindExpressionType(node, TypeSystemServices.ListType);
			TypeSystemServices.MapToConcreteExpressionTypes(node.Items);
		}

		override public void OnExtendedGeneratorExpression(ExtendedGeneratorExpression node)
		{
			BlockExpression block = new BlockExpression(node.LexicalInfo);

			Block body = block.Body;
			Expression e = node.Items[0].Expression;
			foreach (GeneratorExpression ge in node.Items)
			{
				ForStatement fs = new ForStatement(ge.LexicalInfo);
				fs.Iterator = ge.Iterator;
				fs.Declarations = ge.Declarations;

				body.Add(fs);

				if (null == ge.Filter)
				{
					body = fs.Block;
				}
				else
				{
					fs.Block.Add(
						NormalizeStatementModifiers.MapStatementModifier(ge.Filter, out body));
				}


			}
			body.Add(new YieldStatement(e.LexicalInfo, e));

			MethodInvocationExpression mie = new MethodInvocationExpression(node.LexicalInfo);
			mie.Target = block;

			Node parentNode = node.ParentNode;
			bool isGenerator = AstUtil.IsListMultiGenerator(parentNode);
			parentNode.Replace(node, mie);
			mie.Accept(this);

			if (isGenerator)
			{
				parentNode.ParentNode.Replace(
					parentNode,
					CodeBuilder.CreateConstructorInvocation(
						TypeSystemServices.Map(ProcessGenerators.List_IEnumerableConstructor),
						mie));
			}
		}

		override public void OnGeneratorExpression(GeneratorExpression node)
		{
			Visit(node.Iterator);
			node.Iterator = ProcessIterator(node.Iterator, node.Declarations);

			EnterNamespace(new DeclarationsNamespace(CurrentNamespace, TypeSystemServices, node.Declarations));
			Visit(node.Filter);
			Visit(node.Expression);
			LeaveNamespace();

			BooClassBuilder generatorType = CreateGeneratorSkeleton(node);

			IType returnType = generatorType.Entity;
			if (TypeSystemServices.VoidType != (IType) node["GeneratorItemType"])
			{
				//bind to corresponding IEnumerator[of T] type in order to get a safe return type
				//that can be used from external assemblies (ie. not a generated internal nested type)
				returnType = TypeSystemServices.IEnumerableGenericType.GenericInfo.ConstructType((IType) node["GeneratorItemType"]);
			}
			BindExpressionType(node, returnType);
		}

		void CreateGeneratorSkeleton(InternalMethod entity)
		{
			Method method = entity.Method;
			IType itemType = GetGeneratorItemType(entity);
			BooClassBuilder builder = CreateGeneratorSkeleton(method, method, itemType);
			method.DeclaringType.Members.Add(builder.ClassDefinition);
		}

		private IType GetGeneratorItemType(InternalMethod entity)
		{
			IType itemType = null;

			if (CheckGenericGeneratorReturnType(entity.ReturnType))
			{
				itemType = entity.ReturnType.ConstructedInfo.GenericArguments[0];
			}
			if (itemType == null)
			{
				ExpressionCollection yieldExpressions = entity.YieldExpressions;

				itemType = yieldExpressions.Count > 0
					? GetMostGenericType(yieldExpressions)
					: TypeSystemServices.ObjectType;
			}
			return itemType;
		}

		BooClassBuilder CreateGeneratorSkeleton(GeneratorExpression node)
		{
			BooClassBuilder builder = CreateGeneratorSkeleton(node, _currentMethod.Method, GetConcreteExpressionType(node.Expression));
			_currentMethod.Method.DeclaringType.Members.Add(builder.ClassDefinition);
			return builder;
		}

		protected IType GetConstructedType(IType genericType, IType argType)
		{
			return genericType.GenericInfo.ConstructType(argType);
		}

		BooClassBuilder CreateGeneratorSkeleton(Node sourceNode, Method method, IType generatorItemType)
		{
			// create the class skeleton for type inference to work
			BooClassBuilder builder = CodeBuilder.CreateClass(
				Context.GetUniqueName(method.Name),
				TypeMemberModifiers.Internal|TypeMemberModifiers.Final);

			builder.LexicalInfo = sourceNode.LexicalInfo;
			builder.AddAttribute(CodeBuilder.CreateAttribute(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute)));

			BooMethodBuilder getEnumeratorBuilder = null;
			if (generatorItemType != TypeSystemServices.VoidType)
			{
				builder.AddBaseType(
					TypeSystemServices.Map(typeof(GenericGenerator<>)).GenericInfo.ConstructType(generatorItemType));

				getEnumeratorBuilder = builder.AddVirtualMethod(
					"GetEnumerator",
					TypeSystemServices.IEnumeratorGenericType.GenericInfo.ConstructType(generatorItemType));

				getEnumeratorBuilder.Method.LexicalInfo = sourceNode.LexicalInfo;
			}

			sourceNode["GeneratorClassBuilder"] = builder;
			sourceNode["GetEnumeratorBuilder"] = getEnumeratorBuilder;
			sourceNode["GeneratorItemType"] = generatorItemType;

			return builder;
		}

		override public void LeaveHashLiteralExpression(HashLiteralExpression node)
		{
			BindExpressionType(node, TypeSystemServices.HashType);
			foreach (ExpressionPair pair in node.Items)
			{
				GetConcreteExpressionType(pair.First);
				GetConcreteExpressionType(pair.Second);
			}
		}

		override public void LeaveArrayLiteralExpression(ArrayLiteralExpression node)
		{
			TypeSystemServices.MapToConcreteExpressionTypes(node.Items);
			IArrayType type = InferArrayType(node);
			BindExpressionType(node, type);
			if (null == node.Type)
			{
				node.Type = (ArrayTypeReference)CodeBuilder.CreateTypeReference(type);
			}
			else
			{
				CheckItems(type.GetElementType(), node.Items);
			}
		}

		private IArrayType InferArrayType(ArrayLiteralExpression node)
		{
			if (null != node.Type) return (IArrayType)node.Type.Entity;
			if (0 == node.Items.Count) return TypeSystemServices.ObjectArrayType;
			return GetMostGenericType(node.Items).MakeArrayType(1);
		}

		override public void LeaveDeclaration(Declaration node)
		{
			if (null == node.Type) return;
			CheckDeclarationType(node.Type);
		}

		override public void LeaveDeclarationStatement(DeclarationStatement node)
		{
			IType type = GetDeclarationType(node);

			AssertDeclarationName(node.Declaration);

			IEntity localInfo = DeclareLocal(node, node.Declaration.Name, type);
			InternalLocal loopLocal = localInfo as InternalLocal;
			if (null != loopLocal)
				loopLocal.OriginalDeclaration = node.Declaration;

			if (null != node.Initializer)
			{
				IType itype = GetExpressionType(node.Initializer);
				AssertTypeCompatibility(node.Initializer, type, itype);

				if (TypeSystemServices.IsNullable(type) && !TypeSystemServices.IsNullable(itype))
				{
					BindNullableInitializer(node, node.Initializer, type);
				}

				node.ReplaceBy(
					new ExpressionStatement(
						CodeBuilder.CreateAssignment(
							node.LexicalInfo,
							CodeBuilder.CreateReference(localInfo),
							node.Initializer)));
			}
			else
			{
				node.ReplaceBy(
					new ExpressionStatement(
						CreateDefaultLocalInitializer(node, localInfo)));
			}
		}

		private IType GetDeclarationType(DeclarationStatement node)
		{
			if (null != node.Declaration.Type) return GetType(node.Declaration.Type);

			return InferDeclarationType(node);
		}

		private IType InferDeclarationType(DeclarationStatement node)
		{
			if (null == node.Initializer) return TypeSystemServices.ObjectType;

			// The boo syntax does not require this check because
			// there's no way to create an untyped declaration statement.
			// This is here to support languages that do allow untyped variable
			// declarations (unityscript is such an example).
			return GetConcreteExpressionType(node.Initializer);
		}

		virtual protected Expression CreateDefaultLocalInitializer(Node sourceNode, IEntity local)
		{
			return CodeBuilder.CreateDefaultInitializer(
				sourceNode.LexicalInfo,
				(InternalLocal)local);
		}

		override public void LeaveExpressionStatement(ExpressionStatement node)
		{
			AssertHasSideEffect(node.Expression);
		}

		override public void OnNullLiteralExpression(NullLiteralExpression node)
		{
			BindExpressionType(node, Null.Default);
		}

		override public void OnSelfLiteralExpression(SelfLiteralExpression node)
		{
			if (null == _currentMethod)
			{
				Error(node, CompilerErrorFactory.SelfOutsideMethod(node));
			}
			else
			{
				if (_currentMethod.IsStatic)
				{
					if (NodeType.MemberReferenceExpression != node.ParentNode.NodeType)
					{
						// if we are inside a MemberReferenceExpression
						// let the MemberReferenceExpression deal with it
						// as it can provide a better message
						Error(CompilerErrorFactory.SelfIsNotValidInStaticMember(node));
					}
				}
				node.Entity = _currentMethod;
				node.ExpressionType = _currentMethod.DeclaringType;
			}
		}

		override public void LeaveTypeofExpression(TypeofExpression node)
		{
			BindExpressionType(node, TypeSystemServices.TypeType);
		}

		override public void LeaveCastExpression(CastExpression node)
		{
			IType fromType = GetExpressionType(node.Target);
			IType toType = GetType(node.Type);
			if (!TypeSystemServices.AreTypesRelated(toType, fromType) &&
				!(toType.IsInterface && !fromType.IsFinal) &&
				!(TypeSystemServices.IsIntegerNumber(toType) && TypeSystemServices.CanBeExplicitlyCastToInteger(fromType)) &&
				!(TypeSystemServices.IsIntegerNumber(fromType) && TypeSystemServices.CanBeExplicitlyCastToInteger(toType)))
			{
				IMethod explicitOperator = TypeSystemServices.FindExplicitConversionOperator(fromType, toType);
				if (null != explicitOperator)
				{
					node.ParentNode.Replace(
						node,
						CodeBuilder.CreateMethodInvocation(
							explicitOperator,
							node.Target));
					return;
				}

				Error(
					CompilerErrorFactory.IncompatibleExpressionType(
						node,
						toType.ToString(),
						fromType.ToString()));
			}
			BindExpressionType(node, toType);
		}

		override public void LeaveTryCastExpression(TryCastExpression node)
		{
			IType target = GetExpressionType(node.Target);
			IType toType = GetType(node.Type);

			if (target.IsValueType)
			{
				Error(CompilerErrorFactory.CantCastToValueType(node.Target, target.ToString()));
			}
			else if (toType.IsValueType)
			{
				Error(CompilerErrorFactory.CantCastToValueType(node.Type, toType.ToString()));
			}
			BindExpressionType(node, toType);
		}

		protected Expression CreateMemberReferenceTarget(Node sourceNode, IMember member)
		{
			Expression target = null;

			if (member.IsStatic)
			{
				target = new ReferenceExpression(sourceNode.LexicalInfo, member.DeclaringType.FullName);
				Bind(target, member.DeclaringType);
			}
			else
			{
				//check if found entity can't possibly be a member of self:
				if (member.DeclaringType != CurrentType
					&& !(CurrentType.IsSubclassOf(member.DeclaringType)))
				{
					Error(
						CompilerErrorFactory.InstanceRequired(sourceNode,
															  member.DeclaringType.ToString(),
															  member.Name));
				}
				target = new SelfLiteralExpression(sourceNode.LexicalInfo);
			}
			BindExpressionType(target, member.DeclaringType);
			return target;
		}

		protected MemberReferenceExpression MemberReferenceFromReference(ReferenceExpression node, IMember member)
		{
			MemberReferenceExpression memberRef = new MemberReferenceExpression(node.LexicalInfo);
			memberRef.Name = node.Name;
			memberRef.Target = CreateMemberReferenceTarget(node, member);
			return memberRef;
		}

		void ResolveMemberInfo(ReferenceExpression node, IMember member)
		{
			MemberReferenceExpression memberRef = MemberReferenceFromReference(node, member);
			Bind(memberRef, member);
			node.ParentNode.Replace(node, memberRef);
			Visit(memberRef);
		}

		override public void OnRELiteralExpression(RELiteralExpression node)
		{
			if (null != node.Entity)
			{
				return;
			}

			IType type = TypeSystemServices.RegexType;
			BindExpressionType(node, type);

			if (NodeType.Field != node.ParentNode.NodeType)
			{
				ReplaceByStaticFieldReference(node, Context.GetUniqueName("re"), type);
			}
		}

		void ReplaceByStaticFieldReference(Expression node, string fieldName, IType type)
		{
			Node parent = node.ParentNode;

			Field field = CodeBuilder.CreateField(fieldName, type);
			field.Modifiers = TypeMemberModifiers.Internal|TypeMemberModifiers.Static;
			field.Initializer = node;

			_currentMethod.Method.DeclaringType.Members.Add(field);
			parent.Replace(node, CodeBuilder.CreateReference(field));

			AddFieldInitializerToStaticConstructor(0, field);
		}

		override public void LeaveGenericReferenceExpression(GenericReferenceExpression node)
		{
			if (node.Target.Entity == null || TypeSystemServices.IsError(node.Target.Entity))
			{
				BindExpressionType(node, TypeSystemServices.ErrorEntity);
				return;
			}

			IEntity entity = NameResolutionService.ResolveGenericReferenceExpression(node, node.Target.Entity);
			Bind(node, entity);

			if (entity.EntityType == EntityType.Type)
			{
				BindTypeReferenceExpressionType(node, (IType)entity);
			}
			else if (entity.EntityType == EntityType.Method)
			{
				if (null == (node.Target as MemberReferenceExpression)) //no self.
				{
					MemberReferenceExpression target =
						CodeBuilder.MemberReferenceForEntity(
							CreateSelfReference(),
							entity);
					node.Replace(node.Target, target);
				}
				BindExpressionType(node, ((IMethod)entity).Type);
			}
		}

		override public void OnReferenceExpression(ReferenceExpression node)
		{
			if (AlreadyBound(node)) return;

			IEntity entity = ResolveName(node, node.Name);
			if (null == entity)
			{
				Error(node);
				return;
			}

			// BOO-314 - if we are trying to invoke
			// something, let's make sure it is
			// something callable, otherwise, let's
			// try to find something callable
			if (AstUtil.IsTargetOfMethodInvocation(node)
				&& !IsCallableEntity(entity))
			{
				IEntity callable = ResolveCallable(node);
				if (null != callable) entity = callable;
			}

			IMember member = entity as IMember;
			if (null != member)
			{
				if (IsExtensionMethod(member))
				{
					Bind(node, member);
					return;
				}
				ResolveMemberInfo(node, member);
				return;
			}

			EnsureRelatedNodeWasVisited(node, entity);
			node.Entity = entity;
			PostProcessReferenceExpression(node);
		}

		private static bool AlreadyBound(ReferenceExpression node)
		{
			return null != node.ExpressionType;
		}

		private IEntity ResolveCallable(ReferenceExpression node)
		{
			return NameResolutionService.Resolve(node.Name,
												 EntityType.Type
												 | EntityType.Method
												 | EntityType.BuiltinFunction
												 | EntityType.Event);
		}

		private bool IsCallableEntity(IEntity entity)
		{
			switch (entity.EntityType)
			{
				case EntityType.Method:
				case EntityType.Type:
				case EntityType.Event:
				case EntityType.BuiltinFunction:
				case EntityType.Constructor:
					return true;

				case EntityType.Ambiguous:
					// let overload resolution deal with it
					return true;
			}
			ITypedEntity typed = entity as ITypedEntity;
			return null == typed ? false : TypeSystemServices.IsCallable(typed.Type);
		}

		void PostProcessReferenceExpression(ReferenceExpression node)
		{
			IEntity tag = GetEntity(node);
			switch (tag.EntityType)
			{
				case EntityType.Type:
					{
						BindTypeReferenceExpressionType(node, (IType)tag);
						break;
					}

				case EntityType.Ambiguous:
					{
						Ambiguous ambiguous = (Ambiguous) tag;
						tag = ResolveAmbiguousReference(node, ambiguous);
						IMember resolvedMember	= tag as IMember;
						if (null != resolvedMember)
						{
							ResolveMemberInfo(node, resolvedMember);
							break;
						}
						else if (tag is IType)
						{
							BindTypeReferenceExpressionType(node, (IType)tag);
							break;
						}
						else if (!AstUtil.IsTargetOfMethodInvocation(node)
							&& !AstUtil.IsTargetOfSlicing(node)
							&& !AstUtil.IsLhsOfAssignment(node))
						{
							Error(node, CompilerErrorFactory.AmbiguousReference(
								node,
								node.Name,
								ambiguous.Entities));
						}
						break;
					}

				case EntityType.Namespace:
					{
						if (IsStandaloneReference(node))
						{
							Error(node, CompilerErrorFactory.NamespaceIsNotAnExpression(node, tag.Name));
						}
						break;
					}

				case EntityType.Parameter:
				case EntityType.Local:
					{
						ILocalEntity local = (ILocalEntity)node.Entity;
						local.IsUsed = true;
						BindExpressionType(node, local.Type);
						break;
					}

				default:
					{
						if (EntityType.BuiltinFunction == tag.EntityType)
						{
							CheckBuiltinUsage(node, tag);
						}
						else
						{
							if (node.ExpressionType == null)
							{
								BindExpressionType(node, ((ITypedEntity)tag).Type);
							}
						}
						break;
					}
			}
		}

		protected virtual void BindTypeReferenceExpressionType(Expression node, IType type)
		{
			if (IsStandaloneReference(node))
			{
				BindExpressionType(node, TypeSystemServices.TypeType);
			}
			else
			{
				BindExpressionType(node, type);
			}
		}

		protected virtual void CheckBuiltinUsage(ReferenceExpression node, IEntity entity)
		{
			if (!AstUtil.IsTargetOfMethodInvocation(node))
			{
				Error(node, CompilerErrorFactory.BuiltinCannotBeUsedAsExpression(node, entity.Name));
			}
		}

		override public bool EnterMemberReferenceExpression(MemberReferenceExpression node)
		{
			return null == node.ExpressionType;
		}

		INamespace GetReferenceNamespace(MemberReferenceExpression expression)
		{
			Expression target = expression.Target;
			INamespace ns = target.ExpressionType;
			if (null != ns)
			{
				return GetConcreteExpressionType(target);
			}
			return (INamespace)GetEntity(target);
		}

		protected virtual void LeaveExplodeExpression(UnaryExpression node)
		{
			IType type = GetConcreteExpressionType(node.Operand);
			if (!type.IsArray)
				Error(CompilerErrorFactory.ExplodedExpressionMustBeArray(node));

			BindExpressionType(node, type);
		}

		override public void LeaveMemberReferenceExpression(MemberReferenceExpression node)
		{
			_context.TraceVerbose("LeaveMemberReferenceExpression: {0}", node);

			if (TypeSystemServices.IsError(node.Target))
			{
				Error(node);
			}
			else
			{
				ProcessMemberReferenceExpression(node);
			}
		}

		virtual protected void MemberNotFound(MemberReferenceExpression node, INamespace ns)
		{
			EntityType et = (!AstUtil.IsTargetOfMethodInvocation(node)) ? EntityType.Any : EntityType.Method;
			Error(node,
				CompilerErrorFactory.MemberNotFound(node,
										(((IEntity)ns).ToString()),
										NameResolutionService.GetMostSimilarMemberName(ns, node.Name, et)));
		}

		virtual protected bool ShouldRebindMember(IEntity entity)
		{
			return entity == null;
		}

		IEntity ResolveMember(MemberReferenceExpression node)
		{
			IEntity member = node.Entity;
			if (!ShouldRebindMember(member))
				return member;

			INamespace ns = GetReferenceNamespace(node);
			member = NameResolutionService.Resolve(ns, node.Name);

			if (null == member || !IsAccessible(member))
			{
				IEntity extension = TryToResolveMemberAsExtension(node);
				if (extension != null) return extension;
			}

			if (null == member) MemberNotFound(node, ns);

			return member;
		}

		private IEntity TryToResolveMemberAsExtension(MemberReferenceExpression node)
		{
			IEntity extension = NameResolutionService.ResolveExtension(GetReferenceNamespace(node), node.Name);
			if (null != extension)
			{
				node.Annotate(ResolvedAsExtensionAnnotation);
			}

			return extension;
		}

		virtual protected void ProcessMemberReferenceExpression(MemberReferenceExpression node)
		{
			IEntity member = ResolveMember(node);
			if (null == member)
				return;

			if (EntityType.Ambiguous == member.EntityType)
			{
				member = ResolveAmbiguousReference(node, (Ambiguous)member);
			}

			EnsureRelatedNodeWasVisited(node, member);

			if (EntityType.Namespace == member.EntityType)
			{
				MarkRelatedImportAsUsed(node);
			}

			IMember memberInfo = member as IMember;
			if (null != memberInfo)
			{
				if (!AssertTargetContext(node, memberInfo))
				{
					Error(node);
					return;
				}

				if (EntityType.Method != memberInfo.EntityType)
				{
					BindExpressionType(node, GetInferredType(memberInfo));
				}
				else
				{
					BindExpressionType(node, memberInfo.Type);
				}
			}

			if (EntityType.Property == member.EntityType)
			{
				IProperty property = (IProperty)member;
				if (IsIndexedProperty(property))
				{
					if (!AstUtil.IsTargetOfSlicing(node)
						&& (!property.IsExtension || property.GetParameters().Length > 1))
					{
						Error(node, CompilerErrorFactory.PropertyRequiresParameters(
							AstUtil.GetMemberAnchor(node),
							member.FullName));
						return;
					}
				}
				if (IsWriteOnlyProperty(property) && !IsBeingAssignedTo(node))
				{
					Error(node, CompilerErrorFactory.PropertyIsWriteOnly(
						AstUtil.GetMemberAnchor(node),
						member.FullName));
				}
			}
			else if (EntityType.Event == member.EntityType)
			{
				if (!AstUtil.IsTargetOfMethodInvocation(node) &&
					!AstUtil.IsLhsOfInPlaceAddSubtract(node))
				{
					if (CurrentType == memberInfo.DeclaringType)
					{
						InternalEvent ev = (InternalEvent)member;
						node.Name = ev.BackingField.Name;
						node.Entity = ev.BackingField;
						BindExpressionType(node, ev.BackingField.Type);
						return;
					}
					else if (!AstUtil.IsLhsOfAssignment(node)
					         || !IsNull(((BinaryExpression)node.ParentNode).Right))
					{
						Error(node,
							  CompilerErrorFactory.EventIsNotAnExpression(node,
																		  member.FullName));
					}
					else //event=null
					{
						EnsureInternalEventInvocation((IEvent) member, node);
					}
				}
			}

			Bind(node, member);
			PostProcessReferenceExpression(node);
		}

		private void MarkRelatedImportAsUsed(MemberReferenceExpression node)
		{
			string ns = null;
			foreach (Import import in _currentModule.Imports)
			{
				if (ImportAnnotations.IsUsedImport(import)) continue;
				if (null == ns) ns = node.ToCodeString();
				if (import.Namespace == ns)
				{
					ImportAnnotations.MarkAsUsed(import);
					break;
				}
			}
		}

		private bool IsBeingAssignedTo(MemberReferenceExpression node)
		{
			Node current = node;
			Node parent = current.ParentNode;
			BinaryExpression be = parent as BinaryExpression;
			while (null == be)
			{
				current = parent;
				parent = parent.ParentNode;
				if (parent == null || !(parent is Expression))
					return false;
				be = parent as BinaryExpression;
			}
			return be.Left == current;
		}

		private bool IsWriteOnlyProperty(IProperty property)
		{
			return null == property.GetGetMethod();
		}

		private IEntity ResolveAmbiguousLValue(Expression sourceNode, Ambiguous candidates, Expression rvalue)
		{
			if (!candidates.AllEntitiesAre(EntityType.Property)) return null;

			IEntity[] entities = candidates.Entities;
			IEntity[] getters = GetSetMethods(entities);
			ExpressionCollection args = new ExpressionCollection();
			args.Add(rvalue);
			IEntity found = GetCorrectCallableReference(sourceNode, args, getters);
			if (null != found && EntityType.Method == found.EntityType)
			{
				IProperty property = (IProperty)entities[GetIndex(getters, found)];
				BindProperty(sourceNode, property);
				return property;
			}
			return null;
		}

		private static void BindProperty(Expression expression, IProperty property)
		{
			expression.Entity = property;
			expression.ExpressionType = property.Type;
		}

		private IEntity ResolveAmbiguousReference(ReferenceExpression node, Ambiguous candidates)
		{
			IEntity resolved = ResolveAmbiguousReferenceByAccessibility(candidates);
			Ambiguous accessibleCandidates = resolved as Ambiguous;

			if (accessibleCandidates != null &&
				!AstUtil.IsTargetOfSlicing(node) &&
				!AstUtil.IsLhsOfAssignment(node))
			{
				if (accessibleCandidates.AllEntitiesAre(EntityType.Property))
				{
					return ResolveAmbiguousPropertyReference(node, accessibleCandidates, EmptyExpressionCollection);
				}
				if (accessibleCandidates.AllEntitiesAre(EntityType.Method))
				{
					return ResolveAmbiguousMethodReference(node, accessibleCandidates, EmptyExpressionCollection);
				}
				if (accessibleCandidates.AllEntitiesAre(EntityType.Type))
				{
					return ResolveAmbiguousTypeReference(node, accessibleCandidates);
				}
			}

			return resolved;
		}

		private IEntity ResolveAmbiguousMethodReference(ReferenceExpression node, Ambiguous candidates, ExpressionCollection args)
		{
			//BOO-656
			if (!AstUtil.IsTargetOfMethodInvocation(node)
				&& !AstUtil.IsTargetOfSlicing(node)
				&& !AstUtil.IsLhsOfAssignment(node))
			{
				return candidates.Entities[0];
			}
			return candidates;
		}

		private IEntity ResolveAmbiguousPropertyReference(ReferenceExpression node, Ambiguous candidates, ExpressionCollection args)
		{
			IEntity[] entities = candidates.Entities;
			IEntity[] getters = GetGetMethods(entities);
			IEntity found = GetCorrectCallableReference(node, args, getters);
			if (null != found && EntityType.Method == found.EntityType)
			{
				IProperty property = (IProperty)entities[GetIndex(getters, found)];
				BindProperty(node, property);
				return property;
			}

			return candidates;
		}

		private IEntity ResolveAmbiguousTypeReference(ReferenceExpression node, Ambiguous candidates)
		{
			List<IEntity> matches = GetMatchesByGenericity(node, candidates);
			if (matches.Count > 1)
			{
				PreferInternalEntitiesOverNonInternal(matches);
			}

			if (matches.Count == 1)
			{
				Bind(node, matches[0]);
			}
			else
			{
				Bind(node, new Ambiguous(matches));
			}

			return node.Entity;
		}

		private static void PreferInternalEntitiesOverNonInternal(List<IEntity> matches)
		{
			bool isAmbiguousBetweenInternalAndExternalEntities = matches.Contains(EntityPredicates.IsInternalEntity) &&
			                                                     matches.Contains(EntityPredicates.IsNonInternalEntity);
			if (isAmbiguousBetweenInternalAndExternalEntities)
				matches.RemoveAll(EntityPredicates.IsNonInternalEntity);
		}

		private List<IEntity> GetMatchesByGenericity(ReferenceExpression node, Ambiguous candidates)
		{
			bool isGenericReference = (node.ParentNode is GenericReferenceExpression);
			List<IEntity> matches = new List<IEntity>();
			foreach (IEntity candidate in candidates.Entities)
			{
				IType type = candidate as IType;
				bool isGenericType = (type != null && type.GenericInfo != null);
				if (isGenericType == isGenericReference)
				{
					matches.Add(candidate);
				}
			}
			return matches;
		}

		private IEntity ResolveAmbiguousReferenceByAccessibility(Ambiguous candidates)
		{
			List<IEntity> newEntities = new List<IEntity>();
			foreach (IEntity entity in candidates.Entities)
			{
				if (!IsInaccessible(entity))
				{
					newEntities.Add(entity);
				}
			}
			return Entities.EntityFromList(newEntities);
		}

		private int GetIndex(IEntity[] entities, IEntity entity)
		{
			for (int i=0; i<entities.Length; ++i)
			{
				if (entities[i] == entity) return i;
			}
			throw new ArgumentException("entity");
		}

		override public void LeaveUnlessStatement(UnlessStatement node)
		{
			node.Condition = AssertBoolContext(node.Condition);
		}

		override public void LeaveIfStatement(IfStatement node)
		{
			node.Condition = AssertBoolContext(node.Condition);
		}

		override public void LeaveConditionalExpression(ConditionalExpression node)
		{
			node.Condition = AssertBoolContext(node.Condition);

			IType trueType = null;
			IType falseType = null;
			if (null != node.TrueValue)
				trueType = GetExpressionType(node.TrueValue);
			if (null != node.FalseValue)
				falseType = GetExpressionType(node.FalseValue);

			BindExpressionType(node, GetMostGenericType(trueType, falseType));
		}

		override public bool EnterWhileStatement(WhileStatement node)
		{
			return true;
		}

		override public void LeaveWhileStatement(WhileStatement node)
		{
			node.Condition = AssertBoolContext(node.Condition);
		}

		override public void LeaveYieldStatement(YieldStatement node)
		{
			if (EntityType.Constructor == _currentMethod.EntityType)
			{
				Error(CompilerErrorFactory.YieldInsideConstructor(node));
			}
			else
			{
				_currentMethod.AddYieldStatement(node);
			}
		}

		override public void LeaveReturnStatement(ReturnStatement node)
		{
			if (null == node.Expression)
				return;

			// forces anonymous types to be correctly
			// instantiated
			IType expressionType = GetConcreteExpressionType(node.Expression);
			if (TypeSystemServices.VoidType == expressionType
				&& node.ContainsAnnotation(OptionalReturnStatementAnnotation))
			{
				node.ParentNode.Replace(
					node,
					new ExpressionStatement(node.Expression));
				return;
			}

			IType returnType = _currentMethod.ReturnType;
			if (TypeSystemServices.IsUnknown(returnType))
			{
				_currentMethod.AddReturnExpression(node.Expression);
			}
			else
			{
				AssertTypeCompatibility(node.Expression, returnType, expressionType);
			}

			//bind to nullable Value if needed
			if (TypeSystemServices.IsNullable(expressionType) && !TypeSystemServices.IsNullable(returnType))
			{
				MemberReferenceExpression mre = new MemberReferenceExpression(node.Expression.LexicalInfo, node.Expression, "Value");
				Visit(mre);
				node.Replace(node.Expression, mre);
			}
		}

		protected Expression GetCorrectIterator(Expression iterator)
		{
			IType type = GetExpressionType(iterator);
			if (TypeSystemServices.IsError(type))
			{
				return iterator;
			}

			if (!TypeSystemServices.IEnumerableType.IsAssignableFrom(type) &&
				!TypeSystemServices.IEnumeratorType.IsAssignableFrom(type))
			{
				if (IsRuntimeIterator(type))
				{
					if (IsTextReader(type))
					{
						return CodeBuilder.CreateMethodInvocation(TextReaderEnumerator_lines, iterator);
					}
					else
					{
						return CodeBuilder.CreateMethodInvocation(RuntimeServices_GetEnumerable, iterator);
					}
				}
			}
			return iterator;
		}

		/// <summary>
		/// Process a iterator and its declarations and returns a new iterator
		/// expression if necessary.
		/// </summary>
		protected Expression ProcessIterator(Expression iterator, DeclarationCollection declarations)
		{
			iterator = GetCorrectIterator(iterator);
			ProcessDeclarationsForIterator(declarations, GetExpressionType(iterator));
			return iterator;
		}

		public override void OnGotoStatement(GotoStatement node)
		{
			// don't try to resolve label references
		}

		override public void OnForStatement(ForStatement node)
		{
			Visit(node.Iterator);
			node.Iterator = ProcessIterator(node.Iterator, node.Declarations);
			VisitForStatementBlock(node);
		}

		protected void VisitForStatementBlock(ForStatement node)
		{
			EnterForNamespace(node);
			Visit(node.Block);
			Visit(node.OrBlock);
			Visit(node.ThenBlock);
			LeaveNamespace();
		}

		private void EnterForNamespace(ForStatement node)
		{
			EnterNamespace(new DeclarationsNamespace(CurrentNamespace, TypeSystemServices, node.Declarations));
		}

		override public void OnUnpackStatement(UnpackStatement node)
		{
			Visit(node.Expression);

			node.Expression = GetCorrectIterator(node.Expression);

			IType defaultDeclarationType = GetEnumeratorItemType(GetExpressionType(node.Expression));
			foreach (Declaration d in node.Declarations)
			{
				bool declareNewVariable = d.Type != null;

				GetDeclarationType(defaultDeclarationType, d);
				if (declareNewVariable)
				{
					AssertUniqueLocal(d);
				}
				else
				{
					IEntity tag = NameResolutionService.Resolve(d.Name);
					if (null != tag)
					{
						Bind(d, tag);
						AssertLValue(d);
						continue;
					}
				}
				DeclareLocal(d, false);
			}
		}

		override public void LeaveRaiseStatement(RaiseStatement node)
		{
			if (node.Exception == null) return;

			IType exceptionType = GetExpressionType(node.Exception);
			if (TypeSystemServices.StringType == exceptionType)
			{
				node.Exception = CodeBuilder.CreateConstructorInvocation(
					node.Exception.LexicalInfo,
					Exception_StringConstructor,
					node.Exception);
			}
			else if (!TypeSystemServices.IsValidException(exceptionType))
			{
				Error(CompilerErrorFactory.InvalidRaiseArgument(node.Exception,
																exceptionType.ToString()));
			}
		}

		override public void OnExceptionHandler(ExceptionHandler node)
		{
			bool untypedException = (node.Flags & ExceptionHandlerFlags.Untyped) == ExceptionHandlerFlags.Untyped;
			bool anonymousException = (node.Flags & ExceptionHandlerFlags.Anonymous) == ExceptionHandlerFlags.Anonymous;
			bool filterHandler = (node.Flags & ExceptionHandlerFlags.Filter) == ExceptionHandlerFlags.Filter;

			if (untypedException)
			{
				// If untyped, set the handler to except System.Exception
				node.Declaration.Type = CodeBuilder.CreateTypeReference(TypeSystemServices.ExceptionType);
			}
			else
			{
				Visit(node.Declaration.Type);

				// Require typed exception handlers to except only
				// exceptions at least as derived as System.Exception
				if(!TypeSystemServices.IsValidException(GetType(node.Declaration.Type)))
				{
					Errors.Add(CompilerErrorFactory.InvalidExceptArgument(node.Declaration.Type, GetType(node.Declaration.Type).FullName));
				}
			}

			if(!anonymousException)
			{
				// If the exception is not anonymous, place it into a
				// local variable and enter a new namespace
				DeclareLocal(node.Declaration, true);
				EnterNamespace(new DeclarationsNamespace(CurrentNamespace, TypeSystemServices, node.Declaration));
			}

			try
			{
				// The filter handler has access to the exception if it
				// is not anonymous, so it is protected to ensure
				// any exception in the filter condition (a big no-no)
				// will still clean up the namespace if necessary
				if (filterHandler)
				{
					Visit(node.FilterCondition);
				}

				Visit(node.Block);
			}
			finally
			{
				// Clean up the namespace if necessary
				if(!anonymousException)
				{
					LeaveNamespace();
				}
			}
		}

		protected virtual bool IsValidIncrementDecrementOperand(Expression e)
		{
			IType type = GetExpressionType(e);

			if (type.IsPointer)
				return true;

			if (TypeSystemServices.IsNullable(type))
				type = TypeSystemServices.GetNullableUnderlyingType(type);

			return TypeSystemServices.IsNumber(type) || TypeSystemServices.IsDuckType(type);
		}

		void LeaveIncrementDecrement(UnaryExpression node)
		{
			if (AssertLValue(node.Operand))
			{
				if (!IsValidIncrementDecrementOperand(node.Operand))
				{
					InvalidOperatorForType(node);
				}
				else
				{
					ExpandIncrementDecrement(node);
				}
			}
			else
			{
				Error(node);
			}
		}

		void ExpandIncrementDecrement(UnaryExpression node)
		{
			Node expansion = null;
			if (IsArraySlicing(node.Operand))
			{
				expansion = ExpandIncrementDecrementArraySlicing(node);
			}
			else
			{
				expansion = ExpandSimpleIncrementDecrement(node);
			}
			node.ParentNode.Replace(node, expansion);
			Visit(expansion);
		}

		Expression ExpandIncrementDecrementArraySlicing(UnaryExpression node)
		{
			SlicingExpression slicing = (SlicingExpression)node.Operand;
			CheckNoComplexSlicing(slicing);
			Visit(slicing);
			return CreateSideEffectAwareSlicingOperation(
				node.LexicalInfo,
				GetEquivalentBinaryOperator(node.Operator),
				slicing,
				CodeBuilder.CreateIntegerLiteral(1),
				DeclareOldValueTempIfNeeded(node));
		}

		private Expression CreateSideEffectAwareSlicingOperation(LexicalInfo lexicalInfo, BinaryOperatorType binaryOperator, SlicingExpression lvalue, Expression rvalue, InternalLocal returnValue)
		{
			MethodInvocationExpression eval = CodeBuilder.CreateEvalInvocation(lexicalInfo);
			if (HasSideEffect(lvalue.Target))
			{
				InternalLocal temp = AddInitializedTempLocal(eval, lvalue.Target);
				lvalue.Target = CodeBuilder.CreateReference(temp);
			}

			foreach (Slice slice in lvalue.Indices)
			{
				Expression index = slice.Begin;
				if (HasSideEffect(index))
				{
					InternalLocal temp = AddInitializedTempLocal(eval, index);
					slice.Begin = CodeBuilder.CreateReference(temp);
				}
			}

			BinaryExpression addition = CodeBuilder.CreateBoundBinaryExpression(
				GetExpressionType(lvalue),
				binaryOperator,
				CloneOrAssignToTemp(returnValue, lvalue),
				rvalue);
			Expression expansion = CodeBuilder.CreateAssignment(
				lvalue.CloneNode(),
				addition);
			// Resolve operator overloads if any
			BindArithmeticOperator(addition);
			if (eval.Arguments.Count > 0 || null != returnValue)
			{
				eval.Arguments.Add(expansion);
				if (null != returnValue)
				{
					eval.Arguments.Add(CodeBuilder.CreateReference(returnValue));
				}
				BindExpressionType(eval, GetExpressionType(lvalue));
				expansion = eval;
			}
			return expansion;
		}

		InternalLocal AddInitializedTempLocal(MethodInvocationExpression eval, Expression initializer)
		{
			InternalLocal temp = DeclareTempLocal(GetExpressionType(initializer));
			eval.Arguments.Add(
				CodeBuilder.CreateAssignment(
					CodeBuilder.CreateReference(temp),
					initializer));
			return temp;
		}

		InternalLocal DeclareOldValueTempIfNeeded(UnaryExpression node)
		{
			return AstUtil.IsPostUnaryOperator(node.Operator)
				? DeclareTempLocal(GetExpressionType(node.Operand))
				: null;
		}

		Expression ExpandSimpleIncrementDecrement(UnaryExpression node)
		{
			InternalLocal oldValue = DeclareOldValueTempIfNeeded(node);
			IType type = GetExpressionType(node.Operand);

			BinaryExpression addition = CodeBuilder.CreateBoundBinaryExpression(
				type,
				GetEquivalentBinaryOperator(node.Operator),
				CloneOrAssignToTemp(oldValue, node.Operand),
				CodeBuilder.CreateIntegerLiteral(1));

			BinaryExpression assign = CodeBuilder.CreateAssignment(
				node.LexicalInfo,
				node.Operand,
				addition);

			// Resolve operator overloads if any
			BindArithmeticOperator(addition);

			return null == oldValue
				? (Expression) assign
				: CodeBuilder.CreateEvalInvocation(
					node.LexicalInfo,
					assign,
					CodeBuilder.CreateReference(oldValue));
		}

		Expression CloneOrAssignToTemp(InternalLocal temp, Expression operand)
		{
			return null == temp
				? operand.CloneNode()
				: CodeBuilder.CreateAssignment(
					CodeBuilder.CreateReference(temp),
					operand.CloneNode());
		}

		BinaryOperatorType GetEquivalentBinaryOperator(UnaryOperatorType op)
		{
			return op == UnaryOperatorType.Increment || op == UnaryOperatorType.PostIncrement
				? BinaryOperatorType.Addition
				: BinaryOperatorType.Subtraction;
		}

		UnaryOperatorType GetRelatedPreOperator(UnaryOperatorType op)
		{
			switch (op)
			{
				case UnaryOperatorType.PostIncrement:
					{
						return UnaryOperatorType.Increment;
					}
				case UnaryOperatorType.PostDecrement:
					{
						return UnaryOperatorType.Decrement;
					}
			}
			throw new ArgumentException("op");
		}

		override public bool EnterUnaryExpression(UnaryExpression node)
		{
			if (AstUtil.IsPostUnaryOperator(node.Operator))
			{
				if (NodeType.ExpressionStatement == node.ParentNode.NodeType)
				{
					// nothing to do, a post operator inside a statement
					// behaves just like its equivalent pre operator
					node.Operator = GetRelatedPreOperator(node.Operator);
				}
			}
			return true;
		}

		override public void LeaveUnaryExpression(UnaryExpression node)
		{
			switch (node.Operator)
			{
				case UnaryOperatorType.Explode:
					{
						LeaveExplodeExpression(node);
						break;
					}
				case UnaryOperatorType.LogicalNot:
					{
						LeaveLogicalNot(node);
						break;
					}

				case UnaryOperatorType.Increment:
				case UnaryOperatorType.PostIncrement:
				case UnaryOperatorType.Decrement:
				case UnaryOperatorType.PostDecrement:
					{
						LeaveIncrementDecrement(node);
						break;
					}

				case UnaryOperatorType.UnaryNegation:
					{
						LeaveUnaryNegation(node);
						break;
					}

				case UnaryOperatorType.OnesComplement:
					{
						LeaveOnesComplement(node);
						break;
					}

				case UnaryOperatorType.AddressOf:
					{
						LeaveAddressOf(node);
						break;
					}

				case UnaryOperatorType.Indirection:
					{
						LeaveIndirection(node);
						break;
					}

				default:
					{
						NotImplemented(node, "unary operator not supported");
						break;
					}
			}
		}

		private void LeaveOnesComplement(UnaryExpression node)
		{
			if (IsPrimitiveOnesComplementOperand(node.Operand))
			{
				BindExpressionType(node, GetExpressionType(node.Operand));
			}
			else
			{
				ProcessOperatorOverload(node);
			}
		}

		private bool IsPrimitiveOnesComplementOperand(Expression operand)
		{
			IType type = GetExpressionType(operand);
			return TypeSystemServices.IsIntegerNumber(type) || type.IsEnum;
		}

		private void LeaveLogicalNot(UnaryExpression node)
		{
			node.Operand = AssertBoolContext(node.Operand);
			BindExpressionType(node, TypeSystemServices.BoolType);
		}

		private void LeaveUnaryNegation(UnaryExpression node)
		{
			if (IsPrimitiveNumber(node.Operand))
			{
				BindExpressionType(node, GetExpressionType(node.Operand));
			}
			else
			{
				ProcessOperatorOverload(node);
			}
		}

		private void LeaveAddressOf(UnaryExpression node)
		{
			IType dataType = GetExpressionType(node.Operand);
			if (dataType.IsArray) //if array reference take address of first element
			{
				dataType = dataType.GetElementType();
				node.Replace(node.Operand, new SlicingExpression(node.Operand, new IntegerLiteralExpression(0)));
				BindExpressionType(node.Operand, dataType);
			}
			if (TypeSystemServices.IsPointerCompatible(dataType))
			{
				node.Entity = dataType.MakePointerType();
				BindExpressionType(node, dataType.MakePointerType());
				return;
			}

			BindExpressionType(node, TypeSystemServices.ErrorEntity);
			Error(CompilerErrorFactory.PointerIncompatibleType(node.Operand, dataType));
		}

		private void LeaveIndirection(UnaryExpression node)
		{
			if (TypeSystemServices.IsError(node.Operand))
				return;

			IType dataType = GetExpressionType(node.Operand).GetElementType();
			if (null != dataType && TypeSystemServices.IsPointerCompatible(dataType))
			{
				node.Entity = node.Operand.Entity;
				BindExpressionType(node, dataType);
				return;
			}

			BindExpressionType(node, TypeSystemServices.ErrorEntity);
			Error(CompilerErrorFactory.PointerIncompatibleType(node.Operand, dataType));
		}

		private void ProcessOperatorOverload(UnaryExpression node)
		{
			if (! ResolveOperator(node))
			{
				InvalidOperatorForType(node);
			}
		}

		override public bool EnterBinaryExpression(BinaryExpression node)
		{
			if (BinaryOperatorType.Assign == node.Operator)
			{
				if (NodeType.ReferenceExpression == node.Left.NodeType &&
					null == node.Left.Entity)
				{
					// Auto local declaration:
					// assign to unknown reference implies local
					// declaration
					ReferenceExpression reference = (ReferenceExpression)node.Left;
					IEntity info = NameResolutionService.Resolve(reference.Name);
					if (null == info || TypeSystemServices.IsBuiltin(info) || IsInaccessible(info))
					{
						Visit(node.Right);
						IType expressionType = MapNullToObject(GetConcreteExpressionType(node.Right));
						IEntity local = DeclareLocal(reference, reference.Name, expressionType);
						reference.Entity = local;
						BindExpressionType(node.Left, expressionType);
						BindExpressionType(node, expressionType);
						return false;
					}
				}
			}
			return true;
		}

		bool IsInaccessible(IEntity info)
		{
			IAccessibleMember accessible = info as IAccessibleMember;
			if (accessible != null && accessible.IsPrivate
				&& accessible.DeclaringType != CurrentType)
			{
				return true;
			}
			return false;
		}

		override public void LeaveBinaryExpression(BinaryExpression node)
		{
			if (TypeSystemServices.IsUnknown(node.Left) ||
				TypeSystemServices.IsUnknown(node.Right))
			{
				BindExpressionType(node, Unknown.Default);
				return;
			}

			if (TypeSystemServices.IsError(node.Left)
				|| TypeSystemServices.IsError(node.Right))
			{
				Error(node);
				return;
			}
			BindBinaryExpression(node);
		}

		protected virtual void BindBinaryExpression(BinaryExpression node)
		{
			if (IsEnumOperation(node))
			{
				BindEnumOperation(node);
				return;
			}

			switch (node.Operator)
			{
				case BinaryOperatorType.Assign:
					{
						BindAssignment(node);
						break;
					}

				case BinaryOperatorType.Addition:
					{
						if (GetExpressionType(node.Left).IsArray &&
							GetExpressionType(node.Right).IsArray)
						{
							BindArrayAddition(node);
						}
						else
						{
							BindArithmeticOperator(node);
						}
						break;
					}

				case BinaryOperatorType.Subtraction:
				case BinaryOperatorType.Multiply:
				case BinaryOperatorType.Division:
				case BinaryOperatorType.Modulus:
				case BinaryOperatorType.Exponentiation:
					{
						BindArithmeticOperator(node);
						break;
					}

				case BinaryOperatorType.TypeTest:
					{
						BindTypeTest(node);
						break;
					}

				case BinaryOperatorType.ReferenceEquality:
					{
						BindReferenceEquality(node);
						break;
					}

				case BinaryOperatorType.ReferenceInequality:
					{
						BindReferenceEquality(node, true);
						break;
					}

				case BinaryOperatorType.Or:
				case BinaryOperatorType.And:
					{
						BindLogicalOperator(node);
						break;
					}

				case BinaryOperatorType.BitwiseAnd:
				case BinaryOperatorType.BitwiseOr:
				case BinaryOperatorType.ExclusiveOr:
				case BinaryOperatorType.ShiftLeft:
				case BinaryOperatorType.ShiftRight:
					{
						BindBitwiseOperator(node);
						break;
					}

				case BinaryOperatorType.InPlaceSubtraction:
				case BinaryOperatorType.InPlaceAddition:
					{
						BindInPlaceAddSubtract(node);
						break;
					}

				case BinaryOperatorType.InPlaceShiftLeft:
				case BinaryOperatorType.InPlaceShiftRight:
				case BinaryOperatorType.InPlaceDivision:
				case BinaryOperatorType.InPlaceMultiply:
				case BinaryOperatorType.InPlaceModulus:
				case BinaryOperatorType.InPlaceBitwiseOr:
				case BinaryOperatorType.InPlaceBitwiseAnd:
				case BinaryOperatorType.InPlaceExclusiveOr:
				{
					BindInPlaceArithmeticOperator(node);
					break;
				}

				case BinaryOperatorType.GreaterThan:
				case BinaryOperatorType.GreaterThanOrEqual:
				case BinaryOperatorType.LessThan:
				case BinaryOperatorType.LessThanOrEqual:
				case BinaryOperatorType.Inequality:
				case BinaryOperatorType.Equality:
					{
						BindCmpOperator(node);
						break;
					}

				default:
					{
						if (!ResolveOperator(node))
						{
							InvalidOperatorForTypes(node);
						}
						break;
					}
			}
		}

		IType GetMostGenericType(BinaryExpression node)
		{
			return GetMostGenericType(
				GetExpressionType(node.Left),
				GetExpressionType(node.Right));
		}

		bool IsNullableOperation(BinaryExpression node)
		{
			if (null == node.Left.ExpressionType || null == node.Right.ExpressionType)
			{
				return false;
			}

			return TypeSystemServices.IsNullable(GetExpressionType(node.Left))
				|| TypeSystemServices.IsNullable(GetExpressionType(node.Right));
		}

		bool IsEnumOperation(BinaryExpression node)
		{
			switch (node.Operator)
			{
				case BinaryOperatorType.Addition:
				case BinaryOperatorType.Subtraction:
				case BinaryOperatorType.BitwiseAnd:
				case BinaryOperatorType.BitwiseOr:
				case BinaryOperatorType.ExclusiveOr:
					IType lhs = GetExpressionType(node.Left);
					IType rhs = GetExpressionType(node.Right);
					if (lhs.IsEnum) return IsValidEnumOperand(lhs, rhs);
					if (rhs.IsEnum) return IsValidEnumOperand(rhs, lhs);
					break;
			}
			return false;
		}

		bool IsValidEnumOperand(IType expected, IType actual)
		{
			if (expected == actual) return true;
			if (actual.IsEnum) return true;
			return TypeSystemServices.IsIntegerNumber(actual);
		}

		void BindEnumOperation(BinaryExpression node)
		{
			IType lhs = GetExpressionType(node.Left);
			IType rhs = GetExpressionType(node.Right);
			switch(node.Operator)
			{
				case BinaryOperatorType.Addition:
					if (lhs.IsEnum != rhs.IsEnum)
					{
						BindExpressionType(node, lhs.IsEnum ? lhs : rhs);
						return;
					}
					break;
				case BinaryOperatorType.Subtraction:
					if (lhs == rhs)
					{
						BindExpressionType(node, TypeSystemServices.IntType);
						return;
					}
					else if (lhs.IsEnum && !rhs.IsEnum)
					{
						BindExpressionType(node, lhs);
						return;
					}
					break;
				case BinaryOperatorType.BitwiseAnd:
				case BinaryOperatorType.BitwiseOr:
				case BinaryOperatorType.ExclusiveOr:
					if (lhs == rhs)
					{
						BindExpressionType(node, lhs);
						return;
					}
					break;
			}
			if (!ResolveOperator(node))
			{
				InvalidOperatorForTypes(node);
			}
		}

		void BindBitwiseOperator(BinaryExpression node)
		{
			IType lhs = GetExpressionType(node.Left);
			IType rhs = GetExpressionType(node.Right);

			if (TypeSystemServices.IsIntegerOrBool(lhs) &&
				TypeSystemServices.IsIntegerOrBool(rhs))
			{
				IType type;
				switch (node.Operator)
				{
					case BinaryOperatorType.ShiftLeft:
					case BinaryOperatorType.ShiftRight:
						type = lhs;
						break;
					default:
						type = TypeSystemServices.GetPromotedNumberType(lhs, rhs);
						break;
				}
				BindExpressionType(node, type);
			}
			else
			{
//				if (lhs.IsEnum && rhs == lhs)
//				{
//					BindExpressionType(node, lhs);
//				}
//				else
//				{
					if (!ResolveOperator(node))
					{
						InvalidOperatorForTypes(node);
					}
//				}
			}
		}

		bool IsChar(IType type)
		{
			return TypeSystemServices.CharType == type;
		}

		void BindCmpOperator(BinaryExpression node)
		{
			if (BindNullableComparison(node))
			{
				return;
			}

			IType lhs = GetExpressionType(node.Left);
			IType rhs = GetExpressionType(node.Right);

			if (IsPrimitiveComparison(lhs, rhs))
			{
				BindExpressionType(node, TypeSystemServices.BoolType);
				return;
			}

			if (lhs.IsEnum || rhs.IsEnum)
			{
				if (lhs == rhs
				    || TypeSystemServices.IsPrimitiveNumber(rhs)
				    || TypeSystemServices.IsPrimitiveNumber(lhs))
				{
					BindExpressionType(node, TypeSystemServices.BoolType);
				}
				else
				{
					if (!ResolveOperator(node))
					{
						InvalidOperatorForTypes(node);
					}
				}
				return;
			}

			if (!ResolveOperator(node))
			{
				switch (node.Operator)
				{
					case BinaryOperatorType.Equality:
						{
							if (OptimizeNullComparisons
								&& (IsNull(node.Left) || IsNull(node.Right)))
							{
								node.Operator = BinaryOperatorType.ReferenceEquality;
								BindReferenceEquality(node);
								break;
							}
							Expression expression = CreateEquals(node);
							node.ParentNode.Replace(node, expression);
							break;
						}

					case BinaryOperatorType.Inequality:
						{
							if (OptimizeNullComparisons
								&& (IsNull(node.Left) || IsNull(node.Right)))
							{
								node.Operator = BinaryOperatorType.ReferenceInequality;
								BindReferenceEquality(node, true);
								break;
							}
							Expression expression = CreateEquals(node);
							Node parent = node.ParentNode;
							parent.Replace(node, CodeBuilder.CreateNotExpression(expression));
							break;
						}

					default:
						{
							InvalidOperatorForTypes(node);
							break;
						}
				}
			}
		}

		private bool IsPrimitiveComparison(IType lhs, IType rhs)
		{
			if (IsPrimitiveNumberOrChar(lhs) && IsPrimitiveNumberOrChar(rhs)) return true;
			if (IsBool(lhs) && IsBool(rhs)) return true;
			return false;
		}

		private bool IsPrimitiveNumberOrChar(IType lhs)
		{
			return TypeSystemServices.IsPrimitiveNumber(lhs) || IsChar(lhs);
		}

		private bool IsBool(IType lhs)
		{
			return TypeSystemServices.BoolType == lhs;
		}

		private static bool IsNull(Expression node)
		{
			return NodeType.NullLiteralExpression == node.NodeType;
		}

		void BindLogicalOperator(BinaryExpression node)
		{
			AssertBoolContext(node.Left);
			AssertBoolContext(node.Right);
			BindExpressionType(node, GetMostGenericType(node));
		}

		void BindInPlaceAddSubtract(BinaryExpression node)
		{
			IEntity entity = node.Left.Entity;
			if (null != entity &&
				(EntityType.Event == entity.EntityType
				 || EntityType.Ambiguous == entity.EntityType))
			{
				BindEventSubscription(node);
			}
			else
			{
				BindInPlaceArithmeticOperator(node);
			}
		}

		void BindEventSubscription(BinaryExpression node)
		{
			IEntity tag = GetEntity(node.Left);
			if (EntityType.Event != tag.EntityType)
			{
				if (EntityType.Ambiguous == tag.EntityType)
				{
					IList found = ((Ambiguous)tag).Select(IsPublicEvent);
					if (found.Count != 1)
					{
						tag = null;
					}
					else
					{
						tag = (IEntity)found[0];
						Bind(node.Left, tag);
					}
				}
			}

			IEvent eventInfo = (IEvent)tag;
			IType rtype = GetExpressionType(node.Right);
			if (!AssertDelegateArgument(node, eventInfo, rtype))
			{
				Error(node);
				return;
			}

			IMethod method = null;
			if (node.Operator == BinaryOperatorType.InPlaceAddition)
			{
				method = eventInfo.GetAddMethod();
			}
			else
			{
				method = eventInfo.GetRemoveMethod();
				CallableSignature expected = GetCallableSignature(eventInfo.Type);
				CallableSignature actual = GetCallableSignature(node.Right);
				if (expected != actual)
				{
					Warnings.Add(
						CompilerWarningFactory.InvalidEventUnsubscribe(
							node,
							eventInfo.FullName,
							expected));
				}
			}

			MethodInvocationExpression mie = CodeBuilder.CreateMethodInvocation(
				((MemberReferenceExpression)node.Left).Target,
				method,
				node.Right);
			node.ParentNode.Replace(node, mie);
		}

		CallableSignature GetCallableSignature(Expression node)
		{
			return GetCallableSignature(GetExpressionType(node));
		}

		CallableSignature GetCallableSignature(IType type)
		{
			return ((ICallableType)type).GetSignature();
		}

		virtual protected void ProcessBuiltinInvocation(BuiltinFunction function, MethodInvocationExpression node)
		{
			switch (function.FunctionType)
			{
				case BuiltinFunctionType.Len:
					{
						ProcessLenInvocation(node);
						break;
					}

				case BuiltinFunctionType.AddressOf:
					{
						ProcessAddressOfInvocation(node);
						break;
					}

				case BuiltinFunctionType.Eval:
					{
						ProcessEvalInvocation(node);
						break;
					}

				default:
					{
						NotImplemented(node, "BuiltinFunction: " + function);
						break;
					}
			}
		}

		bool ProcessSwitchInvocation(MethodInvocationExpression node)
		{
			if (BuiltinFunction.Switch != node.Target.Entity) return false;
			BindSwitchLabelReferences(node);
			if (CheckSwitchArguments(node)) return true;
			Error(node, CompilerErrorFactory.InvalidSwitch(node.Target));
			return true;
		}

		private static void BindSwitchLabelReferences(MethodInvocationExpression node)
		{
			for (int i = 1; i < node.Arguments.Count; ++i)
			{
				ReferenceExpression label = (ReferenceExpression)node.Arguments[i];
				label.ExpressionType = Unknown.Default;
			}
		}

		bool CheckSwitchArguments(MethodInvocationExpression node)
		{
			ExpressionCollection args = node.Arguments;
			if (args.Count > 1)
			{
				Visit(args[0]);
				if (TypeSystemServices.IsIntegerNumber(GetExpressionType(args[0])))
				{
					for (int i=1; i<args.Count; ++i)
					{
						if (NodeType.ReferenceExpression != args[i].NodeType)
						{
							return false;
						}
					}
					return true;
				}
			}
			return false;
		}

		void ProcessEvalInvocation(MethodInvocationExpression node)
		{
			if (node.Arguments.Count > 0)
			{
				int allButLast = node.Arguments.Count-1;
				for (int i=0; i<allButLast; ++i)
				{
					AssertHasSideEffect(node.Arguments[i]);
				}
				BindExpressionType(node, GetConcreteExpressionType(node.Arguments.Last));
			}
			else
			{
				BindExpressionType(node, TypeSystemServices.VoidType);
			}
		}

		void ProcessAddressOfInvocation(MethodInvocationExpression node)
		{
			if (node.Arguments.Count != 1)
			{
				Error(node, CompilerErrorFactory.MethodArgumentCount(node.Target, "__addressof__", 1));
			}
			else
			{
				Expression arg = node.Arguments[0];

				EntityType type = GetEntity(arg).EntityType;

				if (EntityType.Method != type)
				{
					ReferenceExpression reference = arg as ReferenceExpression;
					if (null != reference && EntityType.Ambiguous == type)
					{
						Error(node, CompilerErrorFactory.AmbiguousReference(arg, reference.Name, ((Ambiguous)arg.Entity).Entities));
					}
					else
					{
						Error(node, CompilerErrorFactory.MethodReferenceExpected(arg));
					}
				}
				else
				{
					BindExpressionType(node, TypeSystemServices.IntPtrType);
				}
			}
		}

		void ProcessLenInvocation(MethodInvocationExpression node)
		{
			if ((node.Arguments.Count < 1) || (node.Arguments.Count > 2))
			{
				Error(node, CompilerErrorFactory.MethodArgumentCount(node.Target, "len", node.Arguments.Count));
				return;
			}

			Expression resultingNode = null;

			Expression target = node.Arguments[0];
			IType type = GetExpressionType(target);
			bool isArray = TypeSystemServices.ArrayType.IsAssignableFrom(type);

			if ((!isArray) && (node.Arguments.Count != 1))
			{
				Error(node, CompilerErrorFactory.MethodArgumentCount(node.Target, "len", node.Arguments.Count));
			}
			if (TypeSystemServices.IsSystemObject(type))
			{
				resultingNode = CodeBuilder.CreateMethodInvocation(RuntimeServices_Len, target);
			}
			else if (TypeSystemServices.StringType == type)
			{
				resultingNode = CodeBuilder.CreateMethodInvocation(target, String_get_Length);
			}
			else if (isArray)
			{
				if (node.Arguments.Count == 1)
				{
					resultingNode = CodeBuilder.CreateMethodInvocation(target, Array_get_Length);
				}
				else
				{
					resultingNode = CodeBuilder.CreateMethodInvocation(target,
																	   Array_GetLength, node.Arguments[1]);
				}
			}
			else if (TypeSystemServices.ICollectionType.IsAssignableFrom(type))
			{
				resultingNode = CodeBuilder.CreateMethodInvocation(target, ICollection_get_Count);
			}
			else if (GenericsServices.HasConstructedType(type, TypeSystemServices.ICollectionGenericType))
			{
				resultingNode = new MemberReferenceExpression(node.LexicalInfo, target, "Count");
				Visit(resultingNode);
			}
			else
			{
				Error(CompilerErrorFactory.InvalidLen(target, type.ToString()));
			}

			if (null != resultingNode)
			{
				node.ParentNode.Replace(node, resultingNode);
			}
		}

		void CheckListLiteralArgumentInArrayConstructor(IType expectedElementType, MethodInvocationExpression constructor)
		{
			ListLiteralExpression elements = constructor.Arguments[1] as ListLiteralExpression;
			if (null == elements) return;
			CheckItems(expectedElementType, elements.Items);
		}

		private void CheckItems(IType expectedElementType, ExpressionCollection items)
		{
			foreach (Expression element in items)
			{
				AssertTypeCompatibility(element, expectedElementType, GetExpressionType(element));
			}
		}

		void ApplyBuiltinMethodTypeInference(MethodInvocationExpression expression, IMethod method)
		{
			IType inferredType = null;

			if (Array_TypedEnumerableConstructor == method ||
				Array_TypedCollectionConstructor == method ||
				Array_TypedIntConstructor == method)
			{
				IType type = TypeSystemServices.GetReferencedType(expression.Arguments[0]);
				if (null != type)
				{
					if (Array_TypedCollectionConstructor == method)
					{
						CheckListLiteralArgumentInArrayConstructor(type,  expression);
					}
					inferredType = type.MakeArrayType(1);
				}
			}
			else if (MultiDimensionalArray_TypedConstructor == method)
			{
				IType type = TypeSystemServices.GetReferencedType(expression.Arguments[0]);
				if (null != type)
				{
					inferredType = type.MakeArrayType(expression.Arguments.Count-1);
				}
			}
			else if (Array_EnumerableConstructor == method)
			{
				IType enumeratorItemType = GetEnumeratorItemType(GetExpressionType(expression.Arguments[0]));
				if (TypeSystemServices.ObjectType != enumeratorItemType)
				{
					inferredType = enumeratorItemType.MakeArrayType(1);
					expression.Target.Entity = Array_TypedEnumerableConstructor;
					expression.ExpressionType = Array_TypedEnumerableConstructor.ReturnType;
					expression.Arguments.Insert(0, CodeBuilder.CreateReference(enumeratorItemType));
				}
			}

			if (null != inferredType)
			{
				Node parent = expression.ParentNode;
				parent.Replace(expression,
							   CodeBuilder.CreateCast(inferredType, expression));
			}
		}

		protected virtual IEntity ResolveAmbiguousMethodInvocation(MethodInvocationExpression node, IEntity entity)
		{
			Ambiguous ambiguous = entity as Ambiguous;
			if (ambiguous == null)
				return entity;

			_context.TraceVerbose("{0}: resolving ambigous method invocation: {1}", node.LexicalInfo, entity);

			IEntity resolved = ResolveCallableReference(node, ambiguous);
			if (null != resolved)
				return resolved;

			// If resolution fails, try to resolve target as an extension method (but no more than once)
			if (!ResolvedAsExtension(node) && TryToProcessAsExtensionInvocation(node))
			{
				return null;
			}

			return CantResolveAmbiguousMethodInvocation(node, ambiguous.Entities);
		}

		private IEntity ResolveCallableReference(MethodInvocationExpression node, Ambiguous entity)
		{
			IEntity resolved = CallableResolutionService.ResolveCallableReference(node.Arguments, entity.Entities);
			if (null == resolved)
				return null;

			IMember member = (IMember)resolved;
			if (NodeType.ReferenceExpression == node.Target.NodeType)
			{
				ResolveMemberInfo((ReferenceExpression)node.Target, member);
			}
			else
			{
				Bind(node.Target, member);
				BindExpressionType(node.Target, member.Type);
			}
			return resolved;
		}

		private bool TryToProcessAsExtensionInvocation(MethodInvocationExpression node)
		{
			IEntity extension = ResolveExtension(node);
			if (null == extension) return false;

			node.Annotate(ResolvedAsExtensionAnnotation);

			ProcessMethodInvocationExpression(node, extension);
			return true;
		}

		private IEntity ResolveExtension(MethodInvocationExpression node)
		{
			ReferenceExpression targetReference = node.Target as ReferenceExpression;
			if (targetReference == null) return null;

			MemberReferenceExpression mre = targetReference as MemberReferenceExpression;
			INamespace extensionNamespace = (mre != null) ? GetReferenceNamespace(mre) : CurrentType;

			return NameResolutionService.ResolveExtension(extensionNamespace, targetReference.Name);
		}

		private bool ResolvedAsExtension(MethodInvocationExpression node)
		{
			return
				node.ContainsAnnotation(ResolvedAsExtensionAnnotation) ||
				node.Target.ContainsAnnotation(ResolvedAsExtensionAnnotation);
		}

		protected virtual IEntity CantResolveAmbiguousMethodInvocation(MethodInvocationExpression node, IEntity[] entities)
		{
			EmitCallableResolutionError(node, entities, node.Arguments);
			Error(node);
			return null;
		}

		override public void OnMethodInvocationExpression(MethodInvocationExpression node)
		{
			if (null != node.ExpressionType)
			{
				_context.TraceVerbose("{0}: Method invocation already bound.", node.LexicalInfo);
				return;
			}

			Visit(node.Target);

			if (ProcessSwitchInvocation(node)) return;
			if (ProcessMetaMethodInvocation(node)) return;

			Visit(node.Arguments);

			if (TypeSystemServices.IsError(node.Target)
				|| TypeSystemServices.IsErrorAny(node.Arguments))
			{
				Error(node);
				return;
			}

			IEntity targetEntity = node.Target.Entity;
			if (null == targetEntity)
			{
				ProcessGenericMethodInvocation(node);
				return;
			}

			ProcessMethodInvocationExpression(node, targetEntity);
		}

		private bool ProcessMetaMethodInvocation(MethodInvocationExpression node)
		{
			IEntity targetEntity = node.Target.Entity;
			if (null == targetEntity) return false;
			if (!IsOrContainMetaMethod(targetEntity)) return false;

			object[] arguments = GetMetaMethodInvocationArguments(node);
			Type[] argumentTypes = MethodResolver.GetArgumentTypes(arguments);
			MethodResolver resolver = new MethodResolver(argumentTypes);
			CandidateMethod method = resolver.ResolveMethod(EnumerateMetaMethods(targetEntity));
			if (null == method) return false;

			// TODO: cache emitted dispatchers
			MethodDispatcherEmitter emitter = new MethodDispatcherEmitter(method, argumentTypes);
			Node replacement = (Node)emitter.Emit()(null, arguments);
			ReplaceMetaMethodInvocationSite(node, replacement);

			return true;
		}

		private static object[] GetMetaMethodInvocationArguments(MethodInvocationExpression node)
		{
			if (node.NamedArguments.Count == 0) return node.Arguments.ToArray();

			List arguments = new List();
			arguments.Add(node.NamedArguments.ToArray());
			arguments.Extend(node.Arguments);
			return arguments.ToArray();
		}

		private void ReplaceMetaMethodInvocationSite(MethodInvocationExpression node, Node replacement)
		{
			if (replacement == null || replacement is Statement)
			{
				if (node.ParentNode.NodeType != NodeType.ExpressionStatement)
				{
					NotImplemented(node, "Cant use an statement where an expression is expected.");
				}
				Node statementParent = node.ParentNode.ParentNode;
				statementParent.Replace(node.ParentNode, replacement);
			}
			else
			{
				node.ParentNode.Replace(node, replacement);
			}
			Visit(replacement);
		}

		private System.Collections.Generic.IEnumerable<System.Reflection.MethodInfo> EnumerateMetaMethods(IEntity entity)
		{
			if (entity.EntityType == EntityType.Method)
			{
				yield return GetMethodInfo(entity);
			}
			else
			{
				foreach (IEntity item in ((Ambiguous)entity).Entities)
				{
					yield return GetMethodInfo(item);
				}
			}
		}

		private static MethodInfo GetMethodInfo(IEntity entity)
		{
			return (MethodInfo)((ExternalMethod) entity).MethodInfo;
		}

		private bool IsOrContainMetaMethod(IEntity entity)
		{
			switch (entity.EntityType)
			{
				case EntityType.Ambiguous:
					return ((Ambiguous) entity).Any(IsMetaMethod);
				case EntityType.Method:
					return IsMetaMethod(entity);
			}
			return false;
		}

		private static bool IsMetaMethod(IEntity entity)
		{
			ExternalMethod m = entity as ExternalMethod;
			if (m == null) return false;
			return m.IsMeta;
		}

		private void ProcessMethodInvocationExpression(MethodInvocationExpression node, IEntity targetEntity)
		{
			if (ResolvedAsExtension(node) || IsExtensionMethod(targetEntity))
			{
				PreNormalizeExtensionInvocation(node, targetEntity as IEntityWithParameters);
			}

			targetEntity = ResolveAmbiguousMethodInvocation(node, targetEntity);
			if (targetEntity == null)
				return;

			switch (targetEntity.EntityType)
			{
				case EntityType.BuiltinFunction:
					{
						ProcessBuiltinInvocation((BuiltinFunction)targetEntity, node);
						break;
					}
				case EntityType.Event:
					{
						ProcessEventInvocation((IEvent)targetEntity, node);
						break;
					}

				case EntityType.Method:
					{
						ProcessMethodInvocation(node, targetEntity);
						break;
					}

				case EntityType.Constructor:
					{
						ProcessConstructorInvocation(node, targetEntity);
						break;
					}

				case EntityType.Type:
					{
						ProcessTypeInvocation(node);
						break;
					}

				case EntityType.Error:
					{
						Error(node);
						break;
					}

				default:
					{
						ProcessGenericMethodInvocation(node);
						break;
					}
			}
		}

		private void ProcessConstructorInvocation(MethodInvocationExpression node, IEntity targetEntity)
		{
			NamedArgumentsNotAllowed(node);
			InternalConstructor constructorInfo = targetEntity as InternalConstructor;
			if (null == constructorInfo) return;

			IType targetType = null;
			if (NodeType.SuperLiteralExpression == node.Target.NodeType)
			{
				constructorInfo.HasSuperCall = true;
				targetType = constructorInfo.DeclaringType.BaseType;
			}
			else if (node.Target.NodeType == NodeType.SelfLiteralExpression)
			{
				constructorInfo.HasSelfCall = true;
				targetType = constructorInfo.DeclaringType;
			}

			IConstructor targetConstructorInfo = GetCorrectConstructor(node, targetType, node.Arguments);
			if (null != targetConstructorInfo)
			{
				Bind(node.Target, targetConstructorInfo);
			}
		}

		protected virtual bool ProcessMethodInvocationWithInvalidParameters(MethodInvocationExpression node, IMethod targetMethod)
		{
			return false;
		}

		protected virtual void ProcessMethodInvocation(MethodInvocationExpression node, IEntity targetEntity)
		{
			IMethod targetMethod = (IMethod)targetEntity;

			if (ResolvedAsExtension(node))
			{
				PostNormalizeExtensionInvocation(node, targetMethod);
			}

			targetMethod = InferGenericMethodInvocation(node, targetMethod);
			if (targetMethod == null) return;

			if (!CheckParameters(targetMethod.CallableType, node.Arguments, false))
			{
				if (!ResolvedAsExtension(node) && TryToProcessAsExtensionInvocation(node)) return;

				if (ProcessMethodInvocationWithInvalidParameters(node, targetMethod)) return;

				AssertParameters(node, targetMethod, node.Arguments);
			}

			AssertTargetContext(node.Target, targetMethod);
			NamedArgumentsNotAllowed(node);

			EnsureRelatedNodeWasVisited(node.Target, targetMethod);
			BindExpressionType(node, GetInferredType(targetMethod));
			ApplyBuiltinMethodTypeInference(node, targetMethod);
		}

		private IMethod InferGenericMethodInvocation(MethodInvocationExpression node, IMethod targetMethod)
		{
			if (targetMethod.GenericInfo == null) return targetMethod;

			GenericParameterInferrer inferrer = new GenericParameterInferrer(Context, targetMethod, node.Arguments);
			inferrer.ResolveClosure += ProcessClosureInMethodInvocation;
			if (!inferrer.Run())
			{
				Error(node, CompilerErrorFactory.CannotInferGenericMethodArguments(node, targetMethod));
				return null;
			}

			IType[] inferredTypeArguments = inferrer.GetInferredTypes();
			if (!Context.GetService<GenericsServices>().CheckGenericConstruction(node, targetMethod, inferredTypeArguments, true))
			{
				Error(node);
				return null;
			}

			IMethod constructedMethod = targetMethod.GenericInfo.ConstructMethod(inferredTypeArguments);
			Bind(node.Target, constructedMethod);
			BindExpressionType(node, GetInferredType(constructedMethod));

			return constructedMethod;
		}

		private bool IsAccessible(IEntity member)
		{
			IAccessibleMember accessible = member as IAccessibleMember;
			if (accessible == null) return true;
			return GetAccessibilityChecker().IsAccessible(accessible);
		}

		private IAccessibilityChecker GetAccessibilityChecker()
		{
			if (null == _currentMethod) return AccessibilityChecker.Global;
			return new AccessibilityChecker(CurrentTypeDefinition);
		}

		private TypeDefinition CurrentTypeDefinition
		{
			get { return _currentMethod.Method.DeclaringType; }
		}

		private void NamedArgumentsNotAllowed(MethodInvocationExpression node)
		{
			if (node.NamedArguments.Count == 0) return;
			Error(CompilerErrorFactory.NamedArgumentsNotAllowed(node.NamedArguments[0]));
		}

		private bool IsExtensionMethod(IEntity entity)
		{
			IExtensionEnabled extension = entity as IExtensionEnabled;
			return null != extension && extension.IsExtension;
		}

		private bool IsOrContainsExtensionMethod(IEntity entity)
		{
			if (entity == null) return false;

			Ambiguous ambiguous = entity as Ambiguous;
			if (ambiguous != null) return ambiguous.Any(IsExtensionMethod);

			return IsExtensionMethod(entity);
		}

		private void PostNormalizeExtensionInvocation(MethodInvocationExpression node, IMethod targetMethod)
		{
			node.Target = CodeBuilder.CreateMethodReference(node.Target.LexicalInfo, targetMethod);
		}

		private void PreNormalizeExtensionInvocation(MethodInvocationExpression node, IEntityWithParameters extension)
		{
			if (0 == node.Arguments.Count
			    || null == extension
			    || node.Arguments.Count < extension.GetParameters().Length)
			{
				node.Arguments.Insert(0, EnsureMemberReferenceForExtension(node).Target);
			}
		}

		private MemberReferenceExpression EnsureMemberReferenceForExtension(MethodInvocationExpression node)
		{
			Expression target = node.Target;
			GenericReferenceExpression gre = target as GenericReferenceExpression;
			if (null != gre)
				target = gre.Target;

			MemberReferenceExpression memberRef = target as MemberReferenceExpression;
			if (null != memberRef)
				return memberRef;

			node.Target = memberRef = CodeBuilder.MemberReferenceForEntity(
				CreateSelfReference(),
				GetEntity(node.Target));

			return memberRef;
		}

		private SelfLiteralExpression CreateSelfReference()
		{
			return CodeBuilder.CreateSelfReference(CurrentType);
		}

		protected virtual bool IsDuckTyped(IMember entity)
		{
			return entity.IsDuckTyped;
		}

		private IType GetInferredType(IMethod entity)
		{
			return IsDuckTyped(entity)
				? this.TypeSystemServices.DuckType
				: entity.ReturnType;
		}

		private IType GetInferredType(IMember entity)
		{
			Debug.Assert(EntityType.Method != entity.EntityType);
			return IsDuckTyped(entity)
				? this.TypeSystemServices.DuckType
				: entity.Type;
		}

		void ReplaceTypeInvocationByEval(IType type, MethodInvocationExpression node)
		{
			Node parent = node.ParentNode;

			parent.Replace(node, EvalForTypeInvocation(type, node));
		}

		private MethodInvocationExpression EvalForTypeInvocation(IType type, MethodInvocationExpression node)
		{
			MethodInvocationExpression eval = CodeBuilder.CreateEvalInvocation(node.LexicalInfo);
			ReferenceExpression local = CreateTempLocal(node.Target.LexicalInfo, type);

			eval.Arguments.Add(CodeBuilder.CreateAssignment(local.CloneNode(), node));

			AddResolvedNamedArgumentsToEval(eval, node.NamedArguments, local);

			node.NamedArguments.Clear();

			eval.Arguments.Add(local);

			BindExpressionType(eval, type);
			return eval;
		}

		private void AddResolvedNamedArgumentsToEval(MethodInvocationExpression eval, ExpressionPairCollection namedArguments, ReferenceExpression instance)
		{
			foreach (ExpressionPair pair in namedArguments)
			{
				if (TypeSystemServices.IsError(pair.First)) continue;

				AddResolvedNamedArgumentToEval(eval, pair, instance);
			}
		}

		protected virtual void AddResolvedNamedArgumentToEval(MethodInvocationExpression eval, ExpressionPair pair, ReferenceExpression instance)
		{
			IEntity entity = GetEntity(pair.First);
			switch (entity.EntityType)
			{
				case EntityType.Event:
					{
						IEvent member = (IEvent)entity;
						eval.Arguments.Add(
							CodeBuilder.CreateMethodInvocation(
								pair.First.LexicalInfo,
								instance.CloneNode(),
								member.GetAddMethod(),
								pair.Second));
						break;
					}

				case EntityType.Field:
					{
						eval.Arguments.Add(
							CodeBuilder.CreateAssignment(
								pair.First.LexicalInfo,
								CodeBuilder.CreateMemberReference(
									instance.CloneNode(),
									(IMember)entity),
								pair.Second));
						break;
					}

				case EntityType.Property:
					{
						IProperty property = (IProperty)entity;
						IMethod setter = property.GetSetMethod();
						if (null == setter)
						{
							Error(CompilerErrorFactory.PropertyIsReadOnly(
								pair.First,
								property.FullName));
						}
						else
						{
							//EnsureRelatedNodeWasVisited(pair.First, setter);
							eval.Arguments.Add(
								CodeBuilder.CreateAssignment(
									pair.First.LexicalInfo,
									CodeBuilder.CreateMemberReference(
										instance.CloneNode(),
										property),
									pair.Second));
						}
						break;
					}
			}
		}

		void ProcessEventInvocation(IEvent ev, MethodInvocationExpression node)
		{
			NamedArgumentsNotAllowed(node);
			if (!EnsureInternalEventInvocation(ev, node)) return;

			IMethod method = ev.GetRaiseMethod();
			if (AssertParameters(node, method, node.Arguments))
			{
				node.Target = CodeBuilder.CreateMemberReference(
					((MemberReferenceExpression)node.Target).Target,
					method);
				BindExpressionType(node, method.ReturnType);
			}
		}

		public bool EnsureInternalEventInvocation(IEvent ev, Expression node)
		{
			if (ev.IsAbstract || ev.IsVirtual || ev.DeclaringType == CurrentType)
				return true;

			Error(CompilerErrorFactory.EventCanOnlyBeInvokedFromWithinDeclaringClass(node, ev));
			return false;
		}

		void ProcessCallableTypeInvocation(MethodInvocationExpression node, ICallableType type)
		{
			NamedArgumentsNotAllowed(node);

			if (node.Arguments.Count == 1)
			{
				AssertTypeCompatibility(node.Arguments[0], type, GetExpressionType(node.Arguments[0]));
				node.ParentNode.Replace(
					node,
					CodeBuilder.CreateCast(
						type,
						node.Arguments[0]));
			}
			else
			{
				IConstructor ctor = GetCorrectConstructor(node, type, node.Arguments);
				if (null != ctor)
				{
					BindConstructorInvocation(node, ctor);
				}
				else
				{
					Error(node);
				}
			}
		}

		void ProcessTypeInvocation(MethodInvocationExpression node)
		{
			IType type = (IType)node.Target.Entity;

			ICallableType callableType = type as ICallableType;
			if (null != callableType)
			{
				ProcessCallableTypeInvocation(node, callableType);
				return;
			}

			if (!AssertCanCreateInstance(node.Target, type))
			{
				Error(node);
				return;
			}

			ResolveNamedArguments(type, node.NamedArguments);
			if (type.IsValueType && node.Arguments.Count == 0)
			{
				ProcessValueTypeInstantiation(type, node);
				return;
			}

			IConstructor ctor = GetCorrectConstructor(node, type, node.Arguments);
			if (null != ctor)
			{
				BindConstructorInvocation(node, ctor);

				if (node.NamedArguments.Count > 0)
				{
					ReplaceTypeInvocationByEval(type, node);
				}
			}
			else
			{
				Error(node);
			}
		}

		void BindConstructorInvocation(MethodInvocationExpression node, IConstructor ctor)
		{
			// rebind the target now we know
			// it is a constructor call
			Bind(node.Target, ctor);
			BindExpressionType(node.Target, ctor.Type);
			BindExpressionType(node, ctor.DeclaringType);
		}

		private void ProcessValueTypeInstantiation(IType type, MethodInvocationExpression node)
		{
			ReferenceExpression target = GetOptimizedValueTypeInstantiationTarget(type, node);

			//if no optimized target given, create a temp local to work with
			if (null == target)
			{
				InternalLocal local = DeclareTempLocal(type);
				target = CodeBuilder.CreateReference(local);
			}

			Expression initializer = CodeBuilder.CreateDefaultInitializer(node.LexicalInfo, target, type);

			MethodInvocationExpression eval = CodeBuilder.CreateEvalInvocation(node.LexicalInfo);
			BindExpressionType(eval, type);
			eval.Arguments.Add(initializer);
			AddResolvedNamedArgumentsToEval(eval, node.NamedArguments, target);
			eval.Arguments.Add(target);
			node.ParentNode.Replace(node, eval);
		}

		protected virtual ReferenceExpression GetOptimizedValueTypeInstantiationTarget(IType type, MethodInvocationExpression node)
		{
			if (!AstUtil.IsAssignment(node.ParentNode))
				return null;

			BinaryExpression be = (BinaryExpression) node.ParentNode;
			ReferenceExpression target = be.Left as ReferenceExpression;
			if (null == target)
				return null;

			//cannot optimize local (not memberref|field) instantiation with named args
			if (be.Left.NodeType == NodeType.ReferenceExpression && 0 != node.NamedArguments.Count)
				return null;

			//TODO: field temp initializer optimization (!!?)
			if (target.Name == TempInitializerName)
				return null;

			if (null == target.Entity)
			{
				Bind(target, type);
				BindExpressionType(target, type);
			}
			return target;
		}

		void ProcessGenericMethodInvocation(MethodInvocationExpression node)
		{
			IType type = GetExpressionType(node.Target);
			if (TypeSystemServices.IsCallable(type))
			{
				ProcessMethodInvocationOnCallableExpression(node);
			}
			else
			{
				Error(node,
					  CompilerErrorFactory.TypeIsNotCallable(node.Target, type.ToString()));
			}
		}

		void ProcessMethodInvocationOnCallableExpression(MethodInvocationExpression node)
		{
			IType type = node.Target.ExpressionType;

			ICallableType delegateType = type as ICallableType;
			if (null != delegateType)
			{
				if (AssertParameters(node.Target, delegateType, delegateType, node.Arguments))
				{
					IMethod invoke = ResolveMethod(delegateType, "Invoke");
					node.Target = CodeBuilder.CreateMemberReference(node.Target, invoke);
					BindExpressionType(node, invoke.ReturnType);
				}
				else
				{
					Error(node);
				}
			}
			else if (TypeSystemServices.ICallableType.IsAssignableFrom(type))
			{
				node.Target = CodeBuilder.CreateMemberReference(node.Target, ICallable_Call);
				ArrayLiteralExpression arg = CodeBuilder.CreateObjectArray(node.Arguments);
				node.Arguments.Clear();
				node.Arguments.Add(arg);

				BindExpressionType(node, ICallable_Call.ReturnType);
			}
			else if (TypeSystemServices.TypeType == type)
			{
				ProcessSystemTypeInvocation(node);
			}
			else
			{
				ProcessInvocationOnUnknownCallableExpression(node);
			}
		}

		private void ProcessSystemTypeInvocation(MethodInvocationExpression node)
		{
			MethodInvocationExpression invocation = CreateInstanceInvocationFor(node);
			if (invocation.NamedArguments.Count == 0)
			{
				node.ParentNode.Replace(node, invocation);
				return;
			}
			ProcessNamedArgumentsForTypeInvocation(invocation);
			node.ParentNode.Replace(node, EvalForTypeInvocation(TypeSystemServices.ObjectType, invocation));
		}

		private void ProcessNamedArgumentsForTypeInvocation(MethodInvocationExpression invocation)
		{
			foreach (ExpressionPair pair in invocation.NamedArguments)
			{
				if (!ProcessNamedArgument(pair)) continue;
				NamedArgumentNotFound(TypeSystemServices.ObjectType, (ReferenceExpression)pair.First);
			}
		}

		private MethodInvocationExpression CreateInstanceInvocationFor(MethodInvocationExpression node)
		{
			MethodInvocationExpression invocation = CodeBuilder.CreateMethodInvocation(Activator_CreateInstance, node.Target);
			if (Activator_CreateInstance.AcceptVarArgs)
			{
				invocation.Arguments.Extend(node.Arguments);
			}
			else
			{
				invocation.Arguments.Add(CodeBuilder.CreateObjectArray(node.Arguments));
			}
			invocation.NamedArguments = node.NamedArguments;
			return invocation;
		}

		protected virtual void ProcessInvocationOnUnknownCallableExpression(MethodInvocationExpression node)
		{
			NotImplemented(node, "Method invocation on type '" + node.Target.ExpressionType + "'.");
		}

		bool AssertIdentifierName(Node node, string name)
		{
			if (TypeSystemServices.IsPrimitive(name))
			{
				Error(CompilerErrorFactory.CantRedefinePrimitive(node, name));
				return false;
			}
			return true;
		}

		private bool CheckIsNotValueType(BinaryExpression node, Expression expression)
		{
			IType tag = GetExpressionType(expression);
			if (tag.IsValueType)
			{
				Error(CompilerErrorFactory.OperatorCantBeUsedWithValueType(
					expression,
					GetBinaryOperatorText(node.Operator),
					tag.ToString()));

				return false;
			}
			return true;
		}

		void BindAssignmentToSlice(BinaryExpression node)
		{
			SlicingExpression slice = (SlicingExpression)node.Left;

			if (!IsAmbiguous(slice.Target.Entity)
				&& GetExpressionType(slice.Target).IsArray)
			{
				BindAssignmentToSliceArray(node);
			}
			else if (TypeSystemServices.IsDuckTyped(slice.Target))
			{
				BindExpressionType(node, TypeSystemServices.DuckType);
			}
			else
			{
				BindAssignmentToSliceProperty(node);
			}
		}

		void BindAssignmentToSliceArray(BinaryExpression node)
		{
			SlicingExpression slice = (SlicingExpression)node.Left;

			IArrayType sliceTargetType = (IArrayType)GetExpressionType(slice.Target);
			IType lhsType = GetExpressionType(node.Right);

			foreach (Slice item in slice.Indices)
			{
				if (!AssertTypeCompatibility(item.Begin, TypeSystemServices.IntType, GetExpressionType(item.Begin)))
				{
					Error(node);
					return;
				}
			}

			if (slice.Indices.Count > 1)
			{
				if (AstUtil.IsComplexSlicing(slice))
				{
					// FIXME: Check type compatibility
					BindAssignmentToComplexSliceArray(node);
				}
				else
				{
					if (!AssertTypeCompatibility(node.Right, sliceTargetType.GetElementType(), lhsType))
					{
						Error(node);
						return;
					}
					BindAssignmentToSimpleSliceArray(node);
				}
			}
			else
			{
				if (!AssertTypeCompatibility(node.Right, sliceTargetType.GetElementType(), lhsType))
				{
					Error(node);
					return;
				}
				node.ExpressionType = sliceTargetType.GetElementType();
			}
		}

		void BindAssignmentToSimpleSliceArray(BinaryExpression node)
		{
			SlicingExpression slice = (SlicingExpression)node.Left;
			MethodInvocationExpression mie = CodeBuilder.CreateMethodInvocation(
				slice.Target,
				TypeSystemServices.Map(typeof(Array).GetMethod("SetValue", new Type[] { typeof(object), typeof(int[]) })),
				node.Right);
			for (int i = 0; i < slice.Indices.Count; i++)
			{
				mie.Arguments.Add(slice.Indices[i].Begin);
			}
			BindExpressionType(mie, TypeSystemServices.VoidType);
			node.ParentNode.Replace(node, mie);
		}

		void BindAssignmentToComplexSliceArray(BinaryExpression node)
		{
			SlicingExpression slice = (SlicingExpression)node.Left;
			ArrayLiteralExpression ale = new ArrayLiteralExpression();
			ArrayLiteralExpression collapse = new ArrayLiteralExpression();
			for (int i = 0; i < slice.Indices.Count; i++)
			{
				ale.Items.Add(slice.Indices[i].Begin);
				if (null == slice.Indices[i].End ||
					OmittedExpression.Default == slice.Indices[i].End)
				{
					ale.Items.Add(new IntegerLiteralExpression(1 + (int)((IntegerLiteralExpression)slice.Indices[i].Begin).Value));
					collapse.Items.Add(new BoolLiteralExpression(true));
				}
				else
				{
					ale.Items.Add(slice.Indices[i].End);
					collapse.Items.Add(new BoolLiteralExpression(false));
				}
			}

			MethodInvocationExpression mie = CodeBuilder.CreateMethodInvocation(
				RuntimeServices_SetMultiDimensionalRange1,
				node.Right,
				slice.Target,
				ale);

			mie.Arguments.Add(collapse);

			BindExpressionType(mie, TypeSystemServices.VoidType);
			BindExpressionType(ale, TypeSystemServices.Map(typeof(int[])));
			BindExpressionType(collapse, TypeSystemServices.Map(typeof(bool[])));
			node.ParentNode.Replace(node, mie);
		}

		void BindAssignmentToSliceProperty(BinaryExpression node)
		{
			SlicingExpression slice = (SlicingExpression)node.Left;

			IEntity lhs = GetEntity(node.Left);
			IMethod setter = null;

			MethodInvocationExpression mie = new MethodInvocationExpression(node.Left.LexicalInfo);
			foreach (Slice index in slice.Indices)
			{
				mie.Arguments.Add(index.Begin);
			}
			mie.Arguments.Add(node.Right);

			if (EntityType.Property == lhs.EntityType)
			{
				IMethod setMethod = ((IProperty)lhs).GetSetMethod();
				if (null == setMethod)
				{
					Error(node, CompilerErrorFactory.PropertyIsReadOnly(slice.Target, lhs.FullName));
					return;
				}
				if (AssertParameters(node.Left, setMethod, mie.Arguments))
				{
					setter = setMethod;
				}
			}
			else if (EntityType.Ambiguous == lhs.EntityType)
			{
				setter = (IMethod)GetCorrectCallableReference(node.Left, mie.Arguments, GetSetMethods(lhs));
				if (setter == null)
				{
					Error(node.Left);
					return;
				}
			}

			if (null == setter)
			{
				Error(node, CompilerErrorFactory.LValueExpected(node.Left));
			}
			else
			{
				mie.Target = CodeBuilder.CreateMemberReference(
					GetIndexedPropertySlicingTarget(slice),
					setter);
				BindExpressionType(mie, setter.ReturnType);
				node.ParentNode.Replace(node, mie);
			}
		}

		private IEntity[] GetSetMethods(IEntity candidates)
		{
			return GetSetMethods(((Ambiguous)candidates).Entities);
		}

		void BindAssignment(BinaryExpression node)
		{
			BindNullableOperation(node);

			if (NodeType.SlicingExpression == node.Left.NodeType)
			{
				BindAssignmentToSlice(node);
			}
			else
			{
				ProcessAssignment(node);
			}
		}

		virtual protected void ProcessAssignment(BinaryExpression node)
		{
			TryToResolveAmbiguousAssignment(node);
			ValidateAssignment(node);
			BindExpressionType(node, GetExpressionType(node.Right));
		}

		virtual protected void ValidateAssignment(BinaryExpression node)
		{
			IEntity lhs = node.Left.Entity;
			IType rtype = GetExpressionType(node.Right);
			if (AssertLValue(node.Left, lhs))
			{
				IType lhsType = GetExpressionType(node.Left);
				AssertTypeCompatibility(node.Right, lhsType, rtype);
				CheckAssignmentToIndexedProperty(node.Left, lhs);
			}
		}

		virtual protected void TryToResolveAmbiguousAssignment(BinaryExpression node)
		{
			IEntity lhs = node.Left.Entity;
			if (null == lhs) return;
			if (EntityType.Ambiguous != lhs.EntityType) return;

			Expression lvalue = node.Left;
			lhs = ResolveAmbiguousLValue(lvalue, (Ambiguous)lhs, node.Right);
			if (NodeType.ReferenceExpression == lvalue.NodeType)
			{
				IMember member = lhs as IMember;
				if (null != member)
				{
					ResolveMemberInfo((ReferenceExpression)lvalue, member);
				}
			}
		}

		private void CheckAssignmentToIndexedProperty(Node node, IEntity lhs)
		{
			IProperty property = lhs as IProperty;
			if (null != property && IsIndexedProperty(property))
			{
				Error(CompilerErrorFactory.PropertyRequiresParameters(AstUtil.GetMemberAnchor(node), property.FullName));
			}
		}

		bool CheckIsaArgument(Expression e)
		{
			if (TypeSystemServices.TypeType != GetExpressionType(e))
			{
				Error(CompilerErrorFactory.IsaArgument(e));
				return false;
			}
			return true;
		}

		bool BindNullableOperation(BinaryExpression node)
		{
			if (!IsNullableOperation(node))
				return false;

			if (BinaryOperatorType.ReferenceEquality == node.Operator)
			{
				node.Operator = BinaryOperatorType.Equality;
				return BindNullableComparison(node);
			}
			else if (BinaryOperatorType.ReferenceInequality == node.Operator)
			{
				node.Operator = BinaryOperatorType.Inequality;
				return BindNullableComparison(node);
			}

			IType lhs = GetExpressionType(node.Left);
			IType rhs = GetExpressionType(node.Right);
			bool lhsIsNullable = TypeSystemServices.IsNullable(lhs);
			bool rhsIsNullable = TypeSystemServices.IsNullable(rhs);

			if (BinaryOperatorType.Assign == node.Operator)
			{
				if (lhsIsNullable)
				{
					if (rhsIsNullable)
						return false;
					BindNullableInitializer(node, node.Right, lhs);
					return false;
				}
			}

			if (lhsIsNullable)
			{
				MemberReferenceExpression mre = new MemberReferenceExpression(node.Left, "Value");
				node.Replace(node.Left, mre);
				Visit(mre);
				mre.Annotate("nullableTarget", true);
			}
			if (rhsIsNullable)
			{
				MemberReferenceExpression mre = new MemberReferenceExpression(node.Right, "Value");
				node.Replace(node.Right, mre);
				Visit(mre);
				mre.Annotate("nullableTarget", true);
			}

			return false;
		}

		bool BindNullableComparison(BinaryExpression node)
		{
			if (!IsNullableOperation(node))
				return false;

			if (IsNull(node.Left) || IsNull(node.Right))
			{
				Expression nullable = IsNull(node.Left) ? node.Right : node.Left;
				Expression val = new MemberReferenceExpression(nullable, "HasValue");
				node.Replace(node.Left, val);
				Visit(val);
				Expression nil = new BoolLiteralExpression(false);
				node.Replace(node.Right, nil);
				Visit(nil);
				BindExpressionType(node, TypeSystemServices.BoolType);
				return true;
			}

			BinaryExpression valueCheck = new BinaryExpression(
				(node.Operator == BinaryOperatorType.Inequality)
					? BinaryOperatorType.BitwiseOr
					: BinaryOperatorType.BitwiseAnd,
				new BinaryExpression(
					GetCorrespondingHasValueOperator(node.Operator),
					CreateNullableHasValueOrTrueExpression(node.Left),
					CreateNullableHasValueOrTrueExpression(node.Right)
				),
				new BinaryExpression(
					node.Operator,
					CreateNullableGetValueOrDefaultExpression(node.Left),
					CreateNullableGetValueOrDefaultExpression(node.Right)
				)
			);
			node.ParentNode.Replace(node, valueCheck);
			Visit(valueCheck);
			return true;
		}

		private BinaryOperatorType GetCorrespondingHasValueOperator(BinaryOperatorType op)
		{
			if (BinaryOperatorType.Equality == op || BinaryOperatorType.Inequality == op)
				return op;
			//when there is at least one non-value operand then any other comparison
			//than equality/inequality is undefined/false (as in C#)
			return BinaryOperatorType.BitwiseAnd;
		}

		private IEnumerable<Expression> FindNullableExpressions(Expression exp)
		{
			if (exp.ContainsAnnotation("nullableTarget"))
			{
				yield return ((MemberReferenceExpression) exp).Target;
			}
			else
			{
				BinaryExpression bex = exp as BinaryExpression;
				if (null != bex)
				{
					foreach (Expression inner in FindNullableExpressions(bex.Left))
						yield return inner;
					foreach (Expression inner in FindNullableExpressions(bex.Right))
						yield return inner;
				}
			}
		}

		private Expression BuildNullableCoalescingConditional(Expression exp)
		{
			if (IsNull(exp)) return null;

			IEnumerator<Expression> enumerator = FindNullableExpressions(exp).GetEnumerator();
			Expression root = null;
			BinaryExpression and = null;
			Expression lookahead = null;

			while (enumerator.MoveNext())
			{
				Expression cur = enumerator.Current;
				lookahead = enumerator.MoveNext() ? enumerator.Current : null;
				if (null != and)
				{
					and.Right = new BinaryExpression(
										BinaryOperatorType.BitwiseAnd,
										and.Right,
										new BinaryExpression(
											BinaryOperatorType.BitwiseAnd,
											CreateNullableHasValueOrTrueExpression(cur),
											CreateNullableHasValueOrTrueExpression(lookahead)
										)
									);
				}
				else
				{
					if (null == lookahead)
						return CreateNullableHasValueOrTrueExpression(cur);
					root = and = new BinaryExpression(
									BinaryOperatorType.BitwiseAnd,
									CreateNullableHasValueOrTrueExpression(cur),
									CreateNullableHasValueOrTrueExpression(lookahead)
								);
				}
			}

			return root;
		}

		void BindNullableInitializer(Node node, Expression rhs, IType type)
		{
			Expression instantiation = CreateNullableInstantiation(rhs, type);
			node.Replace(rhs, instantiation);
			Visit(instantiation);

			Expression coalescing = BuildNullableCoalescingConditional(rhs);
			if (null != coalescing) //rhs contains at least one nullable
			{
				ConditionalExpression cond = new ConditionalExpression();
				cond.Condition = coalescing;
				cond.TrueValue = instantiation;
				cond.FalseValue = CreateNullableInstantiation(type);

				node.Replace(instantiation, cond);
				Visit(cond);
			}
		}

		void BindNullableParameters(ExpressionCollection args, ICallableType target)
		{
			if (null == target)
				return;

			IParameter[] parameters = target.GetSignature().Parameters;
			for (int i = 0; i < parameters.Length; ++i) {
				if (!TypeSystemServices.IsNullable(parameters[i].Type))
					continue;
				if (TypeSystemServices.IsNullable(GetExpressionType(args[i])))
					continue; //already nullable
				args.Replace(args[i], CreateNullableInstantiation(args[i], parameters[i].Type));
				Visit(args[i]);
			}
		}

		private Expression CreateNullableInstantiation(IType type)
		{
			return CreateNullableInstantiation(null, type);
		}

		private Expression CreateNullableInstantiation(Expression val, IType type)
		{
			MethodInvocationExpression mie = new MethodInvocationExpression();
			GenericReferenceExpression gre = new GenericReferenceExpression();
			gre.Target = new MemberReferenceExpression(new ReferenceExpression("System"), "Nullable");
			gre.GenericArguments.Add(TypeReference.Lift(Nullable.GetUnderlyingType(((ExternalType) type).ActualType)));
			mie.Target = gre;
			if (null != val && !IsNull(val))
				mie.Arguments.Add(val);
			return mie;
		}

		private Expression CreateNullableHasValueOrTrueExpression(Expression target)
		{
			if (null == target || !TypeSystemServices.IsNullable(GetExpressionType(target)))
				return new BoolLiteralExpression(true);

			return new MemberReferenceExpression(target, "HasValue");
		}

		private Expression CreateNullableGetValueOrDefaultExpression(Expression target)
		{
			if (null == target || !TypeSystemServices.IsNullable(GetExpressionType(target)))
				return target;

			MethodInvocationExpression mie = new MethodInvocationExpression();
			mie.Target = new MemberReferenceExpression(target, "GetValueOrDefault");
			return mie;
		}

		void BindTypeTest(BinaryExpression node)
		{
			if (CheckIsNotValueType(node, node.Left) &&
				CheckIsaArgument(node.Right))
			{
				BindExpressionType(node, TypeSystemServices.BoolType);
			}
			else
			{
				Error(node);
			}
		}

		void BindReferenceEquality(BinaryExpression node)
		{
			BindReferenceEquality(node, false);
		}

		void BindReferenceEquality(BinaryExpression node, bool inequality)
		{
			if (BindNullableOperation(node))
			{
				return;
			}

			//BOO-1174: accept `booleanExpression is true|false`
			BoolLiteralExpression isBool = node.Right as BoolLiteralExpression;
			if (null != isBool)
			{
				if (GetExpressionType(node.Left) == TypeSystemServices.BoolType)
				{
					Node replacement = (isBool.Value ^ inequality)
					                   ? node.Left
					                   : new UnaryExpression(UnaryOperatorType.LogicalNot, node.Left);
					node.ParentNode.Replace(node, replacement);
					Visit(replacement);
					return;
				}
			}

			if (CheckIsNotValueType(node, node.Left) &&
				CheckIsNotValueType(node, node.Right))
			{
				BindExpressionType(node, TypeSystemServices.BoolType);
			}
			else
			{
				Error(node);
			}
		}

		void BindInPlaceArithmeticOperator(BinaryExpression node)
		{
			if (IsArraySlicing(node.Left))
			{
				BindInPlaceArithmeticOperatorOnArraySlicing(node);
				return;
			}

			Node parent = node.ParentNode;

			Expression target = node.Left;
			if (null != target.Entity && EntityType.Property == target.Entity.EntityType)
			{
				// if target is a property force a rebinding
				target.ExpressionType = null;
			}

			BinaryExpression assign = ExpandInPlaceBinaryExpression(node);
			parent.Replace(node, assign);
			Visit(assign);
		}

		protected BinaryExpression ExpandInPlaceBinaryExpression(BinaryExpression node)
		{
			BinaryExpression assign = new BinaryExpression(node.LexicalInfo);
			assign.Operator = BinaryOperatorType.Assign;
			assign.Left = node.Left.CloneNode();
			assign.Right = node;
			node.Operator = GetRelatedBinaryOperatorForInPlaceOperator(node.Operator);
			return assign;
		}

		private void BindInPlaceArithmeticOperatorOnArraySlicing(BinaryExpression node)
		{
			Node parent = node.ParentNode;
			Expression expansion = CreateSideEffectAwareSlicingOperation(
				node.LexicalInfo,
				GetRelatedBinaryOperatorForInPlaceOperator(node.Operator),
				(SlicingExpression) node.Left,
				node.Right,
				null);
			parent.Replace(node, expansion);
		}

		BinaryOperatorType GetRelatedBinaryOperatorForInPlaceOperator(BinaryOperatorType op)
		{
			switch (op)
			{
				case BinaryOperatorType.InPlaceAddition:
					return BinaryOperatorType.Addition;

				case BinaryOperatorType.InPlaceSubtraction:
					return BinaryOperatorType.Subtraction;

				case BinaryOperatorType.InPlaceMultiply:
					return BinaryOperatorType.Multiply;

				case BinaryOperatorType.InPlaceDivision:
					return BinaryOperatorType.Division;

				case BinaryOperatorType.InPlaceModulus:
					return BinaryOperatorType.Modulus;

				case BinaryOperatorType.InPlaceBitwiseAnd:
					return BinaryOperatorType.BitwiseAnd;

				case BinaryOperatorType.InPlaceBitwiseOr:
					return BinaryOperatorType.BitwiseOr;

				case BinaryOperatorType.InPlaceExclusiveOr:
					return BinaryOperatorType.ExclusiveOr;

				case BinaryOperatorType.InPlaceShiftLeft:
					return BinaryOperatorType.ShiftLeft;

				case BinaryOperatorType.InPlaceShiftRight:
					return BinaryOperatorType.ShiftRight;
			}
			throw new ArgumentException("op");
		}

		void BindArrayAddition(BinaryExpression node)
		{
			IArrayType lhs = (IArrayType)GetExpressionType(node.Left);
			IArrayType rhs = (IArrayType)GetExpressionType(node.Right);

			if (lhs.GetElementType() == rhs.GetElementType())
			{
				node.ParentNode.Replace(
					node,
					CodeBuilder.CreateCast(
						lhs,
						CodeBuilder.CreateMethodInvocation(
							RuntimeServices_AddArrays,
							CodeBuilder.CreateTypeofExpression(lhs.GetElementType()),
							node.Left,
							node.Right)));
			}
			else
			{
				InvalidOperatorForTypes(node);
			}
		}

		void BindArithmeticOperator(BinaryExpression node)
		{
			BindNullableOperation(node);

			IType left = GetExpressionType(node.Left);
			IType right = GetExpressionType(node.Right);
			if (TypeSystemServices.IsPrimitiveNumber(left) && TypeSystemServices.IsPrimitiveNumber(right))
			{
				BindExpressionType(node, TypeSystemServices.GetPromotedNumberType(left, right));
			}
			else if (left.IsPointer && !BindPointerArithmeticOperator(node, left, right))
			{
				InvalidOperatorForTypes(node);
			}
			else if (!ResolveOperator(node))
			{
				InvalidOperatorForTypes(node);
			}
		}

		bool BindPointerArithmeticOperator(BinaryExpression node, IType left, IType right)
		{
			if (!left.IsPointer || !TypeSystemServices.IsPrimitiveNumber(right))
				return false;

			switch (node.Operator)
			{
				case BinaryOperatorType.Addition:
				case BinaryOperatorType.Subtraction:
					if (node.ContainsAnnotation("pointerSizeNormalized"))
						return true;

					BindExpressionType(node, left);

					int size = TypeSystemServices.SizeOf(left);
					if (size == 1)
						return true; //no need for normalization

					//normalize RHS wrt size of pointer
					IntegerLiteralExpression literal = node.Right as IntegerLiteralExpression;
					Expression normalizedRhs = (null != literal)
						? (Expression)
							new IntegerLiteralExpression(literal.Value * size)
						: (Expression)
							new BinaryExpression(BinaryOperatorType.Multiply,
								node.Right,
								new IntegerLiteralExpression(size));
					node.Replace(node.Right, normalizedRhs);
					Visit(node.Right);
					node.Annotate("pointerSizeNormalized", size);
					return true;
			}

			return false;
		}

		static string GetBinaryOperatorText(BinaryOperatorType op)
		{
			return BooPrinterVisitor.GetBinaryOperatorText(op);
		}

		static string GetUnaryOperatorText(UnaryOperatorType op)
		{
			return BooPrinterVisitor.GetUnaryOperatorText(op);
		}

		IEntity ResolveName(Node node, string name)
		{
			IEntity tag = NameResolutionService.Resolve(name);
			CheckNameResolution(node, name, tag);
			return tag;
		}

		bool CheckNameResolution(Node node, string name, IEntity tag)
		{
			if (null == tag)
			{
				Error(CompilerErrorFactory.UnknownIdentifier(node, name));
				return false;
			}
			return true;
		}

		private bool IsPublicEvent(IEntity tag)
		{
			return (EntityType.Event == tag.EntityType) && ((IMember)tag).IsPublic;
		}

		private bool IsPublicFieldPropertyEvent(IEntity entity)
		{
			if (!IsFieldPropertyOrEvent(entity) || !((IMember) entity).IsPublic)
				return false;

			IField field = entity as IField;
			return (null == field) || (!field.IsInitOnly && !field.IsLiteral);
		}

		private static bool IsFieldPropertyOrEvent(IEntity entity)
		{
			return ((EntityType.Field|EntityType.Property|EntityType.Event) & entity.EntityType) > 0;
		}

		private IMember ResolvePublicFieldPropertyEvent(Expression sourceNode, IType type, string name)
		{
			IEntity candidate = ResolveFieldPropertyEvent(type, name);
			if (null == candidate) return null;

			if (IsPublicFieldPropertyEvent(candidate)) return (IMember)candidate;

			if (candidate.EntityType != EntityType.Ambiguous) return null;

			IList found = ((Ambiguous)candidate).Select(IsPublicFieldPropertyEvent);
			if (found.Count == 0) return null;
			if (found.Count == 1) return (IMember)found[0];

			Error(sourceNode, CompilerErrorFactory.AmbiguousReference(sourceNode, name, found));
			return null;
		}

		protected IEntity ResolveFieldPropertyEvent(IType type, string name)
		{
			return NameResolutionService.Resolve(type, name, EntityType.Property|EntityType.Event|EntityType.Field);
		}

		void ResolveNamedArguments(IType type, ExpressionPairCollection arguments)
		{
			foreach (ExpressionPair arg in arguments)
			{
				if (!ProcessNamedArgument(arg)) continue;
				ResolveNamedArgument(type, (ReferenceExpression)arg.First, arg.Second);
			}
		}

		private bool ProcessNamedArgument(ExpressionPair arg)
		{
			Visit(arg.Second);
			if (NodeType.ReferenceExpression != arg.First.NodeType)
			{
				Error(arg.First, CompilerErrorFactory.NamedParameterMustBeIdentifier(arg));
				return false;
			}
			return true;
		}

		void ResolveNamedArgument(IType type, ReferenceExpression name, Expression value)
		{
			IMember member = ResolvePublicFieldPropertyEvent(name, type, name.Name);
			if (null == member)
			{
				NamedArgumentNotFound(type, name);
				return;
			}

			EnsureRelatedNodeWasVisited(name, member);
			Bind(name, member);

			IType memberType = member.Type;
			if (member.EntityType == EntityType.Event)
			{
				AssertDelegateArgument(value, member, GetExpressionType(value));
			}
			else
			{
				AssertTypeCompatibility(value, memberType, GetExpressionType(value));
			}
		}

		protected virtual void NamedArgumentNotFound(IType type, ReferenceExpression name)
		{
			Error(name, CompilerErrorFactory.NotAPublicFieldOrProperty(name, type.ToString(), name.Name));
		}

		bool AssertTypeCompatibility(Node sourceNode, IType expectedType, IType actualType)
		{
			if (TypeSystemServices.IsError(expectedType) || TypeSystemServices.IsError(actualType))
			{
				return false;
			}

			if (expectedType.IsPointer && actualType.IsPointer)
				return true; //if both types are unmanaged pointers casting is always possible

			if (TypeSystemServices.IsNullable(expectedType) && EntityType.Null == actualType.EntityType)
			{
				return true;
			}

			if (!AreTypesRelated(sourceNode, expectedType, actualType))
			{
				Error(CompilerErrorFactory.IncompatibleExpressionType(sourceNode, expectedType.ToString(), actualType.ToString()));
				return false;
			}

			return true;
		}

		bool AssertDelegateArgument(Node sourceNode, ITypedEntity delegateMember, ITypedEntity argumentInfo)
		{
			if (!delegateMember.Type.IsAssignableFrom(argumentInfo.Type))
			{
				Error(CompilerErrorFactory.EventArgumentMustBeAMethod(sourceNode, delegateMember.FullName, delegateMember.Type.ToString()));
				return false;
			}
			return true;
		}

		bool CheckParameterTypesStrictly(IMethod method, ExpressionCollection args)
		{
			IParameter[] parameters = method.GetParameters();
			for (int i=0; i<args.Count; ++i)
			{
				IType expressionType = GetExpressionType(args[i]);
				IType parameterType = parameters[i].Type;
				if (!IsAssignableFrom(parameterType, expressionType) &&
					!(TypeSystemServices.IsNumber(expressionType) && TypeSystemServices.IsNumber(parameterType))
					&& TypeSystemServices.FindImplicitConversionOperator(expressionType,parameterType) == null)
				{
					return false;
				}
			}
			return true;
		}

		bool AssertParameterTypes(ICallableType method, ExpressionCollection args, int count, bool reportErrors)
		{
			IParameter[] parameters = method.GetSignature().Parameters;
			for (int i=0; i<count; ++i)
			{
				IParameter param = parameters[i];
				IType parameterType = param.Type;
				IType argumentType = GetExpressionType(args[i]);
				if (param.IsByRef)
				{
					if (!(args[i] is ReferenceExpression
					    || args[i] is SlicingExpression
					    || (args[i] is SelfLiteralExpression && argumentType.IsValueType)))
					{
						if (reportErrors)
							Error(CompilerErrorFactory.RefArgTakesLValue(args[i]));
						return false;
					}
					if (!CallableResolutionService.IsValidByRefArg(param, parameterType, argumentType, args[i]))
					{
						return false;
					}
				}
				else
				{
					if (!AreTypesRelated(args[i], parameterType, argumentType))
						return false;
				}
			}
			return true;
		}

		bool AssertParameters(Node sourceNode, IMethod method, ExpressionCollection args)
		{
			return AssertParameters(sourceNode, method, method.CallableType, args);
		}

		bool AcceptVarArgs(ICallableType method)
		{
			return method.GetSignature().AcceptVarArgs;
		}

		bool AssertParameters(Node sourceNode, IEntity sourceEntity, ICallableType method, ExpressionCollection args)
		{
			if (CheckParameters(method, args, true))
				return true;

			if (IsLikelyMacroExtensionMethodInvocation(sourceNode, sourceEntity, args))
				Error(CompilerErrorFactory.MacroExpansionError(sourceNode));
			else
				Error(CompilerErrorFactory.MethodSignature(sourceNode, sourceEntity.ToString(), GetSignature(args)));
			return false;
		}

		bool IsLikelyMacroExtensionMethodInvocation(Node node, IEntity entity, ExpressionCollection args)
		{
			IMethod extension = entity as IMethod;
			return null != extension
				&& extension.IsBooExtension
				&& TypeSystemServices.IsMacro(extension.ReturnType)
				&& 2 == extension.GetParameters().Length
				&& TypeSystemServices.IsMacro(extension.GetParameters()[0].Type);
		}

		protected virtual bool CheckParameters(ICallableType method, ExpressionCollection args, bool reportErrors)
		{
			BindNullableParameters(args, method);
			return AcceptVarArgs(method)
				? CheckVarArgsParameters(method, args)
				: CheckExactArgsParameters(method, args, reportErrors);
		}

		protected bool CheckVarArgsParameters(ICallableType method, ExpressionCollection args)
		{
			return CallableResolutionService.IsValidVargsInvocation(method.GetSignature().Parameters, args);
		}

		protected bool CheckExactArgsParameters(ICallableType method, ExpressionCollection args, bool reportErrors)
		{
			if (method.GetSignature().Parameters.Length != args.Count) return false;
			return AssertParameterTypes(method, args, args.Count, reportErrors);
		}

		bool IsRuntimeIterator(IType type)
		{
			return TypeSystemServices.IsSystemObject(type)
				|| IsTextReader(type);
		}

		bool IsTextReader(IType type)
		{
			return IsAssignableFrom(typeof(TextReader), type);
		}

		bool AssertTargetContext(Expression targetContext, IMember member)
		{
			if (member.IsStatic) return true;
			if (NodeType.MemberReferenceExpression != targetContext.NodeType) return true;

			Expression targetReference = ((MemberReferenceExpression)targetContext).Target;
			IEntity entity = targetReference.Entity;
			if ((null != entity && EntityType.Type == entity.EntityType)
				|| (NodeType.SelfLiteralExpression == targetReference.NodeType
					&& _currentMethod.IsStatic))
			{
				Error(CompilerErrorFactory.InstanceRequired(targetContext, member.DeclaringType.ToString(), member.Name));
				return false;
			}
			return true;
		}

		static bool IsAssignableFrom(IType expectedType, IType actualType)
		{
			return expectedType.IsAssignableFrom(actualType);
		}

		bool IsAssignableFrom(Type expectedType, IType actualType)
		{
			return TypeSystemServices.Map(expectedType).IsAssignableFrom(actualType);
		}

		bool IsPrimitiveNumber(Expression expression)
		{
			return TypeSystemServices.IsPrimitiveNumber(GetExpressionType(expression));
		}

		IConstructor GetCorrectConstructor(Node sourceNode, IType type, ExpressionCollection arguments)
		{
			IConstructor[] constructors = type.GetConstructors();
			if (null != constructors && constructors.Length > 0)
			{
				return (IConstructor)GetCorrectCallableReference(sourceNode, arguments, constructors);
			}
			else
			{
				if (!TypeSystemServices.IsError(type))
				{
					if (null == (type as IGenericParameter))
					{
						Error(CompilerErrorFactory.NoApropriateConstructorFound(sourceNode, type.ToString(), GetSignature(arguments)));
					}
					else
					{
						Error(CompilerErrorFactory.CannotCreateAnInstanceOfGenericParameterWithoutDefaultConstructorConstraint(sourceNode, type.ToString()));
					}
				}
			}
			return null;
		}

		IEntity GetCorrectCallableReference(Node sourceNode, ExpressionCollection args, IEntity[] candidates)
		{
			// BOO-844: Ensure all candidates were visited (to make property setters have correct signature)
			foreach (IEntity candidate in candidates)
			{
				EnsureRelatedNodeWasVisited(sourceNode, candidate);
			}

			IEntity found = CallableResolutionService.ResolveCallableReference(args, candidates);
			if (null == found)
				EmitCallableResolutionError(sourceNode, candidates, args);
			else
				BindNullableParameters(args, ((IMethodBase) found).CallableType);

			return found;
		}

		private void EmitCallableResolutionError(Node sourceNode, IEntity[] candidates, ExpressionCollection args)
		{
			if (CallableResolutionService.ValidCandidates.Count > 1)
			{
				Error(CompilerErrorFactory.AmbiguousReference(sourceNode, candidates[0].Name, CallableResolutionService.ValidCandidates));
			}
			else
			{
				IEntity candidate = candidates[0];
				IConstructor constructor = candidate as IConstructor;
				if (null != constructor)
				{
					Error(CompilerErrorFactory.NoApropriateConstructorFound(sourceNode, constructor.DeclaringType.ToString(), GetSignature(args)));
				}
				else
				{
					Error(CompilerErrorFactory.NoApropriateOverloadFound(sourceNode, GetSignature(args), candidate.FullName));
				}
			}
		}

		override protected void EnsureRelatedNodeWasVisited(Node sourceNode, IEntity entity)
		{
			IInternalEntity internalInfo = GetConstructedInternalEntity(entity);
			if (null == internalInfo)
			{
				ITypedEntity typedEntity = entity as ITypedEntity;
				if (null == typedEntity) return;

				internalInfo = typedEntity.Type as IInternalEntity;
				if (null == internalInfo) return;
			}

			Node node = internalInfo.Node;
			switch (node.NodeType)
			{
				case NodeType.Property:
				case NodeType.Field:
					{
						IMember memberEntity = (IMember)entity;
						if (EntityType.Property == entity.EntityType
							|| TypeSystemServices.IsUnknown(memberEntity.Type))
						{
							VisitMemberForTypeResolution(node);
							AssertTypeIsKnown(sourceNode, memberEntity, memberEntity.Type);
						}
						break;
					}

				case NodeType.Method:
					{
						IMethod methodEntity = (IMethod)entity;
						if (TypeSystemServices.IsUnknown(methodEntity.ReturnType))
						{
							// try to preprocess the method to resolve its return type
							Method method = (Method)node;
							PreProcessMethod(method);
							if (TypeSystemServices.IsUnknown(methodEntity.ReturnType))
							{
								// still unknown?
								VisitMemberForTypeResolution(node);
								AssertTypeIsKnown(sourceNode, methodEntity, methodEntity.ReturnType);
							}
						}
						break;
					}
				case NodeType.ClassDefinition:
				case NodeType.StructDefinition:
				case NodeType.InterfaceDefinition:
					{
						if (WasVisited(node)) break;
						VisitInParentNamespace(node);
						//visit dependent attributes such as EnumeratorItemType
						//foreach (Attribute att in ((TypeDefinition)node).Attributes)
						{
							//VisitMemberForTypeResolution(att);
						}
						break;
					}
			}
		}

		private void VisitInParentNamespace(Node node)
		{
			INamespace saved = NameResolutionService.CurrentNamespace;
			try
			{
				NameResolutionService.EnterNamespace((INamespace)node.ParentNode.Entity);
				Visit(node);
			}
			finally
			{
				NameResolutionService.Restore(saved);
			}
		}


		private void AssertTypeIsKnown(Node sourceNode, IEntity sourceEntity, IType type)
		{
			if (TypeSystemServices.IsUnknown(type))
			{
				Error(
					CompilerErrorFactory.UnresolvedDependency(
						sourceNode,
						CurrentMember.FullName,
						sourceEntity.FullName));
			}
		}

		void VisitMemberForTypeResolution(Node node)
		{
			if (WasVisited(node)) return;

			_context.TraceVerbose("Info {0} needs resolving.", node.Entity.Name);
			INamespace saved = NameResolutionService.CurrentNamespace;
			try
			{
				Visit(node);
			}
			finally
			{
				NameResolutionService.Restore(saved);
			}
		}

		bool ResolveOperator(UnaryExpression node)
		{
			MethodInvocationExpression mie = new MethodInvocationExpression(node.LexicalInfo);
			mie.Arguments.Add(node.Operand.CloneNode());

			string operatorName = AstUtil.GetMethodNameForOperator(node.Operator);
			IType operand = GetExpressionType(node.Operand);
			if (ResolveOperator(node, operand, operatorName, mie))
			{
				return true;
			}
			return ResolveOperator(node, TypeSystemServices.RuntimeServicesType, operatorName, mie);
		}

		bool ResolveOperator(BinaryExpression node)
		{
			MethodInvocationExpression mie = new MethodInvocationExpression(node.LexicalInfo);
			mie.Arguments.Add(node.Left.CloneNode());
			mie.Arguments.Add(node.Right.CloneNode());

			string operatorName = AstUtil.GetMethodNameForOperator(node.Operator);
			IType lhs = GetExpressionType(node.Left);
			if (ResolveOperator(node, lhs, operatorName, mie))
			{
				return true;
			}

			IType rhs = GetExpressionType(node.Right);
			if (ResolveOperator(node, rhs, operatorName, mie))
			{
				return true;
			}

			//pointer arithmetic
			if (lhs.IsPointer && TypeSystemServices.IsIntegerNumber(rhs))
			{
				switch (node.Operator)
				{
					case BinaryOperatorType.Addition:
					case BinaryOperatorType.Subtraction:
						return true;
				}
			}

			return ResolveRuntimeOperator(node, operatorName, mie);
		}

		protected virtual bool ResolveRuntimeOperator(BinaryExpression node, string operatorName, MethodInvocationExpression mie)
		{
			return ResolveOperator(node, TypeSystemServices.RuntimeServicesType, operatorName, mie);
		}

		IMethod ResolveAmbiguousOperator(IEntity[] entities, ExpressionCollection args)
		{
			foreach (IEntity entity in entities)
			{
				IMethod method = entity as IMethod;
				if (null != method)
				{
					if (HasOperatorSignature(method, args))
					{
						return method;
					}
				}
			}
			return null;
		}

		bool HasOperatorSignature(IMethod method, ExpressionCollection args)
		{
			return method.IsStatic &&
				(args.Count == method.GetParameters().Length) &&
				CheckParameterTypesStrictly(method, args);
		}

		IMethod FindOperator(IType type, string operatorName, ExpressionCollection args)
		{
			IEntity entity = NameResolutionService.Resolve(type, operatorName, EntityType.Method);
			if (null != entity)
			{
				IMethod method = ResolveOperatorEntity(entity, args);
				if (null != method) return method;
			}

			entity = NameResolutionService.ResolveExtension(type, operatorName);
			if (null != entity)
			{
				return ResolveOperatorEntity(entity, args);
			}

			return null;
		}

		private IMethod ResolveOperatorEntity(IEntity op, ExpressionCollection args)
		{
			if (EntityType.Ambiguous == op.EntityType)
			{
				return ResolveAmbiguousOperator(((Ambiguous)op).Entities, args);
			}

			if (EntityType.Method == op.EntityType)
			{
				IMethod candidate = (IMethod)op;
				if (HasOperatorSignature(candidate, args))
				{
					return candidate;
				}
			}
			return null;
		}

		bool ResolveOperator(Expression node, IType type, string operatorName, MethodInvocationExpression mie)
		{
			IMethod entity = FindOperator(type, operatorName, mie.Arguments);
			if (null == entity)
				return false;
			EnsureRelatedNodeWasVisited(node, entity);

			mie.Target = new ReferenceExpression(entity.FullName);

			IMethod operatorMethod = entity;
			BindExpressionType(mie, operatorMethod.ReturnType);
			BindExpressionType(mie.Target, operatorMethod.Type);
			Bind(mie.Target, entity);

			node.ParentNode.Replace(node, mie);

			return true;
		}

		Expression AssertBoolContext(Expression expression)
		{
			IType type = GetExpressionType(expression);
			if (TypeSystemServices.IsNumberOrBool(type) || type.IsEnum)
				return expression;

			IMethod op_Implicit = TypeSystemServices.FindImplicitConversionOperator(type, TypeSystemServices.BoolType);
			if (null != op_Implicit)
			{
				//return [| $op_Implicit($expression) |]
				expression.Annotate("op_Implicit", op_Implicit); //for logical operator use (trueness only)
				return CodeBuilder.CreateMethodInvocation(op_Implicit, expression);
			}

			// nullable types can be used in bool context
			if (TypeSystemServices.IsNullable(type))
			{
				//return [| $(expression).HasValue |]
				MemberReferenceExpression mre = new MemberReferenceExpression(expression, "HasValue");
				Visit(mre);
				return mre;
			}

			// string in a boolean context means string.IsNullOrEmpty (BOO-1035)
			if (TypeSystemServices.StringType == type)
			{
				//return [| not string.IsNullOrEmpty($expression) |]
				Expression mie = CodeBuilder.CreateMethodInvocation(String_IsNullOrEmpty, expression);
				Expression not = new UnaryExpression(UnaryOperatorType.LogicalNot, mie);
				Visit(not);
				return not;
			}

			// reference types can be used in bool context
			if (!type.IsValueType)
				return expression;

			Error(CompilerErrorFactory.BoolExpressionRequired(expression, type.ToString()));
			return expression;
		}

		ReferenceExpression CreateTempLocal(LexicalInfo li, IType type)
		{
			InternalLocal local = DeclareTempLocal(type);
			ReferenceExpression reference = new ReferenceExpression(li, local.Name);
			reference.Entity = local;
			reference.ExpressionType = type;
			return reference;
		}

		protected InternalLocal DeclareTempLocal(IType localType)
		{
			return CodeBuilder.DeclareTempLocal(_currentMethod.Method, localType);
		}

		IEntity DeclareLocal(Node sourceNode, string name, IType localType)
		{
			return DeclareLocal(sourceNode, name, localType, false);
		}

		virtual protected IEntity DeclareLocal(Node sourceNode, string name, IType localType, bool privateScope)
		{
			Local local = new Local(name, privateScope);
			local.LexicalInfo = sourceNode.LexicalInfo;
			InternalLocal entity = new InternalLocal(local, localType);
			local.Entity = entity;
			_currentMethod.Method.Locals.Add(local);
			return entity;
		}

		protected IType CurrentType
		{
			get
			{
				return _currentMethod.DeclaringType;
			}
		}

		void PushMember(TypeMember member)
		{
			_memberStack.Push(member);
		}

		TypeMember CurrentMember
		{
			get
			{
				return (TypeMember)_memberStack.Peek();
			}
		}

		void PopMember()
		{
			_memberStack.Pop();
		}

		void PushMethodInfo(InternalMethod tag)
		{
			_methodStack.Push(_currentMethod);

			_currentMethod = tag;
		}

		void PopMethodInfo()
		{
			_currentMethod = (InternalMethod)_methodStack.Pop();
		}

		void AssertHasSideEffect(Expression expression)
		{
			if (!HasSideEffect(expression) && !TypeSystemServices.IsError(expression))
			{
				Error(CompilerErrorFactory.ExpressionMustBeExecutedForItsSideEffects(expression));
			}
		}

		protected virtual bool HasSideEffect(Expression node)
		{
			return
				node.NodeType == NodeType.MethodInvocationExpression ||
				AstUtil.IsAssignment(node) ||
				AstUtil.IsIncDec(node);
		}

		bool AssertCanCreateInstance(Node sourceNode, IType type)
		{
			if (type.IsInterface)
			{
				Error(CompilerErrorFactory.CantCreateInstanceOfInterface(sourceNode, type.ToString()));
				return false;
			}
			if (type.IsEnum)
			{
				Error(CompilerErrorFactory.CantCreateInstanceOfEnum(sourceNode, type.ToString()));
				return false;
			}
			if (type.IsAbstract)
			{
				Error(CompilerErrorFactory.CantCreateInstanceOfAbstractType(sourceNode, type.ToString()));
				return false;
			}
			if (!(type is GenericConstructedType)
				&&
				  ((type.GenericInfo != null
				   && type.GenericInfo.GenericParameters.Length > 0)
			   || (type.ConstructedInfo != null
				   && !type.ConstructedInfo.FullyConstructed))
			   )
			{
				Error(CompilerErrorFactory.GenericTypesMustBeConstructedToBeInstantiated(sourceNode));
				return false;
			}
			return true;
		}

		bool AssertDeclarationName(Declaration d)
		{
			if (AssertIdentifierName(d, d.Name))
			{
				return AssertUniqueLocal(d);
			}
			return false;
		}

		bool AssertUniqueLocal(Declaration d)
		{
			if (null == _currentMethod.ResolveLocal(d.Name) &&
				null == _currentMethod.ResolveParameter(d.Name))
			{
				return true;
			}
			Error(CompilerErrorFactory.LocalAlreadyExists(d, d.Name));
			return false;
		}

		void GetDeclarationType(IType defaultDeclarationType, Declaration d)
		{
			if (null != d.Type)
			{
				Visit(d.Type);
				AssertTypeCompatibility(d, GetType(d.Type), defaultDeclarationType);
			}
			else
			{
				d.Type = CodeBuilder.CreateTypeReference(defaultDeclarationType);
			}
		}

		IEntity DeclareLocal(Declaration d, bool privateScope)
		{
			AssertIdentifierName(d, d.Name);

			IEntity local = DeclareLocal(d, d.Name, GetType(d.Type), privateScope);
			d.Entity = local;

			InternalLocal internalLocal = local as InternalLocal;
			if (null != internalLocal)
				internalLocal.OriginalDeclaration = d;

			return local;
		}

		protected IType GetEnumeratorItemType(IType iteratorType)
		{
			return TypeSystemServices.GetEnumeratorItemType(iteratorType);
		}

		protected void ProcessDeclarationsForIterator(DeclarationCollection declarations, IType iteratorType)
		{
			IType defaultDeclType = GetEnumeratorItemType(iteratorType);
			if (declarations.Count > 1)
			{
				// will enumerate (unpack) each item
				defaultDeclType = GetEnumeratorItemType(defaultDeclType);
			}
			else if (declarations.Count == 1) //local reuse (BOO-1111)
			{
				Declaration d = declarations[0];
				Local local = AstUtil.GetLocalByName(_currentMethod.Method, d.Name);
				if (null != local) {
					GetDeclarationType(defaultDeclType, d);
					AssertTypeCompatibility(d, GetType(d.Type), ((InternalLocal) local.Entity).Type);
					d.Entity = local.Entity;
					return; //okay we reuse a previously declared local
				}
			}

			foreach (Declaration d in declarations)
			{
				ProcessDeclarationForIterator(d, defaultDeclType);
			}
		}

		protected void ProcessDeclarationForIterator(Declaration d, IType defaultDeclType)
		{
			GetDeclarationType(defaultDeclType, d);
			DeclareLocal(d, true);
		}

		protected virtual bool AssertLValue(Node node)
		{
			IEntity entity = node.Entity;
			if (null != entity) return AssertLValue(node, entity);

			if (IsArraySlicing(node)) return true;
			Error(CompilerErrorFactory.LValueExpected(node));
			return false;
		}

		protected virtual bool AssertLValue(Node node, IEntity entity)
		{
			if (null != entity)
			{
				switch (entity.EntityType)
				{
					case EntityType.Parameter:
					case EntityType.Local:
					case EntityType.Event: //for Event=null case (other => EventIsNotAnExpression)
						{
							return true;
						}

					case EntityType.Property:
						{
							if (null == ((IProperty)entity).GetSetMethod())
							{
								Error(CompilerErrorFactory.PropertyIsReadOnly(AstUtil.GetMemberAnchor(node), entity.FullName));
								return false;
							}
							return true;
						}

					case EntityType.Field:
						{
							IField fld = (IField)entity;
							if (TypeSystemServices.IsReadOnlyField(fld))
							{
								if (EntityType.Constructor == _currentMethod.EntityType
									&& _currentMethod.DeclaringType == fld.DeclaringType
									&& fld.IsStatic == _currentMethod.IsStatic)
								{
									InternalField ifld = entity as InternalField;
									if (null != ifld && ifld.IsStatic)
										ifld.StaticValue = null; //downgrade 'literal' to 'init-only'
								}
								else
								{
									Error(CompilerErrorFactory.FieldIsReadonly(AstUtil.GetMemberAnchor(node), entity.FullName));
									return false;
								}
							}
							return true;
						}
				}
			}
			Error(CompilerErrorFactory.LValueExpected(node));
			return false;
		}

		public static bool IsArraySlicing(Node node)
		{
			if (node.NodeType != NodeType.SlicingExpression) return false;
			IType type = ((SlicingExpression)node).Target.ExpressionType;
			return null != type && type.IsArray;
		}

		bool IsStandaloneReference(Node node)
		{
			Node parent = node.ParentNode;
			if (parent is GenericReferenceExpression)
			{
				parent = parent.ParentNode;
			}

			return parent.NodeType != NodeType.MemberReferenceExpression;
		}

		string GetSignature(IEnumerable args)
		{
			StringBuilder sb = new StringBuilder("(");
			foreach (Expression arg in args)
			{
				if (sb.Length > 1)
				{
					sb.Append(", ");
				}
				if (AstUtil.IsExplodeExpression(arg))
				{
					sb.Append('*');
				}
				sb.Append(GetExpressionType(arg));
			}
			sb.Append(")");
			return sb.ToString();
		}

		void InvalidOperatorForType(UnaryExpression node)
		{
			Error(node, CompilerErrorFactory.InvalidOperatorForType(node,
																	GetUnaryOperatorText(node.Operator),
																	GetExpressionType(node.Operand).ToString()));
		}

		void InvalidOperatorForTypes(BinaryExpression node)
		{
			Error(node, CompilerErrorFactory.InvalidOperatorForTypes(node,
																	 GetBinaryOperatorText(node.Operator),
																	 GetExpressionType(node.Left).ToString(),
																	 GetExpressionType(node.Right).ToString()));
		}


		bool AreTypesRelated(Node node, IType lhs, IType rhs)
		{
			bool byDowncast;
			bool result = TypeSystemServices.AreTypesRelated(lhs, rhs, out byDowncast);
			if (!result)
				return false;

			if (byDowncast)
			{
				Warnings.Add(CompilerWarningFactory.ImplicitDowncast(node, lhs, rhs));
				if (Parameters.Strict && Parameters.DisabledWarnings.Contains("BCW0028"))
					return false; //get a regular context-dependent error
			}
			return true;
		}


		void TraceReturnType(Method method, IMethod tag)
		{
			_context.TraceInfo("{0}: return type for method {1} bound to {2}", method.LexicalInfo, method.Name, tag.ReturnType);
		}

		#region Method bindings cache
		IMethod RuntimeServices_Len
		{
			get { return CachedRuntimeServicesMethod("Len"); }
		}

		IMethod RuntimeServices_Mid
		{
			get { return CachedRuntimeServicesMethod("Mid"); }
		}

		IMethod RuntimeServices_NormalizeStringIndex
		{
			get { return CachedRuntimeServicesMethod("NormalizeStringIndex"); }
		}

		IMethod RuntimeServices_AddArrays
		{
			get { return CachedRuntimeServicesMethod("AddArrays"); }
		}

		IMethod RuntimeServices_GetRange1
		{
			get { return CachedRuntimeServicesMethod("GetRange1"); }
		}

		IMethod RuntimeServices_GetRange2
		{
			get { return CachedRuntimeServicesMethod("GetRange2"); }
		}

		IMethod RuntimeServices_GetMultiDimensionalRange1
		{
			get { return CachedRuntimeServicesMethod("GetMultiDimensionalRange1"); }
		}

		IMethod RuntimeServices_SetMultiDimensionalRange1
		{
			get { return CachedRuntimeServicesMethod("SetMultiDimensionalRange1"); }
		}

		IMethod RuntimeServices_GetEnumerable
		{
			get { return CachedRuntimeServicesMethod("GetEnumerable"); }
		}


		private IMethod CachedRuntimeServicesMethod(string methodName)
		{
			return CachedMethod("RuntimeServices_" + methodName, delegate
									 {
										 return ResolveMethod(TypeSystemServices.RuntimeServicesType,
															  methodName);
									 });
		}

		IMethod RuntimeServices_EqualityOperator
		{
			get
			{
				return CachedMethod("RuntimeServices_EqualityOperator", delegate
				                                                 {
				                                                 	return TypeSystemServices.Map(Types.RuntimeServices.GetMethod("EqualityOperator", new Type[] { Types.Object, Types.Object }));;
				                                                 });
			}
		}

		IMethod Array_get_Length
		{
			get
			{
				return CachedMethod("Array_get_Length", delegate
				                                                 {
				                                                 	return ResolveProperty(TypeSystemServices.ArrayType, "Length").GetGetMethod();
				                                                 });
			}
		}

		IMethod Array_GetLength
		{
			get
			{
				return CachedMethod("Array_GetLength", delegate
				                                                 {
				                                                 	return ResolveMethod(TypeSystemServices.ArrayType, "GetLength");;
				                                                 });
			}
		}

		IMethod Array_EnumerableConstructor
		{
			get
			{
				return CachedMethod("Array_EnumerableConstructor", delegate
				                                                 {
				                                                 	return TypeSystemServices.Map(Types.Builtins.GetMethod("array", new Type[] { Types.IEnumerable }));
				                                                 });
			}
		}

		IMethod Array_TypedEnumerableConstructor
		{
			get
			{
				return CachedMethod("Array_TypedEnumerableConstructor", delegate
				                                                 {
				                                                 	return TypeSystemServices.Map(Types.Builtins.GetMethod("array", new Type[] { Types.Type, Types.IEnumerable }));
				                                                 });
			}
		}

		IMethod Array_TypedCollectionConstructor
		{
			get
			{
				return CachedMethod("Array_TypedCollectionConstructor", delegate
				                                                 {
				                                                 	return TypeSystemServices.Map(Types.Builtins.GetMethod("array", new Type[] { Types.Type, Types.ICollection }));;
				                                                 });
			}
		}

		IMethod Array_TypedIntConstructor
		{
			get
			{
				return CachedMethod("Array_TypedIntConstructor", delegate
				                                                 {
				                                                 	return TypeSystemServices.Map(Types.Builtins.GetMethod("array", new Type[] { Types.Type, Types.Int }));;
				                                                 });
			}
		}

		IMethod MultiDimensionalArray_TypedConstructor
		{
			get
			{
				return CachedMethod("MultiDimensionalArray_TypedConstructor", delegate
				                                                 {
				                                                 	return TypeSystemServices.Map(Types.Builtins.GetMethod("matrix", new Type[] { Types.Type, typeof(int[]) }));;
				                                                 });
			}
		}

		IMethod String_get_Length
		{
			get
			{
				return CachedMethod("String_get_Length", delegate
				                                                 {
				                                                 	return ResolveProperty(TypeSystemServices.StringType, "Length").GetGetMethod();;
				                                                 });
			}
		}

		IMethod String_IsNullOrEmpty
		{
			get
			{
				return CachedMethod("String_IsNullOrEmpty", delegate
				                                                 {
				                                                 	return TypeSystemServices.Map(Types.String.GetMethod("IsNullOrEmpty"));;
				                                                 });
			}
		}

		IMethod String_Substring_Int
		{
			get
			{
				return CachedMethod("String_Substring_Int", delegate
				                                                 {
				                                                 	return TypeSystemServices.Map(Types.String.GetMethod("Substring", new Type[] { Types.Int }));;
				                                                 });
			}
		}

		IMethod ICollection_get_Count
		{
			get
			{
				return CachedMethod("ICollection_get_Count", delegate
				                                                 {
				                                                 	return ResolveProperty(TypeSystemServices.ICollectionType, "Count").GetGetMethod();;
				                                                 });
			}
		}

		IMethod List_GetRange1
		{
			get
			{
				return CachedMethod("List_GetRange1", delegate
				                                                 {
				                                                 	return TypeSystemServices.Map(Types.List.GetMethod("GetRange", new Type[] { typeof(int) }));;
				                                                 });
			}
		}

		IMethod List_GetRange2
		{
			get
			{
				return CachedMethod("List_GetRange2", delegate
				                                                 {
				                                                 	return TypeSystemServices.Map(Types.List.GetMethod("GetRange", new Type[] { typeof(int), typeof(int) }));;
				                                                 });
			}
		}

		IMethod ICallable_Call
		{
			get
			{
				return CachedMethod("ICallable_Call", delegate
				                                                 {
																	 return ResolveMethod(TypeSystemServices.ICallableType, "Call"); ;
																 });
			}
		}

		Dictionary<string, IMethodBase> _methodCache;

		private delegate IMethodBase MethodProducer();

		IMethod CachedMethod(string key, MethodProducer producer)
		{
			return (IMethod)CachedMethodBase(key, producer);
		}

		IConstructor CachedConstructor(string key, MethodProducer producer)
		{
			return (IConstructor)CachedMethodBase(key, producer);
		}

		private IMethodBase CachedMethodBase(string key, MethodProducer producer)
		{
			IMethodBase method;
			if (!_methodCache.TryGetValue(key, out method))
			{
				method = producer();
				_methodCache.Add(key, method);
			}
			return method;
		}

		IMethod Activator_CreateInstance
		{
			get
			{
				return CachedMethod("Activator_CreateInstance", delegate
				                                                {
				                                                	return
				                                                		TypeSystemServices.Map(
				                                                			typeof(Activator).GetMethod("CreateInstance",
				                                                			                            new Type[]
				                                                			                            {
				                                                			                            	Types.Type,
				                                                			                            	Types.ObjectArray
				                                                			                            }));
				                                                });
			}
		}

		IConstructor Exception_StringConstructor
		{
			get
			{
				return CachedConstructor("Exception_StringConstructor", delegate
				{
					return TypeSystemServices.GetStringExceptionConstructor();
				});
			}
		}

		IMethod TextReaderEnumerator_lines
		{
			get
			{
				return CachedMethod("TextReaderEnumerator_lines", delegate
				{
					return TypeSystemServices.Map(typeof(TextReaderEnumerator).GetMethod("lines"));
				});
			}
		}
		#endregion


		public bool OptimizeNullComparisons
		{
			get { return _optimizeNullComparisons; }
			set { _optimizeNullComparisons = value; }
		}
	}
}

