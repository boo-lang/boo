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
import System.Collections
import System.ComponentModel
import System.Windows.Forms

import ICSharpCode.Core.Services
import ICSharpCode.Core.AddIns

import ICSharpCode.Core.Properties
import ICSharpCode.Core.AddIns.Codons
import System.CodeDom.Compiler

import ICSharpCode.SharpDevelop.Gui
import ICSharpCode.SharpDevelop.Internal.Project
import ICSharpCode.SharpDevelop.Gui.Dialogs
import ICSharpCode.SharpDevelop.Services
import ICSharpCode.SharpDevelop.Commands

import ICSharpCode.SharpRefactory.PrettyPrinter
import ICSharpCode.SharpRefactory.Parser

class ConvertBufferCommand(AbstractMenuCommand):
	override def Run():
		window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow
		
		if window != null and window.ViewContent isa IEditable:
			viewContent as IEditable = window.ViewContent
			p = Parser()
			p.Parse(Lexer(ICSharpCode.SharpRefactory.Parser.StringReader(viewContent.Text)))
			
			if p.Errors.count > 0:
				Console.WriteLine(p.Errors.ErrorOutput)
				messageService as IMessageService = ServiceManager.Services.GetService(typeof(IMessageService))
				messageService.ShowError("Correct source code errors first (only correct source code would convert).")
				return
			bv = BooVisitor();
			bv.Visit(p.compilationUnit, null)
			
			fileService as IFileService = ServiceManager.Services.GetService(typeof(IFileService))
			fileService.NewFile("Generated.boo", "Boo", bv.SourceText.ToString())

class CSharpConvertProjectToBoo(AbstractProjectConverter):
	protected override Extension as string:
		get:
			return '.boo'
	
	// specifying the correct targetLanguage needs at least SharpDevelop 1.0.2a
	protected override def CreateProject(outputPath as string, originalProject as IProject) as IProject:
		return CreateProject(outputPath, originalProject, BooLanguageBinding.LanguageName)
	
	protected override def ConvertFile(fileName as string) as string:
		p as Parser = Parser()
		p.Parse(Lexer(ICSharpCode.SharpRefactory.Parser.FileReader(fileName)))
		bv = BooVisitor()
		bv.Visit(p.compilationUnit, null)
		return bv.SourceText.ToString()
