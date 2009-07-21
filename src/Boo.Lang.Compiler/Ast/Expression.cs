#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;

namespace Boo.Lang.Compiler.Ast
{
	[System.Xml.Serialization.XmlInclude(typeof(BinaryExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(BlockExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(CastExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(ConditionalExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(ExpressionInterpolationExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(ExtendedGeneratorExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(GeneratorExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(QuasiquoteExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(LiteralExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(MethodInvocationExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(OmittedExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(ReferenceExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(SlicingExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(SpliceExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(TryCastExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(TypeofExpression))]
	[System.Xml.Serialization.XmlInclude(typeof(UnaryExpression))]
	public abstract partial class Expression
	{
		public static Expression Lift(string s)
		{
			return new StringLiteralExpression(s);
		}

		public static Expression Lift(char b)
		{
			return new CharLiteralExpression(b);
		}

		public static Expression Lift(byte b)
		{
			return new IntegerLiteralExpression(b);
		}

		public static Expression Lift(bool b)
		{
			return new BoolLiteralExpression(b);
		}

		public static Expression Lift(short s)
		{
			return new IntegerLiteralExpression(s);
		}

		public static Expression Lift(int i)
		{
			return new IntegerLiteralExpression(i);
		}

		public static Expression Lift(long l)
		{
			return new IntegerLiteralExpression(l);
		}

		public static Expression Lift(float f)
		{
			return new DoubleLiteralExpression(f, true);
		}

		public static Expression Lift(double d)
		{
			return new DoubleLiteralExpression(d);
		}

		public static Expression Lift(System.Text.RegularExpressions.Regex regex)
		{
			return new RELiteralExpression(string.Format("/{0}/", regex));
		}

		public static Expression Lift(TimeSpan ts)
		{
			return new TimeSpanLiteralExpression(ts);
		}

		public static Expression Lift(Block block)
		{
			return new BlockExpression(block);
		}
		
		public static Expression Lift(Expression e)
		{
			return e.CloneNode();
		}

		public static Expression Lift(ParameterDeclaration p)
		{
			return new ReferenceExpression(p.LexicalInfo, p.Name);
		}
		
		public static Expression Lift(TypeDefinition type)
		{
			return new TypeofExpression(type.LexicalInfo, TypeReference.Lift(type));
		}
		
		protected Boo.Lang.Compiler.TypeSystem.IType _expressionType;
		
		public Expression()
		{
 		}
 		
		public Expression(LexicalInfo lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		[System.Xml.Serialization.XmlIgnore]
		public Boo.Lang.Compiler.TypeSystem.IType ExpressionType
		{
			get
			{
				return _expressionType;
			}
			
			set
			{
				_expressionType = value;
			}
		}
	}
}
