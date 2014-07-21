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

using Zongsoft.Services;
using Zongsoft.Resources;
using Zongsoft.Terminals;

namespace Zongsoft.Plugins.Commands
{
	[DisplayName("${Text.Commands.FindCommand}")]
	[Description("${Text.Commands.FindCommand.Description}")]
	[CommandOption("obtain", Type = typeof(ObtainMode), DefaultValue = ObtainMode.Never, Description = "${Text.Commands.FindCommand.Options.ObtainMode}")]
	[CommandOption("maxDepth", Type = typeof(int), DefaultValue = 3, Description = "${Text.Commands.FindCommand.Options.MaxDepth}")]
	public class FindCommand : CommandBase<TerminalCommandContext>
	{
		#region 成员字段
		private PluginContext _pluginContext;
		#endregion

		#region 构造函数
		public FindCommand(PluginContext pluginContext) : this("Find", pluginContext)
		{
		}

		public FindCommand(string name, PluginContext pluginContext) : base(name)
		{
			if(pluginContext == null)
				throw new ArgumentNullException("pluginContext");

			_pluginContext = pluginContext;
		}
		#endregion

		#region 重写方法
		protected override void Run(TerminalCommandContext context)
		{
			if(context.Arguments.Length < 1 || string.IsNullOrEmpty(context.Arguments[0].ToString()))
			{
				context.Terminal.Error.WriteLine(ResourceUtility.GetString("${Text.Commands.MissingPluginPath}"));
				return;
			}

			var node = _pluginContext.PluginTree.Find(context.Arguments[0].ToString());

			if(node == null)
			{
				context.Terminal.Error.WriteLine(ResourceUtility.GetString("${Text.Commands.InvalidPluginPath}", context.Arguments[0]));
				return;
			}

			Utility.WritePluginNode(context.Terminal, node,
						(ObtainMode)context.Options["obtain"],
						(int)context.Options["maxDepth"]);
		}
		#endregion
	}
}
