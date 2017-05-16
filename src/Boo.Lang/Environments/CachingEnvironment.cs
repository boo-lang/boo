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
using System.Collections.Generic;
#if DNXCORE50
using System.Reflection;
#endif

namespace Boo.Lang.Environments
{
	public class CachingEnvironment : IEnvironment
	{
		private readonly Dictionary<Type, object> _cache = new Dictionary<Type,object>();
		private readonly IEnvironment _source;

		public event Action<object> InstanceCached;

		public CachingEnvironment(IEnvironment source)
		{
			_source = source;
		}

		public TNeed Provide<TNeed>() where TNeed : class
		{
			object cached;
			if (_cache.TryGetValue(typeof(TNeed), out cached))
				return (TNeed) cached;

			foreach (var instance in _cache.Values)
				if (instance is TNeed)
				{
					_cache.Add(typeof(TNeed), instance);
					return (TNeed) instance;
				}

			var newInstance = _source.Provide<TNeed>();
			if (newInstance != null)
				Add(typeof(TNeed), newInstance);
			return newInstance;
		}

		public void Add(Type type, object instance)
		{
#if !DNXCORE50
			if (!type.IsInstanceOfType(instance))
				throw new ArgumentException(string.Format("{0} is not an instance of {1}", instance, type));
#else
		    if (!type.GetTypeInfo().IsInstanceOfType(instance))
		        throw new ArgumentException(string.Format("{0} is not an instance of {1}", instance, type));
#endif

			_cache.Add(type, instance);

			if (null != InstanceCached)
				InstanceCached(instance);
		}
	}
}
