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


using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.TypeSystem.Services
{
    public class DowncastPermissions
    {
        private readonly CompilerParameters _parameters = My<CompilerParameters>.Instance;
        private readonly TypeSystemServices _typeSystem = My<TypeSystemServices>.Instance;

        public virtual bool CanBeReachedByDowncast(IType expectedType, IType actualType)
        {
            if (actualType.IsFinal)
                return false;

            if (IsDuckType(actualType))
                return true;

            if (!IsDowncastAllowed())
                return false;

            if (expectedType.IsInterface || actualType.IsInterface)
                return CanBeReachedByInterfaceDowncast(expectedType, actualType);
            
            return TypeCompatibilityRules.IsAssignableFrom(actualType, expectedType);
        }

    	private bool IsDuckType(IType actualType)
    	{
    		return _typeSystem.IsDuckType(actualType);
    	}

    	protected virtual bool CanBeReachedByInterfaceDowncast(IType expectedType, IType actualType)
        {
            //FIXME: currently interface downcast implements no type safety check at all (see BOO-1211)
            return true;
        }

        protected virtual bool IsDowncastAllowed()
        {
            return !_parameters.Strict || !_parameters.DisabledWarnings.Contains(CompilerWarningFactory.Codes.ImplicitDowncast);
        }
    }
}