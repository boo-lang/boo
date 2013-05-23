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

namespace booi

from System import AppDomain, Console, Environment, STAThreadAttribute
from System.Diagnostics import Trace, TraceLevel, TextWriterTraceListener
from System.IO import TextReader, Directory, Path
from System.Reflection import Assembly
from Boo.Lang.Compiler import BooCompiler
from Boo.Lang.Compiler.IO import StringInput, FileInput
from Boo.Lang.Compiler.Pipelines import CompileToMemory, CompileToFile
from Boo.Lang.Useful.CommandLine import CommandLineException


class Program:
    
    public static final Version = 'Booi ' + BooVersion
    public static final DefaultErrorCode = 127
    public static final DefaultSuccessCode = 0
    
    _args as (string)
    _cmdline as CommandLine

    _assemblyResolver = AssemblyResolver()
    _compiler = BooCompiler()
    
    def constructor(cmdline, args as (string)):
        _cmdline = cmdline
        _args = args
        # Register a custom resolver to provide compiler assemblies at runtime
        AppDomain.CurrentDomain.AssemblyResolve += _assemblyResolver.AssemblyResolve

    def run() as int:
        params = _compiler.Parameters
        params.Debug = _cmdline.Debug
        params.Ducky = _cmdline.Ducky
        params.Strict = _cmdline.Strict
        params.WhiteSpaceAgnostic = _cmdline.Wsa

        if params.Debug:
            # Save to disk to ensure we get symbols loaded on runtime errors
            params.Pipeline = CompileToFile()
            params.OutputAssembly = Path.GetTempFileName() + '.exe'
        else:
            params.Pipeline = CompileToMemory()

        if _cmdline.Verbose:
            params.TraceLevel = TraceLevel.Verbose
            Trace.Listeners.Add(TextWriterTraceListener(Console.Error))

        # TODO: Set defines and lib paths

        for reference in _cmdline.References:
            try:
                asm = params.LoadAssembly(reference)
                if asm is null:
                    printerr string.Format(Boo.Lang.Resources.StringResources.BooC_UnableToLoadAssembly, reference)
                else:
                    params.References.Add(asm)
                    _assemblyResolver.AddAssembly(asm.Assembly)
            except e:
                printerr e.Message

        for file in _cmdline.Files:
            continue if file == '-'
            if Directory.Exists(file):
                for f in Directory.GetFiles(file, '*.boo'):
                    params.Input.Add(FileInput(f))
            else:
                params.Input.Add(FileInput(file))

        if not len(_cmdline.Files) or _cmdline.Files.Contains('-'):
            params.Input.Add(StringInput('<stdin>', consume(Console.In)))

        generated = compile()
        return DefaultErrorCode if generated is null
        return execute(generated, _args)
        
    def compile():
        result = _compiler.Run()

        if _cmdline.Warnings and len(result.Warnings):
            printerr(result.Warnings.ToString())

        if len(result.Errors):
            printerr(result.Errors.ToString(_cmdline.Verbose))
            return null

        if not result.GeneratedAssembly.EntryPoint:
            printerr 'Error: Entry point not found'
            return null

        if result.Parameters.Debug:
            return Assembly.LoadFrom(result.GeneratedAssemblyFileName)

        return result.GeneratedAssembly
        
    def execute(asm as Assembly, argv as (string)):
        
        exitCode = DefaultSuccessCode
        try: 
            _assemblyResolver.AddAssembly(asm)

            main = asm.EntryPoint
            if len(main.GetParameters()) > 0:
                returnValue = main.Invoke(null, (argv,))
            else:
                returnValue = main.Invoke(null, null)

            exitCode = returnValue if returnValue is not null
        except x as TargetInvocationException:
            printerr(x.InnerException)
            exitCode = DefaultErrorCode
        ensure:
            Environment.ExitCode = exitCode
            
        return exitCode
        

def consume(reader as TextReader):
    return join(line for line in reader, "\n")

def printerr(*args):
    for arg in args:
        Console.Error.Write(arg)
    Console.Error.WriteLine()


[STAThread]
def Main(argv as (string)) as int:
    # Find the number of arguments that apply to booi
    argc = System.Array.IndexOf(argv, '--')
    if argc >= 0:
        argv = argv[:argc] + argv[argc+1:]
    else:
        argc = 0
        for arg in argv:
            argc++
            break unless arg.StartsWith('-')

    try:
        cmdline = CommandLine(argv[:argc])
    except ex as CommandLineException:
        printerr 'Error:', ex.Message
        return Program.DefaultErrorCode

    if cmdline.DoVersion:
        print Program.Version
        return Program.DefaultSuccessCode

    if not cmdline.IsValid or cmdline.DoHelp:
        print "Usage: booi [options] <script|-> [-- [script options]]"
        print "Options:" 
        cmdline.PrintOptions(Console.Out)
        return Program.DefaultErrorCode

    return Program(cmdline, argv[argc:]).run()

