#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
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


namespace Boo.Lang.Parser
{
	public class DocStringFormatter
	{
		// Every newline becomes '\n', the blank lines at either end go, and the
		// indentation the body was written at is removed. The first line is left
		// as written: the opening quotes sit before it, so it has no indentation
		// of its own and any space there was meant.
		public static string Format(string s)
		{
			if (s.Length == 0) return string.Empty;

			var lines = new System.Collections.Generic.List<string>(
				s.Replace("\r\n", "\n").Split('\n'));

			int indent = CommonIndent(lines);
			for (int i = 1; i < lines.Count; ++i)
				lines[i] = lines[i].Length < indent ? lines[i].TrimStart() : lines[i].Substring(indent);

			while (lines.Count > 0 && lines[0].Length == 0)
				lines.RemoveAt(0);
			while (lines.Count > 0 && lines[lines.Count - 1].Length == 0)
				lines.RemoveAt(lines.Count - 1);

			return string.Join("\n", lines);
		}

		// The narrowest indentation any line under the first is written at.
		private static int CommonIndent(System.Collections.Generic.List<string> lines)
		{
			int indent = int.MaxValue;
			for (int i = 1; i < lines.Count; ++i)
			{
				string text = lines[i].TrimStart();
				if (text.Length == 0) continue;
				int width = lines[i].Length - text.Length;
				if (width < indent) indent = width;
			}
			return indent == int.MaxValue ? 0 : indent;
		}
	}
}
