#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Contact Information
//
// mailto:rbo@acm.org
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
		Stack _methodInfoStack;
		
		InternalMethod _currentMethodInfo;
		
		Hash _visited;
		
		int _loopDepth;
		
		/*
		 * Useful method tags.
		 */		
		IMethod RuntimeServices_Len;
		
		IMethod RuntimeServices_Mid;
		
		IMethod RuntimeServices_GetRange;
		
		IMethod RuntimeServices_GetEnumerable;
		
		IMethod Object_StaticEquals;
		
		IMethod Array_get_Length;
		
		IMethod String_get_Length;
		
		IMethod String_Substring_Int;
		
		IMethod ICollection_get_Count;
		
		IMethod IList_Contains;
		
		IMethod IDictionary_Contains;
		
		IMethod Array_TypedEnumerableConstructor;
		
		IMethod Array_TypedCollectionConstructor;
		
		IMethod Array_TypedConstructor2;
		
		IMethod ICallable_Call;
		
		IMethod Activator_CreateInstance;
		
		IConstructor ApplicationException_StringConstructor;
		
		IConstructor TextReaderEnumerator_Constructor;
		
		/*
		 * Useful filters.
		 */
		InfoFilter IsPublicEventFilter;
		
		InfoFilter IsPublicFieldPropertyEventFilter;
		
		Boo.Lang.List _buffer = new Boo.Lang.List();
		
		public ProcessMethodBodies()
		{
			IsPublicFieldPropertyEventFilter = new InfoFilter(IsPublicFieldPropertyEvent);
			IsPublicEventFilter = new InfoFilter(IsPublicEvent);
		}
		
		IMethod ResolveMethod(IType type, string name)
		{
			return (IMethod)ResolveMember(type, name, EntityType.Method);
		}
		
		IProperty ResolveProperty(IType type, string name)
		{
			return (IProperty)ResolveMember(type, name, EntityType.Property);
		}
		
		IEntity ResolveMember(IType type, string name, EntityType elementType)
		{
			_buffer.Clear();
			type.Resolve(_buffer, name, elementType);
			System.Diagnostics.Debug.Assert(1 == _buffer.Count);
			return (IEntity)_buffer[0];
		}
		
		IEntity Resolve(INamespace ns, string name, EntityType elementType)
		{
			_buffer.Clear();
			ns.Resolve(_buffer, name, elementType);
			return NameResolutionService.GetEntityFromList(_buffer);
		}
		
		IEntity Resolve(INamespace ns, string name)
		{
			return Resolve(ns, name, EntityType.Any);
		}
		
		override public void Run()
		{	
			if (Errors.Count > 0)
			{
				return;
			}
			
			NameResolutionService.Reset();
			
			_currentMethodInfo = null;
			_methodInfoStack = new Stack();
			_visited = new Hash();
			_loopDepth = 0;
						
			RuntimeServices_Len = ResolveMethod(TypeSystemServices.RuntimeServicesType, "Len");
			RuntimeServices_Mid = ResolveMethod(TypeSystemServices.RuntimeServicesType, "Mid");
			RuntimeServices_GetRange = ResolveMethod(TypeSystemServices.RuntimeServicesType, "GetRange");
			RuntimeServices_GetEnumerable = ResolveMethod(TypeSystemServices.RuntimeServicesType, "GetEnumerable");			
			Object_StaticEquals = (IMethod)TypeSystemServices.Map(Types.Object.GetMethod("Equals", new Type[] { Types.Object, Types.Object }));
			Array_get_Length = ResolveProperty(TypeSystemServices.ArrayType, "Length").GetGetMethod();
			String_get_Length = ResolveProperty(TypeSystemServices.StringType, "Length").GetGetMethod();
			String_Substring_Int = (IMethod)TypeSystemServices.Map(Types.String.GetMethod("Substring", new Type[] { Types.Int }));
			ICollection_get_Count = ResolveProperty(TypeSystemServices.ICollectionType, "Count").GetGetMethod();
			IList_Contains = ResolveMethod(TypeSystemServices.IListType, "Contains");
			IDictionary_Contains = ResolveMethod(TypeSystemServices.IDictionaryType, "Contains");
			Array_TypedEnumerableConstructor = (IMethod)TypeSystemServices.Map(Types.Builtins.GetMethod("array", new Type[] { Types.Type, Types.IEnumerable }));
			Array_TypedCollectionConstructor= (IMethod)TypeSystemServices.Map(Types.Builtins.GetMethod("array", new Type[] { Types.Type, Types.ICollection }));
			Array_TypedConstructor2 = (IMethod)TypeSystemServices.Map(Types.Builtins.GetMethod("array", new Type[] { Types.Type, Types.Int }));
			ICallable_Call = ResolveMethod(TypeSystemServices.ICallableType, "Call");
			Activator_CreateInstance = (IMethod)TypeSystemServices.Map(typeof(Activator).GetMethod("CreateInstance", new Type[] { Types.Type, Types.ObjectArray }));
			TextReaderEnumerator_Constructor = (IConstructor)TypeSystemServices.Map(typeof(Boo.IO.TextReaderEnumerator).GetConstructor(new Type[] { typeof(System.IO.TextReader) }));
			
			ApplicationException_StringConstructor =
					(IConstructor)TypeSystemServices.Map(
						Types.ApplicationException.GetConstructor(new Type[] { typeof(string) }));
			
			Visit(CompileUnit);
		}
		
		override public void Dispose()
		{
			base.Dispose();
			
			_currentMethodInfo = null;
			_methodInfoStack = null;
			_visited = null;
		}
		
		void EnterLoop()
		{
			++_loopDepth;
		}
		
		bool InLoop()
		{
			return _loopDepth > 0;
		}
		
		void LeaveLoop()
		{
			--_loopDepth;
		}
		
		override public void OnModule(Boo.Lang.Compiler.Ast.Module module)
		{				
			EnterNamespace((INamespace)TypeSystemServices.GetEntity(module));			
			
			Visit(module.Members);
			
			LeaveNamespace();
		}
		
		override public void OnInterfaceDefinition(InterfaceDefinition node)
		{
			if (Visited(node))
			{
				return;
			}			
			MarkVisited(node);
			
			InternalType tag = GetInternalType(node);
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
			
			InternalType tag = GetInternalType(node);			
			EnterNamespace(tag);
			Visit(node.Attributes);		
			ProcessFields(node);
			Visit(node.Members);
			LeaveNamespace();
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
		
		override public void OnProperty(Property node)
		{
			if (Visited(node))
			{
				return;
			}			
			MarkVisited(node);
			
			InternalProperty tag = (InternalProperty)GetEntity(node);
			
			Method setter = node.Setter;
			Method getter = node.Getter;
			
			Visit(node.Attributes);			
			Visit(node.Type);
			
			Visit(node.Parameters);
			if (null != getter)
			{
				if (null != node.Type)
				{
					getter.ReturnType = node.Type.CloneNode();
				}
				getter.Name = "get_" + node.Name;
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
				}
				else
				{
					typeInfo = TypeSystemServices.ObjectType;
				}
				node.Type = CreateTypeReference(typeInfo);
			}
			
			if (null != setter)
			{
				ParameterDeclaration parameter = new ParameterDeclaration();
				parameter.Type = CreateTypeReference(typeInfo);
				parameter.Name = "value";
				parameter.Entity = new InternalParameter(parameter, node.Parameters.Count+GetFirstParameterIndex(setter));
				setter.Parameters.ExtendWithClones(node.Parameters);
				setter.Parameters.Add(parameter);
				Visit(setter);
				
				setter.Name = "set_" + node.Name;
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
			
			ProcessFieldInitializer(node);			
			
			if (null == node.Type)
			{
				if (null == node.Initializer)
				{
					node.Type = CreateTypeReference(TypeSystemServices.ObjectType);
				}
				else
				{
					node.Type = CreateTypeReference(GetExpressionType(node.Initializer));
				}
			}
			else
			{
				Visit(node.Type);
				
				if (null != node.Initializer)
				{
					CheckTypeCompatibility(node.Initializer, GetType(node.Type), GetExpressionType(node.Initializer));
				}
			}
		}
		
		void ProcessFields(ClassDefinition node)
		{
			Node[] fields = node.Members.Select(NodeType.Field);
			if (0 == fields.Length)
			{
				return;
			}
			
			Visit(fields);
			
			int staticFieldIndex = 0;
			int instanceFieldIndex = 0;
			
			foreach (Field f in fields)
			{
				if (null == f.Initializer)
				{
					continue;
				}
				
				if (f.IsStatic)
				{
					AddFieldInitializerToStaticConstructor(staticFieldIndex, f);
					++staticFieldIndex;
				}
				else
				{
					AddFieldInitializerToInstanceConstructors(instanceFieldIndex, f);
					++instanceFieldIndex;
				}
			}
		}		
		
		void ProcessFieldInitializer(Field node)
		{
			Expression initializer = node.Initializer;
			if (null != initializer)
			{
				Method method = new Method();
				method.LexicalInfo = node.LexicalInfo;
				method.Name = "__initializer__";
				method.Modifiers = node.Modifiers;
				
				BinaryExpression assignment = new BinaryExpression(
						node.LexicalInfo,
						BinaryOperatorType.Assign,
						new ReferenceExpression("__temp__"),
						initializer);
						
				method.Body.Add(assignment);
				
				TypeDefinition type = node.DeclaringType;
				try
				{
					type.Members.Add(method);
					method.Entity = new InternalMethod(TypeSystemServices, method);
					Visit(method);
				}
				finally
				{
					node.Initializer = assignment.Right;
					type.Members.Remove(method);
				}
			}
		}
		
		Constructor GetStaticConstructor(TypeDefinition type)
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
		
		void AddFieldInitializerToStaticConstructor(int index, Field node)
		{
			Constructor constructor = GetStaticConstructor(node.DeclaringType);
			if (null == constructor)
			{
				constructor = new Constructor(node.LexicalInfo);
				constructor.Modifiers = TypeMemberModifiers.Public|TypeMemberModifiers.Static;
				node.DeclaringType.Members.Add(constructor);				
				Bind(constructor, new InternalConstructor(TypeSystemServices, constructor));
				MarkVisited(constructor);
			}
			
			Statement stmt = CreateFieldAssignment(node);
			constructor.Body.Statements.Insert(index, stmt);
			node.Initializer = null;
		}
		
		void AddFieldInitializerToInstanceConstructors(int index, Field node)
		{
			foreach (TypeMember member in node.DeclaringType.Members)
			{
				if (NodeType.Constructor == member.NodeType)
				{
					Constructor constructor = (Constructor)member;
					if (!constructor.IsStatic)
					{
						Statement stmt = CreateFieldAssignment(node);
						constructor.Body.Statements.Insert(index, stmt);
					}
				}
			}
			node.Initializer = null;
		}
		
		Statement CreateFieldAssignment(Field node)
		{
			InternalField fieldEntity = (InternalField)GetEntity(node);
			
			ExpressionStatement stmt = new ExpressionStatement(node.Initializer.LexicalInfo);
			
			Expression context = null;
			if (node.IsStatic)
			{
				context = new ReferenceExpression(node.DeclaringType.Name);				
			}
			else
			{
				context = new SelfLiteralExpression();
			}			
			
			BindExpressionType(context, fieldEntity.DeclaringType);
			
			MemberReferenceExpression member = new MemberReferenceExpression(context, node.Name);
			Bind(member, fieldEntity);
			BindExpressionType(member, fieldEntity.Type);
			
			// <node.Name> = <node.Initializer>
			stmt.Expression = new BinaryExpression(BinaryOperatorType.Assign,
									member,
									node.Initializer);
			BindExpressionType(stmt.Expression, fieldEntity.Type);
			
			return stmt;
		}
		
		Statement CreateDefaultConstructorCall(Constructor node, InternalConstructor tag)
		{			
			IConstructor defaultConstructor = GetDefaultConstructor(tag.DeclaringType.BaseType);
			
			MethodInvocationExpression call = new MethodInvocationExpression(new SuperLiteralExpression());			
			
			Bind(call.Target, defaultConstructor);
			BindExpressionType(call, TypeSystemServices.VoidType);
			
			return new ExpressionStatement(call);
		}
		
		override public bool EnterConstructor(Constructor node)
		{			
			if (Visited(node))
			{
				return false;
			}			
			MarkVisited(node);
			
			InternalConstructor tag = (InternalConstructor)GetEntity(node);
			PushMethodInfo(tag);
			EnterNamespace(tag);
			return true;
		}
		
		void CheckRuntimeMethod(Method method)
		{
			if (method.Body.Statements.Count > 0)
			{
				Error(CompilerErrorFactory.RuntimeMethodBodyMustBeEmpty(method, method.FullName));
			}
		}
		
		override public void LeaveConstructor(Constructor node)
		{
			if (node.IsRuntime)
			{
				CheckRuntimeMethod(node);
			}
			else
			{
				InternalConstructor tag = (InternalConstructor)_currentMethodInfo;
				if (!tag.HasSuperCall && !node.IsStatic)
				{
					node.Body.Statements.Insert(0, CreateDefaultConstructorCall(node, tag));
				}
			}
			LeaveNamespace();
			PopMethodInfo();
		}
		
		override public void LeaveParameterDeclaration(ParameterDeclaration node)
		{
			CheckIdentifierName(node, node.Name);
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
			
			PushMethodInfo(tag);
			EnterNamespace(tag);
			
			Visit(method.Body);
			
			LeaveNamespace();
			PopMethodInfo();			
			
			if (parentIsClass)
			{
				if (TypeSystemServices.IsUnknown(tag.ReturnType))
				{
					if (CanResolveReturnType(tag))
					{
						ResolveReturnType(tag);
						CheckMethodOverride(tag);
					}
					else
					{
						Error(CompilerErrorFactory.RecursiveMethodWithoutReturnType(method));
					}
				}
				else
				{
					if (!method.IsOverride)
					{
						CheckMethodOverride(tag);
					}
				}
			}
		}
		
		override public void OnSuperLiteralExpression(SuperLiteralExpression node)
		{			
			Bind(node, _currentMethodInfo);
			if (EntityType.Constructor != _currentMethodInfo.EntityType)
			{
				_currentMethodInfo.SuperExpressions.Add(node);
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
				foreach (Expression super in tag.SuperExpressions)
				{
					Error(CompilerErrorFactory.MethodIsNotOverride(super, GetSignature(tag)));
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
		
		IMethod FindMethodOverride(InternalMethod tag)
		{
			IType baseType = tag.DeclaringType.BaseType;			
			Method method = tag.Method;			
			IEntity baseMethods = Resolve(baseType, tag.Name, EntityType.Method);
			
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
						tag.Method.ReturnType = CreateTypeReference(baseMethod.ReturnType);
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
			foreach (Expression rsExpression in tag.ReturnExpressions)
			{
				IType type = rsExpression.ExpressionType;
				if (null == type || TypeSystemServices.IsUnknown(type))
				{
					return false;
				}
			}
			return true;
		}
		
		void ResolveReturnType(InternalMethod tag)
		{				
			Method method = tag.Method;
			ExpressionCollection returnExpressions = tag.ReturnExpressions;
			if (0 == returnExpressions.Count)
			{					
				method.ReturnType = CreateTypeReference(TypeSystemServices.VoidType);
			}		
			else
			{					
				IType type = GetMostGenericType(returnExpressions);
				if (Null.Default == type)
				{
					type = TypeSystemServices.ObjectType; 
				}
				method.ReturnType = CreateTypeReference(type);
			}
			TraceReturnType(method, tag);	
		}
		
		IType GetMostGenericType(IType current, IType candidate)
		{
			if (IsAssignableFrom(current, candidate))
			{
				return current;
			}
			
			if (IsAssignableFrom(candidate, current))
			{
				return candidate;
			}
			
			if (IsNumber(current) && IsNumber(candidate))
			{
				return TypeSystemServices.GetPromotedNumberType(current, candidate);
			}
			
			IType obj = TypeSystemServices.ObjectType;
			
			if (current.IsClass && candidate.IsClass)
			{
				if (current ==  obj || candidate == obj)
				{
					return obj;
				}
				if (current.GetTypeDepth() < candidate.GetTypeDepth())
				{
					return GetMostGenericType(current.BaseType, candidate);
				}			
				return GetMostGenericType(current, candidate.BaseType);
			}
			
			return obj;
		}
		
		IType GetMostGenericType(ExpressionCollection args)
		{
			IType type = GetExpressionType(args[0]);
			for (int i=1; i<args.Count; ++i)
			{	
				IType newType = GetExpressionType(args[i]);
				
				if (type == newType)
				{
					continue;
				}
				
				type = GetMostGenericType(type, newType);
				if (type == TypeSystemServices.ObjectType)
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
		
		StringLiteralExpression CreateStringLiteral(string value)
		{
			StringLiteralExpression expression = new StringLiteralExpression(value);
			Visit(expression);
			return expression;
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
		
		void BindComplexArraySlicing(SlicingExpression node)
		{			
			if (CheckComplexSlicingParameters(node))
			{
				if (null == node.End || node.End == OmittedExpression.Default)
				{
					node.End = CreateMethodInvocation(node.Target.CloneNode(), Array_get_Length);
				}
				
				MethodInvocationExpression mie = CreateMethodInvocation(RuntimeServices_GetRange, node.Target, node.Begin, node.End);				
				
				BindExpressionType(mie, GetExpressionType(node.Target));
				node.ParentNode.Replace(node, mie);
			}
		}
		
		void BindComplexStringSlicing(SlicingExpression node)
		{
			if (CheckComplexSlicingParameters(node))
			{
				MethodInvocationExpression mie = null;
				
				if (null == node.End || node.End == OmittedExpression.Default)
				{
					mie = CreateMethodInvocation(node.Target, String_Substring_Int);
					mie.Arguments.Add(node.Begin);
				}
				else
				{	
					mie = CreateMethodInvocation(RuntimeServices_Mid, node.Target, node.Begin, node.End);
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
							NotImplemented(node, "complex slicing for anything but arrays and strings");
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
					if (defaultMember)
					{
						mie.Target = new MemberReferenceExpression(node.Target, getter.Name);
					}
					else
					{
						Expression target = ((MemberReferenceExpression)node.Target).Target;
						mie.Target = new MemberReferenceExpression(target, getter.Name);
					}
					
					Bind(mie.Target, getter);
					BindExpressionType(mie.Target, getter.Type);
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
		}
		
		override public void OnGeneratorExpression(GeneratorExpression node)
		{
			Visit(node.Iterator);
			
			Expression newIterator = ProcessIterator(node.Iterator, node.Declarations, true);
			if (null != newIterator)
			{
				node.Iterator = newIterator;
			}
			
			EnterNamespace(new DeclarationsNamespace(CurrentNamespace, TypeSystemServices, node.Declarations));			
			Visit(node.Filter);			
			Visit(node.Expression);
			LeaveNamespace();
		}
		
		override public void LeaveHashLiteralExpression(HashLiteralExpression node)
		{
			BindExpressionType(node, TypeSystemServices.HashType);
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
			
			LocalVariable localInfo = DeclareLocal(node, new Local(node.Declaration, false), type);
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
			if (!HasSideEffect(node.Expression) && !TypeSystemServices.IsError(node.Expression))
			{
				Error(CompilerErrorFactory.ExpressionStatementMustHaveSideEffect(node));
			}
		}
		
		override public void OnNullLiteralExpression(NullLiteralExpression node)
		{
			BindExpressionType(node, Null.Default);
		}
		
		override public void OnSelfLiteralExpression(SelfLiteralExpression node)
		{
			if (null == _currentMethodInfo)
			{
				Error(node, CompilerErrorFactory.SelfOutsideMethod(node));
			}
			else
			{			
				if (_currentMethodInfo.IsStatic)
				{
					Error(node, CompilerErrorFactory.ObjectRequired(node));
				}
				else
				{
					TypeDefinition typedef = _currentMethodInfo.Method.DeclaringType;
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
				BindExpressionType(memberRef, member.Type);
			}
			else
			{
				memberRef.Target = new SelfLiteralExpression(node.LexicalInfo);
			}
			
			Bind(memberRef, member);
			BindExpressionType(memberRef.Target, member.DeclaringType);			
			
			node.ParentNode.Replace(node, memberRef);
			
			if (EntityType.Method != member.EntityType)
			{
				Visit(memberRef);
			}
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
				ReplaceByStaticFieldReference(node, "__re" + _context.AllocIndex() + "__", type);				
			}
		}
		
		void ReplaceByStaticFieldReference(Expression node, string fieldName, IType type)
		{
			Node parent = node.ParentNode;
			
			Field field = new Field(node.LexicalInfo);
			field.Name = fieldName;
			field.Type = CreateTypeReference(type);
			field.Modifiers = TypeMemberModifiers.Private|TypeMemberModifiers.Static;
			field.Initializer = node;
			
			_currentMethodInfo.Method.DeclaringType.Members.Add(field);
			InternalField tag = new InternalField(TypeSystemServices, field);
			Bind(field, tag);
			
			AddFieldInitializerToStaticConstructor(0, field);

			MemberReferenceExpression reference = new MemberReferenceExpression(node.LexicalInfo);
			reference.Target = new ReferenceExpression(field.DeclaringType.FullName);
			reference.Name = field.Name;
			BindExpressionType(reference.Target, _currentMethodInfo.DeclaringType);
			Bind(reference, tag);			
			BindExpressionType(reference, tag.Type);
			
			parent.Replace(node, reference);
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
				
				default:
				{
					if (EntityType.BuiltinFunction != tag.EntityType)
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
				return ns;
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
				INamespace ns = GetReferenceNamespace(node);				
				IEntity member = Resolve(ns, node.Name);				
				if (null == member)
				{										
					Error(node, CompilerErrorFactory.MemberNotFound(node, ((IEntity)ns).FullName));
				}
				else
				{
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
								node.ParentNode.Replace(node, CreateMethodInvocation(node.Target, ((IProperty)member).GetGetMethod()));
								return;
							}
						}
					}
					
					Bind(node, member);
					PostProcessReferenceExpression(node);
				}
			}
		}
		
		override public void LeaveUnlessStatement(UnlessStatement node)
		{
			CheckBoolContext(node.Condition);
		}
		
		override public void LeaveIfStatement(IfStatement node)
		{
			CheckBoolContext(node.Condition);			
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
		
		override public void LeaveReturnStatement(ReturnStatement node)
		{
			if (null != node.Expression)
			{
				IType returnType = _currentMethodInfo.ReturnType;
				if (TypeSystemServices.IsUnknown(returnType))
				{
					_currentMethodInfo.ReturnExpressions.Add(node.Expression);
				}
				else
				{
					IType expressionType = GetExpressionType(node.Expression);
					CheckTypeCompatibility(node.Expression, returnType, expressionType);
				}
			}
		}
		
		/// <summary>
		/// Process a iterator and its declarations and returns a new iterator
		/// expression if necessary.
		/// </summary>
		Expression ProcessIterator(Expression iterator, DeclarationCollection declarations, bool declarePrivateScopeLocals)
		{
			Expression newIterator = null;
			
			IType iteratorType = GetExpressionType(iterator);			
			bool runtimeIterator = false;			
			if (!TypeSystemServices.IsError(iteratorType))
			{
				CheckIterator(iterator, iteratorType, out runtimeIterator);
			}			
			if (runtimeIterator)
			{
				if (IsTextReader(iteratorType))
				{
					iteratorType = TextReaderEnumerator_Constructor.DeclaringType;
					newIterator = CreateMethodInvocation(TextReaderEnumerator_Constructor, iterator);
				}
				else
				{
					newIterator = CreateMethodInvocation(RuntimeServices_GetEnumerable, iterator);
				}
			}
			
			ProcessDeclarationsForIterator(declarations, iteratorType, declarePrivateScopeLocals);
			
			return newIterator;
		}
		
		override public void OnForStatement(ForStatement node)
		{		
			Visit(node.Iterator);
			
			Expression newIterator = ProcessIterator(node.Iterator, node.Declarations, true);
			if (null != newIterator)
			{
				node.Iterator = newIterator;
			}
			
			EnterNamespace(new DeclarationsNamespace(CurrentNamespace, TypeSystemServices, node.Declarations));
			EnterLoop();
			Visit(node.Block);
			LeaveLoop();
			LeaveNamespace();
		}
		
		override public void OnUnpackStatement(UnpackStatement node)
		{
			Visit(node.Expression);
			
			Expression newIterator = ProcessIterator(node.Expression, node.Declarations, false);
			if (null != newIterator)
			{
				node.Expression = newIterator;
			}
		}
		
		override public void LeaveRaiseStatement(RaiseStatement node)
		{
			if (TypeSystemServices.StringType == GetExpressionType(node.Exception))
			{
				MethodInvocationExpression expression = new MethodInvocationExpression(node.Exception.LexicalInfo);
				expression.Arguments.Add(node.Exception);
				expression.Target = new ReferenceExpression("System.ApplicationException");
				Bind(expression.Target, ApplicationException_StringConstructor);
				BindExpressionType(expression, TypeSystemServices.ApplicationExceptionType);

				node.Exception = expression;				
			}
		}
		
		override public void OnExceptionHandler(ExceptionHandler node)
		{
			if (null == node.Declaration.Type)
			{
				node.Declaration.Type = CreateTypeReference(TypeSystemServices.ExceptionType);				
			}
			else
			{
				Visit(node.Declaration.Type);
			}
			
			DeclareLocal(node.Declaration, new Local(node.Declaration, true), GetType(node.Declaration.Type));
			EnterNamespace(new DeclarationsNamespace(CurrentNamespace, TypeSystemServices, node.Declaration));
			Visit(node.Block);
			LeaveNamespace();
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
					IType expressionType = GetExpressionType(node.Right);				
					DeclareLocal(reference, new Local(reference, false), expressionType);
					BindExpressionType(node, expressionType);
					return false;
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
			
			if (IsIntegerNumber(lhs) && IsIntegerNumber(rhs))
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
						parent.Replace(node, CreateNotExpression(expression));
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
			ITypedEntity expressionInfo = (ITypedEntity)GetEntity(node.Right);
			CheckDelegateArgument(node, eventInfo, expressionInfo);
			
			BindExpressionType(node, TypeSystemServices.VoidType);
		}
		
		MethodInvocationExpression CreateMethodInvocation(Expression target, IMethod tag, Expression arg)
		{
			MethodInvocationExpression mie = CreateMethodInvocation(target, tag);
			mie.Arguments.Add(arg);
			return mie;
		}
		
		MethodInvocationExpression CreateMethodInvocation(Expression target, IMethod tag)
		{
			MemberReferenceExpression member = new MemberReferenceExpression(target.LexicalInfo);
			member.Target = target;
			member.Name = tag.Name;			
			
			MethodInvocationExpression mie = new MethodInvocationExpression(target.LexicalInfo);
			mie.Target = member;
			Bind(mie.Target, tag);
			BindExpressionType(mie.Target, tag.Type);
			BindExpressionType(mie, tag.ReturnType);
			
			return mie;			
		}
		
		MethodInvocationExpression CreateMethodInvocation(IMethod staticMethod, Expression arg)
		{
			MethodInvocationExpression mie = new MethodInvocationExpression(arg.LexicalInfo);
			mie.Target = new ReferenceExpression(staticMethod.FullName);
			mie.Arguments.Add(arg);
			
			Bind(mie.Target, staticMethod);
			BindExpressionType(mie, staticMethod.ReturnType);
			return mie;
		}
		
		MethodInvocationExpression CreateMethodInvocation(IMethod staticMethod, Expression arg0, Expression arg1)
		{
			MethodInvocationExpression mie = CreateMethodInvocation(staticMethod, arg0);
			mie.Arguments.Add(arg1);
			return mie;
		}
		
		MethodInvocationExpression CreateMethodInvocation(IMethod staticMethod, Expression arg0, Expression arg1, Expression arg2)
		{
			MethodInvocationExpression mie = CreateMethodInvocation(staticMethod, arg0, arg1);
			mie.Arguments.Add(arg2);
			return mie;
		}
		
		public void OnBuiltinFunction(IEntity tag, MethodInvocationExpression node)
		{
			BuiltinFunction sf = (BuiltinFunction)tag;
			switch (sf.FunctionType)
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
				
				default:
				{
					NotImplemented(node, "BuiltinFunction: " + sf.FunctionType);
					break;
				}
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
				if (TypeSystemServices.ObjectType == type)
				{
					resultingNode = CreateMethodInvocation(RuntimeServices_Len, target);
				}
				else if (TypeSystemServices.StringType == type)
				{
					resultingNode = CreateMethodInvocation(target, String_get_Length);
				}
				else if (TypeSystemServices.ArrayType.IsAssignableFrom(type))
				{
					resultingNode = CreateMethodInvocation(target, Array_get_Length);
				}
				else if (TypeSystemServices.ICollectionType.IsAssignableFrom(type))
				{
					resultingNode = CreateMethodInvocation(target, ICollection_get_Count);
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
			if (Array_TypedEnumerableConstructor == method ||
				Array_TypedCollectionConstructor == method ||				
				Array_TypedConstructor2 == method)
			{				
				Node parent = expression.ParentNode;
				
				IType returnType = TypeSystemServices.GetArrayType(GetType(expression.Arguments[0]));
				
				CastExpression cast = new CastExpression(expression.LexicalInfo);
				cast.Type = CreateTypeReference(returnType);
				cast.Target = expression;
				BindExpressionType(cast, returnType);
				
				parent.Replace(expression, cast);
			}
		}
		
		IEntity ResolveAmbiguousMethodInvocation(MethodInvocationExpression node, Ambiguous entity)
		{
			IEntity[] entities = ((Ambiguous)entity).Entities;
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
				return;
			}
			
			Visit(node.Target);			
			Visit(node.Arguments);			
			
			if (TypeSystemServices.IsError(node.Target) ||
				TypeSystemServices.IsErrorAny(node.Arguments))
			{
				Error(node);
				return;
			}
			
			IEntity targetInfo = GetEntity(node.Target);
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
					OnBuiltinFunction(targetInfo, node);
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
					
					Bind(node, Unknown.Default);
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
					IType typeInfo = ((ITypedEntity)targetInfo).Type;					
					ResolveNamedArguments(node, typeInfo, node.NamedArguments);
					
					IConstructor ctorInfo = FindCorrectConstructor(node, typeInfo, node.Arguments);
					if (null != ctorInfo)
					{
						// rebind the target now we know
						// it is a constructor call
						Bind(node.Target, ctorInfo);
						// expression result type is a new object
						// of type
						BindExpressionType(node, typeInfo);
					}
					else
					{
						Error(node);
					}
					break;
				}
				
				case EntityType.Error:
				{
					Error(node);
					break;
				}
				
				default:
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
					break;
				}
			}
		}	
		
		void ProcessMethodInvocationOnCallableExpression(MethodInvocationExpression node)
		{
			IType type = node.Target.ExpressionType;
			if (TypeSystemServices.ICallableType.IsAssignableFrom(type))
			{
				node.Target = new MemberReferenceExpression(node.Target.LexicalInfo,
									node.Target,
									"Call");
				ArrayLiteralExpression arg = new ArrayLiteralExpression();
				BindExpressionType(arg, TypeSystemServices.ObjectArrayType);
				arg.Items.Extend(node.Arguments);							
				node.Arguments.Clear();
				node.Arguments.Add(arg);							
				
				Bind(node.Target, ICallable_Call);
				BindExpressionType(node, ICallable_Call.ReturnType);
			}
			else if (TypeSystemServices.TypeType == type)
			{
				Expression targetType = node.Target;
				
				node.Target = new ReferenceExpression(targetType.LexicalInfo,
											"System.Activator.CreateInstance");
										
				ArrayLiteralExpression constructorArgs = new ArrayLiteralExpression();
				BindExpressionType(constructorArgs, TypeSystemServices.ObjectArrayType);
				constructorArgs.Items.Extend(node.Arguments);
				
				node.Arguments.Clear();
				node.Arguments.Add(targetType);
				node.Arguments.Add(constructorArgs);							
				
				Bind(node.Target, Activator_CreateInstance);
				BindExpressionType(node, Activator_CreateInstance.ReturnType);
			}
			else
			{
				ICallableType delegateType = type as ICallableType;
				if (null != delegateType)
				{
					if (CheckParameters(node.Target, delegateType, node.Arguments))
					{
						MemberReferenceExpression expression = new MemberReferenceExpression(node.Target.LexicalInfo);
						expression.Target = node.Target;
						expression.Name = "Invoke";
						node.Target = expression;
						Bind(expression, ResolveMethod(delegateType, "Invoke"));
						BindExpressionType(node, delegateType.GetSignature().ReturnType);						
					}
				}
				else
				{
					NotImplemented(node, "Method invocation on type '" + type + "'.");
				}
			}
		}
		
		MethodInvocationExpression CreateEquals(BinaryExpression node)
		{
			return CreateMethodInvocation(Object_StaticEquals, node.Left, node.Right);
		}
		
		UnaryExpression CreateNotExpression(Expression node)
		{
			UnaryExpression notNode = new UnaryExpression();
			notNode.LexicalInfo = node.LexicalInfo;
			notNode.Operand = node;
			notNode.Operator = UnaryOperatorType.LogicalNot;
			
			BindExpressionType(notNode, TypeSystemServices.BoolType);
			return notNode;
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
				MemberReferenceExpression target = new MemberReferenceExpression(slice.Target.LexicalInfo);
				target.Target = slice.Target;
				target.Name = setter.Name;				
				
				mie.Target = target;
				
				Bind(target, setter);
				BindExpressionType(target, setter.Type);
				Bind(mie, setter);
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
			return TypeSystemServices.ObjectType == type ||
				TypeSystemServices.StringType == type;
		}
		
		void BindInPlaceArithmeticOperator(BinaryExpression node)
		{
			Node parent = node.ParentNode;
			
			BinaryExpression assign = new BinaryExpression(node.LexicalInfo);
			assign.Operator = BinaryOperatorType.Assign;
			assign.Left = node.Left.CloneNode();
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
			parent.Replace(node, CreateNotExpression(node));
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
			IEntity candidate = Resolve(type, name, EntityType.Property|EntityType.Event|EntityType.Field);
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
					CheckDelegateArgument(arg.First, member, (ITypedEntity)GetEntity(arg.Second));
				}
				else
				{						
					CheckTypeCompatibility(arg, memberType, GetExpressionType(arg.Second));					
				}
			}
		}
		
		bool CheckTypeCompatibility(Node sourceNode, IType expectedType, IType actualType)
		{
			if (!IsAssignableFrom(expectedType, actualType) &&
				!CanBeReachedByDownCastOrPromotion(expectedType, actualType))
			{
				Error(CompilerErrorFactory.IncompatibleExpressionType(sourceNode, expectedType.FullName, actualType.FullName));
				return false;
			}
			return true;
		}
		
		bool CheckDelegateArgument(Node sourceNode, ITypedEntity delegateMember, ITypedEntity argumentInfo)
		{
			ICallableType delegateType = (ICallableType)delegateMember.Type;
			if (argumentInfo.EntityType != EntityType.Method ||
					    !CheckCallableSignature(delegateType, ((IMethod)argumentInfo).CallableType))
			{
				Error(CompilerErrorFactory.EventArgumentMustBeAMethod(sourceNode, delegateMember.FullName, delegateType.FullName));
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
				IType expressionType = GetExpressionType(args[i]);
				IType parameterType = parameters[i].Type;
				if (!IsAssignableFrom(parameterType, expressionType) &&
				    !CanBeReachedByDownCastOrPromotion(parameterType, expressionType))
				{
					return false;
				}
			}
			return true;
		}
		
		bool CheckParameters(Node sourceNode, IMethod method, ExpressionCollection args)
		{
			return CheckParameters(sourceNode, method.CallableType, args);
		}
		
		bool CheckParameters(Node sourceNode, ICallableType method, ExpressionCollection args)
		{				
			if (method.GetSignature().Parameters.Length != args.Count)
			{
				Error(CompilerErrorFactory.MethodArgumentCount(sourceNode, method.Name, args.Count));
				return false;
			}	
			
			if (!CheckParameterTypes(method, args))
			{
				Error(CompilerErrorFactory.MethodSignature(sourceNode, method.ToString(), GetSignature(args)));
			}
			return true;
		}
		
		
		bool CheckCallableSignature(ICallableType expected, ICallableType actual)
		{
			return expected.GetSignature() == actual.GetSignature();			
		}
		
		bool IsRuntimeIterator(IType type)
		{
			return  TypeSystemServices.ObjectType == type ||
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
		
		bool CanBeReachedByDownCastOrPromotion(IType expectedType, IType actualType)
		{			
			if (actualType.IsAssignableFrom(expectedType))
			{
				return true;
			}
			if (expectedType.IsValueType)
			{
				return IsNumber(expectedType) && IsNumber(actualType);
			}
			return false;
		}
		
		bool IsIntegerNumber(IType type)
		{
			return
				type == TypeSystemServices.ShortType ||
				type == TypeSystemServices.IntType ||
				type == TypeSystemServices.LongType ||
				type == TypeSystemServices.ByteType;
		}
		
		bool IsNumber(IType type)
		{
			return
				IsIntegerNumber(type) ||
				type == TypeSystemServices.DoubleType ||
				type == TypeSystemServices.SingleType;
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
				return (IConstructor)ResolveCallableReference(sourceNode, arguments, constructors, true);				
			}
			else
			{
				Error(CompilerErrorFactory.NoApropriateConstructorFound(sourceNode, typeInfo.FullName, GetSignature(arguments)));
			}
			return null;
		}
		
		IConstructor GetDefaultConstructor(IType type)
		{
			IConstructor[] constructors = type.GetConstructors();
			for (int i=0; i<constructors.Length; ++i)
			{
				IConstructor constructor = constructors[i];
				if (0 == constructor.GetParameters().Length)
				{
					return constructor;
				}
			}
			return null;
		}
		
		class InfoScore : IComparable
		{
			public IMethod Info;
			public int Score;
			
			public InfoScore(IMethod tag, int score)
			{
				Info = tag;
				Score = score;
			}
			
			public int CompareTo(object other)
			{
				return ((InfoScore)other).Score-Score;
			}
			
			override public string ToString()
			{
				return Info.ToString();
			}
		}
		
		void EnsureRelatedNodeWasVisited(IEntity tag)
		{
			if (tag.EntityType == EntityType.Ambiguous)
			{
				foreach (IEntity item in ((Ambiguous)tag).Entities)
				{
					EnsureRelatedNodeWasVisited(item);
				}
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
		
		InfoScore GetBiggerScore(List scores)
		{
			scores.Sort();
			InfoScore first = (InfoScore)scores[0];
			InfoScore second = (InfoScore)scores[1];
			if (first.Score > second.Score)
			{
				return first;
			}
			return null;
		}
		
		void ReScoreByHierarchyDepth(List scores)
		{
			foreach (InfoScore score in scores)
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
							IType expressionType = GetExpressionTypeOrEntityType(args.GetNodeAt(argIndex));
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
							else if (CanBeReachedByDownCastOrPromotion(parameterType, expressionType))
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
							scores.Add(new InfoScore(mb, score));						
						}
					}
				}
			}		
			
			if (1 == scores.Count)
			{
				return ((InfoScore)scores[0]).Info;
			}
			
			if (scores.Count > 1)
			{
				InfoScore score = GetBiggerScore(scores);
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
						Error(CompilerErrorFactory.NoApropriateOverloadFound(node, GetSignature(args), tag.Name));
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
			IEntity tag = Resolve(type, operatorName, EntityType.Method);
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
		
		bool CheckLValue(Node node)
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
						return !((LocalVariable)tag).IsPrivateScope;
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
						return true;
					}
				}
			}
			
			Error(CompilerErrorFactory.LValueExpected(node));
			return false;
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
		
		LocalVariable DeclareLocal(Node sourceNode, Local local, IType localType)
		{			
			LocalVariable tag = new LocalVariable(local, localType);
			Bind(local, tag);
			
			_currentMethodInfo.Method.Locals.Add(local);
			Bind(sourceNode, tag);
			
			return tag;
		}
		
		void PushMethodInfo(InternalMethod tag)
		{
			_methodInfoStack.Push(_currentMethodInfo);
			
			_currentMethodInfo = tag;
		}
		
		void PopMethodInfo()
		{
			_currentMethodInfo = (InternalMethod)_methodInfoStack.Pop();
		}
		
		static bool HasSideEffect(Expression node)
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
			if (null == _currentMethodInfo.ResolveLocal(d.Name) &&
				null == _currentMethodInfo.ResolveParameter(d.Name))
			{
				return true;
			}
			Error(CompilerErrorFactory.LocalAlreadyExists(d, d.Name));
			return false;
		}
		
		IType GetExternalEnumeratorItemType(IType iteratorType)
		{
			Type type = ((ExternalType)iteratorType).ActualType;
			EnumeratorItemTypeAttribute attribute = (EnumeratorItemTypeAttribute)System.Attribute.GetCustomAttribute(type, typeof(EnumeratorItemTypeAttribute));
			if (null != attribute)
			{
				return TypeSystemServices.Map(attribute.ItemType);
			}
			return null;
		}
		
		IType GetEnumeratorItemTypeFromAttribute(IType iteratorType)
		{
			InternalType internalType = iteratorType as InternalType;
			if (null == internalType)
			{
				return GetExternalEnumeratorItemType(iteratorType);
			}
			
			IType enumeratorItemTypeAttribute = TypeSystemServices.Map(typeof(EnumeratorItemTypeAttribute));
			foreach (Boo.Lang.Compiler.Ast.Attribute attribute in internalType.TypeDefinition.Attributes)
			{				
				IConstructor constructor = GetEntity(attribute) as IConstructor;
				if (null != constructor)
				{
					if (constructor.DeclaringType == enumeratorItemTypeAttribute)
					{
						return GetType(attribute.Arguments[0]);
					}
				}
			}
			return null;
		}
		
		IType GetEnumeratorItemType(IType iteratorType)
		{
			if (iteratorType.IsArray)
			{
				return ((IArrayType)iteratorType).GetElementType();
			}
			else
			{
				if (iteratorType.IsClass)
				{
					IType enumeratorItemType = GetEnumeratorItemTypeFromAttribute(iteratorType);
					if (null != enumeratorItemType)
					{
						return enumeratorItemType;
					}
				}
			}
			return TypeSystemServices.ObjectType;
		}
		
		void ProcessDeclarationsForIterator(DeclarationCollection declarations, IType iteratorType, bool declarePrivateLocals)
		{
			IType defaultDeclType = GetEnumeratorItemType(iteratorType);
			if (declarations.Count > 1)
			{
				// will enumerate (unpack) each item
				defaultDeclType = GetEnumeratorItemType(defaultDeclType);
			}
			
			foreach (Declaration d in declarations)
			{	
				bool declareNewVariable = declarePrivateLocals || null != d.Type;
				
				if (null != d.Type)
				{
					Visit(d.Type);
					CheckTypeCompatibility(d, GetType(d.Type), defaultDeclType);					
				}
				
				if (CheckIdentifierName(d, d.Name))
				{
					if (declareNewVariable)
					{
						if (!declarePrivateLocals)
						{
							CheckUniqueLocal(d);
						}
					}
					else
					{
						IEntity tag = Resolve(_currentMethodInfo, d.Name);
						if (null != tag)
						{
							Bind(d, tag);
							continue;
						}
					}
					
					if (null == d.Type)
					{
						d.Type = CreateTypeReference(defaultDeclType);
					}
					
					DeclareLocal(d, new Local(d, declarePrivateLocals), GetType(d.Type));
				}
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
			return TypeSystemServices.GetSignature(tag);
		}
		
		bool Visited(Node node)
		{
			return _visited.ContainsKey(node);
		}
		
		void MarkVisited(Node node)
		{
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
	}
}
