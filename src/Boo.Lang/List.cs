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

#if !NO_SERIALIZATION_INFO
    [Serializable]
#endif
	public class List : List<object>
	{
		public static string operator%(string format, List rhs)
		{
			return string.Format(format, rhs.ToArray());
		}
		
		public List()
		{
		}

		public List(IEnumerable enumerable) : base(enumerable)
		{
		}

		public List(int initialCapacity) : base(initialCapacity)
		{
		}

		public List(object[] items, bool takeOwnership) : base(items, takeOwnership)
		{
		}

		public object Find(Predicate<object> predicate)
		{
			object found;
			return Find(predicate, out found) ? found : null;
		}

		protected override List<object> NewConcreteList(object[] items, bool takeOwnership)
		{
			return new List(items, takeOwnership);
		}

		public Array ToArray(Type targetType)
		{
			var target = Array.CreateInstance(targetType, _count);
			Array.Copy(_items, 0, target, 0, _count);
			return target;
		}
	}
}
