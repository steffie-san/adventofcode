namespace adventofcode_2025
{
    internal class Day3 : IDay
    {
        public void Execute(string input, out string star1, out string star2)
        {
            var banks = input.Split(Environment.NewLine);

            star1 = CalculateJoltageSum(banks, 2);
            star2 = CalculateJoltageSum(banks, 12);
        }

        string CalculateJoltageSum(string[] banks, int batteryCount)
        {
            long result = 0;

            foreach (var bankIt in banks)
            {
                byte[] numbers = new byte[batteryCount];

                int nextStart = 0;
                for (int i = 0; i < batteryCount; i++)
                {
                    var itLength = bankIt.Length - batteryCount + i + 1;
                    for (int ii = nextStart; ii < itLength; ii++)
                    {
                        byte current = (byte)(bankIt[ii] - '0');
                        if (current > numbers[i])
                        {
                            numbers[i] = current;
                            nextStart = ii + 1;
                        }
                    }
                }
                long bankJoltage = 0;
                for (int i = 0; i < numbers.Length; i++)
                {
                    long joltage = numbers[i] * (long)Math.Pow(10, (numbers.Length - i - 1));
                    bankJoltage += joltage;
                    result += joltage;
                }
                //Console.WriteLine(bankJoltage);
            }
            return result.ToString();
        }
    }
}
