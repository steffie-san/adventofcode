using System.Text;

namespace adventofcode_2025
{
    internal class Day7 : IDay
    {
        class Splitter
        {
            public Coordinate Coordinate { get; private set; }

            public long realityCount;

            public Splitter(Coordinate coordinate, long realityCount)
            {
                Coordinate = coordinate;
                this.realityCount = realityCount;
            }
        }

        struct Coordinate
        {
            public int x;
            public int y;

            public Coordinate(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public override bool Equals(object? obj)
            {
                return obj is Coordinate coordinate &&
                       x == coordinate.x &&
                       y == coordinate.y;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(x, y);
            }
        }

        string[] rows;
        Dictionary<Coordinate, Splitter> calculatedSplitters = new();
        long result2 = 0;

        public void Execute(string input, out string star1, out string star2)
        {
            int splitCounter = 0;
            string[] rawRows = input.Split(Environment.NewLine);
            this.rows = rawRows;
            StringBuilder[] rows = new StringBuilder[rawRows.Length];
            rows[0] = new StringBuilder(rawRows[0]);
            for (int i = 0; i < rows.Length - 1; i++)
            {
                rows[i + 1] = new StringBuilder(rawRows[i]);

                int l = rows[i].Length;
                for (int ii = 0; ii < l; ii++)
                {
                    var rowIt = rows[i];
                    var nextRow = rows[i + 1];
                    char c = rowIt[ii];
                    bool propagate = c == 'S' || c == '|';
                    if (propagate)
                    {
                        var next = nextRow[ii];
                        if (next == '.') nextRow[ii] = '|';
                        else if (next == '^')
                        {
                            nextRow[ii - 1] = '|';
                            nextRow[ii + 1] = '|';
                            splitCounter++;
                        }
                    }
                }
            }

            star1 = splitCounter.ToString();

            int x = this.rows[0].IndexOf('S');
            int y = 1;
            while (this.rows[y][x] != '^') y++;
            Coordinate coordinate = new(x, y);
            result2 = CalculateSplitter(coordinate).realityCount;

            star2 = result2.ToString();
        }

        private Splitter CalculateSplitter(Coordinate coordinate)
        {
            long splitterResult = 0;

            splitterResult += CalculateSubtree(new Coordinate(coordinate.x - 1, coordinate.y));
            splitterResult += CalculateSubtree(new Coordinate(coordinate.x + 1, coordinate.y));

            return new Splitter(coordinate, splitterResult);
        }

        private long CalculateSubtree(Coordinate coordinate)
        {
            long splitterResult = 0;

            var x = coordinate.x;
            var y = coordinate.y;

            while (y < rows.Length)
            {
                var current = new Coordinate(x, y);

                char c = rows[y][x];
                if (c == '^')
                {
                    Splitter hitSplitter;
                    if (!calculatedSplitters.TryGetValue(current, out hitSplitter))
                    {
                        hitSplitter = CalculateSplitter(current);
                        calculatedSplitters[current] = hitSplitter;
                    }
                    splitterResult += hitSplitter.realityCount;
                    break;
                }
                else if (c == '.')
                {
                    y++;
                    if (y == rows.Length)
                    {
                        splitterResult++;
                        break;
                    }
                }
            }
            return splitterResult;
        }
    }
}
