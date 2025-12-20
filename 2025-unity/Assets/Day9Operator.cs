using adventofcode_2025;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using static UnityEditor.Progress;
using Debug = UnityEngine.Debug;

public class Day9Operator : MonoBehaviour
{
    public bool useExample;
    public string inputPath;
    public string exampleInputPath;

    [Space]
    public LineRenderer edgePrefab;
    public SpriteRenderer vertexPrefab;

    [Space]
    public bool executeAsFastAsPossible = false;
    public bool playerControlled = false;
    public float updateTime = 0f;

    [Space]
    public float scale = 1f;
    public float lineThickness = 1f;
    public float markerScale = 1f;
    public float vertexScale = 1f;

    [Space]
    public ulong currentArea;

    Transform polyVertexParent;
    Transform polyEdgeParent;
    Transform vertexMarkerParent;
    Transform rectEdgeParent;

    List<LineRenderer> edges = new();
    List<SpriteRenderer> vertices = new();
    List<SpriteRenderer> badMarkers = new();

    SpriteRenderer mouseMarker;
    SpriteRenderer projectedMarker;


    LineRenderer[] rectEdges;
    //Day9.Vector2[] winningCorners;

    Day9 day = new Day9();
    float lastLineThickness = -1;
    float lastScale = -1;
    float lastVertexScale = -1;

    private IEnumerator Start()
    {
        polyVertexParent = new GameObject(nameof(polyVertexParent)).transform;
        polyEdgeParent = new GameObject(nameof(polyEdgeParent)).transform;
        vertexMarkerParent = new GameObject(nameof(vertexMarkerParent)).transform;
        rectEdgeParent = new GameObject(nameof(rectEdgeParent)).transform;


        string path = useExample ? exampleInputPath : inputPath;
        var input = Resources.Load<TextAsset>(path);


        IEnumerator<Day9.Edge[]> iterator = day.Execute(input.text);
        iterator.MoveNext();

        //var first = day.corners[217];
        //var third = day.corners[248];
        //var second = new Day9.Vector2(third.x, first.y);
        //var fourth = new Day9.Vector2(first.x, third.y);
        //winningCorners = new Day9.Vector2[] { first, second, third, fourth };

        Day9.Vector2 min = new Day9.Vector2(int.MaxValue, int.MaxValue);
        Day9.Vector2 max = new Day9.Vector2(0, 0);
        //Day9.Edge[] allEdges = new Day9.Edge[day.corners.Length];

        for (int i = 0; i < day.corners.Length; i++)
        {
            //if (i == 217 || i == 248) Debug.Log(day.corners[i]);
            var current = day.corners[i];
            var next = day.corners[(i + 1) % day.corners.Length];
            //allEdges[i] = new Day9.Edge(current, next);

            if (current.x < min.x) min.x = current.x;
            if (current.y < min.y) min.y = current.y;
            if (current.x > max.x) max.x = current.x;
            if (current.y > max.y) max.y = current.y;

            var edgeInstance = Instantiate(edgePrefab, polyEdgeParent);
            edgeInstance.name = "Edge " + new Day9.Edge(current, next).ToString();
            edges.Add(edgeInstance);
            var vertexInstance = Instantiate(vertexPrefab, polyVertexParent);
            vertexInstance.name = "Vertex " + current;
            vertices.Add(vertexInstance);
        }

        rectEdges = new LineRenderer[4];
        for (int i = 0; i < 4; i++) rectEdges[i] = Instantiate(edgePrefab, rectEdgeParent);

        OnScaleUpdated();

        //badMarkers.Add(Instantiate(vertexPrefab, vertexMarkerParent));
        //badMarkers.Add(Instantiate(vertexPrefab, vertexMarkerParent));
        //badMarkers.Add(Instantiate(vertexPrefab, vertexMarkerParent));
        //badMarkers.Add(Instantiate(vertexPrefab, vertexMarkerParent));

        mouseMarker = Instantiate(vertexPrefab);
        mouseMarker.name = "mouse marker";

        projectedMarker = Instantiate(vertexPrefab);
        projectedMarker.name = "projected mouse marker";

        rectEdgeParent.gameObject.SetActive(false);

        yield return null;

        //int count = 0;
        //int stepCount = 1;
        //for (int y = min.y; y < max.y; y += stepCount)
        //{
        //    for (int x = min.x; x < max.x; x += stepCount)
        //    {
        //        var vector = new Day9.Vector2(x, y);
        //        var pixel = Instantiate(vertexPrefab);
        //        pixel.transform.position = ToWorldSpace(vector);
        //        pixel.transform.localScale = Vector3.one * markerScale;
        //        bool inPolygon = day.TileInPolygon(day.corners, vector);
        //        if (inPolygon) Debug.Log("In polygon!");
        //        pixel.color = inPolygon ? Color.green : Color.red;

        //        if (count > 10)
        //        {
        //            yield return null;
        //            count = 0;
        //        }
        //        else count++;
        //    }
        //}

        var stopwatch = Stopwatch.StartNew();
        do
        {
            Day9.Edge[] currentEdges = iterator.Current;
            currentArea = day.currentArea;
            for (int i = 0; i < rectEdges.Length; i++)
            {
                SetEdgePositions(rectEdges[i], currentEdges[i]);
                rectEdges[i].startColor = rectEdges[i].endColor = Color.green;

            }
            if (executeAsFastAsPossible)
            {
                if (stopwatch.ElapsedMilliseconds > 500f / 60) //Half a frame when running at 60 FPS
                {
                    yield return null;
                    stopwatch.Restart();
                }
            }
            else if (playerControlled)
            {
                yield return null;
                Debug.Break();
            }
            else if (updateTime > 0) yield return new WaitForSeconds(updateTime);
            else yield return null;

        }
        while (iterator.MoveNext());
    }

