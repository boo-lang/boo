import System
import Gtk from "gtk-sharp"
import Gdk from "gdk-sharp" as Gdk
import Pango from "pango-sharp" as Pango
import booish from booish

class PromptView(TextView):
	
	_interpreter = InteractiveInterpreter(RememberLastValue: true)
	
	def constructor():
		
		if Environment.OSVersion.Platform in PlatformID.Win32NT, PlatformID.Win32Windows:
			self.ModifyFont(
				Pango.FontDescription(Family: "Lucida Console", Size: 12))
		else:
			self.ModifyFont(
				Pango.FontDescription(Family: "Courier New"))
			
		_interpreter.References.Add(typeof(TextView).Assembly)
		_interpreter.References.Add(typeof(Gdk.Key).Assembly)
		_interpreter.SetValue("print", print)
		_interpreter.SetValue("dir", dir)
		_interpreter.SetValue("help", help)
		_interpreter.SetValue("cls", { Buffer.Text = "" })
		
		prompt()
		
	override def OnKeyPressEvent(ev as Gdk.EventKey):
		if Gdk.Key.Return == ev.Key:
			try:			
				EvalCurrentLine()
			except x:
				print(x)
			prompt()
			return true
		elif ev.Key in Gdk.Key.BackSpace, Gdk.Key.Left:
			if Buffer.GetIterAtMark(Buffer.InsertMark).LineOffset < 5:
				return true
			
		return super(ev)
		
	def print(obj):
		Buffer.InsertAtCursor("${obj}\n")
		
	def prompt():
		Buffer.MoveMark(Buffer.InsertMark, Buffer.EndIter)
		Buffer.InsertAtCursor(">>> ")
		
	def dir([required] obj):
		type = (obj as Type) or obj.GetType()
		for member in type.GetMembers():
			method = member as System.Reflection.MethodInfo
			if method is not null:
				yield method if method.IsPublic and not method.IsSpecialName
			else:
				yield member
		
	def help(obj):
		print(join(dir(obj), "\n"))
		
	def repr(value):
		writer = System.IO.StringWriter()
		WriteRepr(writer, value)
		return writer.ToString()
		
	def EvalCurrentLine():
		start = Buffer.GetIterAtLine(Buffer.LineCount)
		line = Buffer.GetText(start, Buffer.EndIter, false)
			
		print("")
		result = _interpreter.Eval(line[4:])
		if len(result.Errors):
			
			for error in result.Errors:
				pos = error.LexicalInfo.StartColumn
				print("---" + "-"*pos + "^") if pos > 0
				print("ERROR: ${error.Message}")
		else:
			_ = _interpreter.LastValue
			if _ is not null:
				print(repr(_))
				_interpreter.SetValue("_", _)		

class MainWindow(Window):
	
	def constructor():
		super("booish")
	
		window = ScrolledWindow()
		window.Add(PromptView())
		
		self.Add(window)
		
		self.DeleteEvent += Application.Quit

Application.Init()

window = MainWindow(DefaultWidth:  400,
				DefaultHeight: 250)
				
window.ShowAll()

Application.Run()
