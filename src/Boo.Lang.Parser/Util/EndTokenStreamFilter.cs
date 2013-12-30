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
using antlr;

namespace Boo.Lang.Parser.Util
{
	/// <summary>
	/// Replace END tokens with ID ones. This is needed to make the END keyword
	/// behave like a normal identifier when a lexer is in non-white space agnostic
	/// mode.
	/// </summary>
	public class EndTokenStreamFilter : TokenStream
	{		
		/// <summary>
		/// token input stream.
		/// </summary>
		protected TokenStream _istream;

		/// <summary>
		/// singleton END token.
		/// </summary>
		protected int _endTokenType;

		/// <summary>
		/// singleton ID token.
		/// </summary>
		protected int _idTokenType;

		public EndTokenStreamFilter(TokenStream istream, int endType, int idType)
		{
			if (null == istream)
			{
				throw new ArgumentNullException("istream");
			}

			_istream = istream;
			_endTokenType = endType;
			_idTokenType = idType;
		}

		public TokenStream InnerStream
		{
			get { return _istream; }
		}

		public IToken nextToken()
		{
			IToken token = _istream.nextToken();

			// In non-wsa mode `end` is just another identifier
			if (token.Type == _endTokenType) {
				token.Type = _idTokenType;
			}
			
			return token;
		}				
	}

}