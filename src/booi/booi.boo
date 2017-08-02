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

from System import AppDomain, Console, Environment, Guid, STAThreadAttribute
import System.Linq.Enumerable
from System.Diagnostics import Trace, TraceLevel, TextWriterTraceListener, Stopwatch
from System.IO import TextReader, StreamWriter, Directory, File, Path
from System.Reflection import Assembly
from Boo.Lang.Compiler import CompilerContext, CompilerOutputType
from Boo.Lang.Compiler.IO import StringInput, FileInput
from Boo.Lang.Compiler.Pipelines import CompileToMemory, CompileToFile
from Boo.Lang.Compiler.TypeSystem.Reflection import IAssemblyReference
from Boo.Lang.Useful.CommandLine import CommandLineException


class Program:
    
    public static final Version = 'Booi ' + BooVersion
    public static final DefaultErrorCode = 127
    public static final DefaultSuccessCode = 0
    
    _args as (string)
    _cmdline as CommandLine

    _assemblyResolver = AssemblyResolver()
    _params = CompilerParameters()
    
    def constructor(cmdline, args as (string)):
        _cmdline = cmdline
        _args = args

        # Register a custom resolver to provide compiler assemblies at runtime
        AppDomain.CurrentDomain.AssemblyResolve += _assemblyResolver.AssemblyResolve

    protected def LoadDlls(path as string):
        dlls = Directory.GetFiles(path, '*.dll')
        for dll in dlls:
            Trace.TraceInformation('Loading assembly {0}', dll)
            asm = _params.LoadAssembly(dll)
            _params.References.Add(asm)
            _assemblyResolver.AddAssembly(asm.Assembly)

    protected def LoadPackages(path as string):
        # Load all DLLs in the root directory
        LoadDlls(path)

        # Process NuGet style repositories by looking into package directories,
        # only accessing their latests versions if more than one is present.
        dirs = Directory.GetDirectories(path) \
              .GroupBy({ x | /\.\d.*$/.Replace(x, '') }) \
              .Select({ g | g.OrderBy({ f | f.ToLower() }).Last() }) \
              .ToList()

        # Get executing framework version to discard newer assemblies
        maxversion = typeof(System.Type).Assembly.GetName().Version
        for dir in dirs:
            dir = Path.Combine(dir, 'lib')
            continue unless Directory.Exists(dir)

            # Load any generic assemblies in the lib folder
            LoadDlls(dir)

            # Assume we always target the full framework (netXX)
            path = Directory.GetDirectories(dir, 'net*') \
                 .OrderBy({ f | f[-2:] }) \
                 .Where({ f |
                    maxversion >= System.Version(
                        join(f[-2:].ToCharArray(), '.')
                    )
                 }) \
                 .LastOrDefault()

            LoadDlls(path) if path

    def Run() as int:

        # TODO: Instead of always creating a temp directory to copy
        #       all the references, it should be possible to inject code
        #       in the generated assembly that registers to the AppDomain
        #       assembly load event and loads them based on the configuration
        #       used when compiling. This will allow to load NuGet references
        #       on demand too and should work when using nunit.

        _params.OutputType = CompilerOutputType.Auto
        _params.Debug = _cmdline.Debug
        _params.Ducky = _cmdline.Ducky
        _params.Strict = _cmdline.Strict
        _params.WhiteSpaceAgnostic = _cmdline.Wsa
        _params.Cache = _cmdline.Cache

        if _cmdline.Verbose:
            _params.TraceLevel = TraceLevel.Verbose
            Trace.Listeners.Add(TextWriterTraceListener(Console.Error))

        if (_params.Debug and IsMono()) or _cmdline.Runner or _cmdline.Output:
            # Save to disk to ensure we get symbols loaded on runtime errors
            _params.GenerateInMemory = false
            _params.Pipeline = CompileToFile()

            if not _cmdline.Output:
                # Find a temporary directory name for the output
                while true:
                    path = Path.Combine(Path.GetTempPath(), 'booi-' + Path.GetRandomFileName())
                    path = path.Replace('.', '-')
                    break unless Directory.Exists(path)
                Trace.TraceInformation("Creating temporary directory '{0}'", path)
                Directory.CreateDirectory(path)
                _params.OutputAssembly = Path.Combine(path, 'booi-main.exe')
            else:
                if Directory.Exists(_cmdline.Output):
                    path = _cmdline.Output
                    _params.OutputAssembly = Path.Combine(path, 'booi-main.exe')
                else:
                    path = Path.GetDirectoryName(_cmdline.Output)
                    _params.OutputAssembly = _cmdline.Output

        else:
            _params.GenerateInMemory = true
            _params.Pipeline = CompileToMemory()

        for d in _cmdline.Defines:
            _params.Defines.Add(d.Key, d.Value)

        for libpath in _cmdline.LibPaths:
            try:
                _params.LibPaths.AddUnique(Path.GetFullPath(libpath))
            except ex as System.ArgumentException:
                Trace.TraceError("LibPath '{0}' not found", libpath)

        sw = System.Diagnostics.Stopwatch()
        sw.Start()

        for reference in _cmdline.References:
            try:
                asmref = _params.LoadAssembly(reference)
                if asmref is null:
                    printerr string.Format(Boo.Lang.Resources.StringResources.BooC_UnableToLoadAssembly, reference)
                    return DefaultErrorCode

                _params.References.Add(asmref)
                _assemblyResolver.AddAssembly(asmref.Assembly)
            except ex:
                printerr "Error loading reference '{0}': {1}", reference, ex
                return DefaultErrorCode

        # Load packages
        for package in _cmdline.Packages:
            LoadPackages(package) if Directory.Exists(package)

        sw.Stop()
        Trace.TraceInformation("Loading references took {0}ms", sw.ElapsedMilliseconds)

        for file in _cmdline.Files:
            continue if file == '-'
            if Directory.Exists(file):
                for f in Directory.GetFiles(file, '*.boo'):
                    _params.Input.Add(FileInput(f))
            else:
                _params.Input.Add(FileInput(file))

        if not len(_cmdline.Files) or _cmdline.Files.Contains('-'):
            _params.Input.Add(StringInput('<stdin>', consume(Console.In)))

        # Set the MAIN define if we are compiling a single script
        if 1 == len(_params.Input) and not _params.Defines.ContainsKey('MAIN'):
            _params.Defines.Add('MAIN', '')

        sw.Restart()
        generated = Compile()
        return DefaultErrorCode if generated is null
        sw.Stop()
        Trace.TraceInformation('Compilation took {0}ms', sw.ElapsedMilliseconds)

        if _cmdline.Runner or _cmdline.Output:
            # Collect all dependencies in a temporary dir
            for asmref in _params.References:
                asm = asmref.Assembly
                # Skip dynamically generated assemblies
                continue if asm.IsDynamic

                try:
                    asmfile = Path.GetFileName(asm.Location)
                    if asmfile in ('System.dll', 'System.Core.dll', 'mscorlib.dll'):
                        continue

                    # HACK: The assembly containing the parser is imported dynamically by
                    #       the compiler, looking for it to be placed next to it.
                    #       Here we ensure that when the compiler is referenced the parser
                    #       is also copied next to it.
                    if asmfile == 'Boo.Lang.Compiler.dll':
                        File.Copy(
                            asm.Location.Replace('Boo.Lang.Compiler.dll', 'Boo.Lang.Parser.dll'),
                            Path.Combine(path, 'Boo.Lang.Parser.dll')
                        )

                    File.Copy(asm.Location, Path.Combine(path, asmfile))
                except ex:
                    Trace.TraceError("Error copying assembly '{0}' to '{1}'", asmref.Name, path)

            DumpSolutionFile()

        if _cmdline.Runner:
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
                    arg = arg.Replace('%solution%', asmpath[:-3] + 'sln')
                    if arg == '%':
                        arg = asmpath
                    args.Add('"' + arg + '"')

            # Try to execute the runner
            Trace.TraceInformation('Running command: {0} {1}', _cmdline.Runner, join(args, ' '))
            retval = runcommand(_cmdline.Runner, join(args, ' '))
        else:
            retval = Execute(generated, _args)

        # Clean up generated files
        if path and not _cmdline.Output:
            Trace.TraceInformation("Removing temporary directory '{0}'", path)
            try:
                Directory.Delete(path, true)
            except x:
                Trace.TraceError("Failed to remove temporary directory: {0}", x)

        return retval
        
    def Compile():
        context = CompilerContext(_params)
        _params.Pipeline.Run(context)

        if _cmdline.Warnings and len(context.Warnings):
            printerr(context.Warnings.ToString())

        if len(context.Errors):
            printerr(context.Errors.ToString(_params.TraceVerbose))
            return null

        if not context.GeneratedAssembly:
            return Assembly.LoadFrom(context.GeneratedAssemblyFileName)

        return context.GeneratedAssembly
        
    def Execute(asm as Assembly, argv as (string)):
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
            printerr x.InnerException
            exitCode = DefaultErrorCode
        ensure:
            Environment.ExitCode = exitCode
            
        return exitCode

    protected def DumpSolutionFile():
    """ Create a MonoDevelop solution file to ease debugging. MonoDevelop allows defining
        managed executables as projects.
    """
        asmpath = Path.GetDirectoryName(_params.OutputAssembly)
        asmfile = Path.GetFileName(_params.OutputAssembly)
        asmname = asmfile[:-4]

        # Get assemblies automatically compiled from sources
        assemblies = [asmfile]
        for asmref as IAssemblyReference in _params.References:
            asm = asmref.Assembly
            if asm.IsDynamic:
                assemblies.Add(asmref.Name + '.dll')
                continue

            attrs = asm.GetCustomAttributes(CompilationTimeAttribute, false)
            if len(attrs):
                assemblies.Add(asmref.Name + '.dll')


        fpath = Path.Combine(asmpath, asmname + '.sln')
        using fs = StreamWriter(fpath):
            fs.WriteLine()
            fs.WriteLine('Microsoft Visual Studio Solution File, Format Version 11.00')
            fs.WriteLine('# Visual Studio 2010')
            fs.WriteLine('Global')
            fs.WriteLine('\tGlobalSection(MonoDevelopProperties) = preSolution')
            fs.WriteLine('\t\tStartupItem = {0}', asmname)
            fs.WriteLine('\tEndGlobalSection')
            fs.WriteLine('EndGlobal')

            for asmfile in assemblies:
                # Generate a deterministic Guid based on the file name
                using md5 = System.Security.Cryptography.MD5.Create():
                    bytes = System.Text.Encoding.Default.GetBytes(asmfile)
                    guid = Guid(md5.ComputeHash(bytes))

                fs.WriteLine('Project("{{8BC9CEB9-8B4A-11D0-8D11-00A0C91BC942}}") = "{0}", "{1}", "{{{2}}}"', asmfile[:-4], asmfile, guid)
                fs.WriteLine('\tProjectSection(MonoDevelopProperties) = preProject')
                fs.WriteLine('\t\tConfigurations = $0')
                fs.WriteLine('\t\t$0.Configuration = $1')
                fs.WriteLine('\t\t$1.name = Debug')
                fs.WriteLine('\t\t$1.OutputPath = ./')
                fs.WriteLine('\t\t$1.ctype = ProjectConfiguration')
                fs.WriteLine('\tEndProjectSection')
                fs.WriteLine('EndProject')

            # Include an NUnit assembly test collection project
            fs.WriteLine('Project("{9344bdbb-3e7f-41fc-a0dd-8665d75ee146}") = "nunit", "nunit.mdproj", "{4B9B3417-6A64-4D19-912A-9A7C50828CEA}"')
            fs.WriteLine('EndProject')

        # Generate an mdproj file for the NUnit assembly test collection
        fpath = Path.Combine(asmpath, 'nunit.mdproj')
        using fs = StreamWriter(fpath):
            fs.WriteLine('<?xml version="1.0" encoding="utf-8"?>')
            fs.WriteLine('<Project DefaultTargets="Build" ToolsVersion="4.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">')
            fs.WriteLine('<PropertyGroup>')
            fs.WriteLine(' <Configuration Condition=" \'$(Configuration)\' == \'\' ">Default</Configuration>')
            fs.WriteLine(' <Platform Condition=" \'$(Platform)\' == \'\' ">AnyCPU</Platform>')
            fs.WriteLine(' <ItemType>NUnitAssemblyGroupProject</ItemType>')
            fs.WriteLine(' <ProductVersion>10.0.0</ProductVersion>')
            fs.WriteLine(' <SchemaVersion>2.0</SchemaVersion>')
            fs.WriteLine(' <ProjectGuid>{{4B9B3417-6A64-4D19-912A-9A7C50828CEA}}</ProjectGuid>')
            fs.WriteLine('</PropertyGroup>')
            fs.WriteLine('<PropertyGroup Condition=" \'$(Configuration)|$(Platform)\' == \'Default|AnyCPU\' ">')
            fs.WriteLine(' <Assemblies>')
            fs.WriteLine('  <Assemblies>')
            for asmfile in assemblies:
                fs.WriteLine('   <Assembly Path="{0}" />', Path.GetFullPath(Path.Combine(asmpath, asmfile)))
            fs.WriteLine('  </Assemblies>')
            fs.WriteLine(' </Assemblies>')
            fs.WriteLine('</PropertyGroup>')
            fs.WriteLine('</Project>')

def IsMono():
    return System.Type.GetType("Mono.Runtime") is not null

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
        Console.Error.Write(' ')
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

    if cmdline.Empty or cmdline.DoHelp:
        print "Usage: booi [options] <script|-> [-- [script options]]"
        print "Options:"
        cmdline.PrintOptions(Console.Out)
        return Program.DefaultErrorCode

    return Program(cmdline, argv[argc:]).Run()

