#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;

namespace Boo.Lang.Compiler.Util
{
	/// <summary>
	/// Uma lista de tarefas.
	/// </summary>
	public class TaskList
	{
		System.Collections.ArrayList _tasks = new System.Collections.ArrayList();

		/// <summary>
		/// Nmero de tarefas na lista.
		/// </summary>
		public int Count
		{
			get
			{
				return _tasks.Count;
			}
		}

		/// <summary>
		/// Adiciona uma nova tarefa  lista.
		/// </summary>
		/// <param name="task"></param>
		public void Add(ITask task)
		{
			if (null == task)
			{
				throw new ArgumentNullException("task");
			}
			_tasks.Add(task);
		}

		/// <summary>
		/// Executa todas as tarefas pendentes na lista.
		/// </summary>
		public void Flush()
		{
			try
			{
				for (int i=_tasks.Count; i>0; --i)
				{
					ITask task = (ITask)_tasks[i-1];
					task.Execute();
				}
			}
			finally
			{
				_tasks.Clear();
			}
		}
	}
}
