namespace Boo.Microsoft.Build.Tasks

import System
import System.IO
import System.Text.RegularExpressions

import Microsoft.Build.Utilities
import Microsoft.Build.Framework
import Microsoft.Build.Tasks


// ManagedCompiler has left the public MSBuild API, and ToolTaskExtension below
// it cannot be derived from outside MSBuild either. Booc derives from ToolTask
// and declares what it used to inherit, under the names every compiler task
// shares so that project files keep working.
class Booc(ToolTask):

    # The source files to compile.
    property Sources as (ITaskItem)
    # The assembly to produce.
    property OutputAssembly as ITaskItem
    # The assemblies to reference.
    property References as (ITaskItem)
    # Response files to pass to the compiler.
    property ResponseFiles as (ITaskItem)
    # Resources to embed, tagged Resx or Non-Resx by their Type metadata.
    property Resources as (ITaskItem)
    # library, exe or winexe.
    property TargetType as string
    # The key file to sign the assembly with.
    property KeyFile as string
    # The key container to sign the assembly with.
    property KeyContainer as string
    # Turns every warning into an error.
    property TreatWarningsAsErrors as bool
    # Writes compiler output as UTF-8.
    property Utf8Output as bool
    # Signs the assembly later, leaving room for the signature now.
    property DelaySign as bool
    # Emits debugging information.
    property EmitDebugInformation as bool
    # Suppresses the compiler banner.
    property NoLogo as bool
    # Ignores booc.rsp.
    property NoConfig as bool
    # Extra directories to search for references.
    property AdditionalLibPaths as (string)

    # Allows to compile unsafe code.
    property AllowUnsafeBlocks as bool
    # Gets/sets if integer overflow checking is enabled.
    property CheckForOverflowUnderflow as bool
    # Gets/sets the culture.
    property Culture as string
    # Gets/sets the conditional compilation symbols.
    property DefineSymbols as string
    # Gets/sets a comma-separated list of warnings that should be disabled.
    property DisabledWarnings as string
    # Gets/sets if we want to use ducky mode.
    property Ducky as bool
    # If set to true the task will output warnings and errors with full file paths
    property GenerateFullPaths as bool
    # Gets/sets if we want to link to the standard libraries or not.
    property NoStandardLib as bool
    # Gets/sets a comma-separated list of optional warnings that should be enabled.
    property OptionalWarnings as string
    # Gets/sets a specific pipeline to add to the compiler process.
    property Pipeline as string
    # Specifies target platform.
    property Platform as string
    # Gets/sets the source directory.
    property SourceDirectory as string
    # Gets/sets whether strict mode is enabled.
    property Strict as bool
    # Gets/sets the verbosity level.
    property Verbosity as string
    # Gets/sets a comma-separated list of warnings that should be treated as errors.
    property WarningsAsErrors as string
    # Gets/sets if we want to use whitespace agnostic mode.
    property WhiteSpaceAgnostic as bool

    # booc.dll to run. Defaults to the copy sitting beside this task assembly.
    property BoocPath as string

    // booc ships as a managed dll on .NET, so the tool being run is the dotnet
    // host and booc.dll is its first argument. The old lookup searched for a
    // booc.exe or a Mono booc script, neither of which is produced any more.
    protected override ToolName:
        get:
            return 'dotnet.exe' if Path.DirectorySeparatorChar == char('\\')
            return 'dotnet'

    protected override def GenerateFullPathToTool() as string:
        # ToolPath wins when set; otherwise the host is found on PATH.
        return Path.Combine(ToolPath, ToolName) if not String.IsNullOrEmpty(ToolPath)
        return ToolName

    private def ResolveBoocPath() as string:
        return BoocPath if not String.IsNullOrEmpty(BoocPath)

        beside = Path.Combine(
            Path.GetDirectoryName(GetType().Assembly.Location), 'booc.dll')
        return beside if File.Exists(beside)

        return 'booc.dll'

    protected override def GetResponseFileSwitch(responseFilePath as string):
        return '@"' + responseFilePath + '"'

    // ToolTaskExtension's AddResponseFileCommands and AddCommandLineCommands
    // were overridden here to suppress the standard switches it contributes.
    // ToolTask contributes none, so there is nothing left to suppress.

    protected override def GenerateCommandLineCommands():
    """ Build a totally customized command line """
        cmdln = CommandLineBuilder()

        # The dotnet host takes booc.dll before any of booc's own switches.
        cmdln.AppendFileNameIfNotNull(ResolveBoocPath())

        cmdln.AppendSwitchIfNotNull("-t:", TargetType.ToLower())
        cmdln.AppendSwitchIfNotNull("-o:", OutputAssembly)
        cmdln.AppendSwitchIfNotNull("-c:", Culture)
        cmdln.AppendSwitchIfNotNull("-srcdir:", SourceDirectory)
        cmdln.AppendSwitchIfNotNull("-keyfile:", KeyFile)
        cmdln.AppendSwitchIfNotNull("-keycontainer:", KeyContainer)
        cmdln.AppendSwitchIfNotNull("-p:", Pipeline)
        cmdln.AppendSwitchIfNotNull("-define:", DefineSymbols)
        cmdln.AppendSwitchIfNotNull("-lib:", AdditionalLibPaths, ",")
        cmdln.AppendSwitchIfNotNull("-nowarn:", DisabledWarnings)
        cmdln.AppendSwitchIfNotNull("-warn:", OptionalWarnings)
        cmdln.AppendSwitchIfNotNull("-platform:", Platform)
 
        if TreatWarningsAsErrors:
            cmdln.AppendSwitch("-warnaserror");  # all warnings are errors
        else:
            cmdln.AppendSwitchIfNotNull("-warnaserror:", WarningsAsErrors)  # only specific warnings are errors
    
        cmdln.AppendSwitch("-nologo") if NoLogo
        cmdln.AppendSwitch("-noconfig") if NoConfig
        cmdln.AppendSwitch("-nostdlib") if NoStandardLib
        cmdln.AppendSwitch("-delaysign") if DelaySign
        cmdln.AppendSwitch("-wsa") if WhiteSpaceAgnostic
        cmdln.AppendSwitch("-ducky") if Ducky
        cmdln.AppendSwitch("-utf8") if Utf8Output
        cmdln.AppendSwitch("-strict") if Strict
        cmdln.AppendSwitch("-unsafe") if AllowUnsafeBlocks

        cmdln.AppendSwitch(('-debug+' if EmitDebugInformation else '-debug-'))
        cmdln.AppendSwitch(('-checked+' if CheckForOverflowUnderflow else '-checked-'))

        if ResponseFiles:
            for rsp in ResponseFiles:
                cmdln.AppendSwitch(GetResponseFileSwitch(rsp.ItemSpec))
                cmdln.AppendSwitchIfNotNull('@', rsp.ItemSpec)        

        if References:
            for reference in References:
                cmdln.AppendSwitchIfNotNull('-r:', reference.ItemSpec)
            
        if Resources:
            for resource in Resources:
                type = resource.GetMetadata('Type')
                if type == 'Resx':
                    cmdln.AppendSwitchIfNotNull("-resource:", resource.ItemSpec + "," + resource.GetMetadata("LogicalName"))
                else:  # if type == 'Non-Resx':
                    cmdln.AppendSwitchIfNotNull("-embedres:", resource.ItemSpec + "," + resource.GetMetadata("LogicalName"))

        if not String.IsNullOrEmpty(Verbosity):
            verbosity = Verbosity.ToLower()
            if verbosity == 'normal':       pass
            elif verbosity == 'warning':    cmdln.AppendSwitch("-v")
            elif verbosity == 'info':       cmdln.AppendSwitch("-vv")
            elif verbosity == 'verbose':    cmdln.AppendSwitch("-vvv");
            else:
                // The resource-based overload came from ToolTaskExtension and
                // read MSBuild's own strings.
                Log.LogError(
                    "Verbosity has an invalid value '{0}'. It must be one of: "
                    + "Normal, Warning, Info, Verbose",
                    Verbosity)

        cmdln.AppendFileNamesIfNotNull(Sources, " ")

        return cmdln.ToString()

    # Captures the file, line, column, code, and message from a BOO warning
    # in the form of: Program.boo(1,1): BCW0000: WARNING: This is a warning.
    private warningPattern = Regex(
            "^(?<file>.*?)(\\((?<line>\\d+),(?<column>\\d+)\\):)?" +
            "(\\s?)(?<code>BCW\\d{4}):(\\s)WARNING:(\\s)(?<message>.*)$",
            RegexOptions.Compiled
    )

    # Captures the file, line, column, code, error type, and message from a
    # BOO error of the form of:
    # 1. Program.boo(1,1): BCE0000: This is an error.
    # 2. Program.boo(1,1): BCE0000: Boo.Lang.Compiler.CompilerError:
    #            This is an error. ---> Program.boo:4:19: This is an error
    # 3. BCE0000: This is an error.
    # 4. Fatal error: This is an error.
    #
    #  The second line of the following error format is not cought because 
    # .NET does not support if|then|else in regular expressions,
    #  and the regex will be horrible complicated.  
    #  The second line is as worthless as the first line.
    #  Therefore, it is not worth implementing it.
    #
    #            Fatal error: This is an error.
    #            Parameter name: format.
    private errorPattern = Regex(
            "^(((?<file>.*?)\\((?<line>\\d+),(?<column>\\d+)\\): )?" +
            "(?<code>BCE\\d{4})|(?<errorType>Fatal) error):" +
            "( Boo.Lang.Compiler.CompilerError:)?" +
            " (?<message>.*?)($| --->)",
            RegexOptions.Compiled |
            RegexOptions.ExplicitCapture |
            RegexOptions.Multiline
        )

    override protected def LogEventsFromTextOutput(ln as string, msgImportance as MessageImportance):
        if msgImportance in (MessageImportance.Normal, MessageImportance.High):
            wMatch = warningPattern.Match(ln)
            eMatch = errorPattern.Match(ln)
            line as int = 0
            column as int = 0

            if wMatch.Success:
                int.TryParse(wMatch.Groups["line"].Value, line)
                int.TryParse(wMatch.Groups["column"].Value, column)

                Log.LogWarning(
                    null,
                    wMatch.Groups["code"].Value.Trim(),
                    null,
                    wMatch.Groups["file"].Value.Trim(),
                    line,
                    column,
                    0,
                    0,
                    wMatch.Groups["message"].Value.Trim()
                )
            elif eMatch.Success:
                code = eMatch.Groups["code"].Value.Trim()
                if string.IsNullOrEmpty(code):
                    code = "BCE0000";
                file = eMatch.Groups["file"].Value.Trim()
                if string.IsNullOrEmpty(file):
                    file = "BOOC"

                int.TryParse(eMatch.Groups["line"].Value, line)
                int.TryParse(eMatch.Groups["column"].Value, column)

                Log.LogError(
                    eMatch.Groups["errorType"].Value.Trim().ToLower(),
                    code,
                    null,
                    file,
                    line,
                    column,
                    0,
                    0,
                    eMatch.Groups["message"].Value.Trim()
                )
        
        super(ln, msgImportance)
            
