#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rodrigobamboo@gmail.com)
// All rights reserved.
//
// BooBinding is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// BooBinding is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with BooBinding; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
#endregion

namespace BooBinding

import System
import System.Drawing
import System.Windows.Forms
import ICSharpCode.SharpDevelop.Gui
import ICSharpCode.Core.Services
import ICSharpCode.SharpDevelop.Services
import booish.gui

class CompletionWindowImageProvider(booish.gui.ICompletionWindowImageProvider):
	
	_classBrowserIconService as ClassBrowserIconsService
	
	def constructor():
		self._classBrowserIconService = ServiceManager.Services.GetService(ClassBrowserIconsService)

	ImageList as ImageList:
		get:
			return _classBrowserIconService.ImageList

	NamespaceIndex as int:
		get:
			return _classBrowserIconService.NamespaceIndex
	ClassIndex as int:
		get:
			return _classBrowserIconService.ClassIndex
	InterfaceIndex as int:
		get:
			return _classBrowserIconService.InterfaceIndex
	EnumIndex as int:
		get:
			return _classBrowserIconService.EnumIndex
	StructIndex as int:
		get:
			return _classBrowserIconService.StructIndex
	CallableIndex as int:
		get:
			return _classBrowserIconService.DelegateIndex
	MethodIndex as int:
		get:
			return _classBrowserIconService.MethodIndex
	FieldIndex as int:
		get:
			return _classBrowserIconService.FieldIndex
	LiteralIndex as int:
		get:
			return _classBrowserIconService.LiteralIndex
	PropertyIndex as int:
		get:
			return _classBrowserIconService.PropertyIndex
	EventIndex as int:
		get:
			return _classBrowserIconService.EventIndex

class BooishView(AbstractPadContent):
	
	_booish as InteractiveInterpreterControl
	
	def constructor():
		super("booish", "Boo.ProjectIcon")
		_booish = InteractiveInterpreterControl(Font: System.Drawing.Font("Lucida Console", 10))
		_booish.CompletionWindowImageProvider = CompletionWindowImageProvider()
		_booish.Interpreter.SetValue("Workbench", WorkbenchSingleton.Workbench)
		
	override Control as Control:
		get:
			return _booish

