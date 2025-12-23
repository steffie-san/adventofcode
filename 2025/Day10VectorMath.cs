using MathNet.Numerics.LinearAlgebra;
using System.Collections;
using System.Diagnostics;

namespace adventofcode_2025
{
    internal class Day10VectorMath
    {
        private struct Joltage
        {
            private readonly int[] values;

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

            //public static Vector operator +(Vector left, uint button)
            //{
            //    var len = left.Length;
            //    int[] values = new int[left.Length];
            //    uint checker = 1;
            //    for (int i = 0; i < len; i++)
            //    {
            //        values[i] = left.values[i];
            //        if ((button & checker) == checker) values[i] += 1;
            //        checker <<= 1;
            //    }
            //    return new Vector(values);
            //}

            public static Joltage operator -(Joltage left, Joltage right)
            {
                //Assume equal length
                var len = left.Length;
                int[] values = new int[len];
                for (int i = 0; i < len; i++) values[i] = left.values[i] - right.values[i];
                return new Joltage(values);
            }

            //public static Vector operator -(Vector left, uint button)
            //{
            //    var len = left.Length;
            //    int[] values = new int[left.Length];
            //    uint checker = 1;
            //    for (int i = 0; i < len; i++)
            //    {
            //        values[i] = left.values[i];
            //        if ((button & checker) == checker) values[i] -= 1;
            //        checker <<= 1;
            //    }
            //    return new Vector(values);
            //}

            public static Joltage operator *(Joltage left, int right)
            {
                var len = left.Length;
                int[] values = new int[len];
                for (int i = 0; i < len; i++) values[i] = left.values[i] * right;
                return new Joltage(values);
            }

            [Obsolete]
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

            public int GetMaxComponent()
            {
                int result = 0;
                foreach (var item in values) result = int.Max(result, item);
                return result;
            }

            public int GetMinComponent()
            {
                int result = int.MaxValue;
                foreach (var item in values) result = int.Min(result, item);
                return result;
            }

            public int GetPower()
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

        private readonly int rowIndex;
        private readonly int length;
        private readonly Joltage target;
        private readonly Joltage[] orderedButtons;
        private readonly Dictionary<int, List<int>> joltageToButtonIndexMap;

        public Day10VectorMath(int rowIndex, int[] target, uint[] buttons)
        {
            this.rowIndex = rowIndex;
            length = target.Length;
            this.target = new Joltage(target);
            this.orderedButtons = buttons.Select(x => Joltage.FromButton(x, length)).OrderByDescending(x => x.GetPower()).ToArray();

            int buttonCount = buttons.Length;
            joltageToButtonIndexMap = new();
            for (int i = buttonCount - 1; i >= 0; i--)
            {
                for (int j = 0; j < length; j++)
                {
                    if (orderedButtons[i][j] > 0)
                    {
                        if (!joltageToButtonIndexMap.TryGetValue(j, out var buttonList))
                        {
                            joltageToButtonIndexMap[j] = buttonList = new List<int>();
                        }
                        buttonList.Add(i);
                    }
                }
            }
        }

        public int Calculate()
        {
            Console.WriteLine($"starting {rowIndex}");
            Stopwatch stopwatch = Stopwatch.StartNew();
            var buttonCount = orderedButtons.Length;
            int[] presetButtons = new int[buttonCount];



            Vector<double> approx = GetApproximateSolution();

            FixNegativeButtonPresses(approx);
            //for (int i = 0; i < buttonCount; i++)
            //{
            //    var power = orderedButtons[i].GetPower();
            //    var toDecrease = approx[i] * Lerp(0.1, 0.2, power / length);
            //    //Console.WriteLine($"Button {i}, power {power}, toDecrease {toDecrease}, value: {approx[i]}");
            //    approx[i] -= toDecrease;
            //}

            Joltage currentState = Joltage.Flat(length);
            for (int i = 0; i < presetButtons.Length; i++)
            {
                presetButtons[i] = (int)approx[i];
                currentState += orderedButtons[i] * presetButtons[i];
            }

            if (!IsJoltageValid(currentState)) throw new NotImplementedException();

            for (int i = 0; ; i++)
            {
                foreach (var item in ProcessDot(presetButtons, currentState, 0, i))
                {
                    Joltage missing = target - currentState;
                    if (Calculate(presetButtons, out int buttonPresses))
                    {
                        stopwatch.Stop();
                        Console.WriteLine(
                            $"Row {rowIndex} with target length {length}, " +
                            $"and buttoncount {orderedButtons.Length}, " +
                            $"execution took {stopwatch.Elapsed}" +
                            $", yielding {buttonPresses} buttonpresses");
                        return buttonPresses;
                    }
                }
            }
        }

        private void FixNegativeButtonPresses(Vector<double> approx)
        {
            for (int i = 0; i < approx.Count; i++)
            {
                if (approx[i] < 0)
                {
                    int toIncrease = (int)double.Ceiling(-approx[i]);
                    approx[i] += toIncrease;

                    //Decrease buttons affecting affected joltages to prevent overjoltage
                    //var increasedButton = orderedButtons[i];
                    //for (int j = 0; j < length; j++)
                    //{
                    //    if (increasedButton[j] > 0) //increased button affected joltage j
                    //    {
                    //        DecreaseButtonsAffecting(approx, j, toIncrease);
                    //    }
                    //}
                }
            }

            Joltage currentState = Joltage.Flat(length);
            for (int i = 0; i < approx.Count; i++) currentState += orderedButtons[i] * (int)approx[i];

            for (int i = 0; i < length; i++)
            {
                var overJoltage = currentState[i] - target[i];
                if (overJoltage > 0) DecreaseButtonsAffecting(approx, i, overJoltage);
            }
        }

