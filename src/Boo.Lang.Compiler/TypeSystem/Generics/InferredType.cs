#region license
// Copyright (c) 2004, 2005, 2006, 2007 Rodrigo B. de Oliveira (rbo@acm.org)
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

namespace Boo.Lang.Compiler.TypeSystem.Generics
{
	public class InferredType
	{
		private IType _resultingType = null;

		private List<IType> _lowerBounds = new List<IType>();
		private List<IType> _upperBounds = new List<IType>();

		private IDictionary<InferredType, bool> _dependencies = new Dictionary<InferredType, bool>();
		private IDictionary<InferredType, bool> _dependants = new Dictionary<InferredType, bool>();

		/// <summary>
		/// Gets the type resulting from the inference, or null if none exists.
		/// </summary>
		public IType ResultingType
		{
			get { return _resultingType; }
		}

		public bool Fixed
		{
			get { return (_resultingType != null); }
		}

		public bool HasBounds
		{
			get { return _lowerBounds.Count != 0 || _upperBounds.Count != 0; }
		}

		public bool HasDependencies
		{
			get { return _dependencies.Count != 0; }
		}

		public bool HasDependants
		{
			get { return _dependants.Count != 0; }
		}

		public void ApplyLowerBound(IType type)
		{
			_lowerBounds.Add(type);
		}

		public void ApplyUpperBound(IType type)
		{
			_upperBounds.Add(type);
		}

		public void SetDependencyOn(InferredType dependee)
		{
			_dependencies[dependee] = true;
			dependee._dependants[this] = true;
		}

		public void RemoveDependencyOn(InferredType dependee)
		{
			_dependencies.Remove(dependee);
			dependee._dependants.Remove(this);
		}

		private void ShortenDependencies()
		{
			foreach (InferredType dependant in new List<InferredType>(_dependants.Keys))
			{
				foreach (InferredType dependency in _dependencies.Keys)
				{
					dependant.SetDependencyOn(dependency);
				}
				dependant.RemoveDependencyOn(this);
			}
		}

		/// <summary>
		/// Attempts to infer the type based upon its bounds.
		/// </summary>
		/// <returns>True if the type could be inferred; otherwise, false.</returns>
		public bool Fix()
		{
			if (!HasBounds) return false;

			// Inferred type must be one of the bounds,
			// assignable from every lower bound and assignable to every upper bound

			IType lowerBound = FindSink(
				_lowerBounds, 
				delegate(IType t1, IType t2) { return t1.IsAssignableFrom(t2); });

			IType upperBound = FindSink(
				_upperBounds, 
				delegate(IType t1, IType t2) { return t2.IsAssignableFrom(t1); });

			if (lowerBound == null) return Fix(upperBound);
			if (upperBound == null) return Fix(lowerBound);
			if (upperBound.IsAssignableFrom(lowerBound)) return Fix(lowerBound);
			return false;
		}

		private bool Fix(IType type)
		{
			_resultingType = type;
			if (type == null) return false;

			ShortenDependencies();			
			return true;
		}

		/// <summary>
		/// Gets a type that acts as a sink in a set of types, in regard to
		/// a specified relation.
		/// </summary>
		/// <param name="types">The set of types to find a sink for.</param>
		/// <param name="relation">The directed relation for the sink.</param>
		/// <returns>A type that maintains the specified relation with every other type
		/// in the specified set, or null if none exists.</returns>
		private IType FindSink(IEnumerable<IType> types, Relation<IType> relation)
		{
			IType candidate = null;
			foreach (IType type in types)
			{
				if (candidate == null || relation(type, candidate))
				{
					candidate = type;
				}			
			}

			if (candidate == null) return null;

			foreach (IType type in types)
			{
				if (!relation(candidate, type)) return null;
			}
			return candidate;
		}

		private delegate bool Relation<T>(T item1, T item2);
	}
}