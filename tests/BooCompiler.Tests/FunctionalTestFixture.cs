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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Steps;
using Boo.Lang.Compiler.Pipelines;
using NUnit.Framework;

namespace BooCompiler.Tests
{
	[TestFixture]
	public class FunctionalTestFixture : AbstractCompilerTestCase
	{
		protected override CompilerPipeline SetUpCompilerPipeline()
		{
			CompilerPipeline pipeline = new CompileToFile();
			pipeline.Add(new RunAssembly());
			return pipeline;
		}
		
		override protected string GetTestCasePath(string fname)
		{
			return Path.Combine(
					Path.Combine(Path.GetDirectoryName(_baseTestCasesPath), "functional"),
					fname);
		}
		
		[Test]
		public void RecursiveFunctionOverAst()
		{
			RunCompilerTestCase("RecursiveFunctionOverAst.boo");
		}
	}
}
