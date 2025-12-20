using System.Text;

namespace adventofcode_2025
{
    internal class Day9 : IDay
    {
        private enum Quadrant
        {
            INVALID,
            UPPER_RIGHT,
            LOWER_RIGHT,
            LOWER_LEFT,
            UPPER_LEFT
        }
        public struct Edge
        {
            public Vector2 First { get; }
            public Vector2 Second { get; }

            public bool IsVertical => First.x == Second.x && First.y != Second.y;

            public int XMax => Math.Max(First.x, Second.x);
            public int XMin => Math.Min(First.x, Second.x);
            public int YMax => Math.Max(First.y, Second.y);
            public int YMin => Math.Min(First.y, Second.y);

            public Edge(Vector2 first, Vector2 second)
            {
                this.First = first;
                this.Second = second;
            }

            public bool IsOnEdge(Vector2 point)
            {
                if (point == First || point == Second) return true;
                return
                    (point.x == First.x && point.y >= YMin && point.y <= YMax) ||
                    (point.y == First.y && point.x >= XMin && point.x <= XMax);
            }

            public bool Intersects(Edge other) => GetIntersectionPoint(other) != Vector2.Invalid;

            public Vector2 GetIntersectionPoint(Edge other)
            {
                //In this case, all edges are either horizontal or vertical
                bool isVertical = IsVertical;
                if (isVertical != other.IsVertical)
                {
                    Edge horizontalEdge = isVertical ? other : this;
                    Edge verticalEdge = isVertical ? this : other;

                    int xMin = horizontalEdge.XMin;
                    int xMax = horizontalEdge.XMax;

                    int yMin = verticalEdge.YMin;
                    int yMax = verticalEdge.YMax;

                    int x = verticalEdge.First.x;
                    int y = horizontalEdge.First.y;

                    bool intersects = x > xMin && x < xMax && y > yMin && y < yMax;

                    if (intersects) return new Vector2(x, y);
                }
                return Vector2.Invalid;
            }

            public override string ToString()
            {
                return $"{First} - {Second}";
            }
        }

        public struct Vector2
        {
            public static readonly Vector2 Invalid = new Vector2(-1, -1);

            public int x;
            public int y;

            public Vector2(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public override bool Equals(object? obj)
            {
                return obj is Vector2 vector &&
                       x == vector.x &&
                       y == vector.y;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(x, y);
            }

            public override string ToString()
            {
                string x;
                string y;
                if (this.x == int.MaxValue) x = "max";
                else x = this.x.ToString();
                if (this.y == int.MaxValue) y = "max";
                else y = this.y.ToString();
                return $"({x},{y})";
            }

            public static Vector2 ParseFromString(string str)
            {
                string[] parts = str.Split(',');

                int x = int.Parse(parts[0]);
                int y = int.Parse(parts[1]);

                return new Vector2(x, y);
            }

            public static bool operator ==(Vector2 a, Vector2 b) => a.x == b.x && a.y == b.y;
            public static bool operator !=(Vector2 a, Vector2 b) => !(a == b);
        }

