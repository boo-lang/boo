#region license
// Copyright (c) 2013 by Harald Meyer auf'm Hofe (harald.meyer@users.sourceforge.net)
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

namespace Boo.Lang.Useful.Doc

import System
import System.Xml
import System.Collections
import System.IO

static class DocDB:
"""
	A singleton implementing a database of XML documentation on assemblies
	(probably in Sandcastle style). The database provides automatic reload
	w.r.t. some predefined or user defined look up paths.
"""
	[Getter(LookupPaths)]
	_lookupPaths as Generic.ICollection[of string] = List[of string]()
	
	def constructor():
		p=Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
		if Directory.Exists(p):
			p=Path.Combine(p, "Reference Assemblies\\Microsoft")
			if Directory.Exists(p):
				_lookupPaths.Add(p)
		p=Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
		if Directory.Exists(p):
			p=Path.Combine(p, "Reference Assemblies\\Microsoft")
			if Directory.Exists(p):
				_lookupPaths.Add(p)
	
	_db = Generic.SortedList[of string, AssemblyDoc]() 
	
	public def ToDesignator(ns as string):
	"""
		Returns the designator of a namespace.
	"""
		return "N:"+ns
	
	public def ToDesignator(t as Type):
	"""
		Returns the designator that an be used to retrieve docs on
		a certain type.
	"""
		return "T:"+t.ToString()
	
	public def ToDesignator(a as Reflection.Assembly):
	"""
		Returns a designator that can be used to retrieve docs on
		a certain assembly.
	"""
		return a.GetName().Name	
	
	public def ToDesignator(mi as Reflection.MethodInfo, withArguments as bool):
	"""
		Creates a designator for a particular method.
	"""
		r = "M:"+mi.DeclaringType.ToString()+"."+mi.Name
		parameters = mi.GetParameters()
		if withArguments and parameters != null and parameters.Length > 0:
			r+="("
			i=0
			for p in parameters:
				r+="," if i > 0
				r+=p.ParameterType.ToString()
				i+=1
			r+=")"
		return r
	
	public def ToDesignator(mi as Reflection.ConstructorInfo, withArguments as bool):
	"""
		Creates a designator for a particular ctor.
	"""
		r = "M:"+mi.DeclaringType.ToString()+".#ctor"
		parameters = mi.GetParameters()
		if withArguments and parameters != null and parameters.Length > 0:
			r+="("
			i=0
			for p in parameters:
				r+="," if i > 0
				r+=p.ParameterType.ToString()
				i+=1
			r+=")"
		return r
	
	public def ToDesignator(pi as Reflection.PropertyInfo):
	"""
		Creates a designator for a particular property.
	"""
		return "P:"+pi.DeclaringType.ToString()+"."+pi.Name
	
	public def ToDesignator(fi as Reflection.FieldInfo):
	"""
		Creates a designator for a particular field.
	"""
		return "F:"+fi.DeclaringType.ToString()+"."+fi.Name
	
	public def ToDesignator(evt as Reflection.EventInfo):
	"""
		Creates a designator for a particular event.
	"""
		return "E:"+evt.DeclaringType.ToString()+"."+evt.Name
	
	public def Find(assembly as Reflection.Assembly, elementDesignator):
	"""
		Find documentation of a particular element that refers to
		a particular assembly. This will return null if nothing has
		been found.
	"""
		assemblyDesignator = ToDesignator(assembly)
		assemblyDoc as AssemblyDoc
		if _db.TryGetValue(assemblyDesignator, assemblyDoc):
			return assemblyDoc.Find(elementDesignator)
		try:
			uri = Uri(assembly.CodeBase)
			path=uri.LocalPath
			path=Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path))+".xml"
			if File.Exists(path):
				assemblyDoc=AssemblyDoc(path)
				_db.Add(assemblyDesignator, assemblyDoc)
				return assemblyDoc.Find(elementDesignator)
		except:
			pass
		for d in _lookupPaths:
			try:
				path = FindFile(d, assemblyDesignator+".xml")
				if not string.IsNullOrEmpty(path):
					assemblyDoc=AssemblyDoc(path)
					_db.Add(assemblyDesignator, assemblyDoc)
					return assemblyDoc.Find(elementDesignator)
			except:
				pass
		return null
	
	def FindFile(dir as string, fileName as string) as string:
		path = Path.Combine(dir, fileName)
		return path if File.Exists(path)
		for subdir in Directory.GetDirectories(dir):
			path = FindFile(subdir, fileName)
			if not string.IsNullOrEmpty(path):
				return path
		return null
	
	public def Find(t as Type):
	"""
		Return the XML nodes comprising the documentation on
		a particular type or null if the documentation cannot
		be found.
	"""
		return Find(t.Assembly, ToDesignator(t))
	
	public def Find(mi as Reflection.MethodInfo):
	"""
		Return the XML nodes comprising the documentation on
		a particular method or null if the documentation cannot
		be found.
	"""
		result = Find(mi.DeclaringType.Assembly, ToDesignator(mi, true))
		result = Find(mi.DeclaringType.Assembly, ToDesignator(mi, false)) if result == null
		return result
	
	public def Find(ctor as Reflection.ConstructorInfo):
	"""
		Return the XML nodes comprising the documentation on
		a particular constructor or null if the documentation cannot
		be found.
	"""
		result = Find(ctor.DeclaringType.Assembly, ToDesignator(ctor, true))
		result = Find(ctor.DeclaringType.Assembly, ToDesignator(ctor, false)) if result == null
		return result
	
	public def Find(pi as Reflection.PropertyInfo):
	"""
		Return the XML nodes comprising the documentation on
		a particular property or null if the documentation cannot
		be found.
	"""
		return Find(pi.DeclaringType.Assembly, ToDesignator(pi))
	
	public def Find(fi as Reflection.FieldInfo):
	"""
		Return the XML nodes comprising the documentation on
		a particular property or null if the documentation cannot
		be found.
	"""
		return Find(fi.DeclaringType.Assembly, ToDesignator(fi))
	
	public def Find(evt as Reflection.EventInfo):
	"""
		Return the XML nodes comprising the documentation on
		a particular event or null if the documentation cannot
		be found.
	"""
		return Find(evt.DeclaringType.Assembly, ToDesignator(evt))
	
	
	