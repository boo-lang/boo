#region license
// Copyright (c) 2004, Daniel Grunwald (daniel@danielgrunwald.de)
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
import System.IO
import System.Drawing
import System.Windows.Forms

import ICSharpCode.SharpDevelop.Internal.Project
import ICSharpCode.SharpDevelop.Internal.ExternalTool
import ICSharpCode.SharpDevelop.Gui.Dialogs
import ICSharpCode.Core.Services
import ICSharpCode.Core.Properties
import ICSharpCode.Core.AddIns.Codons

class BooOptionsPanel(AbstractOptionPanel):
	_compilerParameters as BooCompilerParameters = null
	
	override def LoadPanelContents():
		_compilerParameters = cast(IProperties, CustomizationObject).GetProperty("Config");
		
		Controls.Add(PropertyGrid(Dock : DockStyle.Fill, SelectedObject : _compilerParameters))
	
	override def StorePanelContents() as bool:
		return true;
