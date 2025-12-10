using System.Text;

namespace adventofcode_2025
{
    internal class Day7 : IDay
    {
        public void Execute(string input, out string star1, out string star2)
        {
            string[] rawRows = input.Split(Environment.NewLine);
            StringBuilder[] rows = new StringBuilder[rawRows.Length];
            for (int i = 0; i < rows.Length; i++)
            {
                rows[i] = new StringBuilder(rawRows[i]);
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
                        }
                    }
                }
            }

            star1 = "invalid";
            star2 = "invalid";
        }
    }
}
