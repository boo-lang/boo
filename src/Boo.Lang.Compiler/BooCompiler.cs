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
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler;

namespace Boo.Lang.Compiler
{
	/// <summary>
	/// The compiler: a facade to the CompilerParameters/CompilerContext/Pipeline subsystem.
	/// </summary>
	public class BooCompiler : System.MarshalByRefObject
	{
		CompilerParameters _parameters;

		public BooCompiler()
		{
			_parameters = new CompilerParameters();
		}

		public CompilerParameters Parameters
		{
			get
			{
				return _parameters;
			}
		}

		public CompilerContext Run()
		{
			if (null == _parameters.Pipeline)
			{
				throw new InvalidOperationException(Boo.ResourceManager.GetString("BooC.CantRunWithoutPipeline"));
			}
			CompilerContext context = new CompilerContext(_parameters, new CompileUnit());
			_parameters.Pipeline.Run(context);
			return context;
		}		
	}
}
