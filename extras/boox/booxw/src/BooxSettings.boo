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

import System.Drawing
import System.IO
import System.ComponentModel
import System.Xml.Serialization

class BooxSettings:
	
	_textFont = System.Drawing.Font("Lucida Console", 11)
	
	[property(UseAntiAliasFont)]
	_useAntiAliasFont = true
	
	[property(ShowLineNumbers)]
	_showLineNumbers = true
	
	[property(ShowEOLMarkers)]
	_showEOLMarkers = true
	
	[property(ShowSpaces)]
	_showSpaces = true
	
	[property(ShowTabs)]
	_showTabs = true
	
	[property(EnableFolding)]
	_enableFolding = true
	
	[property(IndentStyle)]
	_indentStyle = ICSharpCode.TextEditor.Document.IndentStyle.Smart
	
	[property(LoadPlugins)]
	_loadPlugins = true
	
	[property(Ducky)]
	_ducky = false
	
	[XmlIgnore]
	TextFont:
		get:
			return _textFont
		set:
			assert value is not null
			_textFont = value
			
	[Browsable(false)]
	TextFontName:
		get:
			return FontConverter().ConvertToInvariantString(_textFont)
		set:
			_textFont = FontConverter().ConvertFromInvariantString(value)
			
	def Save(writer as TextWriter):
		XmlSerializer(BooxSettings).Serialize(writer, self)
		
	def Save(fname as string):
		using writer=StreamWriter(fname):
			Save(writer)
		
	static def Load(fname as string):
		using reader=File.OpenText(fname):
			return Load(reader)
		
	static def Load(reader as TextReader) as BooxSettings:
		return XmlSerializer(BooxSettings).Deserialize(reader)

