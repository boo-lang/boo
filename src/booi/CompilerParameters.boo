namespace booi

from System import AppDomain
from System.IO import Path, Directory, StreamReader, StreamWriter
from System.Reflection import Assembly, AssemblyName
from System.Reflection.Emit import AssemblyBuilder, AssemblyBuilderAccess
from Boo.Lang.Compiler import CompilerContext, \
                              CompilerParameters as BooCompilerParameters
from Boo.Lang.Compiler.IO import FileInput
from Boo.Lang.Compiler.Pipelines import CompileToMemory, CompileToFile

/*
class CompilationCache:

    static final HEADER = 'BOO-COMP-CACHE-1'

    [getter(Sources)]
    _sources = {}

    [getter(References)]
    _references = {}

    [getter(Assembly)]
    _assembly as Assembly

    _path as string

    static def Load(fname as string):
        asm as (char)
        symbols as (char)

        cache = CompilationCache()
        using fs = StreamReader(fname):
            version = fs.ReadLine()
            if version != HEADER:
                raise 'Unsupported file'

            while fs.Peek() != -1:
                line = fs.ReadLine()
                parts = line.Split(char('\t'))
                if parts[0] == 'source':
                    ParseSource(parts)
                elif parts[1] == 'assembly':
                    size = ParseSize(parts)
                    asm = array(char, size)
                    fs.Read(asm, size)
                elif parts[0] == 'symbols':
                    size = ParseSize(parts)
                    symbols = array(char, size)
                    fs.Read(symbols, size)
                else:
                    raise 'Malformed file'

        if not symbols:
            cache.Assembly = Assembly.Load(asm)
        else:
            cache.Assembly = Assembly.Load(asm, symbols)

    def Save(fname as string):
        using fs = StreamWriter(fname):
            for src in _sources:
                fs.WriteLine("source\t$(src.Key)\t$(src.Value)")

            asm.Save()

    IsValid:
        get: return CheckValidity()

    def CheckValidity():
        context = CompilerContext.Current

        # Check if any source file was modified
        files = Directory.GetFiles(_path, '*.boo')
        if len(files) != len(Sources):
            context.Trace("Directory contents have changed")
            return false

        for source in Sources:
            if source.Key not in files:
                context.TraceInfo("File $(source.Key) was removed")
                return false
            if source.Value != File.GetLastWriteTimeUtc(source.Key):
                context.TraceInfo("File $(source.Key) was modified")
                return false

        # Check if any reference was modified
        references = [r.Location for r in context.References]
        for reference in References:
            if reference.Key not in references:
                context.TraceInfo("Reference ${reference.Key} was removed")
                return false
            if reference.Value != File.GetLastWriteTimeUtc(reference.Key):
                context.TraceInfo("Reference $(reference.Key) was modified")
                return false

        return true
*/        



class CompilerParameters(BooCompilerParameters):

    Context as CompilerContext:
        get: return CompilerContext.Current

    def constructor():
        super()

    def constructor(loadDefault as bool):
        super(loadDefault)

    override def ForName(asmname as string, throwOnError as bool) as Assembly:
        # Loading default assemblies is performed normally
        if not Context:
            return super(asmname, throwOnError)

        Context.TraceInfo('ForName ' + asmname)

        # Try to load the assembly using Boo's default mechanism
        try:
            return super(asmname, true)
        except ex as System.ApplicationException:
            pass

        for libpath in LibPaths:
            Context.TraceInfo('Looking for namespace {0} in "{1}"', asmname, libpath)
            for dirpath in NamespaceToPaths(asmname):
                path = Path.Combine(libpath, dirpath)
                if Directory.Exists(path):
                    # Collect source files
                    files = Directory.GetFiles(path, '*.boo')
                    # At least one source file must be present
                    continue if not len(files)

                    Context.TraceInfo('Mapping namespace {0} to "{1}"', asmname, path)

                    watch = System.Diagnostics.Stopwatch()
                    watch.Start()

                    # Create new params copying settings from current one
                    params = CompilerParameters(false)
                    params.Strict = self.Strict
                    params.Debug = self.Debug
                    # TODO: Shall we copy by value?
                    params.References = self.References
                    for p in self.LibPaths:
                        params.LibPaths.AddUnique(p)

                    #params.OutputAssembly = Path.GetTempFileName() + '.dll'
                    params.OutputAssembly = Path.GetDirectoryName(self.OutputAssembly)
                    params.OutputAssembly = Path.Combine(params.OutputAssembly, asmname + '.dll')
                    params.Pipeline = CompileToFile()#Memory()

                    # times = {}
                    # params.Pipeline.BeforeStep += def(elf, evt):
                    #     key = evt.Step.GetType().FullName
                    #     times[key] = System.Diagnostics.Stopwatch()
                    #     (times[key] as System.Diagnostics.Stopwatch).Start()

                    # params.Pipeline.AfterStep += def(elf, evt):
                    #     key = evt.Step.GetType().FullName
                    #     (times[key] as System.Diagnostics.Stopwatch).Stop()

                    # params.Pipeline.After += def(elf, evt):
                    #     for time in times:
                    #         sw = time.Value as System.Diagnostics.Stopwatch
                    #         if sw.ElapsedMilliseconds > 0:
                    #             print time.Key + ':' + sw.ElapsedMilliseconds

                    # print 'OutputAssembly', params.OutputAssembly

                    for fname in files:
                        params.Input.Add(FileInput(fname))

                    # Create new context
                    ctxt = CompilerContext(params)
                    params.Pipeline.Run(ctxt)
                    # Inject warnings and errors
                    Context.Warnings.AddRange(ctxt.Warnings)
                    Context.Errors.AddRange(ctxt.Errors)

                    watch.Stop()
                    print 'Compiled {0} in {1}ms' % (asmname, watch.ElapsedMilliseconds)

                    if len(ctxt.Errors):
                        return AppDomain.CurrentDomain.DefineDynamicAssembly(
                            AssemblyName('ImportError'),
                            AssemblyBuilderAccess.Run
                        )

                    return ctxt.GeneratedAssembly

        if throwOnError:
            raise System.ApplicationException("Namespace $asmname not found")

        return null

    protected def NamespaceToPaths(asmname as string):
    """ Builds a list of possible directories to map an assembly name. The
        algorithm starts checking dirs separated by dots and generates 
        variants with sub directories.
        ie: foo.bar.baz, foo.bar/baz, foo/bar/baz        
    """
        parts = asmname.Split(char('.'))
        for i in range(len(parts), 0):
            yield Path.Combine(
                join(parts[:i], '.'),
                Path.Combine(*parts[i:])
            )
