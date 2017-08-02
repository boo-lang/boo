#region license
// Copyright (c) 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
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

namespace Boo.Lang.Useful.CommandLine

import System
import System.IO
import System.Text
import System.Text.RegularExpressions
import System.Collections
import System.Reflection

class AbstractCommandLine:
	
	static OptionValueDescriptionRegex = /\{(.+)\}/
	
	[Getter(Empty)]
	_empty as bool
	
	virtual ParseableMembersFlags:
		get:
			return BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance
		
	virtual def Parse([required] argv as (string)):
		parser = CreateParser()
		parser.Parse(argv)
		_empty = parser.Empty
		
	virtual def PrintOptions():
		self.PrintOptions(Console.Out)
		
	virtual def PrintOptions(writer as TextWriter):
		lines = []
		for attr, member in GetAttributes():
			continue if attr isa ArgumentAttribute
			
			option as OptionAttribute = attr
			description = GetOptionDescription(option)
			
			buffer = StringBuilder()
			if option.ShortForm is not null:
				buffer.Append(" -${option.ShortForm} ")
			else:
				buffer.Append("    ")
				
			buffer.Append(" -${option.LongForm}")
			buffer.Append(GetOptionValueDescription(option, description, member))
			buffer.Append("  ")
				
			lines.Add((option.ToString(), buffer.ToString(), RemoveBracesFromValueDescription(description)))
			
		return if 0 == len(lines)
			
		lines.Sort() do (lhs, rhs):
			_, lline as string = lhs
			_, rline as string = rhs
			return rline.Length - lline.Length
			
		_, longerLine as string = lines[0]
		
		lines.Sort() do (lhs, rhs):
			loption, = lhs
			roption, = rhs
			return loption.ToString().CompareTo(roption.ToString())
			
		for item in lines:
			_, line as string, desc as string = item
			writer.Write(line)
			writer.Write(" " * (longerLine.Length - line.Length))
			writer.WriteLine(desc)
			
	protected virtual def GetOptionValueDescription(option as OptionAttribute, description as string, member as MemberInfo):
		field = member as FieldInfo
		return "[+-]" if field is not null and IsBoolField(field)
		
		m = OptionValueDescriptionRegex.Matches(description)
		return ":${m[0].Groups[1]}" if m.Count > 0
		return ":${option}"
		
	protected virtual def RemoveBracesFromValueDescription(description as string):
		return OptionValueDescriptionRegex.Replace(description) do (match as Match):
			return match.Groups[1].Value
			
	protected virtual def GetOptionDescription(option as OptionAttribute):
		return option.Description
			
	protected def CreateParser():
		parser = Parser()
		for attr, member in GetAttributes():
			if attr isa OptionAttribute:
				AddOption(parser, attr, member)
			else:
				AddArgument(parser, attr, member)
		return parser
				
	protected def GetAttributes():
		for member in GetType().GetMembers(ParseableMembersFlags):
			option = GetOptionAttribute(member)
			argument = GetArgumentAttribute(member)
			if option is not null:
				if argument is not null:
					raise ArgumentException("'${member}' is marked as both an argument and an option.")
				if option.LongForm is null:
					option.LongForm = member.Name.ToLower()
				yield option, member
			elif argument is not null:
				yield argument, member
		
	private def AddOption(parser as Parser, option as OptionAttribute, member):
		field = member as FieldInfo
		if field is not null:
			AddOptionField(parser, option, field)
			return
			
		method = member as MethodInfo
		if method is not null and len(method.GetParameters()) == 1:
			AddOptionMethod(parser, option, method)
			return

		raise NotImplementedException("OptionAttribute not implemented for '${member}'")

	private def AddOptionMethod(parser as Parser, option as OptionAttribute, method as MethodInfo):
		parser.AddOption(option) do (value as string):
			method.Invoke(self, (value,))
		
	private def AddOptionField(parser as Parser, option as OptionAttribute, field as FieldInfo):
		if IsIListField(field):
			parser.AddOption(option) do (value as string):
				AddToListField(field, value)
		elif IsBoolField(field):
			parser.AddOption(option) do (value as string):
				boolValue = value is null or value == "+"
				field.SetValue(self, boolValue)
		else:
			parser.AddOption(option) do (value as string):
				field.SetValue(
					self,
					Convert.ChangeType(value, field.FieldType))
		
	private def AddArgument(parser as Parser, argument as ArgumentAttribute, member):
		method = member as MethodInfo
		if method is not null:
			parser.ArgumentFound += do (value as string):
				method.Invoke(self, (value,))
			return
		
		field = member as FieldInfo
		if field is not null and IsIListField(field):
			parser.ArgumentFound += do (value as string):
				AddToListField(field, value)
			return

		raise NotImplementedException("ArgumentAttribute not implemented for '${member}'")

	private def AddToListField(field as FieldInfo, value):
		(field.GetValue(self) as IList).Add(value)
		
	private def IsIListField(field as FieldInfo):
		return IList in field.FieldType.GetInterfaces()
		
	private def IsBoolField(field as FieldInfo):
		return bool is field.FieldType
				
	protected def GetOptionAttribute(member as MemberInfo) as OptionAttribute:
		return Attribute.GetCustomAttribute(member, OptionAttribute)
		
	protected def GetArgumentAttribute(member as MemberInfo) as ArgumentAttribute:
		return Attribute.GetCustomAttribute(member, ArgumentAttribute)
