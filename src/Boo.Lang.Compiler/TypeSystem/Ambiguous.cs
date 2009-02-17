#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
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

namespace Boo.Lang.Compiler.TypeSystem
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public delegate bool EntityPredicate(IEntity entity);
	
	public class Ambiguous : IEntity
	{
		public static readonly IEntity[] NoEntities = new IEntity[0];
		
		private IEntity[] _entities;
		
		public Ambiguous(ICollection<IEntity> entities) : this(ToArray(entities))
		{
		}
		
		public Ambiguous(IEntity[] entities)
		{
			if (null == entities) throw new ArgumentNullException("entities");
			if (0 == entities.Length) throw new ArgumentException("entities");
			_entities = entities;
		}
		
		public string Name
		{
			get { return _entities[0].Name; }
		}
		
		public string FullName
		{
			get { return _entities[0].FullName; }
		}
		
		public EntityType EntityType
		{
			get { return EntityType.Ambiguous; }
		}
		
		public IEntity[] Entities
		{
			get { return _entities; }
		}
		
		public List<IEntity> Select(EntityPredicate predicate)
		{
			List<IEntity> found = new List<IEntity>();
			foreach (IEntity entity in _entities)
			{
				if (predicate(entity)) found.Add(entity);
			}
			return found;
		}
		
		override public string ToString()
		{
			return string.Format("Ambiguous({0})", Builtins.join(_entities, ", "));
		}
		
		private static IEntity[] ToArray(ICollection<IEntity> entities)
		{
			if (entities.Count == 0) return NoEntities;
			IEntity[] array = new IEntity[entities.Count];
			entities.CopyTo(array, 0);
			return array;
		}

		public bool AllEntitiesAre(EntityType entityType)
		{
			foreach (IEntity entity in _entities)
			{
				if (entityType != entity.EntityType) return false;
			}
			return true;
		}

		public bool Any(EntityPredicate predicate)
		{
			foreach (IEntity entity in _entities)
			{
				if (predicate(entity)) return true;
			}
			return false;
		}
	}
}
