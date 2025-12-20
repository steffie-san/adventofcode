using System;
using System.Collections.Generic;
using System.Linq;
using Debug = UnityEngine.Debug;

namespace adventofcode_2025
{
    public enum Quadrant
    {
        INVALID,
        UPPER_RIGHT,
        LOWER_RIGHT,
        LOWER_LEFT,
        UPPER_LEFT
    }
    public class Day9
    {
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

        public int firstIndex = 0;
        public int secondIndex = 0;
        public ulong biggestArea = 0;
        public ulong currentArea = 0;
        public Vector2[] corners;

        //public List<(Vector2, bool)> invalidPoints = new ();
        //public List<int> badRectEdges = new();
        //public List<int> badPolyEdges = new();
        //public int[] isEdgeWinding;


        public IEnumerator<Edge[]> Execute(string input)
        {
            corners = input.Split(Environment.NewLine).Select(raw => Vector2.ParseFromString(raw)).ToArray();

            int l = corners.Length;


            Edge[] edges = new Edge[l];
            //isEdgeWinding = new int[l];
            int totalWidth = 0;
            int totalHeight = 0;
            for (int i = 0; i < l; i++)
            {
                edges[i] = new Edge(corners[i], corners[(i + 1) % l]);

                totalWidth = Math.Max(corners[i].x + 3, totalWidth);
                totalHeight = Math.Max(corners[i].y + 2, totalHeight);
            }


            for (int i = 0; i < l - 1; i++)
            {
                firstIndex = i;
                //if (i == 217)
                //{
                //    UnityEngine.Debug.Break();
                //}
                Vector2 first = corners[i];
                for (int ii = i + 1; ii < l; ii++)
                {
                    //if (ii == 249)
                    //{
                    //    UnityEngine.Debug.Break();
                    //}
                    secondIndex = ii;
                    //invalidPoints.Clear();
                    //badRectEdges.Clear();
                    //badPolyEdges.Clear();

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

                    ulong width = (ulong)Math.Abs(first.x - third.x) + 1;
                    ulong height = (ulong)Math.Abs(first.y - third.y) + 1;
                    currentArea = width * height;

                    Edge[] rectEdges =
                    {
                            new (first, second),
                            new (second, third),
                            new (third, fourth),
                            new (fourth, first),
                        };
                    bool secondInPol = TileInPolygon(corners, second, i == 217 && ii == 248);
                    bool fourthInPol = TileInPolygon(corners, fourth);
                    if (!secondInPol)
                    {
                        //invalidPoints.Add((second, false));
                        //UnityEngine.Debug.Break();
                    }
                    if (!fourthInPol)
                    {
                        //invalidPoints.Add((fourth, false));
                    }
                    if (secondInPol &&
                        fourthInPol)
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

                                if (!TileInPolygon(corners, pointsToCheck[0]) || !TileInPolygon(corners, pointsToCheck[1]))
                                {
                                    //invalidPoints.Add((intersectionPoint, true));
                                    //badPolyEdges.Add(iiii);
                                    //badRectEdges.Add(iii);
                                    valid = false;
                                    break;
                                }
                            }
                            if (!valid) break;
                        }
                        //valid &= secondInPol && fourthInPol;

                        if (valid)
                        {
                            biggestArea = Math.Max(currentArea, biggestArea);
                            Log($"size: {currentArea}, max: {biggestArea}");
                        }
                    }

                    yield return rectEdges;
                }
                //Console.WriteLine($"{i/(double)(l-1) / 100}%....");
            }

            //string star2 = biggestArea.ToString();
            Log(biggestArea.ToString());
            yield break;
            //1289423295 is too low
            //1562459680 is the right answer from vertices 217, 248
        }

        public bool TileInPolygon(Vector2[] vertices, Vector2 pos, bool debug = false)
        {
            return TileInPolygonWinding(vertices, pos, debug);
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

        private bool TileInPolygonWinding(Vector2[] vertices, Vector2 pos, bool debug)
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
            //throw new Exception($"Cannot get quadrant from {pos} to {inQuadrant}. Are they on a line?");
        }

        bool isOnLine(params Vector2[] vertices)
        {
            bool horizontally = true;
            bool vertically = true;
            for (int i = 1; i < vertices.Length; i++)
            {
                if (!horizontally && !vertically) return false;

                if (vertices[0].x != vertices[i].x) horizontally = false;
                if (vertices[0].y != vertices[i].y) vertically = false;
            }
            return horizontally || vertically;
        }

        void Log(string msg)
        {
#if UNITY_EDITOR
            Debug.Log(msg);
#else
            Console.WriteLine(msg);
#endif
        }
    }
}
