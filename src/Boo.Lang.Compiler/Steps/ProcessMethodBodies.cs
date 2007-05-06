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
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Ast.Visitors;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Runtime;
using Attribute = Boo.Lang.Compiler.Ast.Attribute;

namespace Boo.Lang.Compiler.Steps
{
	/// <summary>
	/// AST semantic evaluation.
	/// </summary>
	public class ProcessMethodBodies : AbstractNamespaceSensitiveVisitorCompilerStep
	{
		static readonly ExpressionCollection EmptyExpressionCollection = new ExpressionCollection();

		static readonly object OptionalReturnStatementAnnotation = new object();
		
		protected Stack _methodStack;

		protected Stack _memberStack;
		// for accurate error reporting during type inference
		
		protected InternalMethod _currentMethod;

		IMethod Array_EnumerableConstructor;
		
		IMethod Array_TypedEnumerableConstructor;
		
		IMethod Array_TypedCollectionConstructor;
		
		IMethod Array_TypedConstructor2;

		IMethod MultiDimensionalArray_TypedConstructor;
		
		InfoFilter IsPublicEventFilter;
		
		InfoFilter IsPublicFieldPropertyEventFilter;

		protected CallableResolutionService _callableResolution;

		protected bool _optimizeNullComparisons = true;
		
		public ProcessMethodBodies()
		{
			IsPublicFieldPropertyEventFilter = new InfoFilter(IsPublicFieldPropertyEvent);
			IsPublicEventFilter = new InfoFilter(IsPublicEvent);
		}

		override public void Run()
		{
			NameResolutionService.Reset();
			
			_currentMethod = null;
			_methodStack = new Stack();
			_memberStack = new Stack();

			_callableResolution = new CallableResolutionService();
			_callableResolution.Initialize(_context);
			
			InitializeMemberCache();
			
			Visit(CompileUnit);
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
			Array_EnumerableConstructor = TypeSystemServices.Map(Types.Builtins.GetMethod("array", new Type[] { Types.IEnumerable }));
			Array_TypedEnumerableConstructor = TypeSystemServices.Map(Types.Builtins.GetMethod("array", new Type[] { Types.Type, Types.IEnumerable }));
			Array_TypedCollectionConstructor= TypeSystemServices.Map(Types.Builtins.GetMethod("array", new Type[] { Types.Type, Types.ICollection }));
			Array_TypedConstructor2 = TypeSystemServices.Map(Types.Builtins.GetMethod("array", new Type[] { Types.Type, Types.Int }));
			MultiDimensionalArray_TypedConstructor = TypeSystemServices.Map(Types.Builtins.GetMethod("matrix", new Type[] { Types.Type, typeof(int[]) }));
		}
		
		override public void Dispose()
		{
			base.Dispose();
			
			if (null != _callableResolution)
			{
				_callableResolution.Dispose();
				_callableResolution = null;
			}

			_currentMethod = null;
			_methodStack = null;
			_memberStack = null;

			_RuntimeServices_Len = null;
			_RuntimeServices_Mid = null;
			_RuntimeServices_NormalizeStringIndex = null;
			_RuntimeServices_AddArrays = null;
			_RuntimeServices_GetRange1 = null;
			_RuntimeServices_GetRange2 = null;
			_RuntimeServices_GetMultiDimensionalRange1 = null;
			_RuntimeServices_SetMultiDimensionalRange1 = null;
			_RuntimeServices_GetEnumerable = null;
			_RuntimeServices_EqualityOperator = null;
			_Array_get_Length = null;
			_Array_GetLength = null;
			_String_get_Length = null;
			_String_Substring_Int = null;
			_ICollection_get_Count = null;
			_List_GetRange1 = null;
			_List_GetRange2 = null;
			_ICallable_Call = null;
			_Activator_CreateInstance = null;
			_ApplicationException_StringConstructor = null;
			_TextReaderEnumerator_Constructor = null;
			_EnumeratorItemType_Constructor = null;
		}

		override public void OnModule(Module module)
		{
			if (WasVisited(module)) return;
			MarkVisited(module);
			
			EnterNamespace((INamespace)TypeSystemServices.GetEntity(module));
			
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
			ProcessFieldInitializers(node);
		}

		void ProcessFieldInitializers(ClassDefinition node)
		{
			foreach (TypeMember member in node.Members)
			{
				if (NodeType.Field == member.NodeType)
				{
					ProcessFieldInitializer((Field) member);
				}
			}
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
					typeInfo = GetType(node.Getter.ReturnType);
					if (typeInfo == TypeSystemServices.VoidType)
					{
						typeInfo = TypeSystemServices.ObjectType;
						node.Getter.ReturnType = CodeBuilder.CreateTypeReference(typeInfo);
					}
				}
				else
				{
					typeInfo = TypeSystemServices.ObjectType;
				}
				node.Type = CodeBuilder.CreateTypeReference(typeInfo);
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
		
		override public void OnField(Field node)
		{
			if (WasVisited(node))
			{
				return;
			}
			MarkVisited(node);
			
			InternalField tag = (InternalField)GetEntity(node);
			
			Visit(node.Attributes);
			Visit(node.Type);
			
			if (null != node.Initializer)
			{
				if (tag.DeclaringType.IsValueType && !node.IsStatic)
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
					node.Type = CodeBuilder.CreateTypeReference(TypeSystemServices.ObjectType);
					node.Type.LexicalInfo = node.LexicalInfo;
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
				node.Type = CodeBuilder.CreateTypeReference(initializerType);
				node.Type.LexicalInfo = node.LexicalInfo;
			}
			else
			{
				AssertTypeCompatibility(node.Initializer, GetType(node.Type), initializerType);
			}
		}
		
		void PreProcessFieldInitializer(Field node)
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
			
			Method method = GetFieldsInitializerMethod(node);
			InternalMethod entity = (InternalMethod)method.Entity;
			
			ReferenceExpression temp = new ReferenceExpression("___temp_initializer");
			BinaryExpression assignment = new BinaryExpression(
				node.LexicalInfo,
				BinaryOperatorType.Assign,
				temp,
				initializer);

			ProcessNodeInMethodContext(entity, entity, assignment);
			method.Locals.RemoveByEntity(temp.Entity);
			
			IType initializerType = ((ITypedEntity)temp.Entity).Type;
			ProcessFieldInitializerType(node, initializerType);
			node.Initializer = assignment.Right;
		}

		void ProcessFieldInitializer(Field node)
		{
			Expression initializer = node.Initializer;
			if (null == initializer) return;

			Method method = GetFieldsInitializerMethod(node);
			method.Body.Add(
				CodeBuilder.CreateAssignment(
					CodeBuilder.CreateReference(node),
					initializer));
			node.Initializer = null;
		}
		
		Method CreateInitializerMethod(TypeDefinition type, string name, TypeMemberModifiers modifiers)
		{
			Method method = new Method(name);
			method.Modifiers |= modifiers;
			method.ReturnType = CodeBuilder.CreateTypeReference(TypeSystemServices.VoidType);
			
			InternalMethod entity = new InternalMethod(TypeSystemServices, method);
			method.Entity = entity;
			type.Members.Add(method);
			MarkVisited(method);
			return method;
		}
		
