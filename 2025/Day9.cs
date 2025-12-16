using System.Text;

namespace adventofcode_2025
{
    internal class Day9 : IDay
    {
        struct Edge
        {
            public Vector2 First { get; }
            public Vector2 Second { get; }

            public bool IsVertical => First.x == Second.x && First.y != Second.y;

            public long XMax => Math.Max(First.x, Second.x);
            public long XMin => Math.Min(First.x, Second.x);
            public long YMax => Math.Max(First.y, Second.y);
            public long YMin => Math.Min(First.y, Second.y);

            public Edge(Vector2 first, Vector2 second)
            {
                this.First = first;
                this.Second = second;
            }

            //public Vector2? GetIntersectionPoint(Edge edge)
            //{
            //    long x, y;
            //    Vector2? result = null;
            //    if (IsVertical != edge.IsVertical)
            //    {
            //        if (IsVertical)
            //        {
            //            y = edge.First.y;
            //            x = First.x;
            //        }
            //        else
            //        {
            //            y = First.y;
            //            x = edge.First.x;
            //        }
            //        result = new(x, y);
            //    }
            //    else
            //    {
            //        //If they overlap, the "intersection" is the point closest to this.First
            //        if (IsVertical)
            //        {
            //            if (First.x == edge.First.x)
            //            {
            //                x = First.x;
            //                if (edge.YMin < YMin) y = YMin;
            //                else y = edge.YMin;
            //                result = new(x, y);
            //            }
            //        }
            //        else
            //        {
            //            if (First.y == edge.First.y)
            //            {
            //                y = First.y;
            //                if (edge.XMin < XMin) x = XMin;
            //                else x = edge.XMin;
            //                result = new(x, y);
            //            }
            //        }
            //    }
            //    if (result != null && IsOnEdge(result.Value) && edge.IsOnEdge(result.Value)) return result;
            //    return null;
            //}

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

                    long xMin = horizontalEdge.XMin;
                    long xMax = horizontalEdge.XMax;

                    long yMin = verticalEdge.YMin;
                    long yMax = verticalEdge.YMax;

                    long x = verticalEdge.First.x;
                    long y = horizontalEdge.First.y;

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

        struct Vector2
        {
            public static readonly Vector2 Invalid = new Vector2(-1, -1);

            public long x;
            public long y;

            public Vector2(long x, long y)
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
                if (this.x == long.MaxValue) x = "max";
                else x = this.x.ToString();
                if (this.y == long.MaxValue) y = "max";
                else y = this.y.ToString();
                return $"({x},{y})";
            }

            public static Vector2 ParseFromString(string str)
            {
                string[] parts = str.Split(',');

                long x = long.Parse(parts[0]);
                long y = long.Parse(parts[1]);

                return new Vector2(x, y);
            }

            public static bool operator ==(Vector2 a, Vector2 b) => a.x == b.x && a.y == b.y;
            public static bool operator !=(Vector2 a, Vector2 b) => !(a == b);
        }

