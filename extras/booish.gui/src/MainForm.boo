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

namespace booish.gui

import System
import System.Windows.Forms
import ICSharpCode.TextEditor
import ICSharpCode.TextEditor.Document

partial class MainForm(Form):
	def constructor():
		self.Text = "booish"
		self.Size = System.Drawing.Size(640, 480)
		self.Controls.Add(InteractiveInterpreterControl(
							Dock: DockStyle.Fill,
							Font: System.Drawing.Font("Lucida Console", 11)))

[STAThread]
def Main(argv as (string)):
	InteractiveInterpreterControl.InstallDefaultSyntaxModeProvider()
	Application.Run(MainForm())

