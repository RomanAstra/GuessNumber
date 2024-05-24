using System;

namespace Helpers
{
    public readonly struct IntRange : IComparable<IntRange>
    {
        public readonly int From;
        public readonly int To;

        public IntRange(int from, int to)
        {
            From = from;
            To = to;
        }

        public bool Contains(int value)
        {
            return From <= value && value <= To;
        }

        public bool IsZero()
        {
            return From == To && From == 0;
        }

        public static IntRange operator +(IntRange a, IntRange b)
        {
            return new IntRange(a.From + b.From, a.To + b.To);
        }

        public static IntRange operator -(IntRange a, IntRange b)
        {
            return new IntRange(a.From - b.From, a.To - b.To);
        }

        public static IntRange operator +(IntRange a, int b)
        {
            return new IntRange(a.From + b, a.To + b);
        }

        public static IntRange operator -(IntRange a, int b)
        {
            return new IntRange(a.From - b, a.To - b);
        }

        public int CompareTo(IntRange other)
        {
            int fromComparison = From.CompareTo(other.From);
            if (fromComparison != 0) return fromComparison;
            return To.CompareTo(other.To);
        }
    }
}
