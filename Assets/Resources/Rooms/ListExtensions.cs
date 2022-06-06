using System.Collections.Generic;

namespace Resources.Rooms
{
	public static class ListExtensions
	{
		private static System.Random rng = new System.Random();  

		public static void Shuffle<T>(this IList<T> list)  
		{  
			var n = list.Count;  
			while (n > 1)
			{  
				n--;  
				var k = rng.Next(n + 1);  
				(list[k], list[n]) = (list[n], list[k]);
			}  
		}
	}
}