#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
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


// By Doug Holton. More stuff added by David Piepgrass.
// For help, please see the help string below.
import System
import System.IO
import System.Xml.Serialization from System.Xml
import Boo.Lang.Compiler from Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Ast.Visitors
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Compiler.Steps
import System.Reflection

[System.Runtime.CompilerServices.CompilerGlobalScopeAttribute]
class Globals:
	static help = """
This script visits the AST structure after each step in the compile
process
and converts it to XML or back to boo syntax. If there are visible
differences since the previous step, it saves the output to a file in a

folder named after the command-line arguments (name of source file plus

options).

How to use:

booi path/to/showcompilersteps.boo [-xml | (-ent | -exp | -nodes) [-short]
[-bind]]
                           path/to/your/script.boo

-xml: generate XML representation of AST
-ent: show entity type names that are associated with AST nodes
(node.Entity)
-exp: show entity type names and expression types (node.ExpressionType)
-nodes: show entity type names and expression types, and for each typed

        expression, also show the AST type (node.GetType())
-bind: show binding information contained in entities
-short: abbreviate the output so that lines hopefully fit on your
screen.

You can also use the "-r:assembly.dll" flag to add assembly references.

ShowSteps will generate a folder in the current directory named after
the
input file and the options you specified. It generates copies of the
script
after each compiler step, and puts them in that folder.

If you use -exp -nodes -bind, the output can get pretty long and
confusing.
For example, a simple expression like "_x", that refers to a variable
_x in
the current class, eventually expands to a the following (all on one
line):

  <InternalField MemberReferenceExpression=double
Bind=Field:Ns.MyClass._x>
    <InternalMethod SelfLiteralExpression=Foo.B
Bind=Method:Ns.MyClass.MyFn>
    self</InternalMethod>._x
  </InternalField>

The -short command-line option will use an abbreviated syntax like
this:

  <ItlField MemberRefrExpr=double @F:Ns.MyClass._x>
    <ItlMethod SelfLiteralExpr=Foo.B @M:Ns.MyClass.MyFn>
    self</IM>._x
  </IF>

Here's how to understand it. First of all, of course, it's not really
XML,
it's just an XML-like notation. If you have a text editor that can do
XML
syntax highlighting, use it. Second, notice that a reference to "self"
has
been added. Third, the outer tag (InternalField) describes the whole
expression, "self._x", whereas the inner tag (InternalMethod) describes
only the "self" part. The tags have the following syntax:

  <E N=T Bind=S:P> where

  N: Class of AST node. For example, "MemberReferenceExpression" refers
     to the Boo.Lang.Compiler.Ast.MemberReferenceExpression class.
  T: The value of node.ExpressionType.ToString(), e.g. int
  E: Type of entity associated with the node, or "_" if the node has no
     entity. For example, "InternalField" actually refers to the
     Boo.Lang.Compiler.TypeSystem.InternalField class. It seems that an
     entity's main purpose is to hold binding information.
  S: The entity's EntityType (although I don't actually know what it's
for.)
     If the EntityType is EntityType.Type, which is the most common
case,
     then S is omitted.
  P: Binding Path. For example, X.Y.Z might represent a variable or
method
     "Z" in class "Y" in namespace "X".

A tag is not printed at all if there is no entity nor data type
associated with a node. That's why you don't see very many tags during
the first compiler steps. The "N=T" part is printed only if the node is
an Expression and it has a a known data type; the Bind=S:P part is only

printed if binding information is available.
	"""
	static format = "boo" //or "xml" //format for output
	static foldername = "compilersteps" //folder where files are saved
	static showents = false  //whether to print entity types
	static showexp = false //show expression types as well
	static shownodetypes = false
	static shorten = false
	static showbindings = false
	//used internally:
	static savefolder as string
	static n = 0
	static laststep as string

//basic boo printer visitor, but adds comments if a node is synthetic (generated
//by the compiler instead of the user).
class BooSyntheticPrinterVisitor(BooPrinterVisitor):
	def constructor(writer as TextWriter):
		super(writer)
	override def Visit(node as Node) as bool:
		if node is not null and node.IsSynthetic:
			WriteIndented("// synthetic")
			WriteLine()
			WriteIndented("")
		return super(node)
			
