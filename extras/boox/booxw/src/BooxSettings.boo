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

