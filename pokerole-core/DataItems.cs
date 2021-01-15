using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Pokerole.Core
{
	public abstract record BaseDataItem
	{
		private readonly DataId dataId;
		public DataId DataId => dataId;
		protected BaseDataItem(DataId id)
		{
			dataId = id;
		}
	}
	public interface IEffect { }
	public class ImageRef { }
	public record Height { }
	public record Weight { }
	public record ItemReference<T> where T : BaseDataItem {
		public ItemReference(DataId id)
		{
			DataId = id;
		}

		public DataId DataId { get; }
	}
	class DataItems
	{
	}
}
