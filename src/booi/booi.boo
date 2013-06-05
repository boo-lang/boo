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
import System.Linq.Enumerable
from System.Diagnostics import Trace, TraceLevel, TextWriterTraceListener
from System.IO import TextReader, Directory, Path
from System.Reflection import Assembly
from Boo.Lang.Compiler import CompilerContext, CompilerOutputType
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
    
    def constructor(cmdline, args as (string)):
        _cmdline = cmdline
        _args = args

        # Register a custom resolver to provide compiler assemblies at runtime
        AppDomain.CurrentDomain.AssemblyResolve += _assemblyResolver.AssemblyResolve

    protected def LoadDlls(path as string, params as CompilerParameters) as int:
        dlls = Directory.GetFiles(path, '*.dll')
        for dll in dlls:
            Trace.TraceInformation('Loading assembly {0}', dll)
            asm = params.LoadAssembly(dll)
            _assemblyResolver.AddAssembly(asm.Assembly)
        return len(dlls)

    def run() as int:

        # TODO: Instead of always creating a temp directory to copy
        #       all the references, it should be possible to inject code
        #       in the generated assembly that registers to the AppDomain
        #       assembly load event and loads them based on the configuration
        #       used when compiling. This will allow to load NuGet references
        #       on demand too and should work when using nunit.

        params = CompilerParameters()
        params.OutputType = CompilerOutputType.Auto
        params.Debug = _cmdline.Debug
        params.Ducky = _cmdline.Ducky
        params.Strict = _cmdline.Strict
        params.WhiteSpaceAgnostic = _cmdline.Wsa
        params.Cache = _cmdline.Cache

        if params.Debug or _cmdline.Runner:
            # Save to disk to ensure we get symbols loaded on runtime errors
            params.GenerateInMemory = false
            params.Pipeline = CompileToFile()
            # Find a temporary directory name for the output
            while true:
                path = Path.Combine(Path.GetTempPath(), 'booi-' + Path.GetRandomFileName())
                path = path.Replace('.', '-')
                break unless Directory.Exists(path)
            print 'OUTPUT:', path
            Directory.CreateDirectory(path)
            params.OutputAssembly = Path.Combine(path, 'booi-main.dll')
        else:
            params.GenerateInMemory = true
            params.Pipeline = CompileToMemory()

        if _cmdline.Verbose:
            params.TraceLevel = TraceLevel.Verbose
            Trace.Listeners.Add(TextWriterTraceListener(Console.Error))

        for p in _cmdline.LibPaths:
            try:
                params.LibPaths.AddUnique(Path.GetFullPath(p))  
            except ex as System.ArgumentException:
                printerr "Path '$p' not found" 

        for d in _cmdline.Defines:
            params.Defines.Add(d.Key, d.Value)

        sw = System.Diagnostics.Stopwatch()
        sw.Start()
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

        # Load packages
        for package in _cmdline.Packages:
            continue unless Directory.Exists(package)

            # Load all DLLs in the root directory
            LoadDlls(package, params)

            # Process NuGet style repositories by looking into package directories,
            # only accessing their latests versions if more than one is present.
            dirs = Directory.GetDirectories(package) \
                  .GroupBy({ x | /\.\d.*$/.Replace(x, '') }) \
                  .Select({ g | g.OrderBy({ f | f.ToLower() }).Last() }) \
                  .ToList()

            # Get executing framework version to discard newer assemblies
            maxversion = typeof(System.Type).Assembly.GetName().Version
            for dir in dirs:
                dir = Path.Combine(dir, 'lib')
                continue unless Directory.Exists(dir)

                # Load any generic assemblies in the lib folder
                LoadDlls(dir, params)

                # Assume we always target the full framework (netXX)
                fwk = Directory.GetDirectories(dir, 'net*') \
                     .OrderBy({ f | f[-2:] }) \
                     .Where({ f |
                        maxversion >= System.Version(
                            join(f[-2:].ToCharArray(), '.')
                        )
                     }) \
                     .LastOrDefault()

                LoadDlls(fwk, params) if fwk


        sw.Stop()
        print 'Loading references took {0}ms' % (sw.ElapsedMilliseconds,)

        for file in _cmdline.Files:
            continue if file == '-'
            if Directory.Exists(file):
                for f in Directory.GetFiles(file, '*.boo'):
                    params.Input.Add(FileInput(f))
            else:
                params.Input.Add(FileInput(file))

        if not len(_cmdline.Files) or _cmdline.Files.Contains('-'):
            params.Input.Add(StringInput('<stdin>', consume(Console.In)))

        # Set the MAIN define if we are compiling a single script
        if 1 == len(params.Input) and not params.Defines.ContainsKey('MAIN'):
            params.Defines.Add('MAIN', '')

        sw = System.Diagnostics.Stopwatch()
        sw.Start()
        generated = compile(params)
        return DefaultErrorCode if generated is null
        sw.Stop()
        print 'Compilation took {0}ms' % (sw.ElapsedMilliseconds,)

        if _cmdline.Runner:
            # Collect all dependencies in a temporary dir
            sw = System.Diagnostics.Stopwatch()
            sw.Start()
            for reference in params.References:
                try:
                    asmpath = (reference as Boo.Lang.Compiler.TypeSystem.Reflection.IAssemblyReference).Assembly.Location
                    asmfile = Path.GetFileName(asmpath)
                    if asmfile in ('System.dll', 'System.Core.dll', 'mscorlib.dll'):
                        continue

                    # HACK: The assembly containing the parser is imported dynamically by
                    #       the compiler, looking for it to be placed next to it.
                    #       Here we ensure that when the compiler is referenced the parser
                    #       is also copied next to it.
                    if asmfile == 'Boo.Lang.Compiler.dll':
                        System.IO.File.Copy(
                            asmpath.Replace('Boo.Lang.Compiler.dll', 'Boo.Lang.Parser.dll'),
                            Path.Combine(path, 'Boo.Lang.Parser.dll')
                        )

                    # print "copy '$asmpath' to '$(Path.Combine(path, asmfile))'" 

                    System.IO.File.Copy(
                        asmpath,
                        Path.Combine(path, asmfile)
                    )
                except ex:
                    print 'Error copying assembly', reference

            sw.Stop()
            print 'Copying assemblies took {0}ms' % (sw.ElapsedMilliseconds,)

            # Replace place holders in script arguments
            asmpath = generated.Location
            args = List of string()
            if not len(_args):
                args.Add('"' + asmpath + '"')
            else:
                for arg in _args:
                    arg = arg.Replace('%filename%', Path.GetFileName(asmpath))
                    arg = arg.Replace('%pathname%', Path.GetDirectoryName(asmpath))
                    arg = arg.Replace('%assembly%', asmpath)
                    if arg == '%':
                        arg = asmpath
                    args.Add('"' + arg + '"')

            # Try to execute the runner
            print 'Command:', _cmdline.Runner, join(args, ' ')
            sw = System.Diagnostics.Stopwatch()
            sw.Start()            
            retval = runcommand(_cmdline.Runner, join(args, ' '))
            sw.Stop()
            print 'Running command took {0}ms' % (sw.ElapsedMilliseconds,)

            # Clean up generated files
            # Directory.Delete(Path.GetDirectoryName(asmpath), true)
        else:
            retval = execute(generated, _args)

        return retval
        
    def compile(params as CompilerParameters):
        context = CompilerContext(params)
        params.Pipeline.Run(context)

        if _cmdline.Warnings and len(context.Warnings):
            printerr(context.Warnings.ToString())

        if len(context.Errors):
            printerr(context.Errors.ToString(params.TraceVerbose))
            return null

        if not context.GeneratedAssembly:
            print 'AsmFileName', context.GeneratedAssemblyFileName
            return Assembly.LoadFrom(context.GeneratedAssemblyFileName)

        return context.GeneratedAssembly
        
    def execute(asm as Assembly, argv as (string)):
        exitCode = DefaultSuccessCode
        try:
            _assemblyResolver.AddAssembly(asm)

            main = asm.EntryPoint
            if not main:
                printerr 'Error: Entry point not found'
                return DefaultErrorCode

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


def runcommand(filename, arguments):
    p = System.Diagnostics.Process()
    p.StartInfo.Arguments = arguments
    p.StartInfo.CreateNoWindow = true
    p.StartInfo.UseShellExecute = true
    p.StartInfo.FileName = filename
    p.Start()
    p.WaitForExit()
    return p.ExitCode

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

