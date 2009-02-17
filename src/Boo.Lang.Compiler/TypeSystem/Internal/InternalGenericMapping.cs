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

using Boo.Lang.Compiler.TypeSystem.Generics;

namespace Boo.Lang.Compiler.TypeSystem
{
	using System;
	using System.Text;
	using System.Collections.Generic;

	/// <summary>
	/// Maps entities onto their constructed counterparts, substituting type arguments for generic parameters.
	/// </summary>
	public class InternalGenericMapping : GenericMapping
	{
		public InternalGenericMapping(IType constructedType, IType[] arguments)
			: base(constructedType, arguments)
		{
		}

		public InternalGenericMapping(IMethod constructedMethod, IType[] arguments)
			: base(constructedMethod, arguments)
		{
		}

		protected override IMember CreateMappedMember(IMember source)
		{
			switch (source.EntityType)
			{
				case EntityType.Method:
					IMethod method = (IMethod)source;
					return new GenericMappedMethod(TypeSystemServices, method, this);

				case EntityType.Constructor:
					IConstructor ctor = (IConstructor)source;
					return new GenericMappedConstructor(TypeSystemServices, ctor, this);

				case EntityType.Field:
					IField field = (IField)source;
					return new GenericMappedField(TypeSystemServices, field, this);

				case EntityType.Property:
					IProperty property = (IProperty)source;
					return new GenericMappedProperty(TypeSystemServices, property, this);

				case EntityType.Event:
					IEvent evt = (IEvent)source;
					return new GenericMappedEvent(TypeSystemServices, evt, this);

				default:
					return source;
			}
		}

		public override IMember UnMap(IMember mapped)
		{
			IGenericMappedMember mappedAsMappedMember = mapped as IGenericMappedMember;
			if (mappedAsMappedMember != null)
			{
				return mappedAsMappedMember.SourceMember;
			}
			return null;
		}
	}
}

