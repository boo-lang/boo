#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

import System
import Gtk from "gtk-sharp"
import GtkSourceView from "gtksourceview-sharp"
import Pango from "pango-sharp"

Application.Init()
	
booSourceLanguage = SourceLanguagesManager().GetLanguageFromMimeType("text/x-boo")
buffer = SourceBuffer(booSourceLanguage, Highlight: true)	
sourceView = SourceView(buffer,
						ShowLineNumbers: true,
						AutoIndent: true,
						TabsWidth: 4)
sourceView.ModifyFont(FontDescription(Family: "Lucida Console"))
				
accelGroup = AccelGroup()
menuBar = MenuBar()
fileMenu = Menu()
fileMenuOpen = ImageMenuItem(Stock.Open, accelGroup)
fileMenuOpen.Activated += do:
	fs = FileSelection("Open file", SelectMultiple: false)
	fs.Complete("*.boo")
	try:			
		if cast(int, ResponseType.Ok) == fs.Run():
			selected, =  fs.Selections
		using reader = System.IO.File.OpenText(selected):
			buffer.Text = reader.ReadToEnd() 
	ensure:
		fs.Hide()
		
fileMenu.Append(fileMenuOpen)
menuBar.Append(MenuItem("_File", Submenu: fileMenu))

vbox = VBox(false, 2)
vbox.PackStart(menuBar, false, false, 0)
scrolledSourceView = ScrolledWindow()
scrolledSourceView.Add(sourceView)
vbox.PackStart(scrolledSourceView, true, true, 0)
		
window = Window("Simple Boo Editor",
				DefaultWidth:  600,
				DefaultHeight: 400,
				DeleteEvent: Application.Quit)
window.AddAccelGroup(accelGroup)
window.Add(vbox)
window.ShowAll()

Application.Run()
