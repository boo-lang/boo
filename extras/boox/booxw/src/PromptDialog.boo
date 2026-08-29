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

import System.Windows.Forms
import System.Drawing
import System

class PromptDialog(Form):
	
	_value as TextBox
	_message as Label
	
	def constructor():		
		
		_message = Label(Location: Point(2, 2),
						Size: System.Drawing.Size(200, 18))
		_value = TextBox(
						Location: Point(2, 20),
						Size: System.Drawing.Size(290, 18))
						
		ok = Button(Text: "OK",
					Location: Point(50, 45),
					DialogResult: DialogResult.OK)
					
		cancel = Button(Text: "Cancel",
					Location: Point(150, 45),
					DialogResult: DialogResult.Cancel)
		
		SuspendLayout()
		self.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
		self.StartPosition = FormStartPosition.CenterParent
		self.Size = System.Drawing.Size(300, 100)
		self.AcceptButton = ok
		self.CancelButton = cancel
		Controls.Add(_message)
		Controls.Add(_value)		
		Controls.Add(ok)
		Controls.Add(cancel)
		ResumeLayout(false)
		
	Message as string:
		set:
			_message.Text = value
			
	Value:
		get:
			return _value.Text
		set:
			_value.Text = value

