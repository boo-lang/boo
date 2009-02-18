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
