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
		


