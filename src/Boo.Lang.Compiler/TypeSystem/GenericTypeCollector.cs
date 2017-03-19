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


using System;
using System.Linq;
using System.Collections.Generic;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem.Internal;

namespace Boo.Lang.Compiler.TypeSystem
{
	public class GenericTypeCollector : FastDepthFirstVisitor
	{
		private class DistinctGenericComparer : IEqualityComparer<IGenericParameter>
		{
			public bool Equals(IGenericParameter l, IGenericParameter r)
			{
				return l.GenericParameterPosition == r.GenericParameterPosition;
			}
			
			public int GetHashCode(IGenericParameter param)
			{
				return param.GenericParameterPosition;
			}
		}
		
		private List<IGenericParameter> _matches;
		
		private BooCodeBuilder _codeBuilder;
		
		public GenericTypeCollector(BooCodeBuilder codeBuilder)
		{
			_matches = new List<IGenericParameter>();
			_codeBuilder = codeBuilder;
		}
		
		private void OnCandidateNode(Node candidate)
		{
			var genericParameterRef = candidate.Entity as IGenericParameter;
			if (genericParameterRef != null)
				_matches.Add(genericParameterRef);
		}
		
		override public void OnReferenceExpression(ReferenceExpression node)
		{
			OnCandidateNode(node);
		}
		
		override public void OnSimpleTypeReference(SimpleTypeReference node)
		{
			OnCandidateNode(node);
		}
		
		public void Process(ClassDefinition cd)
		{
			cd.Accept(this);
			var parameters = GenericParameters.ToArray();
			for (var i = 0; i < parameters.Length; ++i)
			{
			    var param = parameters[i];
                var gen = cd.GenericParameters.FirstOrDefault(gp => gp.Name.Equals(param.Name));
			    var found = gen != null;
				if (!found)
                    gen = _codeBuilder.CreateGenericParameterDeclaration(i, param.Name);
				foreach (IType baseType in param.GetTypeConstraints())
				{
					gen.BaseTypes.Add(_codeBuilder.CreateTypeReference(baseType));
				}
				if (param.MustHaveDefaultConstructor)
					gen.Constraints |= GenericParameterConstraints.Constructable;
				if (param.IsValueType)
					gen.Constraints |= GenericParameterConstraints.ValueType;
				else if (param.IsClass)
					gen.Constraints |= GenericParameterConstraints.ReferenceType;
				if (param.Variance == Variance.Covariant)
						gen.Constraints |= GenericParameterConstraints.Covariant;
				else if (param.Variance == Variance.Contravariant)
						gen.Constraints |= GenericParameterConstraints.Contravariant;
                if (!found)
				    cd.GenericParameters.Add(gen);
			}
		}
		
		private IEnumerable<IGenericParameter> GenericParameters
		{
			get { return _matches.Distinct(new DistinctGenericComparer()).OrderBy(p => p.GenericParameterPosition); }
		}

	}
}
