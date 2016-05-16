#region license
// Copyright (c) 2013 by Harald Meyer auf'm Hofe (harald_meyer@users.sourceforge.net)
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

namespace Boo.Lang.Interpreter

import System
import Boo.Lang.Useful.PlatformInformation

static class ColorScheme:
"""Collection of colors that will be used by the interactive shell."""
	
	public property DisableColors = false
	public property BackgroundColor = ConsoleColor.Black
	public property InterpreterColor = ConsoleColor.DarkGray
	public property EmphasizeColor = ConsoleColor.DarkYellow	
	public property IndentionColor = ConsoleColor.DarkGreen
	public property PromptColor = ConsoleColor.DarkGreen
	public property ExceptionColor = ConsoleColor.DarkRed
	public property WarningColor = ConsoleColor.Yellow
	public property ErrorColor = ConsoleColor.Red
	public property SuggestionsColor = ConsoleColor.DarkYellow
	public property SelectedSuggestionColor = ConsoleColor.DarkMagenta
	public property HelpTextBodyColor = ConsoleColor.DarkGray
	public property HelpCmdColor = ConsoleColor.White
	public property HelpHeadlineColor = ConsoleColor.Cyan
	public property HeadlineColor = ConsoleColor.White		
	
	def Init():
		if IsLinux:
			BackgroundColor = ConsoleColor.White
			InterpreterColor = ConsoleColor.Black
			EmphasizeColor = ConsoleColor.DarkCyan	
			IndentionColor = ConsoleColor.DarkGreen
			PromptColor = ConsoleColor.DarkGreen
			ExceptionColor = ConsoleColor.DarkRed
			WarningColor = ConsoleColor.DarkYellow
			ErrorColor = ConsoleColor.DarkRed
			SuggestionsColor = ConsoleColor.DarkCyan
			SelectedSuggestionColor = ConsoleColor.DarkMagenta
			HelpTextBodyColor = ConsoleColor.DarkGray
			HelpCmdColor = ConsoleColor.Black
			HelpHeadlineColor = ConsoleColor.DarkCyan
			HeadlineColor = ConsoleColor.Black
		else:
			BackgroundColor = ConsoleColor.Black
			InterpreterColor = ConsoleColor.DarkGray
			EmphasizeColor = ConsoleColor.DarkYellow	
			IndentionColor = ConsoleColor.DarkGreen
			PromptColor = ConsoleColor.DarkGreen
			ExceptionColor = ConsoleColor.DarkRed
			WarningColor = ConsoleColor.Yellow
			ErrorColor = ConsoleColor.Red
			SuggestionsColor = ConsoleColor.DarkYellow
			SelectedSuggestionColor = ConsoleColor.DarkMagenta
			HelpTextBodyColor = ConsoleColor.DarkGray
			HelpCmdColor = ConsoleColor.White
			HelpHeadlineColor = ConsoleColor.Cyan
			HeadlineColor = ConsoleColor.White
	
	
	def WithColor(color as ConsoleColor, block as Action):
		if DisableColors:
			block()
		else:
			old = Console.ForegroundColor
			Console.ForegroundColor = color
			try:
				block()
			ensure:
				Console.ForegroundColor = old


