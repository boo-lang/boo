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

namespace Boo.Lang.Compiler.Steps
{
	using System;
	using System.Text;
	using System.Collections;
	using System.Reflection;
	using Boo;
	using Boo.Lang;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.TypeSystem;

	/// <summary>
	/// AST semantic evaluation.
	/// </summary>
	public class ProcessMethodBodies : AbstractNamespaceSensitiveVisitorCompilerStep
	{	
		static readonly ExpressionCollection EmptyExpressionCollection = new ExpressionCollection();
		
		Stack _methodStack;
		
		InternalMethod _currentMethod;
		
		Hash _visited;
		
		List _newAbstractClasses;
		
		IMethod RuntimeServices_Len;
		
		IMethod RuntimeServices_Mid;
		
		IMethod RuntimeServices_NormalizeStringIndex;
		
		IMethod RuntimeServices_GetRange1;
		
		IMethod RuntimeServices_GetRange2;
		
		IMethod RuntimeServices_GetEnumerable;
		
		IMethod RuntimeServices_op_Equality;
		
		IMethod Array_get_Length;
		
		IMethod String_get_Length;
		
		IMethod String_Substring_Int;
		
		IMethod ICollection_get_Count;
		
		IMethod IList_Contains;
		
		IMethod List_GetRange1;
		
		IMethod List_GetRange2;
		
		IMethod IDictionary_Contains;
		
		IMethod Array_EnumerableConstructor;
		
		IMethod Array_TypedEnumerableConstructor;
		
		IMethod Array_TypedCollectionConstructor;
		
		IMethod Array_TypedConstructor2;
		
		IMethod ICallable_Call;
		
		IMethod Activator_CreateInstance;
		
		IConstructor ApplicationException_StringConstructor;
		
		IConstructor TextReaderEnumerator_Constructor;
		
		IConstructor EnumeratorItemType_Constructor;
		
		IMethod Delegate_Combine;
		
		IMethod Delegate_Remove;
		
		InfoFilter IsPublicEventFilter;
		
		InfoFilter IsPublicFieldPropertyEventFilter;
		
		MethodBodyState _methodBodyState;
		
		struct MethodBodyState
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
			if (Errors.Count > 0)
			{
				return;
			}
			
			NameResolutionService.Reset();
			
			_currentMethod = null;
			_methodStack = new Stack();
			_visited = new Hash();
			_newAbstractClasses = new List();
			_methodBodyState = new MethodBodyState();
						
			InitializeMemberCache();
			
			Visit(CompileUnit);
			
			ProcessNewAbstractClasses();
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
			List_GetRange1 = (IMethod)TypeSystemServices.Map(Types.List.GetMethod("GetRange", new Type[] { typeof(int) }));
			List_GetRange2 = (IMethod)TypeSystemServices.Map(Types.List.GetMethod("GetRange", new Type[] { typeof(int), typeof(int) }));
			RuntimeServices_GetRange1 = ResolveMethod(TypeSystemServices.RuntimeServicesType, "GetRange1");
			RuntimeServices_GetRange2 = ResolveMethod(TypeSystemServices.RuntimeServicesType, "GetRange2"); 			
			RuntimeServices_Len = ResolveMethod(TypeSystemServices.RuntimeServicesType, "Len");
			RuntimeServices_Mid = ResolveMethod(TypeSystemServices.RuntimeServicesType, "Mid");
			RuntimeServices_NormalizeStringIndex = ResolveMethod(TypeSystemServices.RuntimeServicesType, "NormalizeStringIndex");			
			RuntimeServices_GetEnumerable = ResolveMethod(TypeSystemServices.RuntimeServicesType, "GetEnumerable");			
			RuntimeServices_op_Equality = (IMethod)TypeSystemServices.Map(Types.RuntimeServices.GetMethod("op_Equality", new Type[] { Types.Object, Types.Object }));
			Array_get_Length = ResolveProperty(TypeSystemServices.ArrayType, "Length").GetGetMethod();
			String_get_Length = ResolveProperty(TypeSystemServices.StringType, "Length").GetGetMethod();
			String_Substring_Int = (IMethod)TypeSystemServices.Map(Types.String.GetMethod("Substring", new Type[] { Types.Int }));
			ICollection_get_Count = ResolveProperty(TypeSystemServices.ICollectionType, "Count").GetGetMethod();
			IList_Contains = ResolveMethod(TypeSystemServices.IListType, "Contains");
			IDictionary_Contains = ResolveMethod(TypeSystemServices.IDictionaryType, "Contains");
			Array_EnumerableConstructor = (IMethod)TypeSystemServices.Map(Types.Builtins.GetMethod("array", new Type[] { Types.IEnumerable }));
			Array_TypedEnumerableConstructor = (IMethod)TypeSystemServices.Map(Types.Builtins.GetMethod("array", new Type[] { Types.Type, Types.IEnumerable }));
			Array_TypedCollectionConstructor= (IMethod)TypeSystemServices.Map(Types.Builtins.GetMethod("array", new Type[] { Types.Type, Types.ICollection }));
			Array_TypedConstructor2 = (IMethod)TypeSystemServices.Map(Types.Builtins.GetMethod("array", new Type[] { Types.Type, Types.Int }));
			ICallable_Call = ResolveMethod(TypeSystemServices.ICallableType, "Call");
			Activator_CreateInstance = (IMethod)TypeSystemServices.Map(typeof(Activator).GetMethod("CreateInstance", new Type[] { Types.Type, Types.ObjectArray }));
			TextReaderEnumerator_Constructor = (IConstructor)TypeSystemServices.Map(typeof(Boo.IO.TextReaderEnumerator).GetConstructor(new Type[] { typeof(System.IO.TextReader) }));
			EnumeratorItemType_Constructor = TypeSystemServices.Map(typeof(Boo.Lang.EnumeratorItemTypeAttribute)).GetConstructors()[0];
			
			Type delegateType = Types.Delegate;
			Type[] delegates = new Type[] { delegateType, delegateType };
			Delegate_Combine = TypeSystemServices.Map(delegateType.GetMethod("Combine", delegates));
			Delegate_Remove = TypeSystemServices.Map(delegateType.GetMethod("Remove", delegates));			
			
			ApplicationException_StringConstructor =
					(IConstructor)TypeSystemServices.Map(
						Types.ApplicationException.GetConstructor(new Type[] { typeof(string) }));
			
		}
		
