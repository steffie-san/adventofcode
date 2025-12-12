using System.Text;

namespace adventofcode_2025
{
    internal class Day7 : IDay
    {
        Dictionary<(int, int), int> splitterHitCount = new();
        public void Execute(string input, out string star1, out string star2)
        {
            int splitCounter = 0;
            string[] rawRows = input.Split(Environment.NewLine);
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

            for (int i = 0; i < rawRows.Length; i++)
            {
                rows[i].Clear();
                rows[i].Append(rawRows[i]);
                //Console.WriteLine(rows[i]);
            }
            int realities = TraverseRecursively(rows, rawRows[0].IndexOf('S'), 1);

            star2 = realities.ToString();
        }

        int TraverseRecursively(StringBuilder[] rows, int x, int y)
        {
            //Console.WriteLine($"start traversal at ({x},{y})");
            int result = 0;
            while (y < rows.Length)
            {
                char c = rows[y][x];
                if (c == '^')
                {
                    if (splitterHitCount.TryGetValue((x, y), out int count)) count++;
                    else count = 1;
                    splitterHitCount[(x, y)] = count;

                    //Console.WriteLine($"Hit splitter at ({x},{y}) (hitcount: {count})");
                    result += TraverseRecursively(rows, x - 1, y);
                    result += TraverseRecursively(rows, x + 1, y);
                    return result;
                }
                else if (c == '|' || c == '.')
                {
                    //Console.WriteLine($"({x},{y}) is traversable ({c})");
                    rows[y][x] = '|';
                    y++;
                }
                //else Console.WriteLine($"unknown char {c} at ({x},{y})");
            }
            Console.WriteLine("Line terminated");
            return 1;
        }
    }
}
