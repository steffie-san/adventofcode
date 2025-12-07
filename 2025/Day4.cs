using System.Text;

namespace adventofcode_2025
{
    internal class Day4 : IDay
    {
        public void Execute(string input, out string star1, out string star2)
        {
            string[] rows = input.Split(Environment.NewLine);

            int result1 = 0;
            for (int y = 0; y < rows.Length; y++)
            {
                for (int x = 0; x < rows[y].Length; x++)
                {
                    if (rows[y][x] != '@') continue;

                    int neighbours = CountNeighbours(rows, x, y);
                    //Console.WriteLine($"({x}, {y}) has {neighbours} neighbours");

                    if (neighbours < 4)
                    {
                        result1++;
                    }
                }
            }

            star1 = result1.ToString();

            int result2 = 0;
            List<(int, int)> toRemove = new();
            StringBuilder mutator = new(rows[0].Length);
            do
            {
                toRemove.Clear();

                for (int y = 0; y < rows.Length; y++)
                {
                    for (int x = 0; x < rows[y].Length; x++)
                    {
                        if (rows[y][x] != '@') continue;

                        int neighbours = CountNeighbours(rows, x, y);

                        if (neighbours < 4) toRemove.Add((x, y));
                    }
                }

                result2 += toRemove.Count;
                for (int i = 0; i < toRemove.Count; i++)
                {
                    var (x, y) = toRemove[i];
                    mutator.Append(rows[y]);
                    mutator[x] = '.';
                    rows[y] = mutator.ToString();
                    mutator.Clear();
                }
                //Console.WriteLine($"Removed {toRemove.Count} rolls");
            }
            while (toRemove.Count > 0);

            

            star2 = result2.ToString();
        }

        int CountNeighbours(string[] rows, int x, int y)
        {
            int count = 0;
            for (int i = 0; i < 3; i++)
            {
                int curY = y + i - 1;
                if (curY < 0 || curY >= rows.Length) continue;
                var curRow = rows[curY];
                for (int ii = 0; ii < 3; ii++)
                {
                    if (i == 1 && ii == 1) continue; //Skip self (0,0)

                    int curX = x + ii - 1;
                    if (curX < 0 || curX >= curRow.Length) continue;
                    if (curRow[curX] == '@') count++;
                }
            }
            return count;
        }
    }
}
