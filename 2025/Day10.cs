
using MathNet.Numerics.LinearAlgebra;
using System.Collections.Concurrent;

namespace adventofcode_2025
{
    internal class Day10 : IDay
    {
        struct Joltage
        {
            public readonly int[] values;

            public int this[int index]
            {
                get => values[index];
                set => values[index] = value;
            }

            public int Length => values.Length;

            public Joltage(int[] values)
            {
                this.values = values;
            }

            public static Joltage Flat(int length) => new Joltage(new int[length]);
            public static Joltage FromButton(uint button, int length)
            {
                int[] values = new int[length];
                uint checker = 1;
                for (int i = 0; i < length; i++)
                {
                    if ((button & checker) == checker) values[i] = 1;
                    checker <<= 1;
                }
                return new Joltage(values);
            }

            public static Joltage operator +(Joltage left, Joltage right)
            {
                //Assume equal length
                var len = left.Length;
                int[] values = new int[len];
                for (int i = 0; i < len; i++) values[i] = left.values[i] + right.values[i];
                return new Joltage(values);
            }

            public static Joltage operator +(Joltage left, uint button)
            {
                var len = left.Length;
                int[] values = new int[left.Length];
                uint checker = 1;
                for (int i = 0; i < len; i++)
                {
                    values[i] = left.values[i];
                    if ((button & checker) == checker) values[i] += 1;
                    checker <<= 1;
                }
                return new Joltage(values);
            }

            public static Joltage operator -(Joltage left, uint button)
            {
                var len = left.Length;
                int[] values = new int[left.Length];
                uint checker = 1;
                for (int i = 0; i < len; i++)
                {
                    values[i] = left.values[i];
                    if ((button & checker) == checker) values[i] -= 1;
                    checker <<= 1;
                }
                return new Joltage(values);
            }

            public static Joltage operator -(Joltage left, Joltage right)
            {
                //Assume equal length
                var len = left.Length;
                int[] values = new int[len];
                for (int i = 0; i < len; i++) values[i] = left.values[i] - right.values[i];
                return new Joltage(values);
            }

            public static Joltage operator *(Joltage left, int right)
            {
                var len = left.Length;
                int[] values = new int[len];
                for (int i = 0; i < len; i++) values[i] = left.values[i] * right;
                return new Joltage(values);
            }

            public bool IsMultipleOf(Joltage other, out int factor)
            {
                factor = -1;
                var len = Length;
                for (int i = 0; i < len; i++)
                {
                    if (values[i] == 0)
                    {
                        if (other.values[i] != 0) return false;
                    }
                    else if (factor == -1)
                    {
                        factor = values[i] / other.values[i];
                    }
                    else if ((values[i] / other.values[i]) != factor) return false;
                }
                return true;
            }

            public int AffectedCount()
            {
                int result = 0;
                foreach (var item in values)
                {
                    if (item > 0) result++;
                }
                return result;
            }
            public override string ToString()
            {
                string result = "{" + values[0];

                for (int i = 1; i < values.Length; i++)
                {
                    result += "," + values[i];
                }
                result += '}';
                return result;
            }
        }

        struct JoltageEntry
        {
            public Joltage state;
            public byte depth;
            public int lastButtonPress;

            public JoltageEntry(Joltage state, byte depth, int lastButtonPress)
            {
                this.state = state;
                this.depth = depth;
                this.lastButtonPress = lastButtonPress;
            }
        }

        private uint[] allButtons;

