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
import Boo.Lang.Compiler.TypeSystem

class CompletionWindowImageProvider(booish.gui.ICompletionWindowImageProvider):
	
	_classBrowserIconService as ClassBrowserIconsService
	
	def constructor():
		self._classBrowserIconService = ServiceManager.Services.GetService(ClassBrowserIconsService)

	ImageList as ImageList:
		get:
			return _classBrowserIconService.ImageList

	def GetImageIndex(entity as IEntity) as int:
		entityType = entity.EntityType
		if EntityType.Namespace == entityType:
			return _classBrowserIconService.NamespaceIndex
		elif EntityType.Type == entityType:
			type as IType = entity
			if type.IsEnum:
				return _classBrowserIconService.EnumIndex
			if type.IsInterface:
				return _classBrowserIconService.InterfaceIndex
			if type.IsValueType:
				return _classBrowserIconService.StructIndex
			if type isa ICallableType:
				return _classBrowserIconService.DelegateIndex
			return _classBrowserIconService.ClassIndex
			
		index as int
		if EntityType.Method == entityType:
			index = _classBrowserIconService.MethodIndex
		elif EntityType.Property == entityType:
			index = _classBrowserIconService.PropertyIndex
		elif EntityType.Event == entityType:
			index = _classBrowserIconService.EventIndex
		elif EntityType.Field == entityType:
			index = _classBrowserIconService.FieldIndex
			if (entity as IField).IsLiteral:
				index = _classBrowserIconService.LiteralIndex			
		return index


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

