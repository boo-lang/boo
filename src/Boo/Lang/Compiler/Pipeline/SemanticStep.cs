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
	class SemanticMethodInfo
	{
		public static readonly SemanticMethodInfo Null = new SemanticMethodInfo(null, null);
		
		public Method Method;
		public ArrayList ReturnStatements;
		
		public SemanticMethodInfo(Method method, ArrayList returnStatements)
		{
			Method = method;
			ReturnStatements = returnStatements;
		}
	}
	
	/// <summary>
	/// Step 4.
	/// </summary>
	public class SemanticStep : AbstractNamespaceSensitiveCompilerStep
	{
		Stack _methodInfoStack;
		
		SemanticMethodInfo _currentMethodInfo;
		
		IMethodBinding RuntimeServices_IsMatchBinding;
		
		IMethodBinding RuntimeServices_Contains;
		
		IConstructorBinding ApplicationException_StringConstructor;
		
		BindingFilter IsPublicEventFilter;
		
		BindingFilter IsPublicFieldPropertyEventFilter;
		
		public SemanticStep()
		{
			IsPublicFieldPropertyEventFilter = new BindingFilter(IsPublicFieldPropertyEvent);
			IsPublicEventFilter = new BindingFilter(IsPublicEvent);
		}
		
		public override void Run()
		{					
			_currentMethodInfo = SemanticMethodInfo.Null;
			_methodInfoStack = new Stack();			
			
			RuntimeServices_IsMatchBinding = (IMethodBinding)BindingManager.RuntimeServicesBinding.Resolve("IsMatch");
			RuntimeServices_Contains = (IMethodBinding)BindingManager.RuntimeServicesBinding.Resolve("Contains");
			ApplicationException_StringConstructor =
					(IConstructorBinding)BindingManager.ToBinding(
						Types.ApplicationException.GetConstructor(new Type[] { typeof(string) }));
			
			Switch(CompileUnit);
		}		
		
		public override void Dispose()
		{
			base.Dispose();
			
			_currentMethodInfo = null;
			_methodInfoStack = null;
		}
		
		public override void OnModule(Boo.Lang.Ast.Module module, ref Boo.Lang.Ast.Module resultingNode)
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
				if (baseBinding.IsClass)
				{
					if (null != baseClass)
					{
						Errors.MultipleClassInheritance(baseType,
							node.Name,
							baseClass.FullName
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
		
		public override void OnClassDefinition(ClassDefinition node, ref ClassDefinition resultingNode)
		{
			BindBaseTypes(node);
			InternalTypeBinding binding = new InternalTypeBinding(BindingManager, node);
			BindingManager.Bind(node, binding);
			PushNamespace(binding);
			Switch(node.Attributes);
			Switch(node.Members);
			PopNamespace();
		}
		
		public override void OnAttribute(Boo.Lang.Ast.Attribute node, ref Boo.Lang.Ast.Attribute resultingNode)
		{
			ITypeBinding binding = (ITypeBinding)BindingManager.GetOptionalBinding(node);
			if (null != binding)
			{				
				Switch(node.Arguments);
				ResolveNamedArguments(node, binding, node.NamedArguments);
				
				IConstructorBinding constructor = FindCorrectConstructor(node, binding, node.Arguments);
				if (null != constructor)
				{
					BindingManager.Bind(node, constructor);
				}
			}
		}
		
		public override void OnProperty(Property node, ref Property resultingNode)
		{
			Method setter = node.Setter;
			Method getter = node.Getter;
			
			Switch(node.Attributes);
			Switch(node.Type);
			Switch(getter);
			
			ITypeBinding binding = null;
			if (null != node.Type)
			{
				binding = GetBoundType(node.Type);
			}
			else
			{
				if (null != getter)
				{
					binding = GetBoundType(node.Getter.ReturnType);
				}
				else
				{
					binding = BindingManager.ObjectTypeBinding;
				}
				node.Type = CreateBoundTypeReference(binding);
			}
			
			if (null != setter)
			{
				ParameterDeclaration parameter = new ParameterDeclaration();
				parameter.Type = CreateBoundTypeReference(binding);
				parameter.Name = "value";
				setter.Parameters.Add(parameter);
				Switch(setter);
				
				setter.Name = "set_" + node.Name;
			}
			if (null != getter)
			{
				getter.Name = "get_" + node.Name;
			}
			
			BindingManager.Bind(node, new InternalPropertyBinding(BindingManager, node));
		}
		
		public override void LeaveField(Field node, ref Field resultingNode)
		{			
			IBinding binding = GetOptionalBinding(node);
			if (null == binding)
			{
				BindingManager.Bind(node, new InternalFieldBinding(BindingManager, node));
			}
			
			if (null == node.Type)
			{
				node.Type = CreateBoundTypeReference(BindingManager.ObjectTypeBinding);
			}
		}
		
		public override bool EnterConstructor(Constructor node, ref Constructor resultingNode)
		{
			InternalConstructorBinding binding = new InternalConstructorBinding(BindingManager, node);
			BindingManager.Bind(node, binding);
			PushMethod(node);
			PushNamespace(binding);
			return true;
		}
		
		public override void LeaveConstructor(Constructor node, ref Constructor resultingNode)
		{
			PopNamespace();
			PopMethod();
			BindParameterIndexes(node);
		}
		
		public override void LeaveParameterDeclaration(ParameterDeclaration parameter, ref ParameterDeclaration resultingNode)
		{			
			if (null == parameter.Type)
			{
				parameter.Type = CreateBoundTypeReference(BindingManager.ObjectTypeBinding);
			}
			Bindings.ParameterBinding binding = new Bindings.ParameterBinding(parameter, GetBoundType(parameter.Type));
			BindingManager.Bind(parameter, binding);
		}	
		
		public override bool EnterMethod(Method method, ref Method resultingNode)
		{				
			InternalMethodBinding binding = (InternalMethodBinding)GetOptionalBinding(method);
			if (null == binding)
			{
				binding = new InternalMethodBinding(BindingManager, method);
				BindingManager.Bind(method, binding);
			}
			else
			{
				if (binding.IsResolved)
				{
					return false;
				}
			}
			
			PushMethod(method);
			PushNamespace(binding);
			return true;
		}
		
		public override void LeaveMethod(Method method, ref Method resultingNode)
		{
			if (null == method.ReturnType)
			{
				method.ReturnType = ResolveReturnType(_currentMethodInfo.ReturnStatements);
				_context.TraceInfo("{0}: return type for method {1} bound to {2}", method.LexicalInfo, method.Name, GetBinding(method.ReturnType));
			}
			
			InternalMethodBinding binding = (InternalMethodBinding)GetBinding(method);
			ResolveMethodOverride(method, binding);
			binding.Resolved();
			
			PopNamespace();
			PopMethod();
			BindParameterIndexes(method);
		}
		
		void ResolveMethodOverride(Method method, InternalMethodBinding binding)
		{
			ITypeBinding baseType = GetBaseType(method.DeclaringType);
			if (null == baseType)
			{
				return;
			}
			
			IBinding baseMethods = baseType.Resolve(method.Name);
			if (null != baseMethods)
			{
				if (BindingType.Method == baseMethods.BindingType)
				{
					IMethodBinding baseMethod = (IMethodBinding)baseMethods;
					if (CheckOverrideSignature(binding, baseMethod))
					{	
						binding.Override = baseMethod;
						TraceOverride(method, baseMethod);
						method.Modifiers |= TypeMemberModifiers.Override;
					}
				}
				else if (BindingType.Ambiguous == baseMethods.BindingType)
				{
					// todo:
				}
			}
		}
		
		void TraceOverride(Method method, IMethodBinding baseMethod)
		{
			_context.TraceInfo("{0}: Method '{1}' overrides '{2}'", method.LexicalInfo, method.Name, baseMethod);
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
		
		TypeReference ResolveReturnType(ArrayList returnStatements)
		{			
			if (0 == returnStatements.Count)
			{					
				return CreateBoundTypeReference(BindingManager.VoidTypeBinding);
			}		
			
			ITypeBinding type = GetBoundType(((ReturnStatement)returnStatements[0]).Expression);
			
			for (int i=1; i<returnStatements.Count; ++i)
			{	
				ITypeBinding newType = GetBoundType(((ReturnStatement)returnStatements[i]).Expression);
				if (type == newType)
				{
					continue;
				}
				
				if (IsAssignableFrom(type, newType))
				{
					continue;
				}
				
				if (IsAssignableFrom(newType, type))
				{
					newType = type;
					continue;
				}
				
				type = BindingManager.ObjectTypeBinding;
				break;
			}
			
			return CreateBoundTypeReference(type);
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
		
		public override void OnTypeReference(TypeReference node, ref TypeReference resultingNode)
		{
			if (BindingManager.IsBound(node))
			{
				return;
			}
			
			IBinding info = ResolveQualifiedName(node, node.Name);
			if (null == info || BindingType.TypeReference != info.BindingType)
			{
				Errors.NameNotType(node, node.Name);
				BindingManager.Error(node);
			}
			else
			{
				BindingManager.Bind(node, info);
			}
		}
		
		public override void OnBoolLiteralExpression(BoolLiteralExpression node, ref Expression resultingNode)
		{
			BindingManager.Bind(node, BindingManager.BoolTypeBinding);
		}
		
		public override void OnIntegerLiteralExpression(IntegerLiteralExpression node, ref Expression resultingNode)
		{
			BindingManager.Bind(node, BindingManager.IntTypeBinding);
		}
		
		public override void OnStringLiteralExpression(StringLiteralExpression node, ref Expression resultingNode)
		{
			BindingManager.Bind(node, BindingManager.StringTypeBinding);
		}
		
		public override void LeaveStringFormattingExpression(StringFormattingExpression node, ref Expression resultingNode)
		{
			BindingManager.Bind(node, BindingManager.StringTypeBinding);
		}
		
		public override void LeaveListLiteralExpression(ListLiteralExpression node, ref Expression resultingNode)
		{			
			BindingManager.Bind(node, BindingManager.ListTypeBinding);
		}
		
		public override void LeaveTupleLiteralExpression(TupleLiteralExpression node, ref Expression resultingNode)
		{
			BindingManager.Bind(node, BindingManager.ObjectArrayBinding);
		}
		
		public override void LeaveDeclarationStatement(DeclarationStatement node, ref Statement resultingNode)
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
				BindingManager.Bind(var, localBinding);				
				
				BinaryExpression assign = new BinaryExpression(node.LexicalInfo);
				assign.Operator = BinaryOperatorType.Assign;
				assign.Left = var;
				assign.Right = node.Initializer;
				BindingManager.Bind(assign, GetBinding(assign.Right));				
				
				resultingNode = new ExpressionStatement(assign);
			}
			else
			{
				resultingNode = null;
			}
		}
		
		public override void LeaveExpressionStatement(ExpressionStatement node, ref Statement resultingNode)
		{
			if (!HasSideEffect(node.Expression))
			{
				Errors.ExpressionStatementMustHaveSideEffect(node);
			}
		}
		
		public override void OnSelfLiteralExpression(SelfLiteralExpression node, ref Expression resultingNode)
		{
			BindingManager.Bind(node, BindingManager.GetBinding(_currentMethodInfo.Method.DeclaringType));
		}
		
		public override void LeaveAsExpression(AsExpression node, ref Expression resultingNode)
		{
			ITypeBinding toType = GetBoundType(node.Type);
			if (toType.IsValueType)
			{
				Errors.CantCastToValueType(node, toType.FullName);
				BindingManager.Error(node);
			}
			else
			{
				BindingManager.Bind(node, toType);
			}
		}
		
		public override void OnReferenceExpression(ReferenceExpression node, ref Expression resultingNode)
		{
			IBinding info = ResolveName(node, node.Name);
			if (null != info)
			{
				BindingManager.Bind(node, info);
				
				// todo: treat ambiguous binding here!!!!
				IMemberBinding binding = info as IMemberBinding;
				if (null != binding)
				{
					if (BindingType.Method == binding.BindingType)
					{
						EnsureMethodIsResolved((IMethodBinding)binding);
					}
					
					if (!binding.IsStatic)
					{
						MemberReferenceExpression memberRef = new MemberReferenceExpression(node.LexicalInfo);
						memberRef.Target = new SelfLiteralExpression(node.LexicalInfo);
						memberRef.Name = node.Name;
						BindingManager.Bind(memberRef, binding);
						
						Switch(memberRef.Target);
						
						resultingNode = memberRef;
					}
				}
			}
			else
			{
				BindingManager.Error(node);
			}
		}
		
		public override void LeaveMemberReferenceExpression(MemberReferenceExpression node, ref Expression resultingNode)
		{
			IBinding nodeBinding = ErrorBinding.Default;
			
			IBinding binding = GetBinding(node.Target);
			if (BindingType.Error != binding.BindingType)
			{
				ITypedBinding typedBinding = binding as ITypedBinding;
				if (null != typedBinding)
				{
					binding = typedBinding.BoundType;
				}
			
				IBinding member = ((INamespace)binding).Resolve(node.Name);				
				if (null == member)
				{										
					Errors.MemberNotFound(node, binding.Name);
				}
				else
				{
					nodeBinding = member;
					if (BindingType.Method == member.BindingType)
					{
						EnsureMethodIsResolved((IMethodBinding)member);
					}
				}
			}
			
			BindingManager.Bind(node, nodeBinding);
		}
		
		public override void LeaveIfStatement(IfStatement node, ref Statement resultingNode)
		{
			CheckBoolContext(node.Expression);			
		}
		
		public override void LeaveReturnStatement(ReturnStatement node, ref Statement resultingNode)
		{
			if (null != node.Expression)
			{
				_currentMethodInfo.ReturnStatements.Add(node);
			}
		}
		
		public override void OnForStatement(ForStatement node, ref Statement resultingNode)
		{
			Switch(node.Iterator);
			
			ITypeBinding iteratorType = GetBoundType(node.Iterator);
			CheckIterator(node.Iterator, iteratorType);
			ProcessDeclarationsForIterator(node.Declarations, iteratorType, true);
			
			PushNamespace(new DeclarationsNamespace(BindingManager, node.Declarations));
			Switch(node.Block);
			PopNamespace();
		}
		
		public override void OnUnpackStatement(UnpackStatement node, ref Statement resultingNode)
		{
			node.Expression = Switch(node.Expression);
			ProcessDeclarationsForIterator(node.Declarations, GetBoundType(node.Expression), false);			
		}
		
		public override void LeaveRaiseStatement(RaiseStatement node, ref Statement resultingNode)
		{
			if (BindingManager.StringTypeBinding == GetBoundType(node.Exception))
			{
				MethodInvocationExpression expression = new MethodInvocationExpression(node.Exception.LexicalInfo);
				expression.Arguments.Add(node.Exception);
				expression.Target = new ReferenceExpression("System.ApplicationException");
				BindingManager.Bind(expression.Target, ApplicationException_StringConstructor);
				BindingManager.Bind(expression, BindingManager.ApplicationExceptionBinding);

				node.Exception = expression;				
			}
		}
		
		public override void OnExceptionHandler(ExceptionHandler node, ref ExceptionHandler resultingNode)
		{
			if (null == node.Declaration.Type)
			{
				node.Declaration.Type = new TypeReference("System.Exception");
				BindingManager.Bind(node.Declaration.Type, BindingManager.ToTypeReference(Types.Exception));
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
		
		public override void LeaveUnaryExpression(UnaryExpression node, ref Expression resultingNode)
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
					BindingManager.Bind(node, BindingManager.BoolTypeBinding);
					break;
				}
					
				default:
				{
					BindingManager.Error(node);
					Errors.NotImplemented(node, "unary operator not supported");
					break;
				}
			}
		}
		
		public override bool EnterBinaryExpression(BinaryExpression node, ref Expression resultingNode)
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
					node.Right = Switch(node.Right);
					ITypeBinding expressionTypeInfo = BindingManager.GetBoundType(node.Right);				
					DeclareLocal(reference, new Local(reference, false), expressionTypeInfo);
					BindingManager.Bind(node, expressionTypeInfo);
					return false;
				}
			}
			return true;
		}
		
		public override void LeaveBinaryExpression(BinaryExpression node, ref Expression resultingNode)
		{
			switch (node.Operator)
			{		
				case BinaryOperatorType.Match:
				{
					BindMatchOperator(node);
					break;
				}
				
				case BinaryOperatorType.Member:
				{
					BindMember(node, ref resultingNode);
					break;
				}
				
				case BinaryOperatorType.NotMember:
				{
					BindMember(node, ref resultingNode);
					resultingNode = CreateNotExpression(resultingNode);
					break;
				}
				
				case BinaryOperatorType.NotMatch:
				{
					if (BindMatchOperator(node))					
					{
						resultingNode = CreateNotExpression(node);
					}
					break;
				}
				
				case BinaryOperatorType.InPlaceAdd:
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
								BindingManager.Bind(node.Left, binding);
							}
						}
						else
						{
							binding = null;
						}
					}
					
					if (null == binding)
					{
						Errors.NotImplemented(node, "InPlaceAdd");
					}
					
					IEventBinding eventBinding = (IEventBinding)binding;
					ITypedBinding expressionBinding = (ITypedBinding)GetBinding(node.Right);
					if (CheckDelegateArgument(node.Left, eventBinding, expressionBinding))
					{						
					}
					
					BindingManager.Bind(node, BindingManager.VoidTypeBinding);
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
					// expression type is the same as the right expression's
					BindingManager.Bind(node, BindingManager.GetBoundType(node.Left));
					break;
				}
			}
		}		
		
		public override void OnMethodInvocationExpression(MethodInvocationExpression node, ref Expression resultingNode)
		{			
			node.Target = Switch(node.Target);
			Switch(node.Arguments);
			
			IBinding targetBinding = BindingManager.GetBinding(node.Target);
			if (BindingType.Ambiguous == targetBinding.BindingType)
			{		
				IBinding[] bindings = ((AmbiguousBinding)targetBinding).Bindings;
				targetBinding = ResolveMethodReference(node, node.Arguments, bindings);				
				if (null == targetBinding)
				{
					return;
				}
				BindingManager.Bind(node.Target, targetBinding);
			}	
			
			switch (targetBinding.BindingType)
			{				
				case BindingType.Method:
				{				
					IBinding nodeBinding = ErrorBinding.Default;
					
					IMethodBinding targetMethod = (IMethodBinding)targetBinding;
					if (CheckParameters(node, targetMethod, node.Arguments))
					{
						if (node.NamedArguments.Count > 0)
						{
							Errors.NamedParametersNotAllowed(node.NamedArguments[0]);							
						}
						else
						{			
							if (CheckTargetContext(node.Target, targetMethod))
							{
								nodeBinding = targetMethod.ReturnType;
							}
						}
					}
					
					BindingManager.Bind(node, nodeBinding);
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
						BindingManager.Bind(node.Target, ctorBinding);
						// expression result type is a new object
						// of type
						BindingManager.Bind(node, typeBinding);
					}
					break;
				}
				
				case BindingType.Error:
				{
					BindingManager.Error(node);
					break;
				}
				
				default:
				{
					Errors.NotImplemented(node, targetBinding.ToString());
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
			
			BindingManager.Bind(notNode, BindingManager.BoolTypeBinding);
			return notNode;
		}
		
		void BindMember(BinaryExpression node, ref Expression resultingNode)
		{
			// todo: generate better/faster expressions for
			// arrays and IList implementations
			MethodInvocationExpression contains = new MethodInvocationExpression();
			contains.LexicalInfo = node.LexicalInfo;
			contains.Arguments.Add(node.Left);
			contains.Arguments.Add(node.Right);
			contains.Target = new ReferenceExpression("Boo.Lang.RuntimeServices.Contains");
			BindingManager.Bind(contains.Target, RuntimeServices_Contains);
			BindingManager.Bind(contains, BindingManager.BoolTypeBinding);
			
			resultingNode = contains;
		}
		
		bool BindMatchOperator(BinaryExpression node)
		{
			ExpressionCollection args = new ExpressionCollection();
			args.Add(node.Left);
			args.Add(node.Right);
			
			if (CheckParameters(node, RuntimeServices_IsMatchBinding, args))
			{
				// todo; trocar Bind e BindOperator por um 
				// unico Bind(node, new OperatorBinding())
				BindingManager.Bind(node, RuntimeServices_IsMatchBinding);					
			}
			else
			{
				BindingManager.Error(node);
				return false;
			}
			return true;
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
				Errors.UnknownName(node, name);			
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
								Errors.AmbiguousName(sourceNode, name, found);
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
			Errors.NotAPublicFieldOrProperty(sourceNode, type.FullName, name);			
			return null;
		}
		
		void ResolveNamedArguments(Node sourceNode, ITypeBinding typeBinding, ExpressionPairCollection arguments)
		{
			foreach (ExpressionPair arg in arguments)
			{			
				arg.Second = Switch(arg.Second);				
				
				ReferenceExpression name = arg.First as ReferenceExpression;
				if (null == name)
				{
					Errors.NamedParameterMustBeReference(arg);
					continue;				
				}
				
				IMemberBinding member = ResolvePublicFieldPropertyEvent(name, typeBinding, name.Name);
				if (null == member)				    
				{					
					continue;
				}
				
				BindingManager.Bind(arg.First, member);
				
				ITypeBinding memberType = member.BoundType;
				ITypedBinding expressionBinding = (ITypedBinding)GetBinding(arg.Second);				
				
				if (member.BindingType == BindingType.Event)
				{
					CheckDelegateArgument(arg.First, member, expressionBinding);
				}
				else
				{						
					ITypeBinding expressionType = expressionBinding.BoundType;
					if (!IsAssignableFrom(memberType, expressionType))
					{
						Errors.IncompatibleExpressionType(arg, memberType.FullName, expressionType.FullName);
					}
				}
			}
		}
		
		bool CheckDelegateArgument(Node sourceNode, ITypedBinding delegateMember, ITypedBinding argumentBinding)
		{
			ITypeBinding delegateType = delegateMember.BoundType;
			if (argumentBinding.BindingType != BindingType.Method ||
					    !CheckDelegateParameterList(delegateType, (IMethodBinding)argumentBinding))
			{
				Errors.EventArgumentMustBeAMethod(sourceNode, delegateMember.Name, delegateType.FullName);
				return false;
			}
			return true;
		}
		
		bool CheckParameters(Node sourceNode, IMethodBinding method, ExpressionCollection args)
		{				
			if (method.ParameterCount != args.Count)
			{
				Errors.MethodArgumentCount(sourceNode, method, args.Count);
				return false;
			}	
			
			for (int i=0; i<args.Count; ++i)
			{
				ITypeBinding expressionType = BindingManager.GetBoundType(args[i]);
				ITypeBinding parameterType = method.GetParameterType(i);
				if (!IsAssignableFrom(parameterType, expressionType) &&
				    !CanBeReachedByDownCastOrPromotion(parameterType, expressionType))
				{
					Errors.MethodSignature(sourceNode, GetSignature(args), GetSignature(method));
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
						Errors.InvalidArray(iterator);
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
						Errors.MemberNeedsInstance(targetContext, member.ToString());
						return false;
					}
				}
			}
			return true;
		}
		
		bool IsAssignableFrom(ITypeBinding expectedType, ITypeBinding actualType)
		{
			return expectedType.IsAssignableFrom(actualType);
		}
		
		bool CanBeReachedByDownCastOrPromotion(ITypeBinding expectedType, ITypeBinding actualType)
		{
			if (expectedType.IsValueType)
			{
				return IsNumber(expectedType) && IsNumber(actualType);
			}
			return actualType.IsAssignableFrom(expectedType);
		}		
		
		bool IsNumber(ITypeBinding type)
		{
			return
				type == BindingManager.IntTypeBinding ||
				type == BindingManager.SingleTypeBinding;
		}
		
		IConstructorBinding FindCorrectConstructor(Node sourceNode, ITypeBinding typeBinding, ExpressionCollection arguments)
		{
			IConstructorBinding[] constructors = typeBinding.GetConstructors();
			if (constructors.Length > 0)
			{
				return (IConstructorBinding)ResolveMethodReference(sourceNode, arguments, constructors);				
			}
			else
			{
				Errors.NoApropriateConstructorFound(sourceNode, typeBinding.FullName, GetSignature(arguments));
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
		
		void EnsureMethodIsResolved(IMethodBinding binding)
		{
			InternalMethodBinding internalMethod = binding as InternalMethodBinding;
			if (null != internalMethod)
			{
				if (!internalMethod.IsResolved)
				{
					_context.TraceVerbose("Method {0} needs resolving.", binding.Name);
					if (!IsInMethodInfoStack(internalMethod.Method))
					{
						Switch(internalMethod.Method);
					}
				}
			}
		}
		
		IBinding ResolveMethodReference(Node node, ExpressionCollection args, IBinding[] bindings)
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
							ITypeBinding expressionType = BindingManager.GetBoundType(args[argIndex]);
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
				
				Errors.AmbiguousName(node, "", scores);
			}
			else
			{
				Errors.NoApropriateOverloadFound(node, GetSignature(args), bindings[0].Name);
			}
			BindingManager.Error(node);	
			return null;
		}
		
		void ResolveOperator(string name, BinaryExpression node)
		{
			ITypeBinding boundType = GetBoundType(node.Left);
			IBinding binding = boundType.Resolve(name);
			if (null == binding)
			{
				BindingManager.Error(node);
			}
			else
			{
				// todo: check parameters
				// todo: resolve when ambiguous
				BindingManager.Bind(node, binding);
			}
		}
		
		bool CheckBoolContext(Expression expression)
		{
			ITypeBinding type = GetBoundType(expression);
			if (type.IsValueType)
			{
				if (type == BindingManager.BoolTypeBinding ||
				    type == BindingManager.IntTypeBinding)
			    {
			    	return true;
			    }
			    Errors.BoolExpressionRequired(expression, type.FullName);
				return false;
			}
			// reference types can be used in bool context
			return true;
		}
		
		LocalBinding DeclareLocal(Node sourceNode, Local local, ITypeBinding localType)
		{			
			LocalBinding binding = new LocalBinding(local, localType);
			BindingManager.Bind(local, binding);
			
			_currentMethodInfo.Method.Locals.Add(local);
			BindingManager.Bind(sourceNode, binding);
			
			return binding;
		}
		
		void PushMethod(Method method)
		{
			_methodInfoStack.Push(_currentMethodInfo);
			
			// todo: alloc ArrayList from a pool
			_currentMethodInfo = new SemanticMethodInfo(method, new ArrayList());
		}
		
		void PopMethod()
		{
			_currentMethodInfo = (SemanticMethodInfo)_methodInfoStack.Pop();
		}
		
		bool IsInMethodInfoStack(Method method)
		{			
			foreach (SemanticMethodInfo info in _methodInfoStack)
			{				
				if (method == info.Method)
				{
					return true;
				}
			}
			return false;
		}
		
		TypeReference CreateBoundTypeReference(ITypeBinding binding)
		{
			TypeReference typeReference = new TypeReference(binding.FullName);
			BindingManager.Bind(typeReference, BindingManager.ToTypeReference(binding));
			return typeReference;
		}
		
		bool HasSideEffect(Expression node)
		{
			return node.NodeType == NodeType.MethodInvocationExpression ||
				IsAssignment(node);
		}
		
		bool IsAssignment(Expression node)
		{
			if (node.NodeType == NodeType.BinaryExpression)
			{
				BinaryOperatorType binaryOperator = ((BinaryExpression)node).Operator;
				return BinaryOperatorType.Assign == binaryOperator ||
						BinaryOperatorType.InPlaceAdd == binaryOperator;
			}
			return false;
		}
		
		void ProcessDeclarationsForIterator(DeclarationCollection declarations, ITypeBinding iteratorType, bool declarePrivateLocals)
		{
			ITypeBinding defaultDeclType = BindingManager.ObjectTypeBinding;
			
			ExternalTypeBinding externalType = iteratorType as ExternalTypeBinding;
			if (null != externalType)			
			{				
				Type type = externalType.Type;
				if (type.IsArray)
				{
					defaultDeclType = BindingManager.ToTypeBinding(type.GetElementType());
				}
			}
			
			foreach (Declaration d in declarations)
			{				
				if (null == d.Type)
				{
					d.Type = new TypeReference(defaultDeclType.FullName);
					BindingManager.Bind(d.Type, defaultDeclType);
				}
				else
				{
					Switch(d.Type);
					// todo: check types here
				}
				
				DeclareLocal(d, new Local(d, declarePrivateLocals), BindingManager.GetBoundType(d.Type));
			}
		}		
		
		string GetSignature(ExpressionCollection args)
		{
			StringBuilder sb = new StringBuilder("(");
			foreach (Expression arg in args)
			{
				if (sb.Length > 1)
				{
					sb.Append(", ");
				}
				sb.Append(BindingManager.GetBoundType(arg));
			}
			sb.Append(")");
			return sb.ToString();
		}
		
		string GetSignature(IMethodBinding binding)
		{
			return BindingManager.GetSignature(binding);
		}	
	}
}
