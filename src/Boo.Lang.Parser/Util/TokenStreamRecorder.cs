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
using System.Collections;
using antlr;

namespace Boo.Lang.Parser.Util
{
	/// <summary>
	/// Records a stream of tokens for later playback.
	/// </summary>
	public class TokenStreamRecorder : TokenStream
	{
		TokenStreamSelector _selector;
		Queue _queue = new Queue();
	
		public TokenStreamRecorder(TokenStreamSelector selector)
		{
			_selector = selector;
		}
	
		public int Count
		{
			get
			{
				return _queue.Count;
			}
		}
	
		public void Enqueue(IToken token)
		{
			//Console.WriteLine("Enqueue({0})", token);
			_queue.Enqueue(token);
		}
		
		public IToken Dequeue()
		{
			 return (IToken)_queue.Dequeue();
		}
	
		public int RecordUntil(TokenStream stream, int closeToken, int openToken)
		{
			//Console.WriteLine("RecordUntil({0}, {1}, {2})", stream, closeToken, openToken);
			int cTokens = 0;
			
			int expectedCount = 1;
			IToken token = stream.nextToken();
			while (true)
			{			
				if (closeToken == token.Type)
				{
					--expectedCount;
					
					//Console.WriteLine("closeToken found. expecting: " + expectedCount);
					if (0 == expectedCount)
					{
						break;
					}
				}
				else if (openToken == token.Type)
				{
					++expectedCount;
					//Console.WriteLine("openToken found. expecting: " + expectedCount);
				}
				else if (token.Type < Token.MIN_USER_TYPE)
				{
					//Console.WriteLine("found {0} which is < Token.MIN_USER_TYPE", token.Type);
					break;
				}
				
				Enqueue(token);			
				++cTokens;			
				token = stream.nextToken();			
			}			
			//Console.WriteLine("RecordUntil() => {0}", cTokens);
			return cTokens;
		}
	
		public IToken nextToken()
		{
			if (_queue.Count > 0)
			{
				return Dequeue();
			}
			return _selector.pop().nextToken();
		}
	}
}
