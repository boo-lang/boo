#region license
// Copyright (c) 2009 Rodrigo B. de Oliveira (rbo@acm.org)
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
using System.Collections.Generic;
#if DNXCORE50
using System.Reflection;
#endif

namespace Boo.Lang.Environments
{
	public delegate object ObjectFactory();

	public class DeferredEnvironment : IEnumerable<KeyValuePair<Type, ObjectFactory>>, IEnvironment
	{
		private readonly List<KeyValuePair<Type, ObjectFactory>> _bindings = new List<KeyValuePair<Type, ObjectFactory>>();

		public void Add(Type need, ObjectFactory binder)
		{
			_bindings.Add(new KeyValuePair<Type, ObjectFactory>(need, binder));
		}

		IEnumerator<KeyValuePair<Type, ObjectFactory>> IEnumerable<KeyValuePair<Type, ObjectFactory>>.GetEnumerator()
		{
			return _bindings.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _bindings.GetEnumerator();
		}

		TNeed IEnvironment.Provide<TNeed>()
		{
			foreach (var binding in _bindings)
#if !DNXCORE50
				if (typeof(TNeed).IsAssignableFrom(binding.Key))
					return (TNeed) binding.Value();
#else
		        if (typeof(TNeed).GetTypeInfo().IsAssignableFrom(binding.Key))
		            return (TNeed) binding.Value();
#endif
			return null;
		}
	}
}