        public void Execute(string input, out string star1, out string star2)
        {
            Vector2[] corners = input.Split(Environment.NewLine).Select(raw => Vector2.ParseFromString(raw)).ToArray();

            int l = corners.Length;

            ulong biggestArea = 0;

            for (int i = 0; i < l - 1; i++)
            {
                Vector2 left = corners[i];
                for (int ii = i + 1; ii < l; ii++)
                {
                    Vector2 right = corners[ii];

                    ulong area = ((ulong)Math.Abs(right.x - left.x) + 1) * ((ulong)Math.Abs(right.y - left.y) + 1);

                    if (area > biggestArea) biggestArea = area;
                }
            }

            star1 = biggestArea.ToString();


            Edge[] edges = new Edge[l];
            int totalWidth = 0;
            int totalHeight = 0;
            for (int i = 0; i < l; i++)
            {
                edges[i] = new Edge(corners[i], corners[(i + 1) % l]);

                totalWidth = Math.Max((int)corners[i].x + 3, totalWidth);
                totalHeight = Math.Max((int)corners[i].y + 2, totalHeight);
            }

            biggestArea = 0;

            //StringBuilder worldPrinter = new(totalHeight * totalWidth);
            //for (int y = 0; y < totalHeight; y++)
            //{
            //    for (int x = 0; x < totalWidth; x++)
            //    {
            //        worldPrinter.Append(TileInPolygon(edges, new Vector2(x, y)) ? '#' : '.');
            //    }
            //    worldPrinter.AppendLine();
            //}
            //Console.WriteLine(worldPrinter.ToString());

            for (int i = 0; i < l - 1; i++)
            {
                Vector2 first = corners[i];
                for (int ii = i + 1; ii < l; ii++)
                {
                    Vector2 third = corners[ii];
                    Vector2 second;
                    Vector2 fourth;
                    if (first.x < third.x == first.y < third.y)
                    {
                        second = new Vector2(first.x, third.y);
                        fourth = new Vector2(third.x, first.y);
                    }
                    else
                    {
                        second = new Vector2(third.x, first.y);
                        fourth = new Vector2(first.x, third.y);
                    }

                    Edge[] rectEdges =
                    {
                            new (first, second),
                            new (second, third),
                            new (third, fourth),
                            new (fourth, first),
                        };
                    if (TileInPolygon(corners, second) &&
                        TileInPolygon(corners, fourth))
                    {

                        bool valid = true;
                        for (int iii = 0; iii < rectEdges.Length; iii++)
                        {
                            for (int iiii = 0; iiii < edges.Length; iiii++)
                            {
                                Edge item = edges[iiii];
                                Edge rectEdgeIt = rectEdges[iii];

                                Vector2 intersectionPoint = rectEdgeIt.GetIntersectionPoint(item);

                                if (intersectionPoint == Vector2.Invalid) continue;

                                int x = intersectionPoint.x;
                                int y = intersectionPoint.y;
                                Vector2 point1;
                                Vector2 point2;
                                if (rectEdgeIt.IsVertical)
                                {
                                    point1 = new Vector2(x, y - 1);
                                    point2 = new Vector2(x, y + 1);
                                }
                                else
                                {
                                    point1 = new Vector2(x - 1, y);
                                    point2 = new Vector2(x + 1, y);
                                }

                                if (!TileInPolygon(corners, point1) || !TileInPolygon(corners, point2))
                                {
                                    valid = false;
                                    break;
                                }
                            }
                            if (!valid) break;
                        }

                        if (valid)
                        {
                            ulong width = (ulong)Math.Abs(first.x - third.x) + 1;
                            ulong height = (ulong)Math.Abs(first.y - third.y) + 1;
                            var currentArea = width * height;
                            biggestArea = Math.Max(currentArea, biggestArea);
                            //Log($"size: {currentArea}, max: {biggestArea}");
                        }
                    }
                }
            }

            star2 = biggestArea.ToString();
        }

        private bool TileInPolygon(Vector2[] vertices, Vector2 pos, bool debug = false)
        {
            //Verify winding goes the correct direction. Terminology changes based on if +y is up or down, but algorithm shouldn't

            if (debug) Log("debugging tileinpolygonwinding for pos" + pos);
            int turningNumber = 0;

            int len = vertices.Length;
            Quadrant precedingQuadrant = Quadrant.INVALID;

            for (int i = 0; precedingQuadrant == Quadrant.INVALID; i--)
            {
                precedingQuadrant = GetRelativeQuadrant(pos, vertices[(i + len) % len]); //Add l here 
            }

            for (int i = 1; i < len; i++)
            {
                Quadrant nextQuadrant = Quadrant.INVALID;
                for (; nextQuadrant == Quadrant.INVALID; i++)
                {
                    nextQuadrant = GetRelativeQuadrant(pos, vertices[i % len]);
                }
                i--;


                int windingValue;
                if (precedingQuadrant == Quadrant.UPPER_RIGHT && nextQuadrant == Quadrant.UPPER_LEFT) windingValue = -1;
                else if (precedingQuadrant == Quadrant.UPPER_LEFT && nextQuadrant == Quadrant.UPPER_RIGHT) windingValue = 1;
                else windingValue = nextQuadrant - precedingQuadrant;

                if (debug && windingValue != 0) Log($"Winding {windingValue} (from {precedingQuadrant} to {nextQuadrant})");
                turningNumber += windingValue;
                precedingQuadrant = nextQuadrant;

            }
            bool inPolygon = turningNumber <= -4;
            if (debug) Log($"Tile is in polygon: {inPolygon} (winding number: {turningNumber})");
            return inPolygon;
        }

        Quadrant GetRelativeQuadrant(Vector2 pos, Vector2 inQuadrant)
        {
            if (inQuadrant.x > pos.x)
            {
                if (inQuadrant.y > pos.y) return Quadrant.UPPER_RIGHT;
                else if (inQuadrant.y < pos.y) return Quadrant.LOWER_RIGHT;
            }
            else if (inQuadrant.x < pos.x)
            {
                if (inQuadrant.y > pos.y) return Quadrant.UPPER_LEFT;
                else if (inQuadrant.y < pos.y) return Quadrant.LOWER_LEFT;
            }
            return Quadrant.INVALID;
        }

        void Log(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
