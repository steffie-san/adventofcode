using adventofcode_2025;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day9Operator : MonoBehaviour
{
    public bool useExample;
    public string inputPath;
    public string exampleInputPath;

    public LineRenderer edgePrefab;
    public SpriteRenderer vertexPrefab;

    public float scale = 1f;
    public float updateTime = 0f;
    public float lineThickness = 1f;
    public float markerScale = 1f;

    List<LineRenderer> edges = new List<LineRenderer>();
    List<SpriteRenderer> badMarkers = new();


    LineRenderer[] rectEdges;

    Day9 day = new Day9();
    float lastLineThickness = -1;
    float lastScale = -1;

    private IEnumerator Start()
    {
        string path = useExample ? exampleInputPath : inputPath;
        var input = Resources.Load<TextAsset>(path);


        IEnumerator<Day9.Edge[]> iterator = day.Execute(input.text);
        iterator.MoveNext();

        Day9.Vector2 min = new Day9.Vector2(int.MaxValue, int.MaxValue);
        Day9.Vector2 max = new Day9.Vector2(0, 0);
        Day9.Edge[] allEdges = new Day9.Edge[day.corners.Length];

        for (int i = 0; i < day.corners.Length; i++)
        {
            var current = day.corners[i];
            var next = i == day.corners.Length - 1 ? day.corners[0] : current;
            allEdges[i] = new Day9.Edge(current, next);

            if (current.x < min.x) min.x = current.x;
            if (current.y < min.y) min.y = current.y;
            if (current.x > max.x) max.x = current.x;
            if (current.y > max.y) max.y = current.y;

            edges.Add(Instantiate(edgePrefab));
        }

        rectEdges = new LineRenderer[4];
        for (int i = 0; i < 4; i++) rectEdges[i] = Instantiate(edgePrefab);

        OnScaleUpdated();



        int count = 0;
        int stepCount = 1000;
        for (int y = min.y; y < max.y; y += stepCount)
        {
            for (int x = min.x; x < max.x; x += stepCount)
            {
                var vector = new Day9.Vector2(x, y);
                var pixel = Instantiate(vertexPrefab);
                pixel.transform.position = ToWorldSpace(vector);
                pixel.transform.localScale = Vector3.one * markerScale;
                pixel.color = day.TileInPolygon(allEdges, vector) ? Color.green : Color.red;

                if (count > 10)
                {
                    yield return null;
                    count = 0;
                }
                else count++;
            }
        }

        //do
        //{
        //    Day9.Edge[] currentEdges = iterator.Current;
        //    for (int i = 0; i < rectEdges.Length; i++)
        //    {
        //        var c1 = currentEdges[i].First;
        //        var c2 = currentEdges[i].Second;
        //        rectEdges[i].SetPosition(0, ToWorldSpace(c1));
        //        rectEdges[i].SetPosition(1, ToWorldSpace(c2));

        //    }
        //    UpdateBadMarkers(day.invalidPoints);
        //    yield return new WaitForSeconds(updateTime);
        //}
        //while (iterator.MoveNext());
        //UpdateBadMarkers(new ());
    }

    void UpdateBadMarkers(List<(Day9.Vector2, bool)> badPoints)
    {
        for (int i = 0; i < badPoints.Count; i++)
        {
            SpriteRenderer instance;
            if (badMarkers.Count <= i)
            {
                instance = Instantiate(vertexPrefab);
                instance.color = Color.red;
                badMarkers.Add(instance);
            }
            else
            {
                instance = badMarkers[i];
                instance.gameObject.SetActive(true);
            }
            var point = badPoints[i].Item1;
            var isIntersection = badPoints[i].Item2;
            instance.color = isIntersection ? Color.red : Color.cyan;
            instance.transform.position = ToWorldSpace(point);
            instance.transform.localScale = Vector3.one * markerScale;
        }

        for (int i = badPoints.Count; i < badMarkers.Count; i++) badMarkers[i].gameObject.SetActive(false);

    }

    private void Update()
    {
        if (lineThickness < 0) lineThickness = 0;
        if (lineThickness != lastLineThickness)
        {
            foreach (LineRenderer item in edges) item.widthMultiplier = lineThickness;
            foreach (var item in rectEdges) item.widthMultiplier = lineThickness;
            lastLineThickness = lineThickness;
        }
        if (scale < 0f) scale = 0f;
        if (scale != lastScale) OnScaleUpdated();
    }

    void OnScaleUpdated()
    {
        var l = edges.Count;
        var center = Vector3.zero;
        for (int i = 0; i < l; i++)
        {
            var c1 = day.corners[i];
            var c2 = day.corners[(i + 1) % l];
            var v1 = new Vector3(c1.x * scale, c1.y * scale);
            var v2 = new Vector3(c2.x * scale, c2.y * scale);
            edges[i].SetPosition(0, v1);
            edges[i].SetPosition(1, v2);
            center += v1 + v2;
        }
        center /= l * 2;
        center.z = -10;
        Camera.main.transform.position = center;
        lastScale = scale;
    }

    Vector3 ToWorldSpace(Day9.Vector2 vector) => new Vector3(vector.x, vector.y) * scale;
}