		Method GetFieldsInitializerMethod(Field node)
		{
			TypeDefinition type = node.DeclaringType;
			string methodName = node.IsStatic ? "___static_initializer" : "___initializer";
			Method method = (Method)type[methodName];
			if (null == method)
			{
				if (node.IsStatic)
				{
					if (null == FindStaticConstructor(type))
					{
						// when the class doesnt have a static constructor
						// yet, create one and use it as the static
						// field initializer method
						method = CreateStaticConstructor(type);
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
					AddInitializerToInstanceConstructors(type, (InternalMethod)method.Entity);
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
		
		void AddInitializerToInstanceConstructors(TypeDefinition type, InternalMethod initializer)
		{
			foreach (TypeMember node in type.Members)
			{
				if (NodeType.Constructor == node.NodeType && !node.IsStatic)
				{
					Constructor constructor = (Constructor)node;
					constructor.Body.Insert(GetIndexAfterSuperInvocation(constructor.Body),
					                        CodeBuilder.CreateMethodInvocation(
					                        	CodeBuilder.CreateSelfReference((IType)type.Entity),
					                        	initializer));
				}
			}
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
		
		Constructor FindStaticConstructor(TypeDefinition type)
		{
			foreach (TypeMember member in type.Members)
			{
				if (member.IsStatic && NodeType.Constructor == member.NodeType)
				{
					return (Constructor)member;
				}
			}
			return null;
		}
		
		Constructor GetStaticConstructor(TypeDefinition type)
		{
			Constructor constructor = FindStaticConstructor(type);
			if (null == constructor)
			{
				constructor = CreateStaticConstructor(type);
			}
			return constructor;
		}
		
		Constructor CreateStaticConstructor(TypeDefinition type)
		{
			Constructor constructor = new Constructor();
			constructor.Entity = new InternalConstructor(TypeSystemServices, constructor);
			constructor.Modifiers = TypeMemberModifiers.Public|TypeMemberModifiers.Static;
			type.Members.Add(constructor);
			MarkVisited(constructor);
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
			if (method.Body.Statements.Count > 0)
			{
				Error(CompilerErrorFactory.RuntimeMethodBodyMustBeEmpty(method, method.FullName));
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
					if (0 == node.Parameters.Count &&
					    !node.IsSynthetic)
					{
						Error(
							CompilerErrorFactory.ValueTypesCannotDeclareParameterlessConstructors(node));
					}
				}
				else if (
					!entity.HasSelfCall	&&
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
		
		override public void OnCallableBlockExpression(CallableBlockExpression node)
		{
			if (WasVisited(node)) return;
			MarkVisited(node);

			TypeMemberModifiers modifiers = TypeMemberModifiers.Internal;
			if (_currentMethod.IsStatic)
			{
				modifiers |= TypeMemberModifiers.Static;
			}

			Method closure = CodeBuilder.CreateMethod(
				"___closure" + _context.AllocIndex(),
				Unknown.Default,
				modifiers);

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
			
			// Connects the closure method namespace with the current
			NamespaceDelegator ns = new NamespaceDelegator(CurrentNamespace, closureEntity);
			ProcessMethodBody(closureEntity, ns);
			TryToResolveReturnType(closureEntity);
			
			node.ExpressionType = closureEntity.Type;
			node.Entity = closureEntity;
		}

		private void AddOptionalReturnStatement(Block body)
		{
			if (body.Statements.Count != 1) return;
			ExpressionStatement stmt = body.Statements[0] as ExpressionStatement;
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
			
			bool ispinvoke = ((IMethod)GetEntity(method)).IsPInvoke;
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
			if (null != candidates)
			{
				IMethod baseMethod = null;
				if (EntityType.Method == candidates.EntityType)
				{
					IMethod candidate = (IMethod)candidates;
					if (TypeSystemServices.CheckOverrideSignature(entity, candidate))
					{
						baseMethod = candidate;
					}
				}
				else if (EntityType.Ambiguous == candidates.EntityType)
				{
					IEntity[] entities = ((Ambiguous)candidates).Entities;
					foreach (IMethod candidate in entities)
					{
						if (TypeSystemServices.CheckOverrideSignature(entity, candidate))
						{
							baseMethod = candidate;
							break;
						}
					}
				}
				if (null != baseMethod)
				{
					EnsureRelatedNodeWasVisited(method, baseMethod);
				}
				return baseMethod;
			}
			return null;
		}
		
		void ResolveMethodOverride(InternalMethod entity)
		{
			IMethod baseMethod = FindMethodOverride(entity);
			if (null == baseMethod)
			{
				Error(CompilerErrorFactory.NoMethodToOverride(entity.Method, entity.ToString()));
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
			if (TypeSystemServices.IsUnknown(entity.ReturnType))
			{
				entity.Method.ReturnType = CodeBuilder.CreateTypeReference(baseMethod.ReturnType);
			}
			else
			{
				if (baseMethod.ReturnType != entity.ReturnType)
				{
					Error(CompilerErrorFactory.InvalidOverrideReturnType(
						entity.Method.ReturnType,
						baseMethod.FullName,
						baseMethod.ReturnType.ToString(),
						entity.ReturnType.ToString()));
				}
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

		class ReturnExpressionFinder : DepthFirstVisitor
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
						node.ReturnType = CodeBuilder.CreateTypeReference(TypeSystemServices.VoidType);
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

			InternalMethod entity = (InternalMethod)GetEntity(node);
			ProcessMethodBody(entity);
			
			bool parentIsClass = node.DeclaringType.NodeType == NodeType.ClassDefinition;
			if (parentIsClass)
			{
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
		}
		
		void CheckGeneratorCantReturnValues(InternalMethod entity)
		{
			if (entity.IsGenerator && null != entity.ReturnExpressions)
			{
				foreach (Expression e in entity.ReturnExpressions)
				{
					Error(CompilerErrorFactory.GeneratorCantReturnValue(e));
				}
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
#if NET_2_0
			return returnType.GenericTypeInfo != null &&
			    (returnType.GenericTypeInfo.GenericDefinition == TypeSystemServices.IEnumerableGenericType ||
			     returnType.GenericTypeInfo.GenericDefinition == TypeSystemServices.IEnumeratorGenericType);
#else
			return false;
#endif
		}
		
		void CheckGeneratorYieldType(InternalMethod method, IType returnType)
		{
#if NET_2_0
			if (CheckGenericGeneratorReturnType(returnType))
			{
				IType returnElementType = returnType.GenericTypeInfo.GenericArguments[0];
				
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
#endif
		}
		
		void ProcessMethodBody(InternalMethod entity)
		{
			ProcessMethodBody(entity, entity);
		}
		
		void ProcessMethodBody(InternalMethod entity, INamespace ns)
		{
			ProcessNodeInMethodContext(entity, ns, entity.Method.Body);
			if (entity.IsGenerator)
			{
				CreateGeneratorSkeleton(entity);
			}
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
			Method method = entity.Method;
#if NET_2_0
			// Make method return a generic IEnumerable
			IType enumerableType = TypeSystemServices.IEnumerableGenericType;
			IType itemType = (IType)method["GeneratorItemType"];
			
			method.ReturnType = CodeBuilder.CreateTypeReference(
				enumerableType.GenericTypeDefinitionInfo.MakeGenericType(itemType));
#else
			if (method.IsVirtual)
			{
				// Make method return a non-generic IEnumerable
				method.ReturnType = CodeBuilder.CreateTypeReference(TypeSystemServices.IEnumerableType);
			}
			else
			{
				// Otherwise use the specific type to allow type inference
				// to discover the item type
				BooClassBuilder generatorType = (BooClassBuilder)method["GeneratorClassBuilder"];
				method.ReturnType = CodeBuilder.CreateTypeReference(generatorType.Entity);
			}
#endif			
		}
		
		void TryToResolveReturnType(InternalMethod tag)
		{
			if (tag.IsGenerator)
			{
				ResolveGeneratorReturnType(tag);
			}
			else
			{
				if (CanResolveReturnType(tag))
				{
					ResolveReturnType(tag);
				}
			}
		}
		
		override public void OnSuperLiteralExpression(SuperLiteralExpression node)
		{
			node.Entity = _currentMethod;
			node.ExpressionType = _currentMethod.DeclaringType.BaseType;

			if (!AstUtil.IsTargetOfMethodInvocation(node)) return;
			if (EntityType.Constructor == _currentMethod.EntityType) return;
			if (null != _currentMethod.Overriden) return;
			
			Error(
				CompilerErrorFactory.MethodIsNotOverride(node, _currentMethod.ToString()));
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
			return (IEntity[])setMethods.ToArray(typeof(IEntity));
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
			return (IEntity[])getMethods.ToArray(typeof(IEntity));
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
					BindExpressionType(mie, TypeSystemServices.GetArrayType(arrayType.GetElementType(), node.Indices.Count - collapseCount));
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
		
		bool IsIndexedProperty(Expression expression)
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
						IEntity member = targetType.GetDefaultMember();
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
			return CodeBuilder.CreateSelfReference(this.CurrentType);
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
			CallableBlockExpression block = new CallableBlockExpression(node.LexicalInfo);
			
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
			
			BindExpressionType(node, generatorType.Entity);
		}
		
		void CreateGeneratorSkeleton(InternalMethod entity)
		{
			Method method = entity.Method;
			IType itemType = null;

#if NET_2_0
			if (CheckGenericGeneratorReturnType(entity.ReturnType))
			{
				itemType = entity.ReturnType.GenericTypeInfo.GenericArguments[0];
			}
#endif
			
			if (itemType == null)
			{
				ExpressionCollection yieldExpressions = entity.YieldExpressions;
					
				itemType = yieldExpressions.Count > 0
					? GetMostGenericType(yieldExpressions)
					: TypeSystemServices.ObjectType;
			}
			BooClassBuilder builder = CreateGeneratorSkeleton(method, method, itemType);
			TypeSystemServices.AddCompilerGeneratedType(builder.ClassDefinition);
		}
		
		BooClassBuilder CreateGeneratorSkeleton(GeneratorExpression node)
		{
			BooClassBuilder builder = CreateGeneratorSkeleton(node, _currentMethod.Method, GetConcreteExpressionType(node.Expression));
			_currentMethod.Method.DeclaringType.Members.Add(builder.ClassDefinition);
			return builder;
		}
		
		BooClassBuilder CreateGeneratorSkeleton(Node sourceNode, Method method, IType generatorItemType)
		{
			// create the class skeleton for type inference to work
			BooClassBuilder builder = CodeBuilder.CreateClass(
				string.Format("{0}___generator{1}", method.Name, _context.AllocIndex()),
				TypeMemberModifiers.Internal|TypeMemberModifiers.Final);
			builder.LexicalInfo = sourceNode.LexicalInfo;
			
			BooMethodBuilder getEnumeratorBuilder = null;
#if NET_2_0
			if (generatorItemType != TypeSystemServices.VoidType)
			{
				builder.AddBaseType(
					TypeSystemServices.Map(
						typeof(AbstractGenerator<>)).GenericTypeDefinitionInfo.MakeGenericType(generatorItemType));
				
				getEnumeratorBuilder = builder.AddVirtualMethod(
					"GetEnumerator",
					TypeSystemServices.IEnumeratorGenericType.
					GenericTypeDefinitionInfo.MakeGenericType(generatorItemType));

				getEnumeratorBuilder.Method.LexicalInfo = sourceNode.LexicalInfo;
			}
#else
			builder.AddBaseType(TypeSystemServices.Map(typeof(AbstractGenerator)));
			builder.AddAttribute(CodeBuilder.CreateAttribute(
				EnumeratorItemType_Constructor,
				CodeBuilder.CreateTypeofExpression(generatorItemType)));
			
			getEnumeratorBuilder = builder.AddVirtualMethod("GetEnumerator", TypeSystemServices.IEnumeratorType);
			getEnumeratorBuilder.Method.LexicalInfo = sourceNode.LexicalInfo;
#endif
			
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
			return TypeSystemServices.GetArrayType(GetMostGenericType(node.Items), 1);
		}

		override public void LeaveDeclaration(Declaration node)
		{
			if (null == node.Type) return;
			CheckDeclarationType(node.Type);
		}
		
		override public void LeaveDeclarationStatement(DeclarationStatement node)
		{
			IType type = TypeSystemServices.ObjectType;
			if (null != node.Declaration.Type)
			{
				type = GetType(node.Declaration.Type);
			}
			else if (null != node.Initializer)
			{
				// The boo syntax does not require this check because
				// there's no way to create an untyped declaration statement.
				// This is here to support languages that do allow untyped variable
				// declarations (unityscript is such an example).
				type = GetExpressionType(node.Initializer);
			}
			
			AssertDeclarationName(node.Declaration);
			
			IEntity localInfo = DeclareLocal(node, node.Declaration.Name, type);
			if (null != node.Initializer)
			{
				AssertTypeCompatibility(node.Initializer, type, GetExpressionType(node.Initializer));
				
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
						Error(CompilerErrorFactory.ObjectRequired(node));
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
				ReplaceByStaticFieldReference(node, "___re" + _context.AllocIndex(), type);
			}
		}
		
		void ReplaceByStaticFieldReference(Expression node, string fieldName, IType type)
		{
			Node parent = node.ParentNode;
			
			Field field = new Field(node.LexicalInfo);
			field.Name = fieldName;
			field.Type = CodeBuilder.CreateTypeReference(type);
			field.Modifiers = TypeMemberModifiers.Private|TypeMemberModifiers.Static;
			field.Initializer = node;
			
			_currentMethod.Method.DeclaringType.Members.Add(field);
			InternalField tag = new InternalField(field);
			Bind(field, tag);
			
			AddFieldInitializerToStaticConstructor(0, field);
			
			parent.Replace(node, CodeBuilder.CreateMemberReference(
				CodeBuilder.CreateReference(node.LexicalInfo, _currentMethod.DeclaringType),
				tag));
		}
		
		override public void LeaveGenericReferenceExpression(GenericReferenceExpression node)
		{
			CompilerError error = CheckGenericReferenceExpression(node);
			if (null != error)
			{
				Error(node, error);
				return;
			}

			switch (node.Target.Entity.EntityType)
			{
				case EntityType.Type:
					IType genericType = ((IType)node.Target.Entity).GenericTypeDefinitionInfo.MakeGenericType(GetGenericArguments(node));
					Bind(node, genericType);
					BindTypeReferenceExpressionType(node, genericType);
					break;
				
				case EntityType.Method:
					IMethod genericMethod = ((IMethod)node.Target.Entity).GenericMethodDefinitionInfo.MakeGenericMethod(GetGenericArguments(node));
					Bind(node, genericMethod);
					BindExpressionType(node, genericMethod.Type);
					break;				
			}
		}

		private IType[] GetGenericArguments(GenericReferenceExpression node)
		{
			IType[] types = new IType[node.GenericArguments.Count];
			for (int i = 0; i < node.GenericArguments.Count; ++i)
			{
				IType type = node.GenericArguments[i].Entity as IType;
				types[i] = type;
			}
			return types;
		}

		CompilerError CheckGenericReferenceExpression(GenericReferenceExpression node)
		{
			IEntity entity = node.Target.Entity;
			IType type = entity as IType;
			IMethod method = entity as IMethod;
			IGenericParameter[] parameters = null;

			// Test for a generic type definition			
			if (type != null && type.GenericTypeDefinitionInfo != null)
			{
				parameters = type.GenericTypeDefinitionInfo.GenericParameters;
			}
			// Test for a generic method definition
			else if (method != null && method.GenericMethodDefinitionInfo != null)
			{
				parameters = method.GenericMethodDefinitionInfo.GenericParameters;
			}
		
			if (parameters == null)
			{
				return CompilerErrorFactory.NotAGenericDefinition(AstUtil.GetMemberAnchor(node.Target), entity.FullName);
			}
			
			if (parameters.Length != node.GenericArguments.Count) 
			{
				return CompilerErrorFactory.GenericDefinitionArgumentCount(AstUtil.GetMemberAnchor(node.Target), entity.FullName, parameters.Length);
			}
			
			return null; 
		}

		override public void OnReferenceExpression(ReferenceExpression node)
		{
			if (null != node.ExpressionType)
			{
				return;
			}
			
			IEntity entity = ResolveName(node, node.Name);
			if (null != entity)
			{
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
					ResolveMemberInfo(node, member);
				}
				else
				{
					EnsureRelatedNodeWasVisited(node, entity);
					node.Entity = entity;
					PostProcessReferenceExpression(node);
				}
			}
			else
			{
				Error(node);
			}
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
						tag = ResolveAmbiguousReference(node, (Ambiguous)tag);
						IMember resolvedMember  = tag as IMember;
						if (null != resolvedMember)
						{
							ResolveMemberInfo(node, resolvedMember);
							break;
						}
						if (tag is IType)
						{
							BindTypeReferenceExpressionType(node, (IType)tag);
							break;
						}
						if (!AstUtil.IsTargetOfMethodInvocation(node)
						    && !AstUtil.IsTargetOfSlicing(node)
						    && !AstUtil.IsLhsOfAssignment(node))
						{
							Error(node, CompilerErrorFactory.AmbiguousReference(
								node,
								node.Name,
								((Ambiguous)tag).Entities));
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
			{
				Error(CompilerErrorFactory.ExplodedExpressionMustBeArray(node));
			}
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
			Error(node, CompilerErrorFactory.MemberNotFound(node, ((IEntity)ns).ToString()));
		}

		virtual protected bool ShouldRebindMember(IEntity entity)
		{
			return entity == null;
		}

		IEntity ResolveMember(MemberReferenceExpression node)
		{
			IEntity member = node.Entity;
			if (!ShouldRebindMember(member)) return member;

			INamespace ns = GetReferenceNamespace(node);
			member = NameResolutionService.Resolve(ns, node.Name);
			if (null != member) return member;
			
			member = NameResolutionService.ResolveExtension(ns, node.Name);
			if (null == member) MemberNotFound(node, ns);

			return member;
		}
		
		virtual protected void ProcessMemberReferenceExpression(MemberReferenceExpression node)
		{
			IEntity member = ResolveMember(node);
			if (null == member) return;
			
			if (EntityType.Ambiguous == member.EntityType)
			{
				member = ResolveAmbiguousReference(node, (Ambiguous)member);
			}
			
			EnsureRelatedNodeWasVisited(node, member);
			
			IMember memberInfo = member as IMember;
			if (null != memberInfo)
			{
				// methods will be checked later
				if (EntityType.Method != memberInfo.EntityType)
				{
					if (!AssertTargetContext(node, memberInfo))
					{
						Error(node);
						return;
					}
					
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
					else
					{
						Error(node,
						      CompilerErrorFactory.EventIsNotAnExpression(node,
						                                                  member.FullName));
					}
				}
			}
			
			Bind(node, member);
			PostProcessReferenceExpression(node);
		}
		
		private bool IsBeingAssignedTo(MemberReferenceExpression node)
		{
			Node current = node;
			Node parent = current.ParentNode;
			while (!(parent is BinaryExpression))
			{
				current = parent;
				parent = parent.ParentNode;
				if (parent == null || !(parent is Expression)) return false;
			}
			return ((BinaryExpression)parent).Left == current;
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
			if (!AstUtil.IsTargetOfSlicing(node)
			    && !AstUtil.IsLhsOfAssignment(node))
			{
				if (candidates.AllEntitiesAre(EntityType.Property))
				{
					return ResolveAmbiguousPropertyReference(node, candidates, EmptyExpressionCollection);
				}
				else if (candidates.AllEntitiesAre(EntityType.Method))
				{
					return ResolveAmbiguousMethodReference(node, candidates, EmptyExpressionCollection);
				}
				else if (candidates.AllEntitiesAre(EntityType.Type))
				{
					return ResolveAmbiguousTypeReference(node, candidates);
				}
			}
			return candidates;
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
			bool isGenericReference = (node.ParentNode is GenericReferenceExpression);
			bool isGenericType;
			
			List matches = new List();
			
			foreach (IEntity candidate in candidates.Entities)
			{
				IType type = candidate as IType;
				isGenericType = (type != null && type.GenericTypeDefinitionInfo != null);
				
				if (isGenericType == isGenericReference)
				{
					matches.Add(candidate);
				}
			}
			
			if (matches.Count == 1)
			{
				Bind(node, (IEntity)matches[0]);
			}
			else
			{
				Bind(node, new Ambiguous(matches));
			}
			
			return node.Entity;
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
			
			IType trueType = GetExpressionType(node.TrueValue);
			IType falseType = GetExpressionType(node.FalseValue);
			
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
			if (null == node.Expression) return;

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
						return CodeBuilder.CreateConstructorInvocation(TextReaderEnumerator_Constructor, iterator);
					}
					else
					{
						return CodeBuilder.CreateMethodInvocation(RuntimeServices_GetEnumerable, iterator);
					}
				}
				else
				{
					IMethod method = ResolveGetEnumerator(iterator, type);
					if (null == method)
					{
						Error(CompilerErrorFactory.InvalidIteratorType(iterator, type.ToString()));
					}
					else
					{
						return CodeBuilder.CreateMethodInvocation(iterator, method);
					}
				}
			}
			return iterator;
		}
		
		IMethod ResolveGetEnumerator(Node sourceNode, IType type)
		{
			IMethod method = ResolveMethod(type, "GetEnumerator");
			if (null != method)
			{
				EnsureRelatedNodeWasVisited(sourceNode, method);
				if (0 == method.GetParameters().Length &&
				    method.ReturnType.IsSubclassOf(TypeSystemServices.IEnumeratorType))
				{
					return method;
				}
			}
			return null;
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
				
				ProcessDeclarationType(defaultDeclarationType, d);
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
					ApplicationException_StringConstructor,
					node.Exception);
			}
			else if (!TypeSystemServices.ExceptionType.IsAssignableFrom(exceptionType))
			{
				Error(CompilerErrorFactory.InvalidRaiseArgument(node.Exception,
				                                                exceptionType.ToString()));
			}
		}
		
		override public void OnExceptionHandler(ExceptionHandler node)
		{
			if (null == node.Declaration)
			{
				node.Declaration = new Declaration(node.LexicalInfo,
				                                   "___exception",
				                                   CodeBuilder.CreateTypeReference(TypeSystemServices.ExceptionType));
			}
			else
			{
				if (null == node.Declaration.Type)
				{
					node.Declaration.Type = CodeBuilder.CreateTypeReference(TypeSystemServices.ExceptionType);
				}
				else
				{
					Visit(node.Declaration.Type);
				}
			}
			
			node.Declaration.Entity = DeclareLocal(node.Declaration, node.Declaration.Name, GetType(node.Declaration.Type), true);
			EnterNamespace(new DeclarationsNamespace(CurrentNamespace, TypeSystemServices, node.Declaration));
			try
			{
				Visit(node.Block);
			}
			finally
			{
				LeaveNamespace();
			}
		}
		
		protected virtual bool IsValidIncrementDecrementOperand(Expression e)
		{
			IType type = GetExpressionType(e);
			return IsNumber(type) || TypeSystemServices.IsDuckType(type);
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

			BinaryExpression addition = CodeBuilder.CreateBoundBinaryExpression(
				GetExpressionType(node.Operand),
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
			if(IsEnumOperation(node))
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
						BindReferenceEquality(node);
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
		
		bool IsEnumOperation(BinaryExpression node)
		{
			switch(node.Operator)
			{
				case BinaryOperatorType.Addition:
				case BinaryOperatorType.Subtraction:
				case BinaryOperatorType.BitwiseAnd:
				case BinaryOperatorType.BitwiseOr:
				case BinaryOperatorType.ExclusiveOr:
					return (GetExpressionType(node.Left).IsEnum || 
					        GetExpressionType(node.Right).IsEnum);
				default:
					return false;
			}
//			switch(node.Operator)
//			{
//				case BinaryOperatorType.Addition:
//					return lhs.IsEnum != rhs.IsEnum;
//				case BinaryOperatorType.Subtraction:
//					return lhs.IsEnum && !rhs.IsEnum || lhs == rhs;
//				case BinaryOperatorType.BitwiseAnd:
//				case BinaryOperatorType.BitwiseOr:
//				case BinaryOperatorType.ExclusiveOr:
//					return lhs == rhs;
//				default:
//					return false;
//			}
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
						BindExpressionType(node, lhs.IsEnum?lhs:rhs);
					}
					else goto default;
					break;
				case BinaryOperatorType.Subtraction:
					if (lhs == rhs)
					{
						BindExpressionType(node, TypeSystemServices.IntType);
					}
					else if (lhs.IsEnum && !rhs.IsEnum)
					{
						BindExpressionType(node, lhs);
					}
					else goto default;
					break;
				case BinaryOperatorType.BitwiseAnd:
				case BinaryOperatorType.BitwiseOr:
				case BinaryOperatorType.ExclusiveOr:
					if (lhs == rhs)
					{
						BindExpressionType(node, lhs);
					}
					else goto default;
					break;
				default:
					if (!ResolveOperator(node))
					{
						InvalidOperatorForTypes(node);
					}
					break;
			}
		}
		
		void BindBitwiseOperator(BinaryExpression node)
		{
			IType lhs = GetExpressionType(node.Left);
			IType rhs = GetExpressionType(node.Right);
			
			if (TypeSystemServices.IsIntegerOrBool(lhs) &&
			    TypeSystemServices.IsIntegerOrBool(rhs))
			{
				BindExpressionType(node, TypeSystemServices.GetPromotedNumberType(lhs, rhs));
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
			IType lhs = GetExpressionType(node.Left);
			IType rhs = GetExpressionType(node.Right);
			
			if (IsPrimitiveNumber(lhs) && IsPrimitiveNumber(rhs))
			{
				BindExpressionType(node, TypeSystemServices.BoolType);
			}
			else if (lhs.IsEnum || rhs.IsEnum || IsChar(lhs) || IsChar(rhs))
			{
				if (lhs == rhs || IsPrimitiveNumber(rhs) || IsPrimitiveNumber(lhs))
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
			}
			else if (!ResolveOperator(node))
			{
				switch (node.Operator)
				{
					case BinaryOperatorType.Equality:
						{
							if (OptimizeNullComparisons && (IsNull(node.Left) || IsNull(node.Right)))
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
							if (OptimizeNullComparisons && (IsNull(node.Left) || IsNull(node.Right)))
							{
								node.Operator = BinaryOperatorType.ReferenceInequality;
								BindReferenceEquality(node);
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
		
		static bool IsNull(Expression node)
		{
			return NodeType.NullLiteralExpression == node.NodeType;
		}
		
		void BindLogicalOperator(BinaryExpression node)
		{
			node.Left = AssertBoolContext(node.Left);
			node.Right = AssertBoolContext(node.Right);
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
					IList found = ((Ambiguous)tag).Filter(IsPublicEventFilter);
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
						NotImplemented(node, "BuiltinFunction: " + function.FunctionType);
						break;
					}
			}
		}
		
		void ProcessSwitchInvocation(MethodInvocationExpression node)
		{
			BindSwitchLabelReferences(node);
			if (CheckSwitchArguments(node)) return;
			Error(node, CompilerErrorFactory.InvalidSwitch(node.Target));
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
				BindExpressionType(node, GetConcreteExpressionType(node.Arguments[-1]));
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

			MethodInvocationExpression resultingNode = null;
			
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
			    Array_TypedConstructor2 == method)
			{
				IType type = TypeSystemServices.GetReferencedType(expression.Arguments[0]);
				if (null != type)
				{
					if (Array_TypedCollectionConstructor == method)
					{
						CheckListLiteralArgumentInArrayConstructor(type,  expression);
					}
					inferredType = TypeSystemServices.GetArrayType(type, 1);
				}
			}
			else if (MultiDimensionalArray_TypedConstructor == method)
			{
				IType type = TypeSystemServices.GetReferencedType(expression.Arguments[0]);
				if (null != type)
				{
					inferredType = TypeSystemServices.GetArrayType(type, expression.Arguments.Count-1);
				}
			}
			else if (Array_EnumerableConstructor == method)
			{
				IType enumeratorItemType = GetEnumeratorItemType(GetExpressionType(expression.Arguments[0]));
				if (TypeSystemServices.ObjectType != enumeratorItemType)
				{
					inferredType = TypeSystemServices.GetArrayType(enumeratorItemType, 1);
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
		
		protected virtual IEntity ResolveAmbiguousMethodInvocation(MethodInvocationExpression node, Ambiguous entity)
		{
			_context.TraceVerbose("{0}: resolving ambigous method invocation: {1}", node.LexicalInfo, entity);

			IEntity resolved = ResolveCallableReference(node, entity);
			if (null != resolved) return resolved;

			if (TryToProcessAsExtensionInvocation(node)) return null;
			
			return CantResolveAmbiguousMethodInvocation(node, entity.Entities);
		}

		private IEntity ResolveCallableReference(MethodInvocationExpression node, Ambiguous entity)
		{
			IEntity resolved = _callableResolution.ResolveCallableReference(node.Arguments, entity.Entities);
			if (null == resolved) return null;
			
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
			
			ProcessExtensionMethodInvocation(node, extension);
			return true;
		}

		private IEntity ResolveExtension(MethodInvocationExpression node)
		{
			MemberReferenceExpression mre = node.Target as MemberReferenceExpression;
			if (mre == null) return null;
			
			return NameResolutionService.ResolveExtension(GetReferenceNamespace(mre), mre.Name);
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
			
			IEntity targetEntity = node.Target.Entity;
			if (BuiltinFunction.Switch == targetEntity)
			{
				ProcessSwitchInvocation(node);
				return;
			}
			
			Visit(node.Arguments);
			
			if (TypeSystemServices.IsError(node.Target)
			    || TypeSystemServices.IsErrorAny(node.Arguments))
			{
				Error(node);
				return;
			}
			
			if (null == targetEntity)
			{
				ProcessGenericMethodInvocation(node);
				return;
			}

			if (IsOrContainsExtensionMethod(targetEntity))
			{
				ProcessExtensionMethodInvocation(node, targetEntity);
				return;
			}

			ProcessMethodInvocationExpression(node, targetEntity);
		}

		private void ProcessMethodInvocationExpression(MethodInvocationExpression node, IEntity targetEntity)
		{
			switch (targetEntity.EntityType)
			{
				case EntityType.Ambiguous:
					{
						ProcessAmbiguousMethodInvocation(node, targetEntity);
						break;
					}
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

		protected virtual void ProcessAmbiguousMethodInvocation(MethodInvocationExpression node, IEntity targetEntity)
		{
			targetEntity = ResolveAmbiguousMethodInvocation(node, (Ambiguous)targetEntity);
			if (null == targetEntity) return;
			ProcessMethodInvocationExpression(node, targetEntity);
		}

		private void ProcessConstructorInvocation(MethodInvocationExpression node, IEntity targetEntity)
		{
			NamedArgumentsNotAllowed(node);
			InternalConstructor constructorInfo = targetEntity as InternalConstructor;
			if (null != constructorInfo)
			{
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
		}

		protected virtual void ProcessMethodInvocation(MethodInvocationExpression node, IEntity targetEntity)
		{
			IMethod targetMethod = (IMethod)targetEntity;
			if (!CheckParameters(targetMethod.CallableType, node.Arguments, false))
			{
				if (TryToProcessAsExtensionInvocation(node)) return;
				AssertParameters(node, targetMethod, node.Arguments);
			}

			AssertTargetContext(node.Target, targetMethod);
			NamedArgumentsNotAllowed(node);
			
			EnsureRelatedNodeWasVisited(node.Target, targetMethod);
			BindExpressionType(node, GetInferredType(targetMethod));
			ApplyBuiltinMethodTypeInference(node, targetMethod);
		}

		private void NamedArgumentsNotAllowed(MethodInvocationExpression node)
		{
			if (node.NamedArguments.Count == 0) return;
			Error(CompilerErrorFactory.NamedArgumentsNotAllowed(node.NamedArguments[0]));
		}

		private void ProcessExtensionMethodInvocation(MethodInvocationExpression node, IEntity targetEntity)
		{
			PreNormalizeExtensionInvocation(node);
			if (EntityType.Ambiguous == targetEntity.EntityType)
			{
				targetEntity = ResolveAmbiguousExtension(node, (Ambiguous)targetEntity);
				if (null == targetEntity) return;
			}
			IMethod targetMethod = (IMethod)targetEntity;
			PostNormalizationExtensionInvocation(node, targetMethod);
			NamedArgumentsNotAllowed(node);
			AssertParameters(node, targetMethod, node.Arguments);
			BindExpressionType(node, targetMethod.ReturnType);
		}

		private IEntity ResolveAmbiguousExtension(MethodInvocationExpression node, Ambiguous ambiguous)
		{
			IEntity resolved = ResolveCallableReference(node, ambiguous);
			if (null != resolved) return resolved;
			
			return CantResolveAmbiguousMethodInvocation(node, ambiguous.Entities);
		}

		private bool IsOrContainsExtensionMethod(IEntity entity)
		{
			if (entity.EntityType == EntityType.Ambiguous) return IsExtensionMethod(((Ambiguous)entity).Entities[0]);
			return IsExtensionMethod(entity);
		}

		private bool IsExtensionMethod(IEntity entity)
		{
			if (EntityType.Method != entity.EntityType) return false;
			return ((IMethod)entity).IsExtension;
		}

		private void PostNormalizationExtensionInvocation(MethodInvocationExpression node, IMethod targetMethod)
		{
			node.Target = CodeBuilder.CreateMethodReference(node.Target.LexicalInfo, targetMethod);
		}

		private static void PreNormalizeExtensionInvocation(MethodInvocationExpression node)
		{
			node.Arguments.Insert(0, ((MemberReferenceExpression)node.Target).Target);
		}

		private IType GetInferredType(IMethod entity)
		{
			return entity.IsDuckTyped
				? this.TypeSystemServices.DuckType
				: entity.ReturnType;
		}

		private IType GetInferredType(IMember entity)
		{
			Debug.Assert(EntityType.Method != entity.EntityType);
			return entity.IsDuckTyped
				? this.TypeSystemServices.DuckType
				: entity.Type;
		}

		void ReplaceTypeInvocationByEval(IType type, MethodInvocationExpression node)
		{
			Node parent = node.ParentNode;
			
			MethodInvocationExpression eval = CodeBuilder.CreateEvalInvocation(node.LexicalInfo);
			ReferenceExpression local = CreateTempLocal(node.Target.LexicalInfo, type);
			
			eval.Arguments.Add(CodeBuilder.CreateAssignment(local.CloneNode(), node));

			AddResolvedNamedArgumentsToEval(eval, node.NamedArguments, local);

			node.NamedArguments.Clear();
			
			eval.Arguments.Add(local);
			
			BindExpressionType(eval, type);
			
			parent.Replace(node, eval);
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
			
			IMethod method = ev.GetRaiseMethod();
			if (AssertParameters(node, method, node.Arguments))
			{
				node.Target = CodeBuilder.CreateMemberReference(
					((MemberReferenceExpression)node.Target).Target,
					method);
				BindExpressionType(node, method.ReturnType);
			}
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
			// XXX: naive and unoptimized but correct approach
			// simply initialize a new temporary value type
			// TODO: OPTIMIZE by detecting assignments to local variables

			MethodInvocationExpression eval = CodeBuilder.CreateEvalInvocation(node.LexicalInfo);
			BindExpressionType(eval, type);

			InternalLocal local = DeclareTempLocal(type);
			ReferenceExpression localReference = CodeBuilder.CreateReference(local);
			eval.Arguments.Add(CodeBuilder.CreateDefaultInitializer(node.LexicalInfo, local));
			AddResolvedNamedArgumentsToEval(eval, node.NamedArguments, localReference);
			eval.Arguments.Add(localReference);

			node.ParentNode.Replace(node, eval);
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
			MethodInvocationExpression invocation = CodeBuilder.CreateMethodInvocation(Activator_CreateInstance, node.Target);
			if (Activator_CreateInstance.AcceptVarArgs)
			{
				invocation.Arguments.Extend(node.Arguments);
			}
			else
			{
				invocation.Arguments.Add(CodeBuilder.CreateObjectArray(node.Arguments));
			}
			node.ParentNode.Replace(node, invocation);
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
		
		bool CheckIsNotValueType(BinaryExpression node, Expression expression)
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
					
				case BinaryOperatorType.InPlaceBitwiseAnd:
					return BinaryOperatorType.BitwiseAnd;
					
				case BinaryOperatorType.InPlaceBitwiseOr:
					return BinaryOperatorType.BitwiseOr;

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
			IType left = GetExpressionType(node.Left);
			IType right = GetExpressionType(node.Right);
			if (IsPrimitiveNumber(left) && IsPrimitiveNumber(right))
			{
				BindExpressionType(node, TypeSystemServices.GetPromotedNumberType(left, right));
			}
			else if (!ResolveOperator(node))
			{
				InvalidOperatorForTypes(node);
			}
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
			return IsFieldPropertyOrEvent(entity) && ((IMember)entity).IsPublic;
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
			
			IList found = ((Ambiguous)candidate).Filter(IsPublicFieldPropertyEventFilter);
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
				Visit(arg.Second);
				
				if (NodeType.ReferenceExpression != arg.First.NodeType)
				{
					Error(arg.First, CompilerErrorFactory.NamedParameterMustBeIdentifier(arg));
					continue;
				}

				ResolveNamedArgument(type, (ReferenceExpression)arg.First, arg.Second);
			}
		}

		void ResolveNamedArgument(IType type, ReferenceExpression name, Expression value)
		{
			IMember member = ResolvePublicFieldPropertyEvent(name, type, name.Name);
			if (null == member)
			{
				NamedArgumentNotFound(type, name);
				return;
			}

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
			if (TypeSystemServices.IsError(expectedType)
			    || TypeSystemServices.IsError(actualType))
			{
				return false;
			}
			
			if (!TypeSystemServices.AreTypesRelated(expectedType, actualType))
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
				    !(IsNumber(expressionType) && IsNumber(parameterType))
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
					      || args[i] is SlicingExpression))
					{
						if (reportErrors) Error(CompilerErrorFactory.RefArgTakesLValue(args[i]));
						return false;
					}
					if (!_callableResolution.IsValidByRefArg(param, parameterType, argumentType, args[i]))
					{
						return false;
					}
				}
				else
				{
					if (!TypeSystemServices.AreTypesRelated(parameterType, argumentType))
					{
						return false;
					}
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
			bool ok = CheckParameters(method, args, true);
			if (!ok) Error(CompilerErrorFactory.MethodSignature(sourceNode, sourceEntity.ToString(), GetSignature(args)));
			return ok;
		}

		private bool CheckParameters(ICallableType method, ExpressionCollection args, bool reportErrors)
		{
			return AcceptVarArgs(method)
				? CheckVarArgsParameters(method, args)
				: CheckExactArgsParameters(method, args, reportErrors);
		}

		bool CheckVarArgsParameters(ICallableType method, ExpressionCollection args)
		{
			IParameter[] parameters = method.GetSignature().Parameters;
			if (args.Count < parameters.Length-1) return false;
			return _callableResolution.CalculateVarArgsScore(parameters, args) >= 0;
		}

		bool CheckExactArgsParameters(ICallableType method, ExpressionCollection args, bool reportErrors)
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
		
		bool IsNumber(IType type)
		{
			return TypeSystemServices.IsNumber(type);
		}
		
		bool IsPrimitiveNumber(IType type)
		{
			return TypeSystemServices.IsPrimitiveNumber(type);
		}
		
		bool IsPrimitiveNumber(Expression expression)
		{
			return IsPrimitiveNumber(GetExpressionType(expression));
		}
		
		IConstructor GetCorrectConstructor(Node sourceNode, IType type, ExpressionCollection arguments)
		{
			IConstructor[] constructors = type.GetConstructors();
			if (constructors.Length > 0)
			{
				return (IConstructor)GetCorrectCallableReference(sourceNode, arguments, constructors);
			}
			else
			{
				if (!TypeSystemServices.IsError(type))
				{
					Error(CompilerErrorFactory.NoApropriateConstructorFound(sourceNode, type.ToString(), GetSignature(arguments)));
				}
			}
			return null;
		}

		IEntity GetCorrectCallableReference(Node sourceNode, NodeCollection args, IEntity[] candidates)
		{
			IEntity found = _callableResolution.ResolveCallableReference(args, candidates);
			if (null == found) EmitCallableResolutionError(sourceNode, candidates, args);
			return found;
		}

		private void EmitCallableResolutionError(Node sourceNode, IEntity[] candidates, NodeCollection args)
		{
			if (_callableResolution.ValidCandidates.Count > 1)
			{
				Error(CompilerErrorFactory.AmbiguousReference(sourceNode, candidates[0].Name, _callableResolution.ValidCandidates));
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
			IInternalEntity internalInfo = entity as IInternalEntity;
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
			if (null == entity) return false;
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
			if (type == TypeSystemServices.BoolType) return expression;
			if (IsNumber(type)) return expression;
			if (type.IsEnum) return expression;

			IMethod method = TypeSystemServices.FindImplicitConversionOperator(type, TypeSystemServices.BoolType);
			if (null != method) return CodeBuilder.CreateMethodInvocation(method, expression);

			// reference types can be used in bool context
			if (!type.IsValueType) return expression;
			
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
		
		void ProcessDeclarationType(IType defaultDeclarationType, Declaration d)
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
		
		void DeclareLocal(Declaration d, bool privateScope)
		{
			if (AssertIdentifierName(d, d.Name))
			{
				d.Entity = DeclareLocal(d, d.Name, GetType(d.Type), privateScope);
			}
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
			
			foreach (Declaration d in declarations)
			{
				ProcessDeclarationForIterator(d, defaultDeclType);
			}
		}

		protected void ProcessDeclarationForIterator(Declaration d, IType defaultDeclType)
		{
			ProcessDeclarationType(defaultDeclType, d);
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
							if (TypeSystemServices.IsReadOnlyField(fld)
							    && !(EntityType.Constructor == _currentMethod.EntityType
							         && _currentMethod.DeclaringType == fld.DeclaringType
							         && fld.IsStatic == _currentMethod.IsStatic))
							{
								Error(CompilerErrorFactory.FieldIsReadonly(AstUtil.GetMemberAnchor(node), entity.FullName));
								return false;
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
		
		string GetSignature(NodeCollection args)
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
		
		void TraceReturnType(Method method, IMethod tag)
		{
			_context.TraceInfo("{0}: return type for method {1} bound to {2}", method.LexicalInfo, method.Name, tag.ReturnType);
		}

		#region Method bindings cache

		IMethod _RuntimeServices_Len;

		IMethod RuntimeServices_Len
		{
			get
			{
				if (null == _RuntimeServices_Len)
				{
					_RuntimeServices_Len = ResolveMethod(TypeSystemServices.RuntimeServicesType, "Len");
				}
				return _RuntimeServices_Len;
			}
		}

		IMethod _RuntimeServices_Mid;

		IMethod RuntimeServices_Mid
		{
			get
			{
				if (null == _RuntimeServices_Mid)
				{
					_RuntimeServices_Mid = ResolveMethod(TypeSystemServices.RuntimeServicesType, "Mid");
				}
				return _RuntimeServices_Mid;
			}
		}
		
		IMethod _RuntimeServices_NormalizeStringIndex;

		IMethod RuntimeServices_NormalizeStringIndex
		{
			get
			{
				if (null == _RuntimeServices_NormalizeStringIndex)
				{
					_RuntimeServices_NormalizeStringIndex = ResolveMethod(TypeSystemServices.RuntimeServicesType, "NormalizeStringIndex");
				}
				return _RuntimeServices_NormalizeStringIndex;
			}
		}
		
		IMethod _RuntimeServices_AddArrays;

		IMethod RuntimeServices_AddArrays
		{
			get
			{
				if (null == _RuntimeServices_AddArrays)
				{
					_RuntimeServices_AddArrays = ResolveMethod(TypeSystemServices.RuntimeServicesType, "AddArrays");
				}
				return _RuntimeServices_AddArrays;
			}
		}
		
		IMethod _RuntimeServices_GetRange1;

		IMethod RuntimeServices_GetRange1
		{
			get
			{
				if (null == _RuntimeServices_GetRange1)
				{
					_RuntimeServices_GetRange1 = ResolveMethod(TypeSystemServices.RuntimeServicesType, "GetRange1");
				}
				return _RuntimeServices_GetRange1;
			}
		}
		
		IMethod _RuntimeServices_GetRange2;

		IMethod RuntimeServices_GetRange2
		{
			get
			{
				if (null == _RuntimeServices_GetRange2)
				{
					_RuntimeServices_GetRange2 = ResolveMethod(TypeSystemServices.RuntimeServicesType, "GetRange2");
				}
				return _RuntimeServices_GetRange2;
			}
		}
		
		IMethod _RuntimeServices_GetMultiDimensionalRange1;

		IMethod RuntimeServices_GetMultiDimensionalRange1
		{
			get
			{
				if (null == _RuntimeServices_GetMultiDimensionalRange1)
				{
					_RuntimeServices_GetMultiDimensionalRange1 = ResolveMethod(TypeSystemServices.RuntimeServicesType, "GetMultiDimensionalRange1");
				}
				return _RuntimeServices_GetMultiDimensionalRange1;
			}
		}
		
		IMethod _RuntimeServices_SetMultiDimensionalRange1;

		IMethod RuntimeServices_SetMultiDimensionalRange1
		{
			get
			{
				if (null == _RuntimeServices_SetMultiDimensionalRange1)
				{
					_RuntimeServices_SetMultiDimensionalRange1 = ResolveMethod(TypeSystemServices.RuntimeServicesType, "SetMultiDimensionalRange1");
				}
				return _RuntimeServices_SetMultiDimensionalRange1;
			}
		}

		IMethod _RuntimeServices_GetEnumerable;

		IMethod RuntimeServices_GetEnumerable
		{
			get
			{
				if (null == _RuntimeServices_GetEnumerable)
				{
					_RuntimeServices_GetEnumerable = ResolveMethod(TypeSystemServices.RuntimeServicesType, "GetEnumerable");
				}
				return _RuntimeServices_GetEnumerable;
			}
		}
		
		IMethod _RuntimeServices_EqualityOperator;

		IMethod RuntimeServices_EqualityOperator
		{
			get
			{
				if (null == _RuntimeServices_EqualityOperator)
				{
					_RuntimeServices_EqualityOperator = TypeSystemServices.Map(Types.RuntimeServices.GetMethod("EqualityOperator", new Type[] { Types.Object, Types.Object }));
				}
				return _RuntimeServices_EqualityOperator;
			}
		}
		
		IMethod _Array_get_Length;
		
		IMethod Array_get_Length
		{
			get
			{
				if (null == _Array_get_Length)
				{
					_Array_get_Length = ResolveProperty(TypeSystemServices.ArrayType, "Length").GetGetMethod();
				}
				return _Array_get_Length;
			}
		}
		
		IMethod _Array_GetLength;
		
		IMethod Array_GetLength
		{
			get
			{
				if (null == _Array_GetLength)
				{
					_Array_GetLength = ResolveMethod(TypeSystemServices.ArrayType, "GetLength");
				}
				return _Array_GetLength;
			}
		}
		
		IMethod _String_get_Length;
		
		IMethod String_get_Length
		{
			get
			{
				if (null == _String_get_Length)
				{
					_String_get_Length = ResolveProperty(TypeSystemServices.StringType, "Length").GetGetMethod();
				}
				return _String_get_Length;
			}
		}
		
		IMethod _String_Substring_Int;
		
		IMethod String_Substring_Int
		{
			get
			{
				if (null == _String_Substring_Int)
				{
					_String_Substring_Int = TypeSystemServices.Map(Types.String.GetMethod("Substring", new Type[] { Types.Int }));
				}
				return _String_Substring_Int;
			}
		}
		
		IMethod _ICollection_get_Count;
		
		IMethod ICollection_get_Count
		{
			get
			{
				if (null == _ICollection_get_Count)
				{
					_ICollection_get_Count = ResolveProperty(TypeSystemServices.ICollectionType, "Count").GetGetMethod();
				}
				return _ICollection_get_Count;
			}
		}
		
		IMethod _List_GetRange1;
		
		IMethod List_GetRange1
		{
			get
			{
				if (null == _List_GetRange1)
				{
					_List_GetRange1 = TypeSystemServices.Map(Types.List.GetMethod("GetRange", new Type[] { typeof(int) }));
				}
				return _List_GetRange1;
			}
		}
		
		IMethod _List_GetRange2;
		
		IMethod List_GetRange2
		{
			get
			{
				if (null == _List_GetRange2)
				{
					_List_GetRange2 = TypeSystemServices.Map(Types.List.GetMethod("GetRange", new Type[] { typeof(int), typeof(int) }));
				}
				return _List_GetRange2;
			}
		}
		
		IMethod _ICallable_Call;
		
		IMethod ICallable_Call
		{
			get
			{
				if (null == _ICallable_Call)
				{
					_ICallable_Call = ResolveMethod(TypeSystemServices.ICallableType, "Call");
				}
				return _ICallable_Call;
			}
		}
		
		IMethod _Activator_CreateInstance;
		
		IMethod Activator_CreateInstance
		{
			get
			{
				if (null == _Activator_CreateInstance)
				{
					_Activator_CreateInstance = TypeSystemServices.Map(typeof(Activator).GetMethod("CreateInstance", new Type[] { Types.Type, Types.ObjectArray }));
				}
				return _Activator_CreateInstance;
			}
		}

		IConstructor _ApplicationException_StringConstructor;
		
		IConstructor ApplicationException_StringConstructor
		{
			get
			{
				if (null == _ApplicationException_StringConstructor)
				{
					_ApplicationException_StringConstructor =
						TypeSystemServices.Map(
							Types.ApplicationException.GetConstructor(new Type[] { typeof(string) }));
				}
				return _ApplicationException_StringConstructor;
			}
		}
		
		IConstructor _TextReaderEnumerator_Constructor;
		
		IConstructor TextReaderEnumerator_Constructor
		{
			get
			{
				if (null == _TextReaderEnumerator_Constructor)
				{
					_TextReaderEnumerator_Constructor = TypeSystemServices.Map(typeof(TextReaderEnumerator).GetConstructor(new Type[] { typeof(TextReader) }));
				}
				return _TextReaderEnumerator_Constructor;
			}
		}
		
		IConstructor _EnumeratorItemType_Constructor;
		
		IConstructor EnumeratorItemType_Constructor
		{
			get
			{
				if (null == _EnumeratorItemType_Constructor)
				{
					_EnumeratorItemType_Constructor = TypeSystemServices.Map(typeof(EnumeratorItemTypeAttribute)).GetConstructors()[0];
				}
				return _EnumeratorItemType_Constructor;
			}
		}

		public bool OptimizeNullComparisons
		{
			get { return _optimizeNullComparisons; }
			set { _optimizeNullComparisons = value; }
		}

		#endregion
	}
}
