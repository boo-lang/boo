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
		
		InternalMethodInfo _currentMethodInfo;
		
		ArrayList _classes;
		
		int _loopDepth;
		
		/*
		 * Useful method bindings.
		 */		
		IMethodInfo RuntimeServices_Len;
		
		IMethodInfo RuntimeServices_Mid;
		
		IMethodInfo RuntimeServices_GetRange;
		
		IMethodInfo RuntimeServices_GetEnumerable;
		
		IMethodInfo Object_StaticEquals;
		
		IMethodInfo Array_get_Length;
		
		IMethodInfo String_get_Length;
		
		IMethodInfo String_Substring_Int;
		
		IMethodInfo ICollection_get_Count;
		
		IMethodInfo IList_Contains;
		
		IMethodInfo IDictionary_Contains;
		
		IMethodInfo Array_TypedEnumerableConstructor;
		
		IMethodInfo Array_TypedCollectionConstructor;
		
		IMethodInfo Array_TypedConstructor2;
		
		IMethodInfo ICallable_Call;
		
		IMethodInfo Activator_CreateInstance;
		
		IConstructorInfo ApplicationException_StringConstructor;
		
		IConstructorInfo TextReaderEnumerator_Constructor;
		
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
						
			RuntimeServices_Len = (IMethodInfo)InfoService.RuntimeServicesInfo.Resolve("Len");
			RuntimeServices_Mid = (IMethodInfo)InfoService.RuntimeServicesInfo.Resolve("Mid");
			RuntimeServices_GetRange = (IMethodInfo)InfoService.RuntimeServicesInfo.Resolve("GetRange");
			RuntimeServices_GetEnumerable = (IMethodInfo)InfoService.RuntimeServicesInfo.Resolve("GetEnumerable");
			Object_StaticEquals = (IMethodInfo)InfoService.AsInfo(Types.Object.GetMethod("Equals", new Type[] { Types.Object, Types.Object }));
			Array_get_Length = ((IPropertyInfo)InfoService.ArrayTypeInfo.Resolve("Length")).GetGetMethod();
			String_get_Length = ((IPropertyInfo)InfoService.StringTypeInfo.Resolve("Length")).GetGetMethod();
			String_Substring_Int = (IMethodInfo)InfoService.AsInfo(Types.String.GetMethod("Substring", new Type[] { Types.Int }));
			ICollection_get_Count = ((IPropertyInfo)InfoService.ICollectionTypeInfo.Resolve("Count")).GetGetMethod();
			IList_Contains = (IMethodInfo)InfoService.IListTypeInfo.Resolve("Contains");
			IDictionary_Contains = (IMethodInfo)InfoService.IDictionaryTypeInfo.Resolve("Contains");
			Array_TypedEnumerableConstructor = (IMethodInfo)InfoService.AsInfo(Types.Builtins.GetMethod("array", new Type[] { Types.Type, Types.IEnumerable }));
			Array_TypedCollectionConstructor= (IMethodInfo)InfoService.AsInfo(Types.Builtins.GetMethod("array", new Type[] { Types.Type, Types.ICollection }));
			Array_TypedConstructor2 = (IMethodInfo)InfoService.AsInfo(Types.Builtins.GetMethod("array", new Type[] { Types.Type, Types.Int }));
			ICallable_Call = (IMethodInfo)InfoService.ICallableTypeInfo.Resolve("Call");
			Activator_CreateInstance = (IMethodInfo)InfoService.AsInfo(typeof(Activator).GetMethod("CreateInstance", new Type[] { Types.Type, Types.ObjectArray }));
			TextReaderEnumerator_Constructor = (IConstructorInfo)InfoService.AsInfo(typeof(Boo.IO.TextReaderEnumerator).GetConstructor(new Type[] { typeof(System.IO.TextReader) }));
			
			ApplicationException_StringConstructor =
					(IConstructorInfo)InfoService.AsInfo(
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
				ITypeInfo binding = GetBoundType(baseType);
				if (binding.IsInterface)
				{
					ResolveClassInterfaceMembers(node, baseType, binding);
				}
			}
		}
		
		Method CreateAbstractMethod(Node sourceNode, IMethodInfo baseMethod)
		{
			Method method = new Method(sourceNode.LexicalInfo);
			method.Name = baseMethod.Name;
			method.Modifiers = TypeMemberModifiers.Public | TypeMemberModifiers.Abstract;
			for (int i=0; i<baseMethod.ParameterCount; ++i)
			{
				method.Parameters.Add(new ParameterDeclaration("arg" + i, CreateBoundTypeReference(baseMethod.GetParameterType(i))));
			}
			method.ReturnType = CreateBoundTypeReference(baseMethod.ReturnType);
			
			Bind(method, new InternalMethodInfo(InfoService, method));
			return method;
		}
		
		bool CheckPropertyAccessors(IPropertyInfo expected, IPropertyInfo actual)
		{			
			return CheckPropertyAccessor(expected.GetGetMethod(), actual.GetGetMethod()) &&
				CheckPropertyAccessor(expected.GetSetMethod(), actual.GetSetMethod());
		}
		
		bool CheckPropertyAccessor(IMethodInfo expected, IMethodInfo actual)
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
											IPropertyInfo binding)
		{
			TypeMember member = node.Members[binding.Name];
			if (null != member && NodeType.Property == member.NodeType)
			{
				if (binding.BoundType == GetBoundType(member))
				{
					if (CheckPropertyAccessors(binding, (IPropertyInfo)GetInfo(member)))
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
			//node.Members.Add(CreateAbstractProperty(interfaceReference, binding));
			//node.Modifiers |= TypeMemberModifiers.Abstract;
		}
		
		void ResolveClassInterfaceMethod(ClassDefinition node,
										TypeReference interfaceReference,
										IMethodInfo binding)
		{			
			if (binding.IsSpecialName)
			{
				return;
			}
			
			TypeMember member = node.Members[binding.Name];
			if (null != member && NodeType.Method == member.NodeType)
			{							
				Method method = (Method)member;
				if (CheckOverrideSignature((IMethodInfo)GetInfo(method), binding))
				{
					// TODO: check return type here
					if (!method.IsOverride && !method.IsVirtual)
					{
						method.Modifiers |= TypeMemberModifiers.Virtual;
					}
					
					_context.TraceInfo("{0}: Method {1} implements {2}", method.LexicalInfo, method, binding);
					return;
				}
			}
			
			node.Members.Add(CreateAbstractMethod(interfaceReference, binding));
			node.Modifiers |= TypeMemberModifiers.Abstract;
		}
		
		void ResolveClassInterfaceMembers(ClassDefinition node,
											TypeReference interfaceReference,
											ITypeInfo interfaceInfo)
		{			
			foreach (ITypeInfo binding in interfaceInfo.GetInterfaces())
			{
				ResolveClassInterfaceMembers(node, interfaceReference, binding);
			}
			
			foreach (IMemberInfo binding in interfaceInfo.GetMembers())
			{
				switch (binding.InfoType)
				{
					case InfoType.Method:
					{
						ResolveClassInterfaceMethod(node, interfaceReference, (IMethodInfo)binding);
						break;
					}
					
					case InfoType.Property:
					{
						ResolveClassInterfaceProperty(node, interfaceReference, (IPropertyInfo)binding);
						break;
					}
					
					default:
					{
						NotImplemented(interfaceReference, "interface member: " + binding);
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
			PushNamespace((INamespace)InfoService.GetInfo(module));			
			
			Accept(module.Members);
			
			PopNamespace();
		}
		
		void BindBaseInterfaceTypes(InterfaceDefinition node)
		{
			Accept(node.BaseTypes);
			
			foreach (TypeReference baseType in node.BaseTypes)
			{
				ITypeInfo baseInfo = GetBoundType(baseType);
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
			
			ITypeInfo baseClass = null;
			foreach (TypeReference baseType in node.BaseTypes)
			{				
				ITypeInfo baseInfo = GetBoundType(baseType);
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
				node.BaseTypes.Insert(0, CreateBoundTypeReference(InfoService.ObjectTypeInfo)	);
			}
		}
		
		override public void OnEnumDefinition(EnumDefinition node)
		{
			EnumTypeInfo binding = (EnumTypeInfo)GetOptionalInfo(node);
			if (null == binding)
			{
				binding = new EnumTypeInfo(InfoService, (EnumDefinition)node);
			}
			else if (binding.Visited)
			{
				return;
			}
			
			binding.Visited = true;
			
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
			InternalTypeInfo binding = GetInternalTypeInfo(node);
			if (binding.Visited)
			{
				return;
			}
			
			binding.Visited = true;
			BindBaseInterfaceTypes(node);
			
			PushNamespace(binding);
			Accept(node.Attributes);
			Accept(node.Members);
			PopNamespace();
		}
		
		override public void OnClassDefinition(ClassDefinition node)
		{
			InternalTypeInfo binding = GetInternalTypeInfo(node);
			if (binding.Visited)
			{
				return;
			}
			
			_classes.Add(node);
			
			binding.Visited = true;
			BindBaseTypes(node);
			
			PushNamespace(binding);
			Accept(node.Attributes);		
			ProcessFields(node);
			Accept(node.Members);
			PopNamespace();
		}		
		
		override public void OnAttribute(Boo.Lang.Compiler.Ast.Attribute node)
		{
			ITypeInfo binding = InfoService.GetBoundType(node);
			if (null != binding && !InfoService.IsError(binding))
			{			
				Accept(node.Arguments);
				ResolveNamedArguments(node, binding, node.NamedArguments);
				
				IConstructorInfo constructor = FindCorrectConstructor(node, binding, node.Arguments);
				if (null != constructor)
				{
					Bind(node, constructor);
				}
			}
		}
		
		override public void OnProperty(Property node)
		{
			InternalPropertyInfo binding = (InternalPropertyInfo)GetOptionalInfo(node);
			if (null == binding)
			{
				binding = new InternalPropertyInfo(InfoService, node);
				Bind(node, binding);
			}
			else
			{
				if (binding.Visited)
				{
					return;
				}
			}
			
			binding.Visited = true;
			
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
			
			ITypeInfo typeInfo = null;
			if (null != node.Type)
			{
				typeInfo = GetBoundType(node.Type);
			}
			else
			{
				if (null != getter)
				{
					typeInfo = GetBoundType(node.Getter.ReturnType);
				}
				else
				{
					typeInfo = InfoService.ObjectTypeInfo;
				}
				node.Type = CreateBoundTypeReference(typeInfo);
			}
			
			if (null != setter)
			{
				ParameterDeclaration parameter = new ParameterDeclaration();
				parameter.Type = CreateBoundTypeReference(typeInfo);
				parameter.Name = "value";
				setter.Parameters.ExtendWithClones(node.Parameters);
				setter.Parameters.Add(parameter);
				Accept(setter);
				
				setter.Name = "set_" + node.Name;
			}
		}
		
		override public void OnField(Field node)
		{
			InternalFieldInfo binding = (InternalFieldInfo)GetOptionalInfo(node);
			if (null == binding)
			{
				binding = new InternalFieldInfo(InfoService, node);
				Bind(node, binding);
			}
			else
			{
				if (binding.Visited)
				{
					return;
				}
			}
			
			// first time here
			binding.Visited = true;			
			
			Accept(node.Attributes);			
			
			ProcessFieldInitializer(node);			
			
			if (null == node.Type)
			{
				if (null == node.Initializer)
				{
					node.Type = CreateBoundTypeReference(InfoService.ObjectTypeInfo);
				}
				else
				{
					node.Type = CreateBoundTypeReference(GetBoundType(node.Initializer));
				}
			}
			else
			{
				Accept(node.Type);
				
				if (null != node.Initializer)
				{
					CheckTypeCompatibility(node.Initializer, GetBoundType(node.Type), GetBoundType(node.Initializer));
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
				Bind(constructor, new InternalConstructorInfo(InfoService, constructor, true));
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
			InternalFieldInfo binding = (InternalFieldInfo)GetInfo(node);
			
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
			
			Bind(context, binding.DeclaringType);
			
			MemberReferenceExpression member = new MemberReferenceExpression(context, node.Name);
			Bind(member, binding);
			
			// <node.Name> = <node.Initializer>
			stmt.Expression = new BinaryExpression(BinaryOperatorType.Assign,
									member,
									node.Initializer);
			Bind(stmt.Expression, binding.BoundType);
			
			return stmt;
		}
		
		Statement CreateDefaultConstructorCall(Constructor node, InternalConstructorInfo binding)
		{			
			IConstructorInfo defaultConstructor = GetDefaultConstructor(binding.DeclaringType.BaseType);
			
			MethodInvocationExpression call = new MethodInvocationExpression(new SuperLiteralExpression());
			
			Bind(call, defaultConstructor);
			Bind(call.Target, defaultConstructor);
			
			return new ExpressionStatement(call);
		}
		
		override public bool EnterConstructor(Constructor node)
		{			
			InternalConstructorInfo binding = (InternalConstructorInfo)InfoService.GetOptionalInfo(node);
			if (null == binding)
			{
				binding = new InternalConstructorInfo(InfoService, node);
			}
			else
			{
				if (binding.Visited)
				{
					return false;
				}
			}
			
			binding.Visited = true;
			
			Bind(node, binding);
			PushMethodInfo(binding);
			PushNamespace(binding);
			return true;
		}
		
		override public void LeaveConstructor(Constructor node)
		{
			InternalConstructorInfo binding = (InternalConstructorInfo)_currentMethodInfo;
			if (!binding.HasSuperCall && !node.IsStatic)
			{
				node.Body.Statements.Insert(0, CreateDefaultConstructorCall(node, binding));
			}
			PopNamespace();
			PopMethodInfo();
			BindParameterIndexes(node);
		}
		
		override public bool EnterParameterDeclaration(ParameterDeclaration parameter)
		{
			return !InfoService.IsBound(parameter);
		}
		
		override public void LeaveParameterDeclaration(ParameterDeclaration parameter)
		{			
			if (null == parameter.Type)
			{
				parameter.Type = CreateBoundTypeReference(InfoService.ObjectTypeInfo);
			}
			CheckIdentifierName(parameter, parameter.Name);
			Taxonomy.ParameterInfo binding = new Taxonomy.ParameterInfo(parameter, GetBoundType(parameter.Type));
			Bind(parameter, binding);
		}	
		
		override public void OnMethod(Method method)
		{				
			InternalMethodInfo binding = (InternalMethodInfo)GetOptionalInfo(method);
			if (null == binding)
			{
				binding = new InternalMethodInfo(InfoService, method);
				Bind(method, binding);
			}
			else
			{
				if (binding.Visited)
				{
					return;
				}
			}
			
			bool parentIsClass = method.DeclaringType.NodeType == NodeType.ClassDefinition;
			
			binding.Visited = true;
			Accept(method.Attributes);
			Accept(method.Parameters);
			Accept(method.ReturnType);
			Accept(method.ReturnTypeAttributes);
			
			if (method.IsOverride)
			{
				if (parentIsClass)
				{
					ResolveMethodOverride(binding);
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
			
			PushMethodInfo(binding);
			PushNamespace(binding);
			
			Accept(method.Body);
			
			PopNamespace();
			PopMethodInfo();
			BindParameterIndexes(method);			
			
			if (parentIsClass)
			{
				if (InfoService.IsUnknown(binding.BoundType))
				{
					if (CanResolveReturnType(binding))
					{
						ResolveReturnType(binding);
						CheckMethodOverride(binding);
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
						CheckMethodOverride(binding);
					}
				}
			}
		}
		
		override public void OnSuperLiteralExpression(SuperLiteralExpression node)
		{			
			Bind(node, _currentMethodInfo);
			if (InfoType.Constructor != _currentMethodInfo.InfoType)
			{
				_currentMethodInfo.SuperExpressions.Add(node);
			}
		}
		
		/// <summary>
		/// Checks if the specified method overrides any virtual
		/// method in the base class.
		/// </summary>
		void CheckMethodOverride(InternalMethodInfo binding)
		{		
			IMethodInfo baseMethod = FindMethodOverride(binding);
			if (null == baseMethod || binding.BoundType != baseMethod.ReturnType)
			{
				foreach (Expression super in binding.SuperExpressions)
				{
					Error(CompilerErrorFactory.MethodIsNotOverride(super, GetSignature(binding)));
				}
			}
			else
			{
				if (baseMethod.IsVirtual)
				{
					SetOverride(binding, baseMethod);
				}
			}
		}
		
		IMethodInfo FindMethodOverride(InternalMethodInfo binding)
		{
			ITypeInfo baseType = binding.DeclaringType.BaseType;			
			Method method = binding.Method;			
			IInfo baseMethods = baseType.Resolve(binding.Name);
			
			if (null != baseMethods)
			{
				if (InfoType.Method == baseMethods.InfoType)
				{
					IMethodInfo baseMethod = (IMethodInfo)baseMethods;
					if (CheckOverrideSignature(binding, baseMethod))
					{	
						return baseMethod;
					}
				}
				else if (InfoType.Ambiguous == baseMethods.InfoType)
				{
					IInfo[] bindings = ((AmbiguousInfo)baseMethods).Taxonomy;
					IMethodInfo baseMethod = (IMethodInfo)ResolveMethodReference(method, method.Parameters, bindings, false);
					if (null != baseMethod)
					{
						return baseMethod;
					}
				}
			}
			return null;
		}
		
		void ResolveMethodOverride(InternalMethodInfo binding)
		{	
			IMethodInfo baseMethod = FindMethodOverride(binding);
			if (null == baseMethod)
			{
				Error(CompilerErrorFactory.NoMethodToOverride(binding.Method, GetSignature(binding)));
			}
			else
			{
				if (!baseMethod.IsVirtual)
				{
					CantOverrideNonVirtual(binding.Method, baseMethod);
				}
				else
				{
					if (InfoService.IsUnknown(binding.BoundType))
					{
						binding.Method.ReturnType = CreateBoundTypeReference(baseMethod.ReturnType);
					}
					else
					{
						if (baseMethod.ReturnType != binding.BoundType)
						{
							Error(CompilerErrorFactory.InvalidOverrideReturnType(
											binding.Method.ReturnType,
											baseMethod.FullName,
											baseMethod.ReturnType.FullName,
											binding.BoundType.FullName));
						}
					}
					SetOverride(binding, baseMethod);
				}
			}
		}
		
		void CantOverrideNonVirtual(Method method, IMethodInfo baseMethod)
		{
			Error(CompilerErrorFactory.CantOverrideNonVirtual(method, baseMethod.ToString()));
		}
		
		void SetOverride(InternalMethodInfo binding, IMethodInfo baseMethod)
		{
			binding.Override = baseMethod;
			TraceOverride(binding.Method, baseMethod);
			binding.Method.Modifiers |= TypeMemberModifiers.Override;
		}
		
		bool CheckOverrideSignature(IMethodInfo impl, IMethodInfo baseMethod)
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
		
		ITypeInfo GetBaseType(TypeDefinition typeDefinition)
		{
			return ((ITypeInfo)GetInfo(typeDefinition)).BaseType;
		}
		
		bool CanResolveReturnType(InternalMethodInfo binding)
		{
			foreach (Expression rsExpression in binding.ReturnExpressions)
			{
				if (InfoService.IsUnknown(GetBoundType(rsExpression)))
				{
					return false;
				}
			}
			return true;
		}
		
		void ResolveReturnType(InternalMethodInfo binding)
		{				
			Method method = binding.Method;
			ExpressionCollection returnExpressions = binding.ReturnExpressions;
			if (0 == returnExpressions.Count)
			{					
				method.ReturnType = CreateBoundTypeReference(InfoService.VoidTypeInfo);
			}		
			else
			{					
				ITypeInfo type = GetMostGenericType(returnExpressions);
				if (NullInfo.Default == type)
				{
					type = InfoService.ObjectTypeInfo; 
				}
				method.ReturnType = CreateBoundTypeReference(type);
			}
			TraceReturnType(method, binding);	
		}
		
		ITypeInfo GetMostGenericType(ITypeInfo current, ITypeInfo candidate)
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
				return InfoService.GetPromotedNumberType(current, candidate);
			}
			
			ITypeInfo obj = InfoService.ObjectTypeInfo;
			
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
		
		ITypeInfo GetMostGenericType(ExpressionCollection args)
		{
			ITypeInfo type = GetExpressionType(args[0]);
			for (int i=1; i<args.Count; ++i)
			{	
				ITypeInfo newType = GetExpressionType(args[i]);
				
				if (type == newType)
				{
					continue;
				}
				
				type = GetMostGenericType(type, newType);
				if (type == InfoService.ObjectTypeInfo)
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
				((ParameterInfo)GetInfo(parameters[i])).Index = i + delta;
			}
		}
		
		override public void OnArrayTypeReference(ArrayTypeReference node)
		{
			if (InfoService.IsBound(node))
			{
				return;
			}

			Accept(node.ElementType);
			
			ITypeInfo elementType = GetBoundType(node.ElementType);
			if (InfoService.IsError(elementType))
			{
				Bind(node, elementType);
			}
			else
			{
				ITypeInfo arrayType = InfoService.AsArrayInfo(elementType);
				Bind(node, arrayType);
			}
		}
		
		override public void OnSimpleTypeReference(SimpleTypeReference node)
		{
			ResolveSimpleTypeReference(node);
		}
		
		override public void OnBoolLiteralExpression(BoolLiteralExpression node)
		{
			Bind(node, InfoService.BoolTypeInfo);
		}
		
		override public void OnTimeSpanLiteralExpression(TimeSpanLiteralExpression node)
		{
			Bind(node, InfoService.TimeSpanTypeInfo);
		}
		
		override public void OnIntegerLiteralExpression(IntegerLiteralExpression node)
		{
			if (node.IsLong)
			{
				Bind(node, InfoService.LongTypeInfo);
			}
			else
			{
				Bind(node, InfoService.IntTypeInfo);
			}
		}
		
		override public void OnDoubleLiteralExpression(DoubleLiteralExpression node)
		{
			Bind(node, InfoService.DoubleTypeInfo);
		}
		
		override public void OnStringLiteralExpression(StringLiteralExpression node)
		{
			Bind(node, InfoService.StringTypeInfo);
		}
		
		IInfo[] GetSetMethods(IInfo[] bindings)
		{
			ArrayList setMethods = new ArrayList();
			for (int i=0; i<bindings.Length; ++i)
			{
				IPropertyInfo property = bindings[i] as IPropertyInfo;
				if (null != property)
				{
					IMethodInfo setter = property.GetSetMethod();
					if (null != setter)
					{
						setMethods.Add(setter);
					}
				}
			}
			return (IInfo[])setMethods.ToArray(typeof(IInfo));
		}
		
		IInfo[] GetGetMethods(IInfo[] bindings)
		{
			ArrayList getMethods = new ArrayList();
			for (int i=0; i<bindings.Length; ++i)
			{
				IPropertyInfo property = bindings[i] as IPropertyInfo;
				if (null != property)
				{
					IMethodInfo getter = property.GetGetMethod();
					if (null != getter)
					{
						getMethods.Add(getter);
					}
				}
			}
			return (IInfo[])getMethods.ToArray(typeof(IInfo));
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
				if (!CheckTypeCompatibility(node.Begin, InfoService.IntTypeInfo, GetExpressionType(node.Begin)))
				{
					Error(node);
					return false;
				}
			}			
			
			if (null != node.End && OmittedExpression.Default != node.End)
			{
				if (!CheckTypeCompatibility(node.End, InfoService.IntTypeInfo, GetExpressionType(node.End)))
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
			ITypeInfo targetType = GetExpressionType(node.Target);
			if (InfoService.IsError(targetType))
			{
				Error(node);
				return;
			}
			
			IInfo binding = GetInfo(node.Target);
			if (IsIndexedProperty(binding))
			{
				CheckNoComplexSlicing(node);
				SliceMember(node, binding, false);
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
						if (InfoService.StringTypeInfo == targetType)
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
						IInfo member = targetType.GetDefaultMember();
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
		
		bool IsIndexedProperty(IInfo binding)
		{
			return InfoType.Property == binding.InfoType &&
				((IPropertyInfo)binding).GetIndexParameters().Length > 0;
		}
		
		void SliceMember(SlicingExpression node, IInfo member, bool defaultMember)
		{
			if (AstUtil.IsLhsOfAssignment(node))
			{
				// leave it to LeaveBinaryExpression to resolve
				Bind(node, member);
				return;
			}
			
			MethodInvocationExpression mie = new MethodInvocationExpression(node.LexicalInfo);
			mie.Arguments.Add(node.Begin);
			
			IMethodInfo getter = null;
			
			if (InfoType.Ambiguous == member.InfoType)
			{
				IInfo[] bindings = GetGetMethods(((AmbiguousInfo)member).Taxonomy);
				getter = (IMethodInfo)ResolveMethodReference(node, mie.Arguments, bindings, true);						
			}
			else if (InfoType.Property == member.InfoType)
			{
				getter = ((IPropertyInfo)member).GetGetMethod();
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
			Bind(node, InfoService.StringTypeInfo);
		}
		
		override public void LeaveListLiteralExpression(ListLiteralExpression node)
		{			
			Bind(node, InfoService.ListTypeInfo);
		}
		
		override public void OnGeneratorExpression(GeneratorExpression node)
		{
			Accept(node.Iterator);
			
			Expression newIterator = ProcessIterator(node.Iterator, node.Declarations, true);
			if (null != newIterator)
			{
				node.Iterator = newIterator;
			}
			
			PushNamespace(new DeclarationsNamespace(CurrentNamespace, InfoService, node.Declarations));			
			Accept(node.Filter);			
			Accept(node.Expression);
			PopNamespace();
		}
		
		override public void LeaveHashLiteralExpression(HashLiteralExpression node)
		{
			Bind(node, InfoService.HashTypeInfo);
		}
		
		override public void LeaveArrayLiteralExpression(ArrayLiteralExpression node)
		{
			ExpressionCollection items = node.Items;
			if (0 == items.Count)
			{
				Bind(node, InfoService.ObjectArrayInfo);
			}
			else
			{
				Bind(node, InfoService.AsArrayInfo(GetMostGenericType(items)));
			}
		}
		
		override public void LeaveDeclarationStatement(DeclarationStatement node)
		{
			ITypeInfo binding = InfoService.ObjectTypeInfo;
			if (null != node.Declaration.Type)
			{
				binding = GetBoundType(node.Declaration.Type);			
			}			
			
			CheckDeclarationName(node.Declaration);
			
			LocalInfo localInfo = DeclareLocal(node, new Local(node.Declaration, false), binding);
			if (null != node.Initializer)
			{
				CheckTypeCompatibility(node.Initializer, binding, GetExpressionType(node.Initializer));
				
				ReferenceExpression var = new ReferenceExpression(node.Declaration.LexicalInfo);
				var.Name = node.Declaration.Name;
				Bind(var, localInfo);				
				
				BinaryExpression assign = new BinaryExpression(node.LexicalInfo);
				assign.Operator = BinaryOperatorType.Assign;
				assign.Left = var;
				assign.Right = node.Initializer;
				Bind(assign, binding);				
				
				node.ReplaceBy(new ExpressionStatement(assign));
			}
			else
			{
				node.ReplaceBy(null);
			}
		}
		
		override public void LeaveExpressionStatement(ExpressionStatement node)
		{
			if (!HasSideEffect(node.Expression) && !InfoService.IsError(node.Expression))
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
					Bind(node, InfoService.GetInfo(_currentMethodInfo.Method.DeclaringType));
				}
			}
		}
		
		override public void LeaveTypeofExpression(TypeofExpression node)
		{
			if (InfoService.IsError(node.Type))
			{
				Error(node);
			}
			else
			{
				Bind(node, InfoService.TypeTypeInfo);
			}
		}
		
		override public void LeaveCastExpression(CastExpression node)
		{
			ITypeInfo toType = GetBoundType(node.Type);
			Bind(node, toType);
		}
		
		override public void LeaveAsExpression(AsExpression node)
		{
			ITypeInfo target = GetExpressionType(node.Target);
			ITypeInfo toType = GetBoundType(node.Type);
			
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
		
		void ResolveMemberInfo(ReferenceExpression node, IMemberInfo member)
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
			if (InfoService.IsBound(node))
			{
				return;
			}
			
			ITypeInfo type = InfoService.AsTypeInfo(typeof(System.Text.RegularExpressions.Regex));
			Bind(node, type);
			
			if (NodeType.Field != node.ParentNode.NodeType)
			{		
				ReplaceByStaticFieldReference(node, "__re" + _context.AllocIndex() + "__", type);				
			}
		}
		
		void ReplaceByStaticFieldReference(Expression node, string fieldName, ITypeInfo type)
		{
			Node parent = node.ParentNode;
			
			Field field = new Field(node.LexicalInfo);
			field.Name = fieldName;
			field.Type = CreateBoundTypeReference(type);
			field.Modifiers = TypeMemberModifiers.Private|TypeMemberModifiers.Static;
			field.Initializer = node;
			
			_currentMethodInfo.Method.DeclaringType.Members.Add(field);
			InternalFieldInfo binding = new InternalFieldInfo(InfoService, field);
			Bind(field, binding);
			
			AddFieldInitializerToStaticConstructor(0, field);

			MemberReferenceExpression reference = new MemberReferenceExpression(node.LexicalInfo);
			reference.Target = new ReferenceExpression(field.DeclaringType.FullName);
			reference.Name = field.Name;
			Bind(reference, binding);
			
			parent.Replace(node, reference);
		}
		
		override public void OnReferenceExpression(ReferenceExpression node)
		{
			if (InfoService.IsBound(node))
			{
				return;
			}
			
			IInfo binding = ResolveName(node, node.Name);
			if (null != binding)
			{
				Bind(node, binding);
				
				EnsureRelatedNodeWasVisited(binding);
				
				IMemberInfo member = binding as IMemberInfo;
				if (null != member)
				{	
					ResolveMemberInfo(node, member);
				}
				else
				{
					if (InfoType.TypeReference == binding.InfoType)
					{
						node.Name = binding.FullName;
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
			if (InfoService.IsBound(node))
			{
				return false;
			}
			return true;
		}
		
		override public void LeaveMemberReferenceExpression(MemberReferenceExpression node)
		{
			IInfo binding = GetInfo(node.Target);
			ITypedInfo typedInfo = binding as ITypedInfo;
			if (null != typedInfo)
			{
				binding = typedInfo.BoundType;
			}
			
			if (InfoService.IsError(binding))
			{
				Error(node);
			}
			else
			{
				IInfo member = ((INamespace)binding).Resolve(node.Name);				
				if (null == member)
				{										
					Error(node, CompilerErrorFactory.MemberNotFound(node, binding.FullName));
				}
				else
				{
					IMemberInfo memberInfo = member as IMemberInfo;
					if (null != memberInfo)
					{
						if (!CheckTargetContext(node, memberInfo))
						{
							Error(node);
							return;
						}
					}
					
					EnsureRelatedNodeWasVisited(member);
					
					if (InfoType.Property == member.InfoType)
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
								node.ParentNode.Replace(node, CreateMethodInvocation(node.Target, ((IPropertyInfo)member).GetGetMethod()));
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
				ITypeInfo returnType = _currentMethodInfo.BoundType;
				if (InfoService.IsUnknown(returnType))
				{
					_currentMethodInfo.ReturnExpressions.Add(node.Expression);
				}
				else
				{
					ITypeInfo expressionType = GetBoundType(node.Expression);
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
			
			ITypeInfo iteratorType = GetExpressionType(iterator);			
			bool runtimeIterator = false;			
			if (!InfoService.IsError(iteratorType))
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
			
			PushNamespace(new DeclarationsNamespace(CurrentNamespace, InfoService, node.Declarations));
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
			if (InfoService.StringTypeInfo == GetBoundType(node.Exception))
			{
				MethodInvocationExpression expression = new MethodInvocationExpression(node.Exception.LexicalInfo);
				expression.Arguments.Add(node.Exception);
				expression.Target = new ReferenceExpression("System.ApplicationException");
				Bind(expression.Target, ApplicationException_StringConstructor);
				Bind(expression, InfoService.ApplicationExceptionInfo);

				node.Exception = expression;				
			}
		}
		
		override public void OnExceptionHandler(ExceptionHandler node)
		{
			if (null == node.Declaration.Type)
			{
				node.Declaration.Type = CreateBoundTypeReference(InfoService.ExceptionTypeInfo);				
			}
			else
			{
				Accept(node.Declaration.Type);
			}
			
			DeclareLocal(node.Declaration, new Local(node.Declaration, true), GetBoundType(node.Declaration.Type));
			PushNamespace(new DeclarationsNamespace(CurrentNamespace, InfoService, node.Declaration));
			Accept(node.Block);
			PopNamespace();
		}
		
		void OnIncrementDecrement(UnaryExpression node)
		{			
			IInfo binding = GetInfo(node.Operand);
			if (CheckLValue(node.Operand, binding))
			{
				ITypedInfo typed = (ITypedInfo)binding;
				if (!IsNumber(typed.BoundType))
				{
					InvalidOperatorForType(node);					
				}
				else
				{
					InfoService.Unbind(node.Operand);
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
					IInfo binding = ErrorInfo.Default;					
					if (CheckBoolContext(node.Operand))
					{
						binding = InfoService.BoolTypeInfo;
					}
					Bind(node, InfoService.BoolTypeInfo);
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
				IInfo info = Resolve(node, reference.Name);					
				if (null == info || IsBuiltin(info))
				{
					Accept(node.Right);
					ITypeInfo expressionTypeInfo = GetExpressionType(node.Right);				
					DeclareLocal(reference, new Local(reference, false), expressionTypeInfo);
					Bind(node, expressionTypeInfo);
					return false;
				}
			}
			return true;
		}
		
		bool IsBuiltin(IInfo binding)
		{
			if (InfoType.Method == binding.InfoType)
			{
				return InfoService.BuiltinsInfo == ((IMethodInfo)binding).DeclaringType;
			}
			return false;
		}
		
		override public void LeaveBinaryExpression(BinaryExpression node)
		{					
			if (InfoService.IsUnknown(node.Left) || InfoService.IsUnknown(node.Right))
			{
				Bind(node, UnknownInfo.Default);
				return;
			}
			
			if (InfoService.IsError(node.Left) || InfoService.IsError(node.Right))
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
					IInfo binding = GetInfo(node.Left);
					InfoType bindingType = binding.InfoType;
					if (InfoType.Event == bindingType ||
						InfoType.Ambiguous == bindingType)
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
		
		ITypeInfo GetMostGenericType(BinaryExpression node)
		{
			ExpressionCollection args = new ExpressionCollection();
			args.Add(node.Left);
			args.Add(node.Right);
			return GetMostGenericType(args);
		}
		
		void BindBitwiseOperator(BinaryExpression node)
		{
			ITypeInfo lhs = GetExpressionType(node.Left);
			ITypeInfo rhs = GetExpressionType(node.Right);
			
			if (IsIntegerNumber(lhs) && IsIntegerNumber(rhs))
			{
				Bind(node, InfoService.GetPromotedNumberType(lhs, rhs));
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
			ITypeInfo lhs = GetExpressionType(node.Left);
			ITypeInfo rhs = GetExpressionType(node.Right);
			
			if (IsNumber(lhs) && IsNumber(rhs))
			{
				Bind(node, InfoService.BoolTypeInfo);
			}
			else if (lhs.IsEnum || rhs.IsEnum)
			{
				if (lhs == rhs)
				{
					Bind(node, InfoService.BoolTypeInfo);
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
			IInfo binding = GetInfo(node.Left);
			if (InfoType.Event != binding.InfoType)
			{						
				if (InfoType.Ambiguous == binding.InfoType)
				{
					IList found = ((AmbiguousInfo)binding).Filter(IsPublicEventFilter);
					if (found.Count != 1)
					{
						binding = null;
					}
					else
					{
						binding = (IInfo)found[0];
						Bind(node.Left, binding);
					}
				}
			}
			
			IEventInfo eventInfo = (IEventInfo)binding;
			ITypedInfo expressionInfo = (ITypedInfo)GetInfo(node.Right);
			CheckDelegateArgument(node.Left, eventInfo, expressionInfo);
			
			Bind(node, InfoService.VoidTypeInfo);
		}
		
		MethodInvocationExpression CreateMethodInvocation(Expression target, IMethodInfo binding, Expression arg)
		{
			MethodInvocationExpression mie = CreateMethodInvocation(target, binding);
			mie.Arguments.Add(arg);
			return mie;
		}
		
		MethodInvocationExpression CreateMethodInvocation(Expression target, IMethodInfo binding)
		{
			MemberReferenceExpression member = new MemberReferenceExpression(target.LexicalInfo);
			member.Target = target;
			member.Name = binding.Name;
			
			MethodInvocationExpression mie = new MethodInvocationExpression(target.LexicalInfo);
			mie.Target = member;
			Bind(mie.Target, binding);
			Bind(mie, binding);
			
			return mie;			
		}
		
		MethodInvocationExpression CreateMethodInvocation(IMethodInfo staticMethod, Expression arg)
		{
			MethodInvocationExpression mie = new MethodInvocationExpression(arg.LexicalInfo);
			mie.Target = new ReferenceExpression(staticMethod.FullName);
			mie.Arguments.Add(arg);
			
			Bind(mie.Target, staticMethod);
			Bind(mie, staticMethod);
			return mie;
		}
		
		MethodInvocationExpression CreateMethodInvocation(IMethodInfo staticMethod, Expression arg0, Expression arg1)
		{
			MethodInvocationExpression mie = CreateMethodInvocation(staticMethod, arg0);
			mie.Arguments.Add(arg1);
			return mie;
		}
		
		MethodInvocationExpression CreateMethodInvocation(IMethodInfo staticMethod, Expression arg0, Expression arg1, Expression arg2)
		{
			MethodInvocationExpression mie = CreateMethodInvocation(staticMethod, arg0, arg1);
			mie.Arguments.Add(arg2);
			return mie;
		}
		
		public void OnSpecialFunction(IInfo binding, MethodInvocationExpression node)
		{
			SpecialFunctionInfo sf = (SpecialFunctionInfo)binding;
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
						ITypeInfo type = GetExpressionType(target);
						if (InfoService.ObjectTypeInfo == type)
						{
							resultingNode = CreateMethodInvocation(RuntimeServices_Len, target);
						}
						else if (InfoService.StringTypeInfo == type)
						{
							resultingNode = CreateMethodInvocation(target, String_get_Length);
						}
						else if (InfoService.ArrayTypeInfo.IsAssignableFrom(type))
						{
							resultingNode = CreateMethodInvocation(target, Array_get_Length);
						}
						else if (InfoService.ICollectionTypeInfo.IsAssignableFrom(type))
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
			if (InfoService.IsBound(node))
			{
				return;
			}
			Accept(node.Target);			
			Accept(node.Arguments);
			
			IInfo targetInfo = InfoService.GetInfo(node.Target);
			if (InfoService.IsError(targetInfo) ||
				InfoService.IsErrorAny(node.Arguments))
			{
				Error(node);
				return;
			}
			
			if (InfoType.Ambiguous == targetInfo.InfoType)
			{		
				IInfo[] bindings = ((AmbiguousInfo)targetInfo).Taxonomy;
				targetInfo = ResolveMethodReference(node, node.Arguments, bindings, true);				
				if (null == targetInfo)
				{
					return;
				}
				
				if (NodeType.ReferenceExpression == node.Target.NodeType)
				{
					ResolveMemberInfo((ReferenceExpression)node.Target, (IMemberInfo)targetInfo);
				}
				Bind(node.Target, targetInfo);
			}	
			
			switch (targetInfo.InfoType)
			{		
				case InfoType.SpecialFunction:
				{
					OnSpecialFunction(targetInfo, node);
					break;
				}
				
				case InfoType.Method:
				{				
					IInfo nodeInfo = ErrorInfo.Default;
					
					IMethodInfo targetMethod = (IMethodInfo)targetInfo;
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
				
				case InfoType.Constructor:
				{					
					InternalConstructorInfo constructorInfo = targetInfo as InternalConstructorInfo;
					if (null != constructorInfo)
					{
						// super constructor call					
						constructorInfo.HasSuperCall = true;
						
						ITypeInfo baseType = constructorInfo.DeclaringType.BaseType;
						IConstructorInfo superConstructorInfo = FindCorrectConstructor(node, baseType, node.Arguments);
						if (null != superConstructorInfo)
						{
							Bind(node.Target, superConstructorInfo);
							Bind(node, superConstructorInfo);
						}
					}
					break;
				}
				
				case InfoType.TypeReference:
				{					
					ITypeInfo typeInfo = ((ITypedInfo)targetInfo).BoundType;					
					ResolveNamedArguments(node, typeInfo, node.NamedArguments);
					
					IConstructorInfo ctorInfo = FindCorrectConstructor(node, typeInfo, node.Arguments);
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
				
				case InfoType.Error:
				{
					Error(node);
					break;
				}
				
				default:
				{
					ITypedInfo typedInfo = targetInfo as ITypedInfo;
					if (null != typedInfo)
					{
						ITypeInfo type = typedInfo.BoundType;
						if (InfoService.ICallableTypeInfo.IsAssignableFrom(type))
						{
							node.Target = new MemberReferenceExpression(node.Target.LexicalInfo,
												node.Target,
												"Call");
							ArrayLiteralExpression arg = new ArrayLiteralExpression();
							arg.Items.Extend(node.Arguments);
							
							node.Arguments.Clear();
							node.Arguments.Add(arg);
							
							Bind(arg, InfoService.ObjectArrayInfo);
							
							Bind(node.Target, ICallable_Call);
							Bind(node, ICallable_Call);
							return;
						}
						else if (InfoService.TypeTypeInfo == type)
						{
							Expression targetType = node.Target;
							
							node.Target = new ReferenceExpression(targetType.LexicalInfo,
														"System.Activator.CreateInstance");
													
							ArrayLiteralExpression constructorArgs = new ArrayLiteralExpression();
							constructorArgs.Items.Extend(node.Arguments);
							
							node.Arguments.Clear();
							node.Arguments.Add(targetType);
							node.Arguments.Add(constructorArgs);							
							
							Bind(constructorArgs, InfoService.ObjectArrayInfo);
							
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
			
			Bind(notNode, InfoService.BoolTypeInfo);
			return notNode;
		}
		
		bool CheckIdentifierName(Node node, string name)
		{
			if (InfoService.IsPrimitive(name))
			{
				Error(CompilerErrorFactory.CantRedefinePrimitive(node, name));
				return false;
			}
			return true;
		}
		
		bool CheckIsNotValueType(BinaryExpression node, Expression expression)
		{
			ITypeInfo binding = GetExpressionType(expression);
			if (binding.IsValueType)
			{
				Error(CompilerErrorFactory.OperatorCantBeUsedWithValueType(
								expression,
								GetBinaryOperatorText(node.Operator),
								binding.FullName));
								
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
			ITypeInfo sliceTargetType = GetExpressionType(slice.Target);
			ITypeInfo lhsType = GetExpressionType(node.Right);
			
			if (!CheckTypeCompatibility(node.Right, sliceTargetType.GetElementType(), lhsType) ||
				!CheckTypeCompatibility(slice.Begin, InfoService.IntTypeInfo, GetExpressionType(slice.Begin)))
			{
				Error(node);
				return;
			}
			
			Bind(node, sliceTargetType.GetElementType());
		}
		
		void BindAssignmentToSliceProperty(BinaryExpression node)
		{
			SlicingExpression slice = (SlicingExpression)node.Left;
			IInfo lhs = GetInfo(node.Left);
			ITypeInfo rhs = GetExpressionType(node.Right);
			IMethodInfo setter = null;

			MethodInvocationExpression mie = new MethodInvocationExpression(node.Left.LexicalInfo);
			mie.Arguments.Add(slice.Begin);
			mie.Arguments.Add(node.Right);			
			
			if (InfoType.Property == lhs.InfoType)
			{
				IMethodInfo setMethod = ((IPropertyInfo)lhs).GetSetMethod();
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
			else if (InfoType.Ambiguous == lhs.InfoType)
			{		
				setter = (IMethodInfo)ResolveMethodReference(node.Left, mie.Arguments, GetSetMethods(((AmbiguousInfo)lhs).Taxonomy), false);
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
				IInfo resultingType = ErrorInfo.Default;
				
				IInfo lhs = GetInfo(node.Left);
				if (CheckLValue(node.Left, lhs))
				{
					ITypeInfo lhsType = GetBoundType(node.Left);
					if (CheckTypeCompatibility(node.Right, lhsType, GetExpressionType(node.Right)))
					{
						resultingType = lhsType;
						
						if (InfoType.Property == lhs.InfoType)
						{
							IPropertyInfo property = (IPropertyInfo)lhs;
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
				Bind(node, InfoService.BoolTypeInfo);
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
				Bind(node, InfoService.BoolTypeInfo);
			}
			else
			{
				Error(node);
			}
		}
		
		bool IsDictionary(ITypeInfo type)
		{
			return InfoService.IDictionaryTypeInfo.IsAssignableFrom(type);
		}
		
		bool IsList(ITypeInfo type)
		{
			return InfoService.IListTypeInfo.IsAssignableFrom(type);
		}
		
		bool CanBeString(ITypeInfo type)
		{
			return InfoService.ObjectTypeInfo == type ||
				InfoService.StringTypeInfo == type;
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
			ITypeInfo left = GetExpressionType(node.Left);
			ITypeInfo right = GetExpressionType(node.Right);
			if (IsNumber(left) && IsNumber(right))
			{
				Bind(node, InfoService.GetPromotedNumberType(left, right));
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
		
		IInfo ResolveName(Node node, string name)
		{
			IInfo binding = Resolve(node, name);
			CheckNameResolution(node, name, binding);
			return binding;
		}
		
		bool CheckNameResolution(Node node, string name, IInfo binding)
		{
			if (null == binding)
			{
				Error(CompilerErrorFactory.UnknownIdentifier(node, name));			
				return false;
			}
			return true;
		}	
		
		bool IsPublicEvent(IInfo binding)
		{
			if (InfoType.Event == binding.InfoType)
			{
				return ((IMemberInfo)binding).IsPublic;
			}
			return false;
		}
		
		bool IsPublicFieldPropertyEvent(IInfo binding)
		{
			InfoType flags = InfoType.Field|InfoType.Property|InfoType.Event;
			if ((flags & binding.InfoType) > 0)
			{
				IMemberInfo member = (IMemberInfo)binding;
				return member.IsPublic;
			}
			return false;
		}
		
		IMemberInfo ResolvePublicFieldPropertyEvent(Node sourceNode, ITypeInfo type, string name)
		{
			IInfo candidate = type.Resolve(name);
			if (null != candidate)
			{					
				
				if (IsPublicFieldPropertyEvent(candidate))
				{
					return (IMemberInfo)candidate;
				}
				else
				{
					if (candidate.InfoType == InfoType.Ambiguous)
					{
						IList found = ((AmbiguousInfo)candidate).Filter(IsPublicFieldPropertyEventFilter);
						if (found.Count > 0)
						{
							if (found.Count > 1)
							{
								Error(CompilerErrorFactory.AmbiguousReference(sourceNode, name, found));
								return null;
							}
							else
							{
								return (IMemberInfo)found[0];
							}
						}					
					}
				}
			}
			Error(CompilerErrorFactory.NotAPublicFieldOrProperty(sourceNode, type.FullName, name));			
			return null;
		}
		
		void ResolveNamedArguments(Node sourceNode, ITypeInfo typeInfo, ExpressionPairCollection arguments)
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
				IMemberInfo member = ResolvePublicFieldPropertyEvent(name, typeInfo, name.Name);
				if (null == member)				    
				{					
					continue;
				}
				
				Bind(arg.First, member);
				
				ITypeInfo memberType = member.BoundType;
				ITypedInfo expressionInfo = (ITypedInfo)GetInfo(arg.Second);				
				
				if (member.InfoType == InfoType.Event)
				{
					CheckDelegateArgument(arg.First, member, expressionInfo);
				}
				else
				{						
					ITypeInfo expressionType = expressionInfo.BoundType;
					CheckTypeCompatibility(arg, memberType, expressionType);					
				}
			}
		}
		
		bool CheckTypeCompatibility(Node sourceNode, ITypeInfo expectedType, ITypeInfo actualType)
		{
			if (!IsAssignableFrom(expectedType, actualType) &&
				!CanBeReachedByDownCastOrPromotion(expectedType, actualType))
			{
				Error(CompilerErrorFactory.IncompatibleExpressionType(sourceNode, expectedType.FullName, actualType.FullName));
				return false;
			}
			return true;
		}
		
		bool CheckDelegateArgument(Node sourceNode, ITypedInfo delegateMember, ITypedInfo argumentInfo)
		{
			ITypeInfo delegateType = delegateMember.BoundType;
			if (argumentInfo.InfoType != InfoType.Method ||
					    !CheckDelegateParameterList(delegateType, (IMethodInfo)argumentInfo))
			{
				Error(CompilerErrorFactory.EventArgumentMustBeAMethod(sourceNode, delegateMember.Name, delegateType.FullName));
				return false;
			}
			return true;
		}
		
		bool CheckParameterTypesStrictly(IMethodInfo method, ExpressionCollection args)
		{
			for (int i=0; i<args.Count; ++i)
			{
				ITypeInfo expressionType = GetExpressionType(args[i]);
				ITypeInfo parameterType = method.GetParameterType(i);
				if (!IsAssignableFrom(parameterType, expressionType) &&
					!(IsNumber(expressionType) && IsNumber(parameterType)))
				{					
					return false;
				}
			}
			return true;
		}
		
		bool CheckParameterTypes(IMethodInfo method, ExpressionCollection args)
		{
			for (int i=0; i<args.Count; ++i)
			{
				ITypeInfo expressionType = GetExpressionType(args[i]);
				ITypeInfo parameterType = method.GetParameterType(i);
				if (!IsAssignableFrom(parameterType, expressionType) &&
				    !CanBeReachedByDownCastOrPromotion(parameterType, expressionType))
				{
					
					return false;
				}
			}
			return true;
		}
		
		bool CheckParameters(Node sourceNode, IMethodInfo method, ExpressionCollection args)
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
		
		
		bool CheckDelegateParameterList(ITypeInfo delegateType, IMethodInfo target)
		{
			IMethodInfo invoke = (IMethodInfo)delegateType.Resolve("Invoke");
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
		
		bool IsRuntimeIterator(ITypeInfo type)
		{
			return  InfoService.ObjectTypeInfo == type ||
					IsTextReader(type);					
		}
		
		bool IsTextReader(ITypeInfo type)
		{
			return IsAssignableFrom(typeof(System.IO.TextReader), type);
		}
		
		void CheckIterator(Expression iterator, ITypeInfo type, out bool runtimeIterator)
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
				ITypeInfo enumerable = InfoService.IEnumerableTypeInfo;
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
		
		bool CheckTargetContext(Expression targetContext, IMemberInfo member)
		{
			if (!member.IsStatic)					  
			{			
				if (NodeType.MemberReferenceExpression == targetContext.NodeType)
				{				
					Expression targetReference = ((MemberReferenceExpression)targetContext).Target;
					if (InfoType.TypeReference == GetInfo(targetReference).InfoType)
					{						
						Error(CompilerErrorFactory.MemberNeedsInstance(targetContext, member.FullName));
						return false;
					}
				}
			}
			return true;
		}
		
		static bool IsAssignableFrom(ITypeInfo expectedType, ITypeInfo actualType)
		{
			return expectedType.IsAssignableFrom(actualType);
		}
		
		bool IsAssignableFrom(Type expectedType, ITypeInfo actualType)
		{
			return InfoService.AsTypeInfo(expectedType).IsAssignableFrom(actualType);
		}
		
		bool CanBeReachedByDownCastOrPromotion(ITypeInfo expectedType, ITypeInfo actualType)
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
		
		bool IsIntegerNumber(ITypeInfo type)
		{
			return
				type == InfoService.ShortTypeInfo ||
				type == InfoService.IntTypeInfo ||
				type == InfoService.LongTypeInfo ||
				type == InfoService.ByteTypeInfo;
		}
		
		bool IsNumber(ITypeInfo type)
		{
			return
				IsIntegerNumber(type) ||
				type == InfoService.DoubleTypeInfo ||
				type == InfoService.SingleTypeInfo;
		}
		
		bool IsNumber(Expression expression)
		{
			return IsNumber(GetExpressionType(expression));
		}
		
		bool IsString(Expression expression)
		{
			return InfoService.StringTypeInfo == GetExpressionType(expression);
		}
		
		IConstructorInfo FindCorrectConstructor(Node sourceNode, ITypeInfo typeInfo, ExpressionCollection arguments)
		{
			IConstructorInfo[] constructors = typeInfo.GetConstructors();
			if (constructors.Length > 0)
			{				
				return (IConstructorInfo)ResolveMethodReference(sourceNode, arguments, constructors, true);				
			}
			else
			{
				Error(CompilerErrorFactory.NoApropriateConstructorFound(sourceNode, typeInfo.FullName, GetSignature(arguments)));
			}
			return null;
		}
		
		IConstructorInfo GetDefaultConstructor(ITypeInfo type)
		{
			IConstructorInfo[] constructors = type.GetConstructors();
			for (int i=0; i<constructors.Length; ++i)
			{
				IConstructorInfo constructor = constructors[i];
				if (0 == constructor.ParameterCount)
				{
					return constructor;
				}
			}
			return null;
		}
		
		class InfoScore : IComparable
		{
			public IMethodInfo Info;
			public int Score;
			
			public InfoScore(IMethodInfo binding, int score)
			{
				Info = binding;
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
		
		void EnsureRelatedNodeWasVisited(IInfo binding)
		{
			if (binding.InfoType == InfoType.TypeReference)
			{
				binding = ((TypeReferenceInfo)binding).BoundType;
			}		
			else if (binding.InfoType == InfoType.Ambiguous)
			{
				foreach (IInfo item in ((AmbiguousInfo)binding).Taxonomy)
				{
					EnsureRelatedNodeWasVisited(item);
				}
				return;
			}
			
			IInternalInfo internalInfo = binding as IInternalInfo;
			if (null != internalInfo)
			{
				if (!internalInfo.Visited)
				{
					_context.TraceVerbose("Info {0} needs resolving.", binding.Name);
					
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
		int GetInterfaceDepth(ITypeInfo type)
		{
			ITypeInfo[] interfaces = type.GetInterfaces();
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
		
		IInfo ResolveMethodReference(Node node, NodeCollection args, IInfo[] bindings, bool treatErrors)
		{
			List scores = new List();
			for (int i=0; i<bindings.Length; ++i)
			{				
				IInfo binding = bindings[i];
				IMethodInfo mb = binding as IMethodInfo;
				if (null != mb)
				{			
					if (args.Count == mb.ParameterCount)
					{
						int score = 0;
						for (int argIndex=0; argIndex<args.Count; ++argIndex)
						{
							ITypeInfo expressionType = GetExpressionType(args.GetNodeAt(argIndex));
							ITypeInfo parameterType = mb.GetParameterType(argIndex);						
							
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
					Error(node, CompilerErrorFactory.AmbiguousReference(node, bindings[0].Name, scores));
				}
			}
			else
			{	
				if (treatErrors)
				{
					IInfo binding = bindings[0];
					IConstructorInfo constructor = binding as IConstructorInfo;
					if (null != constructor)
					{
						Error(node, CompilerErrorFactory.NoApropriateConstructorFound(node, constructor.DeclaringType.FullName, GetSignature(args)));
					}
					else
					{
						Error(node, CompilerErrorFactory.NoApropriateOverloadFound(node, GetSignature(args), binding.Name));
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
			ITypeInfo lhs = GetExpressionType(node.Left);
			if (ResolveOperator(node, lhs, operatorName, mie))
			{
				return true;
			}
			
			ITypeInfo rhs = GetExpressionType(node.Right);
			if (ResolveOperator(node, rhs, operatorName, mie))
			{
				return true;
			}
			return ResolveOperator(node, InfoService.RuntimeServicesInfo, operatorName, mie);
		}
		
		IMethodInfo ResolveAmbiguousOperator(IInfo[] bindings, ExpressionCollection args)
		{
			foreach (IInfo binding in bindings)
			{
				IMethodInfo method = binding as IMethodInfo;
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
		
		bool ResolveOperator(BinaryExpression node, ITypeInfo type, string operatorName, MethodInvocationExpression mie)
		{
			IInfo binding = type.Resolve(operatorName);
			if (null == binding)
			{
				return false;
			}
			
			if (InfoType.Ambiguous == binding.InfoType)
			{	
				binding = ResolveAmbiguousOperator(((AmbiguousInfo)binding).Taxonomy, mie.Arguments);
				if (null == binding)
				{
					return false;
				}
			}
			else if (InfoType.Method == binding.InfoType)
			{					
				IMethodInfo method = (IMethodInfo)binding;
				
				if (!method.IsStatic || !CheckParameterTypesStrictly(method, mie.Arguments))
				{
					return false;
				}
			}
			else
			{
				return false;
			}
			
			mie.Target = new ReferenceExpression(binding.FullName);
			
			Bind(mie, binding);
			Bind(mie.Target, binding);
			
			node.ParentNode.Replace(node, mie);
			
			return true;
		}
		
		Node GetMemberAnchor(Node node)
		{
			MemberReferenceExpression member = node as MemberReferenceExpression;
			return member != null ? member.Target : node;
		}
		
		bool CheckLValue(Node node, IInfo binding)
		{
			switch (binding.InfoType)
			{
				case InfoType.Parameter:
				{
					return true;
				}
				
				case InfoType.Local:
				{
					return !((LocalInfo)binding).IsPrivateScope;
				}
				
				case InfoType.Property:
				{
					if (null == ((IPropertyInfo)binding).GetSetMethod())
					{
						Error(CompilerErrorFactory.PropertyIsReadOnly(GetMemberAnchor(node), binding.FullName));
						return false;
					}
					return true;
				}
				
				case InfoType.Field:
				{
					return true;
				}
			}
			
			Error(CompilerErrorFactory.LValueExpected(node));
			return false;
		}
		
		bool CheckBoolContext(Expression expression)
		{
			ITypeInfo type = GetBoundType(expression);
			if (type.IsValueType)
			{
				if (type == InfoService.BoolTypeInfo ||
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
		
		LocalInfo DeclareLocal(Node sourceNode, Local local, ITypeInfo localType)
		{			
			LocalInfo binding = new LocalInfo(local, localType);
			Bind(local, binding);
			
			_currentMethodInfo.Method.Locals.Add(local);
			Bind(sourceNode, binding);
			
			return binding;
		}
		
		void PushMethodInfo(InternalMethodInfo binding)
		{
			_methodInfoStack.Push(_currentMethodInfo);
			
			_currentMethodInfo = binding;
		}
		
		void PopMethodInfo()
		{
			_currentMethodInfo = (InternalMethodInfo)_methodInfoStack.Pop();
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
		
		ITypeInfo GetExternalEnumeratorItemType(ITypeInfo iteratorType)
		{
			Type type = ((ExternalTypeInfo)iteratorType).Type;
			EnumeratorItemTypeAttribute attribute = (EnumeratorItemTypeAttribute)System.Attribute.GetCustomAttribute(type, typeof(EnumeratorItemTypeAttribute));
			if (null != attribute)
			{
				return InfoService.AsTypeInfo(attribute.ItemType);
			}
			return null;
		}
		
		ITypeInfo GetEnumeratorItemTypeFromAttribute(ITypeInfo iteratorType)
		{
			InternalTypeInfo internalType = iteratorType as InternalTypeInfo;
			if (null == internalType)
			{
				return GetExternalEnumeratorItemType(iteratorType);
			}
			
			ITypeInfo enumeratorItemTypeAttribute = InfoService.AsTypeInfo(typeof(EnumeratorItemTypeAttribute));
			foreach (Boo.Lang.Compiler.Ast.Attribute attribute in internalType.TypeDefinition.Attributes)
			{				
				IConstructorInfo constructor = GetInfo(attribute) as IConstructorInfo;
				if (null != constructor)
				{
					if (constructor.DeclaringType == enumeratorItemTypeAttribute)
					{
						return GetBoundType(attribute.Arguments[0]);
					}
				}
			}
			return null;
		}
		
		ITypeInfo GetEnumeratorItemType(ITypeInfo iteratorType)
		{
			if (iteratorType.IsArray)
			{
				return iteratorType.GetElementType();
			}
			else
			{
				if (iteratorType.IsClass)
				{
					ITypeInfo enumeratorItemType = GetEnumeratorItemTypeFromAttribute(iteratorType);
					if (null != enumeratorItemType)
					{
						return enumeratorItemType;
					}
				}
			}
			return InfoService.ObjectTypeInfo;
		}
		
		void ProcessDeclarationsForIterator(DeclarationCollection declarations, ITypeInfo iteratorType, bool declarePrivateLocals)
		{
			ITypeInfo defaultDeclType = GetEnumeratorItemType(iteratorType);
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
					CheckTypeCompatibility(d, GetBoundType(d.Type), defaultDeclType);					
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
						IInfo binding = _currentMethodInfo.Resolve(d.Name);
						if (null != binding)
						{
							Bind(d, binding);
							continue;
						}
					}
					
					if (null == d.Type)
					{
						d.Type = CreateBoundTypeReference(defaultDeclType);
					}
					
					DeclareLocal(d, new Local(d, declarePrivateLocals), GetBoundType(d.Type));
				}
			}
		}		
		
		protected ITypeInfo GetExpressionType(Node node)
		{			
			if (IsStandaloneTypeReference(node))
			{
				return InfoService.TypeTypeInfo;
			}
			
			/*
			if (IsStandaloneMethodReference(node))
			{
				return InfoService.GetMethodReference(GetInfo(node));
			}
			*/
			
			ITypedInfo binding = (ITypedInfo)GetInfo(node);
			if (Array_TypedEnumerableConstructor == binding ||
				Array_TypedCollectionConstructor == binding ||				
				Array_TypedConstructor2 == binding)
			{
				return InfoService.AsArrayInfo(GetBoundType(((MethodInvocationExpression)node).Arguments[0]));
			}
			return binding.BoundType;
		}
		
		protected IInfo GetOptionalInfo(Node node)
		{
			return InfoService.GetOptionalInfo(node);
		}
		
		bool IsStandaloneTypeReference(Node node)
		{
			return node.ParentNode.NodeType != NodeType.MemberReferenceExpression &&
					GetInfo(node).InfoType == InfoType.TypeReference;
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
		
		string GetSignature(IMethodInfo binding)
		{
			return InfoService.GetSignature(binding);
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
		
		void TraceOverride(Method method, IMethodInfo baseMethod)
		{
			_context.TraceInfo("{0}: Method '{1}' overrides '{2}'", method.LexicalInfo, method.Name, baseMethod);
		}
		
		void TraceReturnType(Method method, IMethodInfo binding)
		{
			_context.TraceInfo("{0}: return type for method {1} bound to {2}", method.LexicalInfo, method.Name, binding.BoundType);
		}		
	}
}
