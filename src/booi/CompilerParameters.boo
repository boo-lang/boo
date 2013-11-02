namespace booi

from System import AppDomain, DateTime
from System.IO import Path, Directory, File
from System.Reflection import Assembly, AssemblyName
from System.Diagnostics import Stopwatch

from Boo.Lang.Compiler import CompilerContext, \
                              CompilerParameters as BooCompilerParameters
from Boo.Lang.Compiler.IO import FileInput, StringInput
from Boo.Lang.Compiler.Pipelines import CompileToMemory, CompileToFile
from Boo.Lang.Compiler.TypeSystem.Reflection import IAssemblyReference


class CompilerParameters(BooCompilerParameters):

    Context as CompilerContext:
        get: return CompilerContext.Current

    property Cache as bool

    # Keeps track namespaces already mapped to a directory to 
    # detect mutual/cyclic dependencies
    property MappedNamespaces = []

    def constructor():
        super()

    def constructor(loadDefault as bool):
        super(loadDefault)

    virtual protected def GetCacheFilePath(path as string) as string:
        return path + '.booc'

    virtual protected def GetNamespacePaths(ns as string) as string*:
    """ Builds a list of possible directories to map an assembly name. The
        algorithm starts checking dirs separated by dots and generates 
        variants with sub directories.
        ie: foo.bar.baz, foo.bar/baz, foo/bar/baz        
    """
        paths = []
        parts = ns.Split(char('.'))
        for i in range(len(parts), 0):
            paths.Add(Path.Combine(
                join(parts[:i], '.'),
                Path.Combine(*parts[i:])
            ))

        for libpath in LibPaths:
            Context.TraceInfo('Looking for namespace {0} in "{1}"', ns, libpath)
            for dirpath in paths:
                path = Path.Combine(libpath, dirpath)
                if Directory.Exists(path):
                    yield path

    virtual protected def ForkCompiler(asmname as string, path as string) as CompilerContext:
    """ Forks the current compiler into a new one to compile the source files referenced
        in an import.
    """
        # Create new params copying settings from current one
        params = CompilerParameters(false)
        params.Cache = self.Cache
        params.Strict = self.Strict
        params.Debug = self.Debug
        params.References = self.References
        params.MappedNamespaces = self.MappedNamespaces

        # Copy configured lib paths
        for path in self.LibPaths:
            params.LibPaths.AddUnique(path)

        # If we are generating an assembly on disk do so for this namespace too
        if self.OutputAssembly:
            params.OutputAssembly = Path.Combine(
                Path.GetDirectoryName(self.OutputAssembly),
                asmname + '.dll'
            )
            params.Pipeline = CompileToFile()
        else:
            # TODO: When compiling to memory we cannot use compilation cache.
            #       Unless we define a temp path for the assemblies to be generated
            params.Pipeline = CompileToMemory()

        return CompilerContext(params)

    override def ForName(asmname as string, throwOnError as bool) as Assembly:
        # Loading default assemblies is performed normally
        if not Context:
            return super(asmname, throwOnError)

        # Try to load the assembly using Boo's default mechanism
        try:
            return super(asmname, true)
        except ex as System.ApplicationException:
            pass

        # Try to compile from source files
        asm = FromSources(asmname)
        if not asm and throwOnError:
            raise System.ApplicationException("Namespace $asmname not found")

        return asm

    virtual def FromSources(asmname as string) as Assembly:

        Context.TraceEnter('FromSources {0}', asmname)

        watch = Stopwatch()
        watch.Start()

        # Check every possible variation of paths for a namespace
        for path in GetNamespacePaths(asmname):
            # Collect source files, at least one must be present
            files = Directory.GetFiles(path, '*.boo')
            continue if not len(files)

            if asmname in MappedNamespaces:
                Context.TraceLeave('Namespace {0} already mapped. Mutual reference?', asmname)
                return null

            MappedNamespaces.AddUnique(asmname)

            Context.TraceInfo("Mapping namespace {0} to '{1}'", asmname, path)

            cachepath = GetCacheFilePath(path)
            if Cache and File.Exists(cachepath):
                try:
                    cache = CompilationCache.Load(cachepath)
                    if cache.CheckValidity(Context):
                        # Make sure we copy the .dll into the output directory
                        if self.OutputAssembly:
                            asmpath = Path.GetDirectoryName(self.OutputAssembly)
                            cache.DumpAssembly(asmpath, asmname)
                            Context.TraceInfo('Dumped cached assembly to "{0}"', asmpath)

                        asm = cache.GetAssembly()
                        AddAssembly(asm)
                        Context.TraceLeave('Using cached compilation for {0}', asmname)
                        return asm
                    else:
                        Context.TraceInfo('Cached compilation is out of date for {0}', asmname)
                except ex:
                    Context.TraceError('Error loading compilation cache for "{0}": {1}', asmname, ex)

            context = Context

            # Create new compiler context
            ctxt = ForkCompiler(asmname, path)

            # HACK: Dynamic assemblies produce different module version ids depending on how they
            #       are loaded. So we use a different mechanism to signal changes in them, using a 
            #       custom assembly attribute recording the compilation time we can later check 
            #       if they have changed correlating this value with the one stored in cache
            #       references.
            ctxt.Parameters.Input.Add(
                StringInput(
                    'BooiAssemblyInfo.boo', 
                    '[assembly: booi.CompilationTimeAttribute({0})]' % (DateTime.UtcNow.ToFileTimeUtc(),)
                )
            )

            for fname in files:
                ctxt.Parameters.Input.Add(FileInput(fname))

            # Run the compiler
            ctxt.Parameters.Pipeline.Run(ctxt)
            # Inject warnings and errors into the parent context
            Context.Warnings.AddRange(ctxt.Warnings)
            Context.Errors.AddRange(ctxt.Errors)

            if len(ctxt.Errors):
                Context.TraceLeave('Errors compiling {0} from {1}', asmname, path)
                return null
                # return AppDomain.CurrentDomain.DefineDynamicAssembly(
                #     AssemblyName('ImportError'),
                #     System.Reflection.Emit.AssemblyBuilderAccess.Run
                # )

            if Cache:
                try:
                    cache = CompilationCache()
                    cache.BasePath = path
                    for fpath in files:
                        fname = Path.GetFileName(fpath)
                        cache.Sources[fname] = File.GetLastWriteTimeUtc(fpath).ToFileTimeUtc()

                    for rf as IAssemblyReference in ctxt.Parameters.References:
                        asm = rf.Assembly
                        attrs = asm.GetCustomAttributes(CompilationTimeAttribute, false)
                        if len(attrs):
                            dt = (attrs[0] as CompilationTimeAttribute).GetDateTime()
                            cache.References[rf.Name] = dt.ToFileTimeUtc()
                        else:
                            cache.References[rf.Name] = asm.ManifestModule.ModuleVersionId

                    cache.Save(cachepath, ctxt.Parameters.OutputAssembly)
                    Context.TraceInfo('Saved cached compilation into {0}', cachepath)
                except ex:
                    Context.TraceError('Error saving cached compilation: {0}', ex)

            watch.Stop()
            Context.TraceLeave('Compilation for {0} took {1}ms', asmname, watch.ElapsedMilliseconds)

            return ctxt.GeneratedAssembly

        Context.TraceLeave('No directory found to map {0}', asmname)
        return null

