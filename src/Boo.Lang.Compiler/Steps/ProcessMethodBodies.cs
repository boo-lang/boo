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
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Taxonomy;
	using List=Boo.Lang.List;

	/// <summary>
	/// AST semantic evaluation.
	/// </summary>
	public class ProcessMethodBodies : AbstractNamespaceSensitiveVisitorCompilerStep
	{	
		Stack _methodInfoStack;
		
		InternalMethod _currentMethodInfo;
		
		ArrayList _classes;
		
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
		
		public ProcessMethodBodies()
		{
			IsPublicFieldPropertyEventFilter = new InfoFilter(IsPublicFieldPropertyEvent);
			IsPublicEventFilter = new InfoFilter(IsPublicEvent);
		}
		
		override public void Run()
		{					
			_currentMethodInfo = null;
			_methodInfoStack = new Stack();
			_classes = new ArrayList();
			_nameResolution.Initialize(_context);
			_loopDepth = 0;
						
			RuntimeServices_Len = (IMethod)TagService.RuntimeServicesType.Resolve("Len");
			RuntimeServices_Mid = (IMethod)TagService.RuntimeServicesType.Resolve("Mid");
			RuntimeServices_GetRange = (IMethod)TagService.RuntimeServicesType.Resolve("GetRange");
			RuntimeServices_GetEnumerable = (IMethod)TagService.RuntimeServicesType.Resolve("GetEnumerable");
			Object_StaticEquals = (IMethod)TagService.AsInfo(Types.Object.GetMethod("Equals", new Type[] { Types.Object, Types.Object }));
			Array_get_Length = ((IProperty)TagService.ArrayType.Resolve("Length")).GetGetMethod();
			String_get_Length = ((IProperty)TagService.StringType.Resolve("Length")).GetGetMethod();
			String_Substring_Int = (IMethod)TagService.AsInfo(Types.String.GetMethod("Substring", new Type[] { Types.Int }));
			ICollection_get_Count = ((IProperty)TagService.ICollectionType.Resolve("Count")).GetGetMethod();
			IList_Contains = (IMethod)TagService.IListType.Resolve("Contains");
			IDictionary_Contains = (IMethod)TagService.IDictionaryType.Resolve("Contains");
			Array_TypedEnumerableConstructor = (IMethod)TagService.AsInfo(Types.Builtins.GetMethod("array", new Type[] { Types.Type, Types.IEnumerable }));
			Array_TypedCollectionConstructor= (IMethod)TagService.AsInfo(Types.Builtins.GetMethod("array", new Type[] { Types.Type, Types.ICollection }));
			Array_TypedConstructor2 = (IMethod)TagService.AsInfo(Types.Builtins.GetMethod("array", new Type[] { Types.Type, Types.Int }));
			ICallable_Call = (IMethod)TagService.ICallableType.Resolve("Call");
			Activator_CreateInstance = (IMethod)TagService.AsInfo(typeof(Activator).GetMethod("CreateInstance", new Type[] { Types.Type, Types.ObjectArray }));
			TextReaderEnumerator_Constructor = (IConstructor)TagService.AsInfo(typeof(Boo.IO.TextReaderEnumerator).GetConstructor(new Type[] { typeof(System.IO.TextReader) }));
			
			ApplicationException_StringConstructor =
					(IConstructor)TagService.AsInfo(
						Types.ApplicationException.GetConstructor(new Type[] { typeof(string) }));
			
			Accept(CompileUnit);
			
			if (0 == Errors.Count)
			{
				ResolveClassInterfaceMembers();
			}
		}		
		
		void ResolveClassInterfaceMembers()
		{
			foreach (ClassDefinition node in _classes)
			{
				ResolveClassInterfaceMembers(node);
			}
		}
		
		void ResolveClassInterfaceMembers(ClassDefinition node)
		{	
			foreach (TypeReference baseType in node.BaseTypes)
			{
				IType tag = GetType(baseType);
				if (tag.IsInterface)
				{
					ResolveClassInterfaceMembers(node, baseType, tag);
				}
			}
		}
		
		Method CreateAbstractMethod(Node sourceNode, IMethod baseMethod)
		{
			Method method = new Method(sourceNode.LexicalInfo);
			method.Name = baseMethod.Name;
			method.Modifiers = TypeMemberModifiers.Public | TypeMemberModifiers.Abstract;
			for (int i=0; i<baseMethod.ParameterCount; ++i)
			{
				method.Parameters.Add(new ParameterDeclaration("arg" + i, CreateTypeReference(baseMethod.GetParameterType(i))));
			}
			method.ReturnType = CreateTypeReference(baseMethod.ReturnType);
			
			Bind(method, new InternalMethod(TagService, method));
			return method;
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
					!CheckOverrideSignature(expected, actual))
				{
					return false;
				}
			}
			return true;
		}
		
		void ResolveClassInterfaceProperty(ClassDefinition node,
											TypeReference interfaceReference,
											IProperty tag)
		{
			TypeMember member = node.Members[tag.Name];
			if (null != member && NodeType.Property == member.NodeType)
			{
				if (tag.BoundType == GetType(member))
				{
					if (CheckPropertyAccessors(tag, (IProperty)GetTag(member)))
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
			//node.Members.Add(CreateAbstractProperty(interfaceReference, tag));
			//node.Modifiers |= TypeMemberModifiers.Abstract;
		}
		
		void ResolveClassInterfaceMethod(ClassDefinition node,
										TypeReference interfaceReference,
										IMethod tag)
		{			
			if (tag.IsSpecialName)
			{
				return;
			}
			
			TypeMember member = node.Members[tag.Name];
			if (null != member && NodeType.Method == member.NodeType)
			{							
				Method method = (Method)member;
				if (CheckOverrideSignature((IMethod)GetTag(method), tag))
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
			
			node.Members.Add(CreateAbstractMethod(interfaceReference, tag));
			node.Modifiers |= TypeMemberModifiers.Abstract;
		}
		
		void ResolveClassInterfaceMembers(ClassDefinition node,
											TypeReference interfaceReference,
											IType interfaceInfo)
		{			
			foreach (IType tag in interfaceInfo.GetInterfaces())
			{
				ResolveClassInterfaceMembers(node, interfaceReference, tag);
			}
			
			foreach (IMember tag in interfaceInfo.GetMembers())
			{
				switch (tag.ElementType)
				{
					case ElementType.Method:
					{
						ResolveClassInterfaceMethod(node, interfaceReference, (IMethod)tag);
						break;
					}
					
					case ElementType.Property:
					{
						ResolveClassInterfaceProperty(node, interfaceReference, (IProperty)tag);
						break;
					}
					
					default:
					{
						NotImplemented(interfaceReference, "interface member: " + tag);
						break;
					}
				}
			}
		}
		
		override public void Dispose()
		{
			base.Dispose();
			
			_currentMethodInfo = null;
			_methodInfoStack = null;
			_classes = null;
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
			PushNamespace((INamespace)TagService.GetTag(module));			
			
			Accept(module.Members);
			
			PopNamespace();
		}
		
		void BindBaseInterfaceTypes(InterfaceDefinition node)
		{
			Accept(node.BaseTypes);
			
			foreach (TypeReference baseType in node.BaseTypes)
			{
				IType baseInfo = GetType(baseType);
				EnsureRelatedNodeWasVisited(baseInfo);
				if (!baseInfo.IsInterface)
				{
					Error(CompilerErrorFactory.InterfaceCanOnlyInheritFromInterface(baseType, node.FullName, baseInfo.FullName));
				}
			}
		}
		
		void BindBaseTypes(ClassDefinition node)
		{
			Accept(node.BaseTypes);
			
			IType baseClass = null;
			foreach (TypeReference baseType in node.BaseTypes)
			{				
				IType baseInfo = GetType(baseType);
				EnsureRelatedNodeWasVisited(baseInfo);
				if (baseInfo.IsClass)
				{
					if (null != baseClass)
					{
						Error(
						    CompilerErrorFactory.ClassAlreadyHasBaseType(baseType,
								node.Name,
								baseClass.FullName)
							); 
					}
					else
					{
						baseClass = baseInfo;
					}
				}
			}
			
			if (null == baseClass)
			{
				node.BaseTypes.Insert(0, CreateTypeReference(TagService.ObjectType)	);
			}
		}
		
		override public void OnEnumDefinition(EnumDefinition node)
		{
			EnumType tag = (EnumType)GetOptionalInfo(node);
			if (null == tag)
			{
				tag = new EnumType(TagService, (EnumDefinition)node);
			}
			else if (tag.Visited)
			{
				return;
			}
			
			tag.Visited = true;
			
			if (!node.IsVisibilitySet)
			{
				node.Modifiers |= TypeMemberModifiers.Public;
			}
			
			Accept(node.Attributes);
			
			long lastValue = 0;
			foreach (EnumMember member in node.Members)
			{
				if (null == member.Initializer)
				{
					member.Initializer = new IntegerLiteralExpression(lastValue);
				}
				Accept(member.Attributes);
				Accept(member.Initializer);
				lastValue = member.Initializer.Value + 1;
			}
		}
		
		override public void OnInterfaceDefinition(InterfaceDefinition node)
		{
			InternalType tag = GetInternalType(node);
			if (tag.Visited)
			{
				return;
			}
			
			tag.Visited = true;
			BindBaseInterfaceTypes(node);
			
			PushNamespace(tag);
			Accept(node.Attributes);
			Accept(node.Members);
			PopNamespace();
		}
		
		override public void OnClassDefinition(ClassDefinition node)
		{
			InternalType tag = GetInternalType(node);
			if (tag.Visited)
			{
				return;
			}
			
			_classes.Add(node);
			
			tag.Visited = true;
			BindBaseTypes(node);
			
			PushNamespace(tag);
			Accept(node.Attributes);		
			ProcessFields(node);
			Accept(node.Members);
			PopNamespace();
		}		
		
		override public void OnAttribute(Boo.Lang.Compiler.Ast.Attribute node)
		{
			IType tag = TagService.GetType(node);
			if (null != tag && !TagService.IsError(tag))
			{			
				Accept(node.Arguments);
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
			InternalProperty tag = (InternalProperty)GetOptionalInfo(node);
			if (null == tag)
			{
				tag = new InternalProperty(TagService, node);
				Bind(node, tag);
			}
			else
			{
				if (tag.Visited)
				{
					return;
				}
			}
			
			tag.Visited = true;
			
			Method setter = node.Setter;
			Method getter = node.Getter;
			
			Accept(node.Attributes);			
			Accept(node.Type);
			
			Accept(node.Parameters);
			if (null != getter)
			{
				if (null != node.Type)
				{
					getter.ReturnType = node.Type.CloneNode();
				}
				getter.Name = "get_" + node.Name;
				getter.Parameters.ExtendWithClones(node.Parameters);
				Accept(getter);
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
					typeInfo = TagService.ObjectType;
				}
				node.Type = CreateTypeReference(typeInfo);
			}
			
			if (null != setter)
			{
				ParameterDeclaration parameter = new ParameterDeclaration();
				parameter.Type = CreateTypeReference(typeInfo);
				parameter.Name = "value";
				setter.Parameters.ExtendWithClones(node.Parameters);
				setter.Parameters.Add(parameter);
				Accept(setter);
				
				setter.Name = "set_" + node.Name;
			}
		}
		
		override public void OnField(Field node)
		{
			InternalField tag = (InternalField)GetOptionalInfo(node);
			if (null == tag)
			{
				tag = new InternalField(TagService, node);
				Bind(node, tag);
			}
			else
			{
				if (tag.Visited)
				{
					return;
				}
			}
			
			// first time here
			tag.Visited = true;			
			
			Accept(node.Attributes);			
			
			ProcessFieldInitializer(node);			
			
			if (null == node.Type)
			{
				if (null == node.Initializer)
				{
					node.Type = CreateTypeReference(TagService.ObjectType);
				}
				else
				{
					node.Type = CreateTypeReference(GetType(node.Initializer));
				}
			}
			else
			{
				Accept(node.Type);
				
				if (null != node.Initializer)
				{
					CheckTypeCompatibility(node.Initializer, GetType(node.Type), GetType(node.Initializer));
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
			
			Accept(fields);
			
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
					Accept(method);
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
				Bind(constructor, new InternalConstructor(TagService, constructor, true));
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
			InternalField tag = (InternalField)GetTag(node);
			
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
			
			Bind(context, tag.DeclaringType);
			
			MemberReferenceExpression member = new MemberReferenceExpression(context, node.Name);
			Bind(member, tag);
			
			// <node.Name> = <node.Initializer>
			stmt.Expression = new BinaryExpression(BinaryOperatorType.Assign,
									member,
									node.Initializer);
			Bind(stmt.Expression, tag.BoundType);
			
			return stmt;
		}
		
		Statement CreateDefaultConstructorCall(Constructor node, InternalConstructor tag)
		{			
			IConstructor defaultConstructor = GetDefaultConstructor(tag.DeclaringType.BaseType);
			
			MethodInvocationExpression call = new MethodInvocationExpression(new SuperLiteralExpression());
			
			Bind(call, defaultConstructor);
			Bind(call.Target, defaultConstructor);
			
			return new ExpressionStatement(call);
		}
		
		override public bool EnterConstructor(Constructor node)
		{			
			InternalConstructor tag = (InternalConstructor)TagService.GetOptionalInfo(node);
			if (null == tag)
			{
				tag = new InternalConstructor(TagService, node);
			}
			else
			{
				if (tag.Visited)
				{
					return false;
				}
			}
			
			tag.Visited = true;
			
			Bind(node, tag);
			PushMethodInfo(tag);
			PushNamespace(tag);
			return true;
		}
		
		override public void LeaveConstructor(Constructor node)
		{
			InternalConstructor tag = (InternalConstructor)_currentMethodInfo;
			if (!tag.HasSuperCall && !node.IsStatic)
			{
				node.Body.Statements.Insert(0, CreateDefaultConstructorCall(node, tag));
			}
			PopNamespace();
			PopMethodInfo();
			BindParameterIndexes(node);
		}
		
		override public bool EnterParameterDeclaration(ParameterDeclaration parameter)
		{
			return !TagService.IsBound(parameter);
		}
		
		override public void LeaveParameterDeclaration(ParameterDeclaration parameter)
		{			
			if (null == parameter.Type)
			{
				parameter.Type = CreateTypeReference(TagService.ObjectType);
			}
			CheckIdentifierName(parameter, parameter.Name);
			Taxonomy.ParameterInfo tag = new Taxonomy.ParameterInfo(parameter, GetType(parameter.Type));
			Bind(parameter, tag);
		}	
		
		override public void OnMethod(Method method)
		{				
			InternalMethod tag = (InternalMethod)GetOptionalInfo(method);
			if (null == tag)
			{
				tag = new InternalMethod(TagService, method);
				Bind(method, tag);
			}
			else
			{
				if (tag.Visited)
				{
					return;
				}
			}
			
			bool parentIsClass = method.DeclaringType.NodeType == NodeType.ClassDefinition;
			
			tag.Visited = true;
			Accept(method.Attributes);
			Accept(method.Parameters);
			Accept(method.ReturnType);
			Accept(method.ReturnTypeAttributes);
			
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
			PushNamespace(tag);
			
			Accept(method.Body);
			
			PopNamespace();
			PopMethodInfo();
			BindParameterIndexes(method);			
			
			if (parentIsClass)
			{
				if (TagService.IsUnknown(tag.BoundType))
				{
					if (CanResolveReturnType(tag))
					{
						ResolveReturnType(tag);
						CheckMethodOverride(tag);
					}
					else
					{
						Error(method.ReturnType, CompilerErrorFactory.RecursiveMethodWithoutReturnType(method));
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
			if (ElementType.Constructor != _currentMethodInfo.ElementType)
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
			if (null == baseMethod || tag.BoundType != baseMethod.ReturnType)
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
			IElement baseMethods = baseType.Resolve(tag.Name);
			
			if (null != baseMethods)
			{
				if (ElementType.Method == baseMethods.ElementType)
				{
					IMethod baseMethod = (IMethod)baseMethods;
					if (CheckOverrideSignature(tag, baseMethod))
					{	
						return baseMethod;
					}
				}
				else if (ElementType.Ambiguous == baseMethods.ElementType)
				{
					IElement[] tags = ((Ambiguous)baseMethods).Taxonomy;
					IMethod baseMethod = (IMethod)ResolveMethodReference(method, method.Parameters, tags, false);
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
					if (TagService.IsUnknown(tag.BoundType))
					{
						tag.Method.ReturnType = CreateTypeReference(baseMethod.ReturnType);
					}
					else
					{
						if (baseMethod.ReturnType != tag.BoundType)
						{
							Error(CompilerErrorFactory.InvalidOverrideReturnType(
											tag.Method.ReturnType,
											baseMethod.FullName,
											baseMethod.ReturnType.FullName,
											tag.BoundType.FullName));
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
		
		bool CheckOverrideSignature(IMethod impl, IMethod baseMethod)
		{
			if (impl.ParameterCount == baseMethod.ParameterCount)
			{
				for (int i=0; i<impl.ParameterCount; ++i)
				{
					if (impl.GetParameterType(i) != baseMethod.GetParameterType(i))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}
		
		IType GetBaseType(TypeDefinition typeDefinition)
		{
			return ((IType)GetTag(typeDefinition)).BaseType;
		}
		
		bool CanResolveReturnType(InternalMethod tag)
		{
			foreach (Expression rsExpression in tag.ReturnExpressions)
			{
				if (TagService.IsUnknown(GetType(rsExpression)))
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
				method.ReturnType = CreateTypeReference(TagService.VoidType);
			}		
			else
			{					
				IType type = GetMostGenericType(returnExpressions);
				if (NullInfo.Default == type)
				{
					type = TagService.ObjectType; 
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
				return TagService.GetPromotedNumberType(current, candidate);
			}
			
			IType obj = TagService.ObjectType;
			
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
				if (type == TagService.ObjectType)
				{
					break;
				}
			}
			return type;
		}
		
		void BindParameterIndexes(Method method)
		{
			ParameterDeclarationCollection parameters = method.Parameters;
			
			int delta = 1; // arg0 is the this pointer
			if (method.IsStatic)
			{
				delta = 0; // no this poiner
			}
			
			for (int i=0; i<parameters.Count; ++i)
			{
				((ParameterInfo)GetTag(parameters[i])).Index = i + delta;
			}
		}
		
		override public void OnArrayTypeReference(ArrayTypeReference node)
		{
			if (TagService.IsBound(node))
			{
				return;
			}

			Accept(node.ElementType);
			
			IType elementType = GetType(node.ElementType);
			if (TagService.IsError(elementType))
			{
				Bind(node, elementType);
			}
			else
			{
				IType arrayType = TagService.AsArrayInfo(elementType);
				Bind(node, arrayType);
			}
		}
		
		override public void OnSimpleTypeReference(SimpleTypeReference node)
		{
			ResolveSimpleTypeReference(node);
		}
		
		override public void OnBoolLiteralExpression(BoolLiteralExpression node)
		{
			Bind(node, TagService.BoolType);
		}
		
		override public void OnTimeSpanLiteralExpression(TimeSpanLiteralExpression node)
		{
			Bind(node, TagService.TimeSpanType);
		}
		
		override public void OnIntegerLiteralExpression(IntegerLiteralExpression node)
		{
			if (node.IsLong)
			{
				Bind(node, TagService.LongType);
			}
			else
			{
				Bind(node, TagService.IntType);
			}
		}
		
		override public void OnDoubleLiteralExpression(DoubleLiteralExpression node)
		{
			Bind(node, TagService.DoubleType);
		}
		
		override public void OnStringLiteralExpression(StringLiteralExpression node)
		{
			Bind(node, TagService.StringType);
		}
		
		IElement[] GetSetMethods(IElement[] tags)
		{
			ArrayList setMethods = new ArrayList();
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
			return (IElement[])setMethods.ToArray(typeof(IElement));
		}
		
		IElement[] GetGetMethods(IElement[] tags)
		{
			ArrayList getMethods = new ArrayList();
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
			return (IElement[])getMethods.ToArray(typeof(IElement));
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
			Accept(expression);
			return expression;
		}
		
		IntegerLiteralExpression CreateIntegerLiteral(long value)
		{
			IntegerLiteralExpression expression = new IntegerLiteralExpression(value);
			Accept(expression);
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
				if (!CheckTypeCompatibility(node.Begin, TagService.IntType, GetExpressionType(node.Begin)))
				{
					Error(node);
					return false;
				}
			}			
			
			if (null != node.End && OmittedExpression.Default != node.End)
			{
				if (!CheckTypeCompatibility(node.End, TagService.IntType, GetExpressionType(node.End)))
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
				
				Bind(mie, GetExpressionType(node.Target));
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
		
		override public void LeaveSlicingExpression(SlicingExpression node)
		{
			IType targetType = GetExpressionType(node.Target);
			if (TagService.IsError(targetType))
			{
				Error(node);
				return;
			}
			
			IElement tag = GetTag(node.Target);
			if (IsIndexedProperty(tag))
			{
				CheckNoComplexSlicing(node);
				SliceMember(node, tag, false);
			}
			else
			{
				if (targetType.IsArray)
				{
					if (targetType.GetArrayRank() != 1)
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
						Bind(node, targetType.GetElementType());
					}
				}
				else
				{
					if (IsComplexSlicing(node))
					{
						if (TagService.StringType == targetType)
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
						IElement member = targetType.GetDefaultMember();
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
		
		bool IsIndexedProperty(IElement tag)
		{
			return ElementType.Property == tag.ElementType &&
				((IProperty)tag).GetIndexParameters().Length > 0;
		}
		
		void SliceMember(SlicingExpression node, IElement member, bool defaultMember)
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
			
			if (ElementType.Ambiguous == member.ElementType)
			{
				IElement[] tags = GetGetMethods(((Ambiguous)member).Taxonomy);
				getter = (IMethod)ResolveMethodReference(node, mie.Arguments, tags, true);						
			}
			else if (ElementType.Property == member.ElementType)
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
					Bind(mie, getter);
					
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
			Bind(node, TagService.StringType);
		}
		
		override public void LeaveListLiteralExpression(ListLiteralExpression node)
		{			
			Bind(node, TagService.ListType);
		}
		
		override public void OnGeneratorExpression(GeneratorExpression node)
		{
			Accept(node.Iterator);
			
			Expression newIterator = ProcessIterator(node.Iterator, node.Declarations, true);
			if (null != newIterator)
			{
				node.Iterator = newIterator;
			}
			
			PushNamespace(new DeclarationsNamespace(CurrentNamespace, TagService, node.Declarations));			
			Accept(node.Filter);			
			Accept(node.Expression);
			PopNamespace();
		}
		
		override public void LeaveHashLiteralExpression(HashLiteralExpression node)
		{
			Bind(node, TagService.HashType);
		}
		
		override public void LeaveArrayLiteralExpression(ArrayLiteralExpression node)
		{
			ExpressionCollection items = node.Items;
			if (0 == items.Count)
			{
				Bind(node, TagService.ObjectArrayType);
			}
			else
			{
				Bind(node, TagService.AsArrayInfo(GetMostGenericType(items)));
			}
		}
		
		override public void LeaveDeclarationStatement(DeclarationStatement node)
		{
			IType tag = TagService.ObjectType;
			if (null != node.Declaration.Type)
			{
				tag = GetType(node.Declaration.Type);			
			}			
			
			CheckDeclarationName(node.Declaration);
			
			LocalVariable localInfo = DeclareLocal(node, new Local(node.Declaration, false), tag);
			if (null != node.Initializer)
			{
				CheckTypeCompatibility(node.Initializer, tag, GetExpressionType(node.Initializer));
				
				ReferenceExpression var = new ReferenceExpression(node.Declaration.LexicalInfo);
				var.Name = node.Declaration.Name;
				Bind(var, localInfo);				
				
				BinaryExpression assign = new BinaryExpression(node.LexicalInfo);
				assign.Operator = BinaryOperatorType.Assign;
				assign.Left = var;
				assign.Right = node.Initializer;
				Bind(assign, tag);				
				
				node.ReplaceBy(new ExpressionStatement(assign));
			}
			else
			{
				node.ReplaceBy(null);
			}
		}
		
		override public void LeaveExpressionStatement(ExpressionStatement node)
		{
			if (!HasSideEffect(node.Expression) && !TagService.IsError(node.Expression))
			{
				Error(CompilerErrorFactory.ExpressionStatementMustHaveSideEffect(node));
			}
		}
		
		override public void OnNullLiteralExpression(NullLiteralExpression node)
		{
			Bind(node, NullInfo.Default);
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
					Bind(node, TagService.GetTag(_currentMethodInfo.Method.DeclaringType));
				}
			}
		}
		
		override public void LeaveTypeofExpression(TypeofExpression node)
		{
			if (TagService.IsError(node.Type))
			{
				Error(node);
			}
			else
			{
				Bind(node, TagService.TypeType);
			}
		}
		
		override public void LeaveCastExpression(CastExpression node)
		{
			IType toType = GetType(node.Type);
			Bind(node, toType);
		}
		
		override public void LeaveAsExpression(AsExpression node)
		{
			IType target = GetExpressionType(node.Target);
			IType toType = GetType(node.Type);
			
			if (target.IsValueType)
			{
				Error(node, CompilerErrorFactory.CantCastToValueType(node.Target, target.FullName));
			}
			else if (toType.IsValueType)
			{
				Error(node, CompilerErrorFactory.CantCastToValueType(node.Type, toType.FullName));
			}
			else
			{
				Bind(node, toType);
			}
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
			node.ParentNode.Replace(node, memberRef);
			Accept(memberRef);
		}
		
		override public void OnRELiteralExpression(RELiteralExpression node)
		{			
			if (TagService.IsBound(node))
			{
				return;
			}
			
			IType type = TagService.AsTypeInfo(typeof(System.Text.RegularExpressions.Regex));
			Bind(node, type);
			
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
			InternalField tag = new InternalField(TagService, field);
			Bind(field, tag);
			
			AddFieldInitializerToStaticConstructor(0, field);

			MemberReferenceExpression reference = new MemberReferenceExpression(node.LexicalInfo);
			reference.Target = new ReferenceExpression(field.DeclaringType.FullName);
			reference.Name = field.Name;
			Bind(reference, tag);
			
			parent.Replace(node, reference);
		}
		
		override public void OnReferenceExpression(ReferenceExpression node)
		{
			if (TagService.IsBound(node))
			{
				return;
			}
			
			IElement tag = ResolveName(node, node.Name);
			if (null != tag)
			{
				Bind(node, tag);
				
				EnsureRelatedNodeWasVisited(tag);
				
				IMember member = tag as IMember;
				if (null != member)
				{	
					ResolveMemberInfo(node, member);
				}
				else
				{
					if (ElementType.TypeReference == tag.ElementType)
					{
						node.Name = tag.FullName;
					}
				}
			}
			else
			{	
				Error(node);
			}
		}
		
		override public bool EnterMemberReferenceExpression(MemberReferenceExpression node)
		{
			if (TagService.IsBound(node))
			{
				return false;
			}
			return true;
		}
		
		override public void LeaveMemberReferenceExpression(MemberReferenceExpression node)
		{
			IElement tag = GetTag(node.Target);
			ITypedElement typedInfo = tag as ITypedElement;
			if (null != typedInfo)
			{
				tag = typedInfo.BoundType;
			}
			
			if (TagService.IsError(tag))
			{
				Error(node);
			}
			else
			{
				IElement member = ((INamespace)tag).Resolve(node.Name);				
				if (null == member)
				{										
					Error(node, CompilerErrorFactory.MemberNotFound(node, tag.FullName));
				}
				else
				{
					IMember memberInfo = member as IMember;
					if (null != memberInfo)
					{
						if (!CheckTargetContext(node, memberInfo))
						{
							Error(node);
							return;
						}
					}
					
					EnsureRelatedNodeWasVisited(member);
					
					if (ElementType.Property == member.ElementType)
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
				IType returnType = _currentMethodInfo.BoundType;
				if (TagService.IsUnknown(returnType))
				{
					_currentMethodInfo.ReturnExpressions.Add(node.Expression);
				}
				else
				{
					IType expressionType = GetType(node.Expression);
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
			if (!TagService.IsError(iteratorType))
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
			Accept(node.Iterator);
			
			Expression newIterator = ProcessIterator(node.Iterator, node.Declarations, true);
			if (null != newIterator)
			{
				node.Iterator = newIterator;
			}
			
			PushNamespace(new DeclarationsNamespace(CurrentNamespace, TagService, node.Declarations));
			EnterLoop();
			Accept(node.Block);
			LeaveLoop();
			PopNamespace();
		}
		
		override public void OnUnpackStatement(UnpackStatement node)
		{
			Accept(node.Expression);
			
			Expression newIterator = ProcessIterator(node.Expression, node.Declarations, false);
			if (null != newIterator)
			{
				node.Expression = newIterator;
			}
		}
		
		override public void LeaveRaiseStatement(RaiseStatement node)
		{
			if (TagService.StringType == GetType(node.Exception))
			{
				MethodInvocationExpression expression = new MethodInvocationExpression(node.Exception.LexicalInfo);
				expression.Arguments.Add(node.Exception);
				expression.Target = new ReferenceExpression("System.ApplicationException");
				Bind(expression.Target, ApplicationException_StringConstructor);
				Bind(expression, TagService.ApplicationExceptionType);

				node.Exception = expression;				
			}
		}
		
		override public void OnExceptionHandler(ExceptionHandler node)
		{
			if (null == node.Declaration.Type)
			{
				node.Declaration.Type = CreateTypeReference(TagService.ExceptionType);				
			}
			else
			{
				Accept(node.Declaration.Type);
			}
			
			DeclareLocal(node.Declaration, new Local(node.Declaration, true), GetType(node.Declaration.Type));
			PushNamespace(new DeclarationsNamespace(CurrentNamespace, TagService, node.Declaration));
			Accept(node.Block);
			PopNamespace();
		}
		
		void OnIncrementDecrement(UnaryExpression node)
		{			
			IElement tag = GetTag(node.Operand);
			if (CheckLValue(node.Operand, tag))
			{
				ITypedElement typed = (ITypedElement)tag;
				if (!IsNumber(typed.BoundType))
				{
					InvalidOperatorForType(node);					
				}
				else
				{
					TagService.Unbind(node.Operand);
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
					Accept(assign);
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
					IElement tag = ErrorInfo.Default;					
					if (CheckBoolContext(node.Operand))
					{
						tag = TagService.BoolType;
					}
					Bind(node, TagService.BoolType);
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
						Bind(node, GetExpressionType(node.Operand));
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
				IElement info = Resolve(node, reference.Name);					
				if (null == info || IsBuiltin(info))
				{
					Accept(node.Right);
					IType expressionTypeInfo = GetExpressionType(node.Right);				
					DeclareLocal(reference, new Local(reference, false), expressionTypeInfo);
					Bind(node, expressionTypeInfo);
					return false;
				}
			}
			return true;
		}
		
		bool IsBuiltin(IElement tag)
		{
			if (ElementType.Method == tag.ElementType)
			{
				return TagService.BuiltinsType == ((IMethod)tag).DeclaringType;
			}
			return false;
		}
		
		override public void LeaveBinaryExpression(BinaryExpression node)
		{					
			if (TagService.IsUnknown(node.Left) || TagService.IsUnknown(node.Right))
			{
				Bind(node, UnknownInfo.Default);
				return;
			}
			
			if (TagService.IsError(node.Left) || TagService.IsError(node.Right))
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
				
				case BinaryOperatorType.InPlaceSubtract:
				{
					BindInPlaceArithmeticOperator(node);
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
				
				case BinaryOperatorType.InPlaceAdd:
				{
					IElement tag = GetTag(node.Left);
					ElementType tagType = tag.ElementType;
					if (ElementType.Event == tagType ||
						ElementType.Ambiguous == tagType)
					{
						BindInPlaceAddEvent(node);
					}
					else
					{
						BindInPlaceArithmeticOperator(node);
					}
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
			ExpressionCollection args = new ExpressionCollection();
			args.Add(node.Left);
			args.Add(node.Right);
			return GetMostGenericType(args);
		}
		
		void BindBitwiseOperator(BinaryExpression node)
		{
			IType lhs = GetExpressionType(node.Left);
			IType rhs = GetExpressionType(node.Right);
			
			if (IsIntegerNumber(lhs) && IsIntegerNumber(rhs))
			{
				Bind(node, TagService.GetPromotedNumberType(lhs, rhs));
			}
			else
			{
				if (lhs.IsEnum && rhs == lhs)
				{
					Bind(node, lhs);
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
				Bind(node, TagService.BoolType);
			}
			else if (lhs.IsEnum || rhs.IsEnum)
			{
				if (lhs == rhs)
				{
					Bind(node, TagService.BoolType);
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
				Bind(node, GetMostGenericType(node));
			}
			else
			{
				Error(node);
			}
		}
		
		void BindInPlaceAddEvent(BinaryExpression node)
		{
			IElement tag = GetTag(node.Left);
			if (ElementType.Event != tag.ElementType)
			{						
				if (ElementType.Ambiguous == tag.ElementType)
				{
					IList found = ((Ambiguous)tag).Filter(IsPublicEventFilter);
					if (found.Count != 1)
					{
						tag = null;
					}
					else
					{
						tag = (IElement)found[0];
						Bind(node.Left, tag);
					}
				}
			}
			
			IEvent eventInfo = (IEvent)tag;
			ITypedElement expressionInfo = (ITypedElement)GetTag(node.Right);
			CheckDelegateArgument(node.Left, eventInfo, expressionInfo);
			
			Bind(node, TagService.VoidType);
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
			Bind(mie, tag);
			
			return mie;			
		}
		
		MethodInvocationExpression CreateMethodInvocation(IMethod staticMethod, Expression arg)
		{
			MethodInvocationExpression mie = new MethodInvocationExpression(arg.LexicalInfo);
			mie.Target = new ReferenceExpression(staticMethod.FullName);
			mie.Arguments.Add(arg);
			
			Bind(mie.Target, staticMethod);
			Bind(mie, staticMethod);
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
		
		public void OnSpecialFunction(IElement tag, MethodInvocationExpression node)
		{
			BuiltinFunction sf = (BuiltinFunction)tag;
			switch (sf.Function)
			{
				case SpecialFunction.Len:
				{
					if (node.Arguments.Count != 1)
					{						
						Error(CompilerErrorFactory.MethodArgumentCount(node.Target, "len", 1));
					}
					else
					{
						MethodInvocationExpression resultingNode = null;
						
						Expression target = node.Arguments[0];
						IType type = GetExpressionType(target);
						if (TagService.ObjectType == type)
						{
							resultingNode = CreateMethodInvocation(RuntimeServices_Len, target);
						}
						else if (TagService.StringType == type)
						{
							resultingNode = CreateMethodInvocation(target, String_get_Length);
						}
						else if (TagService.ArrayType.IsAssignableFrom(type))
						{
							resultingNode = CreateMethodInvocation(target, Array_get_Length);
						}
						else if (TagService.ICollectionType.IsAssignableFrom(type))
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
					break;
				}
				
				default:
				{
					NotImplemented(node, "SpecialFunction: " + sf.Function);
					break;
				}
			}
		}
		
		override public void OnMethodInvocationExpression(MethodInvocationExpression node)
		{			
			if (TagService.IsBound(node))
			{
				return;
			}
			Accept(node.Target);			
			Accept(node.Arguments);
			
			IElement targetInfo = TagService.GetTag(node.Target);
			if (TagService.IsError(targetInfo) ||
				TagService.IsErrorAny(node.Arguments))
			{
				Error(node);
				return;
			}
			
			if (ElementType.Ambiguous == targetInfo.ElementType)
			{		
				IElement[] tags = ((Ambiguous)targetInfo).Taxonomy;
				targetInfo = ResolveMethodReference(node, node.Arguments, tags, true);				
				if (null == targetInfo)
				{
					return;
				}
				
				if (NodeType.ReferenceExpression == node.Target.NodeType)
				{
					ResolveMemberInfo((ReferenceExpression)node.Target, (IMember)targetInfo);
				}
				Bind(node.Target, targetInfo);
			}	
			
			switch (targetInfo.ElementType)
			{		
				case ElementType.SpecialFunction:
				{
					OnSpecialFunction(targetInfo, node);
					break;
				}
				
				case ElementType.Method:
				{				
					IElement nodeInfo = ErrorInfo.Default;
					
					IMethod targetMethod = (IMethod)targetInfo;
					if (CheckParameters(node, targetMethod, node.Arguments))
					{
						if (node.NamedArguments.Count > 0)
						{
							Error(CompilerErrorFactory.NamedArgumentsNotAllowed(node.NamedArguments[0]));							
						}
						else
						{			
							if (CheckTargetContext(node.Target, targetMethod))
							{
								nodeInfo = targetMethod;
							}
						}
					}
					
					Bind(node, nodeInfo);
					break;
				}
				
				case ElementType.Constructor:
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
							Bind(node, superConstructorInfo);
						}
					}
					break;
				}
				
				case ElementType.TypeReference:
				{					
					IType typeInfo = ((ITypedElement)targetInfo).BoundType;					
					ResolveNamedArguments(node, typeInfo, node.NamedArguments);
					
					IConstructor ctorInfo = FindCorrectConstructor(node, typeInfo, node.Arguments);
					if (null != ctorInfo)
					{
						// rebind the target now we know
						// it is a constructor call
						Bind(node.Target, ctorInfo);
						// expression result type is a new object
						// of type
						Bind(node, typeInfo);
					}
					else
					{
						Error(node);
					}
					break;
				}
				
				case ElementType.Error:
				{
					Error(node);
					break;
				}
				
				default:
				{
					ITypedElement typedInfo = targetInfo as ITypedElement;
					if (null != typedInfo)
					{
						IType type = typedInfo.BoundType;
						if (TagService.ICallableType.IsAssignableFrom(type))
						{
							node.Target = new MemberReferenceExpression(node.Target.LexicalInfo,
												node.Target,
												"Call");
							ArrayLiteralExpression arg = new ArrayLiteralExpression();
							arg.Items.Extend(node.Arguments);
							
							node.Arguments.Clear();
							node.Arguments.Add(arg);
							
							Bind(arg, TagService.ObjectArrayType);
							
							Bind(node.Target, ICallable_Call);
							Bind(node, ICallable_Call);
							return;
						}
						else if (TagService.TypeType == type)
						{
							Expression targetType = node.Target;
							
							node.Target = new ReferenceExpression(targetType.LexicalInfo,
														"System.Activator.CreateInstance");
													
							ArrayLiteralExpression constructorArgs = new ArrayLiteralExpression();
							constructorArgs.Items.Extend(node.Arguments);
							
							node.Arguments.Clear();
							node.Arguments.Add(targetType);
							node.Arguments.Add(constructorArgs);							
							
							Bind(constructorArgs, TagService.ObjectArrayType);
							
							Bind(node.Target, Activator_CreateInstance);
							Bind(node, Activator_CreateInstance);
							return;
						}
					}
					NotImplemented(node, targetInfo.ToString());
					break;
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
			
			Bind(notNode, TagService.BoolType);
			return notNode;
		}
		
		bool CheckIdentifierName(Node node, string name)
		{
			if (TagService.IsPrimitive(name))
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
			IType sliceTargetType = GetExpressionType(slice.Target);
			IType lhsType = GetExpressionType(node.Right);
			
			if (!CheckTypeCompatibility(node.Right, sliceTargetType.GetElementType(), lhsType) ||
				!CheckTypeCompatibility(slice.Begin, TagService.IntType, GetExpressionType(slice.Begin)))
			{
				Error(node);
				return;
			}
			
			Bind(node, sliceTargetType.GetElementType());
		}
		
		void BindAssignmentToSliceProperty(BinaryExpression node)
		{
			SlicingExpression slice = (SlicingExpression)node.Left;
			IElement lhs = GetTag(node.Left);
			IType rhs = GetExpressionType(node.Right);
			IMethod setter = null;

			MethodInvocationExpression mie = new MethodInvocationExpression(node.Left.LexicalInfo);
			mie.Arguments.Add(slice.Begin);
			mie.Arguments.Add(node.Right);			
			
			if (ElementType.Property == lhs.ElementType)
			{
				IMethod setMethod = ((IProperty)lhs).GetSetMethod();
				if (null == setMethod)
				{
					Error(node, CompilerErrorFactory.PropertyIsReadOnly(slice.Target, lhs.FullName));
					return;
				}
				 
				if (2 != setMethod.ParameterCount)
				{
					Error(node, CompilerErrorFactory.MethodArgumentCount(node.Left, setMethod.FullName, 2));
					return;
				}
				else
				{
					if (!CheckTypeCompatibility(slice.Begin, setMethod.GetParameterType(0), GetExpressionType(slice.Begin)) ||
						!CheckTypeCompatibility(node.Right, setMethod.GetParameterType(1), rhs))
					{					
						Error(node);
						return;
					}
					setter = setMethod;
				}
			}
			else if (ElementType.Ambiguous == lhs.ElementType)
			{		
				setter = (IMethod)ResolveMethodReference(node.Left, mie.Arguments, GetSetMethods(((Ambiguous)lhs).Taxonomy), false);
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
				Bind(mie, setter);
				
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
				IElement resultingType = ErrorInfo.Default;
				
				IElement lhs = GetTag(node.Left);
				if (CheckLValue(node.Left, lhs))
				{
					IType lhsType = GetType(node.Left);
					if (CheckTypeCompatibility(node.Right, lhsType, GetExpressionType(node.Right)))
					{
						resultingType = lhsType;
						
						if (ElementType.Property == lhs.ElementType)
						{
							IProperty property = (IProperty)lhs;
							if (IsIndexedProperty(property))
							{
								Error(CompilerErrorFactory.PropertyRequiresParameters(GetMemberAnchor(node.Left), property.FullName));
								resultingType = ErrorInfo.Default;
							}	
						}						
					}
				}
				Bind(node, resultingType);
			}			
		}		
		
		bool CheckIsaArgument(Expression e)
		{
			if (!IsStandaloneTypeReference(e))
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
				Bind(node, TagService.BoolType);
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
				Bind(node, TagService.BoolType);
			}
			else
			{
				Error(node);
			}
		}
		
		bool IsDictionary(IType type)
		{
			return TagService.IDictionaryType.IsAssignableFrom(type);
		}
		
		bool IsList(IType type)
		{
			return TagService.IListType.IsAssignableFrom(type);
		}
		
		bool CanBeString(IType type)
		{
			return TagService.ObjectType == type ||
				TagService.StringType == type;
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
			Accept(assign);
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
				Bind(node, TagService.GetPromotedNumberType(left, right));
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
		
		IElement ResolveName(Node node, string name)
		{
			IElement tag = Resolve(node, name);
			CheckNameResolution(node, name, tag);
			return tag;
		}
		
		bool CheckNameResolution(Node node, string name, IElement tag)
		{
			if (null == tag)
			{
				Error(CompilerErrorFactory.UnknownIdentifier(node, name));			
				return false;
			}
			return true;
		}	
		
		bool IsPublicEvent(IElement tag)
		{
			if (ElementType.Event == tag.ElementType)
			{
				return ((IMember)tag).IsPublic;
			}
			return false;
		}
		
		bool IsPublicFieldPropertyEvent(IElement tag)
		{
			ElementType flags = ElementType.Field|ElementType.Property|ElementType.Event;
			if ((flags & tag.ElementType) > 0)
			{
				IMember member = (IMember)tag;
				return member.IsPublic;
			}
			return false;
		}
		
		IMember ResolvePublicFieldPropertyEvent(Node sourceNode, IType type, string name)
		{
			IElement candidate = type.Resolve(name);
			if (null != candidate)
			{					
				
				if (IsPublicFieldPropertyEvent(candidate))
				{
					return (IMember)candidate;
				}
				else
				{
					if (candidate.ElementType == ElementType.Ambiguous)
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
				Accept(arg.Second);
				
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
				
				IType memberType = member.BoundType;
				ITypedElement expressionInfo = (ITypedElement)GetTag(arg.Second);				
				
				if (member.ElementType == ElementType.Event)
				{
					CheckDelegateArgument(arg.First, member, expressionInfo);
				}
				else
				{						
					IType expressionType = expressionInfo.BoundType;
					CheckTypeCompatibility(arg, memberType, expressionType);					
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
		
		bool CheckDelegateArgument(Node sourceNode, ITypedElement delegateMember, ITypedElement argumentInfo)
		{
			IType delegateType = delegateMember.BoundType;
			if (argumentInfo.ElementType != ElementType.Method ||
					    !CheckDelegateParameterList(delegateType, (IMethod)argumentInfo))
			{
				Error(CompilerErrorFactory.EventArgumentMustBeAMethod(sourceNode, delegateMember.Name, delegateType.FullName));
				return false;
			}
			return true;
		}
		
		bool CheckParameterTypesStrictly(IMethod method, ExpressionCollection args)
		{
			for (int i=0; i<args.Count; ++i)
			{
				IType expressionType = GetExpressionType(args[i]);
				IType parameterType = method.GetParameterType(i);
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
			for (int i=0; i<args.Count; ++i)
			{
				IType expressionType = GetExpressionType(args[i]);
				IType parameterType = method.GetParameterType(i);
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
			if (method.ParameterCount != args.Count)
			{
				Error(CompilerErrorFactory.MethodArgumentCount(sourceNode, method.Name, args.Count));
				return false;
			}	
			
			if (!CheckParameterTypes(method, args))
			{
				Error(CompilerErrorFactory.MethodSignature(sourceNode, GetSignature(method), GetSignature(args)));
			}
			return true;
		}
		
		
		bool CheckDelegateParameterList(IType delegateType, IMethod target)
		{
			IMethod invoke = (IMethod)delegateType.Resolve("Invoke");
			if (null == invoke)
			{
				throw new ArgumentException(string.Format("{0} is not a valid delegate type!", delegateType), "delegateType");
			}			
			
			if (invoke.ParameterCount != target.ParameterCount)
			{				
				return false;
			}
			
			for (int i=0; i<target.ParameterCount; ++i)
			{
				if (invoke.GetParameterType(i) != target.GetParameterType(i))
				{
					return false;
				}
			}
			return true;			
		}
		
		bool IsRuntimeIterator(IType type)
		{
			return  TagService.ObjectType == type ||
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
				if (type.GetArrayRank() != 1)
				{
					Error(CompilerErrorFactory.InvalidArray(iterator));
				}
			}
			else
			{
				IType enumerable = TagService.IEnumerableType;
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
					if (ElementType.TypeReference == GetTag(targetReference).ElementType)
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
			return TagService.AsTypeInfo(expectedType).IsAssignableFrom(actualType);
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
				type == TagService.ShortType ||
				type == TagService.IntType ||
				type == TagService.LongType ||
				type == TagService.ByteType;
		}
		
		bool IsNumber(IType type)
		{
			return
				IsIntegerNumber(type) ||
				type == TagService.DoubleType ||
				type == TagService.SingleType;
		}
		
		bool IsNumber(Expression expression)
		{
			return IsNumber(GetExpressionType(expression));
		}
		
		bool IsString(Expression expression)
		{
			return TagService.StringType == GetExpressionType(expression);
		}
		
		IConstructor FindCorrectConstructor(Node sourceNode, IType typeInfo, ExpressionCollection arguments)
		{
			IConstructor[] constructors = typeInfo.GetConstructors();
			if (constructors.Length > 0)
			{				
				return (IConstructor)ResolveMethodReference(sourceNode, arguments, constructors, true);				
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
				if (0 == constructor.ParameterCount)
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
		
		void EnsureRelatedNodeWasVisited(IElement tag)
		{
			if (tag.ElementType == ElementType.TypeReference)
			{
				tag = ((TypeReference)tag).BoundType;
			}		
			else if (tag.ElementType == ElementType.Ambiguous)
			{
				foreach (IElement item in ((Ambiguous)tag).Taxonomy)
				{
					EnsureRelatedNodeWasVisited(item);
				}
				return;
			}
			
			IInternalElement internalInfo = tag as IInternalElement;
			if (null != internalInfo)
			{
				if (!internalInfo.Visited)
				{
					_context.TraceVerbose("Info {0} needs resolving.", tag.Name);
					
					INamespace saved = _nameResolution.CurrentNamespace;
					try
					{
						TypeMember member = internalInfo.Node as TypeMember;
						if (null != member)
						{
							Accept(member.ParentNode);
						}
						Accept(internalInfo.Node);
					}
					finally
					{
						_nameResolution.Restore(saved);
					}
				}
			}
		}
		
		/*
		int GetInterfaceDepth(IType type)
		{
			IType[] interfaces = type.GetInterfaces();
			int[] depths = new int[interfaces.Length];
			for (int i=0; i<interfaces.Length; ++i)
			{
				depths[i] = interf
			}
		}
		*/
		
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
				
				int count = score.Info.ParameterCount;
				for (int i=0; i<count; ++i)
				{
					score.Score += score.Info.GetParameterType(i).GetTypeDepth();
				}
			}			
		}
		
		IElement ResolveMethodReference(Node node, NodeCollection args, IElement[] tags, bool treatErrors)
		{
			List scores = new List();
			for (int i=0; i<tags.Length; ++i)
			{				
				IElement tag = tags[i];
				IMethod mb = tag as IMethod;
				if (null != mb)
				{			
					if (args.Count == mb.ParameterCount)
					{
						int score = 0;
						for (int argIndex=0; argIndex<args.Count; ++argIndex)
						{
							IType expressionType = GetExpressionType(args.GetNodeAt(argIndex));
							IType parameterType = mb.GetParameterType(argIndex);						
							
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
					Error(node, CompilerErrorFactory.AmbiguousReference(node, tags[0].Name, scores));
				}
			}
			else
			{	
				if (treatErrors)
				{
					IElement tag = tags[0];
					IConstructor constructor = tag as IConstructor;
					if (null != constructor)
					{
						Error(node, CompilerErrorFactory.NoApropriateConstructorFound(node, constructor.DeclaringType.FullName, GetSignature(args)));
					}
					else
					{
						Error(node, CompilerErrorFactory.NoApropriateOverloadFound(node, GetSignature(args), tag.Name));
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
			return ResolveOperator(node, TagService.RuntimeServicesType, operatorName, mie);
		}
		
		IMethod ResolveAmbiguousOperator(IElement[] tags, ExpressionCollection args)
		{
			foreach (IElement tag in tags)
			{
				IMethod method = tag as IMethod;
				if (null != method)
				{
					if (method.IsStatic)
					{
						if (2 == method.ParameterCount)
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
			IElement tag = type.Resolve(operatorName);
			if (null == tag)
			{
				return false;
			}
			
			if (ElementType.Ambiguous == tag.ElementType)
			{	
				tag = ResolveAmbiguousOperator(((Ambiguous)tag).Taxonomy, mie.Arguments);
				if (null == tag)
				{
					return false;
				}
			}
			else if (ElementType.Method == tag.ElementType)
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
			
			Bind(mie, tag);
			Bind(mie.Target, tag);
			
			node.ParentNode.Replace(node, mie);
			
			return true;
		}
		
		Node GetMemberAnchor(Node node)
		{
			MemberReferenceExpression member = node as MemberReferenceExpression;
			return member != null ? member.Target : node;
		}
		
		bool CheckLValue(Node node, IElement tag)
		{
			switch (tag.ElementType)
			{
				case ElementType.Parameter:
				{
					return true;
				}
				
				case ElementType.Local:
				{
					return !((LocalVariable)tag).IsPrivateScope;
				}
				
				case ElementType.Property:
				{
					if (null == ((IProperty)tag).GetSetMethod())
					{
						Error(CompilerErrorFactory.PropertyIsReadOnly(GetMemberAnchor(node), tag.FullName));
						return false;
					}
					return true;
				}
				
				case ElementType.Field:
				{
					return true;
				}
			}
			
			Error(CompilerErrorFactory.LValueExpected(node));
			return false;
		}
		
		bool CheckBoolContext(Expression expression)
		{
			IType type = GetType(expression);
			if (type.IsValueType)
			{
				if (type == TagService.BoolType ||
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
			if (null == _currentMethodInfo.Resolve(d.Name))
			{
				return true;
			}
			Error(CompilerErrorFactory.LocalAlreadyExists(d, d.Name));
			return false;
		}
		
		IType GetExternalEnumeratorItemType(IType iteratorType)
		{
			Type type = ((ExternalType)iteratorType).Type;
			EnumeratorItemTypeAttribute attribute = (EnumeratorItemTypeAttribute)System.Attribute.GetCustomAttribute(type, typeof(EnumeratorItemTypeAttribute));
			if (null != attribute)
			{
				return TagService.AsTypeInfo(attribute.ItemType);
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
			
			IType enumeratorItemTypeAttribute = TagService.AsTypeInfo(typeof(EnumeratorItemTypeAttribute));
			foreach (Boo.Lang.Compiler.Ast.Attribute attribute in internalType.TypeDefinition.Attributes)
			{				
				IConstructor constructor = GetTag(attribute) as IConstructor;
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
				return iteratorType.GetElementType();
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
			return TagService.ObjectType;
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
					Accept(d.Type);
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
						IElement tag = _currentMethodInfo.Resolve(d.Name);
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
		
		protected IType GetExpressionType(Node node)
		{			
			if (IsStandaloneTypeReference(node))
			{
				return TagService.TypeType;
			}
			
			/*
			if (IsStandaloneMethodReference(node))
			{
				return TagService.GetMethodReference(GetTag(node));
			}
			*/
			
			ITypedElement tag = (ITypedElement)GetTag(node);
			if (Array_TypedEnumerableConstructor == tag ||
				Array_TypedCollectionConstructor == tag ||				
				Array_TypedConstructor2 == tag)
			{
				return TagService.AsArrayInfo(GetType(((MethodInvocationExpression)node).Arguments[0]));
			}
			return tag.BoundType;
		}
		
		protected IElement GetOptionalInfo(Node node)
		{
			return TagService.GetOptionalInfo(node);
		}
		
		bool IsStandaloneTypeReference(Node node)
		{
			return node.ParentNode.NodeType != NodeType.MemberReferenceExpression &&
					GetTag(node).ElementType == ElementType.TypeReference;
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
			return TagService.GetSignature(tag);
		}

		void NotImplemented(Node node, string feature)
		{
			throw CompilerErrorFactory.NotImplemented(node, feature);
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
			_context.TraceInfo("{0}: return type for method {1} bound to {2}", method.LexicalInfo, method.Name, tag.BoundType);
		}		
	}
}
