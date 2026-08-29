#region license
// Copyright (c) 2003, 2004, Rodrigo B. de Oliveira (rbo@acm.org)
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


import Mono.GetOptions from Mono.GetOptions

class CommandLineOptions(Options):

	def constructor(argv):
		ProcessArgs(argv)
	
	[Option("thumbnail width", "width")]
	public Width = 70
	
	[Option("thumbnail height", "height")]
	public Height = 70
	
	[Option("output file", "output")]
	public OutputFileName = ""
	
	[Option("input file", "input")]
	public InputFileName = ""
	
	[Option("encoding quality level (1-100), default is 75", "encoding-quality")]
	public EncodingQuality = 75L
	
	IsValid as bool:
		get:
			return (0 == len(RemainingArguments) and
					len(OutputFileName) > 0 and
					len(InputFileName) > 0 and
					Width > 0 and
					Height > 0)
					
options = CommandLineOptions(argv)
if options.IsValid:	
	for field in typeof(CommandLineOptions).GetFields():
		print("${field.Name}: ${field.GetValue(options)}")
else:
	options.DoHelp()
		


