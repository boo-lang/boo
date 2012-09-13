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

using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.TypeSystem.Internal
{
	public class InternalProperty : InternalEntity<Property>, IProperty
	{
		private readonly InternalTypeSystemProvider _provider;
		private IParameter[] _parameters;

		public InternalProperty(InternalTypeSystemProvider provider, Property property) : base(property)
		{
			_provider = provider;
		}
		
		override public EntityType EntityType
		{
			get { return EntityType.Property; }
		}
		
		public IType Type
		{
			get { return _node.Type != null  ? TypeSystemServices.GetType(_node.Type) : Unknown.Default; }
		}

		public bool AcceptVarArgs
		{
			get { return false; }
		}
		
		public IParameter[] GetParameters()
		{
			return _parameters ?? (_parameters = _provider.Map(_node.Parameters));
		}

		public IProperty Overriden { get; set; }

		public IMethod GetGetMethod()
		{
			if (_node.Getter != null)
				return (IMethod)TypeSystemServices.GetEntity(_node.Getter);
			return Overriden != null ? Overriden.GetGetMethod() : null;
		}
		
		public IMethod GetSetMethod()
		{
			if (_node.Setter != null)
				return (IMethod)TypeSystemServices.GetEntity(_node.Setter);
			return Overriden != null ? Overriden.GetSetMethod() : null;
		}

		public Property Property
		{
			get { return _node; }
		}
		
		override public string ToString()
		{
			return string.Format("{0} as {1}", Name, Type);
		}

		public bool IsDuckTyped
		{
			get { return Type == _provider.DuckType || _node.Attributes.Contains("Boo.Lang.DuckTypedAttribute"); }
		}
	}
}
