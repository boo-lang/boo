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


using System;

namespace Boo.Lang.Compiler.Ast
{
	public partial class DepthFirstVisitor
	{
		public virtual bool Visit(Node node)
		{
			if (null != node)
			{
				try
				{
					node.Accept(this);
					return true;
				}
				catch (LongJumpException)
				{
					throw;
				}
				catch (Boo.Lang.Compiler.CompilerError)
				{
					throw;
				}
				catch (Exception error)
				{
					OnError(node, error);
				}
			}
			return false;
		}
		
		protected bool VisitAllowingCancellation(Node node)
		{
			try
			{
				Visit(node);
				return true;
			}
			catch (LongJumpException)
			{	
			}
			return false;
		}

		static readonly LongJumpException CancellationException = new LongJumpException();

		protected void Cancel()
		{
			throw CancellationException;
		}

		protected virtual void OnError(Node node, Exception error)
		{
			throw Boo.Lang.Compiler.CompilerErrorFactory.InternalError(node, error);
		}

		public void Visit(Node[] array, NodeType nodeType)
		{
			foreach (Node node in array)
			{
				if (node.NodeType == nodeType)
				{
					Visit(node);
				}
			}
		}
		
		public void VisitCollection(System.Collections.IEnumerable collection)
		{
			foreach (Node node in collection)
			{
				Visit(node);
			}
		}

		public bool Visit<T>(NodeCollection<T> collection, NodeType nodeType) where T : Node
		{
			if (null != collection)
			{
				Visit(collection.ToArray(), nodeType);
				return true;
			}
			return false;
		}

		public void Visit(Node[] array)
		{
			foreach (Node node in array)
			{
				Visit(node);
			}
		}

		public bool Visit<T>(NodeCollection<T> collection) where T : Node
		{
			if (null != collection)
			{
				Visit(collection.ToArray());
				return true;
			}
			return false;
		}
	}
}
