#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// mailto:rbo@acm.org
#endregion

namespace Boo.Ast.Compilation.Binding
{
	public class ErrorBinding : ITypeBinding, INamespace
	{
		public static ErrorBinding Default = new ErrorBinding();
		
		private ErrorBinding()
		{			
		}
		
		public string FullName
		{
			get
			{
				return Name;
			}
		}
		
		public string Name
		{
			get
			{
				return "Error";
			}
		}
		
		public BindingType BindingType
		{
			get
			{
				return BindingType.Error;
			}
		}
		
		public ITypeBinding BoundType
		{
			get
			{
				return this;
			}
		}
		
		public bool IsValueType
		{
			get
			{
				return false;
			}
		}
		
		public bool IsSubclassOf(ITypeBinding other)
		{
			return false;
		}
		
		public bool IsAssignableFrom(ITypeBinding other)
		{
			return false;
		}
		
		public IConstructorBinding[] GetConstructors()
		{
			return new IConstructorBinding[0];
		}
		
		public IBinding Resolve(string name)
		{
			return null;
		}
	}
}
