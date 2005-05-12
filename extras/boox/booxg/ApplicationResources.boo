#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
//
// This file is part of Boo Explorer.
//
// Boo Explorer is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// Boo Explorer is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Foobar; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
#endregion

namespace BooExplorer

class ApplicationResources:

	static _manager = System.Resources.ResourceManager("BooExplorer", typeof(MainWindow).Assembly)
	
	class Icons:
		public static final Class = LoadIcon("class")
		public static final Method = LoadIcon("method")
		public static final Field = LoadIcon("field")
		public static final Event = LoadIcon("event")
		public static final Property = LoadIcon("property")
		
		static def LoadIcon(name as string):
			return Gdk.Pixbuf(cast((byte), _manager.GetObject(name)))
