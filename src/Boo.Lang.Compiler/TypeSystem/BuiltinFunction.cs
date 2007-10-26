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
	public class BuiltinFunction : IEntity
	{
		public static BuiltinFunction Quack = new BuiltinFunction("quack", BuiltinFunctionType.Quack);
		
		public static BuiltinFunction Len = new BuiltinFunction("len", BuiltinFunctionType.Len);
		
		public static BuiltinFunction AddressOf = new BuiltinFunction("__addressof__", BuiltinFunctionType.AddressOf);
		
		public static BuiltinFunction Eval = new BuiltinFunction("__eval__", BuiltinFunctionType.Eval);
		
		public static BuiltinFunction Switch = new BuiltinFunction("__switch__", BuiltinFunctionType.Switch);

		public static BuiltinFunction InitValueType = new BuiltinFunction("__initobj__", BuiltinFunctionType.InitValueType);
		
		BuiltinFunctionType _type;
		
		string _name;
		
		public BuiltinFunction(string name, BuiltinFunctionType type)
		{
			_name = name;
			_type = type;
		}
		
		public string Name
		{
			get { return _name; }
		}
		
		public string FullName
		{
			get { return Name; }
		}
		
		public EntityType EntityType
		{
			get { return EntityType.BuiltinFunction; }
		}
		
		public BuiltinFunctionType FunctionType
		{
			get { return _type; }
		}

		public override string ToString()
		{
			return string.Format("BuiltinFunction(\"{0}\", {1})", _name, _type);
		}
	}
}