    private void Update()
    {
        if (lineThickness < 0) lineThickness = 0;
        if (lineThickness != lastLineThickness)
        {

        }
        if (scale < 0f) scale = 0f;
        if (scale != lastScale || vertexScale != lastVertexScale || lineThickness != lastLineThickness) OnScaleUpdated();

        bool doPrint = Input.GetMouseButtonDown(0);
        //if ()
        {
            Vector2 pos = Input.mousePosition;
            if (doPrint) print(pos);
            pos = Camera.main.ScreenToWorldPoint(pos);
            mouseMarker.transform.position = pos;
            if (doPrint) print(pos);
            pos /= scale;
            if (doPrint) print(pos);
            var tile = new Day9.Vector2(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
            if (doPrint) print(tile);
            mouseMarker.transform.localScale = Vector3.one * vertexScale * 0.5f;

            projectedMarker.transform.position = ToWorldSpace(tile);
            projectedMarker.transform.localScale = Vector3.one * vertexScale;
            projectedMarker.color = day.TileInPolygon(day.corners, tile, doPrint) ? Color.green : Color.red;
        }
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
            SetEdgePositions(edges[i], new Day9.Edge(c1, c2));
            center += v1 + v2;

            vertices[i].transform.position = v1;
            vertices[i].transform.localScale = Vector3.one * vertexScale;
            edges[i].widthMultiplier = lineThickness;
        }
        center /= l * 2;
        center.z = -10;
        Camera.main.transform.position = center;
        lastScale = scale;
        lastVertexScale = vertexScale;

        foreach (var item in rectEdges) item.widthMultiplier = lineThickness;
        lastLineThickness = lineThickness;
    }

    void SetEdgePositions(LineRenderer edgeRenderer, Day9.Edge edge)
    {
        bool isVertical = edge.IsVertical;
        bool backwards = isVertical ? edge.Second.y < edge.First.y : edge.Second.x < edge.First.x;
        Vector3 buffer = (isVertical ? Vector3.up : Vector3.right) * lineThickness * 0.5f;
        if (backwards) buffer *= -1;
        edgeRenderer.SetPosition(0, ToWorldSpace(edge.First) - buffer);
        edgeRenderer.SetPosition(1, ToWorldSpace(edge.Second) + buffer);
    }

    Vector3 ToWorldSpace(Day9.Vector2 vector) => new Vector3(vector.x, vector.y) * scale;
}
