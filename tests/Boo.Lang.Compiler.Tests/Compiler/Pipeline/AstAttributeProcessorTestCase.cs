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
using System.IO;
using Boo.Lang;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Pipeline;
using Boo.Lang.Compiler.Pipeline.Definitions;
using NUnit.Framework;

namespace Boo.Lang.Compiler.Tests
{
	public class AllParametersRequiredAttribute : AbstractAstAttribute
	{
		public AllParametersRequiredAttribute()
		{
		}

		override public void Apply(Node node)
		{
			Method m = node as Method;
			if (null == m)
			{
				throw new ApplicationException("This attribute can only be applied to methods.");
			}

			foreach (ParameterDeclaration pd in m.Parameters)
			{
				pd.Attributes.Add(new Boo.Lang.Compiler.Ast.Attribute("required"));
			}
		}
	}

	public class ViewStateAttribute : AbstractAstAttribute
	{
		Expression _default;

		public ViewStateAttribute()
		{
		}

		public Expression Default
		{
			get
			{
				return _default;
			}

			set
			{
				_default = value;
			}
		}

		override public void Apply(Node node)
		{
			Field f = (Field)node;

			Property p = new Property();
			p.Name = f.Name;
			p.Type = f.Type;
			p.Setter = CreateSetter(f);
			p.Getter = CreateGetter(f);

			((TypeDefinition)f.ParentNode).Members.Replace(f, p);			
		}

		Method CreateSetter(Field f)
		{
			Method m = new Method();
			m.Name = "set";
			m.Body.Statements.Add(
				new ExpressionStatement(
					new BinaryExpression(
						BinaryOperatorType.Assign,
						CreateViewStateSlice(f),
						new ReferenceExpression("value")
					)
				)
			);
			return m;
		}

		Method CreateGetter(Field f)
		{
			Method m = new Method();
			m.Name = "get";

			// value = ViewState["<f.Name>"]
			m.Body.Statements.Add(
				new ExpressionStatement(
					new BinaryExpression(
						BinaryOperatorType.Assign,
						new ReferenceExpression("value"),
						CreateViewStateSlice(f)
					)
				)
			);

			if (null != _default)
			{
				// return <_default> unless value
				ReturnStatement rs = new ReturnStatement(_default);
				rs.Modifier = new StatementModifier(StatementModifierType.Unless, new ReferenceExpression("value"));
				m.Body.Statements.Add(rs);
			}

			// return value
			m.Body.Statements.Add(
				new ReturnStatement(
					new ReferenceExpression("value")
				)
			);

			return m;
		}

		Expression CreateViewStateSlice(Field f)
		{
			// ViewState["<f.Name>"]
			return new SlicingExpression(
				new ReferenceExpression("ViewState"),
				new StringLiteralExpression(f.Name),
				null, 
				null
				);
		}
	}

	/// <summary>	
	/// </summary>
	[TestFixture]
	public class AttributesTestCase : AbstractCompilerTestCase
	{
		[Test]
		public void RequiredAttribute()
		{
			RunCompilerTestCase("required.boo");
		}

		[Test]
		public void GetterAttribute()
		{
			RunCompilerTestCase("getter.boo");
		}
		
		[Test]
		public void PropertyAttribute()
		{
			RunCompilerTestCase("property.boo");
		}

		[Test]
		public void AttributeGeneratingAttribute()
		{
			RunCompilerTestCase("allparametersrequired.boo");
		}

		[Test]
		public void AttributeWithNamedParameter()
		{
			RunCompilerTestCase("viewstate.boo");
		}
		
		override protected string GetTestCasePath(string fname)
		{
			return Path.Combine(
					Path.Combine(BooTestCaseUtil.TestCasesPath, "attributes"),
					fname);
		}
		
		override protected void SetUpCompilerPipeline(CompilerPipeline pipeline)
		{
			pipeline.Load(typeof(ParsePipelineDefinition));
			pipeline.Add(new ImportResolutionStep());
			pipeline.Add(new AstAttributesStep());
			pipeline.Add(new BooPrinterStep());
		}
	}
}