        public void Execute(string input, out string star1, out string star2)
        {
            List<Task<int>> star2Tasks = new();

            var rows = input.Split(Environment.NewLine);

            int totalMoves = 0;
            int totalMoves2 = 0;
            int index = 0;
            int completedRows = 0;
            foreach (var rowIt in rows)
            {
                //Parse string
                var parts = rowIt.Split(' ');
                var joltageString = parts[^1];

                byte len;
                var targetLampState = ParseLampString(parts[0], out len);
                uint[] buttonWirings = new uint[parts.Length - 2];
                allButtons = buttonWirings;
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

                var targetJoltages = ParseJoltageString(joltageString);
                Day10VectorMath vectorMathSolver = new Day10VectorMath(index, targetJoltages.values, buttonWirings);
                //star2Tasks.Add(Task.Run(() =>
                //{
                int star2Moves = vectorMathSolver.Calculate();
                completedRows++;
                Log($"Completed {completedRows}/{rows.Length}");
                totalMoves2 += star2Moves;
                //    return star2Moves;
                //}));

                //Joltage state = Joltage.Flat(targetJoltages.Length);

                //while (true)
                //{
                //    var buttonInput = Console.ReadKey();
                //    var buttonIndex = buttonInput.Key - ConsoleKey.D1;
                //    if (buttonIndex >= 0 && buttonIndex < buttonWirings.Length)
                //    {
                //        state += buttonWirings[buttonIndex];
                //    }
                //    Log(state.ToString());
                //}
                index++;
            }

            star1 = totalMoves.ToString();

            Task.WhenAll(star2Tasks).Wait();
            foreach (var item in star2Tasks) totalMoves2 += item.Result;
            star2 = totalMoves2.ToString();
        }

        private int GetLeastButtonPressesForJoltage(Joltage targetState, uint[] allButtons)
        {
            int[] pressesPerButton = new int[allButtons.Length];

            Joltage[] buttons = new Joltage[allButtons.Length];
            for (int i = 0; i < allButtons.Length; i++) buttons[i] = Joltage.FromButton(allButtons[i], targetState.Length);

            int presses = 0;
            for (; ; presses++)
            {

            }

            int result = 0;
            foreach (var item in pressesPerButton) result += item;
            return result;
        }

        private int BreadthFirstSolveJoltage(Joltage target, uint[] allButtons)
        {
            Stack<int> pressedButtons = new();

            Joltage current = Joltage.Flat(target.Length);
            int pressCount = 1;
            int totalOptions = 0;
            int invalidOptions = 0;
            while (true)
            {
                while (pressedButtons.Count < pressCount)
                {
                    int toPush = 0;
                    pressedButtons.TryPeek(out toPush);
                    pressedButtons.Push(toPush);
                    current += allButtons[toPush];
                }


                totalOptions++;
                if (IsJoltagesEqual(current, target))
                {
                    return pressedButtons.Count;
                }
                else if (!IsJoltageValid(current, target))
                {
                    invalidOptions++;
                }

                var currentButton = pressedButtons.Pop();
                current -= allButtons[currentButton];

                bool increasedPressCount = false;
                while (currentButton == allButtons.Length - 1)
                {
                    if (!pressedButtons.TryPop(out currentButton))
                    {
                        pressCount++;
                        Log($"Presscount {pressCount} invalid: {invalidOptions}, total: {totalOptions}");
                        increasedPressCount = true;
                        break;
                    }
                    else
                    {
                        current -= allButtons[currentButton];
                    }
                }
                if (!increasedPressCount)
                {
                    pressedButtons.Push(currentButton + 1);
                    current += allButtons[currentButton + 1];
                }
            }
        }

        private int FindBestJoltagePath(Joltage targetState, uint[] allButtons)
        {
            var len = targetState.Length;
            int bestResult = int.MaxValue;
            Stack<JoltageEntry> nextButtonPresses = new();
            nextButtonPresses.Push(new JoltageEntry(Joltage.Flat(len), 1, 0));
            while (nextButtonPresses.TryPop(out JoltageEntry nextEntry))
            {
                Joltage currentState = nextEntry.state;
                byte depth = nextEntry.depth;

                for (int i = nextEntry.lastButtonPress; i < allButtons.Length; i++)
                {
                    var buttonJoltage = Joltage.FromButton(allButtons[i], len);
                    currentState += allButtons[i];
                    if (IsJoltagesEqual(currentState, targetState))
                    {
                        //Log($"Reached target state with buttons {GetButtonSequenceString(nextEntry.buttonPresses)}");
                        if (depth < bestResult) bestResult = depth;
                    }
                    else if (IsJoltageValid(currentState, targetState))
                    {
                        nextButtonPresses.Push(new JoltageEntry(CopyJoltageState(currentState), (byte)(depth + 1), i));
                    }
                    else
                    {
                        //Log($"Overjoltage for buttons {GetButtonSequenceString(nextEntry.buttonPresses)}");
                    }
                    currentState -= allButtons[i];
                }
            }

            if (bestResult == int.MaxValue) throw new Exception("Failed to reach state");
            return bestResult;

        }