class BooTypePrinterVisitor(BooPrinterVisitor):
	_showexp = false
	_shownodetypes = false
	_shorten = false
	_showbindings = false
	def constructor(writer as TextWriter, show_expressions as bool, shownodetypes as bool, shorten as bool, showbindings as bool):
		super(writer)
		_showexp = show_expressions
		_shownodetypes = shownodetypes
		_shorten = shorten
		_showbindings = showbindings

	override def Visit(node as Node) as bool:
		return true if node is null
		if node.IsSynthetic:
			WriteIndented("// synthetic")
			WriteLine()
			
		WriteIndented() // Automatically indent iff starting a new line
		tagname = ""
		
		entity = TypeSystemServices.GetOptionalEntity(node) // aka node.Entity
		if entity is not null:
			tagname = ShortName(entity.GetType())
			s = "<"
			s += tagname
			s += ExtraJunk(node)
			s += ">"
			Write(s)
			if _shorten:
				tagname = InitialsOf(tagname)
		elif _showexp or _showbindings:
			junk = ExtraJunk(node)
			if junk.Length > 0:
				tagname = "_"
				s = "<_${junk}>"
				Write(s)
				
		result = super(node)
		if tagname != "":
			WriteIndented("</"+tagname+">")
		return result

	def ShortName(t as object):
		t2 = t.ToString(). \
			Replace("Boo.Lang.Compiler.TypeSystem.",""). \
			Replace("Boo.Lang.Compiler.Ast.","")
		return t2 unless _shorten
		return t2. \
			Replace("Expression", "Expr"). \
			Replace("Reference", "Refr"). \
			Replace("Internal", "Itl").Replace("External", "Xtl")

	def InitialsOf(s as string):
		s2 = System.Text.StringBuilder()
		for ch in s:
			if ch >= char('A') and ch <= char('Z'):
				s2.Append(ch)
		if s2.Length>0:
			return s2.ToString()
		else:
			return s

	def ExtraJunk(node as Node):
		s = System.Text.StringBuilder()
		if _showexp:
			exp = node as Expression
			if exp is not null and exp.ExpressionType is not null:
				if _shownodetypes:
					s.Append(" ")
					s.Append(ShortName(node.GetType()))
					s.Append("=")
				elif _shorten:
					s.Append(":")
				else:
					s.Append(" EType=")
				s.Append(ShortName(exp.ExpressionType.ToString()))

		if _showbindings:
			entity = TypeSystemServices.GetOptionalEntity(node) // aka node.Entity
			if entity is not null:
				if _shorten:
					s.Append(" @")
				else:
					s.Append(" Bind=")
				if entity.EntityType != EntityType.Type:
					if _shorten:
						s.Append(InitialsOf(entity.EntityType.ToString()))
					else:
						s.Append(entity.EntityType.ToString())
					s.Append(char(':'))
				s.Append(entity.FullName)
		return s.ToString()

def PrintAST([required]result as CompilerContext, [required]o as TextWriter):
	astobject = result.CompileUnit
	try:
		s = XmlSerializer( astobject.GetType() )
		s.Serialize( o, astobject )
	except e:
		print
		print e.GetType(), ":", e.Message

def AfterStep(sender, e as CompilerStepEventArgs):
	++n
	stepname = e.Step.ToString().Replace("Boo.Lang.Parser.","").Replace("Boo.Lang.Compiler.Steps.","")

	tempfile = Path.GetTempFileName()
	using temp = StreamWriter(tempfile):
		if format == "xml":
			PrintAST(e.Context, temp)
		else:
			try:
				printer as BooPrinterVisitor
				if showents:
					printer = BooTypePrinterVisitor(temp, showexp, shownodetypes, shorten, showbindings)
				else:
					printer = BooSyntheticPrinterVisitor(temp)
				printer.Print(e.Context.CompileUnit)
			except e:
				print e.Message + "\n" + e.StackTrace

	using r = StreamReader(tempfile):
		thisstep = r.ReadToEnd()

	filename = string.Format("STEP{0:D2}-{1}.{2}", n, stepname, format)

	if thisstep != laststep:
		File.Move(tempfile, Path.Combine(savefolder, filename))
		laststep = thisstep
		print string.Format("STEP{0:D2}-{1}: SAVED TO {2} FILE.", n, stepname, format.ToUpper())
	else:
		File.Delete(tempfile)
		print string.Format("STEP{0:D2}-{1}: NO CHANGE TO AST.", n, stepname)

def LoadAssembly(assemblyName as string) as Assembly:
	reference as Assembly
	if File.Exists(Path.GetFullPath(assemblyName)):
		reference = Assembly.LoadFrom(Path.GetFullPath(assemblyName))
	if reference is null:
		reference = Assembly.LoadWithPartialName(assemblyName)
		if reference is null:
			raise ApplicationException(
				ResourceManager.Format("BooC.UnableToLoadAssembly", 
							assemblyName))
	return reference

///////////////////////////////////////////////////

if len(argv) == 0:
	print help
	return

compiler = BooCompiler()

compiler.Parameters.Pipeline = Compile()
compiler.Parameters.Pipeline.AfterStep += AfterStep

foldername_base = foldername_extra = ""

for arg in argv:

	if arg[0:3] == "-r:":
		compiler.Parameters.References.Add(LoadAssembly(arg[3:]))
		continue
	elif arg == "-xml":
		format = "xml"
	elif arg == "-ent":
		showents = true
	elif arg == "-exp":
		showents = true
		showexp = true
	elif arg == "-ducky":
		compiler.Parameters.Ducky = true
	elif arg == "-nodes":
		showents = true
		showexp = true
		shownodetypes = true
	elif arg == "-short":
		shorten = true
	elif arg == "-bind":
		showbindings = true
	else:
		compiler.Parameters.Input.Add(FileInput(arg))
		foldername_base += /^(.*?[\/\\])*([^\\\/]+?)(\.[^.\\\/]*)?$/.Match(arg).Groups[2]
		continue
	foldername_extra += " " + arg

foldername = foldername_base + foldername_extra

//delete old folder if running more than once:
if Directory.Exists(foldername):
	Directory.Delete(foldername, true)

savedir = Directory.CreateDirectory(foldername)
if savedir is null or not Directory.Exists(foldername):
	print "The directory '${foldername}' could not be created."
	return

savefolder = savedir.FullName

try:
	print
	print "See boo/src/Boo.Lang.Compiler/Steps/ for the source code for these steps."
	print
	result = compiler.Run()
	if len(result.Errors) > 0:
		print "\nThere were ${len(result.Errors)} errors compiling the boo file(s)"
		print result.Errors.ToString(true)
	else:
		print "\nSuccessful: See the files under: '${savefolder}'"
except e:
	print e.GetType(), ":", e.Message
