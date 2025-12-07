namespace adventofcode_2025
{
    internal class Day5 : IDay
    {
        public void Execute(string input, out string star1, out string star2)
        {
            string[] sections = input.Split(Environment.NewLine + Environment.NewLine);
            string[] ranges = sections[0].Split(Environment.NewLine);
            string[] ids = sections[1].Split(Environment.NewLine);

            foreach (var item in ranges) Console.WriteLine(item);
            Console.WriteLine();
            foreach (var item in ids) Console.WriteLine(item);


            star1 = "invalid";
            star2 = "invalid";
        }
    }
}
