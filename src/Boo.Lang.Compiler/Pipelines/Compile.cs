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

namespace Boo.Lang.Compiler.Pipelines
{
	using System;
	using Boo.Lang.Compiler.Steps;
	
	public class Compile : Parse
	{
		public Compile()
		{			
			Add(new InitializeNameResolutionService());
			Add(new IntroduceGlobalNamespaces());
			Add(new BindTypeDefinitions());			
			Add(new BindNamespaces());
			Add(new BindBaseTypes());
			Add(new BindAndApplyAttributes());
			Add(new ExpandMacros());
			Add(new IntroduceModuleClasses());
			Add(new NormalizeTypeMembers());
			Add(new NormalizeStatementModifiers());
			
			// todo: run this 2 steps again only if
			// any attributes or mixins were 
			// applied
			Add(new BindTypeDefinitions());
			Add(new BindBaseTypes());
			
			Add(new BindTypeMembers());			
			Add(new ProcessMethodBodies());
			Add(new ProcessGenerators());
			Add(new CheckInterfaceImplementations());
			Add(new InjectCasts());			
		}
	}
}
