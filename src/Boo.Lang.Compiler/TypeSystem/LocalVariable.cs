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
	public class LocalVariable : ITypedEntity
	{		
		Boo.Lang.Compiler.Ast.Local _local;
		
		IType _type;
		
		System.Reflection.Emit.LocalBuilder _builder;
		
		public LocalVariable(Boo.Lang.Compiler.Ast.Local local, IType type)
		{			
			_local = local;
			_type = type;
		}
		
		public string Name
		{
			get
			{
				return _local.Name;
			}
		}
		
		public string FullName
		{
			get
			{
				return _local.Name;
			}
		}
		
		public EntityType EntityType
		{
			get
			{
				return EntityType.Local;
			}
		}
		
		public bool IsPrivateScope
		{
			get
			{
				return _local.PrivateScope;
			}
		}
		
		public Boo.Lang.Compiler.Ast.Local Local
		{
			get
			{
				return _local;
			}
		}
		
		public IType Type
		{
			get
			{
				return _type;
			}
		}
		
		public System.Reflection.Emit.LocalBuilder LocalBuilder
		{
			get
			{
				return _builder;
			}
			
			set
			{
				_builder = value;
			}
		}
		
		override public string ToString()
		{
			return string.Format("Local<Name={0}, Type={1}>", Name, Type);
		}
	}
}
