/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Pokerole.Core
{
	//for quick reference
	//Nature:Confidence
	/*
	Adamant:4
	Bashful:6
	Bold:9
	Brave:9
	Calm:8
	Careful:5
	Docile:7
	Gentle:10
	Hardy:9
	Hasty:7
	Impish:7
	Jolly:10
	Lax:8
	Lonely:5
	Mild:8
	Modest:10
	Naive:7
	Naughty:6
	Quiet:5
	Quirky:9
	Rash:6
	Relaxed:8
	Sassy:7
	Serious:4
	Timid:4
	*/
	public enum Nature
	{
		Adamant,
		Bashful,
		Bold,
		Brave,
		Calm,
		Careful,
		Docile,
		Gentle,
		Hardy,
		Hasty,
		Impish,
		Jolly,
		Lax,
		Lonely,
		Mild,
		Modest,
		Naive,
		Naughty,
		Quiet,
		Quirky,
		Rash,
		Relaxed,
		Sassy,
		Serious,
		Timid,
	}
	public static class NatureExtensions
	{
		public static int GetBaseConfidence(this Nature nature)
		{
			return nature switch
			{
				Nature.Adamant or Nature.Serious or Nature.Timid => 4,
				Nature.Careful or Nature.Lonely or Nature.Quiet => 5,
				Nature.Bashful or Nature.Naughty or Nature.Rash => 6,
				Nature.Docile or Nature.Hasty or Nature.Impish or Nature.Naive or Nature.Sassy => 7,
				Nature.Calm or Nature.Lax or Nature.Mild or Nature.Relaxed => 8,
				Nature.Bold or Nature.Brave or Nature.Hardy or Nature.Quirky => 9,
				Nature.Gentle or Nature.Jolly or Nature.Modest => 10,
				_ => throw new InvalidEnumArgumentException(nameof(nature), (int)nature, typeof(Nature)),
			};
		}
	}
}