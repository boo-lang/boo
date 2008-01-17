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

import Boo.Lang.Useful.CommandLine

enum BooCommandLineStyle:
	None
	InteractiveInterpreter
	Interpreter
	Compiler

class BooCommandLine(AbstractCommandLine):
	
	[Option("boo.CommandLine.target", ShortForm: "t")]
	public Target as string
	
	[Option("boo.CommandLine.output", ShortForm: "o")]
	public Output as string
	
	[Option("boo.CommandLine.reference", ShortForm: "r", LongForm: "reference")]
	public References = []
	
	[Argument("boo.CommandLine.argument")]
	public SourceFiles = []
	
	[Option("boo.CommandLine.resource", LongForm: "resource")]
	public Resources = []
	
	[Option("boo.CommandLine.embedres", LongForm: "embedres")]
	public EmbeddedResources = []
	
	[Option("boo.CommandLine.pipeline", ShortForm: "p")]
	public Pipeline = "compile"
	
	[Option("boo.CommandLine.utf8")]
	public UTF8 = false
	
	[Option("boo.CommandLine.wsa", LongForm: "wsa")]
	public WhiteSpaceAgnostic = false
	
	[Option("boo.CommandLine.culture", ShortForm: "c")]
	public Culture as string
	
	[Option("boo.CommandLine.debug")]
	public Debug = true
	
	[Option("boo.CommandLine.ducky")]
	public Ducky = false
	
	[Option("boo.CommandLine.help")]
	public Help = false
	
	public Style = BooCommandLineStyle.None
	
	IsValid:
		get:
			return true
			
	override def Parse(argv as (string)):
		super(argv)
		SetStyle()
		
	private def SetStyle():
		if len(SourceFiles) == 0:
			Style = BooCommandLineStyle.InteractiveInterpreter
		elif Output or Target:
			Style = BooCommandLineStyle.Compiler
		else:
			Style = BooCommandLineStyle.Interpreter

	override protected def GetOptionDescription(option as OptionAttribute):
		return ResourceManager.GetString(option.Description)
