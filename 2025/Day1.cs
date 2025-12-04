namespace adventofcode_2025
{
    internal class Day1 : IDay
    {


        public void Execute(string input, out string star1, out string star2)
        {
            var rows = input.Split(Environment.NewLine);

            int result1 = 0;
            int result2 = 0;
            int dialValue = 50;
            foreach (var item in rows)
            {
                int toAdd2 = 0;
                int preVal = dialValue;
                var term = int.Parse(item[1..]);
                if (item[0] == 'L')
                {
                    term *= -1;
                    if (preVal == 0) toAdd2--;
                }

                dialValue += term;

                while (dialValue < 0)
                {
                    toAdd2++;
                    dialValue += 100;
                }
                while (dialValue > 99)
                {
                    toAdd2++;
                    dialValue -= 100;
                }
                if (dialValue == 0)
                {
                    result1++;
                    if (item[0] == 'L') toAdd2++;
                }

                //Console.WriteLine($"{preVal}, {item}, {term}, {dialValue}, {toAdd2}");
                result2 += toAdd2;
            }

            star1 = result1.ToString();
            star2 = result2.ToString();
        }
    }
}
