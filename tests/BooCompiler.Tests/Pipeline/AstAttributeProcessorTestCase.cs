#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
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
using Boo.Lang.Compiler.Steps;
using Boo.Lang.Compiler.Pipelines;
using NUnit.Framework;

namespace BooCompiler.Tests
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
		
		override protected CompilerPipeline SetUpCompilerPipeline()
		{
			CompilerPipeline pipeline = new Boo.Lang.Compiler.Pipelines.Parse();
			pipeline.Add(new InitializeNameResolutionService());
			pipeline.Add(new IntroduceGlobalNamespaces());	
			pipeline.Add(new BindNamespaces());
			pipeline.Add(new BindAndApplyAttributes());
			pipeline.Add(new PrintBoo());
			return pipeline;
		}
	}
}
