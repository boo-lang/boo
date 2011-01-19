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

namespace Boo.Lang.Environments
{
	/// <summary>
	/// Holds the active dynamically scoped <see cref="IEnvironment">environment</see> instance.
	/// 
	/// The active environment is responsible for providing code with any <see cref="My&lt;TNeed&gt.Instance">needs</see>.
	/// 
	/// A particular <see cref="IEnvironment">environment</see> can be made active for the execution of a specific piece of code through <see cref="ActiveEnvironment.With" />.
	/// 
	/// The environment design pattern has been previously described <see cref="http://bamboo.github.com/2009/02/16/environment-based-programming.html">in this article</see>.
	/// </summary>
	public static class ActiveEnvironment
	{
		/// <summary>
		/// The active environment.
		/// </summary>
		public static IEnvironment Instance { get { return _instance; } }

		/// <summary>
		/// Executes <paramref name="code"/> in the specified <paramref name="environment"/>.
		/// </summary>
		/// <param name="environment">environment that should be made active during the execution of <paramref name="code"/></param>
		/// <param name="code">code to execute</param>
		public static void With(IEnvironment environment, Procedure code)
		{
			var previous = _instance;
			try
			{
				_instance = environment;
				code(); 
			}
			finally
			{
				_instance = previous;
			}
		}

		[ThreadStatic]
		private static IEnvironment _instance;
	}
}