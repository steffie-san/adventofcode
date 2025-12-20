namespace adventofcode_2025
{
    internal class Day10 : IDay
    {
        public void Execute(string input, out string star1, out string star2)
        {
            var rows = input.Split(Environment.NewLine);

            int totalMoves = 0;
            foreach (var rowIt in rows)
            {
                //Parse string
                var parts = rowIt.Split(' ');
                var finalString = parts[^1];

                byte len;
                var targetLampState = ParseLampString(parts[0], out len);
                uint[] buttonWirings = new uint[parts.Length - 2];
                for (int i = 0; i < buttonWirings.Length; i++) buttonWirings[i] = ParseButtonString(parts[i + 1]);

                //Print parsed result to verify parsing
                //string toPrint = string.Empty;
                //toPrint += LampStateToString(targetLampState) + ' ';
                //foreach (var buttonWiring in buttonWirings) toPrint += ButtonWiringToString(buttonWiring) + ' ';
                //toPrint += finalString;
                //Log(toPrint);

                int moves = FindBestPath(-1, 0, targetLampState, buttonWirings, 0, int.MaxValue);
                if (moves < int.MaxValue)
                {
                    totalMoves += moves;
                    //Log($"{moves} moves for row {rowIt}");
                }
                else Log("ERROR: Could not reach target state for " + rowIt);
            }

            star1 = totalMoves.ToString();
            star2 = "invalid";
        }

        private int FindBestPath(int lastPressedButton, uint currentState, uint targetState, uint[] allButtons, int count, int bestSoFar)
        {
            count++;
            if (count >= bestSoFar) return int.MaxValue;
            for (int i = lastPressedButton + 1; i < allButtons.Length; i++)
            {
                //Log("Apply " + i);
                currentState ^= allButtons[i];
                if (currentState == targetState)
                {
                    //Log("Reached target");
                    return count;
                }
                else
                {
                    var deeperValue = FindBestPath(i, currentState, targetState, allButtons, count, bestSoFar);
                    //Log($"Deeper down best is {deeperValue}, current best: {bestSoFar}");
                    if (deeperValue < bestSoFar) bestSoFar = deeperValue;
                }
                currentState ^= allButtons[i];
            }
            return bestSoFar;
        }

        public static uint ParseButtonString(string buttonString)
        {
            uint result = 0;

            var values = buttonString.Substring(1, buttonString.Length - 2).Split(',');
            foreach (var item in values)
            {
                var value = int.Parse(item);
                result |= (uint)(1 << value);
            }

            return result;
        }

        public static uint ParseLampString(string lampString, out byte length)
        {
            length = (byte)(lampString.Length - 2);
            uint result = 0;
            uint checker = 1;
            for (int i = 0; i < length; i++)
            {
                if (lampString[i + 1] == '#') result |= checker;
                checker <<= 1;
            }
            return result;
        }

        private uint OnButtonPressed(uint lampState, uint button) => lampState ^ button;

        public byte Distance(uint state, uint targetState, byte length)
        {
            byte result = 0;
            uint checker = 1;
            for (byte i = 0; i < length; i++)
            {
                if ((state & checker) != (targetState & checker)) result++;
                checker <<= 1;
            }
            return result;
        }

        private string StateToString(uint state, byte length)
        {
            string result = string.Empty;
            uint checker = 1;
            for (int i = 0; i < length; i++)
            {
                result += (state & checker) == checker ? '#' : '.';
                checker <<= 1;
            }
            return result;
        }

        private void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
