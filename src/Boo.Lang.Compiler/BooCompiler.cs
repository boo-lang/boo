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
using System.IO;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler;

namespace Boo.Lang.Compiler
{
	/// <summary>
	/// The compiler: a facade to the CompilerParameters/CompilerContext/Pipeline subsystem.
	/// </summary>
	public class BooCompiler
	{
		private readonly CompilerParameters _parameters;

		public BooCompiler()
		{
			_parameters = new CompilerParameters();
		}
		
		public BooCompiler(CompilerParameters parameters)
		{
			if (null == parameters)
				throw new ArgumentNullException("parameters");
			_parameters = parameters;
		}

		public CompilerParameters Parameters
		{
			get { return _parameters; }
		}
		
		public CompilerContext Run(CompileUnit compileUnit)
		{
			if (null == compileUnit)
				throw new ArgumentNullException("compileUnit");
			if (null == _parameters.Pipeline)
				throw new InvalidOperationException(Boo.Lang.ResourceManager.GetString("BooC.CantRunWithoutPipeline"));
			
			CompilerContext context = new CompilerContext(_parameters, compileUnit);
			_parameters.Pipeline.Run(context);
			return context;
		}

		public CompilerContext Run()
		{
			return Run(new CompileUnit());
		}
		
	}
}
