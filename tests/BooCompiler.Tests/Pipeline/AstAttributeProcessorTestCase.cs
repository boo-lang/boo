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

namespace BooCompiler.Tests
{
	using System;
	using System.IO;
	using Boo.Lang;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.IO;
	using Boo.Lang.Compiler.Steps;
	using Boo.Lang.Compiler.Pipelines;
	using NUnit.Framework;

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
