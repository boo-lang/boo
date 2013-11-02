#region license
// Copyright (c) 2009 Rodrigo B. de Oliveira (rbo@acm.org)
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

namespace Boo.Lang.Parser
{
	public class ParserSettings
	{
		public const int DefaultTabSize = 4;
		
		private int _tabSize = DefaultTabSize;

		public int TabSize
		{
			get { return _tabSize; }

			set
			{
				if (value < 1) throw new ArgumentOutOfRangeException("TabSize");
				_tabSize = value;
			}
		}

		public ParserErrorHandler ErrorHandler { get; set; }

		/// <summary>
		/// If different to 0 will force the parser to trigger an error if the rules
		/// exceed the recursion limit.
		/// </summary>
		/// <remarks>
		/// The parser must be build with the antlr `-traceParser` (Nant: antlr.trace=true) option 
		/// for this to have any actual effect.
		/// </remarks>		
		public uint MaxRecursionLimit { get; set; }

		public ErrorPattern[] ErrorPatterns { get; set; }


		public ParserSettings()
		{
			// This number comes from issue #56, eight or more '[' will trigger a StackOverflow under .Net
			MaxRecursionLimit = 133;

			// Assign the generated error patterns by default
			ErrorPatterns = GeneratedErrorPatterns.Patterns;
		}
	}
}
