#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// mailto:rbo@acm.org
#endregion

namespace Boo.Examples.Web

import System
import System.IO
import System.Web
import System.Web.UI
import System.Web.UI.WebControls
import System.Web.UI.HtmlControls
import Boo.AntlrParser
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Ast.Visitors

class PrettyPrinter(BooPrinterVisitor):
	
	Server = HttpContext.Current.Server
	
	def constructor(writer as TextWriter):
		super(writer)
		
	def Write(text as string):
		Server.HtmlEncode(text, _writer)
		
	def WriteLine():		
		_writer.Write("<br />")
		super()
		
	def WriteKeyword(text as string):
		_writer.Write("<span class='keyword'>${text}</span>")
		
	def WriteOperator(text as string):
		_writer.Write("<span class='operator'>${Server.HtmlEncode(text)}</span>")
		
	override def OnStringFormattingExpression(node as StringFormattingExpression):
		_writer.Write("<span class='string'>")
		super(node)	
		_writer.Write("</span>")
		
	def WriteStringLiteral(text as string):
		_writer.Write("<span class='string'>")		
		buffer = StringWriter()		
		BooPrinterVisitor.WriteStringLiteral(text, buffer)
		Server.HtmlEncode(buffer.ToString(), _writer)
		_writer.Write("</span>")
		
	def OnIntegerLiteralExpression(node as IntegerLiteralExpression):
		_writer.Write("<span class='integer'>${node.Value}</span>")
		

class PrettyPrinterPage(Page):
	
	_src as TextBox
	_pretty as HtmlContainerControl
	
	def Page_Load(sender, args as EventArgs):
		PrettyPrint() if Page.IsPostBack
		
	def PrettyPrint():		
		printer = PrettyPrinter(StringWriter(), IndentText: "&nbsp;&nbsp;")
		printer.Print(Parse())
		_pretty.InnerHtml = printer.Writer.ToString()
		
	def Parse():
		return BooParser.ParseString("<string>", _src.Text)

			
		
