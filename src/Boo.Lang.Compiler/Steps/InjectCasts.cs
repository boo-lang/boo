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
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;
	
	public class InjectCasts : AbstractVisitorCompilerStep
	{
		override public void Run()
		{
			if (0 == Errors.Count)
			{
				Visit(CompileUnit);
			}
		}
		
		override public void LeaveBinaryExpression(BinaryExpression node)
		{
			if (BinaryOperatorType.Assign == node.Operator)
			{
				IType lhsType = node.Left.ExpressionType;
				if (IsCallableType(lhsType))
				{					
					IType rhsType = GetExpressionType(node.Right);
					if (lhsType != rhsType)
					{
						node.Right = CreateDelegate(lhsType, node.Right);  
					}
				}
			}
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
			
			constructor.Arguments.Add(CreateAddressOfExpression(source));
			Bind(constructor.Target, type.GetConstructors()[0]);
			BindExpressionType(constructor, type);
			
			return constructor;
		}
		
		Expression CreateAddressOfExpression(Expression arg)
		{
			MethodInvocationExpression mie = new MethodInvocationExpression(arg.LexicalInfo);
			mie.Target = new ReferenceExpression("__addressof__");
			mie.Arguments.Add(arg);
			Bind(mie.Target, TypeSystemServices.ResolvePrimitive("__addressof__"));
			BindExpressionType(mie, TypeSystemServices.IntPtrType);
			return mie;
		}
	}
}
