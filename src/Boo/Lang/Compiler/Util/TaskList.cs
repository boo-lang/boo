#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;

namespace Boo.Lang.Ast.Compiler.Util
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
