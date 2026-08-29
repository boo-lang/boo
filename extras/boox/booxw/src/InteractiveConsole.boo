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

"""
Interactive Forms-based Console
"""

namespace BooExplorer

import System
import System.Windows.Forms
import System.Drawing
import WeifenLuo.WinFormsUI

class InteractiveConsole(DockContent):
	
	def constructor(mainForm as MainForm):
		self.Text = "Interactive Console"
		self.DockPadding.Top = 2
		self.Controls.Add(console=booish.gui.InteractiveInterpreterControl(Font: mainForm.Settings.TextFont))					
		console.Interpreter.SetValue("MainForm", mainForm)				
	
	override def GetPersistString():
		return "InteractiveConsole|"
