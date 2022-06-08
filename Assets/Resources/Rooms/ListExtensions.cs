using System;
using System.Collections.Generic;
using System.Linq;

namespace Resources.Rooms
{
	public static class ListExtensions
	{
		private static readonly Random rnd = new Random();

		public static void Shuffle<T>(this IList<T> list)
		{
			var n = list.Count;
			while (n > 1)
			{
				n--;
				var k = rnd.Next(n + 1);
				(list[k], list[n]) = (list[n], list[k]);
			}
		}

		public static (T item, int chance) SelectItem<T>(this IList<(T item, int chance)> items)
		{
			var poolSize = items.Sum(t => t.chance);

			var randomNumber = rnd.Next(0, poolSize) + 1;

			var accumulatedProbability = 0;
			foreach (var t in items)
			{
				accumulatedProbability += t.chance;
				if (randomNumber <= accumulatedProbability)
					return t;
			}

			throw new InvalidOperationException();
		}
	}
}