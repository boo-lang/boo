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

namespace Boo.Lang
{
	using System;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;

	/// <summary>
	/// debug foo, bar
	/// debug
	/// debug "Hello"
	///
	/// You can view debug messages by:
	/// A. Pass the "-d" option to booi, if you have the patched booi version.
	///    *Note* debug is already on by default in booi and booc.  This option just
	///    prints debug messages to System.Console.Error as well.  To turn debugging
	///    completely OFF, pass "-d-" or "-debug-" to booi or booc.
	/// B. In Windows: using DebugView
	///     http://www.sysinternals.com/ntw2k/freeware/debugview.shtml
	/// C. In Mono (not tested): export MONO_TRACE=debug=Console.Out
	/// D. In your boo script: add a few lines like:
	///    import System.Diagnostics
	///    Debug.Listeners.Add(TextWriterTraceListener(System.Console.Error))
	///    #Trace.Listeners.Add(TextWriterTraceListener(System.Console.Error)) 
	///      #(if you want to print Trace messages as well)
	///
	/// To turn debugging off in booc or booi, use the "-d-" or "-debug-" command-line option
	/// ("-d" or "-debug+" turns debugging on, but it is already on by default)
	///
	/// </summary>
	public class DebugMacro : AbstractPrintMacro
	{
		static Expression Debug_WriteLine = AstUtil.CreateReferenceExpression("System.Diagnostics.Debug.WriteLine");
		
		static Expression Debug_Write = AstUtil.CreateReferenceExpression("System.Diagnostics.Debug.Write");
		
		override public Statement Expand(MacroStatement macro)
		{
			if (!Context.Parameters.Debug)
			{
				return null;
			}
			
			if (0 == macro.Arguments.Count)
			{
				return new ExpressionStatement(
					AstUtil.CreateMethodInvocationExpression(
						Debug_WriteLine.CloneNode(),
						new StringLiteralExpression("<debug>")));
			}
			
			return Expand(macro, Debug_Write, Debug_WriteLine);
		}
	}
}

