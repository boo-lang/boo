#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;
using System.Text;
using System.Collections;
using System.Reflection;
using Boo;
using Boo.Lang.Ast;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Bindings;
using List=Boo.Lang.List;

namespace Boo.Lang.Compiler.Pipeline
{		
	/// <summary>
	/// AST semantic evaluation.
	/// </summary>
	public class SemanticStep : AbstractSwitcherCompilerStep
	{
		NameResolutionSupport _nameResolution = new NameResolutionSupport();
		
		DependencyGraph _pending;
		
		Stack _methodBindingStack;
		
		InternalMethodBinding _currentMethodBinding;
		
		ArrayList _classes;
		
		int _loopDepth;
		
		/*
		 * Useful method bindings.
		 */		
		IMethodBinding RuntimeServices_Len;
		
		IMethodBinding RuntimeServices_Mid;
		
		IMethodBinding RuntimeServices_GetRange;
		
		IMethodBinding Object_StaticEquals;
		
		IMethodBinding Array_get_Length;
		
		IMethodBinding String_get_Length;
		
		IMethodBinding String_Substring_Int;
		
		IMethodBinding ICollection_get_Count;
		
		IMethodBinding IList_Contains;
		
		IMethodBinding IDictionary_Contains;
		
		IMethodBinding Tuple_TypedConstructor1;
		
		IMethodBinding Tuple_TypedConstructor2;
		
		IConstructorBinding ApplicationException_StringConstructor;
		
		/*
		 * Useful filters.
		 */
		BindingFilter IsPublicEventFilter;
		
		BindingFilter IsPublicFieldPropertyEventFilter;
		
		public SemanticStep()
		{
			IsPublicFieldPropertyEventFilter = new BindingFilter(IsPublicFieldPropertyEvent);
			IsPublicEventFilter = new BindingFilter(IsPublicEvent);
		}
		
		override public void Run()
		{					
			_currentMethodBinding = null;
			_methodBindingStack = new Stack();
			_pending = new DependencyGraph(_context);
			_classes = new ArrayList();
			_nameResolution.Initialize(_context);
			_loopDepth = 0;
						
			RuntimeServices_Len = (IMethodBinding)BindingManager.RuntimeServicesBinding.Resolve("Len");
			RuntimeServices_Mid = (IMethodBinding)BindingManager.RuntimeServicesBinding.Resolve("Mid");
			RuntimeServices_GetRange = (IMethodBinding)BindingManager.RuntimeServicesBinding.Resolve("GetRange");
			Object_StaticEquals = (IMethodBinding)BindingManager.AsBinding(Types.Object.GetMethod("Equals", new Type[] { Types.Object, Types.Object }));
			Array_get_Length = ((IPropertyBinding)BindingManager.ArrayTypeBinding.Resolve("Length")).GetGetMethod();
			String_get_Length = ((IPropertyBinding)BindingManager.StringTypeBinding.Resolve("Length")).GetGetMethod();
			String_Substring_Int = (IMethodBinding)BindingManager.AsBinding(Types.String.GetMethod("Substring", new Type[] { Types.Int }));
			ICollection_get_Count = ((IPropertyBinding)BindingManager.ICollectionTypeBinding.Resolve("Count")).GetGetMethod();
			IList_Contains = (IMethodBinding)BindingManager.IListTypeBinding.Resolve("Contains");
			IDictionary_Contains = (IMethodBinding)BindingManager.IDictionaryTypeBinding.Resolve("Contains");
			Tuple_TypedConstructor1 = (IMethodBinding)BindingManager.AsBinding(Types.Builtins.GetMethod("tuple", new Type[] { Types.Type, Types.IEnumerable }));
			Tuple_TypedConstructor2 = (IMethodBinding)BindingManager.AsBinding(Types.Builtins.GetMethod("tuple", new Type[] { Types.Type, Types.Int }));
			
			ApplicationException_StringConstructor =
					(IConstructorBinding)BindingManager.AsBinding(
						Types.ApplicationException.GetConstructor(new Type[] { typeof(string) }));
			
			Switch(CompileUnit);
			
			ResolveDependencyGraph();
			
			if (0 == Errors.Count)
			{
				ResolveClassInterfaceMethods();
			}
		}		
		
		void ResolveClassInterfaceMethods()
		{
			foreach (ClassDefinition node in _classes)
			{
				ResolveClassInterfaceMethods(node);
			}
		}
		
		void ResolveClassInterfaceMethods(ClassDefinition node)
		{
			foreach (TypeReference baseType in node.BaseTypes)
			{
				ITypeBinding binding = GetBoundType(baseType);
				if (binding.IsInterface)
				{
					ResolveClassInterfaceMethods(node, baseType, binding);
				}
			}
		}
		
		Method CreateAbstractMethod(Node sourceNode, IMethodBinding baseMethod)
		{
			Method method = new Method(sourceNode.LexicalInfo);
			method.Name = baseMethod.Name;
			method.Modifiers = TypeMemberModifiers.Public | TypeMemberModifiers.Abstract;
			for (int i=0; i<baseMethod.ParameterCount; ++i)
			{
				method.Parameters.Add(new ParameterDeclaration("arg" + i, CreateBoundTypeReference(baseMethod.GetParameterType(i))));
			}
			method.ReturnType = CreateBoundTypeReference(baseMethod.ReturnType);
			
			Bind(method, new InternalMethodBinding(BindingManager, method));
			return method;
		}
		
