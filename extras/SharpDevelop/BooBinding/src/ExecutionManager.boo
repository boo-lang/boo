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

import ICSharpCode.SharpDevelop.Internal.Project
import ICSharpCode.SharpDevelop.Gui
import ICSharpCode.SharpDevelop.Services

import ICSharpCode.Core.Services

class BooBindingExecutionManager:
	def Execute(filename as string, debug as bool):
		exe = Path.ChangeExtension(filename, ".exe")
		debuggerService as DebuggerService = ServiceManager.Services.GetService(typeof(DebuggerService))
		if debug:
			debuggerService.Start(exe, Path.GetDirectoryName(exe), "")
		else:
			psi = ProcessStartInfo(Environment.GetEnvironmentVariable("ComSpec"), "/c \"${exe}\" & pause")
			psi.WorkingDirectory = Path.GetDirectoryName(exe)
			psi.UseShellExecute = false
			debuggerService.StartWithoutDebugging(psi)
	
	def Execute(project as BooProject, debug as bool):
		parameters as BooCompilerParameters = project.ActiveConfiguration
		fileUtilityService as FileUtilityService = ServiceManager.Services.GetService(typeof(FileUtilityService))
		
		directory = fileUtilityService.GetDirectoryNameWithSeparator(parameters.OutputDirectory)
		exe = parameters.OutputAssembly + ".exe"
		args = parameters.CommandLineParameters
		
		if parameters.CompileTarget == CompileTarget.Library:
			messageService as IMessageService = ServiceManager.Services.GetService(typeof(IMessageService))
			messageService.ShowError('${res:BackendBindings.ExecutionManager.CantExecuteDLLError}')
			return
		
		debuggerService as DebuggerService = ServiceManager.Services.GetService(typeof(DebuggerService))
		
		if debug:
			debuggerService.Start(Path.Combine(directory, exe), directory, args)
			return
		
		runtimeStarter = String.Empty;
		if parameters.Runtime == NetRuntime.Mono:
			runtimeStarter = "mono "
		if parameters.Runtime == NetRuntime.MonoInterpreter:
			runtimeStarter = "mint "
		
		psi as ProcessStartInfo = null;
		if parameters.CompileTarget != CompileTarget.WinExe and parameters.PauseConsoleOutput:
			psi = ProcessStartInfo(Environment.GetEnvironmentVariable("ComSpec"), "/c ${runtimeStarter}\"${directory}${exe}\" ${args} & pause")
		else:
			psi = ProcessStartInfo(runtimeStarter + "\"" + directory + exe + "\"")
			psi.Arguments = args
		
		psi.WorkingDirectory = Path.GetDirectoryName(directory)
		psi.UseShellExecute = false
		debuggerService.StartWithoutDebugging(psi)