        public void Execute(string input, out string star1, out string star2)
        {
            Vector2[] corners = input.Split(Environment.NewLine).Select(raw => Vector2.ParseFromString(raw)).ToArray();

            int l = corners.Length;

            long biggestArea = 0;

            for (int i = 0; i < l - 1; i++)
            {
                Vector2 left = corners[i];
                for (int ii = i + 1; ii < l; ii++)
                {
                    Vector2 right = corners[ii];

                    long area = (Math.Abs(right.x - left.x) + 1) * (Math.Abs(right.y - left.y) + 1);

                    //Console.WriteLine($"Area of {left} and {right} is {area}");

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
                    Vector2 second = corners[ii];

                    long xMin = Math.Min(first.x, second.x);
                    long xMax = Math.Max(first.x, second.x);

                    long yMin = Math.Min(first.y, second.y);
                    long yMax = Math.Max(first.y, second.y);

                    Vector2 topLeft = new Vector2(xMin, yMin);
                    Vector2 topRight = new Vector2(xMax, yMin);
                    Vector2 bottomLeft = new Vector2(xMin, yMax);
                    Vector2 bottomRight = new Vector2(xMax, yMax);

                    if (TileInPolygon(edges, topLeft) &&
                        TileInPolygon(edges, topRight) &&
                        TileInPolygon(edges, bottomLeft) &&
                        TileInPolygon(edges, bottomRight))
                    {
                        Edge[] rectEdges =
                        [
                            new (topLeft, topRight),
                            new (topRight, bottomRight),
                            new (bottomRight, bottomLeft),
                            new (bottomLeft, topLeft),
                        ];

                        bool valid = true;
                        foreach (Edge rectEdgeIt in rectEdges)
                        {
                            foreach (Edge item in edges)
                            {
                                Vector2 intersectionPoint = rectEdgeIt.GetIntersectionPoint(item);

                                if (intersectionPoint == Vector2.Invalid) continue;

                                var x = intersectionPoint.x;
                                var y = intersectionPoint.y;
                                Vector2[] pointsToCheck = new Vector2[2];
                                if (rectEdgeIt.IsVertical)
                                {
                                    pointsToCheck[0] = new Vector2(x, y - 1);
                                    pointsToCheck[1] = new Vector2(x, y + 1);
                                }
                                else
                                {
                                    pointsToCheck[0] = new Vector2(x - 1, y);
                                    pointsToCheck[1] = new Vector2(x + 1, y);
                                }

                                if (!TileInPolygon(edges, pointsToCheck[0]) || !TileInPolygon(edges, pointsToCheck[1]))
                                {
                                    valid = false;
                                    break;
                                }
                            }
                            if (!valid) break;
                        }


                        if (valid)
                        {
                            long area = (xMax - xMin + 1) * (yMax - yMin + 1);
                            biggestArea = Math.Max(area, biggestArea);
                        }
                    }
                }
                //Console.WriteLine($"{i/(double)(l-1) / 100}%....");
            }

            star2 = biggestArea.ToString();
            //1289423295 is too low
        }

        private bool TileInPolygon(Edge[] edges, Vector2 pos)
        {
            foreach (var item in edges)
            {
                if (item.IsOnEdge(pos)) return true;
            }

            return TileInPolygonWinding(edges, pos);
        }

        private bool TileInPolygonRaycast(Edge[] edges, Vector2 pos)
        {
            //Assuming all edges are in positive y space

            Edge testEdge = new Edge(pos, new Vector2(pos.x, -1));
            int intersectCount = 0;
            foreach (Edge item in edges)
            {
                if (item.First == pos) return true;
                else if (item.IsOnEdge(pos)) return true;
                else if (testEdge.Intersects(item)) intersectCount++;
            }
            return intersectCount % 2 == 1;
        }

        private bool TileInPolygonWinding(Edge[] edges, Vector2 pos)
        {
            int turningNumber = 0;
            foreach (var item in edges)
            {
                var currentPoint = item.First;
                var nextPoint = item.Second;

                //Horizontal, left to right
                if (currentPoint.x <= pos.x && nextPoint.x > pos.x)
                {
                    if (currentPoint.y < pos.y) turningNumber++;
                    else if (currentPoint.y > pos.y) turningNumber--;
                    else throw new Exception("This shouldn't happen");
                }
                //Horizontal, right to left
                else if (currentPoint.x >= pos.x && nextPoint.x < pos.x)
                {
                    if (currentPoint.y > pos.y) turningNumber++;
                    else if (currentPoint.y < pos.y) turningNumber--;
                    else throw new Exception("This shouldn't happen");
                }
                //Vertical, up to down
                else if (currentPoint.y <= pos.y && nextPoint.y > pos.y)
                {
                    if (currentPoint.x > pos.x) turningNumber++;
                    else if (currentPoint.x < pos.x) turningNumber--;
                    else throw new Exception("This shouldn't happen");
                }
                //Vertical, down to up
                else if (currentPoint.y >= pos.y && nextPoint.y < pos.y)
                {
                    if (currentPoint.x > pos.x) turningNumber--;
                    else if (currentPoint.x < pos.x) turningNumber++;
                    else throw new Exception("This shouldn't happen");
                }

            }
            return turningNumber >= 4;
        }

        private void BuildRectangles(Edge[] edges)
        {

        }
    }
}
