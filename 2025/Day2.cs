namespace adventofcode_2025
{
    internal class Day2 : IDay
    {
        public void Execute(string input, out string star1, out string star2)
        {
            ulong result1 = 0, result2 = 0;

            string[] rawRanges = input.Split(',');
            foreach (string item in rawRanges)
            {
                var raw = item.Split('-');
                var low = ulong.Parse(raw[0]);
                var high = ulong.Parse(raw[1]);

                for (ulong i = low; i <= high; i++)
                {
                    var str = i.ToString();
                    int l = str.Length;
                    if (l % 2 != 0) continue;
                    int l2 = l / 2;
                    string firstHalf = str[0..l2];
                    string secondHalf = str[l2..];
                    //Console.WriteLine($"id: {str}, first: {firstHalf}, second: {secondHalf}");
                    if (firstHalf == secondHalf)
                    {
                        //Console.WriteLine($"{str} is invalid");
                        result1 += i;
                    }
                }
            }

            star1 = result1.ToString();
            star2 = "Undefined";// result2.ToString();
        }
    }
}
