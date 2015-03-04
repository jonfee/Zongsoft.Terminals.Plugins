/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2013 Zongsoft Corporation <http://www.zongsoft.com>
 *
 * This file is part of Zongsoft.Terminals.Plugins.
 *
 * Zongsoft.Terminals.Plugins is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * Zongsoft.Terminals.Plugins is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with Zongsoft.Terminals.Plugins; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 */

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

using Zongsoft.Plugins;
using Zongsoft.Diagnostics;
using Zongsoft.Terminals;
using Zongsoft.Resources;
using Zongsoft.Services;

namespace Zongsoft.Terminals.Plugins
{
	public class Workbench : Zongsoft.Plugins.WorkbenchBase
	{
		#region 成员字段
		private ConsoleTerminal _terminal;
		#endregion

		#region 构造函数
		public Workbench(PluginApplicationContext applicationContext) : base(applicationContext)
		{
		}
		#endregion

		#region 公共属性
		public ConsoleTerminal Terminal
		{
			get
			{
				if(_terminal == null)
				{
					System.Threading.Interlocked.CompareExchange(ref _terminal, new ConsoleTerminal(), null);
				}

				return _terminal;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_terminal = value;
			}
		}

		public CommandExecutor Executor
		{
			get
			{
				return this.Terminal.Executor;
			}
		}
		#endregion

		#region 打开方法
		protected override void OnStart(string[] args)
		{
			var terminal = this.Terminal;
			var executor = this.Terminal.Executor;

			//挂载终端运行器的失败事件
			executor.Failed += delegate(object source, FailureEventArgs e)
			{
				if(e.Exception is CommandNotFoundException)
					terminal.WriteLine(TerminalColor.Red, string.Format(ResourceUtility.GetString("${Text.CommandNotFound}"), ((CommandNotFoundException)e.Exception).Path));
				else
				{
					terminal.WriteLine(TerminalColor.Red, ResourceUtility.GetString("${Text.CommandInvokeFailed}"));

					if(e.Exception != null)
						terminal.WriteLine(TerminalColor.Magenta, e.Exception.Message);
				}
			};

			//调用基类同名方法
			base.OnStart(args);

			//激发“Opened”事件
			this.OnOpened(EventArgs.Empty);

			//启动命令运行器
			executor.Run();
		}
		#endregion

		#region 关闭方法
		protected override void OnStop()
		{
			//获取终端命令的根节点
			PluginTreeNode node = this.PluginContext.PluginTree.Find(this.PluginContext.Settings.WorkbenchPath, "Terminal", "Commands");

			if(node != null)
			{
				foreach(var commandNode in node.Children)
				{
					object command = commandNode.UnwrapValue(ObtainMode.Never, this, null);

					if(command != null && command is IDisposable)
						((IDisposable)command).Dispose();
				}
			}

			//调用基类同名方法
			base.OnStop();
		}
		#endregion
	}
}
