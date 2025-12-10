namespace adventofcode_2025
{
    internal class Day6 : IDay
    {
        public void Execute(string input, out string star1, out string star2)
        {
            string[] rows = input.Split(Environment.NewLine);
            string[] operators = rows[rows.Length - 1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            long[] sums = new long[operators.Length];

            for (int i = 0; i < sums.Length; i++) sums[i] = operators[i] == "+" ? 0 : 1;

            for (int i = 0; i < rows.Length - 1; i++)
            {
                var operands = rows[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                for (int ii = 0; ii < operands.Length; ii++)
                {
                    var operand = long.Parse(operands[ii]);
                    if (operators[ii] == "+") sums[ii] += operand;
                    else sums[ii] *= operand;
                }
            }
            long totalSum = 0;
            foreach (var item in sums) totalSum += item;

            star1 = totalSum.ToString();

            char op = default;
            int resultIndex = -1;
            for (int i = 0; i < sums.Length; i++) sums[i] = operators[i] == "+" ? 0 : 1;
            for (int i = 0; i < rows[0].Length; i++)
            {
                var newOp = rows[rows.Length - 1][i];
                if (newOp != ' ')
                {
                    op = newOp;
                    resultIndex++;
                }

                string numberString = "";
                for (int ii = 0; ii < rows.Length - 1; ii++) numberString += rows[ii][i];
                if (string.IsNullOrWhiteSpace(numberString)) continue;

                //Console.WriteLine(numberString);
                int number = int.Parse(numberString);
                if (op == '+') sums[resultIndex] += number;
                else sums[resultIndex] *= number;
            }


            totalSum = 0;
            foreach (var item in sums)
            {
                //Console.WriteLine(item);
                totalSum += item;
            }

            star2 = totalSum.ToString();
        }
    }
}
