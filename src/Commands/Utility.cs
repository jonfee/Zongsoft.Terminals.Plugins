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
using System.Collections.Generic;

using Zongsoft.Resources;
using Zongsoft.Terminals;

namespace Zongsoft.Plugins.Commands
{
	internal static class Utility
	{
		public static void WritePluginNode(ITerminal terminal, PluginTreeNode node, ObtainMode obtainMode, int maxDepth)
		{
			if(node == null)
				return;

			terminal.Write(TerminalColor.DarkYellow, "[{0}]", node.NodeType);
			terminal.WriteLine(node.FullPath);
			terminal.Write(TerminalColor.DarkYellow, "Plugin File: ");

			if(node.Plugin == null)
				terminal.WriteLine(TerminalColor.Red, "N/A");
			else
				terminal.WriteLine(node.Plugin.FilePath);

			terminal.Write(TerminalColor.DarkYellow, "Node Properties: ");
			terminal.WriteLine(node.Properties.Count);

			if(node.Properties.Count > 0)
			{
				terminal.WriteLine(TerminalColor.Gray, "{");

				foreach(PluginExtendedProperty property in node.Properties)
				{
					terminal.Write(TerminalColor.DarkYellow, "\t" + property.Name);
					terminal.Write(" = ");
					terminal.Write(property.RawValue);

					if(property.Value != null)
					{
						terminal.Write(TerminalColor.DarkGray, " [");
						terminal.Write(TerminalColor.Blue, property.Value.GetType().FullName);
						terminal.Write(TerminalColor.DarkGray, "]");
					}

					terminal.WriteLine();
				}

				terminal.WriteLine(TerminalColor.Gray, "}");
			}

			terminal.WriteLine(TerminalColor.DarkYellow, "Children: {0}", node.Children.Count);
			if(node.Children.Count > 0)
			{
				terminal.WriteLine();

				foreach(var child in node.Children)
				{
					terminal.WriteLine(child);
				}
			}

			object value = node.UnwrapValue(obtainMode, null);
			if(value != null)
			{
				terminal.WriteLine();
				Zongsoft.Runtime.Serialization.Serializer.Text.Serialize(terminal.OutputStream, value);
			}
		}
	}
}
