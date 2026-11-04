using System.Collections.Generic;
using UnityEngine;

public class ArenaLayout : MonoBehaviour
{
    private readonly List<Bounds> walls = new List<Bounds>();
    private const int Width = 27;
    private readonly int[] previous = new int[Width * Width];
    private readonly Queue<int> frontier = new Queue<int>();

    public void Build(int level, Material material)
    {
        AddWall(new Vector3(-5f, 1.5f, 3f), new Vector3(1f, 3f, 6f), material);
        AddWall(new Vector3(5f, 1.5f, -3f), new Vector3(1f, 3f, 6f), material);
        if (level >= 1)
        {
            AddWall(new Vector3(-5f, 1.5f, -6f), new Vector3(6f, 3f, 1f), material);
            AddWall(new Vector3(5f, 1.5f, 6f), new Vector3(6f, 3f, 1f), material);
        }
        if (level >= 2)
        {
            AddWall(new Vector3(-10f, 1.5f, 0f), new Vector3(1f, 3f, 8f), material);
            AddWall(new Vector3(10f, 1.5f, 0f), new Vector3(1f, 3f, 8f), material);
            AddWall(new Vector3(0f, 1.5f, 10f), new Vector3(6f, 3f, 1f), material);
            AddWall(new Vector3(0f, 1.5f, -10f), new Vector3(6f, 3f, 1f), material);
        }
    }

    private void AddWall(Vector3 position, Vector3 size, Material material)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "Arena Wall";
        wall.transform.SetParent(transform);
        wall.transform.position = position;
        wall.transform.localScale = size;
        wall.GetComponent<Renderer>().sharedMaterial = material;
        walls.Add(new Bounds(position, size));
    }

    public bool IsClear(Vector3 position, float radius)
    {
        if (Mathf.Abs(position.x) > 13f || Mathf.Abs(position.z) > 13f) return false;
        foreach (Bounds wall in walls)
        {
            if (Mathf.Abs(position.x - wall.center.x) < wall.extents.x + radius &&
                Mathf.Abs(position.z - wall.center.z) < wall.extents.z + radius)
                return false;
        }
        return true;
    }

    private static int Cell(Vector3 position)
    {
        int x = Mathf.Clamp(Mathf.RoundToInt(position.x) + 13, 0, 26);
        int z = Mathf.Clamp(Mathf.RoundToInt(position.z) + 13, 0, 26);
        return z * Width + x;
    }

    private static Vector3 Position(int cell) => new Vector3(cell % Width - 13, 0.65f, cell / Width - 13);

    // A small grid search lets chasers go around the new arena walls.
    public Vector3 NextStep(Vector3 startPosition, Vector3 targetPosition)
    {
        int start = Cell(startPosition);
        int goal = Cell(targetPosition);
        if (start == goal) return new Vector3(targetPosition.x, 0.65f, targetPosition.z);
        System.Array.Fill(previous, -1);
        frontier.Clear();
        frontier.Enqueue(start);
        previous[start] = start;
        int closest = start;
        float distance = (Position(start) - targetPosition).sqrMagnitude;
        while (frontier.Count > 0)
        {
            int cell = frontier.Dequeue();
            float candidate = (Position(cell) - targetPosition).sqrMagnitude;
            if (candidate < distance) { closest = cell; distance = candidate; }
            if (cell == goal) { closest = goal; break; }
            int x = cell % Width;
            int z = cell / Width;
            if (x > 0) Visit(cell - 1, cell);
            if (x < Width - 1) Visit(cell + 1, cell);
            if (z > 0) Visit(cell - Width, cell);
            if (z < Width - 1) Visit(cell + Width, cell);
        }
        if (closest == start) return startPosition;
        while (previous[closest] != start) closest = previous[closest];
        return Position(closest);
    }

    private void Visit(int next, int from)
    {
        if (previous[next] != -1 || !IsClear(Position(next), 0.7f)) return;
        previous[next] = from;
        frontier.Enqueue(next);
    }
}
