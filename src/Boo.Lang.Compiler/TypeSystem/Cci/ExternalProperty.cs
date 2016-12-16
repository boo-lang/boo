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

using System.Linq;
using Microsoft.Cci;

namespace Boo.Lang.Compiler.TypeSystem.Cci
{
	public class ExternalProperty : ExternalEntity<IPropertyDefinition>, IProperty
	{
		private IParameter[] _parameters;

        private IMethodDefinition _accessor;

	    private CachedMethod _getter;

	    private CachedMethod _setter;

        public ExternalProperty(ICciTypeSystemProvider typeSystemServices, IPropertyDefinition property)
			: base(typeSystemServices, property)
		{
		}

		public virtual IType DeclaringType
		{
			get
			{
				return _provider.Map(_memberInfo.ContainingTypeDefinition);
			}
		}
		
		public bool IsStatic
		{
			get
			{
				return GetAccessor().IsStatic;
			}
		}
		
		public bool IsPublic
		{
			get
			{
			    return GetAccessor().Visibility == TypeMemberVisibility.Public;
			}
		}
		
		public bool IsProtected
		{
			get
			{
			    var accessor = GetAccessor();
                return accessor.Visibility == TypeMemberVisibility.Family 
                    || accessor.Visibility == TypeMemberVisibility.FamilyOrAssembly;
			}
		}
		
		public bool IsInternal
		{
			get
			{
			    return GetAccessor().Visibility == TypeMemberVisibility.Assembly;
			}
		}
		
		public bool IsPrivate
		{
			get
			{
			    return GetAccessor().Visibility == TypeMemberVisibility.Private;
			}
		}

        public override EntityType EntityType
		{
			get
			{
				return EntityType.Property;
			}
		}
		
		public virtual IType Type
		{
			get
			{
				return _provider.Map(_memberInfo.Type.ResolvedType);
			}
		}
		
		public IPropertyDefinition PropertyInfo
		{
			get
			{
				return _memberInfo;
			}
		}

		public bool AcceptVarArgs
		{
			get
			{
				return false;
			}
		}

		protected override ITypeDefinition MemberType
		{
			get { return _memberInfo.Type.ResolvedType; }
		}
		
		public virtual IParameter[] GetParameters()
		{
            if (null != _parameters) return _parameters;

            return _parameters = _provider.Map(_memberInfo.Parameters.ToArray());
		}
		
		public virtual IMethod GetGetMethod()
		{
            if (null != _getter) return _getter.Value;
		    return (_getter = new CachedMethod(FindGetMethod())).Value;
		}

	    private IMethod FindGetMethod()
	    {
	        IMethodDefinition getter = _memberInfo.Getter.ResolvedMethod;
            if (null == getter)
            {
                var baseProperty = FindBaseProperty();
                if (null == baseProperty) return null;

                getter = baseProperty.Getter.ResolvedMethod;
                if (null == getter) return null;
            }
	        return _provider.Map(getter);
	    }

	    private IPropertyDefinition FindBaseProperty()
	    {
	        var baseType = _memberInfo.ContainingTypeDefinition.BaseClasses.FirstOrDefault();
	        while (baseType != null)
	        {

	            var candidates = baseType.ResolvedType.GetMembersNamed(_memberInfo.Name, false).OfType<IPropertyDefinition>();
	            var result = candidates.SingleOrDefault(p => TypeHelper.TypesAreEquivalent(p.Type, _memberInfo.Type, true)
	                            && TypeHelper.ParameterListsAreEquivalent(p.Parameters, _memberInfo.Parameters));
	            if (result != null)
	                return result;
	            baseType = baseType.ResolvedType.BaseClasses.FirstOrDefault();
	        }
	        return null;
	    }

        public virtual IMethod GetSetMethod()
		{
            if (null != _setter) return _setter.Value;
            return (_setter = new CachedMethod(FindSetMethod())).Value;
		}

	    private IMethod FindSetMethod()
	    {
	        var setter = _memberInfo.Setter;
	        if (null == setter) return null;
	        return _provider.Map(setter.ResolvedMethod);
	    }

        private IMethodDefinition GetAccessor()
		{
            if (null != _accessor) return _accessor;

            return _accessor = FindAccessor();
		}

	    private IMethodDefinition FindAccessor()
	    {
	        var getter = _memberInfo.Getter.ResolvedMethod;
	        if (null != getter) return getter;
	        return _memberInfo.Setter.ResolvedMethod;
	    }
	}
}