		override public void Dispose()
		{
			base.Dispose();
			
			_currentMethod = null;
			_methodStack = null;
			_visited = null;
			_newAbstractClasses = null;
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
		
		override public void OnModule(Boo.Lang.Compiler.Ast.Module module)
		{				
			if (Visited(module))
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
			if (Visited(node))
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
			if (Visited(node))
			{
				return;
			}			
			MarkVisited(node);
			VisitBaseTypes(node);
			
			InternalClass tag = (InternalClass)GetEntity(node);			
			EnterNamespace(tag);
			Visit(node.Attributes);
			//Visit(node.Members, NodeType.Field);
			Visit(node.Members);			
			LeaveNamespace();
			
			ProcessInheritedAbstractMembers(node);
		}		
		
		void VisitBaseTypes(ClassDefinition node)
		{
			foreach (TypeReference baseTypeRef in node.BaseTypes)
			{
				EnsureRelatedNodeWasVisited(baseTypeRef.Entity);
			}
		}
		
		override public void OnAttribute(Boo.Lang.Compiler.Ast.Attribute node)
		{
			IType tag = node.Entity as IType;
			if (null != tag && !TypeSystemServices.IsError(tag))
			{			
				Visit(node.Arguments);
				ResolveNamedArguments(node, tag, node.NamedArguments);
				
				IConstructor constructor = FindCorrectConstructor(node, tag, node.Arguments);
				if (null != constructor)
				{
					Bind(node, constructor);
				}
			}
		}
		
		Method CreateEventMethod(Event node, string prefix)
		{
			Method method = CodeBuilder.CreateMethod(prefix + node.Name,
													TypeSystemServices.VoidType,
													node.Modifiers);
			method.Parameters.Add(
					CodeBuilder.CreateParameterDeclaration(
						1,
						"handler",
						GetType(node.Type)));
			return method;
		}
		
		Method CreateEventAddMethod(Event node, Field backingField)
		{
			Method m = CreateEventMethod(node, "add_");
			m.Body.Add(
				CodeBuilder.CreateAssignment(
					CodeBuilder.CreateReference(backingField),
					CodeBuilder.CreateMethodInvocation(
						Delegate_Combine,
						CodeBuilder.CreateReference(backingField),
						CodeBuilder.CreateReference(m.Parameters[0]))));
			return m;
		}
		
		Method CreateEventRemoveMethod(Event node, Field backingField)
		{
			Method m = CreateEventMethod(node, "remove_");
			m.Body.Add(
				CodeBuilder.CreateAssignment(
					CodeBuilder.CreateReference(backingField),
					CodeBuilder.CreateMethodInvocation(
						Delegate_Remove,
						CodeBuilder.CreateReference(backingField),
						CodeBuilder.CreateReference(m.Parameters[0]))));
			return m;
		}
		
		Method CreateEventRaiseMethod(Event node, Field backingField)
		{
			Method method = CodeBuilder.CreateMethod("raise_" + node.Name,
													TypeSystemServices.VoidType,
													node.Modifiers);
													
			ICallableType type = GetEntity(node.Type) as ICallableType;
			if (null != type)
			{
				int index = 1;
				foreach (IParameter parameter in type.GetSignature().Parameters)
				{
					method.Parameters.Add(
						CodeBuilder.CreateParameterDeclaration(
							index,
							parameter.Name,
							parameter.Type));
					++index;
				}
			}
			
			MethodInvocationExpression mie = CodeBuilder.CreateMethodInvocation(
							CodeBuilder.CreateReference(backingField),
							ResolveMethod(GetType(backingField.Type), "Invoke"));
			foreach (ParameterDeclaration parameter in method.Parameters)
			{
				mie.Arguments.Add(CodeBuilder.CreateReference(parameter));
			}
			
			IfStatement stmt = new IfStatement(node.LexicalInfo);
			stmt.Condition = CodeBuilder.CreateNotNullTest(
								CodeBuilder.CreateReference(backingField));
			stmt.TrueBlock = new Block();
			stmt.TrueBlock.Add(mie);
			method.Body.Add(stmt);
			return method;
		}
		
		override public void OnEvent(Event node)
		{
			if (Visited(node))
			{
				return;
			}
			MarkVisited(node);
			
			Visit(node.Attributes);
			Visit(node.Type);
			
			IType type = GetType(node.Type);
			bool typeIsCallable = type is ICallableType;
			if (!typeIsCallable)
			{
				Errors.Add(
					CompilerErrorFactory.EventTypeIsNotCallable(node.Type,
						type.FullName));
			}
			
			Field backingField = CodeBuilder.CreateField("___" + node.Name, type);
			node.DeclaringType.Members.Add(backingField);
			
			((InternalEvent)node.Entity).BackingField = (InternalField)backingField.Entity;
			
			if (null == node.Add)
			{
				node.Add = CreateEventAddMethod(node, backingField);
			}
			else
			{
				Visit(node.Add);
			}
			
			if (null == node.Remove)
			{
				node.Remove = CreateEventRemoveMethod(node, backingField);
			}
			else
			{
				Visit(node.Remove);
			}
			
			if (null == node.Raise)
			{
				if (typeIsCallable)
				{
					node.Raise = CreateEventRaiseMethod(node, backingField);
				}
			}
			else
			{
				Visit(node.Raise);
			}
		}
		
		override public void OnProperty(Property node)
		{
			if (Visited(node))
			{
				return;
			}			
			MarkVisited(node);
			
			InternalProperty property = (InternalProperty)GetEntity(node);
			
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
				getter.Name = "get_" + node.Name;
				getter.Parameters.ExtendWithClones(node.Parameters);
				
				SetPropertyAccessorModifiers(node, getter);
				
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
				SetPropertyAccessorModifiers(node, setter);
				
				ParameterDeclaration parameter = new ParameterDeclaration();
				parameter.Type = CodeBuilder.CreateTypeReference(typeInfo);
				parameter.Name = "value";
				parameter.Entity = new InternalParameter(parameter, node.Parameters.Count+GetFirstParameterIndex(setter));
				setter.Parameters.ExtendWithClones(node.Parameters);
				setter.Parameters.Add(parameter);
				setter.Name = "set_" + node.Name;				
				Visit(setter);
			}
		}
		
		void SetPropertyAccessorModifiers(Property property, Method accessor)
		{
			if (property.IsStatic)
			{
				accessor.Modifiers |= TypeMemberModifiers.Static;
			}
			
			if (property.IsVirtual)
			{
				accessor.Modifiers |= TypeMemberModifiers.Virtual;
			}
			
			if (property.IsOverride)
			{
				accessor.Modifiers |= TypeMemberModifiers.Override;
			}
			
			if (property.IsAbstract)
			{
				accessor.Modifiers |= TypeMemberModifiers.Abstract;
			}
		}
		
		int GetFirstParameterIndex(TypeMember member)
		{
			return member.IsStatic ? 0 : 1;
		}
		
		override public void OnField(Field node)
		{
			if (Visited(node))
			{
				return;
			}			
			MarkVisited(node);
			
			InternalField tag = (InternalField)GetEntity(node);
			
			Visit(node.Attributes);
			Visit(node.Type);			
			
			if (null != node.Initializer)
			{
				ProcessFieldInitializer(node);
			}
			else
			{			
				if (null == node.Type)
				{
					node.Type = CodeBuilder.CreateTypeReference(TypeSystemServices.ObjectType);
				}
			}
		}	
		
		void ProcessFieldInitializer(Field node)
		{			
			Expression initializer = node.Initializer;
			
			Method method = GetFieldsInitializerMethod(node);
			InternalMethod entity = (InternalMethod)method.Entity;
				
			ReferenceExpression temp = new ReferenceExpression("___temp_initializer");
			BinaryExpression assignment = new BinaryExpression(
						node.LexicalInfo,
						BinaryOperatorType.Assign,
						temp,
						initializer);
						
			method.Body.Add(assignment);
			ProcessNodeInMethodContext(entity, entity, assignment);
			method.Locals.RemoveByEntity(temp.Entity);
				
			IType initializerType = ((ITypedEntity)temp.Entity).Type;
			if (null == node.Type)
			{
				node.Type = CodeBuilder.CreateTypeReference(initializerType);
			}
			else
			{			
				CheckTypeCompatibility(node.Initializer, GetType(node.Type), initializerType);
			}
			assignment.Left = CodeBuilder.CreateReference(node);
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
			Statement stmt = CreateFieldAssignment(node);
			constructor.Body.Statements.Insert(index, stmt);
			node.Initializer = null;			
		}
		
