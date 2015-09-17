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

namespace Boo.Lang.ParserV4
{
	using System;
	using Antlr4.Runtime;
	
	/// <summary>
	/// A token that stores filename information.
	/// </summary>
	public class BooTokenV4 : Antlr4.Runtime.CommonToken
	{
		public static readonly Antlr4.Runtime.ITokenFactory TokenCreator = new BooTokenCreator();
		
		protected string _fname;

		public BooTokenV4(int type) : base(type)
		{
		}

		public BooTokenV4(int type, string text) : base(type, text)
		{
		}

		public BooTokenV4(Tuple<ITokenSource, ICharStream> source, int type, int channel, int start, int stop): base(source, type, channel, start, stop)
		{
		}

		public BooTokenV4(int type, string text, string fname, int start, int stop, int line, int column)
			: base(type, text)
		{
			setFilename(fname);
			this.StartIndex = start;
			this.StopIndex = stop;
			this.Line = line;
			this.Column = column;
		}

		public void setFilename(string name)
		{
			_fname = name;
		}

		public string getFilename()
		{
			return _fname;
		}
		
		public class BooTokenCreator : CommonTokenFactory
		{
			
			override public CommonToken Create(Tuple<ITokenSource, ICharStream> source, int type, string text, int channel, int start, int stop, int line, int charPositionInLine)
			{
				var result = new BooTokenV4(source, type, channel, start, stop);
				result.Line = line;
				result.Column = charPositionInLine;
				if (text != null)
				{
					result.Text = text;
				}
				else
				{
					if (this.copyText && source.Item2 != null)
					{
						result.Text = source.Item2.GetText(Antlr4.Runtime.Misc.Interval.Of(start, stop));
					}
				}
				return result;
			}
			
			override public CommonToken Create(int type, string text)
			{
				return new BooTokenV4(type, text);
			}
		}
	}
}
