/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
using System;
using System.Collections.Generic;
using System.Text;

namespace Pokerole.Core
{
	public readonly struct MegaEvolutionEntry : IEquatable<MegaEvolutionEntry>
	{
		public MegaEvolutionEntry(int dbId, Guid crossBackingId, IDataPointer<Item> item,
			IDataPointer<DexEntry> targetEvolution)
		{
			DbId = dbId;
			CrossBackingId = crossBackingId;
			Item = item ?? throw new ArgumentNullException(nameof(item));
			TargetEvolution = targetEvolution ?? throw new ArgumentNullException(nameof(targetEvolution));
		}

		public int DbId { get; }
		public Guid CrossBackingId { get; }
		public IDataPointer<Item> Item { get; }
		public IDataPointer<DexEntry> TargetEvolution { get; }

		public override bool Equals(object? obj) => obj is MegaEvolutionEntry entry && Equals(entry);
		public bool Equals(MegaEvolutionEntry other) => DbId == other.DbId &&
			CrossBackingId.Equals(other.CrossBackingId) &&
			EqualityComparer<IDataPointer<Item>>.Default.Equals(Item, other.Item) &&
			EqualityComparer<IDataPointer<DexEntry>>.Default.Equals(TargetEvolution, other.TargetEvolution);
		public override int GetHashCode() => HashCode.Combine(DbId, CrossBackingId, Item, TargetEvolution);
	}
	public class DexEntry : IDataBackedItem<DexEntry>
	{
		private readonly IDataBacking<DexEntry> backing;

		public DexEntry(IDataBacking<DexEntry> backing)
		{
			ArgCheck.NotNull(backing, nameof(backing));
			this.backing = backing;
		}
		public int ItemId => backing.ItemId;

		public IDataBacking<DexEntry> GetDataBacking() => backing;
		//Note: Using CallerMemberNameAttribute to avoid duplicate nameof() "calls"

	}
}
