namespace booi

from System import Environment
from System.IO import Path, Directory
from Boo.Lang.Useful.CommandLine import *


class CommandLine(AbstractCommandLine):

    [getter(References)]
    _references = List[of string]()

    [getter(Files)]
    _files = List[of string]()

    [getter(Defines)]
    _defines = {}

    [getter(LibPaths)]
    _libpaths = List[of string]()

    [getter(Packages)]
    _packages = List[of string]()

    IsValid:
        get: return true

    [Option("Turns on duck typing by default", LongForm: "ducky")]
    public Ducky = false

    [Option("Generate debugging information (default: +)", LongForm: "debug")]
    public Debug = true

    [Option("Turns on strict mode", LongForm: "strict")]
    public Strict = false

    [Option("Enables white-space-agnostic build", LongForm: "wsa")]
    public Wsa = false

    [Option("Report warnings (default: -)", ShortForm: "w", LongForm: "warnings")]
    public Warnings = false

    [Option("Generate verbose information (default: -)", ShortForm: "v", LongForm: "verbose")]
    public Verbose = false

    [Option("Generate compilation cache files (.booc) (default: -)", LongForm: "cache")]
    public Cache = true

    [Option("Save generated assembly in the given file name (copying dependencies next to it)", ShortForm: "o", LongForm: "output")]
    public Output = null

    [Option("Defines a {symbol}s with optional values (=val)", ShortForm: "d", LongForm: "define")]
    def AddDefine(define as string):
        if not define:
            raise CommandLineException("No value given for the define")

        parts = define.Split(char('='), 2)
        _defines[ parts[0] ] = (null if len(parts) == 1 else parts[1])

    [Option("Adds a {directory} to the list of assembly search paths", ShortForm: "l", LongForm: "lib", MaxOccurs: int.MaxValue)]
    def AddLibPath(libpath as string):
        if not libpath:
            raise CommandLineException('No value given for the lib path')
        _libpaths.AddUnique(TranslatePath(libpath))

    [Option("References {assembly}", ShortForm: 'r', LongForm: "reference", MaxOccurs: int.MaxValue)]
    def AddReference(reference as string):
        if not reference:
            raise CommandLineException("No reference given (ie: -r:my.project.reference)")
        _references.AddUnique(TranslatePath(reference))

    [Option("Adds a packages {directory} for assemblies to load", ShortForm: "p", LongForm: "packages", MaxOccurs: int.MaxValue)]
    def AddPackages(dir as string):
        if not dir:
            raise CommandLineException("No directory given (ie: -p:path/to/packages)")
        _packages.AddUnique(TranslatePath(dir))

    [Option("Runs an {executable} file passing the generated assembly", LongForm: "runner")]
    public Runner as string

    [Option("Display this help and exit", ShortForm: "h", LongForm: "help")]
    public DoHelp = false

    [Option("Display program version", LongForm: "version")]
    public DoVersion = false

    [Argument]
    def AddFile([required] file as string):
        file = TranslatePath(file)

        # this allows wildcard expanding inside .rsp files 
        if '*' in file or '?' in file:
            files = Directory.GetFiles(Path.GetDirectoryName(file), Path.GetFileName(file))
        else:
            files = (file,)

        _files.AddRange(files)

    def constructor(argv):
        Parse(argv)

    IsCygwin:
        get: return Environment.GetEnvironmentVariable("TERM") == "cygwin"
    
    CygwinRoot:
        get:
            home = Environment.GetEnvironmentVariable("HOME")
            assert home is not null
            return home[:home.IndexOf("\\home")]        

    protected def TranslatePath(path as string):
        if path.StartsWith('"') or path.StartsWith("'"):
            path = path[1:-1]

        if not IsCygwin or not path.StartsWith('/'):
            return path

        if path.StartsWith('/cygdrive/'):
            path = path[len('/cygdrive/'):]
            drive = path[0]
            return drive + ':' + path[1:]

        return Path.Combine(CygwinRoot, path[1:])

