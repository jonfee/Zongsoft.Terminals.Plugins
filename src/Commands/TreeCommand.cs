﻿/*
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

using Zongsoft.Services;
using Zongsoft.Resources;
using Zongsoft.Terminals;

namespace Zongsoft.Plugins.Commands
{
	[DisplayName("${Text.Commands.TreeCommand}")]
	[Description("${Text.Commands.TreeCommand.Description}")]
	[CommandOption("maxDepth", Type = typeof(int), DefaultValue = 3, Description = "${Text.Commands.TreeCommand.Options.MaxDepth}")]
	[CommandOption("fullPath", Description = "${Text.Commands.TreeCommand.Options.FullPath}")]
	public class TreeCommand : CommandBase<TerminalCommandContext>
	{
		#region 成员字段
		private PluginContext _pluginContext;
		#endregion

		#region 构造函数
		public TreeCommand(PluginContext pluginContext) : this("Tree", pluginContext)
		{
		}

		public TreeCommand(string name, PluginContext pluginContext) : base(name)
		{
			if(pluginContext == null)
				throw new ArgumentNullException("pluginContext");

			_pluginContext = pluginContext;
		}
		#endregion

		#region 重写方法
		protected override void OnExecute(TerminalCommandContext context)
		{
			if(context.Arguments.Length > 1)
				throw new CommandException("The arguments too many.");

			var node = _pluginContext.PluginTree.RootNode;

			if(context.Arguments.Length == 1)
			{
				node = _pluginContext.PluginTree.Find(context.Arguments[0]);

				if(node == null)
				{
					context.Terminal.WriteLine(TerminalColor.DarkRed, ResourceUtility.GetString("${Text.Commands.InvalidPluginPath}", context.Arguments[0]));
					return;
				}
			}

			this.WritePluginTree(context.Terminal, node, (int)context.Options["maxDepth"], 0, 0, context.Options.Contains("fullPath"));
		}
		#endregion

		public void WritePluginTree(ITerminal terminal, PluginTreeNode node, int maxDepth, int depth, int index, bool isFullPath)
		{
			if(node == null)
				return;

			var indent = depth > 0 ? new string('\t', depth) : string.Empty;

			if(depth > 0)
			{
				terminal.Write(TerminalColor.DarkMagenta, indent + "[{0}.{1}] ", depth, index);
			}

			terminal.Write(isFullPath ? node.FullPath : node.Name);
			terminal.Write(TerminalColor.DarkGray, " [{0}]", node.NodeType);

			if(node.Plugin == null)
				terminal.WriteLine();
			else
			{
				terminal.Write(TerminalColor.DarkGreen, "@");
				terminal.WriteLine(TerminalColor.DarkGray, node.Plugin.Name);
			}

			var target = node.UnwrapValue(ObtainMode.Never, null);
			if(target != null)
				terminal.WriteLine(TerminalColor.DarkYellow, "{0}{1}", indent, target.GetType().FullName);

			if(maxDepth > 0 && depth >= maxDepth)
				return;

			for(int i = 0; i < node.Children.Count; i++)
			{
				WritePluginTree(terminal, node.Children[i], maxDepth, depth + 1, i + 1, isFullPath);
			}
		}
	}
}
