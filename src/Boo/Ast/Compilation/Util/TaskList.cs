using System;

namespace Boo.Ast.Compilation.Util
{
	/// <summary>
	/// Uma lista de tarefas.
	/// </summary>
	public class TaskList
	{
		System.Collections.ArrayList _tasks = new System.Collections.ArrayList();

		/// <summary>
		/// Número de tarefas na lista.
		/// </summary>
		public int Count
		{
			get
			{
				return _tasks.Count;
			}
		}

		/// <summary>
		/// Adiciona uma nova tarefa à lista.
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