		Statement CreateFieldAssignment(Field node)
		{
			InternalField fieldEntity = (InternalField)GetEntity(node);
			
			ExpressionStatement stmt = new ExpressionStatement(node.Initializer.LexicalInfo);
			
			Expression context = null;
			if (node.IsStatic)
			{
				context = CodeBuilder.CreateReference(node.LexicalInfo, fieldEntity.DeclaringType);				
			}
			else
			{
				context = CodeBuilder.CreateSelfReference(fieldEntity.Type);
			}			
			
			// <node.Name> = <node.Initializer>
			stmt.Expression = new BinaryExpression(BinaryOperatorType.Assign,
									CodeBuilder.CreateMemberReference(context, fieldEntity),
									node.Initializer);
			BindExpressionType(stmt.Expression, fieldEntity.Type);
			
			return stmt;
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
			if (Visited(node))
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
				if (!entity.HasSuperCall && !entity.IsStatic)
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
			
			CodeBuilder.BindParameterDeclarations(_currentMethod.IsStatic, closure.Parameters);
			
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
			if (Visited(method))
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
				ProcessRegularMethod(method);
			}
		}
		
		void ProcessRegularMethod(Method method)
		{
			bool parentIsClass = method.DeclaringType.NodeType == NodeType.ClassDefinition;
			
			InternalMethod tag = (InternalMethod)GetEntity(method);
			if (method.IsOverride)
			{
				if (parentIsClass)
				{
					ResolveMethodOverride(tag);
				}
				else
				{
					// TODO: only class methods can be marked 'override'
				}
			}
			
			if (method.IsAbstract)
			{
				if (parentIsClass)
				{
					method.DeclaringType.Modifiers |= TypeMemberModifiers.Abstract;
					if (method.Body.Statements.Count > 0)
					{
						Error(CompilerErrorFactory.AbstractMethodCantHaveBody(method, method.FullName));
					}
				}
				else
				{
					// TODO: only class method can be marked 'abstract'
				}
			}
			
			ProcessMethodBody(tag);
			
			if (parentIsClass)
			{				
				if (TypeSystemServices.IsUnknown(tag.ReturnType))
				{
					TryToResolveReturnType(tag);					
				}
				else
				{
					if (!method.IsOverride)
					{
						CheckMethodOverride(tag);
					}
					
					if (tag.IsGenerator)
					{
						CheckGeneratorReturnType(method, tag.ReturnType);
					}
				}
				CheckGeneratorCantReturnValues(tag);
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
		
		void ProcessMethodBody(InternalMethod tag)
		{
			ProcessMethodBody(tag, tag);
		}
		
		void ProcessMethodBody(InternalMethod tag, INamespace ns)
		{
			ProcessNodeInMethodContext(tag, ns, tag.Method.Body);
			ResolveLabelReferences(tag);
		}
		
		void ProcessNodeInMethodContext(InternalMethod tag, INamespace ns, Node node)
		{
			PushMethodInfo(tag);
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
			
			IType itemType = GetMostGenericType(entity.YieldExpressions);
			BooClassBuilder generatorType = CreateGeneratorSkeleton(method.DeclaringType, method, itemType);
			
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
					CheckMethodOverride(tag);
				}
				else
				{
					Error(CompilerErrorFactory.RecursiveMethodWithoutReturnType(tag.Method));
				}
			}
		}
		
		override public void OnSuperLiteralExpression(SuperLiteralExpression node)
		{			
			node.Entity = _currentMethod;
			node.ExpressionType = _currentMethod.DeclaringType.BaseType;
			if (EntityType.Constructor != _currentMethod.EntityType)
			{
				_currentMethod.AddSuperExpression(node);
			}
		}
		
		/// <summary>
		/// Checks if the specified method overrides any virtual
		/// method in the base class.
		/// </summary>
		void CheckMethodOverride(InternalMethod tag)
		{		
			IMethod baseMethod = FindMethodOverride(tag);
			if (null == baseMethod || tag.ReturnType != baseMethod.ReturnType)
			{
				if (null != tag.SuperExpressions)
				{
					foreach (Expression super in tag.SuperExpressions)
					{
						Error(CompilerErrorFactory.MethodIsNotOverride(super, GetSignature(tag)));
					}
				}
			}
			else
			{
				if (baseMethod.IsVirtual)
				{
					SetOverride(tag, baseMethod);
				}
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
					// TODO: check type and signature
				}
				else
				{
					property.Modifiers |= TypeMemberModifiers.Override;					
				}
				
				if (null == property.Type)
				{
					property.Type = CodeBuilder.CreateTypeReference(entity.Override.Type);
				}
			}
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
				
				IMethod accessorEntity = (IMethod)accessor.Entity;
				if (TypeSystemServices.CheckOverrideSignature(accessorEntity, baseMethod))
				{
					return baseMethod;
				}
			}
			return null;
		}
		
		IMethod FindMethodOverride(InternalMethod tag)
		{
			Method method = tag.Method;
			if (NodeType.Property == method.ParentNode.NodeType)
			{
				return FindPropertyAccessorOverride((Property)method.ParentNode, method);
			}
			
			IType baseType = tag.DeclaringType.BaseType;						
			IEntity baseMethods = NameResolutionService.Resolve(baseType, tag.Name, EntityType.Method);
			
			if (null != baseMethods)
			{
				if (EntityType.Method == baseMethods.EntityType)
				{
					IMethod baseMethod = (IMethod)baseMethods;
					if (TypeSystemServices.CheckOverrideSignature(tag, baseMethod))
					{	
						return baseMethod;
					}
				}
				else if (EntityType.Ambiguous == baseMethods.EntityType)
				{
					IEntity[] tags = ((Ambiguous)baseMethods).Entities;
					IMethod baseMethod = (IMethod)ResolveCallableReference(method, method.Parameters, tags, false);
					if (null != baseMethod)
					{
						return baseMethod;
					}
				}
			}
			return null;
		}
		
		void ResolveMethodOverride(InternalMethod tag)
		{	
			IMethod baseMethod = FindMethodOverride(tag);
			if (null == baseMethod)
			{
				Error(CompilerErrorFactory.NoMethodToOverride(tag.Method, GetSignature(tag)));
			}
			else
			{
				if (!baseMethod.IsVirtual)
				{
					CantOverrideNonVirtual(tag.Method, baseMethod);
				}
				else
				{
					if (TypeSystemServices.IsUnknown(tag.ReturnType))
					{
						tag.Method.ReturnType = CodeBuilder.CreateTypeReference(baseMethod.ReturnType);
					}
					else
					{
						if (baseMethod.ReturnType != tag.ReturnType)
						{
							Error(CompilerErrorFactory.InvalidOverrideReturnType(
											tag.Method.ReturnType,
											baseMethod.FullName,
											baseMethod.ReturnType.FullName,
											tag.ReturnType.FullName));
						}
					}
					SetOverride(tag, baseMethod);
				}
			}
		}
		
		void CantOverrideNonVirtual(Method method, IMethod baseMethod)
		{
			Error(CompilerErrorFactory.CantOverrideNonVirtual(method, baseMethod.ToString()));
		}
		
		void SetOverride(InternalMethod tag, IMethod baseMethod)
		{
			tag.Override = baseMethod;
			TraceOverride(tag.Method, baseMethod);
			tag.Method.Modifiers |= TypeMemberModifiers.Override;
		}
		
		IType GetBaseType(TypeDefinition typeDefinition)
		{
			return ((IType)GetEntity(typeDefinition)).BaseType;
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
		
		override public void OnArrayTypeReference(ArrayTypeReference node)
		{
			NameResolutionService.ResolveArrayTypeReference(node);
		}
		
		override public void OnSimpleTypeReference(SimpleTypeReference node)
		{
			NameResolutionService.ResolveSimpleTypeReference(node);
			if (node.Entity is InternalCallableType)
			{
				EnsureRelatedNodeWasVisited(node.Entity);
			}
		}
		
		override public void LeaveCallableTypeReference(CallableTypeReference node)
		{
			IParameter[] parameters = new IParameter[node.Parameters.Count];
			for (int i=0; i<parameters.Length; ++i)
			{
				parameters[i] = new SimpleParameter("arg" + i, GetType(node.Parameters[i]));
			}
			
			IType returnType = null;
			if (null != node.ReturnType)
			{
				returnType = GetType(node.ReturnType);
			}
			else
			{
				returnType = TypeSystemServices.VoidType;
			}
			
			node.Entity = TypeSystemServices.GetConcreteCallableType(node, new CallableSignature(parameters, returnType));
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
			BindExpressionType(node, TypeSystemServices.DoubleType);
		}
		
		override public void OnStringLiteralExpression(StringLiteralExpression node)
		{
			BindExpressionType(node, TypeSystemServices.StringType);
		}
		
		IEntity[] GetSetMethods(IEntity[] tags)
		{
			Boo.Lang.List setMethods = new Boo.Lang.List();
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
			Boo.Lang.List getMethods = new Boo.Lang.List();
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
			return null != node.End || null != node.Step || OmittedExpression.Default == node.Begin;
		}
		
		protected MethodInvocationExpression CreateEquals(BinaryExpression node)
		{
			return CodeBuilder.CreateMethodInvocation(RuntimeServices_op_Equality, node.Left, node.Right);
		}
		
		IntegerLiteralExpression CreateIntegerLiteral(long value)
		{
			IntegerLiteralExpression expression = new IntegerLiteralExpression(value);
			Visit(expression);
			return expression;
		}
		
		bool CheckComplexSlicingParameters(SlicingExpression node)
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
					Error(node);
					return false;
				}
			}			
			
			if (null != node.End && OmittedExpression.Default != node.End)
			{
				if (!CheckTypeCompatibility(node.End, TypeSystemServices.IntType, GetExpressionType(node.End)))
				{
					Error(node);
					return false;
				}
			}
			
			return true;
		}
		
		void BindComplexListSlicing(SlicingExpression node)
		{
			if (CheckComplexSlicingParameters(node))
			{
				MethodInvocationExpression mie = null;
				
				if (null == node.End || node.End == OmittedExpression.Default)
				{
					mie = CodeBuilder.CreateMethodInvocation(node.Target, List_GetRange1);
					mie.Arguments.Add(node.Begin);
				}
				else
				{				
					mie = CodeBuilder.CreateMethodInvocation(node.Target, List_GetRange2);
					mie.Arguments.Add(node.Begin);
					mie.Arguments.Add(node.End);
				}
				node.ParentNode.Replace(node, mie);				
			}
		}
		
		void BindComplexArraySlicing(SlicingExpression node)
		{			
			if (CheckComplexSlicingParameters(node))
			{
				MethodInvocationExpression mie = null; 
				
				if (null == node.End || node.End == OmittedExpression.Default)
				{
					mie = CodeBuilder.CreateMethodInvocation(RuntimeServices_GetRange1, node.Target, node.Begin);
				}
				else
				{
					mie = CodeBuilder.CreateMethodInvocation(RuntimeServices_GetRange2, node.Target, node.Begin, node.End);
				}				
				
				BindExpressionType(mie, GetExpressionType(node.Target));
				node.ParentNode.Replace(node, mie);
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
			if (CheckComplexSlicingParameters(node))
			{
				MethodInvocationExpression mie = null;
				
				if (null == node.End || node.End == OmittedExpression.Default)
				{
					if (NeedsNormalization(node.Begin))
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
									node.Begin)));
					}
					else
					{
						mie = CodeBuilder.CreateMethodInvocation(node.Target, String_Substring_Int, node.Begin);
					}
				}
				else
				{	
					mie = CodeBuilder.CreateMethodInvocation(RuntimeServices_Mid, node.Target, node.Begin, node.End);
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
			IType targetType = GetExpressionType(node.Target);
			if (TypeSystemServices.IsError(targetType))
			{
				Error(node);
				return;
			}
			
			if (IsIndexedProperty(node.Target))
			{
				CheckNoComplexSlicing(node);
				SliceMember(node, node.Target.Entity, false);
			}
			else
			{
				if (targetType.IsArray)
				{
					IArrayType arrayType = (IArrayType)targetType;
					if (arrayType.GetArrayRank() != 1)
					{
						Error(node, CompilerErrorFactory.InvalidArray(node.Target));
						return;
					}
					
					if (IsComplexSlicing(node))
					{
						BindComplexArraySlicing(node);
					}
					else
					{
						BindExpressionType(node, arrayType.GetElementType());
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
							SliceMember(node, member, true);
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
		
		void SliceMember(SlicingExpression node, IEntity member, bool defaultMember)
		{
			if (AstUtil.IsLhsOfAssignment(node))
			{
				// leave it to LeaveBinaryExpression to resolve
				Bind(node, member);
				return;
			}
			
			MethodInvocationExpression mie = new MethodInvocationExpression(node.LexicalInfo);
			mie.Arguments.Add(node.Begin);
			
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
					Expression target = node.Target;
					if (!defaultMember)
					{
						target = ((MemberReferenceExpression)node.Target).Target;						
					}
					
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
			builder.AddBaseType(TypeSystemServices.Map(typeof(Boo.Lang.AbstractGenerator)));
			builder.AddAttribute(CodeBuilder.CreateAttribute(
												EnumeratorItemType_Constructor,
												CodeBuilder.CreateTypeofExpression(generatorItemType)));
			builder.LexicalInfo = node.LexicalInfo;
			parentType.Members.Add(builder.ClassDefinition);
			
			node["GeneratorClassBuilder"] = builder;
			node["GetEnumeratorBuilder"] = builder.AddVirtualMethod("GetEnumerator", TypeSystemServices.IEnumeratorType);
			
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
				BindExpressionType(node, TypeSystemServices.GetArrayType(GetMostGenericType(items)));
			}
		}
		
		override public void LeaveDeclarationStatement(DeclarationStatement node)
		{
			IType type = TypeSystemServices.ObjectType;
			if (null != node.Declaration.Type)
			{
				type = GetType(node.Declaration.Type);			
			}			
			
			CheckDeclarationName(node.Declaration);
			
			IEntity localInfo = DeclareLocal(node.Declaration.Name, type);
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
				if (_currentMethod.IsStatic)
				{
					Error(node, CompilerErrorFactory.ObjectRequired(node));
				}
				else
				{
					TypeDefinition typedef = _currentMethod.Method.DeclaringType;
					IType type = (IType)TypeSystemServices.GetEntity(typedef);
					BindExpressionType(node, type);
				}
			}
		}
		
		override public void LeaveTypeofExpression(TypeofExpression node)
		{			
			BindExpressionType(node, TypeSystemServices.TypeType);
		}
		
		override public void LeaveCastExpression(CastExpression node)
		{
			IType toType = GetType(node.Type);
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
				BindExpressionType(memberRef, member.Type);
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
			
			IType type = TypeSystemServices.Map(typeof(System.Text.RegularExpressions.Regex));
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
			InternalField tag = new InternalField(TypeSystemServices, field);
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
			
			IEntity tag = ResolveName(node, node.Name);
			if (null != tag)
			{	
				EnsureRelatedNodeWasVisited(tag);
				
				IMember member = tag as IMember;
				if (null != member)
				{	
					ResolveMemberInfo(node, member);
				}
				else
				{
					node.Entity = tag;
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
					if (NodeType.ReferenceExpression == node.NodeType)
					{
						node.Name = tag.FullName;
					}
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
					if (NodeType.ReferenceExpression == node.NodeType &&
						!AstUtil.IsTargetOfMethodInvocation(node))
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
		
		virtual protected void ProcessMemberReferenceExpression(MemberReferenceExpression node)
		{				
			IEntity member = node.Entity;
			if (null == member)
			{
				INamespace ns = GetReferenceNamespace(node);				
				member = NameResolutionService.Resolve(ns, node.Name);
				if (null == member)
				{										
					Error(node, CompilerErrorFactory.MemberNotFound(node, ((IEntity)ns).FullName));
					return;
				}
			}
			
			EnsureRelatedNodeWasVisited(member);
			
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
				if (!AstUtil.IsLhsOfAssignment(node) &&
					!IsPreIncDec(node.ParentNode))
				{
					if (IsIndexedProperty(member))
					{
						if (!AstUtil.IsTargetOfSlicing(node))
						{
							Error(node, CompilerErrorFactory.PropertyRequiresParameters(GetMemberAnchor(node), member.FullName));
							return;
						}
					}
					else
					{
						node.ParentNode.Replace(node, CodeBuilder.CreateMethodInvocation(node.Target, ((IProperty)member).GetGetMethod()));
						return;
					}
				}
			}
			
			if (EntityType.Event == member.EntityType)
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
			CheckBoolContext(node.Condition);
		}
		
		override public void LeaveIfStatement(IfStatement node)
		{
			CheckBoolContext(node.Condition);			
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
			if (!InLoop())
			{
				Error(CompilerErrorFactory.NoEnclosingLoop(node));		
			}
		}
		
		override public void OnContinueStatement(ContinueStatement node)
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
			CheckBoolContext(node.Condition);
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
				_currentMethod.AddYieldExpression(node.Expression);
			
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
			bool runtimeIterator = false;
			
			IType iteratorType = GetExpressionType(iterator);
			if (!TypeSystemServices.IsError(iteratorType))
			{
				CheckIterator(iterator, iteratorType, out runtimeIterator);
			}			
			
			if (runtimeIterator)
			{
				if (IsTextReader(iteratorType))
				{					
					return CodeBuilder.CreateConstructorInvocation(TextReaderEnumerator_Constructor, iterator);
				}
				else
				{
					return CodeBuilder.CreateMethodInvocation(RuntimeServices_GetEnumerable, iterator);
				}
			}
			
			return iterator;
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
					IEntity tag = NameResolutionService.Resolve(d.Name, 
										EntityType.Local|EntityType.Parameter) as ILocalEntity;
					if (null != tag)
					{
						Bind(d, tag);
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
			
			node.Declaration.Entity = DeclareLocal(node.Declaration.Name, GetType(node.Declaration.Type), true);
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
		
		void OnIncrementDecrement(UnaryExpression node)
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
					node.Operand.ExpressionType = null;
					
					BinaryExpression addition = new BinaryExpression(
														node.Operator == UnaryOperatorType.Increment ?
																BinaryOperatorType.Addition : BinaryOperatorType.Subtraction,
														node.Operand.CloneNode(),
														new IntegerLiteralExpression(1));
														
					BinaryExpression assign = new BinaryExpression(node.LexicalInfo,
													BinaryOperatorType.Assign,
													node.Operand,
													addition);
													
					node.ParentNode.Replace(node, assign);
					Visit(assign);
				}
			}
			else
			{
				Error(node);
			}
		}
		
		override public void LeaveUnaryExpression(UnaryExpression node)
		{
			switch (node.Operator)
			{
				case UnaryOperatorType.LogicalNot:					
				{
					IEntity tag = TypeSystemServices.ErrorEntity;					
					if (CheckBoolContext(node.Operand))
					{
						tag = TypeSystemServices.BoolType;
					}
					BindExpressionType(node, TypeSystemServices.BoolType);
					break;
				}
				
				case UnaryOperatorType.Increment:
				{
					OnIncrementDecrement(node);
					break;
				}
				
				case UnaryOperatorType.Decrement:
				{
					OnIncrementDecrement(node);
					break;
				}
				
				case UnaryOperatorType.UnaryNegation:
				{
					if (!IsNumber(node.Operand))
					{
						InvalidOperatorForType(node);
					}
					else
					{
						BindExpressionType(node, GetExpressionType(node.Operand));
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
			if (node.Operator == BinaryOperatorType.Assign &&
			    NodeType.ReferenceExpression == node.Left.NodeType)
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
					CheckIsResolvedType(expressionType, node.Right);
					IEntity local = DeclareLocal(reference.Name, expressionType);
					reference.Entity = local;
					BindExpressionType(node.Left, expressionType);
					BindExpressionType(node, expressionType);
					return false;
				}
			}
			return true;
		}
		
		void CheckIsResolvedType(IType type, Expression expression)
		{
			if (TypeSystemServices.IsUnknown(type))
			{
				MethodInvocationExpression mie = expression as MethodInvocationExpression;
				if (null != mie)
				{
					InternalMethod entity = (InternalMethod)GetEntity(mie.Target);
					Error(CompilerErrorFactory.RecursiveMethodWithoutReturnType(entity.Method));
				}
			}
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
			
			switch (node.Operator)
			{		
				case BinaryOperatorType.Assign:
				{
					BindAssignment(node);
					break;
				}
				
				case BinaryOperatorType.Addition:
				{
					BindArithmeticOperator(node);
					break;
				}
				
				case BinaryOperatorType.Subtraction:
				{
					BindArithmeticOperator(node);
					break;
				}
				
				case BinaryOperatorType.Multiply:
				{
					BindArithmeticOperator(node);
					break;
				}
				
				case BinaryOperatorType.Division:
				{
					BindArithmeticOperator(node);
					break;
				}
				
				case BinaryOperatorType.Modulus:
				{
					BindArithmeticOperator(node);
					break;
				}
				
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
				
				case BinaryOperatorType.InPlaceMultiply:
				{
					BindInPlaceArithmeticOperator(node);
					break;
				}
				
				case BinaryOperatorType.InPlaceDivide:
				{
					BindInPlaceArithmeticOperator(node);
					break;
				}
				
				case BinaryOperatorType.Or:
				{
					BindLogicalOperator(node);
					break;
				}
				
				case BinaryOperatorType.And:
				{
					BindLogicalOperator(node);
					break;
				}
				
				case BinaryOperatorType.BitwiseOr:
				{
					BindBitwiseOperator(node);
					break;
				}
				
				case BinaryOperatorType.InPlaceSubtract:
				{
					BindInPlaceAddSubtract(node);
					break;
				}
				
				case BinaryOperatorType.InPlaceAdd:
				{
					BindInPlaceAddSubtract(node);
					break;
				}
				
				case BinaryOperatorType.GreaterThan:
				{
					BindCmpOperator(node);
					break;
				}
				
				case BinaryOperatorType.GreaterThanOrEqual:
				{
					BindCmpOperator(node);
					break;
				}
				
				case BinaryOperatorType.LessThan:
				{
					BindCmpOperator(node);
					break;
				}
				
				case BinaryOperatorType.LessThanOrEqual:
				{
					BindCmpOperator(node);
					break;
				}
				
				case BinaryOperatorType.Inequality:
				{
					BindCmpOperator(node);
					break;
				}
				
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
			
			if (TypeSystemServices.IsIntegerNumber(lhs) &&
				TypeSystemServices.IsIntegerNumber(rhs))
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
		
		void BindCmpOperator(BinaryExpression node)
		{
			IType lhs = GetExpressionType(node.Left);
			IType rhs = GetExpressionType(node.Right);
			
			if (IsNumber(lhs) && IsNumber(rhs))
			{
				BindExpressionType(node, TypeSystemServices.BoolType);
			}
			else if (lhs.IsEnum || rhs.IsEnum)
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
						Expression expression = CreateEquals(node);
						node.ParentNode.Replace(node, expression);
						break;
					}
					
					case BinaryOperatorType.Inequality:
					{
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
		
		void BindLogicalOperator(BinaryExpression node)
		{			
			if (CheckBoolContext(node.Left) &&
				CheckBoolContext(node.Right))
			{			
				BindExpressionType(node, GetMostGenericType(node));
			}
			else
			{				
				Error(node);
			}
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
			IMethod method = node.Operator == BinaryOperatorType.InPlaceAdd ? 
								eventInfo.GetAddMethod() :
								eventInfo.GetRemoveMethod();
								
			IType rtype = GetExpressionType(node.Right);
			CheckDelegateArgument(node, eventInfo, rtype);
			
			MethodInvocationExpression mie = CodeBuilder.CreateMethodInvocation(
												((MemberReferenceExpression)node.Left).Target,
												method,
												node.Right);
			
			node.ParentNode.Replace(node, mie);
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
			if (node.Arguments.Count != 1)
			{						
				Error(node, CompilerErrorFactory.MethodArgumentCount(node.Target, "len", 1));
			}
			else
			{
				MethodInvocationExpression resultingNode = null;
				
				Expression target = node.Arguments[0];
				IType type = GetExpressionType(target);
				if (TypeSystemServices.IsSystemObject(type))
				{
					resultingNode = CodeBuilder.CreateMethodInvocation(RuntimeServices_Len, target);
				}
				else if (TypeSystemServices.StringType == type)
				{
					resultingNode = CodeBuilder.CreateMethodInvocation(target, String_get_Length);
				}
				else if (TypeSystemServices.ArrayType.IsAssignableFrom(type))
				{
					resultingNode = CodeBuilder.CreateMethodInvocation(target, Array_get_Length);
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
					inferredType = TypeSystemServices.GetArrayType(type);
				}
			}
			else if (Array_EnumerableConstructor == method)
			{
				IType enumeratorItemType = GetEnumeratorItemType(GetExpressionType(expression.Arguments[0]));
				if (TypeSystemServices.ObjectType != enumeratorItemType)
				{
					inferredType = TypeSystemServices.GetArrayType(enumeratorItemType);
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
			
			IEntity targetInfo = node.Target.Entity;
			if (BuiltinFunction.Switch == targetInfo)
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
			
			if (null == targetInfo)
			{
				ProcessGenericMethodInvocation(node);
				return;
			}
			
			if (EntityType.Ambiguous == targetInfo.EntityType)
			{		
				targetInfo = ResolveAmbiguousMethodInvocation(node, (Ambiguous)targetInfo);
				if (null == targetInfo)
				{
					Error(node);
					return;
				}
			}	
			
			switch (targetInfo.EntityType)
			{	
				case EntityType.BuiltinFunction:
				{
					ProcessBuiltinInvocation((BuiltinFunction)targetInfo, node);
					break;
				}
				case EntityType.Event:
				{
					ProcessEventInvocation((IEvent)targetInfo, node);
					break;
				}
				
				case EntityType.Method:
				{				
					IMethod targetMethod = (IMethod)targetInfo;
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
					InternalConstructor constructorInfo = targetInfo as InternalConstructor;
					if (null != constructorInfo)
					{
						// super constructor call					
						constructorInfo.HasSuperCall = true;
						
						IType baseType = constructorInfo.DeclaringType.BaseType;
						IConstructor superConstructorInfo = FindCorrectConstructor(node, baseType, node.Arguments);
						if (null != superConstructorInfo)
						{
							Bind(node.Target, superConstructorInfo);
							BindExpressionType(node, superConstructorInfo.ReturnType);
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
							eval.Arguments.Add(CodeBuilder.CreateMethodInvocation(
											local.CloneNode(),
											setter,
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
			ResolveNamedArguments(node, type, node.NamedArguments);
			
			IConstructor ctor = FindCorrectConstructor(node, type, node.Arguments);
			if (null != ctor)
			{
				// rebind the target now we know
				// it is a constructor call
				Bind(node.Target, ctor);
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
				NotImplemented(node, "Method invocation on type '" + type + "'.");
			}
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
			
			if (!CheckTypeCompatibility(node.Right, sliceTargetType.GetElementType(), lhsType) ||
				!CheckTypeCompatibility(slice.Begin, TypeSystemServices.IntType, GetExpressionType(slice.Begin)))
			{
				Error(node);
				return;
			}
			
			Bind(node, sliceTargetType.GetElementType());
		}
		
		void BindAssignmentToSliceProperty(BinaryExpression node)
		{
			SlicingExpression slice = (SlicingExpression)node.Left;
			IEntity lhs = GetEntity(node.Left);
			IType rhs = GetExpressionType(node.Right);
			IMethod setter = null;

			MethodInvocationExpression mie = new MethodInvocationExpression(node.Left.LexicalInfo);
			mie.Arguments.Add(slice.Begin);
			mie.Arguments.Add(node.Right);			
			
			if (EntityType.Property == lhs.EntityType)
			{
				IMethod setMethod = ((IProperty)lhs).GetSetMethod();
				if (null == setMethod)
				{
					Error(node, CompilerErrorFactory.PropertyIsReadOnly(slice.Target, lhs.FullName));
					return;
				}
				 
				IParameter[] parameters = setMethod.GetParameters();
				if (2 != parameters.Length)
				{
					Error(node, CompilerErrorFactory.MethodArgumentCount(node.Left, setMethod.FullName, 2));
					return;
				}
				else
				{
					if (!CheckTypeCompatibility(slice.Begin, parameters[0].Type, GetExpressionType(slice.Begin)) ||
						!CheckTypeCompatibility(node.Right, parameters[1].Type, rhs))
					{					
						Error(node);
						return;
					}
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
				mie.Target = CodeBuilder.CreateMemberReference(slice.Target, setter);
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
							Error(CompilerErrorFactory.PropertyRequiresParameters(GetMemberAnchor(node.Left), property.FullName));
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
		
		bool IsDictionary(IType type)
		{
			return TypeSystemServices.IDictionaryType.IsAssignableFrom(type);
		}
		
		bool IsList(IType type)
		{
			return TypeSystemServices.IListType.IsAssignableFrom(type);
		}
		
		bool CanBeString(IType type)
		{
			return TypeSystemServices.IsSystemObject(type) ||
				TypeSystemServices.StringType == type;
		}
		
		void BindInPlaceArithmeticOperator(BinaryExpression node)
		{
			Node parent = node.ParentNode;
			
			// if target is a property force a rebinding
			Expression target = node.Left;
			if (EntityType.Property == target.Entity.EntityType)
			{
				target.Entity = null;
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
				case BinaryOperatorType.InPlaceAdd:
					return BinaryOperatorType.Addition;
					
				case BinaryOperatorType.InPlaceSubtract:
					return BinaryOperatorType.Subtraction;
					
				case BinaryOperatorType.InPlaceMultiply:
					return BinaryOperatorType.Multiply;
					
				case BinaryOperatorType.InPlaceDivide:
					return BinaryOperatorType.Division;
			}
			throw new ArgumentException("op");
		}
		
		void BindArithmeticOperator(BinaryExpression node)
		{
			IType left = GetExpressionType(node.Left);
			IType right = GetExpressionType(node.Right);
			if (IsNumber(left) && IsNumber(right))
			{
				BindExpressionType(node, TypeSystemServices.GetPromotedNumberType(left, right));
			}
			else if (!ResolveOperator(node))
			{
				InvalidOperatorForTypes(node);
			}
		}
		
		void Negate(BinaryExpression node, BinaryOperatorType newOperator)
		{
			Node parent = node.ParentNode;
			node.Operator = newOperator;
			parent.Replace(node, CodeBuilder.CreateNotExpression(node));
		}
		
		static string GetBinaryOperatorText(BinaryOperatorType op)
		{
			return Boo.Lang.Compiler.Ast.Visitors.BooPrinterVisitor.GetBinaryOperatorText(op);
		}
		static string GetUnaryOperatorText(UnaryOperatorType op)
		{
			return Boo.Lang.Compiler.Ast.Visitors.BooPrinterVisitor.GetUnaryOperatorText(op);
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
		
		void ResolveNamedArguments(Node sourceNode, IType typeInfo, ExpressionPairCollection arguments)
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
		
		bool CheckParameterTypes(IMethod method, ExpressionCollection args)
		{
			return CheckParameterTypes(method.CallableType, args);
		}
		
		bool CheckParameterTypes(ICallableType method, ExpressionCollection args)
		{
			IParameter[] parameters = method.GetSignature().Parameters;
			for (int i=0; i<args.Count; ++i)
			{
				Expression arg = args[i];
				IType expressionType = GetExpressionType(arg);
				IType parameterType = parameters[i].Type;
				if (parameterType.IsByRef)
				{
					if (!IsValidByRefArg(parameterType, expressionType, arg))
					{
						return false;
					}
				}
				else
				{
					if (!TypeSystemServices.AreTypesRelated(parameterType, expressionType))
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
		
		bool CheckParameters(Node sourceNode, IEntity sourceEntity, ICallableType method, ExpressionCollection args)
		{				
			if (method.GetSignature().Parameters.Length != args.Count)
			{
				Error(CompilerErrorFactory.MethodArgumentCount(sourceNode, sourceEntity.Name, args.Count));
				return false;
			}	
			
			if (!CheckParameterTypes(method, args))
			{
				Error(CompilerErrorFactory.MethodSignature(sourceNode, sourceEntity.ToString(), GetSignature(args)));
			}
			return true;
		}
		
		
		bool CheckCallableSignature(ICallableType expected, ICallableType actual)
		{
			return expected.GetSignature() == actual.GetSignature();			
		}
		
		bool IsRuntimeIterator(IType type)
		{
			return  TypeSystemServices.IsSystemObject(type) ||
					IsTextReader(type);					
		}
		
		bool IsTextReader(IType type)
		{
			return IsAssignableFrom(typeof(System.IO.TextReader), type);
		}
		
		void CheckIterator(Expression iterator, IType type, out bool runtimeIterator)
		{	
			runtimeIterator = false;
			
			if (type.IsArray)
			{				
				if (((IArrayType)type).GetArrayRank() != 1)
				{
					Error(CompilerErrorFactory.InvalidArray(iterator));
				}
			}
			else
			{
				IType enumerable = TypeSystemServices.IEnumerableType;
				if (!enumerable.IsAssignableFrom(type))
				{
					runtimeIterator = IsRuntimeIterator(type);
					if (!runtimeIterator)
					{
						Error(CompilerErrorFactory.InvalidIteratorType(iterator, type.FullName));
					}
				}
			}
		}		
		
		bool CheckTargetContext(Expression targetContext, IMember member)
		{
			if (!member.IsStatic)					  
			{			
				if (NodeType.MemberReferenceExpression == targetContext.NodeType)
				{				
					Expression targetReference = ((MemberReferenceExpression)targetContext).Target;										
					IEntity entity = targetReference.Entity;
					if (null != entity && EntityType.Type == entity.EntityType)
					{						
						Error(CompilerErrorFactory.MemberNeedsInstance(targetContext, member.FullName));
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
		
		bool IsNumber(Expression expression)
		{
			return IsNumber(GetExpressionType(expression));
		}
		
		bool IsString(Expression expression)
		{
			return TypeSystemServices.StringType == GetExpressionType(expression);
		}
		
		IConstructor FindCorrectConstructor(Node sourceNode, IType typeInfo, ExpressionCollection arguments)
		{
			IConstructor[] constructors = typeInfo.GetConstructors();
			if (constructors.Length > 0)
			{		
				EnsureRelatedNodesWereVisited(constructors);
				return (IConstructor)ResolveCallableReference(sourceNode, arguments, constructors, true);				
			}
			else
			{
				Error(CompilerErrorFactory.NoApropriateConstructorFound(sourceNode, typeInfo.FullName, GetSignature(arguments)));
			}
			return null;
		}
		
		class CallableScore : IComparable
		{
			public IMethod Info;
			public int Score;
			
			public CallableScore(IMethod tag, int score)
			{
				Info = tag;
				Score = score;
			}
			
			public int CompareTo(object other)
			{
				return ((CallableScore)other).Score-Score;
			}
			
			override public int GetHashCode()
			{
				return Info.GetHashCode();
			}
			
			override public bool Equals(object other)
			{
				CallableScore score = other as CallableScore;
				if (null == score)
				{
					return false;
				}
				return object.Equals(Info, score.Info);
			}
			
			override public string ToString()
			{
				return Info.ToString();
			}
		}
		
		void EnsureRelatedNodesWereVisited(IEntity[] entities)
		{
			foreach (IEntity entity in entities)
			{
				EnsureRelatedNodeWasVisited(entity);
			}
		}
		
		void EnsureRelatedNodeWasVisited(IEntity tag)
		{
			if (tag.EntityType == EntityType.Ambiguous)
			{
				EnsureRelatedNodesWereVisited(((Ambiguous)tag).Entities);
				return;
			}
			
			IInternalEntity internalInfo = tag as IInternalEntity;
			if (null != internalInfo)
			{
				if (!Visited(internalInfo.Node))
				{
					_context.TraceVerbose("Info {0} needs resolving.", tag.Name);
					
					INamespace saved = NameResolutionService.CurrentNamespace;
					try
					{
						TypeMember member = internalInfo.Node as TypeMember;
						if (null != member)
						{
							Visit(member.ParentNode);
						}
						Visit(internalInfo.Node);
					}
					finally
					{
						NameResolutionService.Restore(saved);
					}
				}
			}
		}
		
		CallableScore GetBiggerScore(List scores)
		{
			scores.Sort();
			CallableScore first = (CallableScore)scores[0];
			CallableScore second = (CallableScore)scores[1];
			if (first.Score > second.Score)
			{
				return first;
			}
			return null;
		}
		
		void ReScoreByHierarchyDepth(List scores)
		{
			foreach (CallableScore score in scores)
			{
				score.Score += score.Info.DeclaringType.GetTypeDepth();
				
				IParameter[] parameters = score.Info.GetParameters();
				for (int i=0; i<parameters.Length; ++i)
				{
					score.Score += parameters[i].Type.GetTypeDepth();
				}
			}			
		}
		
		IType GetExpressionTypeOrEntityType(Node node)
		{
			Expression e = node as Expression;
			if (null != e)
			{
				return GetExpressionType(e);
			}
			return GetType(node);
		}
		
		bool IsValidByRefArg(IType parameterType, IType argType, Node arg)
		{
			return parameterType.IsByRef && 
					(argType == parameterType.GetElementType()) &&
					CanLoadAddress(arg);
		}
		
		bool CanLoadAddress(Node node)
		{
			IEntity entity = node.Entity;
			if (null != entity)
			{
				switch (entity.EntityType)
				{
					case EntityType.Local:
					{
						return !((InternalLocal)entity).IsPrivateScope;
					}
					
					case EntityType.Parameter:
					{
						return true;
					}
					
					case EntityType.Field:
					{
						return !IsReadOnlyField((IField)entity);
					}
				}
			}
			return false;
		}
		
		IEntity ResolveCallableReference(Node node, NodeCollection args, IEntity[] tags, bool treatErrors)
		{
			List scores = new List();
			for (int i=0; i<tags.Length; ++i)
			{				
				IEntity tag = tags[i];
				IMethod mb = tag as IMethod;
				if (null != mb)
				{	
					IParameter[] parameters = mb.GetParameters();
					if (args.Count == parameters.Length)
					{
						int score = 0;
						for (int argIndex=0; argIndex<parameters.Length; ++argIndex)
						{							 
							Node arg = args.GetNodeAt(argIndex);
							IType expressionType = GetExpressionTypeOrEntityType(arg);
							IType parameterType = parameters[argIndex].Type;						
							
							if (parameterType == expressionType)
							{
								// exact match scores 3
								score += 3;
							}
							else if (IsAssignableFrom(parameterType, expressionType))
							{
								// upcast scores 2
								score += 2;
							}
							else if (
								TypeSystemServices.CanBeReachedByDownCastOrPromotion(parameterType, expressionType) ||
								IsValidByRefArg(parameterType, expressionType, arg))
							{
								// downcast scores 1
								score += 1;
							}
							else
							{
								score = -1;
								break;
							}
						}						
						
						if (score >= 0)
						{
							// only positive scores are compatible
							scores.Add(new CallableScore(mb, score));						
						}
					}
				}
			}		
			
			if (1 == scores.Count)
			{
				return ((CallableScore)scores[0]).Info;
			}
			
			if (scores.Count > 1)
			{
				CallableScore score = GetBiggerScore(scores);
				if (null != score)
				{
					return score.Info;
				}
				
				ReScoreByHierarchyDepth(scores);
				score = GetBiggerScore(scores);
				if (null != score)
				{
					return score.Info;					
				}
				
				if (treatErrors)
				{
					Error(CompilerErrorFactory.AmbiguousReference(node, tags[0].Name, scores));
				}
			}
			else
			{	
				if (treatErrors)
				{
					IEntity tag = tags[0];
					IConstructor constructor = tag as IConstructor;
					if (null != constructor)
					{
						Error(CompilerErrorFactory.NoApropriateConstructorFound(node, constructor.DeclaringType.FullName, GetSignature(args)));
					}
					else
					{
						Error(CompilerErrorFactory.NoApropriateOverloadFound(node, GetSignature(args), tag.FullName));
					}
				}
			}
			return null;
		}
		
		string GetMethodNameForOperator(BinaryOperatorType op)
		{
			return "op_" + op.ToString();
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
		
		IMethod ResolveAmbiguousOperator(IEntity[] tags, ExpressionCollection args)
		{
			foreach (IEntity tag in tags)
			{
				IMethod method = tag as IMethod;
				if (null != method)
				{
					if (method.IsStatic)
					{
						if (2 == method.GetParameters().Length)
						{
							if (CheckParameterTypesStrictly(method, args))
							{
								return method;
							}
						}
					}
				}
			}
			return null;
		}
		
		bool ResolveOperator(BinaryExpression node, IType type, string operatorName, MethodInvocationExpression mie)
		{
			IEntity tag = NameResolutionService.Resolve(type, operatorName, EntityType.Method);
			if (null == tag)
			{
				return false;
			}
			
			if (EntityType.Ambiguous == tag.EntityType)
			{	
				tag = ResolveAmbiguousOperator(((Ambiguous)tag).Entities, mie.Arguments);
				if (null == tag)
				{
					return false;
				}
			}
			else if (EntityType.Method == tag.EntityType)
			{					
				IMethod method = (IMethod)tag;
				
				if (!method.IsStatic || !CheckParameterTypesStrictly(method, mie.Arguments))
				{
					return false;
				}
			}
			else
			{
				return false;
			}
			
			mie.Target = new ReferenceExpression(tag.FullName);
			
			IMethod operatorMethod = (IMethod)tag;
			BindExpressionType(mie, operatorMethod.ReturnType);
			BindExpressionType(mie.Target, operatorMethod.Type);
			Bind(mie.Target, tag);
			
			node.ParentNode.Replace(node, mie);
			
			return true;
		}
		
		Node GetMemberAnchor(Node node)
		{
			MemberReferenceExpression member = node as MemberReferenceExpression;
			return member != null ? member.Target : node;
		}
		
		protected virtual bool CheckLValue(Node node)
		{
			IEntity tag = node.Entity;
			
			if (null != tag)
			{
				switch (tag.EntityType)
				{
					case EntityType.Parameter:
					{
						return true;
					}
					
					case EntityType.Local:
					{
						return !((InternalLocal)tag).IsPrivateScope;
					}
					
					case EntityType.Property:
					{
						if (null == ((IProperty)tag).GetSetMethod())
						{
							Error(CompilerErrorFactory.PropertyIsReadOnly(GetMemberAnchor(node), tag.FullName));
							return false;
						}
						return true;
					}
					
					case EntityType.Field:
					{
						return !IsReadOnlyField((IField)tag);
					}
				}
			}
			
			Error(CompilerErrorFactory.LValueExpected(node));
			return false;
		}
		
		bool IsReadOnlyField(IField field)
		{
			return field.IsInitOnly || field.IsLiteral;
		}
		
		bool CheckBoolContext(Expression expression)
		{
			IType type = GetExpressionType(expression);
			if (type.IsValueType)
			{
				if (type == TypeSystemServices.BoolType ||
				    IsNumber(type))
			    {
			    	return true;
			    }
			    Error(CompilerErrorFactory.BoolExpressionRequired(expression, type.FullName));
				return false;
			}
			// reference types can be used in bool context
			return true;
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
		
		IEntity DeclareLocal(string name, IType localType)
		{
			return DeclareLocal(name, localType, false);
		}
		
		virtual protected IEntity DeclareLocal(string name, IType localType, bool privateScope)
		{			
			Local local = new Local(name, privateScope);
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
				IsAssignment(node) ||
				IsPreIncDec(node);
		}
		
		static bool IsPreIncDec(Node node)
		{
			if (node.NodeType == NodeType.UnaryExpression)
			{
				UnaryOperatorType op = ((UnaryExpression)node).Operator;
				return UnaryOperatorType.Increment == op ||
					UnaryOperatorType.Decrement == op;
			}
			return false;
		}
		
		static bool IsAssignment(Expression node)
		{
			if (node.NodeType == NodeType.BinaryExpression)
			{
				BinaryOperatorType binaryOperator = ((BinaryExpression)node).Operator;
				return BinaryOperatorType.Assign == binaryOperator ||
						BinaryOperatorType.InPlaceAdd == binaryOperator ||						
						BinaryOperatorType.InPlaceSubtract == binaryOperator;
			}
			return false;
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
			if (IsAbstract(type))
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
				d.Entity = DeclareLocal(d.Name, GetType(d.Type), privateScope);
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
				sb.Append(GetExpressionType(arg));
			}
			sb.Append(")");
			return sb.ToString();
		}
		
		string GetSignature(IMethod tag)
		{
			return tag.ToString();
		}
		
		bool Visited(Node node)
		{
			return _visited.ContainsKey(node);
		}
		
		void MarkVisited(Node node)
		{			
			_context.TraceInfo("{0}: node '{1}' mark visited.", node.LexicalInfo, node);
			_visited.Add(node, null);
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
		
		void TraceOverride(Method method, IMethod baseMethod)
		{
			_context.TraceInfo("{0}: Method '{1}' overrides '{2}'", method.LexicalInfo, method.Name, baseMethod);
		}
		
		void TraceReturnType(Method method, IMethod tag)
		{
			_context.TraceInfo("{0}: return type for method {1} bound to {2}", method.LexicalInfo, method.Name, tag.ReturnType);
		}

		#region Abstract Member Processing
		void ProcessInheritedAbstractMembers(ClassDefinition node)
		{
			foreach (TypeReference baseTypeRef in node.BaseTypes)
			{
				IType baseType = GetType(baseTypeRef);
				if (baseType.IsInterface)
				{
					ResolveInterfaceMembers(node, baseTypeRef, baseType);
				}
				else
				{					
					if (IsAbstract(baseType))
					{
						ResolveAbstractMembers(node, baseTypeRef, baseType);
					}
				}
			}
		}
		
		bool IsAbstract(IType type)
		{
			if (type.IsAbstract)
			{
				return true;
			}
			
			AbstractInternalType internalType = type as AbstractInternalType;
			if (null != internalType)
			{
				return _newAbstractClasses.Contains(internalType.TypeDefinition);
			}
			return false;
		}
		
		void ResolveClassAbstractProperty(ClassDefinition node,
											Boo.Lang.Compiler.Ast.TypeReference baseTypeRef,
											IProperty tag)
		{
			TypeMember member = node.Members[tag.Name];
			if (null != member && NodeType.Property == member.NodeType)
			{
				if (tag.Type == GetType(member))
				{
					if (CheckPropertyAccessors(tag, (IProperty)GetEntity(member)))
					{
						Property p = (Property)member;
						if (null != p.Getter)
						{
							p.Getter.Modifiers |= TypeMemberModifiers.Virtual;
						}
						if (null != p.Setter)
						{
							p.Setter.Modifiers |= TypeMemberModifiers.Virtual;
						}
					}
				}
			}
			else
			{
				if (null == member)
				{
					node.Members.Add(CreateAbstractProperty(baseTypeRef, tag));
					AbstractMemberNotImplemented(node, baseTypeRef, tag);
				}
				else
				{
					NotImplemented(baseTypeRef, "member name conflict");
				}
			}
		}
		
		Property CreateAbstractProperty(TypeReference reference, IProperty property)
		{
			System.Diagnostics.Debug.Assert(0 == property.GetParameters().Length);
			Property p = CodeBuilder.CreateProperty(property.Name, property.Type);
			p.Modifiers |= TypeMemberModifiers.Abstract;
			
			IMethod getter = property.GetGetMethod();
			if (getter != null)
			{
				p.Getter = CodeBuilder.CreateAbstractMethod(reference.LexicalInfo, getter); 
			}
			
			IMethod setter = property.GetSetMethod(); 
			if (setter != null)
			{
				p.Setter = CodeBuilder.CreateAbstractMethod(reference.LexicalInfo, setter);				
			}
			return p;
		}
		
		void ResolveAbstractMethod(ClassDefinition node,
										TypeReference baseTypeRef,
										IMethod tag)
		{			
			if (tag.IsSpecialName)
			{
				return;
			}
			
			foreach (TypeMember member in node.Members)
			{
				if (tag.Name == member.Name && NodeType.Method == member.NodeType)
				{							
					Method method = (Method)member;
					if (TypeSystemServices.CheckOverrideSignature((IMethod)GetEntity(method), tag))
					{
						// TODO: check return type here
						if (!method.IsOverride && !method.IsVirtual)
						{
							method.Modifiers |= TypeMemberModifiers.Virtual;
						}
						
						_context.TraceInfo("{0}: Method {1} implements {2}", method.LexicalInfo, method, tag);
						return;
					}
				}
			}
			
			node.Members.Add(CodeBuilder.CreateAbstractMethod(baseTypeRef.LexicalInfo, tag));
			AbstractMemberNotImplemented(node, baseTypeRef, tag); 			
		}
		
		void AbstractMemberNotImplemented(ClassDefinition node, TypeReference baseTypeRef, IMember member)
		{
			if (!node.IsAbstract)
			{
				Warnings.Add(
						CompilerWarningFactory.AbstractMemberNotImplemented(baseTypeRef,
																					node.FullName, member.FullName));
				_newAbstractClasses.AddUnique(node);
			}
		}
		
		void ResolveInterfaceMembers(ClassDefinition node,
											TypeReference baseTypeRef,
											IType baseType)
		{			
			foreach (IType tag in baseType.GetInterfaces())
			{
				ResolveInterfaceMembers(node, baseTypeRef, tag);
			}
			
			foreach (IMember tag in baseType.GetMembers())
			{
				ResolveAbstractMember(node, baseTypeRef, tag);
			}
		}		
		
		void ResolveAbstractMembers(ClassDefinition node,
											TypeReference baseTypeRef,
											IType baseType)
		{
			foreach (IMember member in baseType.GetMembers())
			{
				switch (member.EntityType)
				{
					case EntityType.Method:
					{
						IMethod method = (IMethod)member;
						if (method.IsAbstract)
						{
							ResolveAbstractMethod(node, baseTypeRef, method);
						}
						break;
					}
					
					case EntityType.Property:
					{
						IProperty property = (IProperty)member;
						if (IsAbstractAccessor(property.GetGetMethod()) ||
							IsAbstractAccessor(property.GetSetMethod()))
						{
							ResolveClassAbstractProperty(node, baseTypeRef, property);
						}
						break;
					}
				}
			}
		}									
		
		bool IsAbstractAccessor(IMethod accessor)
		{
			if (null != accessor)
			{
				return accessor.IsAbstract;
			}
			return false;
		}
		
		void ResolveAbstractMember(ClassDefinition node,
											TypeReference baseTypeRef,
											IMember member)
		{
			switch (member.EntityType)
			{
				case EntityType.Method:
				{
					ResolveAbstractMethod(node, baseTypeRef, (IMethod)member);
					break;
				}
				
				case EntityType.Property:
				{
					ResolveClassAbstractProperty(node, baseTypeRef, (IProperty)member);
					break;
				}
				
				default:
				{
					NotImplemented(baseTypeRef, "abstract member: " + member);
					break;
				}
			}
		}
		
		bool CheckPropertyAccessors(IProperty expected, IProperty actual)
		{			
			return CheckPropertyAccessor(expected.GetGetMethod(), actual.GetGetMethod()) &&
				CheckPropertyAccessor(expected.GetSetMethod(), actual.GetSetMethod());
		}
		
		bool CheckPropertyAccessor(IMethod expected, IMethod actual)
		{			
			if (null != expected)
			{								
				if (null == actual ||
					!TypeSystemServices.CheckOverrideSignature(expected, actual))
				{
					return false;
				}
			}
			return true;
		}
		
		void ProcessNewAbstractClasses()
		{
			foreach (ClassDefinition node in _newAbstractClasses)
			{
				node.Modifiers |= TypeMemberModifiers.Abstract;
			}
		}
		#endregion		
	}
}
