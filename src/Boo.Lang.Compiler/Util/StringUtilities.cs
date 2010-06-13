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

namespace Boo.Lang.Compiler.Util
{
	internal sealed class StringUtilities
	{
		public static string GetSoundex(string s)
		{
			if (s.Length < 2)
				return null;
			char[] code = "?0000".ToCharArray();
			string ws = s.ToLowerInvariant();
			int wsLen = ws.Length;
			char lastChar = ' ';
			int lastCharPos = 1;

			code[0] = ws[0];
			for (int i = 1; i < wsLen; i++)
			{
				char wsc = ws[i];
				char c = ' ';
				if (wsc == 'b' || wsc == 'f' || wsc == 'p' || wsc == 'v') c = '1';
				if (wsc == 'c' || wsc == 'g' || wsc == 'j' || wsc == 'k' || wsc == 'q' || wsc == 's' || wsc == 'x' || wsc == 'z') c = '2';
				if (wsc == 'd' || wsc == 't') c = '3';
				if (wsc == 'l') c = '4';
				if (wsc == 'm' || wsc == 'n') c = '5';
				if (wsc == 'r') c = '6';
				if (c == lastChar)
					continue;
				lastChar = c;
				if (c == ' ')
					continue;
				code[lastCharPos] = c;
				lastCharPos++;
				if (lastCharPos > 4)
					break;
			}
			return new string(code);
		}

		public static int GetDistance(string s, string t)
		{
			int n = s.Length;
			int m = t.Length;
			int[,] d = new int[n + 1, m + 1];
			int cost;
			if(n == 0)
				return m;
			if(m == 0)
				return n;
			for(int i = 0; i <= n; d[i, 0] = i++);
			for(int j = 0; j <= m; d[0, j] = j++);
			for(int i = 1; i <= n;i++) {
				for(int j = 1; j <= m;j++) {
					cost = (t.Substring(j - 1, 1) == s.Substring(i - 1, 1) ? 0 : 1);
					d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
									d[i - 1, j - 1] + cost);
				}
			}
			return d[n, m];
		}
	}
}
