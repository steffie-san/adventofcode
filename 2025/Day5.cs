namespace adventofcode_2025
{
    internal class Day5 : IDay
    {
        private struct Range
        {
            public long Min { get; set; }
            public long Max { get; set; }

            public Range(long min, long max)
            {
                Min = min;
                Max = max;
            }

            public bool CompletelyContains(Range other)
            {
                return other.Min >= Min && other.Max <= Max;
            }

            public Range Merge(Range other)
            {
                return new Range(Math.Min(Min, other.Min), Math.Max(Max, other.Max));
            }
        }

        public void Execute(string input, out string star1, out string star2)
        {
            string[] sections = input.Split(Environment.NewLine + Environment.NewLine);
            string[] rawRanges = sections[0].Split(Environment.NewLine);
            string[] rawIDs = sections[1].Split(Environment.NewLine);

            List<Range> ranges = new(rawRanges.Length);

            foreach (var item in rawRanges)
            {
                string[] parts = item.Split('-');
                long min = long.Parse(parts[0]);
                long max = long.Parse(parts[1]);
                ranges.Add(new(min, max));
            }

            int result1 = 0;
            foreach (var rawID in rawIDs)
            {
                long id = long.Parse(rawID);

                bool inAnyRange = false;
                foreach (var item in ranges)
                {
                    if (id >= item.Min && id <= item.Max)
                    {
                        inAnyRange = true;
                        break;
                    }
                }

                if (inAnyRange) result1++;
            }

            star1 = result1.ToString();

            LinkedList<Range> mergedRanges = new();

            mergedRanges.AddLast(ranges[0]);

            for (int i = 0; i < ranges.Count; i++)
            {
                Range newRange = ranges[i];
                bool added = false;
                for (LinkedListNode<Range>? rangeIt = mergedRanges.First; rangeIt != null; rangeIt = rangeIt.Next)
                {
                    Range nextRange = rangeIt.Value;


                    if (newRange.Max < nextRange.Min) mergedRanges.AddBefore(rangeIt, newRange);
                    else if (newRange.Min > nextRange.Max) continue;
                    else
                    {
                        //Since the old one completely contains the new one, nothing happens
                        if (nextRange.CompletelyContains(newRange))
                        {
                            added = true;
                            break;
                        }
                        //If the new one completely contains the old one, we should remove the old one and see if the new one overlaps with the next
                        else if (newRange.CompletelyContains(nextRange)) mergedRanges.Remove(rangeIt);
                        else
                        {
                            Range merged = newRange.Merge(nextRange);
                            //We have partial overlap. If we grow above existing max, remove, merge, and process as a new one
                            if (newRange.Max > nextRange.Max)
                            {
                                mergedRanges.Remove(rangeIt);
                                newRange = merged;
                            }
                            else
                            {
                                //Just merge easily
                                rangeIt.Value = merged;
                                added = true;
                                break;
                            }
                        }
                    }
                }
                if (!added) mergedRanges.AddLast(newRange);
            }

            long count = 0;
            foreach (var item in mergedRanges)
            {
                Console.WriteLine($"{item.Min}-{item.Max}");
                count += (item.Max - item.Min) + 1;
            }


            star2 = count.ToString();
        }
    }
}
