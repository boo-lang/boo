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
		
		/*
		 * Useful method bindings.
		 */
		IMethodBinding RuntimeServices_IsMatchBinding;
		
		IMethodBinding RuntimeServices_Contains;
		
		IMethodBinding RuntimeServices_Len;
		
		IMethodBinding Array_get_Length;
		
		IMethodBinding String_get_Length;
		
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
		
		public override void Run()
		{					
			_currentMethodBinding = null;
			_methodBindingStack = new Stack();
			_pending = new DependencyGraph(_context);
			_nameResolution.Initialize(_context);
			
			RuntimeServices_IsMatchBinding = (IMethodBinding)BindingManager.RuntimeServicesBinding.Resolve("IsMatch");
			RuntimeServices_Contains = (IMethodBinding)BindingManager.RuntimeServicesBinding.Resolve("Contains");
			RuntimeServices_Len = (IMethodBinding)BindingManager.RuntimeServicesBinding.Resolve("Len");
			Array_get_Length = ((IPropertyBinding)BindingManager.ArrayTypeBinding.Resolve("Length")).GetGetMethod();
			String_get_Length = ((IPropertyBinding)BindingManager.StringTypeBinding.Resolve("Length")).GetGetMethod();
			ICollection_get_Count = ((IPropertyBinding)BindingManager.ICollectionTypeBinding.Resolve("Count")).GetGetMethod();
			IList_Contains = (IMethodBinding)BindingManager.IListTypeBinding.Resolve("Contains");
			IDictionary_Contains = (IMethodBinding)BindingManager.IDictionaryTypeBinding.Resolve("Contains");
			Tuple_TypedConstructor1 = (IMethodBinding)BindingManager.AsBinding(Types.Builtins.GetMethod("tuple", new Type[] { typeof(Type), typeof(IEnumerable) }));
			Tuple_TypedConstructor2 = (IMethodBinding)BindingManager.AsBinding(Types.Builtins.GetMethod("tuple", new Type[] { typeof(Type), typeof(int) }));
			
			ApplicationException_StringConstructor =
					(IConstructorBinding)BindingManager.AsBinding(
						Types.ApplicationException.GetConstructor(new Type[] { typeof(string) }));
			
			Switch(CompileUnit);
			
			ResolveDependencyGraph();
		}		
		
		void ResolveDependencyGraph()
		{
			int iterations = _pending.Resolve(this);
			_context.TraceInfo("Type inference concluded in {0} iteration(s).", iterations);
		}
		
		public override void Dispose()
		{
			base.Dispose();
			
			_currentMethodBinding = null;
			_methodBindingStack = null;
			_pending = null;
			_nameResolution.Dispose();
		}
		
		void PushNamespace(INamespace ns)
		{
			_nameResolution.PushNamespace(ns);
		}
		
		void PopNamespace()
		{
			_nameResolution.PopNamespace();
		}
		
		IBinding Resolve(Node sourceNode, string name)
		{
			return _nameResolution.Resolve(sourceNode, name);
		}
		
		IBinding ResolveQualifiedName(Node sourceNode, string name)
		{
			return _nameResolution.ResolveQualifiedName(sourceNode, name);
		}
		
		public override void OnModule(Boo.Lang.Ast.Module module)
		{				
			PushNamespace(new ModuleNamespace(BindingManager, module));			
			
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
				node.BaseTypes.Add(
					CreateBoundTypeReference(BindingManager.ObjectTypeBinding)
				);
			}
		}
		
		public override void OnClassDefinition(ClassDefinition node)
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
			
			binding.Visited = true;
			BindBaseTypes(node);
			
			PushNamespace(binding);
			Switch(node.Attributes);
			Switch(node.Members);
			PopNamespace();
		}
		
		public override void OnAttribute(Boo.Lang.Ast.Attribute node)
		{
			ITypeBinding binding = (ITypeBinding)BindingManager.GetOptionalBinding(node);
			if (null != binding)
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
		
		public override void OnProperty(Property node)
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
			Switch(getter);
			
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
				setter.Parameters.Add(parameter);
				Switch(setter);
				
				setter.Name = "set_" + node.Name;
			}
			
			if (null != getter)
			{
				getter.Name = "get_" + node.Name;
			}
		}
		
		public override bool EnterField(Field node)
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
		
		public override void LeaveField(Field node)
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
		
		public override bool EnterConstructor(Constructor node)
		{
			InternalConstructorBinding binding = new InternalConstructorBinding(BindingManager, node);
			binding.Visited = true;
			
			Bind(node, binding);
			PushMethodBinding(binding);
			PushNamespace(binding);
			return true;
		}
		
		public override void LeaveConstructor(Constructor node)
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
		
		public override void LeaveParameterDeclaration(ParameterDeclaration parameter)
		{			
			if (null == parameter.Type)
			{
				parameter.Type = CreateBoundTypeReference(BindingManager.ObjectTypeBinding);
			}
			Bindings.ParameterBinding binding = new Bindings.ParameterBinding(parameter, GetBoundType(parameter.Type));
			Bind(parameter, binding);
		}	
		
		public override bool EnterMethod(Method method)
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
					return false;
				}
			}
			
			binding.Visited = true;
			PushMethodBinding(binding);
			PushNamespace(binding);
			return true;
		}
		
		public override void OnSuperLiteralExpression(SuperLiteralExpression node)
		{			
			Bind(node, _currentMethodBinding);
		}
		
		public override void LeaveMethod(Method method)
		{
			InternalMethodBinding binding = _currentMethodBinding;
			
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
		
		internal void ResolveMethodOverride(InternalMethodBinding binding)
		{
			ITypeBinding baseType = binding.DeclaringType.BaseType;
			
			Method method = binding.Method;
			
			IBinding baseMethods = baseType.Resolve(binding.Name);
			if (null != baseMethods)
			{
				if (BindingType.Method == baseMethods.BindingType)
				{
					IMethodBinding baseMethod = (IMethodBinding)baseMethods;
					if (CheckOverrideSignature(binding, baseMethod))
					{	
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
		
		public override void OnTupleTypeReference(TupleTypeReference node)
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
		
		public override void OnSimpleTypeReference(SimpleTypeReference node)
		{
			if (BindingManager.IsBound(node))
			{
				return;
			}
			
			IBinding info = ResolveQualifiedName(node, node.Name);
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
		
		public override void OnBoolLiteralExpression(BoolLiteralExpression node)
		{
			Bind(node, BindingManager.BoolTypeBinding);
		}
		
		public override void OnTimeSpanLiteralExpression(TimeSpanLiteralExpression node)
		{
			Bind(node, BindingManager.TimeSpanTypeBinding);
		}
		
		public override void OnIntegerLiteralExpression(IntegerLiteralExpression node)
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
		
		public override void OnRealLiteralExpression(RealLiteralExpression node)
		{
			Bind(node, BindingManager.RealTypeBinding);
		}
		
		public override void OnStringLiteralExpression(StringLiteralExpression node)
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
		
		public override void LeaveSlicingExpression(SlicingExpression node)
		{
			if (null != node.End || null != node.Step)
			{
				NotImplemented(node, "full slicing");
			}
			
			ITypeBinding targetType = GetBoundType(node.Target);
			if (BindingManager.IsError(targetType))
			{
				Error(node);
				return;
			}
			
			if (targetType.IsArray)
			{
				Bind(node, targetType.GetElementType());
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
							mie.Target = new MemberReferenceExpression(node.Target, getter.Name);
							Bind(mie.Target, getter);
							Bind(mie, getter);
							
							node.ReplaceBy(mie);
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
			}
		}
		
		public override void LeaveStringFormattingExpression(StringFormattingExpression node)
		{
			Bind(node, BindingManager.StringTypeBinding);
		}
		
		public override void LeaveListLiteralExpression(ListLiteralExpression node)
		{			
			Bind(node, BindingManager.ListTypeBinding);
		}
		
		public override void LeaveHashLiteralExpression(HashLiteralExpression node)
		{
			Bind(node, BindingManager.HashTypeBinding);
		}
		
		public override void LeaveTupleLiteralExpression(TupleLiteralExpression node)
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
		
		public override void LeaveDeclarationStatement(DeclarationStatement node)
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
		
		public override void LeaveExpressionStatement(ExpressionStatement node)
		{
			if (!HasSideEffect(node.Expression))
			{
				Error(CompilerErrorFactory.ExpressionStatementMustHaveSideEffect(node));
			}
		}
		
		public override void OnNullLiteralExpression(NullLiteralExpression node)
		{
			Bind(node, NullBinding.Default);
		}
		
		public override void OnSelfLiteralExpression(SelfLiteralExpression node)
		{
			Bind(node, BindingManager.GetBinding(_currentMethodBinding.Method.DeclaringType));
		}
		
		public override void LeaveAsExpression(AsExpression node)
		{
			ITypeBinding toType = GetBoundType(node.Type);
			if (toType.IsValueType)
			{
				Error(node, CompilerErrorFactory.CantCastToValueType(node, toType.FullName));
			}
			else
			{
				Bind(node, toType);
			}
		}
		
		public override void OnReferenceExpression(ReferenceExpression node)
		{
			IBinding info = ResolveName(node, node.Name);
			if (null != info)
			{
				Bind(node, info);
				
				// todo: treat ambiguous binding here!!!!
				IMemberBinding binding = info as IMemberBinding;
				if (null != binding)
				{					
					EnsureRelatedNodeWasVisited(binding);
					
					if (!binding.IsStatic)
					{
						MemberReferenceExpression memberRef = new MemberReferenceExpression(node.LexicalInfo);
						memberRef.Target = new SelfLiteralExpression(node.LexicalInfo);
						memberRef.Name = node.Name;
						Bind(memberRef, binding);
						
						Switch(memberRef.Target);
						
						node.ReplaceBy(memberRef);
					}
				}
			}
			else
			{	
				Error(node);
			}
		}
		
		public override void LeaveMemberReferenceExpression(MemberReferenceExpression node)
		{
			IBinding nodeBinding = ErrorBinding.Default;
			
			IBinding binding = GetBinding(node.Target);
			if (!BindingManager.IsError(binding))
			{
				ITypedBinding typedBinding = binding as ITypedBinding;
				if (null != typedBinding)
				{
					binding = typedBinding.BoundType;
				}
			
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
						if (!AstUtil.IsLhsOfAssignment(node))
						{
							node.ReplaceBy(CreateMethodInvocation(node.Target, ((IPropertyBinding)member).GetGetMethod()));
							return;
						}
					}
				}
			}
			
			Bind(node, nodeBinding);
		}
		
		public override void LeaveIfStatement(IfStatement node)
		{
			CheckBoolContext(node.Expression);			
		}
		
		public override void LeaveReturnStatement(ReturnStatement node)
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
		
		public override void OnForStatement(ForStatement node)
		{
			Switch(node.Iterator);
			
			ITypeBinding iteratorType = GetExpressionType(node.Iterator);
			CheckIterator(node.Iterator, iteratorType);
			ProcessDeclarationsForIterator(node.Declarations, iteratorType, true);
			
			PushNamespace(new DeclarationsNamespace(BindingManager, node.Declarations));
			Switch(node.Block);
			PopNamespace();
		}
		
		public override void OnUnpackStatement(UnpackStatement node)
		{
			Switch(node.Expression);
			ProcessDeclarationsForIterator(node.Declarations, GetBoundType(node.Expression), false);
		}
		
		public override void LeaveRaiseStatement(RaiseStatement node)
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
		
		public override void OnExceptionHandler(ExceptionHandler node)
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
			PushNamespace(new DeclarationsNamespace(BindingManager, node.Declaration));
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
		
		public override void LeaveUnaryExpression(UnaryExpression node)
		{
			switch (node.Operator)
			{
				case UnaryOperatorType.Not:					
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
		
		public override bool EnterBinaryExpression(BinaryExpression node)
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
		
		public override void LeaveBinaryExpression(BinaryExpression node)
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
				
				case BinaryOperatorType.Add:
				{
					BindArithmeticOperator(node);
					break;
				}
				
				case BinaryOperatorType.Subtract:
				{
					BindArithmeticOperator(node);
					break;
				}
				
				case BinaryOperatorType.Multiply:
				{
					BindArithmeticOperator(node);
					break;
				}
				
				case BinaryOperatorType.Divide:
				{
					BindArithmeticOperator(node);
					break;
				}
				
				case BinaryOperatorType.Match:
				{
					BindMatchOperator(node);
					break;
				}
				
				case BinaryOperatorType.Member:
				{
					BindMember(node);
					break;
				}
				
				case BinaryOperatorType.NotMember:
				{
					BindMember(node);
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
				
				case BinaryOperatorType.NotMatch:
				{
					BindMatchOperator(node);
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
				
				case BinaryOperatorType.GreaterEqualThan:
				{
					BindCmpOperator(node);
					break;
				}
				
				case BinaryOperatorType.LessThan:
				{
					BindCmpOperator(node);
					break;
				}
				
				case BinaryOperatorType.LessEqualThan:
				{
					BindCmpOperator(node);
					break;
				}
				
				case BinaryOperatorType.Inequality:
				{
					ResolveOperator("op_Inequality", node);
					break;
				}
				
				case BinaryOperatorType.Equality:
				{
					ResolveOperator("op_Equality", node);
					break;
				}
				
				default:
				{
					NotImplemented(node, node.Operator.ToString());
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
					Error(node,
						CompilerErrorFactory.InvalidOperatorForTypes(node,
							GetBinaryOperatorText(node.Operator),
							lhs.FullName, rhs.FullName));
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
			else
			{
				Error(node,
						CompilerErrorFactory.InvalidOperatorForTypes(node,
							GetBinaryOperatorText(node.Operator),
							lhs.FullName, rhs.FullName));
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
							node.ReplaceBy(resultingNode);
						}
					}
					break;
				}
			}
		}
		
		public override void OnMethodInvocationExpression(MethodInvocationExpression node)
		{			
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
					// super constructor call
					InternalConstructorBinding constructorBinding = (InternalConstructorBinding)targetBinding;
					constructorBinding.HasSuperCall = true;
					
					ITypeBinding baseType = constructorBinding.DeclaringType.BaseType;
					IConstructorBinding superConstructorBinding = FindCorrectConstructor(node, baseType, node.Arguments);
					if (null != superConstructorBinding)
					{
						Bind(node.Target, superConstructorBinding);
						Bind(node, superConstructorBinding);
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
		
		UnaryExpression CreateNotExpression(Expression node)
		{
			UnaryExpression notNode = new UnaryExpression();
			notNode.LexicalInfo = node.LexicalInfo;
			notNode.Operand = node;
			notNode.Operator = UnaryOperatorType.Not;
			
			Bind(notNode, BindingManager.BoolTypeBinding);
			return notNode;
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
				
				node.ReplaceBy(mie);
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
								
								node.ReplaceBy(CreateMethodInvocation(target, setter, node.Right));
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
		
		void BindMember(BinaryExpression node)
		{
			Node parent = node.ParentNode;
			
			MethodInvocationExpression contains = new MethodInvocationExpression();
			contains.LexicalInfo = node.LexicalInfo;
			
			ITypeBinding rhs = GetExpressionType(node.Right);
			if (IsDictionary(rhs))
			{
				contains.Target = new MemberReferenceExpression(node.Right.LexicalInfo, node.Right, "Contains");
				contains.Arguments.Add(node.Left);
				Bind(contains.Target, IDictionary_Contains);
			}
			else if (IsList(rhs))
			{
				contains.Target = new MemberReferenceExpression(node.Right.LexicalInfo, node.Right, "Contains");
				contains.Arguments.Add(node.Left);
				Bind(contains.Target, IList_Contains);
			}
			else
			{
				// todo: generate better/faster expressions for
				// arrays and IList implementations
				
				contains.Arguments.Add(node.Left);
				contains.Arguments.Add(node.Right);
				contains.Target = new ReferenceExpression("Boo.Lang.RuntimeServices.Contains");
				Bind(contains.Target, RuntimeServices_Contains);
			}
			
			Bind(contains, BindingManager.BoolTypeBinding);			
			if (BinaryOperatorType.NotMember == node.Operator)
			{
				parent.Replace(node, CreateNotExpression(contains));
			}
			else
			{
				parent.Replace(node, contains);
			}
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
					return BinaryOperatorType.Add;
					
				case BinaryOperatorType.InPlaceSubtract:
					return BinaryOperatorType.Subtract;
					
				case BinaryOperatorType.InPlaceMultiply:
					return BinaryOperatorType.Multiply;
					
				case BinaryOperatorType.InPlaceDivide:
					return BinaryOperatorType.Divide;
			}
			throw new ArgumentException("op");
		}
		
		void BindArithmeticOperator(BinaryExpression node)
		{
			ITypeBinding left = GetExpressionType(node.Left);
			ITypeBinding right = GetExpressionType(node.Right);
			if (IsNumber(left))
			{
				if (IsNumber(right))
				{
					Bind(node, BindingManager.GetPromotedNumberType(left, right));
				}
				else
				{
					InvalidOperatorForTypes(node, node.Operator, left, right);
				}
			}
			else
			{
				InvalidOperatorForTypes(node, node.Operator, left, right);
			}
		}
		
		void Negate(BinaryExpression node, BinaryOperatorType newOperator)
		{
			Node parent = node.ParentNode;
			node.Operator = newOperator;
			parent.Replace(node, CreateNotExpression(node));
		}
		
		void BindMatchOperator(BinaryExpression node)
		{
			ExpressionCollection args = new ExpressionCollection();
			args.Add(node.Left);
			args.Add(node.Right);
			
			if (CheckParameters(node, RuntimeServices_IsMatchBinding, args))
			{
				// todo; trocar Bind e BindOperator por um 
				// unico Bind(node, new OperatorBinding())
				Bind(node, RuntimeServices_IsMatchBinding);
				if (BinaryOperatorType.NotMatch == node.Operator)
				{
					Negate(node, BinaryOperatorType.Match);
				}
			}
			else
			{
				Error(node);
			}
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
		
		bool CheckParameters(Node sourceNode, IMethodBinding method, ExpressionCollection args)
		{				
			if (method.ParameterCount != args.Count)
			{
				Error(CompilerErrorFactory.MethodArgumentCount(sourceNode, method.Name, args.Count));
				return false;
			}	
			
			for (int i=0; i<args.Count; ++i)
			{
				ITypeBinding expressionType = GetExpressionType(args[i]);
				ITypeBinding parameterType = method.GetParameterType(i);
				if (!IsAssignableFrom(parameterType, expressionType) &&
				    !CanBeReachedByDownCastOrPromotion(parameterType, expressionType))
				{
					Error(CompilerErrorFactory.MethodSignature(sourceNode, GetSignature(method), GetSignature(args)));
					return false;
				}
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
		
		void CheckIterator(Expression iterator, ITypeBinding binding)
		{						
			ExternalTypeBinding externalType = binding as ExternalTypeBinding;
			if (null != externalType)
			{
				Type type = externalType.Type;
				if (type.IsArray)
				{
					if (type.GetArrayRank() > 1)
					{
						Error(CompilerErrorFactory.InvalidArray(iterator));
					}
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
				type == BindingManager.IntTypeBinding ||
				type == BindingManager.LongTypeBinding ||
				type == BindingManager.ByteTypeBinding;
		}
		
		bool IsNumber(ITypeBinding type)
		{
			return
				IsIntegerNumber(type) ||
				type == BindingManager.RealTypeBinding ||
				type == BindingManager.SingleTypeBinding;
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
			
			public override string ToString()
			{
				return Binding.ToString();
			}
		}
		
		void EnsureRelatedNodeWasVisited(IBinding binding)
		{
			IInternalBinding internalBinding = binding as IInternalBinding;
			if (null != internalBinding)
			{
				if (!internalBinding.Visited)
				{
					_context.TraceVerbose("Binding {0} needs resolving.", binding.Name);
					Switch(internalBinding.Node);
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
		
		bool ResolveOperator(string name, BinaryExpression node)
		{
			ITypeBinding boundType = GetBoundType(node.Left);
			IBinding binding = boundType.Resolve(name);
			if (null == binding)
			{
				Error(node);
				return false;
			}
			
			// todo: check parameters
			// todo: resolve when ambiguous
			Bind(node, binding);
			
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
				
				DeclareLocal(d, new Local(d, declarePrivateLocals), GetBoundType(d.Type));
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
		
		void InvalidOperatorForTypes(BinaryExpression node, BinaryOperatorType op, ITypeBinding left, ITypeBinding right)
		{			
			Error(node, CompilerErrorFactory.InvalidOperatorForTypes(node, GetBinaryOperatorText(op), left.FullName, right.FullName));
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
