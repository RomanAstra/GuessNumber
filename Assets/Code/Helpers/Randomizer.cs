using System;
using System.Collections.Generic;

namespace Helpers
{
    public sealed class Randomizer
    {
        private readonly Random _random = new();

        public int GetRandom(int min, int max)
        {
            return _random.Next(min, max);
        }

        public int GetRandom(IntRange range)
        {
            return GetRandom(range.From, range.To);
        }

        public T GetRandom<T>(IReadOnlyList<T> list)
        {
            return list[GetRandom(0, list.Count)];
        }

        public List<T> GetRandomized<T>(IReadOnlyList<T> list)
        {
            List<T> newList = new List<T>(list.Count);

            for (var i = 0; i < list.Count; i++)
            {
                newList.Insert(GetRandom(0, i + 1), list[i]);
            }

            return newList;
        }
        
        public void Shuffle<T>(IList<T> list)  
        {  
            int n = list.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int r = _random.Next(i + 1);
                (list[r], list[i]) = (list[i], list[r]);
            }
        }
    }
}
