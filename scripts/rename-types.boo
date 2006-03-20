#!/usr/bin/env booi
import Boo.Lang.Useful.IO from Boo.Lang.Useful
import System
import System.IO

class Program:
	types = (
		("System.Int32", "int"),
		("System.Int64", "long"),
		("System.Single", "single"),
		("System.Double", "double"),
		("System.Boolean", "bool"),
		("System.Void", "void"),
		("System.Object", "object"),
		("System.String", "string"),
	)
	
	def run(path as string):
		for fname in Directory.GetFiles(path, "*.boo"):
			processFile(fname)
			
	def processFile(fname as string):
		originalContents = contents = TextFile.ReadFile(fname)
		for systemType, builtinType in types:
			contents = contents.Replace(systemType, builtinType)
		if contents != originalContents:
			print fname
			TextFile.WriteFile(fname, contents)
			
path, = argv
Program().run(path)


	
