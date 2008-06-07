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


namespace Boo.Lang.Compiler.Util
{
	using System;
	
	public class TypeUtilities
	{
		public static string GetFullName(Type type)
		{
			if (type.IsByRef) return "ref " + GetFullName(type.GetElementType());
			if (type.DeclaringType != null) return GetFullName(type.DeclaringType) + "." + TypeName(type);		
			
			// HACK: Some constructed generic types report a FullName of null
			if (type.FullName == null) 
			{
				string[] argumentNames = Array.ConvertAll<Type, string>(
					type.GetGenericArguments(),
					delegate(Type t) { return GetFullName(t); });
				
				return string.Format(
					"{0}[{1}]",
					GetFullName(type.GetGenericTypeDefinition()),
					string.Join(", ", argumentNames));
				
			}
			string name = TypeName(type.FullName);
			if (type.IsGenericTypeDefinition)
			{
				name = string.Format(
					"{0}[of {1}]", 
					name, 
					string.Join(", ", Array.ConvertAll<Type, string>(
						type.GetGenericArguments(), 
						TypeName)));
			}
			return name;
		}

	    public static string TypeName(Type type)
		{
			if (!type.IsGenericTypeDefinition) return type.Name;
			return TypeName(type.Name);
		}

		public static string TypeName(string typeName)
		{
			int index = typeName.LastIndexOf('`');
			if (index < 0) return typeName;
			return typeName.Substring(0, index);
		}
	}
}
