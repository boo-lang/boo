#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;

namespace Boo
{
	/// <summary>
	/// Resource manager.
	/// </summary>
	public sealed class ResourceManager
	{
		static System.Resources.ResourceManager _rm = new System.Resources.ResourceManager("strings", typeof(ResourceManager).Assembly);

		private ResourceManager()
		{
		}

		public static string GetString(string name)
		{
			return _rm.GetString(name);
		}

		public static string Format(string name, params object[] args)
		{
			return string.Format(GetString(name), args);
		}
		
		public static string Format(string name, object param)
		{
			return string.Format(GetString(name), param);
		}
	}
}
