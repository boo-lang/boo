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
import System.Diagnostics
import System.Collections
import System.Reflection
import System.Resources
import System.Windows.Forms
import System.Xml
import System.CodeDom.Compiler
import System.Threading

import ICSharpCode.SharpDevelop.Internal.Project;
import ICSharpCode.SharpDevelop.Internal.Templates;
import ICSharpCode.SharpDevelop.Gui;

class BooLanguageBinding(ILanguageBinding):
	public static LanguageName = "Boo"
	
	Language as string:
		get:
			return LanguageName
	
	_compilerManager = BooBindingCompilerManager();
	_executionManager = BooBindingExecutionManager();
	
	def Execute(filename as string, debug as bool):
		_executionManager.Execute(filename, debug)
	
	def Execute(project as IProject, debug as bool):
		_executionManager.Execute(project, debug)
	
	def GetCompiledOutputName(fileName as string) as string:
		return _compilerManager.GetCompiledOutputName(fileName)
	
	def GetCompiledOutputName(project as IProject) as string:
		return _compilerManager.GetCompiledOutputName(project);
	
	def CanCompile(fileName as string) as bool:
		return _compilerManager.CanCompile(fileName);
	
	def CompileFile(fileName as string) as ICompilerResult:
		param = BooCompilerParameters();
		param.OutputAssembly = Path.ChangeExtension(fileName, ".exe");
		return _compilerManager.CompileFile(fileName, param)
	
	def CompileProject(project as IProject) as ICompilerResult:
		return _compilerManager.CompileProject(project)
	
	def RecompileProject(project as IProject) as ICompilerResult:
		return CompileProject(project)
	
	def CreateProject(info as ProjectCreateInformation, projectOptions as XmlElement) as IProject:
		return BooProject(info, projectOptions)
