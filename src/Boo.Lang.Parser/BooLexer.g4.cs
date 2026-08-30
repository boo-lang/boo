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

using System.Collections.Generic;

/// <summary>
/// The helpers BooLexer.g4's actions call.
/// </summary>
partial class BooLexer
{
	protected int _skipWhitespaceRegion = 0;

	private readonly Stack<int> _beginInterpolationType = new();
	private readonly Stack<int> _endInterpolationType = new();

	private bool SkipWhitespace => _skipWhitespaceRegion > 0;

	private static bool IsDigit(int ch) => ch >= '0' && ch <= '9';

	private void EnterSkipWhitespaceRegion() => _skipWhitespaceRegion++;

	private void LeaveSkipWhitespaceRegion() => _skipWhitespaceRegion--;

	private void HandleInterpolatedExpression(int beginInterpolationType, int endTokenType)
	{
		_beginInterpolationType.Push(beginInterpolationType);
		_endInterpolationType.Push(endTokenType);
		PushMode(DEFAULT_MODE);
	}

	private void HandleInterpolationToken(int type)
	{
		if (_beginInterpolationType.Count == 0)
			return;

		if (_beginInterpolationType.Peek() == type)
		{
			PushMode(DEFAULT_MODE);
		}
		else if (_endInterpolationType.Peek() == type)
		{
			PopMode();
			if (CurrentMode != DEFAULT_MODE)
			{
				_beginInterpolationType.Pop();
				_endInterpolationType.Pop();
			}
		}
	}
}
