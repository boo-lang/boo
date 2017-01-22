#!/usr/local/bin/booi

#region license
// Copyright (c) 2014, Harald Meyer auf'm Hofe (harald_meyer@users.sourceforge.net)
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
import System.IO
import System.Collections
import System.Diagnostics

import Boo.Lang.Useful.PlatformInformation

libs=(of string:
	"booc.exe",
	"booc.rsp",
	"booc.xml",
	"booc.exe.config",
	"booish.exe",
	"booish.rsp",
	"booish.xml",
	"booish.exe.config",
	"booi.exe",
	"booi.xml",
	"booi.exe.config",
	"booish.mod.os.dll",
	"booish.mod.os.xml",
	"Boo.Lang.CodeDom.dll",
	"Boo.Lang.CodeDom.xml",
	"Boo.Lang.Compiler.dll",
	"Boo.Lang.Compiler.xml",
	"Boo.Lang.dll",
	"Boo.Lang.xml",
	"Boo.Lang.Parser.dll",
	"Boo.Lang.Parser.xml",
	"Boo.Lang.PatternMatching.dll",
	"Boo.Lang.PatternMatching.xml",
	"Boo.Lang.Useful.dll",
	"Boo.Lang.Useful.xml",
	"Boo.Lang.Extensions.dll",
	"Boo.Lang.Extensions.xml",
	"Boo.Lang.Interpreter.dll",
	"Boo.Lang.Interpreter.xml",
	"Boo.Microsoft.Build.targets",
	"Boo.Microsoft.Build.Tasks.dll",
	"Boo.Microsoft.Build.Tasks.xml",
	"Boo.NAnt.Tasks.dll"
	)

callPattern=("#!/bin/sh\n"
	+"if [ -x /usr/local/bin/cli ]; then\n"
	+'    env /usr/local/bin/cli $MONO_OPTIONS [PRG_FULL_PATH] "$@"\n'
	+"else\n"
	+'    env mono $MONO_OPTIONS [PRG_FULL_PATH] "$@"\n'
	+"fi\n")


if not IsLinux:
	print "This is not Linux. You will definitely only want to run this on Linux."
	print "I will quit now without doing anything to your system."
	Environment.Exit(1)

#region Look at folder of this program and current working directory for the libs
exeDir = Path.GetDirectoryName(Uri(typeof(Boo.Lang.List).Assembly.CodeBase).LocalPath)
libsSources=Generic.List of string()
for lib in libs:
	fname=Path.Combine(exeDir, lib)
	if File.Exists(fname):
		libsSources.Add(fname)
	elif File.Exists(Path.GetFullPath(fname)):
		libsSources.Add(fname)
	else:
		print "Cannot find",lib+". I will quit now without doing anything to your system."
		print "Searched",exeDir
		print "     and",Path.GetFullPath(".")
		Environment.Exit(10)

installBinPath=null
installLibPath=null
print "Install to /usr/local? You must be administrator to do this. (Y/N)? "
c=Console.ReadKey(true)
if c.Key == ConsoleKey.Y:
	installBinPath = "/usr/local/bin/"
	installLibPath = "/usr/local/bin/"
else:
	print "Shall I install Boo into ~${Environment.UserName}/bin? (Y/N)? "
	c=Console.ReadKey(true)
	if c.Key == ConsoleKey.Y:
		installBinPath = "/home/"+Environment.UserName+"/bin/"
		installLibPath = "/home/"+Environment.UserName+"/boo-"+BooVersion.ToString()+"/"
if string.IsNullOrEmpty(installBinPath) or string.IsNullOrEmpty(installLibPath):
	print "Ok, abort mission without doing anything."
	Environment.Exit(2)

print "Installing startup scripts to "+installBinPath
print "            and assemblies to "+installLibPath+"."

if not Directory.Exists(installBinPath): Directory.CreateDirectory(installBinPath)
if not Directory.Exists(installLibPath): Directory.CreateDirectory(installLibPath)

for libsSource in libsSources:
	libsSourceBase=Path.GetFileName(libsSource)
	libsDest=Path.Combine(installLibPath, libsSourceBase)
	File.Copy(libsSource, libsDest)
	print "Created",libsDest
	if libsSource.EndsWith(".exe"):
		binDest=Path.Combine(installBinPath, libsSourceBase[:-4])
		using f = File.CreateText(binDest):
			f.Write(callPattern.Replace("[PRG_FULL_PATH]", libsDest))
		p=ProcessStartInfo("chmod")
		p.Arguments="a+x "+binDest
		Process.Start(p)
		print "Created",binDest

[assembly: AssemblyTitle("Small installation program for Linux")]
[assembly: AssemblyDescription("")]

