namespace adventofcode_2025
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            Day1(GetInput(1));

            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }


        static void Day1(string input)
        {
            var rows = input.Split(Environment.NewLine);

            int result = 0;
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
                    result++;
                    if (item[0] == 'L') toAdd2++;
                }

                //Console.WriteLine($"{preVal}, {item}, {term}, {dialValue}, {toAdd2}");
                result2 += toAdd2;
            }
            Console.WriteLine($"Day 1 star 1: {result}");
            Console.WriteLine($"Day 1 star 2: {result2}");
        }

        static string GetInput(int dayIndex)
        {
            string path = "input/day " + dayIndex + ".txt";
            return File.ReadAllText(path);
        }
    }
}
