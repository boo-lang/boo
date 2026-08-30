#region license
// Copyright (c) the Boo contributors
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

namespace Boo.Lang.Parser;

using System;
using Antlr4.Runtime;

/// <summary>
/// A token that stores filename information.
/// </summary>
public class BooToken : Antlr4.Runtime.CommonToken
{
	public static Antlr4.Runtime.ITokenFactory CreateTokenFactory(int tabSize)
	{
		return new BooTokenCreator(tabSize);
	}
	
	protected string _fname;

	private bool _magic;

	public BooToken(int type) : base(type)
	{
	}

	public BooToken(int type, string text) : base(type, text)
	{
	}

	public BooToken(Tuple<ITokenSource, ICharStream> source, int type, int channel, int start, int stop): base(source, type, channel, start, stop)
	{
	}

	public BooToken(Tuple<ITokenSource, ICharStream> source, int type, string text, string fname, int start, int stop, int line, int column, bool magic)
		: base(type, text)
	{
		setFilename(fname);
		this.source = source;
		this.StartIndex = start;
		this.StopIndex = stop;
		this.Line = line;
		this.Column = column;
		this._magic = magic;
	}

	public void setFilename(string name)
	{
		_fname = name;
	}

	public string getFilename()
	{
		return _fname;
	}
	
	public bool MagicToken => _magic;
	
	public class BooTokenCreator : CommonTokenFactory
	{
		private readonly int _tabSize;

		public BooTokenCreator(int tabSize)
		{
			_tabSize = tabSize;
		}

		override public CommonToken Create(Tuple<ITokenSource, ICharStream> source, int type, string text, int channel, int start, int stop, int line, int charPositionInLine)
		{
			var result = new BooToken(source, type, channel, start, stop);
			result.Line = line;
			result.Column = ColumnOf(source.Item2, start, charPositionInLine);
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
		
		/// <summary>
		/// The 1 based column of a token, counting a tab as a jump to the next
		/// tab stop. ANTLR 4 counts characters, so the text before the token on
		/// its line has to be measured again here.
		/// </summary>
		private int ColumnOf(ICharStream input, int start, int charPositionInLine)
		{
			if (input == null || charPositionInLine <= 0)
				return charPositionInLine + 1;

			var lineStart = start - charPositionInLine;
			if (lineStart < 0)
				return charPositionInLine + 1;

			var prefix = input.GetText(Antlr4.Runtime.Misc.Interval.Of(lineStart, start - 1));
			var column = 1;
			foreach (var c in prefix)
				column = c == '\t' ? ((column - 1) / _tabSize + 1) * _tabSize + 1 : column + 1;

			return column;
		}

		override public CommonToken Create(int type, string text)
		{
			return new BooToken(type, text);
		}
	}
}
