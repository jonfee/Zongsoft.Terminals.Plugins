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
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;

using Zongsoft.Services;
using Zongsoft.Resources;
using Zongsoft.Terminals;

namespace Zongsoft.Plugins.Commands
{
	[DisplayName("${Text.Commands.ListCommand}")]
	[Description("${Text.Commands.ListCommand.Description}")]
	public class ListCommand : CommandBase<TerminalCommandContext>
	{
		#region 成员字段
		private PluginContext _pluginContext;
		#endregion

		#region 构造函数
		public ListCommand(PluginContext pluginContext) : this("List", pluginContext)
		{
		}

		public ListCommand(string name, PluginContext pluginContext) : base(name)
		{
			if(pluginContext == null)
				throw new ArgumentNullException("pluginContext");

			_pluginContext = pluginContext;
		}
		#endregion

		#region 重写方法
		protected override void OnExecute(TerminalCommandContext context)
		{
			int index = 0;

			foreach(var plugin in _pluginContext.Plugins)
			{
				this.WritePlugin(context.Terminal, plugin, 0, index++);
			}
		}
		#endregion

		#region 私有方法
		private void WritePlugin(ITerminal terminal, Plugin plugin, int depth, int index)
		{
			if(plugin == null)
				return;

			var indent = depth > 0 ? new string('\t', depth) : string.Empty;

			terminal.Write(TerminalColor.DarkMagenta, indent + "[{0}] ", index + 1);
			terminal.Write(plugin.Name);

			if(plugin.IsMaster)
				terminal.WriteLine(TerminalColor.DarkCyan, " (Master)");
			else
				terminal.WriteLine();

			terminal.Write(indent);

			var directoryName = this.GetCurrentDirectoryName(plugin.FilePath);

			if(!string.IsNullOrEmpty(directoryName))
			{
				terminal.Write(TerminalColor.DarkGreen, "{0}", directoryName);
				terminal.Write("/");
			}

			terminal.Write(TerminalColor.DarkYellow, Path.GetFileName(plugin.FilePath));

			if(System.IO.File.Exists(plugin.FilePath))
			{
				var fileInfo = new System.IO.FileInfo(plugin.FilePath);
				terminal.WriteLine(TerminalColor.DarkGray, " [{0}]", fileInfo.LastWriteTime);
			}
			else
			{
				terminal.WriteLine();
			}

			if(plugin.Children.Count > 0)
			{
				for(int i = 0; i < plugin.Children.Count; i++)
				{
					this.WritePlugin(terminal, plugin.Children[i], depth + 1, i);
				}

				terminal.WriteLine();
			}
		}

		private string GetCurrentDirectoryName(string filePath)
		{
			if(string.IsNullOrWhiteSpace(filePath))
				return string.Empty;

			var directoryPath = Path.GetDirectoryName(filePath);
			var index = directoryPath.LastIndexOf(Path.DirectorySeparatorChar);

			if(index > 0 && index < directoryPath.Length - 1)
				return directoryPath.Substring(index + 1);

			return string.Empty;
		}
		#endregion
	}
}
