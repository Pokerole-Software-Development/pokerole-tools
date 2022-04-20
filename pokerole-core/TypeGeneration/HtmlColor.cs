/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Pokerole.Core.TypeGeneration
{
	//derived from https://stackoverflow.com/a/4322461/1366594
	public class HtmlColor
	{
		private Color c;
		public HtmlColor() { }

		public HtmlColor(Color c)
		{
			this.c = c;
		}

		private Color ToColor()
		{
			return c;
		}
		public String Value
		{
			get => ColorTranslator.ToHtml(c);
			set
			{
				try
				{
					c = ColorTranslator.FromHtml(value);
				}
				catch (ArgumentException)
				{
					c = Color.Black;
				}
			}
		}

		public static implicit operator Color(HtmlColor x)
		{
			return x.ToColor();
		}

		public static implicit operator HtmlColor(Color c)
		{
			return new HtmlColor(c);
		}
	}
}
