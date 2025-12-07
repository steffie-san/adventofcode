namespace adventofcode_2025
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            IDay[] days =
            {
                new Day1(),
                new Day2(),
            };

            bool useExampleInput = false;
            int dayIndex = 1;

            string result1, result2;
            days[dayIndex].Execute(GetInput(dayIndex + 1, useExampleInput), out result1, out result2);

            Console.WriteLine($"Day {dayIndex + 1}, star1: {result1}, star2: {result2}");

            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }

        static string GetInput(int dayIndex, bool useExampleInput)
        {
            string path = "input/day " + dayIndex;
            if (useExampleInput) path += ".example";
            return File.ReadAllText(path + ".txt");
        }
    }
}
