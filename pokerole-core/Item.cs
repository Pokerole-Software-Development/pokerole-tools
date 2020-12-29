using System;
using System.Collections.Generic;
using System.Text;

namespace Pokerole.Core
{
	public class Item : IDataBackedItem<Item>
	{
		public int ItemId => throw new NotImplementedException();

		public IDataBacking<Item> GetDataBacking() => throw new NotImplementedException();
	}
}