		void ResolveClassInterfaceMethods(ClassDefinition node,
											TypeReference interfaceReference,
											ITypeBinding interfaceBinding)
		{
			foreach (IMemberBinding binding in interfaceBinding.GetMembers())
			{
				switch (binding.BindingType)
				{
					case BindingType.Method:
					{
						IMethodBinding interfaceMethod = (IMethodBinding)binding;
						TypeMember member = node.Members[interfaceMethod.Name];
						if (null != member && NodeType.Method == member.NodeType)
						{							
							Method method = (Method)member;
							if (CheckOverrideSignature((IMethodBinding)GetBinding(method), interfaceMethod))
							{
								// TODO: check return type here
								if (!method.IsOverride && !method.IsVirtual)
								{
									method.Modifiers |= TypeMemberModifiers.Virtual;
								}
								
								_context.TraceInfo("{0}: Method {1} implements {2}", method.LexicalInfo, method, interfaceMethod);
								continue;
							}
						}
						
						node.Members.Add(CreateAbstractMethod(interfaceReference, interfaceMethod));
						node.Modifiers |= TypeMemberModifiers.Abstract;
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
		
		void ResolveDependencyGraph()
		{
			int iterations = _pending.Resolve(this);
			_context.TraceInfo("Type inference concluded in {0} iteration(s).", iterations);
			foreach (Node node in _pending)
			{
				if (NodeType.Method == node.NodeType)
				{					
					if (BindingManager.IsUnknown(((Method)node).ReturnType))
					{
						Error(CompilerErrorFactory.CouldNotInferReturnType(node, GetSignature((IMethodBinding)GetBinding(node))));
					}
				}
			}
		}
		
		override public void Dispose()
		{
			base.Dispose();
			
			_currentMethodBinding = null;
			_methodBindingStack = null;
			_pending = null;
			_nameResolution.Dispose();
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
		
		void PushNamespace(INamespace ns)
		{
			_nameResolution.PushNamespace(ns);
		}
		
		INamespace CurrentNamespace
		{
			get
			{
				return _nameResolution.CurrentNamespace;
			}
		}
		
		void PopNamespace()
		{
			_nameResolution.PopNamespace();
		}
		
		IBinding Resolve(Node sourceNode, string name, BindingType bindings)
		{
			return _nameResolution.Resolve(sourceNode, name, bindings);
		}
		
		IBinding Resolve(Node sourceNode, string name)
		{
			return _nameResolution.Resolve(sourceNode, name);
		}
		
		bool IsQualifiedName(string name)
		{
			return name.IndexOf('.') > 0;
		}
		
		IBinding ResolveQualifiedName(Node sourceNode, string name)
		{
			return _nameResolution.ResolveQualifiedName(sourceNode, name);
		}
		
		override public void OnModule(Boo.Lang.Ast.Module module)
		{				
			PushNamespace((INamespace)BindingManager.GetBinding(module));			
			
			Switch(module.Members);
			
			PopNamespace();
		}
		
		void BindBaseTypes(ClassDefinition node)
		{
			Switch(node.BaseTypes);
			
			ITypeBinding baseClass = null;
			foreach (TypeReference baseType in node.BaseTypes)
			{				
				ITypeBinding baseBinding = GetBoundType(baseType);
				EnsureRelatedNodeWasVisited(baseBinding);
				if (baseBinding.IsClass)
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
						baseClass = baseBinding;
					}
				}
			}
			
			if (null == baseClass)
			{
				node.BaseTypes.Insert(0, CreateBoundTypeReference(BindingManager.ObjectTypeBinding)	);
			}
		}
		
		override public void OnEnumDefinition(EnumDefinition node)
		{
			EnumTypeBinding binding = (EnumTypeBinding)GetOptionalBinding(node);
			if (null == binding)
			{
				binding = new EnumTypeBinding(BindingManager, (EnumDefinition)node);
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
			
			Switch(node.Attributes);
			
			long lastValue = 0;
			foreach (EnumMember member in node.Members)
			{
				if (null == member.Initializer)
				{
					member.Initializer = new IntegerLiteralExpression(lastValue);
				}
				Switch(member.Attributes);
				Switch(member.Initializer);
				lastValue = member.Initializer.Value + 1;
			}
		}
		
		override public void OnClassDefinition(ClassDefinition node)
		{
			InternalTypeBinding binding = (InternalTypeBinding)BindingManager.GetOptionalBinding(node);
			if (null == binding)
			{
				binding = new InternalTypeBinding(BindingManager, node);
				Bind(node, binding);
			}
			else
			{
				if (binding.Visited)
				{
					return;
				}
			}
			
			_classes.Add(node);
			
			binding.Visited = true;
			BindBaseTypes(node);
			
			PushNamespace(binding);
			Switch(node.Attributes);
			Switch(node.Members);
			PopNamespace();
		}
		
		override public void OnAttribute(Boo.Lang.Ast.Attribute node)
		{
			ITypeBinding binding = BindingManager.GetBoundType(node);
			if (null != binding && !BindingManager.IsError(binding))
			{			
				Switch(node.Arguments);
				ResolveNamedArguments(node, binding, node.NamedArguments);
				
				IConstructorBinding constructor = FindCorrectConstructor(node, binding, node.Arguments);
				if (null != constructor)
				{
					Bind(node, constructor);
				}
			}
		}
		
		override public void OnProperty(Property node)
		{
			InternalPropertyBinding binding = (InternalPropertyBinding)GetOptionalBinding(node);
			if (null == binding)
			{
				binding = new InternalPropertyBinding(BindingManager, node);
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
			
			Switch(node.Attributes);			
			Switch(node.Type);
			
			Switch(node.Parameters);
			if (null != getter)
			{
				getter.Name = "get_" + node.Name;
				getter.Parameters.AddClones(node.Parameters);
				Switch(getter);
			}
			
			ITypeBinding typeBinding = null;
			if (null != node.Type)
			{
				typeBinding = GetBoundType(node.Type);
			}
			else
			{
				if (null != getter)
				{
					typeBinding = GetBoundType(node.Getter.ReturnType);
				}
				else
				{
					typeBinding = BindingManager.ObjectTypeBinding;
				}
				node.Type = CreateBoundTypeReference(typeBinding);
			}
			
			if (null != setter)
			{
				ParameterDeclaration parameter = new ParameterDeclaration();
				parameter.Type = CreateBoundTypeReference(typeBinding);
				parameter.Name = "value";
				setter.Parameters.AddClones(node.Parameters);
				setter.Parameters.Add(parameter);
				Switch(setter);
				
				setter.Name = "set_" + node.Name;
			}
		}
		
		override public bool EnterField(Field node)
		{
			InternalFieldBinding binding = (InternalFieldBinding)GetOptionalBinding(node);
			if (null == binding)
			{
				binding = new InternalFieldBinding(BindingManager, node);
				Bind(node, binding);
			}
			else
			{
				if (binding.Visited)
				{
					return false;
				}
			}
			
			// first time here
			binding.Visited = true;
			return true;			
		}
		
		override public void LeaveField(Field node)
		{	
			if (null == node.Type)
			{
				if (null == node.Initializer)
				{
					node.Type = CreateBoundTypeReference(BindingManager.ObjectTypeBinding);
				}
				else
				{
					node.Type = CreateBoundTypeReference(GetBoundType(node.Initializer));
				}
			}
			else
			{
				if (null != node.Initializer)
				{
					CheckTypeCompatibility(node.Initializer, GetBoundType(node.Type), GetBoundType(node.Initializer));
				}
			}
			
			if (null != node.Initializer)
			{
				AddFieldInitializerToConstructors(node);
			}
		}
		
		void AddFieldInitializerToConstructors(Field node)
		{
			foreach (TypeMember member in node.DeclaringType.Members)
			{
				if (NodeType.Constructor == member.NodeType)
				{
					Constructor constructor = (Constructor)member;
					
					// find the StatementGroup with name="FieldInitializers"
					// if not found, create
					// append the statement at the end of the group
					constructor.Body.Statements.Insert(0, CreateFieldAssignment(node));
				}
			}
			node.Initializer = null;
		}
		
		Statement CreateFieldAssignment(Field node)
		{
			ExpressionStatement stmt = new ExpressionStatement(node.Initializer.LexicalInfo);
			
			// self.<node.Name> = <node.Initializer>
			stmt.Expression = new BinaryExpression(BinaryOperatorType.Assign,
									new MemberReferenceExpression(new SelfLiteralExpression(),
																	node.Name),
									node.Initializer);
			return stmt;
		}
		
		Statement CreateDefaultConstructorCall(Constructor node, InternalConstructorBinding binding)
		{			
			IConstructorBinding defaultConstructor = GetDefaultConstructor(binding.DeclaringType.BaseType);
			
			MethodInvocationExpression call = new MethodInvocationExpression(new SuperLiteralExpression());
			
			Bind(call, defaultConstructor);
			Bind(call.Target, defaultConstructor);
			
			return new ExpressionStatement(call);
		}
		
		override public bool EnterConstructor(Constructor node)
		{			
			InternalConstructorBinding binding = (InternalConstructorBinding)BindingManager.GetOptionalBinding(node);
			if (null == binding)
			{
				binding = new InternalConstructorBinding(BindingManager, node);
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
			PushMethodBinding(binding);
			PushNamespace(binding);
			return true;
		}
		
		override public void LeaveConstructor(Constructor node)
		{
			InternalConstructorBinding binding = (InternalConstructorBinding)_currentMethodBinding;
			if (!binding.HasSuperCall)
			{
				node.Body.Statements.Insert(0, CreateDefaultConstructorCall(node, binding));
			}
			PopNamespace();
			PopMethodBinding();
			BindParameterIndexes(node);
		}
		
		override public bool EnterParameterDeclaration(ParameterDeclaration parameter)
		{
			return !BindingManager.IsBound(parameter);
		}
		
		override public void LeaveParameterDeclaration(ParameterDeclaration parameter)
		{			
			if (null == parameter.Type)
			{
				parameter.Type = CreateBoundTypeReference(BindingManager.ObjectTypeBinding);
			}
			CheckIdentifierName(parameter, parameter.Name);
			Bindings.ParameterBinding binding = new Bindings.ParameterBinding(parameter, GetBoundType(parameter.Type));
			Bind(parameter, binding);
		}	
		
		override public void OnMethod(Method method)
		{				
			InternalMethodBinding binding = (InternalMethodBinding)GetOptionalBinding(method);
			if (null == binding)
			{
				binding = new InternalMethodBinding(BindingManager, method);
				Bind(method, binding);
			}
			else
			{
				if (binding.Visited)
				{
					return;
				}
			}
			
			binding.Visited = true;
			Switch(method.Attributes);
			Switch(method.Parameters);
			Switch(method.ReturnType);
			Switch(method.ReturnTypeAttributes);
			
			PushMethodBinding(binding);
			PushNamespace(binding);
			
			Switch(method.Body);
			
			PopNamespace();
			PopMethodBinding();
			BindParameterIndexes(method);			
			
			if (BindingManager.IsUnknown(binding.BoundType))
			{
				if (CanResolveReturnType(binding))
				{
					ResolveResolvableReturnType(binding);
					ResolveMethodOverride(binding);
				}
				else
				{
					_pending.Add(method, new ReturnTypeResolver(binding));
				}
			}
			else
			{
				ResolveMethodOverride(binding);
			}
		}
		
		override public void OnSuperLiteralExpression(SuperLiteralExpression node)
		{			
			Bind(node, _currentMethodBinding);
			if (BindingType.Constructor != _currentMethodBinding.BindingType)
			{
				_currentMethodBinding.SuperExpressions.Add(node);
			}
		}
		
		internal void ResolveMethodOverride(InternalMethodBinding binding)
		{
			ITypeBinding baseType = binding.DeclaringType.BaseType;
			
			Method method = binding.Method;
			
			IBinding baseMethods = baseType.Resolve(binding.Name);
			
			bool found = false;
			
			if (null != baseMethods)
			{
				if (BindingType.Method == baseMethods.BindingType)
				{
					IMethodBinding baseMethod = (IMethodBinding)baseMethods;
					if (CheckOverrideSignature(binding, baseMethod))
					{	
						found = true;
						if (baseMethod.IsVirtual)
						{
							SetOverride(binding, method, baseMethod);
						}
						else
						{
							if (method.IsOverride)
							{
								CantOverrideNonVirtual(method, baseMethod);
							}
						}
					}
				}
				else if (BindingType.Ambiguous == baseMethods.BindingType)
				{
					IBinding[] bindings = ((AmbiguousBinding)baseMethods).Bindings;
					IMethodBinding baseMethod = (IMethodBinding)ResolveMethodReference(method, method.Parameters, bindings, false);
					if (null != baseMethod)
					{
						found = true;
						if (baseMethod.IsVirtual)
						{
							SetOverride(binding, method, baseMethod);
						}
						else
						{
							if (method.IsOverride)
							{
								CantOverrideNonVirtual(method, baseMethod);
							}
						}
					}
				}
			}
			
			if (!found)
			{
				if (method.IsOverride)
				{
					Error(CompilerErrorFactory.NoMethodToOverride(method, GetSignature(binding)));
				}
				
				foreach (Expression super in binding.SuperExpressions)
				{
					Error(CompilerErrorFactory.MethodIsNotOverride(super, GetSignature(binding)));
				}
			}
		}
		
		void CantOverrideNonVirtual(Method method, IMethodBinding baseMethod)
		{
			Error(CompilerErrorFactory.CantOverrideNonVirtual(method, baseMethod.ToString()));
		}
		
		void SetOverride(InternalMethodBinding binding, Method method, IMethodBinding baseMethod)
		{
			binding.Override = baseMethod;
			TraceOverride(method, baseMethod);
			method.Modifiers |= TypeMemberModifiers.Override;
		}
		
		bool CheckOverrideSignature(IMethodBinding impl, IMethodBinding baseMethod)
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
		
		ITypeBinding GetBaseType(TypeDefinition typeDefinition)
		{
			return ((ITypeBinding)GetBinding(typeDefinition)).BaseType;
		}
		
		bool CanResolveReturnType(InternalMethodBinding binding)
		{
			foreach (Expression rsExpression in binding.ReturnExpressions)
			{
				if (BindingManager.IsUnknown(GetBoundType(rsExpression)))
				{
					return false;
				}
			}
			return true;
		}
		
		internal void ResolveReturnType(InternalMethodBinding binding)
		{			
			ITypeBinding type = binding.BoundType;
			if (BindingManager.ObjectTypeBinding == type)
			{
				// can go no further than System.Object
				return;
			}
			
			Method method = binding.Method;
			foreach (Expression rsExpression in binding.ReturnExpressions)
			{
				ITypeBinding newType = GetBoundType(rsExpression);
				if (BindingManager.IsUnknown(type))
				{
					type = newType;
					continue;
				}
				
				if (BindingManager.IsUnknown(newType) || type == newType)
				{
					continue;
				}
				
				type = GetMostGenericType(type, newType);
				if (type == BindingManager.ObjectTypeBinding)
				{
					break;
				}
			}
			
			if (binding.BoundType != type)
			{
				method.ReturnType = CreateBoundTypeReference(type);
				TraceReturnType(method, binding);
			}
		}
		
		void ResolveResolvableReturnType(InternalMethodBinding binding)
		{				
			Method method = binding.Method;
			ExpressionCollection returnExpressions = binding.ReturnExpressions;
			if (0 == returnExpressions.Count)
			{					
				method.ReturnType = CreateBoundTypeReference(BindingManager.VoidTypeBinding);
			}		
			else
			{					
				method.ReturnType = CreateBoundTypeReference(GetMostGenericType(returnExpressions));
			}
			TraceReturnType(method, binding);	
		}
		
		ITypeBinding GetMostGenericType(ITypeBinding current, ITypeBinding candidate)
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
				return BindingManager.GetPromotedNumberType(current, candidate);
			}
			
			return BindingManager.ObjectTypeBinding;
		}
		
		ITypeBinding GetMostGenericType(ExpressionCollection args)
		{
			ITypeBinding type = GetExpressionType(args[0]);
			for (int i=1; i<args.Count; ++i)
			{	
				ITypeBinding newType = GetExpressionType(args[i]);
				
				if (type == newType)
				{
					continue;
				}
				
				type = GetMostGenericType(type, newType);
				if (type == BindingManager.ObjectTypeBinding)
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
				((ParameterBinding)GetBinding(parameters[i])).Index = i + delta;
			}
		}
		
		override public void OnTupleTypeReference(TupleTypeReference node)
		{
			if (BindingManager.IsBound(node))
			{
				return;
			}

			Switch(node.ElementType);
			
			ITypeBinding elementType = GetBoundType(node.ElementType);
			if (BindingManager.IsError(elementType))
			{
				Bind(node, elementType);
			}
			else
			{
				ITypeBinding tupleType = BindingManager.AsTupleBinding(elementType);
				Bind(node, tupleType);
			}
		}
		
		override public void OnSimpleTypeReference(SimpleTypeReference node)
		{
			if (BindingManager.IsBound(node))
			{
				return;
			}
			
			IBinding info = null;
			if (IsQualifiedName(node.Name))
			{
				info = ResolveQualifiedName(node, node.Name);
			}
			else
			{
				info = Resolve(node, node.Name, BindingType.TypeReference);
			}
			
			if (null == info || BindingType.TypeReference != info.BindingType)
			{
				Error(CompilerErrorFactory.NameNotType(node, node.Name));
				Error(node);
			}
			else
			{
				node.Name = info.Name;
				Bind(node, info);
			}
		}
		
		override public void OnBoolLiteralExpression(BoolLiteralExpression node)
		{
			Bind(node, BindingManager.BoolTypeBinding);
		}
		
		override public void OnTimeSpanLiteralExpression(TimeSpanLiteralExpression node)
		{
			Bind(node, BindingManager.TimeSpanTypeBinding);
		}
		
		override public void OnIntegerLiteralExpression(IntegerLiteralExpression node)
		{
			if (node.IsLong)
			{
				Bind(node, BindingManager.LongTypeBinding);
			}
			else
			{
				Bind(node, BindingManager.IntTypeBinding);
			}
		}
		
		override public void OnDoubleLiteralExpression(DoubleLiteralExpression node)
		{
			Bind(node, BindingManager.DoubleTypeBinding);
		}
		
		override public void OnStringLiteralExpression(StringLiteralExpression node)
		{
			Bind(node, BindingManager.StringTypeBinding);
		}
		
		IBinding[] GetSetMethods(IBinding[] bindings)
		{
			ArrayList setMethods = new ArrayList();
			for (int i=0; i<bindings.Length; ++i)
			{
				IPropertyBinding property = bindings[i] as IPropertyBinding;
				if (null != property)
				{
					IMethodBinding setter = property.GetSetMethod();
					if (null != setter)
					{
						setMethods.Add(setter);
					}
				}
			}
			return (IBinding[])setMethods.ToArray(typeof(IBinding));
		}
		
		IBinding[] GetGetMethods(IBinding[] bindings)
		{
			ArrayList getMethods = new ArrayList();
			for (int i=0; i<bindings.Length; ++i)
			{
				IPropertyBinding property = bindings[i] as IPropertyBinding;
				if (null != property)
				{
					IMethodBinding getter = property.GetGetMethod();
					if (null != getter)
					{
						getMethods.Add(getter);
					}
				}
			}
			return (IBinding[])getMethods.ToArray(typeof(IBinding));
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
		
		IntegerLiteralExpression CreateIntegerLiteral(long value)
		{
			IntegerLiteralExpression expression = new IntegerLiteralExpression(value);
			Switch(expression);
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
				if (!CheckTypeCompatibility(node.Begin, BindingManager.IntTypeBinding, GetExpressionType(node.Begin)))
				{
					Error(node);
					return false;
				}
			}			
			
			if (null != node.End && OmittedExpression.Default != node.End)
			{
				if (!CheckTypeCompatibility(node.End, BindingManager.IntTypeBinding, GetExpressionType(node.End)))
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
			ITypeBinding targetType = GetExpressionType(node.Target);
			if (BindingManager.IsError(targetType))
			{
				Error(node);
				return;
			}
			
			IBinding binding = GetBinding(node.Target);
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
						if (BindingManager.StringTypeBinding == targetType)
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
						IBinding member = targetType.GetDefaultMember();
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
		
		bool IsIndexedProperty(IBinding binding)
		{
			return BindingType.Property == binding.BindingType &&
				((IPropertyBinding)binding).GetIndexParameters().Length > 0;
		}
		
		void SliceMember(SlicingExpression node, IBinding member, bool defaultMember)
		{
			if (AstUtil.IsLhsOfAssignment(node))
			{
				// leave it to LeaveBinaryExpression to resolve
				Bind(node, member);
				return;
			}
			
			MethodInvocationExpression mie = new MethodInvocationExpression(node.LexicalInfo);
			mie.Arguments.Add(node.Begin);
			
			IMethodBinding getter = null;
			
			if (BindingType.Ambiguous == member.BindingType)
			{
				IBinding[] bindings = GetGetMethods(((AmbiguousBinding)member).Bindings);
				getter = (IMethodBinding)ResolveMethodReference(node, mie.Arguments, bindings, true);						
			}
			else if (BindingType.Property == member.BindingType)
			{
				getter = ((IPropertyBinding)member).GetGetMethod();
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
		
		override public void LeaveStringFormattingExpression(StringFormattingExpression node)
		{
			Bind(node, BindingManager.StringTypeBinding);
		}
		
		override public void LeaveListLiteralExpression(ListLiteralExpression node)
		{			
			Bind(node, BindingManager.ListTypeBinding);
		}
		
		override public void LeaveHashLiteralExpression(HashLiteralExpression node)
		{
			Bind(node, BindingManager.HashTypeBinding);
		}
		
		override public void LeaveTupleLiteralExpression(TupleLiteralExpression node)
		{
			ExpressionCollection items = node.Items;
			if (0 == items.Count)
			{
				Bind(node, BindingManager.ObjectTupleBinding);
			}
			else
			{
				Bind(node, BindingManager.AsTupleBinding(GetMostGenericType(items)));
			}
		}
		
		override public void LeaveDeclaration(Declaration node)
		{
			CheckIdentifierName(node, node.Name);
		}
		
		override public void LeaveDeclarationStatement(DeclarationStatement node)
		{
			ITypeBinding binding = BindingManager.ObjectTypeBinding;
			if (null != node.Declaration.Type)
			{
				binding = GetBoundType(node.Declaration.Type);			
			}
			
			LocalBinding localBinding = DeclareLocal(node, new Local(node.Declaration, false), binding);
			if (null != node.Initializer)
			{
				ReferenceExpression var = new ReferenceExpression(node.Declaration.LexicalInfo);
				var.Name = node.Declaration.Name;
				Bind(var, localBinding);				
				
				BinaryExpression assign = new BinaryExpression(node.LexicalInfo);
				assign.Operator = BinaryOperatorType.Assign;
				assign.Left = var;
				assign.Right = node.Initializer;
				Bind(assign, GetBinding(assign.Right));				
				
				node.ReplaceBy(new ExpressionStatement(assign));
			}
			else
			{
				node.ReplaceBy(null);
			}
		}
		
		override public void LeaveExpressionStatement(ExpressionStatement node)
		{
			if (!HasSideEffect(node.Expression) && !BindingManager.IsError(node.Expression))
			{
				Error(CompilerErrorFactory.ExpressionStatementMustHaveSideEffect(node));
			}
		}
		
		override public void OnNullLiteralExpression(NullLiteralExpression node)
		{
			Bind(node, NullBinding.Default);
		}
		
		override public void OnSelfLiteralExpression(SelfLiteralExpression node)
		{
			if (_currentMethodBinding.IsStatic)
			{
				Error(node, CompilerErrorFactory.ObjectRequired(node));
			}
			else
			{
				Bind(node, BindingManager.GetBinding(_currentMethodBinding.Method.DeclaringType));
			}
		}
		
		override public void LeaveAsExpression(AsExpression node)
		{
			ITypeBinding target = GetExpressionType(node.Target);
			ITypeBinding toType = GetBoundType(node.Type);
			
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
		
		override public void OnReferenceExpression(ReferenceExpression node)
		{
			if (BindingManager.IsBound(node))
			{
				return;
			}
			
			IBinding binding = ResolveName(node, node.Name);
			if (null != binding)
			{
				Bind(node, binding);
				
				EnsureRelatedNodeWasVisited(binding);
				
				IMemberBinding member = binding as IMemberBinding;
				if (null != member)
				{	
					if (!member.IsStatic)
					{
						MemberReferenceExpression memberRef = new MemberReferenceExpression(node.LexicalInfo);
						memberRef.Target = new SelfLiteralExpression(node.LexicalInfo);
						memberRef.Name = node.Name;
						
						node.ParentNode.Replace(node, memberRef);
						
						Switch(memberRef);
					}
					else
					{
						node.Name = member.FullName;
					}
				}
				else
				{
					if (BindingType.TypeReference == binding.BindingType)
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
			if (BindingManager.IsBound(node))
			{
				return false;
			}
			return true;
		}
		
		override public void LeaveMemberReferenceExpression(MemberReferenceExpression node)
		{
			IBinding nodeBinding = ErrorBinding.Default;
			
			IBinding binding = GetBinding(node.Target);
			ITypedBinding typedBinding = binding as ITypedBinding;
			if (null != typedBinding)
			{
				binding = typedBinding.BoundType;
			}
			
			if (!BindingManager.IsError(binding))
			{
				IBinding member = ((INamespace)binding).Resolve(node.Name);				
				if (null == member)
				{										
					Error(CompilerErrorFactory.MemberNotFound(node, binding.FullName));
				}
				else
				{
					nodeBinding = member;					
					EnsureRelatedNodeWasVisited(member);
					
					if (BindingType.Property == member.BindingType)
					{
						if (!AstUtil.IsLhsOfAssignment(node) &&
							!(
								AstUtil.IsTargetOfSlicing(node) &&
								IsIndexedProperty(member)
							 ))
						{
							node.ParentNode.Replace(node, CreateMethodInvocation(node.Target, ((IPropertyBinding)member).GetGetMethod()));
							return;
						}
					}
				}
			}
			
			Bind(node, nodeBinding);
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
				ITypeBinding returnType = _currentMethodBinding.BoundType;
				if (BindingManager.IsUnknown(returnType))
				{
					_currentMethodBinding.ReturnExpressions.Add(node.Expression);
				}
				else
				{
					ITypeBinding expressionType = GetBoundType(node.Expression);
					CheckTypeCompatibility(node.Expression, returnType, expressionType);
				}
			}
		}
		
		override public void OnForStatement(ForStatement node)
		{
			Switch(node.Iterator);
			
			ITypeBinding iteratorType = GetExpressionType(node.Iterator);
			CheckIterator(node.Iterator, iteratorType);
			ProcessDeclarationsForIterator(node.Declarations, iteratorType, true);
			
			PushNamespace(new DeclarationsNamespace(CurrentNamespace, BindingManager, node.Declarations));
			EnterLoop();
			Switch(node.Block);
			LeaveLoop();
			PopNamespace();
		}
		
		override public void OnUnpackStatement(UnpackStatement node)
		{
			Switch(node.Expression);
			ProcessDeclarationsForIterator(node.Declarations, GetBoundType(node.Expression), false);
		}
		
		override public void LeaveRaiseStatement(RaiseStatement node)
		{
			if (BindingManager.StringTypeBinding == GetBoundType(node.Exception))
			{
				MethodInvocationExpression expression = new MethodInvocationExpression(node.Exception.LexicalInfo);
				expression.Arguments.Add(node.Exception);
				expression.Target = new ReferenceExpression("System.ApplicationException");
				Bind(expression.Target, ApplicationException_StringConstructor);
				Bind(expression, BindingManager.ApplicationExceptionBinding);

				node.Exception = expression;				
			}
		}
		
		override public void OnExceptionHandler(ExceptionHandler node)
		{
			if (null == node.Declaration.Type)
			{
				node.Declaration.Type = CreateBoundTypeReference(BindingManager.ExceptionTypeBinding);				
			}
			else
			{
				Switch(node.Declaration.Type);
			}
			
			DeclareLocal(node.Declaration, new Local(node.Declaration, true), GetBoundType(node.Declaration.Type));
			PushNamespace(new DeclarationsNamespace(CurrentNamespace, BindingManager, node.Declaration));
			Switch(node.Block);
			PopNamespace();
		}
		
		void OnIncrementDecrement(UnaryExpression node)
		{
			IBinding result = ErrorBinding.Default;
			IBinding binding = GetBinding(node.Operand);
			if (CheckLValue(node.Operand, binding))
			{
				ITypedBinding typed = (ITypedBinding)binding;
				if (!IsNumber(typed.BoundType))
				{
					Error(CompilerErrorFactory.InvalidOperatorForType(node,
							GetUnaryOperatorText(node.Operator),
							typed.BoundType.FullName));
				}
				else
				{
					result = typed.BoundType;
				}
			}
			Bind(node, result);			
		}
		
		override public void LeaveUnaryExpression(UnaryExpression node)
		{
			switch (node.Operator)
			{
				case UnaryOperatorType.LogicalNot:					
				{
					IBinding binding = ErrorBinding.Default;					
					if (CheckBoolContext(node.Operand))
					{
						binding = BindingManager.BoolTypeBinding;
					}
					Bind(node, BindingManager.BoolTypeBinding);
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
				IBinding info = Resolve(node, reference.Name);					
				if (null == info)
				{
					Switch(node.Right);
					ITypeBinding expressionTypeInfo = GetExpressionType(node.Right);				
					DeclareLocal(reference, new Local(reference, false), expressionTypeInfo);
					Bind(node, expressionTypeInfo);
					return false;
				}
			}
			return true;
		}
		
		override public void LeaveBinaryExpression(BinaryExpression node)
		{
			if (BindingManager.IsUnknown(node.Left) || BindingManager.IsUnknown(node.Right))
			{
				BindingManager.Bind(node, UnknownBinding.Default);
				_pending.Add(node, new BinaryExpressionResolver(node));
			}
			else
			{
				ResolveBinaryExpression(node);
			}
		}
		
		public void ResolveBinaryExpression(BinaryExpression node)
		{			
			if (BindingManager.IsError(node.Left) || BindingManager.IsError(node.Right))
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
					IBinding binding = GetBinding(node.Left);
					BindingType bindingType = binding.BindingType;
					if (BindingType.Event == bindingType ||
						BindingType.Ambiguous == bindingType)
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
		
		ITypeBinding GetMostGenericType(BinaryExpression node)
		{
			ExpressionCollection args = new ExpressionCollection();
			args.Add(node.Left);
			args.Add(node.Right);
			return GetMostGenericType(args);
		}
		
		void BindBitwiseOperator(BinaryExpression node)
		{
			ITypeBinding lhs = GetExpressionType(node.Left);
			ITypeBinding rhs = GetExpressionType(node.Right);
			
			if (IsIntegerNumber(lhs) && IsIntegerNumber(rhs))
			{
				Bind(node, BindingManager.GetPromotedNumberType(lhs, rhs));
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
			ITypeBinding lhs = GetExpressionType(node.Left);
			ITypeBinding rhs = GetExpressionType(node.Right);
			
			if (IsNumber(lhs) && IsNumber(rhs))
			{
				BindingManager.Bind(node, BindingManager.BoolTypeBinding);
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
				BindingManager.Bind(node, GetMostGenericType(node));
			}
			else
			{
				Error(node);
			}
		}
		
		void BindInPlaceAddEvent(BinaryExpression node)
		{
			IBinding binding = GetBinding(node.Left);
			if (BindingType.Event != binding.BindingType)
			{						
				if (BindingType.Ambiguous == binding.BindingType)
				{
					IList found = ((AmbiguousBinding)binding).Filter(IsPublicEventFilter);
					if (found.Count != 1)
					{
						binding = null;
					}
					else
					{
						binding = (IBinding)found[0];
						Bind(node.Left, binding);
					}
				}
			}
			
			IEventBinding eventBinding = (IEventBinding)binding;
			ITypedBinding expressionBinding = (ITypedBinding)GetBinding(node.Right);
			if (CheckDelegateArgument(node.Left, eventBinding, expressionBinding))
			{						
			}
			
			Bind(node, BindingManager.VoidTypeBinding);
		}
		
		MethodInvocationExpression CreateMethodInvocation(Expression target, IMethodBinding binding, Expression arg)
		{
			MethodInvocationExpression mie = CreateMethodInvocation(target, binding);
			mie.Arguments.Add(arg);
			return mie;
		}
		
		MethodInvocationExpression CreateMethodInvocation(Expression target, IMethodBinding binding)
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
		
		MethodInvocationExpression CreateMethodInvocation(IMethodBinding staticMethod, Expression arg)
		{
			MethodInvocationExpression mie = new MethodInvocationExpression(arg.LexicalInfo);
			mie.Target = new ReferenceExpression(staticMethod.FullName);
			mie.Arguments.Add(arg);
			
			Bind(mie.Target, staticMethod);
			Bind(mie, staticMethod);
			return mie;
		}
		
		MethodInvocationExpression CreateMethodInvocation(IMethodBinding staticMethod, Expression arg0, Expression arg1)
		{
			MethodInvocationExpression mie = CreateMethodInvocation(staticMethod, arg0);
			mie.Arguments.Add(arg1);
			return mie;
		}
		
		MethodInvocationExpression CreateMethodInvocation(IMethodBinding staticMethod, Expression arg0, Expression arg1, Expression arg2)
		{
			MethodInvocationExpression mie = CreateMethodInvocation(staticMethod, arg0, arg1);
			mie.Arguments.Add(arg2);
			return mie;
		}
		
		public void OnSpecialFunction(IBinding binding, MethodInvocationExpression node)
		{
			SpecialFunctionBinding sf = (SpecialFunctionBinding)binding;
			switch (sf.Function)
			{
				case SpecialFunction.Typeof:
				{
					if (node.Arguments.Count != 1 ||
						GetBinding(node.Arguments[0]).BindingType != BindingType.TypeReference)
					{
						Error(CompilerErrorFactory.InvalidTypeof(node));
					}
					else
					{
						Bind(node, BindingManager.AsTypeBinding(Types.Type));
					}
					break;
				}
				
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
						ITypeBinding type = GetExpressionType(target);
						if (BindingManager.ObjectTypeBinding == type)
						{
							resultingNode = CreateMethodInvocation(RuntimeServices_Len, target);
						}
						else if (BindingManager.StringTypeBinding == type)
						{
							resultingNode = CreateMethodInvocation(target, String_get_Length);
						}
						else if (BindingManager.ArrayTypeBinding.IsAssignableFrom(type))
						{
							resultingNode = CreateMethodInvocation(target, Array_get_Length);
						}
						else if (BindingManager.ICollectionTypeBinding.IsAssignableFrom(type))
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
			}
		}
		
		override public void OnMethodInvocationExpression(MethodInvocationExpression node)
		{			
			if (BindingManager.IsBound(node))
			{
				return;
			}
			Switch(node.Target);			
			Switch(node.Arguments);
			
			IBinding targetBinding = BindingManager.GetBinding(node.Target);
			if (BindingManager.IsError(targetBinding) ||
				BindingManager.IsErrorAny(node.Arguments))
			{
				Error(node);
				return;
			}
			
			if (BindingType.Ambiguous == targetBinding.BindingType)
			{		
				IBinding[] bindings = ((AmbiguousBinding)targetBinding).Bindings;
				targetBinding = ResolveMethodReference(node, node.Arguments, bindings, true);				
				if (null == targetBinding)
				{
					return;
				}
				
				if (NodeType.ReferenceExpression == node.Target.NodeType)
				{
					((ReferenceExpression)node.Target).Name = targetBinding.FullName;
				}
				Bind(node.Target, targetBinding);
			}	
			
			switch (targetBinding.BindingType)
			{		
				case BindingType.SpecialFunction:
				{
					OnSpecialFunction(targetBinding, node);
					break;
				}
				
				case BindingType.Method:
				{				
					IBinding nodeBinding = ErrorBinding.Default;
					
					IMethodBinding targetMethod = (IMethodBinding)targetBinding;
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
								nodeBinding = targetMethod;
							}
						}
					}
					
					Bind(node, nodeBinding);
					break;
				}
				
				case BindingType.Constructor:
				{					
					InternalConstructorBinding constructorBinding = targetBinding as InternalConstructorBinding;
					if (null != constructorBinding)
					{
						// super constructor call					
						constructorBinding.HasSuperCall = true;
						
						ITypeBinding baseType = constructorBinding.DeclaringType.BaseType;
						IConstructorBinding superConstructorBinding = FindCorrectConstructor(node, baseType, node.Arguments);
						if (null != superConstructorBinding)
						{
							Bind(node.Target, superConstructorBinding);
							Bind(node, superConstructorBinding);
						}
					}
					break;
				}
				
				case BindingType.TypeReference:
				{					
					ITypeBinding typeBinding = ((ITypedBinding)targetBinding).BoundType;					
					ResolveNamedArguments(node, typeBinding, node.NamedArguments);
					
					IConstructorBinding ctorBinding = FindCorrectConstructor(node, typeBinding, node.Arguments);
					if (null != ctorBinding)
					{
						// rebind the target now we know
						// it is a constructor call
						Bind(node.Target, ctorBinding);
						// expression result type is a new object
						// of type
						Bind(node, typeBinding);
					}
					else
					{
						Error(node);
					}
					break;
				}
				
				case BindingType.Error:
				{
					Error(node);
					break;
				}
				
				default:
				{
					NotImplemented(node, targetBinding.ToString());
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
			
			Bind(notNode, BindingManager.BoolTypeBinding);
			return notNode;
		}
		
		bool CheckIdentifierName(Node node, string name)
		{
			if (BindingManager.IsPrimitive(name))
			{
				Error(CompilerErrorFactory.CantRedefinePrimitive(node, name));
				return false;
			}
			return true;
		}
		
		bool CheckIsNotValueType(BinaryExpression node, Expression expression)
		{
			ITypeBinding binding = GetExpressionType(expression);
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
			ITypeBinding sliceTargetType = GetExpressionType(slice.Target);
			ITypeBinding lhsType = GetExpressionType(node.Right);
			
			if (!CheckTypeCompatibility(node.Right, sliceTargetType.GetElementType(), lhsType) ||
				!CheckTypeCompatibility(slice.Begin, BindingManager.IntTypeBinding, GetExpressionType(slice.Begin)))
			{
				Error(node);
				return;
			}
			
			BindingManager.Bind(node, sliceTargetType.GetElementType());
		}
		
		void BindAssignmentToSliceProperty(BinaryExpression node)
		{
			SlicingExpression slice = (SlicingExpression)node.Left;
			IBinding lhs = GetBinding(node.Left);
			ITypeBinding rhs = GetExpressionType(node.Right);
			IMethodBinding setter = null;

			MethodInvocationExpression mie = new MethodInvocationExpression(node.Left.LexicalInfo);
			mie.Arguments.Add(slice.Begin);
			mie.Arguments.Add(node.Right);			
			
			if (BindingType.Property == lhs.BindingType)
			{
				IMethodBinding setMethod = ((IPropertyBinding)lhs).GetSetMethod();
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
			else if (BindingType.Ambiguous == lhs.BindingType)
			{		
				setter = (IMethodBinding)ResolveMethodReference(node.Left, mie.Arguments, GetSetMethods(((AmbiguousBinding)lhs).Bindings), false);
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
				IBinding resultingType = ErrorBinding.Default;
				
				IBinding lhs = GetBinding(node.Left);
				if (CheckLValue(node.Left, lhs))
				{
					ITypeBinding lhsType = GetBoundType(node.Left);
					if (CheckTypeCompatibility(node.Right, lhsType, GetExpressionType(node.Right)))
					{
						if (BindingType.Property == lhs.BindingType)
						{
							Expression target = ((MemberReferenceExpression)node.Left).Target;
							IMethodBinding setter = ((IPropertyBinding)lhs).GetSetMethod();
							if (null == setter)
							{
								Error(node, CompilerErrorFactory.PropertyIsReadOnly(target, lhs.FullName));
							}
							else
							{
								
								node.ParentNode.Replace(node, CreateMethodInvocation(target, setter, node.Right));
							}
							return;
						}
						resultingType = lhsType;							
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
				Bind(node, BindingManager.BoolTypeBinding);
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
				if (BinaryOperatorType.ReferenceInequality == node.Operator)
				{
					Negate(node, BinaryOperatorType.ReferenceEquality);
				}
				else
				{
					Bind(node, BindingManager.BoolTypeBinding);
				}
			}
			else
			{
				Error(node);
			}
		}
		
		bool IsDictionary(ITypeBinding type)
		{
			return BindingManager.IDictionaryTypeBinding.IsAssignableFrom(type);
		}
		
		bool IsList(ITypeBinding type)
		{
			return BindingManager.IListTypeBinding.IsAssignableFrom(type);
		}
		
		bool CanBeString(ITypeBinding type)
		{
			return BindingManager.ObjectTypeBinding == type ||
				BindingManager.StringTypeBinding == type;
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
			Switch(assign);
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
			ITypeBinding left = GetExpressionType(node.Left);
			ITypeBinding right = GetExpressionType(node.Right);
			if (IsNumber(left) && IsNumber(right))
			{
				Bind(node, BindingManager.GetPromotedNumberType(left, right));
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
			return Boo.Lang.Ast.Visitors.BooPrinterVisitor.GetBinaryOperatorText(op);
		}
		static string GetUnaryOperatorText(UnaryOperatorType op)
		{
			return Boo.Lang.Ast.Visitors.BooPrinterVisitor.GetUnaryOperatorText(op);
		}
		
		IBinding ResolveName(Node node, string name)
		{
			IBinding binding = Resolve(node, name);
			CheckNameResolution(node, name, binding);
			return binding;
		}
		
		bool CheckNameResolution(Node node, string name, IBinding binding)
		{
			if (null == binding)
			{
				Error(CompilerErrorFactory.UnknownIdentifier(node, name));			
				return false;
			}
			return true;
		}	
		
		bool IsPublicEvent(IBinding binding)
		{
			if (BindingType.Event == binding.BindingType)
			{
				return ((IMemberBinding)binding).IsPublic;
			}
			return false;
		}
		
		bool IsPublicFieldPropertyEvent(IBinding binding)
		{
			BindingType flags = BindingType.Field|BindingType.Property|BindingType.Event;
			if ((flags & binding.BindingType) > 0)
			{
				IMemberBinding member = (IMemberBinding)binding;
				return member.IsPublic;
			}
			return false;
		}
		
		IMemberBinding ResolvePublicFieldPropertyEvent(Node sourceNode, ITypeBinding type, string name)
		{
			IBinding candidate = type.Resolve(name);
			if (null != candidate)
			{					
				
				if (IsPublicFieldPropertyEvent(candidate))
				{
					return (IMemberBinding)candidate;
				}
				else
				{
					if (candidate.BindingType == BindingType.Ambiguous)
					{
						IList found = ((AmbiguousBinding)candidate).Filter(IsPublicFieldPropertyEventFilter);
						if (found.Count > 0)
						{
							if (found.Count > 1)
							{
								Error(CompilerErrorFactory.AmbiguousReference(sourceNode, name, found));
								return null;
							}
							else
							{
								return (IMemberBinding)found[0];
							}
						}					
					}
				}
			}
			Error(CompilerErrorFactory.NotAPublicFieldOrProperty(sourceNode, type.FullName, name));			
			return null;
		}
		
		void ResolveNamedArguments(Node sourceNode, ITypeBinding typeBinding, ExpressionPairCollection arguments)
		{
			foreach (ExpressionPair arg in arguments)
			{			
				Switch(arg.Second);
				
				if (NodeType.ReferenceExpression != arg.First.NodeType)
				{
					Error(CompilerErrorFactory.NamedParameterMustBeIdentifier(arg));
					continue;				
				}
				
				ReferenceExpression name = (ReferenceExpression)arg.First;
				IMemberBinding member = ResolvePublicFieldPropertyEvent(name, typeBinding, name.Name);
				if (null == member)				    
				{					
					continue;
				}
				
				Bind(arg.First, member);
				
				ITypeBinding memberType = member.BoundType;
				ITypedBinding expressionBinding = (ITypedBinding)GetBinding(arg.Second);				
				
				if (member.BindingType == BindingType.Event)
				{
					CheckDelegateArgument(arg.First, member, expressionBinding);
				}
				else
				{						
					ITypeBinding expressionType = expressionBinding.BoundType;
					CheckTypeCompatibility(arg, memberType, expressionType);					
				}
			}
		}
		
		bool CheckTypeCompatibility(Node sourceNode, ITypeBinding expectedType, ITypeBinding actualType)
		{
			if (!IsAssignableFrom(expectedType, actualType) &&
				!CanBeReachedByDownCastOrPromotion(expectedType, actualType))
			{
				Error(CompilerErrorFactory.IncompatibleExpressionType(sourceNode, expectedType.FullName, actualType.FullName));
				return false;
			}
			return true;
		}
		
		bool CheckDelegateArgument(Node sourceNode, ITypedBinding delegateMember, ITypedBinding argumentBinding)
		{
			ITypeBinding delegateType = delegateMember.BoundType;
			if (argumentBinding.BindingType != BindingType.Method ||
					    !CheckDelegateParameterList(delegateType, (IMethodBinding)argumentBinding))
			{
				Error(CompilerErrorFactory.EventArgumentMustBeAMethod(sourceNode, delegateMember.Name, delegateType.FullName));
				return false;
			}
			return true;
		}
		
		bool CheckParameterTypesStrictly(IMethodBinding method, ExpressionCollection args)
		{
			for (int i=0; i<args.Count; ++i)
			{
				ITypeBinding expressionType = GetExpressionType(args[i]);
				ITypeBinding parameterType = method.GetParameterType(i);
				if (!IsAssignableFrom(parameterType, expressionType) &&
					!(IsNumber(expressionType) && IsNumber(parameterType)))
				{					
					return false;
				}
			}
			return true;
		}
		
		bool CheckParameterTypes(IMethodBinding method, ExpressionCollection args)
		{
			for (int i=0; i<args.Count; ++i)
			{
				ITypeBinding expressionType = GetExpressionType(args[i]);
				ITypeBinding parameterType = method.GetParameterType(i);
				if (!IsAssignableFrom(parameterType, expressionType) &&
				    !CanBeReachedByDownCastOrPromotion(parameterType, expressionType))
				{
					
					return false;
				}
			}
			return true;
		}
		
		bool CheckParameters(Node sourceNode, IMethodBinding method, ExpressionCollection args)
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
		
		
		bool CheckDelegateParameterList(ITypeBinding delegateType, IMethodBinding target)
		{
			IMethodBinding invoke = (IMethodBinding)delegateType.Resolve("Invoke");
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
		
		void CheckIterator(Expression iterator, ITypeBinding type)
		{						
			if (type.IsArray)
			{				
				if (type.GetArrayRank() != 1)
				{
					Error(CompilerErrorFactory.InvalidArray(iterator));
				}
			}
			else
			{
				ITypeBinding enumerable = BindingManager.IEnumerableTypeBinding;
				if (!enumerable.IsAssignableFrom(type) &&
					!type.IsAssignableFrom(enumerable))
				{
					Error(CompilerErrorFactory.InvalidIteratorType(iterator, type.FullName));
				}
			}
		}		
		
		bool CheckTargetContext(Expression targetContext, IMemberBinding member)
		{
			if (!member.IsStatic)					  
			{			
				if (NodeType.MemberReferenceExpression == targetContext.NodeType)
				{				
					Expression targetReference = ((MemberReferenceExpression)targetContext).Target;
					if (BindingType.TypeReference == GetBinding(targetReference).BindingType)
					{						
						Error(CompilerErrorFactory.MemberNeedsInstance(targetContext, member.ToString()));
						return false;
					}
				}
			}
			return true;
		}
		
		static bool IsAssignableFrom(ITypeBinding expectedType, ITypeBinding actualType)
		{
			return expectedType.IsAssignableFrom(actualType);
		}
		
		bool CanBeReachedByDownCastOrPromotion(ITypeBinding expectedType, ITypeBinding actualType)
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

		bool IsLValue(IBinding binding)
		{
			switch (binding.BindingType)
			{
				case BindingType.Local:
				{
					return !((LocalBinding)binding).IsPrivateScope;
				}
				
				case BindingType.Property:
				{
					return true;
				}
				
				case BindingType.Field:
				{
					return true;
				}
			}
			return false;
		}
		
		bool IsIntegerNumber(ITypeBinding type)
		{
			return
				type == BindingManager.ShortTypeBinding ||
				type == BindingManager.IntTypeBinding ||
				type == BindingManager.LongTypeBinding ||
				type == BindingManager.ByteTypeBinding;
		}
		
		bool IsNumber(ITypeBinding type)
		{
			return
				IsIntegerNumber(type) ||
				type == BindingManager.DoubleTypeBinding ||
				type == BindingManager.SingleTypeBinding;
		}
		
		bool IsNumber(Expression expression)
		{
			return IsNumber(GetExpressionType(expression));
		}
		
		bool IsString(Expression expression)
		{
			return BindingManager.StringTypeBinding == GetExpressionType(expression);
		}
		
		IConstructorBinding FindCorrectConstructor(Node sourceNode, ITypeBinding typeBinding, ExpressionCollection arguments)
		{
			IConstructorBinding[] constructors = typeBinding.GetConstructors();
			if (constructors.Length > 0)
			{				
				return (IConstructorBinding)ResolveMethodReference(sourceNode, arguments, constructors, true);				
			}
			else
			{
				Error(CompilerErrorFactory.NoApropriateConstructorFound(sourceNode, typeBinding.FullName, GetSignature(arguments)));
			}
			return null;
		}
		
		IConstructorBinding GetDefaultConstructor(ITypeBinding type)
		{
			IConstructorBinding[] constructors = type.GetConstructors();
			for (int i=0; i<constructors.Length; ++i)
			{
				IConstructorBinding constructor = constructors[i];
				if (0 == constructor.ParameterCount)
				{
					return constructor;
				}
			}
			return null;
		}
		
		class BindingScore : IComparable
		{
			public IBinding Binding;
			public int Score;
			
			public BindingScore(IBinding binding, int score)
			{
				Binding = binding;
				Score = score;
			}
			
			public int CompareTo(object other)
			{
				return ((BindingScore)other).Score-Score;
			}
			
			override public string ToString()
			{
				return Binding.ToString();
			}
		}
		
		void EnsureRelatedNodeWasVisited(IBinding binding)
		{
			if (BindingType.TypeReference == binding.BindingType)
			{
				binding = ((TypeReferenceBinding)binding).BoundType;
			}
			
			IInternalBinding internalBinding = binding as IInternalBinding;
			if (null != internalBinding)
			{
				if (!internalBinding.Visited)
				{
					_context.TraceVerbose("Binding {0} needs resolving.", binding.Name);
					
					INamespace saved = _nameResolution.CurrentNamespace;
					try
					{
						TypeMember member = internalBinding.Node as TypeMember;
						if (null != member)
						{
							Switch(member.ParentNode);
						}
						Switch(internalBinding.Node);
					}
					finally
					{
						_nameResolution.Restore(saved);
					}
				}
			}
		}
		
		IBinding ResolveMethodReference(Node node, NodeCollection args, IBinding[] bindings, bool treatErrors)
		{
			List scores = new List();
			for (int i=0; i<bindings.Length; ++i)
			{				
				IBinding binding = bindings[i];
				IMethodBinding mb = binding as IMethodBinding;
				if (null != mb)
				{			
					if (args.Count == mb.ParameterCount)
					{
						int score = 0;
						for (int argIndex=0; argIndex<args.Count; ++argIndex)
						{
							ITypeBinding expressionType = GetExpressionType(args.GetNodeAt(argIndex));
							ITypeBinding parameterType = mb.GetParameterType(argIndex);						
							
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
							scores.Add(new BindingScore(binding, score));						
						}
					}
				}
			}		
			
			if (1 == scores.Count)
			{
				return ((BindingScore)scores[0]).Binding;
			}
			
			if (scores.Count > 1)
			{
				scores.Sort();
				
				BindingScore first = (BindingScore)scores[0];
				BindingScore second = (BindingScore)scores[1];
				if (first.Score > second.Score)
				{
					return first.Binding;
				}
				// todo: remove from scores, all the lesser
				// scored bindings
				
				if (treatErrors)
				{
					Error(CompilerErrorFactory.AmbiguousReference(node, first.Binding.Name, scores));
				}
			}
			else
			{	
				if (treatErrors)
				{
					Error(CompilerErrorFactory.NoApropriateOverloadFound(node, GetSignature(args), bindings[0].Name));
				}
			}
			
			if (treatErrors)
			{
				Error(node);
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
			ITypeBinding lhs = GetExpressionType(node.Left);
			if (ResolveOperator(node, lhs, operatorName, mie))
			{
				return true;
			}
			
			ITypeBinding rhs = GetExpressionType(node.Right);
			if (ResolveOperator(node, rhs, operatorName, mie))
			{
				return true;
			}
			return ResolveOperator(node, BindingManager.RuntimeServicesBinding, operatorName, mie);
		}
		
		IMethodBinding ResolveAmbiguousOperator(IBinding[] bindings, ExpressionCollection args)
		{
			foreach (IBinding binding in bindings)
			{
				IMethodBinding method = binding as IMethodBinding;
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
		
		bool ResolveOperator(BinaryExpression node, ITypeBinding type, string operatorName, MethodInvocationExpression mie)
		{
			IBinding binding = type.Resolve(operatorName);
			if (null == binding)
			{
				return false;
			}
			
			if (BindingType.Ambiguous == binding.BindingType)
			{	
				binding = ResolveAmbiguousOperator(((AmbiguousBinding)binding).Bindings, mie.Arguments);
				if (null == binding)
				{
					return false;
				}
			}
			else if (BindingType.Method == binding.BindingType)
			{					
				IMethodBinding method = (IMethodBinding)binding;
				
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
		
		bool CheckLValue(Node node, IBinding binding)
		{
			if (!IsLValue(binding))
			{
				Error(CompilerErrorFactory.LValueExpected(node));
				return false;
			}
			return true;
		}
		
		bool CheckBoolContext(Expression expression)
		{
			ITypeBinding type = GetBoundType(expression);
			if (type.IsValueType)
			{
				if (type == BindingManager.BoolTypeBinding ||
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
		
		LocalBinding DeclareLocal(Node sourceNode, Local local, ITypeBinding localType)
		{			
			LocalBinding binding = new LocalBinding(local, localType);
			Bind(local, binding);
			
			_currentMethodBinding.Method.Locals.Add(local);
			Bind(sourceNode, binding);
			
			return binding;
		}
		
		void PushMethodBinding(InternalMethodBinding binding)
		{
			_methodBindingStack.Push(_currentMethodBinding);
			
			_currentMethodBinding = binding;
		}
		
		void PopMethodBinding()
		{
			_currentMethodBinding = (InternalMethodBinding)_methodBindingStack.Pop();
		}
		
		static bool HasSideEffect(Expression node)
		{
			return
				node.NodeType == NodeType.MethodInvocationExpression ||
				IsAssignment(node) ||
				IsPreIncDec(node);
		}
		
		static bool IsPreIncDec(Expression node)
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
		
		void ProcessDeclarationsForIterator(DeclarationCollection declarations, ITypeBinding iteratorType, bool declarePrivateLocals)
		{
			ITypeBinding defaultDeclType = BindingManager.ObjectTypeBinding;
			
			if (iteratorType.IsArray)
			{
				defaultDeclType = iteratorType.GetElementType();
			}
			
			foreach (Declaration d in declarations)
			{				
				if (null == d.Type)
				{
					d.Type = CreateBoundTypeReference(defaultDeclType);
				}
				else
				{
					Switch(d.Type);
					// todo: check types here
				}
				if (CheckIdentifierName(d, d.Name))
				{
					DeclareLocal(d, new Local(d, declarePrivateLocals), GetBoundType(d.Type));
				}
			}
		}		
		
		protected ITypeBinding GetExpressionType(Node node)
		{
			ITypedBinding binding = (ITypedBinding)GetBinding(node);
			BindingType bindingType = binding.BindingType;
			if (IsStandaloneTypeReference(node))
			{
				return BindingManager.TypeTypeBinding;
			}
			if (Tuple_TypedConstructor1 == binding ||
				Tuple_TypedConstructor2 == binding)
			{
				return BindingManager.AsTupleBinding(GetBoundType(((MethodInvocationExpression)node).Arguments[0]));
			}
			return binding.BoundType;
		}
		
		protected IBinding GetOptionalBinding(Node node)
		{
			return BindingManager.GetOptionalBinding(node);
		}
		
		protected TypeReference CreateBoundTypeReference(ITypeBinding binding)
		{
			return BindingManager.CreateBoundTypeReference(binding);
		}
		
		bool IsStandaloneTypeReference(Node node)
		{
			return node.ParentNode.NodeType != NodeType.MemberReferenceExpression &&
					GetBinding(node).BindingType == BindingType.TypeReference;
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
		
		string GetSignature(IMethodBinding binding)
		{
			return BindingManager.GetSignature(binding);
		}

		void NotImplemented(Node node, string feature)
		{
			throw CompilerErrorFactory.NotImplemented(node, feature);
		}
		
		void InvalidOperatorForTypes(BinaryExpression node)
		{					
			Error(node, CompilerErrorFactory.InvalidOperatorForTypes(node,
							GetBinaryOperatorText(node.Operator),
							GetExpressionType(node.Left).FullName,
							GetExpressionType(node.Right).FullName));
		}
		
		void Error(Node node, CompilerError error)
		{
			BindingManager.Error(node);
			Errors.Add(error);
		}
		
		void Error(CompilerError error)
		{
			Errors.Add(error);
		}
		
		void Error(Node node)
		{
			BindingManager.Error(node);
		}
		
		void Bind(Node node, IBinding binding)
		{
			_context.TraceVerbose("{0}: Node '{1}' bound to '{2}'.", node.LexicalInfo, node, binding);
			BindingManager.Bind(node, binding);
		}
		
		void TraceOverride(Method method, IMethodBinding baseMethod)
		{
			_context.TraceInfo("{0}: Method '{1}' overrides '{2}'", method.LexicalInfo, method.Name, baseMethod);
		}
		
		void TraceReturnType(Method method, IMethodBinding binding)
		{
			_context.TraceInfo("{0}: return type for method {1} bound to {2}", method.LexicalInfo, method.Name, binding.BoundType);
		}		
	}
	
	class BinaryExpressionResolver : ITypeResolver
	{
		BinaryExpression _node;
		
		public BinaryExpressionResolver(BinaryExpression node)
		{
			_node = node;
		}
		
		public IBinding Resolve(SemanticStep parent)
		{
			if (!BindingManager.IsUnknown(_node.Left) &&
				!BindingManager.IsUnknown(_node.Right))
			{
				parent.ResolveBinaryExpression(_node);
			}
			return ((ITypedBinding)BindingManager.GetBinding(_node)).BoundType;
		}
		
		public void OnResolved(SemanticStep parent)
		{
		}
	}
	
	class ReturnTypeResolver : ITypeResolver
	{
		InternalMethodBinding _binding;
		
		public ReturnTypeResolver(InternalMethodBinding binding) 
		{
			_binding = binding;
		}
		
		public IBinding Resolve(SemanticStep parent)
		{
			parent.ResolveReturnType(_binding);
			return _binding.BoundType;
		}
		
		public void OnResolved(SemanticStep parent)
		{
			parent.ResolveMethodOverride(_binding);
		}
	}
	
}
