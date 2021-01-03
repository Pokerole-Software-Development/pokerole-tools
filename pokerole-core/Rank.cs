using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Pokerole.Core
{
	public enum Rank
	{
		//standard rankings
		Starter,
		Beginner,
		Amateur,
		Ace,
		Pro,
		Master,
		Champion,
		//Legendary rankings
		Hero,
		Guardian,
		DemiGod,
		God,
		Firstborn,
		OriginalOne
	}
	public static class RankExtensions
	{
		public static bool IsLegendary(this Rank rank)
		{
			return rank switch
			{
				Rank.Starter or Rank.Beginner or Rank.Amateur or Rank.Ace or Rank.Pro or Rank.Master or
					Rank.Champion => false,
				Rank.Hero or Rank.Guardian or Rank.DemiGod or Rank.God or Rank.Firstborn or Rank.OriginalOne => true,
				_ => throw new InvalidEnumArgumentException(nameof(rank), (int)rank, typeof(Rank)),
			};
		}
	}
}
