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
	[System.Xml.Serialization.XmlInclude(typeof(DeclarationStatement))]
	[System.Xml.Serialization.XmlInclude(typeof(AssertStatement))]
	[System.Xml.Serialization.XmlInclude(typeof(TryStatement))]
	[System.Xml.Serialization.XmlInclude(typeof(IfStatement))]
	[System.Xml.Serialization.XmlInclude(typeof(ForStatement))]
	[System.Xml.Serialization.XmlInclude(typeof(WhileStatement))]
	[System.Xml.Serialization.XmlInclude(typeof(GivenStatement))]
	[System.Xml.Serialization.XmlInclude(typeof(BreakStatement))]
	[System.Xml.Serialization.XmlInclude(typeof(ContinueStatement))]
	[System.Xml.Serialization.XmlInclude(typeof(RetryStatement))]
	[System.Xml.Serialization.XmlInclude(typeof(ReturnStatement))]
	[System.Xml.Serialization.XmlInclude(typeof(YieldStatement))]
	[System.Xml.Serialization.XmlInclude(typeof(RaiseStatement))]
	[System.Xml.Serialization.XmlInclude(typeof(UnpackStatement))]
	[System.Xml.Serialization.XmlInclude(typeof(ExpressionStatement))]
	[Serializable]
	public abstract class Statement : StatementImpl
	{		
		public Statement()
		{
 		}
		
		public Statement(StatementModifier modifier) : base(modifier)
		{
		}	
		
		public Statement(LexicalInfo lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public void ReplaceBy(Statement other)
		{
			Block block = (Block)ParentNode;
			if (null == block)
			{
				throw new InvalidOperationException();
			}
			
			block.Statements.Replace(this, other);
		}
	}
}
