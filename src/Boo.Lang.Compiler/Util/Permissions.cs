#region license
// Copyright (c) 2009, Rodrigo B. de Oliveira (rbo@acm.org)
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
	/// <summary>
	/// Runs work that used to be guarded by a code access permission.
	/// </summary>
	/// <remarks>
	/// Code access security was removed from .NET: the permission types throw
	/// wherever they survive, and there is nothing to demand against. The calls
	/// stay so the callers read the same, but the work now simply runs, and a
	/// failure is still swallowed the way a refused permission was.
	/// </remarks>
	internal static class Permissions
	{
		public static T WithEnvironmentPermission<T>(Func<T> function)
		{
			return Attempt(function);
		}

		public static T WithDiscoveryPermission<T>(Func<T> function)
		{
			return Attempt(function);
		}

		public static void WithAppDomainPermission(Action action)
		{
			Attempt(() => { action(); return false; });
		}

		private static T Attempt<T>(Func<T> function)
		{
			try
			{
				return function();
			}
			catch (Exception)
			{
				return default(T);
			}
		}
	}
}