        private string GetButtonSequenceString(List<int> buttonSequence)
        {
            string buttonString = "(" + buttonSequence[0].ToString();
            for (int ii = 1; ii < buttonSequence.Count; ii++)
            {
                buttonString += "," + buttonSequence[ii];
            }
            return buttonString + ")";
        }

        private Joltage CopyJoltageState(Joltage state)
        {
            var l = state.Length;
            int[] result = new int[l];
            for (int i = 0; i < l; i++) result[i] = state[i];
            return new Joltage(result);
        }

        private static bool IsJoltagesEqual(Joltage current, Joltage target)
        {
            for (int i = 0; i < current.Length; i++)
            {
                if (current[i] != target[i]) return false;
            }
            return true;
        }

        private bool IsJoltageValid(Joltage current, Joltage target)
        {
            for (int i = 0; i < current.Length; i++)
            {
                if (current[i] > target[i]) return false;
            }
            return true;
        }

        private uint GetMostAllowedButtonPress(byte[] currentState, byte[] targetState, uint button)
        {
            uint maxPresses = int.MaxValue;
            int len = currentState.Length;
            uint checker = 1;
            for (int i = 0; i < len; i++)
            {
                int remaining = targetState[i] - currentState[i];
                if (remaining < 0) throw new Exception("Currentstate invalid already");
                if ((button & checker) == checker && maxPresses > remaining) maxPresses = (uint)remaining;
                checker <<= 1;
            }

            return maxPresses;
        }

        //private bool IsJoltageMultiples(int[] current, int[] target)
        //{

        //}

        //private int[] GetJoltageDifference(int[] current, int[] target)
        //{
        //    int len = current.Length;
        //    int[] result = new int[len];
        //    for (int i = 0; i < len; i++) result[i] = target[i] - current[i];
        //}

        //private void ChangeJoltage(ref int[] state, uint button, int multiplier)
        //{
        //    //uint result = 0;
        //    uint checker = 1;
        //    for (int i = 0; i < state.Length; i++)
        //    {
        //        if ((button & checker) == checker)
        //        {
        //            state[i] += multiplier;
        //        }
        //        checker <<= 1;
        //    }
        //}

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

        private static Joltage ParseJoltageString(string joltageString)
        {
            var rawResult = joltageString.Substring(1, joltageString.Length - 2).Split(',');
            int[] result = new int[rawResult.Length];
            for (int i = 0; i < result.Length; i++) result[i] = int.Parse(rawResult[i]);
            return new Joltage(result);
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

        //private IntMatrix FromButtons(uint[] buttons, int length)
        //{
        //    var rows = buttons.Length;
        //    var columns = length;
        //    var result = new IntMatrix(rows, columns);
        //    for (int i = 0; i < rows; i++)
        //    {
        //        uint checker = 1;
        //        for (int ii = 0; ii < columns; ii++)
        //        {
        //            result[i, ii] = (buttons[i] & checker) == checker ? 1 : 0;
        //            checker <<= 1;
        //        }
        //    }
        //}

        //private class IntMatrix
        //{
        //    private int[,] values;

        //    public int Rows => values.GetLength(0);
        //    public int Columns => values.GetLength(1);

        //    public int this[int i, int j]
        //    {
        //        get => values[i, j];
        //        set => values[i, j] = value;
        //    }

        //    public IntMatrix(int rows, int columns)
        //    {
        //        values = new int[rows, columns];
        //    }

        //    public IntMatrix Transpose()
        //    {
        //        var rows = Columns;
        //        var columns = Rows;
        //        var result = new IntMatrix(rows, columns);
        //        for (int i = 0; i < rows; i++)
        //        {
        //            for (int ii = 0; ii < columns; ii++) result[i, ii] = this[ii, i];
        //        }
        //        return result;
        //    }

        //    public IntMatrix Inverse()
        //    {
        //        if (Columns != Rows) throw new InvalidOperationException("Cannot inverse non-square matrix");

        //        int rows = Rows;
        //        int columns = Columns;
        //        IntMatrix result = new IntMatrix(rows, columns);


        //    }
        //}
    }
}
