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

import System
import System.Windows.Forms from System.Windows.Forms

class App:
	[getter(Times)]
	_times as int
	
	private def OnClick(sender, args as EventArgs):
		print("clicked!")	
		++_times
		
	private def OnClosed(sender, args as EventArgs):
		Application.Exit()
	
	def Run():
		f = Form(Text: "My first boo winforms app",
				Closed: OnClosed)
		
		f.Controls.Add(
				Button(Text: "click me!",
						Click: OnClick))
		f.Show()
		
		Application.Run(f)

app = App()
app.Run()
print("The button was clicked ${app.Times} times.")

