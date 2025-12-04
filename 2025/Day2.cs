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
                    var id = i.ToString();
                    int l = id.Length;
                    int l2 = l / 2;
                    bool valid = false;

                    Console.WriteLine($"ID: {id}");
                    for (int stepSize = 1; stepSize <= l2; stepSize++)
                    {
                        int steps = l / stepSize;
                        if (l % stepSize != 0) continue;
                        var seq = id[0..stepSize];

                        Console.WriteLine($"\tSequence: {seq}");

                        for (int step = 1; step < steps; step++)
                        {
                            var start = stepSize * step;
                            var end = stepSize * (step + 1);
                            var comparee = id[start..end];
                            Console.WriteLine($"\t\tComparing \"{seq}\" with \"{comparee}\" (range: {start}-{end})");
                            if (seq != comparee)
                            {
                                Console.WriteLine($"\t\tunequal, not invalid with stepsize {stepSize}");
                                valid = true;
                                break;
                            }
                            else Console.WriteLine($"\t\tequal, continue...");
                        }
                        if (!valid)
                        {
                            Console.WriteLine($"\t{id} invalid. Sequence: {seq}");
                            break;
                        }
                    }

                    if (!valid)
                    {
                        result2 += i;
                    }

                    if (l % 2 != 0) continue;
                    string firstHalf = id[0..l2];
                    string secondHalf = id[l2..];
                    //Console.WriteLine($"id: {str}, first: {firstHalf}, second: {secondHalf}");
                    if (firstHalf == secondHalf)
                    {
                        //Console.WriteLine($"{str} is invalid");
                        result1 += i;
                    }
                }
            }

            star1 = result1.ToString();
            star2 = result2.ToString();
        }
    }
}
