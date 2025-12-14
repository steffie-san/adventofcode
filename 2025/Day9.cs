namespace adventofcode_2025
{
    internal class Day9 : IDay
    {
        struct Vector2
        {
            public int x;
            public int y;

            public Vector2(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public override bool Equals(object? obj)
            {
                return obj is Vector2 vector &&
                       x == vector.x &&
                       y == vector.y;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(x, y);
            }

            public override string ToString()
            {
                return $"({x},{y})";
            }

            public static Vector2 ParseFromString(string str)
            {
                string[] parts = str.Split(',');

                int x = int.Parse(parts[0]);
                int y = int.Parse(parts[1]);

                return new Vector2(x, y);
            }
        }

        public void Execute(string input, out string star1, out string star2)
        {
            star1 = "invalid";
            star2 = "invalid";
        }
    }
}
