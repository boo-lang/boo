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
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[System.Xml.Serialization.XmlInclude(typeof(OmittedExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(MethodInvocationExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(UnaryExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(BinaryExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(TernaryExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(ReferenceExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(LiteralExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(StringFormattingExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(IteratorExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(SlicingExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(AsExpression))]
	[Serializable]
	public abstract class Expression : ExpressionImpl
	{		
		public Expression()
		{
 		}
 		
		public Expression(LexicalInfo lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
	}
}
