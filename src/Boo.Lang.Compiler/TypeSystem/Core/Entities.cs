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


using System.Collections.Generic;
using Boo.Lang.Compiler.Steps;
using Boo.Lang.Compiler.Util;

namespace Boo.Lang.Compiler.TypeSystem.Core
{
	public static class Entities
	{
		public static IEntity PreferInternalEntitiesOverExternalOnes(IEntity entity)
		{
			Ambiguous ambiguous = entity as Ambiguous;
			if (null == ambiguous)
				return entity;

			bool isAmbiguousBetweenInternalAndExternalEntities = ambiguous.Any(EntityPredicates.IsInternalEntity)
			                                                     && ambiguous.Any(EntityPredicates.IsNonInternalEntity);
			if (!isAmbiguousBetweenInternalAndExternalEntities)
				return entity;

			return EntityFromList(ambiguous.Select(EntityPredicates.IsInternalEntity));
		}

		public static IEntity EntityFromList(ICollection<IEntity> entities)
		{
			switch (entities.Count)
			{
				case 0:
					return null;
				case 1:
					return Collections.First(entities);
				default:
					return new Ambiguous(entities);
			}
		}

		public static bool IsFlagSet(EntityType flags, EntityType flag)
		{
			return flag == (flags & flag);
		}
	}
}
