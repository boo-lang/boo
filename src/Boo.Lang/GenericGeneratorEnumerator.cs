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
	using System.Collections;
	using System.Collections.Generic;

	public abstract class GenericGeneratorEnumerator<T> : IEnumerator<T>
	{
		protected T _current;

		// HACK: _state should be protected, not public.
		// Marked public because of a bug in Mono. See BOO-796 and
		// http://lists.ximian.com/pipermail/mono-devel-list/2007-February/022431.html
		public int _state;

		public GenericGeneratorEnumerator()
		{
			_state = 0;
		}

		public T Current
		{
			get { return _current; }
		}

		object IEnumerator.Current
		{
			get { return _current; }
		}

		public virtual void Dispose()
		{
		}

		public void Reset()
		{
			// We cannot reset the local variables of the generator method,
			// so let's do it like C# and throw a NotSupportedException
			throw new NotSupportedException();
		}

		public abstract bool MoveNext();

		protected bool Yield(int state, T value)
		{
			_state = state;
			_current = value;
			return true;
		}

		protected bool YieldDefault(int state)
		{
			_state = state;
			_current = default(T);
			return true;
		}
	}
}