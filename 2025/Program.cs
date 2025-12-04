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
            int dialValue = 50;
            foreach (var item in rows)
            {
                int preVal = dialValue;
                var term = int.Parse(item[1..]);
                if (item[0] == 'L') term *= -1;

                dialValue += term;

                int removed = 0;
                int added = 0;
                while (dialValue < 0)
                {
                    added++;
                    dialValue += 100;
                }
                while (dialValue > 99)
                {
                    removed++;
                    dialValue -= 100;
                }
                if (dialValue == 0) result++;

                //Console.WriteLine($"{preVal}, {item}, {term}, {dialValue} ({removed}, {added})");
            }
            Console.WriteLine($"Day 1 star 1: {result}");
        }

        static string GetInput(int dayIndex)
        {
            string path = "input/day " + dayIndex + ".txt";
            return File.ReadAllText(path);
        }
    }
}
