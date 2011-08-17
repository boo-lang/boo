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

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Boo.Lang.Compiler.TypeSystem.Services
{
	public class EntityFormatter
	{
		public virtual string FormatType(IType type)
		{
			if (type is IGenericParameter)
				return type.Name;
			
			var callableType = type as ICallableType;
			if (callableType != null && callableType.IsAnonymous)
				return callableType.GetSignature().ToString();

			var generics = GenericsFrom(type.ConstructedInfo, type.GenericInfo);
			if (generics != null)
				return FormatGenericType(type.FullName, generics);

			return type.FullName;
		}

		public virtual string FormatTypeMember(IMember member)
		{
			var method = member as IMethod;
			if (method != null)
				return FormatMethod(method);
			return FormatType(member.DeclaringType) + "." + member.Name;
		}

		protected virtual string FormatMethod(IMethod method)
		{
			return FormatType(method.DeclaringType) + "." + FormatSignature(method);
		}

		protected virtual string FormatGenericType(string typeName, IEnumerable<string> genericArgs)
		{
			return typeName + FormatGenericArguments(genericArgs);
		}

		protected virtual string FormatGenericArguments(IEnumerable<string> genericArgs)
		{
			return string.Format("[of {0}]", Builtins.@join(genericArgs, ", "));
		}

		public virtual string FormatSignature(IEntityWithParameters method)
		{
			var buffer = new StringBuilder(method.Name);

			var genericArgs = GenericArgumentsOf(method);
			if (genericArgs != null)
				buffer.Append(FormatGenericArguments(genericArgs));

			buffer.Append("(");
			var parameters = method.GetParameters();
			var varArgsIndex = method.AcceptVarArgs ? parameters.Length - 1 : -1;
			for (var i = 0; i < parameters.Length; ++i)
			{
				if (i > 0)
					buffer.Append(", ");

				if (i == varArgsIndex)
					buffer.Append('*');

				buffer.Append(FormatType(parameters[i].Type));
			}
			buffer.Append(")");
			return buffer.ToString();
		}

		private IEnumerable<string> GenericsFrom(IGenericArgumentsProvider genericArgumentsProvider, IGenericParametersProvider genericParametersProvider)
		{
			if (genericArgumentsProvider != null)
				return FormatTypes(genericArgumentsProvider.GenericArguments);

			if (genericParametersProvider != null)
				return genericParametersProvider.GenericParameters.Select(p => p.Name);

			return null;
		}

		private IEnumerable<string> GenericArgumentsOf(IEntityWithParameters entity)
		{
			var method = entity as IMethod;
			if (method != null)
				return GenericsFrom(method.ConstructedInfo, method.GenericInfo);
			return null;
		}

		private IEnumerable<string> FormatTypes(IEnumerable<IType> genericArguments)
		{
			return genericArguments.Select(a => FormatType(a));
		}
	}
}