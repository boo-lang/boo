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

namespace Boo.Lang.Compiler.TypeSystem.Services
{
	public static class MemberCollector
	{
		public static IEntity[] CollectAllMembers(INamespace entity)
		{
			var members = new List<IEntity>();
			CollectAllMembers(members, entity);
			return members.ToArray();
		}

		private static void CollectAllMembers(List<IEntity> members, INamespace entity)
		{
			var type = entity as IType;
			if (null != type)
			{
				members.ExtendUnique(type.GetMembers());
				CollectBaseTypeMembers(members, type.BaseType);
			}
			else
			{
				members.Extend(entity.GetMembers());
			}
		}

		private static void CollectBaseTypeMembers(List<IEntity> members, IType baseType)
		{
			if (null == baseType) return;

			members.Extend(baseType.GetMembers().Where(m => !(m is IConstructor) && !IsHiddenBy(m, members)));

			CollectBaseTypeMembers(members, baseType.BaseType);
		}

		private static bool IsHiddenBy(IEntity entity, IEnumerable<IEntity> members)
		{
			var m = entity as IMethod;
			if (m != null)
				return members.OfType<IMethod>().Any(existing => SameNameAndSignature(m, existing));
			return members.OfType<IEntity>().Any(existing => existing.Name == entity.Name);
		}

		private static bool SameNameAndSignature(IMethod method, IMethod existing)
		{
			if (method.Name != existing.Name)
				return false;
			return method.CallableType == existing.CallableType;
		}
	}
}
