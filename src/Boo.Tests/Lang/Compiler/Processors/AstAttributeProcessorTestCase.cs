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
using Boo.Lang.Ast;
using Boo.AntlrParser;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Pipeline;
using NUnit.Framework;
using Boo.Tests;

namespace Boo.Tests.Lang.Compiler.Pipeline
{
	/// <summary>
	/// Um exemplo de atributo que adiciona o atributo required
	/// a todos os atributos de um mtodo.
	/// </summary>
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
				pd.Attributes.Add(new Boo.Lang.Ast.Attribute("required"));
			}
		}
	}

	public class ViewStateAttribute : AbstractAstAttribute
	{
		Expression _default;

		public ViewStateAttribute()
		{
		}

		/// <summary>
		/// Valor do campo quando no existe ou  nulo no ViewState.
		/// </summary>
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
	public class AstAttributesStepTestCase : Assertion
	{
		[Test]
		public void TestRequiredAttribute()
		{
			RunTestCase("AttributeProcessor_1_expected.boo", "AttributeProcessor_1.boo");
		}

		[Test]
		public void TestGetterAttribute()
		{
			RunTestCase("AttributeProcessor_2_expected.boo", "AttributeProcessor_2.boo");
		}

		[Test]
		public void TestAttributeGeneratingAttribute()
		{
			string actual = @"

import Boo.Tests.Lang.Compiler.Pipeline

class Customer:
	[AllParametersRequired]
	def constructor(fname as string, lname as string):
		pass
";

			string expected = @"

import Boo.Tests.Lang.Compiler.Pipeline

class Customer:	
	def constructor(fname as string, lname as string):
		raise System.ArgumentNullException('fname') if fname is null
		raise System.ArgumentNullException('lname') if lname is null
";
			RunStringTestCase("[ata generating ata]", expected, actual);
			
		}

		[Test]
		public void TestAttributeWithNamedParameter()
		{
			string actual = @"
import Boo.Tests.Lang.Compiler.Pipeline
import System.Web.UI from System.Web

class MyControl(System.Web.UI.Control):
	[ViewState(Default: 70)]
	Width as int
";
			string expected = @"
import Boo.Tests.Lang.Compiler.Pipeline
import System.Web.UI from System.Web

class MyControl(System.Web.UI.Control):
	Width as int:
		get:
			value = ViewState['Width']	
			return 70 unless value
			return value
		set:
			ViewState['Width'] = value
";
			RunStringTestCase("[atributo com parmetro nomeado]", expected, actual);
		}

		public void RunTestCase(string expectedFile, string actualFile)
		{
			CompileUnit cu = RunCompiler(new FileInput(GetTestCasePath(actualFile)));
			cu.Modules[0].Name = BooParser.CreateModuleName(expectedFile);
			AssertEquals("[required]", ParseTestCase(expectedFile), cu);
		}		

		public void RunStringTestCase(string message, string expected, string actual)
		{
			CompileUnit cu = RunCompiler(new StringInput("<actual>", actual));
			cu.Modules[0].Name = BooParser.CreateModuleName("<expected>");
			AssertEquals(message, ParseString("<expected>", expected), cu);
		}

		CompileUnit RunCompiler(ICompilerInput input)
		{
			BooCompiler compiler = new BooCompiler();
			CompilerParameters options = compiler.Parameters;
			options.Input.Add(input);
			options.Pipeline.Add(new BooParsingStep());
			options.Pipeline.Add(new ImportResolutionStep());
			options.Pipeline.Add(new AstAttributesStep());
			options.References.Add(GetType().Assembly);
			
			CompilerContext context = compiler.Run();

			if (context.Errors.Count > 0)
			{
				Fail(context.Errors[0].ToString());
			}

			return context.CompileUnit;
		}

		void AssertEquals(string message, CompileUnit expected, CompileUnit actual)
		{
			BooTestCaseUtil.AssertEquals(message, expected, actual);
		}

		CompileUnit ParseTestCase(string fname)
		{
			return BooParser.ParseFile(GetTestCasePath(fname));
		}

		CompileUnit ParseString(string name, string text)
		{
			return BooParser.ParseReader(name, new StringReader(text));
		}

		string GetTestCasePath(string fname)
		{
			return Path.Combine(BooTestCaseUtil.GetTestCasePath("compilation"), fname);
		}
	}
}
