using System.Numerics;

namespace adventofcode_2025
{
    internal class Day8 : IDay
    {
        struct Connection
        {
            public Vector3 left;
            public Vector3 right;

            public BigInteger sqrdDistance;

            public Connection(Vector3 left, Vector3 right) : this()
            {
                this.left = left;
                this.right = right;
                sqrdDistance = Vector3.SqrdDistance(left, right);
            }
        }

        struct Vector3
        {
            public long x;
            public long y;
            public long z;

            public Vector3(long x, long y, long z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public override bool Equals(object? obj)
            {
                return obj is Vector3 vector &&
                       x == vector.x &&
                       y == vector.y &&
                       z == vector.z;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(x, y, z);
            }

            public override string ToString()
            {
                return $"({x},{y},{z})";
            }

            public static Vector3 ParseFromString(string str)
            {
                string[] parts = str.Split(',');

                long x = long.Parse(parts[0]);
                long y = long.Parse(parts[1]);
                long z = long.Parse(parts[2]);

                return new Vector3(x, y, z);
            }

            public static BigInteger SqrdDistance(Vector3 left, Vector3 right)
            {
                return 
                    BigInteger.Pow(left.x - right.x, 2) + 
                    BigInteger.Pow(left.y - right.y, 2) + 
                    BigInteger.Pow(left.z - right.z, 2);
            }
        }

        public void Execute(string input, out string star1, out string star2)
        {
            Vector3[] positions = input.Split(Environment.NewLine).Select(x => Vector3.ParseFromString(x)).ToArray();
            int l = positions.Length;

            List<HashSet<Vector3>> allCircuits = new();
            Dictionary<Vector3, HashSet<Vector3>> circuits = new();

            foreach (Vector3 posIt in positions)
            {
                HashSet<Vector3> newCircuit = new HashSet<Vector3>() { posIt };
                circuits.Add(posIt, newCircuit);
                allCircuits.Add(newCircuit);
            }

            List<Connection> potentialConnections = new List<Connection>(l * l);

            for (int i = 0; i < l - 1; i++)
            {
                for (int ii = i + 1; ii < l; ii++)
                {
                    potentialConnections.Add(new Connection(positions[i], positions[ii]));
                }
            }

            potentialConnections.Sort((left, right) => left.sqrdDistance.CompareTo(right.sqrdDistance));
            int star1ConnectionCount = 10;

            for (int i = 0; i < star1ConnectionCount; i++)
            {
                Connection connection = potentialConnections[i];

                if (circuits[connection.left].Contains(connection.right))
                {
                    //Console.WriteLine($"{connection.left} is already in the same circuit as {connection.right}");
                    continue;
                }

                //Console.WriteLine($"Connection {i}: {connection.left} - {connection.right} ({connection.sqrdDistance})");

                HashSet<Vector3> leftCircuit = circuits[connection.left];
                HashSet<Vector3> rightCircuit = circuits[connection.right];

                foreach (var item in rightCircuit)
                {
                    circuits[item] = leftCircuit;
                    leftCircuit.Add(item);
                }
                allCircuits.Remove(rightCircuit);
                rightCircuit.Clear();
            }

            allCircuits.Sort((x, y) => y.Count.CompareTo(x.Count));

            long result1 = allCircuits[0].Count * allCircuits[1].Count * allCircuits[2].Count;

            star1 = result1.ToString();
            long result2 = 0;

            for (int i = star1ConnectionCount; i < potentialConnections.Count; i++)
            {
                Connection connection = potentialConnections[i];

                if (circuits[connection.left].Contains(connection.right)) continue;

                HashSet<Vector3> leftCircuit = circuits[connection.left];
                HashSet<Vector3> rightCircuit = circuits[connection.right];

                foreach (Vector3 boxIt in rightCircuit)
                {
                    circuits[boxIt] = leftCircuit;
                    leftCircuit.Add(boxIt);
                }
                allCircuits.Remove(rightCircuit);
                rightCircuit.Clear();

                if(allCircuits.Count == 1)
                {
                    result2 = connection.left.x * connection.right.x;
                    break;
                }
            }

            star2 = result2.ToString();
        }
    }
}