        private void DecreaseButtonsAffecting(Vector<double> solution, int joltageIndex, int pointsToDecrease)
        {
            var buttonList = joltageToButtonIndexMap[joltageIndex];
            //Only for debugging
            var state = Joltage.Flat(length);
            for (int i = 0; i < solution.Count; i++) state += orderedButtons[i] * (int)solution[i];
            int actualToDecrease = state[joltageIndex] - target[joltageIndex];
            var skipCount = 0;

            for (int i = 0; pointsToDecrease > 0; i++)
            {
                int buttonToDecrease = buttonList[i % buttonList.Count];
                if (solution[buttonToDecrease] > 1)
                {
                    solution[buttonToDecrease]--;
                    skipCount = 0;
                    pointsToDecrease--;
                }
                else
                {
                    skipCount++;
                    //All affecting buttons are below 1: rounding will take care of the rest
                    if (skipCount > buttonList.Count) return;
                }
            }
        }

        private Vector<double> GetApproximateSolution()
        {
            double[] targetAsDouble = new double[length];
            for (int i = 0; i < length; i++) targetAsDouble[i] = target[i];

            Vector<double> targetVector = Vector<double>.Build.DenseOfArray(targetAsDouble);

            Matrix<double> buttonMatrix = Matrix<double>.Build.Dense(length, orderedButtons.Length);
            for (int i = 0; i < orderedButtons.Length; i++)
            {
                for (int ii = 0; ii < length; ii++) buttonMatrix[ii, i] = orderedButtons[i][ii];
            }

            Matrix<double> inversed = buttonMatrix.PseudoInverse();

            Vector<double> solution = inversed * targetVector;

            //Console.WriteLine(solution.ToString());
            //var sum = solution.Sum();
            //Console.WriteLine($"Sum: {sum} (rounded: {(int)Math.Round(sum)})");
            return solution;
        }

        //One minus
        //1000
        //0100
        //0010
        //0001

        //Two minus
        //2000
        //1100
        //1010
        //1001

        //0200
        //0110
        //0101

        //0020
        //0011

        //0002

        //three minus

        //3000
        //2100
        //2010
        //2001

        //1200
        //1110
        //1101


        //#000
        //0#00
        //00#0
        //000#

        //#000
        //#000

        //0#00
        //#000

        //00#0
        //#000

        private IEnumerable ProcessDot(int[] presetButtons, Joltage currentState, int minPosition, int dotIndex)
        {
            if (dotIndex == 0)
            {
                yield return null;
                yield break;
            }
            for (int i = minPosition; i < orderedButtons.Length; i++)
            {
                if (GetMaxPressCount(currentState, orderedButtons[i]) <= 0) continue;

                presetButtons[i]++;
                currentState += orderedButtons[i];
                foreach (var item in ProcessDot(presetButtons, currentState, i, dotIndex - 1))
                {
                    yield return null;
                }
                presetButtons[i]--;
                currentState -= orderedButtons[i];
            }
        }

        private bool Calculate(int[] presetButtons, out int buttonPresses)
        {
            buttonPresses = 0;
            Joltage currentState = Joltage.Flat(length);
            for (int i = 0; i < presetButtons.Length; i++)
            {
                currentState += orderedButtons[i] * presetButtons[i];
                buttonPresses += presetButtons[i];
            }
            if (!IsJoltageValid(currentState)) throw new Exception("Entering calulate with completely invalid solution");
            for (int i = 0; i < orderedButtons.Length; i++)
            {
                var buttonIt = orderedButtons[i];
                int maxPressCount = GetMaxPressCount(currentState, buttonIt);
                currentState += buttonIt * maxPressCount;
                buttonPresses += maxPressCount;
                //Console.WriteLine($"Press button {buttonIt} {maxPressCount} times");
            }
            return IsTarget(currentState);
        }

        private int GetMaxPressCount(Joltage currentState, Joltage button)
        {
            int max = int.MaxValue;
            for (int i = 0; i < button.Length; i++)
            {
                if (button[i] == 0) continue;
                max = int.Min(max, target[i] - currentState[i]); //buttons increase 0 or 1 only
            }
            //Console.WriteLine($"for states {currentState} - {target} and button {button} maxpresscount is {max}");
            return max;
        }

        private bool IsTarget(Joltage vector)
        {
            for (int i = 0; i < vector.Length; i++)
            {
                if (vector[i] != target[i]) return false;
            }
            return true;
        }
        private bool IsJoltageValid(Joltage current)
        {
            for (int i = 0; i < current.Length; i++)
            {
                if (current[i] > target[i]) return false;
            }
            return true;
        }

        private double Lerp(double a, double b, double t)
        {
            return a + (b - a) * t;
        }
    }
}
