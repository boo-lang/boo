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

namespace Boo.Lang.Compiler.Steps
{
	using System;
	using System.ComponentModel;
	using System.Diagnostics;
	using Boo.Lang.Compiler;

	public class PEVerify : AbstractCompilerStep
	{
		public static bool IsSupported
		{
			get
			{
				return 128 != (int)System.Environment.OSVersion.Platform;
			}
		}
		
		override public void Run()
		{			
			if (Errors.Count > 0)
			{
				return;
			}			
			
			if (!IsSupported)
			{
				_context.TraceWarning("PEVerify is not supported on this platform.");
				// linux
				return;
			}
			
			try
			{
				Process p = Boo.Lang.Builtins.shellp("peverify.exe", Parameters.OutputAssembly);
				p.WaitForExit();
				if (0 != p.ExitCode)
				{
					Errors.Add(new CompilerError(Boo.Lang.Compiler.Ast.LexicalInfo.Empty, p.StandardOutput.ReadToEnd()));
				}
			}
			catch (Win32Exception e)
            {
                _context.TraceWarning("Could not start peverify.exe: " + e.Message);
			}
		}
	}
}
