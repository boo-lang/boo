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

using System.Linq;
using Microsoft.Cci;

namespace Boo.Lang.Compiler.TypeSystem.Cci
{
	public class ExternalGenericParameter : ExternalType, IGenericParameter
	{
	    readonly IMethod _declaringMethod;

        public ExternalGenericParameter(ICciTypeSystemProvider provider, Microsoft.Cci.IGenericParameter type)
            : base(provider, type)
        {
            var methodParam = type as IGenericMethodParameter;
            if (methodParam != null)
			{
                _declaringMethod = provider.Map(methodParam.DefiningMethod);
			}
		}
		
		public int GenericParameterPosition
		{
			get { return ((Microsoft.Cci.IGenericParameter)ActualType).Index; }
		}
		
		public override string FullName 
		{
			get { return string.Format("{0}.{1}", DeclaringEntity.FullName, Name); }
		}
		
		public override IEntity DeclaringEntity
		{
			get 
			{
				//NB: do not use ?? op to workaround csc bug generating invalid IL
				return (null != _declaringMethod) ? (IEntity) _declaringMethod : (IEntity) DeclaringType;
			}
		}

		public Variance Variance
		{
			get
			{
			    var variance = ((Microsoft.Cci.IGenericParameter) ActualType).Variance;
				switch (variance)
				{
					case TypeParameterVariance.NonVariant:
						return Variance.Invariant;

                    case TypeParameterVariance.Covariant:
						return Variance.Covariant;

                    case TypeParameterVariance.Contravariant:
						return Variance.Contravariant;

					default:
						return Variance.Invariant;
				}
			}
		}

		public IType[] GetTypeConstraints()
		{
		    return ((Microsoft.Cci.IGenericParameter) ActualType).Constraints
		        .Select(c => _provider.Map(c.ResolvedType)).ToArray();
		}

		public bool MustHaveDefaultConstructor
		{
			get
			{
			    var at = ((Microsoft.Cci.IGenericParameter) ActualType);
			    return at.MustHaveDefaultConstructor;
			}
		}

		public override bool IsClass
		{
			get
			{
                var at = ((Microsoft.Cci.IGenericParameter)ActualType);
                return at.MustBeReferenceType;
			}
		}

		public override bool IsValueType
		{
			get
			{
                var at = ((Microsoft.Cci.IGenericParameter)ActualType);
                return at.MustBeValueType;
			}
		}

		public override string ToString()
		{
			return Name;
		}
	}

}
