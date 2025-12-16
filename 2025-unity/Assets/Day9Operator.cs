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
    public GameObject vertexPrefab;

    public float scale = 1f;
    public float updateTime = 0f;
    public float lineThickness = 1f;

    List<LineRenderer> edges = new List<LineRenderer>();

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

        for (int i = 0; i < day.corners.Length; i++)
        {
            edges.Add(Instantiate(edgePrefab));
        }

        rectEdges = new LineRenderer[4];
        for (int i = 0; i < 4; i++) rectEdges[i] = Instantiate(edgePrefab);

        OnScaleUpdated();

        do
        {
            Day9.Edge[] currentEdges = iterator.Current;
            for (int i = 0; i < rectEdges.Length; i++)
            {
                var c1 = currentEdges[i].First;
                var c2 = currentEdges[i].Second;
                rectEdges[i].SetPosition(0, new Vector3(c1.x * scale, c1.y * scale));
                rectEdges[i].SetPosition(1, new Vector3(c2.x * scale, c2.y * scale));

            }
            yield return new WaitForSeconds(updateTime);
        }
        while (iterator.MoveNext());
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
}
