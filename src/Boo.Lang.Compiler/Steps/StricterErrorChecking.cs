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
	
	public class StricterErrorChecking : AbstractVisitorCompilerStep
	{
		override public void Run()
		{
			if (0 == Errors.Count)
			{
				Visit(CompileUnit);
			}
		}
		
		override public void OnMethodInvocationExpression(MethodInvocationExpression node)
		{
			if (IsAddressOfBuiltin(node.Target))
			{
				if (!IsSecondArgumentOfDelegateConstructor(node))
				{
					Error(CompilerErrorFactory.AddressOfOutsideDelegateConstructor(node.Target));
				}
			}
		}
		
		bool IsSecondArgumentOfDelegateConstructor(Expression node)
		{                 
			MethodInvocationExpression mie = node.ParentNode as MethodInvocationExpression;
			if (null != mie)
			{
				if (IsDelegateConstructorInvocation(mie))
				{
					return mie.Arguments[2] == node;
				}
			}
			return false;
		}
		
		bool IsConstructorReference(Expression expression)
		{
			return expression is ReferenceExpression &&
				EntityType.Constructor == expression.Entity.EntityType;
		}
		
		bool IsDelegateConstructorInvocation(MethodInvocationExpression node)
		{
			IConstructor constructor = node.Target as IConstructor;
			if (null != constructor)
			{
				return constructor.DeclaringType is ICallableType;
			}
			return false;
		}
		
		bool IsAddressOfBuiltin(Expression node)
		{
			BuiltinFunction function = node.Entity as BuiltinFunction; 
			return null != function && BuiltinFunctionType.AddressOf == function.FunctionType;
		}
	}
}
