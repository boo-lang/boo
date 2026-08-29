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

import WeifenLuo.WinFormsUI
import System
import System.Windows.Forms
import System.Drawing

class OutputPane(DockContent):
	
	_richBox as RichTextBox
	
	def constructor():
		_richBox = RichTextBox(Dock: DockStyle.Fill,
								Multiline: true,
								ReadOnly: true,
								Font: System.Drawing.Font("Lucida Console", 10))
		SuspendLayout()
		
		Controls.Add(_richBox)
		self.HideOnClose = true
		self.DockableAreas = (
					DockAreas.Float |
					DockAreas.DockBottom |
					DockAreas.DockTop |
					DockAreas.DockLeft |
					DockAreas.DockRight)
		self.ClientSize = System.Drawing.Size(295, 347)
		self.ShowHint = DockState.DockBottom
		self.Text = "Output"

		ResumeLayout(false)

	def SetBuildText(text as string):
		_richBox.Text = text
		
	override protected def GetPersistString():
		return "OutputPane|"

