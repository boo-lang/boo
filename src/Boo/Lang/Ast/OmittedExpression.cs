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

using System;
using Boo.Lang.Ast.Impl;

namespace Boo.Lang.Ast
{
	/// <summary>
	/// Permite diferenar entre uma expresso que no foi
	/// passada de uma que foi omitidade como nosso caso
	/// de slices na forma [expression:] ou [:expression]
	/// </summary>
	public sealed class OmittedExpression : Expression
	{
		public static readonly Expression Default = new OmittedExpression();

		public OmittedExpression()
		{
		}
		
		override public NodeType NodeType
		{
			get
			{
				return NodeType.OmittedExpression;
			}
		}
		
		override public object Clone()
		{
			return this;
		}

		override public void Switch(IAstSwitcher switcher)
		{
			switcher.OnOmittedExpression(this);
		}
		
		override public void Switch(IAstTransformer transformer, out Node resultingNode)
		{
			Expression result = this;
			transformer.OnOmittedExpression(this, ref result);
			resultingNode = result;
		}
	}	
}
