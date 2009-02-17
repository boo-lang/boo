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

using System.Collections.Generic;
using Boo.Lang.Compiler.TypeSystem.Services;

namespace Boo.Lang.Compiler.TypeSystem
{	
	/// <summary>
	/// A namespace.
	/// </summary>
	public interface INamespace : IEntity
	{					
		/// <summary>
		/// The parent namespace.
		/// </summary>
		INamespace ParentNamespace
		{
			get;
		}
		
		/// <summary>
		/// Resolves the name passed as argument to the appropriate elements
		/// in the namespace, all elements with the specified name must be
		/// added to the resultingSet.
		/// </summary>
		/// <param name="resultingSet">list where to put the found elements</param>
		/// <param name="name">name of the desired elements</param>
		/// <param name="typesToConsider">element filter</param>
		/// <returns>
		/// true if at least one element was added to the resultingSet, false
		/// otherwise.
		/// </returns>
		bool Resolve(ICollection<IEntity> resultingSet, string name, EntityType typesToConsider);
		
		/// <summary>
		/// Returns all members of this namespace.
		/// </summary>
		IEnumerable<IEntity> GetMembers();
	}

	public abstract class AbstractNamespace : INamespace
	{
		#region Implementation of IEntity

		public virtual string Name
		{
			get { return string.Empty; }
		}

		public virtual string FullName
		{
			get
			{
				INamespace parent = ParentNamespace;
				if (parent == null)
					return Name;

				string parentName = parent.FullName;
				return string.IsNullOrEmpty(parentName)
				       	? Name
				       	: parentName + "." + Name;
			}
		}

		public virtual EntityType EntityType
		{
			get { return EntityType.Namespace; }
		}

		#endregion

		#region Implementation of INamespace

		public virtual INamespace ParentNamespace
		{
			get { return null; }
		}

		public virtual bool Resolve(ICollection<IEntity> resultingSet, string name, EntityType typesToConsider)
		{
			return My<NameResolutionService>.Instance.Resolve(name, GetMembers(), typesToConsider, resultingSet);
		}

		public abstract IEnumerable<IEntity> GetMembers();

		#endregion

		public override string ToString()
		{
			return FullName;
		}
	}
	
	public class NullNamespace : AbstractNamespace
	{
		public static readonly INamespace Default = new NullNamespace();
		public static readonly IEntity[] EmptyEntityArray = new IEntity[0];
		
		private NullNamespace()
		{
		}

		public override bool Resolve(ICollection<IEntity> resultingSet, string name, EntityType flags)
		{
			return false;
		}
		
		public override IEnumerable<IEntity> GetMembers()
		{
			return NullNamespace.EmptyEntityArray;
		}
	}
}
