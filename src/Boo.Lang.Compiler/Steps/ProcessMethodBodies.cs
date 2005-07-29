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
		
		protected MethodBodyState _methodBodyState;

		protected CallableResolutionService _callableResolution;
		
		protected struct MethodBodyState
		{
			public int LoopDepth;
		
			public int TryBlockDepth;
		
			public int ExceptionHandlerDepth;
		};
		
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
			_methodBodyState = new MethodBodyState();
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
		
		void EnterTryBlock()
		{
			++_methodBodyState.TryBlockDepth;
		}
		
		void LeaveTryBlock()
		{
			--_methodBodyState.TryBlockDepth;
		}
		
		int CurrentTryBlockDepth
		{
			get
			{
				return _methodBodyState.TryBlockDepth;
			}
		}
		
		void EnterLoop()
		{
			++_methodBodyState.LoopDepth;
		}
		
		bool InLoop()
		{
			return _methodBodyState.LoopDepth > 0;
		}
		
		void LeaveLoop()
		{
			--_methodBodyState.LoopDepth;
		}
		
		void EnterExceptionHandler()
		{
			++_methodBodyState.ExceptionHandlerDepth;
		}
		
		bool InExceptionHandler()
		{
			return _methodBodyState.ExceptionHandlerDepth > 0;
		}
		
		void LeaveExceptionHandler()
		{
			--_methodBodyState.ExceptionHandlerDepth;
		}
		
		override public void OnModule(Module module)
		{
			if (WasVisited(module))
			{
				return;
			}
			MarkVisited(module);
			
			EnterNamespace((INamespace)TypeSystemServices.GetEntity(module));
			
			Visit(module.Members);
			Visit(module.AssemblyAttributes);
			
			LeaveNamespace();
		}
		
		override public void OnInterfaceDefinition(InterfaceDefinition node)
		{
			if (WasVisited(node))
			{
				return;
			}
			MarkVisited(node);
			
			InternalInterface tag = (InternalInterface)GetEntity(node);
			EnterNamespace(tag);
			Visit(node.Attributes);
			Visit(node.Members);
			LeaveNamespace();
		}
		
		override public void OnClassDefinition(ClassDefinition node)
		{
			if (WasVisited(node))
			{
				return;
			}
			MarkVisited(node);
			
			InternalClass entity = (InternalClass)GetEntity(node);
			EnterNamespace(entity);
			Visit(node.Attributes);
			Visit(node.Members);
			LeaveNamespace();

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
				
				IConstructor constructor = FindCorrectConstructor(node, tag, node.Arguments);
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
				if (tag.DeclaringType.IsValueType)
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
				}
			}
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
			}
			else
			{
				CheckTypeCompatibility(node.Initializer, GetType(node.Type), initializerType);
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
			GetStaticConstructor(type).Body.Add(
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
					IConstructor super = FindCorrectConstructor(node, baseType, EmptyExpressionCollection);
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
			CheckIdentifierName(node, node.Name);
		}
		
		override public void OnCallableBlockExpression(CallableBlockExpression node)
		{
			TypeMemberModifiers modifiers = TypeMemberModifiers.Internal;
			if (_currentMethod.IsStatic)
			{
				modifiers |= TypeMemberModifiers.Static;
			}

			Method closure = CodeBuilder.CreateMethod(
								"___closure" + _context.AllocIndex(),
								Unknown.Default,
								modifiers);
			InternalMethod closureEntity = (InternalMethod)closure.Entity;
							
			closure.LexicalInfo = node.LexicalInfo;
			closure.Parameters = node.Parameters;
			closure.Body = node.Body;
			
			_currentMethod.Method.DeclaringType.Members.Add(closure);
			
			CodeBuilder.BindParameterDeclarations(_currentMethod.IsStatic, closure);
			
			// check for invalid names and
			// resolve parameter types
			Visit(closure.Parameters);
			
			// Connects the closure method namespace with the current
			NamespaceDelegator ns = new NamespaceDelegator(CurrentNamespace, closureEntity);
			ProcessMethodBody(closureEntity, ns);
			TryToResolveReturnType(closureEntity);
			
			node.ExpressionType = closureEntity.Type;
			node.Entity = closureEntity;
		}

		override public void OnMethod(Method method)
		{
			if (WasVisited(method))
			{
				return;
			}
			MarkVisited(method);
			
			Visit(method.Attributes);
			Visit(method.Parameters);
			Visit(method.ReturnType);
			Visit(method.ReturnTypeAttributes);
			
			if (method.IsRuntime)
			{
				CheckRuntimeMethod(method);
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
			IMethod overriden = FindMethodOverride(entity);
			if (null == overriden) return;
            
			ProcessMethodOverride(entity, overriden);
		}

		IMethod FindPropertyAccessorOverride(Property property, Method accessor)
		{
			IProperty baseProperty = ((InternalProperty)property.Entity).Override;
			if (null != baseProperty)
			{
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
						baseMethod.ReturnType.FullName,
						entity.ReturnType.FullName));
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
		
		void ResolvePropertyOverride(Property property)
		{
			InternalProperty entity = (InternalProperty)property.Entity;
			IType baseType = entity.DeclaringType.BaseType;
			IEntity baseProperties = NameResolutionService.Resolve(baseType, property.Name, EntityType.Property);
			if (null != baseProperties)
			{
				if (EntityType.Property == baseProperties.EntityType)
				{
					entity.Override = (IProperty)baseProperties;
				}
			}
			
			if (null != entity.Override)
			{
				if (property.IsOverride)
				{
					SetPropertyAccessorOverride(property.Getter);
					SetPropertyAccessorOverride(property.Setter);
				}
				else
				{
					//property.Modifiers |= TypeMemberModifiers.Override;
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
			if (TypeSystemServices.IEnumerableType != returnType &&
				!TypeSystemServices.IsSystemObject(returnType))
			{
				Error(CompilerErrorFactory.InvalidGeneratorReturnType(method.ReturnType));
			}
		}
		
		void ResolveLabelReferences(InternalMethod entity)
		{
			foreach (ReferenceExpression reference in entity.LabelReferences)
			{
				InternalLabel label = entity.ResolveLabel(reference.Name);
				if (null == label)
				{
					Error(reference,
						CompilerErrorFactory.NoSuchLabel(reference, reference.Name));
				}
				else
				{
					reference.Entity = label;
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
			ResolveLabelReferences(entity);
			if (entity.IsGenerator)
			{
				CreateGeneratorSkeleton(entity);
			}
		}
		
		void ProcessNodeInMethodContext(InternalMethod entity, INamespace ns, Node node)
		{
			PushMethodInfo(entity);
			EnterNamespace(ns);
			
			MethodBodyState saved = _methodBodyState;
			_methodBodyState = new MethodBodyState();
			try
			{
				Visit(node);
			}
			finally
			{
				_methodBodyState = saved;
				LeaveNamespace();
				PopMethodInfo();
			}
		}
		
		void ResolveGeneratorReturnType(InternalMethod entity)
		{
			Method method = entity.Method;
			BooClassBuilder generatorType = (BooClassBuilder)method["GeneratorClassBuilder"];
			method.ReturnType = CodeBuilder.CreateTypeReference(generatorType.Entity);
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
			if (EntityType.Constructor != _currentMethod.EntityType)
			{
				if (null == _currentMethod.Overriden)
				{
					Error(
						CompilerErrorFactory.MethodIsNotOverride(node, _currentMethod.ToString()));
				}
			}
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
		
		IEntity[] GetSetMethods(IEntity[] tags)
		{
			List setMethods = new List();
			for (int i=0; i<tags.Length; ++i)
			{
				IProperty property = tags[i] as IProperty;
				if (null != property)
				{
					IMethod setter = property.GetSetMethod();
					if (null != setter)
					{
						setMethods.Add(setter);
					}
				}
			}
			return (IEntity[])setMethods.ToArray(typeof(IEntity));
		}
		
		IEntity[] GetGetMethods(IEntity[] tags)
		{
			List getMethods = new List();
			for (int i=0; i<tags.Length; ++i)
			{
				IProperty property = tags[i] as IProperty;
				if (null != property)
				{
					IMethod getter = property.GetGetMethod();
					if (null != getter)
					{
						getMethods.Add(getter);
					}
				}
			}
			return (IEntity[])getMethods.ToArray(typeof(IEntity));
		}
		
		void CheckNoComplexSlicing(SlicingExpression node)
		{
			if (IsComplexSlicing(node))
			{
				NotImplemented(node, "complex slicing");
			}
		}
		
		bool IsComplexSlicing(SlicingExpression node)
		{
			foreach (Slice slice in node.Indices)
			{
				if (IsComplexSlice(slice))
				{
					return true;
				}
			}
			return false;
		}
		
		protected static bool IsComplexSlice(Slice slice)
		{
			return null != slice.End || null != slice.Step || OmittedExpression.Default == slice.Begin;
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
				if (!CheckTypeCompatibility(node.Begin, TypeSystemServices.IntType, GetExpressionType(node.Begin)))
				{
					return false;
				}
			}
			
			if (null != node.End && OmittedExpression.Default != node.End)
			{
				if (!CheckTypeCompatibility(node.End, TypeSystemServices.IntType, GetExpressionType(node.End)))
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
			// target[indices]
			IType targetType = GetExpressionType(node.Target);
			if (TypeSystemServices.IsError(targetType))
			{
				Error(node);
				return;
			}
			
			if (IsIndexedProperty(node.Target))
			{
				CheckNoComplexSlicing(node);
				SliceMember(node, node.Target.Entity);
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

					if (IsComplexSlicing(node))
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
					if (IsComplexSlicing(node))
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
							Error(node, CompilerErrorFactory.TypeDoesNotSupportSlicing(node.Target, targetType.FullName));
						}
						else
						{
							node.Target = new MemberReferenceExpression(node.Target, member.Name);
							node.Target.Entity = member;
							// to be resolved later
							node.Target.ExpressionType = Null.Default;
							SliceMember(node, member);
						}
					}
				}
			}
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
				IEntity[] tags = GetGetMethods(((Ambiguous)member).Entities);
				getter = (IMethod)ResolveCallableReference(node, mie.Arguments, tags, true);
			}
			else if (EntityType.Property == member.EntityType)
			{
				getter = ((IProperty)member).GetGetMethod();
			}
			
			if (null != getter)
			{
				if (CheckParameters(node, getter, mie.Arguments))
				{
					Expression target = ((MemberReferenceExpression)node.Target).Target;
					
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
			ExpressionCollection yieldExpressions = entity.YieldExpressions;
			IType itemType = yieldExpressions.Count > 0
				? GetMostGenericType(yieldExpressions)
				: TypeSystemServices.ObjectType;
			CreateGeneratorSkeleton(method.DeclaringType, method, itemType);
		}
		
		BooClassBuilder CreateGeneratorSkeleton(GeneratorExpression node)
		{
			return CreateGeneratorSkeleton(_currentMethod.Method.DeclaringType, node, GetConcreteExpressionType(node.Expression));
		}
		
		BooClassBuilder CreateGeneratorSkeleton(TypeDefinition parentType, Node node, IType generatorItemType)
		{
			// create the class skeleton for type inference to work
			BooClassBuilder builder = CodeBuilder.CreateClass(
														string.Format("___generator{0}", _context.AllocIndex()),
														TypeMemberModifiers.Private|TypeMemberModifiers.Final);
			builder.AddBaseType(TypeSystemServices.Map(typeof(AbstractGenerator)));
			builder.AddAttribute(CodeBuilder.CreateAttribute(
												EnumeratorItemType_Constructor,
												CodeBuilder.CreateTypeofExpression(generatorItemType)));
			builder.LexicalInfo = node.LexicalInfo;
			parentType.Members.Add(builder.ClassDefinition);
			
			node["GeneratorClassBuilder"] = builder;
			node["GetEnumeratorBuilder"] = builder.AddVirtualMethod("GetEnumerator", TypeSystemServices.IEnumeratorType);
			node["GeneratorItemType"] = generatorItemType;
			
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
			ExpressionCollection items = node.Items;
			if (0 == items.Count)
			{
				BindExpressionType(node, TypeSystemServices.ObjectArrayType);
			}
			else
			{
				TypeSystemServices.MapToConcreteExpressionTypes(node.Items);
				BindExpressionType(node, TypeSystemServices.GetArrayType(GetMostGenericType(items), 1));
			}
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
			
			CheckDeclarationName(node.Declaration);
			
			IEntity localInfo = DeclareLocal(node, node.Declaration.Name, type);
			if (null != node.Initializer)
			{
				CheckTypeCompatibility(node.Initializer, type, GetExpressionType(node.Initializer));
				
				ReferenceExpression var = new ReferenceExpression(node.Declaration.LexicalInfo);
				var.Name = node.Declaration.Name;
				Bind(var, localInfo);
				BindExpressionType(var, type);
				
				BinaryExpression assign = new BinaryExpression(node.LexicalInfo);
				assign.Operator = BinaryOperatorType.Assign;
				assign.Left = var;
				assign.Right = node.Initializer;
				BindExpressionType(assign, type);
				
				node.ReplaceBy(new ExpressionStatement(assign));
			}
			else
			{
				node.ReplaceBy(null);
			}
		}
		
		override public void LeaveExpressionStatement(ExpressionStatement node)
		{
			CheckHasSideEffect(node.Expression);
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
				node.Entity = _currentMethod;
				node.ExpressionType = _currentMethod.DeclaringType;
			}
		}
		
		override public void LeaveTypeofExpression(TypeofExpression node)
		{
			BindExpressionType(node, TypeSystemServices.TypeType);
		}
		
		bool IsConversionOperator(IMethod method, IType fromType, IType toType)
		{
			if (method.IsStatic)
			{
				if (method.ReturnType == toType)
				{
					IParameter[] parameters = method.GetParameters();
					if (1 == parameters.Length &&
						fromType == parameters[0].Type)
					{
						return true;
					}
				}
			}
			return false;
		}
		
		IMethod FindExplicitConversionOperator(IType fromType, IType toType)
		{
			return FindConversionOperator("op_Explicit", fromType, toType);
		}
		
		IMethod FindImplicitConversionOperator(IType fromType, IType toType)
		{
			return FindConversionOperator("op_Implicit", fromType, toType);
		}
		
		IMethod FindConversionOperator(string name, IType fromType, IType toType)
		{
			while (fromType != TypeSystemServices.ObjectType)
			{
				foreach (IEntity entity in fromType.GetMembers())
				{
					if (EntityType.Method == entity.EntityType &&
						name == entity.Name)
					{
						IMethod method = (IMethod)entity;
						if (IsConversionOperator(method, fromType, toType))
						{
							return method;
						}
					}
				}
				fromType = fromType.BaseType;
				if (null == fromType)
				{
				// FIXME: this null check should not be needed
				// but Boo.Nant.Tasks is failing to compiler
				// otherwise
					break;
				}
			}
			return null;
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
				IMethod explicitOperator = FindExplicitConversionOperator(fromType, toType);
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
						toType.FullName,
						fromType.FullName));
			}
			BindExpressionType(node, toType);
		}
		
		override public void LeaveAsExpression(AsExpression node)
		{
			IType target = GetExpressionType(node.Target);
			IType toType = GetType(node.Type);
			
			if (target.IsValueType)
			{
				Error(CompilerErrorFactory.CantCastToValueType(node.Target, target.FullName));
			}
			else if (toType.IsValueType)
			{
				Error(CompilerErrorFactory.CantCastToValueType(node.Type, toType.FullName));
			}
			BindExpressionType(node, toType);
		}
		
		void ResolveMemberInfo(ReferenceExpression node, IMember member)
		{	
			MemberReferenceExpression memberRef = new MemberReferenceExpression(node.LexicalInfo);
			memberRef.Name = node.Name;
			
			if (member.IsStatic)
			{
				memberRef.Target = new ReferenceExpression(node.LexicalInfo, member.DeclaringType.FullName);
				Bind(memberRef.Target, member.DeclaringType);
			}
			else
			{
				memberRef.Target = new SelfLiteralExpression(node.LexicalInfo);
			}
			
			Bind(memberRef, member);
			BindExpressionType(memberRef.Target, member.DeclaringType);
			
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
		
		override public void OnReferenceExpression(ReferenceExpression node)
		{
			if (null != node.ExpressionType)
			{
				return;
			}
			
			IEntity entity = ResolveName(node, node.Name);
			if (null != entity)
			{	
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
		
		void PostProcessReferenceExpression(ReferenceExpression node)
		{
			IEntity tag = GetEntity(node);
			switch (tag.EntityType)
			{
				case EntityType.Type:
				{
					if (IsStandaloneReference(node))
					{
						BindExpressionType(node, TypeSystemServices.TypeType);
					}
					else
					{
						BindExpressionType(node, (IType)tag);
					}
					break;
				}
				
				case EntityType.Ambiguous:
				{
					if (!AstUtil.IsTargetOfMethodInvocation(node) &&
						((Ambiguous)tag).AllEntitiesAre(EntityType.Method))
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
						if (!AstUtil.IsTargetOfMethodInvocation(node))
						{
							Error(node, CompilerErrorFactory.BuiltinCannotBeUsedAsExpression(node, tag.Name));
						}
					}
					else
					{
						BindExpressionType(node, ((ITypedEntity)tag).Type);
					}
					break;
				}
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
			if (!IsLastArgumentOfVarArgInvocation(node))
			{
				Error(CompilerErrorFactory.ExplodeExpressionMustMatchVarArgCall(node));
			}
			BindExpressionType(node, type);
		}

		private bool IsLastArgumentOfVarArgInvocation(UnaryExpression node)
		{
			MethodInvocationExpression parent = node.ParentNode as MethodInvocationExpression;
			if (null == parent) return false;
			if (parent.Arguments.Count == 0 || node != parent.Arguments[-1]) return false;
			ICallableType type = parent.Target.ExpressionType as ICallableType;
			if (null == type) return false;
			return AcceptVarArgs(type);
		}

		override public void LeaveMemberReferenceExpression(MemberReferenceExpression node)
		{
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
			Error(node, CompilerErrorFactory.MemberNotFound(node, ((IEntity)ns).FullName));
		}
		
		virtual protected void ProcessMemberReferenceExpression(MemberReferenceExpression node)
		{
			IEntity member = node.Entity;
			if (null == member)
			{
				INamespace ns = GetReferenceNamespace(node);
				member = NameResolutionService.Resolve(ns, node.Name);
				if (null == member)
				{
					MemberNotFound(node, ns);
					return;
				}
			}
			
			EnsureRelatedNodeWasVisited(node, member);
			
			IMember memberInfo = member as IMember;
			if (null != memberInfo)
			{
				// methods will be checked later
				if (EntityType.Method != memberInfo.EntityType)
				{
					if (!CheckTargetContext(node, memberInfo))
					{
						Error(node);
						return;
					}
				}
				BindExpressionType(node, memberInfo.Type);
			}
			
			if (EntityType.Property == member.EntityType)
			{
				if (IsIndexedProperty(member))
				{
					if (!AstUtil.IsTargetOfSlicing(node))
					{
						Error(node, CompilerErrorFactory.PropertyRequiresParameters(AstUtil.GetMemberAnchor(node), member.FullName));
						return;
					}
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
						node.ExpressionType = ev.BackingField.Type;
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
		
		override public void LeaveUnlessStatement(UnlessStatement node)
		{
			node.Condition = CheckBoolContext(node.Condition);
		}
		
		override public void LeaveIfStatement(IfStatement node)
		{
			node.Condition = CheckBoolContext(node.Condition);
		}
		
		override public void OnLabelStatement(LabelStatement node)
		{
			ContextAnnotations.SetTryBlockDepth(node, CurrentTryBlockDepth);
			
			if (null == _currentMethod.ResolveLabel(node.Name))
			{
				_currentMethod.AddLabel(new InternalLabel(node));
			}
			else
			{
				Error(
					CompilerErrorFactory.LabelAlreadyDefined(node,
											_currentMethod.FullName,
											node.Name));
			}
		}
		
		override public void OnGotoStatement(GotoStatement node)
		{
			ContextAnnotations.SetTryBlockDepth(node, CurrentTryBlockDepth);
			
			_currentMethod.AddLabelReference(node.Label);
		}
		
		override public void OnBreakStatement(BreakStatement node)
		{
			CheckInLoop(node);
		}

		override public void OnContinueStatement(ContinueStatement node)
		{
			CheckInLoop(node);
		}

		private void CheckInLoop(Statement node)
		{
			if (!InLoop())
			{
				Error(CompilerErrorFactory.NoEnclosingLoop(node));
			}
		}
		
		override public bool EnterWhileStatement(WhileStatement node)
		{
			EnterLoop();
			return true;
		}

		override public void LeaveWhileStatement(WhileStatement node)
		{
			node.Condition = CheckBoolContext(node.Condition);
			LeaveLoop();
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
			
				if (CurrentTryBlockDepth > 0)
				{
					Error(CompilerErrorFactory.YieldInsideTryBlock(node));
				}
			}
		}
		
		override public void LeaveReturnStatement(ReturnStatement node)
		{
			if (null != node.Expression)
			{
				IType returnType = _currentMethod.ReturnType;
				
				// forces anonymous types to be correctly
				// instantiated
				IType expressionType = GetConcreteExpressionType(node.Expression);
				
				if (TypeSystemServices.IsUnknown(returnType))
				{
					_currentMethod.AddReturnExpression(node.Expression);
				}
				else
				{
					CheckTypeCompatibility(node.Expression, returnType, expressionType);
				}
			}
		}
		
		Expression GetCorrectIterator(Expression iterator)
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
						Error(CompilerErrorFactory.InvalidIteratorType(iterator, type.FullName));
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
		Expression ProcessIterator(Expression iterator, DeclarationCollection declarations)
		{
			iterator = GetCorrectIterator(iterator);
			ProcessDeclarationsForIterator(declarations, GetExpressionType(iterator));
			return iterator;
		}
		
		override public void OnForStatement(ForStatement node)
		{
			Visit(node.Iterator);
			
			node.Iterator = ProcessIterator(node.Iterator, node.Declarations);
			
			EnterNamespace(new DeclarationsNamespace(CurrentNamespace, TypeSystemServices, node.Declarations));
			EnterLoop();
			Visit(node.Block);
			LeaveLoop();
			LeaveNamespace();
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
					CheckUniqueLocal(d);
				}
				else
				{
					IEntity tag = NameResolutionService.Resolve(d.Name);
					if (null != tag)
					{
						Bind(d, tag);
						CheckLValue(d);
						continue;
					}
				}
				DeclareLocal(d, false);
			}
		}
		
		override public void LeaveRaiseStatement(RaiseStatement node)
		{
			if (node.Exception != null)
			{
				IType exceptionType = GetExpressionType(node.Exception);
				if (TypeSystemServices.StringType == exceptionType)
				{
					MethodInvocationExpression expression = new MethodInvocationExpression(node.Exception.LexicalInfo);
					expression.Arguments.Add(node.Exception);
					expression.Target = new ReferenceExpression("System.ApplicationException");
					Bind(expression.Target, ApplicationException_StringConstructor);
					BindExpressionType(expression, TypeSystemServices.ApplicationExceptionType);

					node.Exception = expression;
				}
				else if (!TypeSystemServices.ExceptionType.IsAssignableFrom(exceptionType))
				{
					Error(CompilerErrorFactory.InvalidRaiseArgument(node.Exception,
								exceptionType.FullName));
				}
			}
			else
			{
				if (!InExceptionHandler())
				{
					Error(CompilerErrorFactory.ReRaiseOutsideExceptionHandler(node));
				}
			}
		}
		
		override public void OnTryStatement(TryStatement node)
		{
			EnterTryBlock();
			Visit(node.ProtectedBlock);
			
			EnterTryBlock();
			Visit(node.ExceptionHandlers);
			
			EnterTryBlock();
			Visit(node.EnsureBlock);
			LeaveTryBlock();
			
			LeaveTryBlock();
			LeaveTryBlock();
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
			EnterExceptionHandler();
			try
			{
				Visit(node.Block);
			}
			finally
			{
				LeaveExceptionHandler();
				LeaveNamespace();
			}
		}
		
		void LeaveIncrementDecrement(UnaryExpression node)
		{
			if (CheckLValue(node.Operand))
			{
				IType type = GetExpressionType(node.Operand);
				if (!IsNumber(type))
				{
					InvalidOperatorForType(node);
				}
				else
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
			}
			else
			{
				Error(node);
			}
		}
		
		Expression ExpandIncrementDecrementArraySlicing(UnaryExpression node)
		{
			SlicingExpression slicing = (SlicingExpression)node.Operand;
			CheckNoComplexSlicing(slicing);
			Visit(slicing);
			
			MethodInvocationExpression eval = CodeBuilder.CreateEvalInvocation(node.LexicalInfo);
			if (HasSideEffect(slicing.Target))
			{
				InternalLocal temp = AddInitializedTempLocal(eval, slicing.Target);
				slicing.Target = CodeBuilder.CreateReference(temp);
			}
			
			foreach (Slice slice in slicing.Indices)
			{
				Expression index = slice.Begin;
				if (HasSideEffect(index))
				{
					InternalLocal temp = AddInitializedTempLocal(eval, index);
					slice.Begin = CodeBuilder.CreateReference(temp);
				}
			}

			InternalLocal oldValue = DeclareOldValueTempIfNeeded(node);
			
			BinaryExpression addition = CodeBuilder.CreateBoundBinaryExpression(
				GetExpressionType(slicing),
				GetEquivalentBinaryOperator(node.Operator),
				CloneOrAssignToTemp(oldValue, slicing),
				CodeBuilder.CreateIntegerLiteral(1));
			Expression expansion = CodeBuilder.CreateAssignment(
					slicing.CloneNode(),
					addition);
			// Resolve operator overloads if any
			BindArithmeticOperator(addition);
			if (eval.Arguments.Count > 0 || null != oldValue)
			{
				eval.Arguments.Add(expansion);
				if (null != oldValue)
				{
					eval.Arguments.Add(CodeBuilder.CreateReference(oldValue));
				}
				BindExpressionType(eval, GetExpressionType(slicing));
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
					node.Operand = CheckBoolContext(node.Operand);
					BindExpressionType(node, TypeSystemServices.BoolType);
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
					if (IsPrimitiveNumber(node.Operand))
					{
						BindExpressionType(node, GetExpressionType(node.Operand));
					}
					else if (! ResolveOperator(node))
					{
						InvalidOperatorForType(node);
					}
					break;
				}
					
				default:
				{
					NotImplemented(node, "unary operator not supported");
					break;
				}
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
					if (null == info || TypeSystemServices.IsBuiltin(info))
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
		
		override public void LeaveBinaryExpression(BinaryExpression node)
		{
			if (TypeSystemServices.IsUnknown(node.Left) ||
				TypeSystemServices.IsUnknown(node.Right))
			{
				BindExpressionType(node, Unknown.Default);
				return;
			}
			
			if (TypeSystemServices.IsError(node.Left) || TypeSystemServices.IsError(node.Right))
			{
				Error(node);
				return;
			}
			BindBinaryExpression(node);
		}
		
		protected virtual void BindBinaryExpression(BinaryExpression node)
		{
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
				if (lhs.IsEnum && rhs == lhs)
				{
					BindExpressionType(node, lhs);
				}
				else
				{
					if (!ResolveOperator(node))
					{
						InvalidOperatorForTypes(node);
					}
				}
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
				if (lhs == rhs)
				{
					BindExpressionType(node, TypeSystemServices.BoolType);
				}
				else
				{
					InvalidOperatorForTypes(node);
				}
			}
			else if (!ResolveOperator(node))
			{
				switch (node.Operator)
				{
					case BinaryOperatorType.Equality:
					{
						if (IsNull(node.Left) || IsNull(node.Right))
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
						if (IsNull(node.Left) || IsNull(node.Right))
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
			node.Left = CheckBoolContext(node.Left);
			node.Right = CheckBoolContext(node.Right);
			BindExpressionType(node, GetMostGenericType(node));
		}
		
		void BindInPlaceAddSubtract(BinaryExpression node)
		{
			EntityType elementType = GetEntity(node.Left).EntityType;
			if (EntityType.Event == elementType ||
				EntityType.Ambiguous == elementType)
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
			if (!CheckDelegateArgument(node, eventInfo, rtype))
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
			if (CheckSwitchArguments(node))
			{
				for (int i=1; i<node.Arguments.Count; ++i)
				{
					ReferenceExpression label = (ReferenceExpression)node.Arguments[i];
					_currentMethod.AddLabelReference(label);
					label.ExpressionType = Unknown.Default;
				}
			}
			else
			{
				Error(node,
					CompilerErrorFactory.InvalidSwitch(node.Target));
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
					CheckHasSideEffect(node.Arguments[i]);
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
  			}
			else
			{
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
					Error(CompilerErrorFactory.InvalidLen(target, type.FullName));
				}
				if (null != resultingNode)
				{
					node.ParentNode.Replace(node, resultingNode);
				}
			}
		}
		
		void CheckListLiteralArgumentInArrayConstructor(IType expectedElementType, MethodInvocationExpression constructor)
		{
			ListLiteralExpression elements = constructor.Arguments[1] as ListLiteralExpression;
			if (null != elements)
			{
				foreach (Expression element in elements.Items)
				{
					CheckTypeCompatibility(element, expectedElementType, GetExpressionType(element));
				}
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
		
		IEntity ResolveAmbiguousMethodInvocation(MethodInvocationExpression node, Ambiguous entity)
		{
			_context.TraceVerbose("{0}: resolving ambigous method invocation: {1}", node.LexicalInfo, entity);
			
			IEntity[] entities = entity.Entities;
			IEntity resolved = ResolveCallableReference(node, node.Arguments, entities, true);
			if (null != resolved)
			{
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
			}
			return resolved;
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
			
			if (TypeSystemServices.IsError(node.Target) ||
				TypeSystemServices.IsErrorAny(node.Arguments))
			{
				Error(node);
				return;
			}
			
			if (null == targetEntity)
			{
				ProcessGenericMethodInvocation(node);
				return;
			}
			
			if (EntityType.Ambiguous == targetEntity.EntityType)
			{
				targetEntity = ResolveAmbiguousMethodInvocation(node, (Ambiguous)targetEntity);
				if (null == targetEntity)
				{
					Error(node);
					return;
				}
			}
			
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
					IMethod targetMethod = (IMethod)targetEntity;
					if (CheckParameters(node, targetMethod, node.Arguments))
					{
						if (node.NamedArguments.Count > 0)
						{
							Error(CompilerErrorFactory.NamedArgumentsNotAllowed(node.NamedArguments[0]));
						}
						else
						{
							CheckTargetContext(node.Target, targetMethod);
						}
					}
					
					BindExpressionType(node, targetMethod.ReturnType);
					ApplyBuiltinMethodTypeInference(node, targetMethod);
					
					break;
				}
				
				case EntityType.Constructor:
				{
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

						IConstructor targetConstructorInfo = FindCorrectConstructor(node, targetType, node.Arguments);
						if (null != targetConstructorInfo)
						{
							Bind(node.Target, targetConstructorInfo);
							BindExpressionType(node, targetConstructorInfo.ReturnType);
						}
					}
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
		
		void ReplaceTypeInvocationByEval(IType type, MethodInvocationExpression node)
		{
			Node parent = node.ParentNode;
			
			MethodInvocationExpression eval = CodeBuilder.CreateEvalInvocation(node.LexicalInfo);
			ReferenceExpression local = CreateTempLocal(node.Target.LexicalInfo, type);
			
			eval.Arguments.Add(CodeBuilder.CreateAssignment(local.CloneNode(), node));
			foreach (ExpressionPair pair in node.NamedArguments)
			{
				IEntity entity = GetEntity(pair.First);
				switch (entity.EntityType)
				{
					case EntityType.Event:
					{
						IEvent member = (IEvent)entity;
						eval.Arguments.Add(CodeBuilder.CreateMethodInvocation(
											local.CloneNode(),
											member.GetAddMethod(),
											pair.Second));
						break;
					}
					
					case EntityType.Field:
					{
						eval.Arguments.Add(CodeBuilder.CreateAssignment(
											CodeBuilder.CreateMemberReference(
												local.CloneNode(),
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
										node.LexicalInfo,
										CodeBuilder.CreateMemberReference(
											local.CloneNode(),
											property),
										pair.Second));
						}
						break;
					}
				}
			}
			node.NamedArguments.Clear();
			
			eval.Arguments.Add(local);
			
			BindExpressionType(eval, type);
			
			parent.Replace(node, eval);
		}
		
		void ProcessEventInvocation(IEvent ev, MethodInvocationExpression node)
		{
			IMethod method = ev.GetRaiseMethod();
			if (CheckParameters(node, method, node.Arguments))
			{
				node.Target = CodeBuilder.CreateMemberReference(
						((MemberReferenceExpression)node.Target).Target,
						method);
			}
		}
		
		void ProcessTypeInvocation(MethodInvocationExpression node)
		{
			IType type = (IType)node.Target.Entity;
			
			if (!CheckCanCreateInstance(node.Target, type))
			{
				Error(node);
				return;
			}
			ResolveNamedArguments(type, node.NamedArguments);
			
			IConstructor ctor = FindCorrectConstructor(node, type, node.Arguments);
			if (null != ctor)
			{
				// rebind the target now we know
				// it is a constructor call
				Bind(node.Target, ctor);
				BindExpressionType(node.Target, ctor.Type);
				BindExpressionType(node, type);
				
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
					CompilerErrorFactory.TypeIsNotCallable(node.Target, type.FullName));
			}
		}
		
		void ProcessMethodInvocationOnCallableExpression(MethodInvocationExpression node)
		{
			IType type = node.Target.ExpressionType;
			
			ICallableType delegateType = type as ICallableType;
			if (null != delegateType)
			{
				if (CheckParameters(node.Target, delegateType, delegateType, node.Arguments))
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
				Expression targetType = node.Target;
				
				node.Target = new ReferenceExpression(targetType.LexicalInfo,
											"System.Activator.CreateInstance");
										
				ArrayLiteralExpression args = CodeBuilder.CreateObjectArray(node.Arguments);
				
				node.Arguments.Clear();
				node.Arguments.Add(targetType);
				node.Arguments.Add(args);
				
				Bind(node.Target, Activator_CreateInstance);
				BindExpressionType(node, Activator_CreateInstance.ReturnType);
			}
			else
			{
				ProcessInvocationOnUnknownCallableExpression(node);
			}
		}
		
		protected virtual void ProcessInvocationOnUnknownCallableExpression(MethodInvocationExpression node)
		{
			NotImplemented(node, "Method invocation on type '" + node.Target.ExpressionType + "'.");
		}
		
		bool CheckIdentifierName(Node node, string name)
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
								tag.FullName));
								
				return false;
			}
			return true;
		}
		
		void BindAssignmentToSlice(BinaryExpression node)
		{
			SlicingExpression slice = (SlicingExpression)node.Left;
			
			if (GetExpressionType(slice.Target).IsArray)
			{
				BindAssignmentToSliceArray(node);
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
				if (!CheckTypeCompatibility(item.Begin, TypeSystemServices.IntType, GetExpressionType(item.Begin)))
				{
					Error(node);
					return;
				}
			}

			if (slice.Indices.Count > 1)
			{
				if (IsComplexSlicing(slice))
				{
					// FIXME: Check type compatibility
					BindAssignmentToComplexSliceArray(node);
				}
				else
				{
				if (!CheckTypeCompatibility(node.Right, sliceTargetType.GetElementType(), lhsType))
				{
					Error(node);
					return;
				}
					BindAssignmentToSimpleSliceArray(node);
				}
			}
			else
			{
				if (!CheckTypeCompatibility(node.Right, sliceTargetType.GetElementType(), lhsType))
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
				if (CheckParameters(node.Left, setMethod, mie.Arguments))
				{
					setter = setMethod;
				}
			}
			else if (EntityType.Ambiguous == lhs.EntityType)
			{
				setter = (IMethod)ResolveCallableReference(node.Left, mie.Arguments, GetSetMethods(((Ambiguous)lhs).Entities), false);
			}
			
			if (null == setter)
			{
				Error(node, CompilerErrorFactory.LValueExpected(node.Left));
			}
			else
			{
				mie.Target = CodeBuilder.CreateMemberReference(
											((MemberReferenceExpression)slice.Target).Target,
											setter);
				BindExpressionType(mie, setter.ReturnType);
				node.ParentNode.Replace(node, mie);
			}
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
			IType resultingType = TypeSystemServices.ErrorEntity;
			if (CheckLValue(node.Left))
			{
				IEntity lhs = GetEntity(node.Left);
				IType lhsType = GetExpressionType(node.Left);
				if (CheckTypeCompatibility(node.Right, lhsType, GetExpressionType(node.Right)))
				{
					resultingType = lhsType;
					
					if (EntityType.Property == lhs.EntityType)
					{
						IProperty property = (IProperty)lhs;
						if (IsIndexedProperty(property))
						{
							Error(CompilerErrorFactory.PropertyRequiresParameters(AstUtil.GetMemberAnchor(node.Left), property.FullName));
							resultingType = TypeSystemServices.ErrorEntity;
						}
					}
				}
			}
			BindExpressionType(node, resultingType);
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
			Node parent = node.ParentNode;
			
			Expression target = node.Left;
			if (null != target.Entity && EntityType.Property == target.Entity.EntityType)
			{
				// if target is a property force a rebinding
				target.ExpressionType = null;
			}
			
			BinaryExpression assign = new BinaryExpression(node.LexicalInfo);
			assign.Operator = BinaryOperatorType.Assign;
			assign.Left = target.CloneNode();
			assign.Right = node;
			node.Operator = GetRelatedBinaryOperatorForInPlaceOperator(node.Operator);
			
			parent.Replace(node, assign);
			Visit(assign);
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
		
		bool IsPublicEvent(IEntity tag)
		{
			if (EntityType.Event == tag.EntityType)
			{
				return ((IMember)tag).IsPublic;
			}
			return false;
		}
		
		bool IsPublicFieldPropertyEvent(IEntity tag)
		{
			EntityType flags = EntityType.Field|EntityType.Property|EntityType.Event;
			if ((flags & tag.EntityType) > 0)
			{
				IMember member = (IMember)tag;
				return member.IsPublic;
			}
			return false;
		}
		
		IMember ResolvePublicFieldPropertyEvent(Node sourceNode, IType type, string name)
		{
			IEntity candidate = NameResolutionService.Resolve(type, name, EntityType.Property|EntityType.Event|EntityType.Field);
			if (null != candidate)
			{
				if (IsPublicFieldPropertyEvent(candidate))
				{
					return (IMember)candidate;
				}
				else
				{
					if (candidate.EntityType == EntityType.Ambiguous)
					{
						IList found = ((Ambiguous)candidate).Filter(IsPublicFieldPropertyEventFilter);
						if (found.Count > 0)
						{
							if (found.Count > 1)
							{
								Error(CompilerErrorFactory.AmbiguousReference(sourceNode, name, found));
								return null;
							}
							else
							{
								return (IMember)found[0];
							}
						}
					}
				}
			}
			Error(CompilerErrorFactory.NotAPublicFieldOrProperty(sourceNode, type.FullName, name));
			return null;
		}
		
		void ResolveNamedArguments(IType typeInfo, ExpressionPairCollection arguments)
		{
			foreach (ExpressionPair arg in arguments)
			{
				Visit(arg.Second);
				
				if (NodeType.ReferenceExpression != arg.First.NodeType)
				{
					Error(CompilerErrorFactory.NamedParameterMustBeIdentifier(arg));
					continue;
				}
				
				ReferenceExpression name = (ReferenceExpression)arg.First;
				IMember member = ResolvePublicFieldPropertyEvent(name, typeInfo, name.Name);
				if (null == member)
				{
					continue;
				}
				
				Bind(arg.First, member);
				
				IType memberType = member.Type;
				if (member.EntityType == EntityType.Event)
				{
					CheckDelegateArgument(arg.First, member, GetExpressionType(arg.Second));
				}
				else
				{
					CheckTypeCompatibility(arg, memberType, GetExpressionType(arg.Second));
				}
			}
		}
		
		bool CheckTypeCompatibility(Node sourceNode, IType expectedType, IType actualType)
		{	
			if (!TypeSystemServices.AreTypesRelated(expectedType, actualType))
			{
				Error(CompilerErrorFactory.IncompatibleExpressionType(sourceNode, expectedType.FullName, actualType.FullName));
				return false;
			}
			return true;
		}
		
		bool CheckDelegateArgument(Node sourceNode, ITypedEntity delegateMember, ITypedEntity argumentInfo)
		{
			if (!delegateMember.Type.IsAssignableFrom(argumentInfo.Type))
			{
				Error(CompilerErrorFactory.EventArgumentMustBeAMethod(sourceNode, delegateMember.FullName, delegateMember.Type.FullName));
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
					!(IsNumber(expressionType) && IsNumber(parameterType)))
				{
					return false;
				}
			}
			return true;
		}
		
		bool CheckParameterTypes(ICallableType method, ExpressionCollection args, int count)
		{
			IParameter[] parameters = method.GetSignature().Parameters;
			for (int i=0; i<count; ++i)
			{
				IType parameterType = parameters[i].Type;
				IType argumentType = GetExpressionType(args[i]);
				if (parameterType.IsByRef)
				{
					if (!_callableResolution.IsValidByRefArg(parameterType, argumentType, args[i]))
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
		
		bool CheckParameters(Node sourceNode, IMethod method, ExpressionCollection args)
		{
			return CheckParameters(sourceNode, method, method.CallableType, args);
		}

		bool AcceptVarArgs(ICallableType method)
		{
			return method.GetSignature().AcceptVarArgs;
		}
		
		bool CheckParameters(Node sourceNode, IEntity sourceEntity, ICallableType method, ExpressionCollection args)
		{	
			return AcceptVarArgs(method)
				? CheckVarArgsParameters(sourceNode, sourceEntity, method, args)
				: CheckExactArgsParameters(sourceNode, sourceEntity, method, args);

		}

		bool CheckVarArgsParameters(Node sourceNode, IEntity sourceEntity, ICallableType method, ExpressionCollection args)
		{
			IParameter[] parameters = method.GetSignature().Parameters;
			if (args.Count < parameters.Length-1)
			{
				Error(CompilerErrorFactory.MethodArgumentCount(sourceNode, sourceEntity.Name, args.Count));
				return false;
			}
			if (_callableResolution.CalculateVarArgsScore(parameters, args) < 0)
			{
				Error(CompilerErrorFactory.MethodSignature(sourceNode, sourceEntity.ToString(), GetSignature(args)));
				return false;
			}
			return true;
		}

		bool CheckExactArgsParameters(Node sourceNode, IEntity sourceEntity, ICallableType method, ExpressionCollection args)
		{
			if (method.GetSignature().Parameters.Length != args.Count)
			{
				Error(CompilerErrorFactory.MethodArgumentCount(sourceNode, sourceEntity.Name, args.Count));
				return false;
			}
			
			if (!CheckParameterTypes(method, args, args.Count))
			{
				Error(CompilerErrorFactory.MethodSignature(sourceNode, sourceEntity.ToString(), GetSignature(args)));
				return false;
			}
			return true;
		}
		
		bool IsRuntimeIterator(IType type)
		{
			return  TypeSystemServices.IsSystemObject(type) ||
					IsTextReader(type);
		}
		
		bool IsTextReader(IType type)
		{
			return IsAssignableFrom(typeof(TextReader), type);
		}
		
		bool CheckTargetContext(Expression targetContext, IMember member)
		{
			if (!member.IsStatic)
			{
				if (NodeType.MemberReferenceExpression == targetContext.NodeType)
				{
					Expression targetReference = ((MemberReferenceExpression)targetContext).Target;
					IEntity entity = targetReference.Entity;
					if ((null != entity && EntityType.Type == entity.EntityType)
						|| (NodeType.SelfLiteralExpression == targetReference.NodeType
							&& _currentMethod.IsStatic))
					{
						Error(CompilerErrorFactory.InstanceRequired(targetContext, member.DeclaringType.FullName, member.Name));
						return false;
					}
				}
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
		
		IConstructor FindCorrectConstructor(Node sourceNode, IType typeInfo, ExpressionCollection arguments)
		{	
			IConstructor[] constructors = typeInfo.GetConstructors();
			if (constructors.Length > 0)
			{	
				return (IConstructor)ResolveCallableReference(sourceNode, arguments, constructors, true);
			}
			else
			{
				if (!TypeSystemServices.IsError(typeInfo))
				{
					Error(CompilerErrorFactory.NoApropriateConstructorFound(sourceNode, typeInfo.FullName, GetSignature(arguments)));
				}
			}
			return null;
		}

		IEntity ResolveCallableReference(Node sourceNode, NodeCollection args, IEntity[] candidates, bool treatErrors)
		{
			IEntity found = _callableResolution.ResolveCallableReference(args, candidates);
			if (null == found && treatErrors)
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
						Error(CompilerErrorFactory.NoApropriateConstructorFound(sourceNode, constructor.DeclaringType.FullName, GetSignature(args)));
					}
					else
					{
						Error(CompilerErrorFactory.NoApropriateOverloadFound(sourceNode, GetSignature(args), candidate.FullName));
					}
				}
			}
			return found;
		}
		
		override protected void EnsureRelatedNodeWasVisited(Node sourceNode, IEntity entity)
		{
			IInternalEntity internalInfo = entity as IInternalEntity;
			if (null != internalInfo)
			{
				Node node = internalInfo.Node;
				switch (node.NodeType)
				{
					case NodeType.Property:
					case NodeType.Field:
					{
						IMember memberEntity = (IMember)entity;
						if (TypeSystemServices.IsUnknown(memberEntity.Type))
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
				}
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
		
		protected string GetMethodNameForOperator(BinaryOperatorType op)
		{
			return "op_" + op.ToString();
		}
		
		protected string GetMethodNameForOperator(UnaryOperatorType op)
		{
			return "op_" + op.ToString();
		}
		
		bool ResolveOperator(UnaryExpression node)
		{
			MethodInvocationExpression mie = new MethodInvocationExpression(node.LexicalInfo);
			mie.Arguments.Add(node.Operand);
			
			string operatorName = GetMethodNameForOperator(node.Operator);
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
			mie.Arguments.Add(node.Left);
			mie.Arguments.Add(node.Right);
			
			string operatorName = GetMethodNameForOperator(node.Operator);
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
			IMethod method = null;
			IEntity entity = NameResolutionService.Resolve(type, operatorName, EntityType.Method);
			if (null != entity)
			{
				if (EntityType.Ambiguous == entity.EntityType)
				{
					method = ResolveAmbiguousOperator(((Ambiguous)entity).Entities, args);
				}
				else if (EntityType.Method == entity.EntityType)
				{
					IMethod candidate = (IMethod)entity;
					if (HasOperatorSignature(candidate, args))
					{
						method = candidate;
					}
				}
			}
			return method;
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
		
		Expression CheckBoolContext(Expression expression)
		{
			IType type = GetExpressionType(expression);
			if (type.IsValueType)
			{
				if (type != TypeSystemServices.BoolType &&
				    !IsNumber(type))
			    {
					Error(CompilerErrorFactory.BoolExpressionRequired(expression, type.FullName));
				}
			}
			else
			{
				IMethod method = FindImplicitConversionOperator(type, TypeSystemServices.BoolType);
				if (null != method)
				{
					expression = CodeBuilder.CreateMethodInvocation(
									method,
									expression);
				}
			}
			// reference types can be used in bool context
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
		
		InternalLocal DeclareTempLocal(IType localType)
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
		
		IType CurrentType
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
		
		void CheckHasSideEffect(Expression expression)
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
		
		bool CheckCanCreateInstance(Node sourceNode, IType type)
		{
			if (type.IsInterface)
			{
				Error(CompilerErrorFactory.CantCreateInstanceOfInterface(sourceNode, type.FullName));
				return false;
			}
			if (type.IsEnum)
			{
				Error(CompilerErrorFactory.CantCreateInstanceOfEnum(sourceNode, type.FullName));
				return false;
			}
			if (type.IsAbstract)
			{
				Error(CompilerErrorFactory.CantCreateInstanceOfAbstractType(sourceNode, type.FullName));
				return false;
			}
			return true;
		}
		
		bool CheckDeclarationName(Declaration d)
		{
			if (CheckIdentifierName(d, d.Name))
			{
				return CheckUniqueLocal(d);
			}
			return false;
		}
		
		bool CheckUniqueLocal(Declaration d)
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
				CheckTypeCompatibility(d, GetType(d.Type), defaultDeclarationType);
			}
			else
			{
				d.Type = CodeBuilder.CreateTypeReference(defaultDeclarationType);
			}
		}
		
		void DeclareLocal(Declaration d, bool privateScope)
		{
			if (CheckIdentifierName(d, d.Name))
			{
				d.Entity = DeclareLocal(d, d.Name, GetType(d.Type), privateScope);
			}
		}
		
		IType GetEnumeratorItemType(IType iteratorType)
		{
			return TypeSystemServices.GetEnumeratorItemType(iteratorType);
		}
		
		void ProcessDeclarationsForIterator(DeclarationCollection declarations, IType iteratorType)
		{
			IType defaultDeclType = GetEnumeratorItemType(iteratorType);
			if (declarations.Count > 1)
			{
				// will enumerate (unpack) each item
				defaultDeclType = GetEnumeratorItemType(defaultDeclType);
			}
			
			foreach (Declaration d in declarations)
			{
				ProcessDeclarationType(defaultDeclType, d);
				DeclareLocal(d, true);
			}
		}

		protected virtual bool CheckLValue(Node node)
		{
			IEntity tag = node.Entity;
			if (null != tag)
			{
				switch (tag.EntityType)
				{
					case EntityType.Parameter:
					case EntityType.Local:
					{
						return true;
					}
					
					case EntityType.Property:
					{
						if (null == ((IProperty)tag).GetSetMethod())
						{
							Error(CompilerErrorFactory.PropertyIsReadOnly(AstUtil.GetMemberAnchor(node), tag.FullName));
							return false;
						}
						return true;
					}
					
					case EntityType.Field:
					{
						if (TypeSystemServices.IsReadOnlyField((IField)tag))
						{
							Error(CompilerErrorFactory.FieldIsReadonly(AstUtil.GetMemberAnchor(node), tag.FullName));
							return false;
						}
						return true;
					}
				}
			}
			else
			{
				if (IsArraySlicing(node))
				{
					return true;
				}
			}
			
			Error(CompilerErrorFactory.LValueExpected(node));
			return false;
		}
		
		protected bool IsArraySlicing(Node node)
		{
			if (node.NodeType == NodeType.SlicingExpression)
			{
				IType type = ((SlicingExpression)node).Target.ExpressionType;
				return null != type && type.IsArray;
			}
			return false;
		}

		bool IsStandaloneReference(Node node)
		{
			return node.ParentNode.NodeType != NodeType.MemberReferenceExpression;
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
							GetExpressionType(node.Operand).FullName));
		}
		
		void InvalidOperatorForTypes(BinaryExpression node)
		{
			Error(node, CompilerErrorFactory.InvalidOperatorForTypes(node,
							GetBinaryOperatorText(node.Operator),
							GetExpressionType(node.Left).FullName,
							GetExpressionType(node.Right).FullName));
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
		#endregion
	}
}
