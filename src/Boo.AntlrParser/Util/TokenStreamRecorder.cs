#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;
using System.Collections;
using antlr;

namespace Boo.AntlrParser.Util
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
	
		public void Enqueue(Token token)
		{
			_queue.Enqueue(token);
		}
	
		public int RecordUntil(TokenStream stream, int ttype)
		{
			int cTokens = 0;
		
			ods("> RecordUntil");
			Token token = stream.nextToken();
			while (ttype != token.Type)
			{			
				if (token.Type < Token.MIN_USER_TYPE)
				{
					break;
				}
			
				ods("  > {0}", token);
				_queue.Enqueue(token);			
			
				++cTokens;			
				token = stream.nextToken();			
			}
			ods("< RecordUntil");
			return cTokens;
		}
	
		public Token nextToken()
		{
			if (_queue.Count > 0)
			{
				return (Token)_queue.Dequeue();
			}
			return _selector.pop().nextToken();
		}
	
		void ods(string s, params object[] args)
		{
			//Console.WriteLine(s, args);
		}
	}
}