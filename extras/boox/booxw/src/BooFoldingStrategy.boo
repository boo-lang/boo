#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
//
// This file is part of Boo Explorer.
//
// Boo Explorer is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// Boo Explorer is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Foobar; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
#endregion

namespace BooExplorer

import ICSharpCode.TextEditor
import ICSharpCode.TextEditor.Document
import ICSharpCode.TextEditor.Actions

import System.Collections

import Boo.Lang.Compiler.Ast

class BooFoldingStrategy(IFoldingStrategy):
	
	[getter(Instance)]
	static _instance = BooFoldingStrategy()
	
	class FoldingVisitor(DepthFirstVisitor):
		
		[getter(Markers)]
		_markers = ArrayList()
		
		_document as IDocument
		
		def constructor(document):
			_document = document
			
		override def LeaveConstructor(node as Constructor):
			LeaveMethod(node)
			
		override def LeaveMethod(node as Method):
			AddMarker(node.LexicalInfo, node.Body.EndSourceLocation, FoldType.MemberBody)
			
		override def LeaveProperty(node as Property):
			end as SourceLocation
			if node.Getter is not null:
				end = node.Getter.Body.EndSourceLocation
			if node.Setter is not null:
				if end is not null:
					candidate = node.Setter.Body.EndSourceLocation
					if candidate.Line > end.Line:
						end = candidate
			AddMarker(node.LexicalInfo, end, FoldType.MemberBody)
			
		override def LeaveClassDefinition(node as ClassDefinition):
			LeaveTypeDefinition(node)
			
		override def LeaveInterfaceDefinition(node as InterfaceDefinition):
			LeaveTypeDefinition(node)
			
		override def LeaveEnumDefinition(node as EnumDefinition):
			LeaveTypeDefinition(node)
			
		def LeaveTypeDefinition(node as Node):
			AddMarker(node, FoldType.TypeBody)
			
		def AddMarker(node as Node, type as FoldType):
			start = node.LexicalInfo
			end = node.EndSourceLocation
			AddMarker(start, end, type)
			
		def AddMarker(start as SourceLocation, end as SourceLocation, type as FoldType):			
			return unless start.IsValid and end.IsValid
			_markers.Add(
				FoldMarker(_document,
						start.Line-1, _document.GetLineSegment(start.Line-1).Length + 1,
						end.Line-1, _document.GetLineSegment(end.Line-1).Length,
						type))
	
	def GenerateFoldMarkers(document as IDocument,
					fileName as string,
					parseInformation as object) as ArrayList:
						
		module as Module = parseInformation
		visitor = FoldingVisitor(document)
		visitor.Visit(module)
		return visitor.Markers

