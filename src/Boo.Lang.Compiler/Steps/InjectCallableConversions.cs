#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
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
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;
	
	public class InjectCallableConversions : AbstractVisitorCompilerStep
	{
		IMethod _current;
		
		IType _asyncResultType;
		
		IMethod _asyncResultTypeAsyncDelegateGetter;
		
		override public void Run()
		{
			if (0 == Errors.Count)
			{
				Initialize();
				Visit(CompileUnit);
			}
		}
		
		void Initialize()
		{
			Type type = typeof( System.Runtime.Remoting.Messaging.AsyncResult);
			_asyncResultType = TypeSystemServices.Map(type);
			_asyncResultTypeAsyncDelegateGetter = (IMethod)TypeSystemServices.Map(type.GetProperty("AsyncDelegate").GetGetMethod());
		}
		
		override public void Dispose()
		{
			_asyncResultType = null;
			_asyncResultTypeAsyncDelegateGetter = null;
			base.Dispose();
		}
		
		override public void OnMethod(Method node)
		{
			_current = (IMethod)GetEntity(node);
			Visit(node.Body);
		}
		
		bool HasReturnType(IMethod method)
		{
			return TypeSystemServices.VoidType != method.ReturnType;
		}
		
		bool IsMethodReference(Expression node)
		{
			return EntityType.Method == node.Entity.EntityType;
		}
		
		bool IsNotTargetOfMethodInvocation(Expression node)
		{
			MethodInvocationExpression mie = node.ParentNode as MethodInvocationExpression;
			if (null != mie)
			{
				return mie.Target != node;
			}
			return true;
		}
		
		bool IsStandaloneMethodReference(Expression node)
		{
			return
				(node is ReferenceExpression) &&
				IsMethodReference(node) &&
				IsNotTargetOfMethodInvocation(node);
		}
		
		override public void LeaveReturnStatement(ReturnStatement node)
		{
			if (HasReturnType(_current))
			{
				Expression expression = node.Expression;
				if (null != expression)
				{
					Expression newExpression = Convert(_current.ReturnType, expression);
					if (null != newExpression)
					{
						node.Expression = newExpression;
					}
				}
			}			
		}
		
		override public void LeaveExpressionPair(ExpressionPair pair)
		{
			Expression converted = ConvertExpression(pair.First);
			if (null != converted)
			{
				pair.First = converted;
			}
			
			converted = ConvertExpression(pair.Second);
			if (null != converted)
			{
				pair.Second = converted;
			}
		}
		
		override public void LeaveListLiteralExpression(ListLiteralExpression node)
		{
			ConvertExpressions(node.Items);
		}
		
		override public void LeaveArrayLiteralExpression(ArrayLiteralExpression node)
		{
			ConvertExpressions(node.Items);
		}
		
		override public void LeaveMethodInvocationExpression(MethodInvocationExpression node)
		{
			ICallableType type = node.Target.ExpressionType as ICallableType;
			if (null == type)
			{
				return;
			}
			
			IParameter[] parameters = type.GetSignature().Parameters;
			ExpressionCollection arguments = node.Arguments;
			for (int i=0; i<parameters.Length; ++i)
			{
				Expression newArgument = Convert(parameters[i].Type, arguments[i]);
				if (null != newArgument)
				{
					arguments.ReplaceAt(i, newArgument);
				}
			}
		}
		
		override public void LeaveMemberReferenceExpression(MemberReferenceExpression node)
		{
			if (IsEndInvokeOnStandaloneMethodReference(node) &&
				AstUtil.IsTargetOfMethodInvocation(node))
			{
				ReplaceEndInvokeTargetByGetAsyncDelegate((MethodInvocationExpression)node.ParentNode);
			}
			else
			{
				Expression newTarget = ConvertExpression(node.Target);
				if (null != newTarget)
				{
					node.Target = newTarget;
				}
			}
		}
		
		override public void LeaveBinaryExpression(BinaryExpression node)
		{
			if (BinaryOperatorType.Assign == node.Operator)
			{
				Expression newRight = Convert(node.Left.ExpressionType, node.Right);
				if (null != newRight)
				{
					node.Right = newRight;
				}
			}
		}
		
		void ConvertExpressions(ExpressionCollection items)
		{
			for (int i=0; i<items.Count; ++i)
			{
				Expression converted = ConvertExpression(items[i]);
				if (null != converted)
				{
					items.ReplaceAt(i, converted);
				}
			}
		}
		
		Expression ConvertExpression(Expression expression)
		{
			return Convert(expression.ExpressionType, expression);
		}
		
		Expression Convert(IType expectedType, Expression argument)
		{
			if (IsStandaloneMethodReference(argument))
			{
				// todo: criar o delegate sempre, ainda
				// que o tipo esperado não seja callable, nesse
				// caso criar com o próprio tipo do método
				if (IsCallableType(expectedType))
				{
					return CreateDelegate(expectedType, argument);
				}
				else
				{
					return CreateDelegate(GetConcreteExpressionType(argument), argument);
				}
			}
			return null;
		}
		
		bool IsEndInvokeOnStandaloneMethodReference(MemberReferenceExpression node)
		{
			if (IsStandaloneMethodReference(node.Target))
			{
				InternalCallableType type = (InternalCallableType)node.Target.ExpressionType;
				return type.GetEndInvokeMethod() == node.Entity;
			}
			return false;
		}
		
		CastExpression CreateCast(IType type, Expression target)
		{
			CastExpression expression = new CastExpression();
			expression.Type = CreateTypeReference(type);
			expression.Target = target;
			BindExpressionType(expression, type);
			return expression;
		}
		
		void ReplaceEndInvokeTargetByGetAsyncDelegate(MethodInvocationExpression node)
		{
			Expression asyncResult = node.Arguments[0];
			MemberReferenceExpression endInvoke = (MemberReferenceExpression)node.Target;
			IType callableType = ((IMember)endInvoke.Entity).DeclaringType;
			
			endInvoke.Target = CreateCast(callableType,
									CreateMethodInvocation(
										CreateCast(_asyncResultType, asyncResult.CloneNode()),
										_asyncResultTypeAsyncDelegateGetter));
										
		}
		
		bool IsNull(IType type)
		{
			return EntityType.Null == type.EntityType;
		}
		
		bool IsCallableType(IType type)
		{
			return type is ICallableType;
		}
		
		Expression CreateDelegate(IType type, Expression source)
		{
			IMethod method = (IMethod)GetEntity(source);
			
			MethodInvocationExpression constructor = new MethodInvocationExpression(source.LexicalInfo);
			constructor.Target = new ReferenceExpression(type.FullName);
			
			if (method.IsStatic)
			{
				constructor.Arguments.Add(new NullLiteralExpression());
			}
			else
			{
				constructor.Arguments.Add(((MemberReferenceExpression)source).Target);
			}
			
			constructor.Arguments.Add(CreateAddressOfExpression(method));
			Bind(constructor.Target, type.GetConstructors()[0]);
			BindExpressionType(constructor, type);
			
			return constructor;
		}
		
		ReferenceExpression CreateReference(IType type)
		{
			ReferenceExpression reference = new ReferenceExpression(type.FullName);
			reference.Entity = type;
			return reference;
		}
		
		Expression CreateMethodReference(IMethod method)
		{			
			return CreateMemberReference(CreateReference(method.DeclaringType), method);
		}
		
		Expression CreateAddressOfExpression(IMethod method)
		{
			MethodInvocationExpression mie = new MethodInvocationExpression();
			mie.Target = new ReferenceExpression("__addressof__");
			mie.Arguments.Add(CreateMethodReference(method));
			Bind(mie.Target, TypeSystemServices.ResolvePrimitive("__addressof__"));
			BindExpressionType(mie, TypeSystemServices.IntPtrType);
			return mie;
		}
	}
}
