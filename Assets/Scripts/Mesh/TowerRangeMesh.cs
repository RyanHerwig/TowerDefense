using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TowerRangeMesh : MonoBehaviour
{
    //mesh properties
    Mesh mesh;
    Vector3[] polygonPoints;
    int[] polygonTriangles;

    [NonSerialized] public bool HasMinimumRange;
    [NonSerialized] public float MaxRange;
    [NonSerialized] public float MinimumRange;

    Tower tower;
    int polygonSides = 16;

    public void Init()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        tower = GetComponentInParent<Tower>();

        MaxRange = tower.MaxRange;
        MinimumRange = tower.MinRange;
        HasMinimumRange = true;

        if (HasMinimumRange)
        {
            DrawRangeWithMinimum(polygonSides, MaxRange, MinimumRange);
        }
        else
        {
            DrawRange(polygonSides, MaxRange);
        }
    }

    public void ShowTowerRange()
    {
        gameObject.SetActive(true);
    }

    public void HideTowerRange()
    {
        gameObject.SetActive(false);
    }

    public void ToggleTowerRange()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void UpdateTowerRange()
    {
        if (HasMinimumRange)
        {
            DrawRangeWithMinimum(polygonSides, MaxRange, MinimumRange);
        }
        else
        {
            DrawRange(polygonSides, MaxRange);
        }
    }

    void DrawRange(int sides, float radius)
    {
        polygonPoints = GetCircumferencePoints(sides, radius).ToArray();
        polygonTriangles = DrawFilledTriangles(polygonPoints);
        
        mesh.Clear();
        mesh.vertices = polygonPoints;
        mesh.triangles = polygonTriangles;
        mesh.Optimize();
    }

    void DrawRangeWithMinimum(int sides, float outerRadius, float innerRadius)
    {
        List<Vector3> pointsList = new();
        List<Vector3> outerPoints = GetCircumferencePoints(sides, outerRadius);
        pointsList.AddRange(outerPoints);
        List<Vector3> innerPoints = GetCircumferencePoints(sides, innerRadius);
        pointsList.AddRange(innerPoints);

        polygonPoints = pointsList.ToArray();
        polygonTriangles = DrawHollowTriangles(polygonPoints);

        mesh.Clear();
        mesh.vertices = polygonPoints;
        mesh.triangles = polygonTriangles;
        mesh.Optimize();
    }

    int[] DrawFilledTriangles(Vector3[] points)
    {
        int triangleAmount = points.Length - 2;
        List<int> newTriangles = new();
        for (int i = 0; i < triangleAmount; i++)
        {
            newTriangles.Add(0);
            newTriangles.Add(i + 2);
            newTriangles.Add(i + 1);
        }
        return newTriangles.ToArray();
    }

    int[] DrawHollowTriangles(Vector3[] points)
    {
        int sides = points.Length / 2;

        List<int> newTriangles = new();
        for (int i = 0; i < sides; i++)
        {
            int outerIndex = i;
            int innerIndex = i + sides;

            //first triangle starting at outer edge i
            newTriangles.Add(outerIndex);
            newTriangles.Add(innerIndex);
            newTriangles.Add((i + 1) % sides);

            //second triangle starting at outer edge i
            newTriangles.Add(outerIndex);
            newTriangles.Add(sides + ((sides + i - 1) % sides));
            newTriangles.Add(outerIndex + sides);
        }
        return newTriangles.ToArray();
    }

    List<Vector3> GetCircumferencePoints(int sides, float radius)
    {
        List<Vector3> points = new();
        float circumferenceProgressPerStep = (float)1 / sides;
        float TAU = 2 * Mathf.PI;
        float radianProgressPerStep = circumferenceProgressPerStep * TAU;

        for (int i = 0; i < sides; i++)
        {
            float currentRadian = radianProgressPerStep * i;
            points.Add(new Vector3(Mathf.Cos(currentRadian) * radius, 0, Mathf.Sin(currentRadian) * radius));
        }
        return points;
    }
}