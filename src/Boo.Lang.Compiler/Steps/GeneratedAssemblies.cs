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

using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;

namespace Boo.Lang.Compiler.Steps;

/// <summary>
/// Loads the assemblies the compiler generates and keeps them resolvable
/// by name.
/// </summary>
/// <remarks>
/// An assembly loaded from an image is not bound by name, so a generated
/// assembly that references another one, which is what compiling from a
/// macro or from MetaProgramming.compile produces, cannot find it. They are
/// registered here and handed back from the default load context's
/// Resolving event. Nothing unloads them; assemblies in the default context
/// live as long as the process either way.
/// </remarks>
internal static class GeneratedAssemblies
{
	private static readonly Dictionary<string, Assembly> Loaded = new Dictionary<string, Assembly>();

	private static bool _resolving;

	internal static Assembly Load(byte[] image)
	{
		var assembly = Assembly.Load(image);
		lock (Loaded)
		{
			Loaded[assembly.GetName().Name] = assembly;
			if (!_resolving)
			{
				AssemblyLoadContext.Default.Resolving += Resolve;
				_resolving = true;
			}
		}
		return assembly;
	}

	private static Assembly Resolve(AssemblyLoadContext context, AssemblyName name)
	{
		lock (Loaded)
		{
			Loaded.TryGetValue(name.Name, out var assembly);
			return assembly;
		}
	}
}
