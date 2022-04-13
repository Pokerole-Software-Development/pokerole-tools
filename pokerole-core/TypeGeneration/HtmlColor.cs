using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Pokerole.Core.TypeGeneration
{
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
