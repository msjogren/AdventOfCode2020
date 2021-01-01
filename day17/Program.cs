using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


var active3D = new HashSet<(int x, int y, int z)>();
var active4D = new HashSet<(int x, int y, int z, int w)>();

int inputY = 0;
foreach (string line in File.ReadAllLines("input.txt"))
{
    for (int inputX = 0; inputX < line.Length; inputX++)
    {
        if (line[inputX] == '#')
        {
            active3D.Add((inputX, inputY, 0));
            active4D.Add((inputX, inputY, 0, 0));
        }
    }
    inputY++;
}

// Part 1
for (int round = 1; round <= 6; round++)
{
    int fromx = active3D.Min(pt => pt.x) - 1;
    int tox = active3D.Max(pt => pt.x) + 1;
    int fromy = active3D.Min(pt => pt.y) - 1;
    int toy = active3D.Max(pt => pt.y) + 1;
    int fromz = active3D.Min(pt => pt.z) - 1;
    int toz = active3D.Max(pt => pt.z) + 1;

    var changedset = new HashSet<(int x, int y, int z)>();

    for (int z = fromz; z <= toz; z++)
    {
        for (int y = fromy; y <= toy; y++)
        {
            for (int x = fromx; x <= tox; x++)
            {
                var point = (x, y, z);
                int activeNeighbors = GetAdjacent3D(point).Count(adjpt => active3D.Contains(adjpt));
                bool wasActive = active3D.Contains(point);
                bool shouldBeActive = activeNeighbors switch {
                    2 or 3 when wasActive => true,
                    3 when !wasActive => true,
                    _ => false
                };
                if (shouldBeActive) changedset.Add(point);
            }
        }
    }

    active3D = changedset;
}

Console.WriteLine(active3D.Count);

// Part 2
for (int round = 1; round <= 6; round++)
{
    int fromx = active4D.Min(pt => pt.x) - 1;
    int tox = active4D.Max(pt => pt.x) + 1;
    int fromy = active4D.Min(pt => pt.y) - 1;
    int toy = active4D.Max(pt => pt.y) + 1;
    int fromz = active4D.Min(pt => pt.z) - 1;
    int toz = active4D.Max(pt => pt.z) + 1;
    int fromw = active4D.Min(pt => pt.w) - 1;
    int tow = active4D.Max(pt => pt.w) + 1;

    var changedset = new HashSet<(int x, int y, int z, int w)>();

    for (int w = fromw; w <= tow; w++)
    {
        for (int z = fromz; z <= toz; z++)
        {
            for (int y = fromy; y <= toy; y++)
            {
                for (int x = fromx; x <= tox; x++)
                {
                    var point = (x, y, z, w);
                    int activeNeighbors = GetAdjacent4D(point).Count(adjpt => active4D.Contains(adjpt));
                    bool wasActive = active4D.Contains(point);
                    bool shouldBeActive = activeNeighbors switch {
                        2 or 3 when wasActive => true,
                        3 when !wasActive => true,
                        _ => false
                    };
                    if (shouldBeActive) changedset.Add(point);
                }
            }
        }
    }

    active4D = changedset;
}

Console.WriteLine(active4D.Count);


IEnumerable<(int x, int y, int z)> GetAdjacent3D((int x, int y, int z) point)
{
    for (int dz = -1; dz <= 1; dz++)
    {
        for (int dy = -1; dy <= 1; dy++)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                if (dx != 0 || dy != 0 || dz != 0) yield return (point.x + dx, point.y + dy, point.z + dz);
            }
        }
    }
}

IEnumerable<(int x, int y, int z, int w)> GetAdjacent4D((int x, int y, int z, int w) point)
{
    for (int dw = -1; dw <= 1; dw++)
    {
        for (int dz = -1; dz <= 1; dz++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (dx != 0 || dy != 0 || dz != 0 || dw != 0) yield return (point.x + dx, point.y + dy, point.z + dz, point.w + dw);
                }
            }
        }
    }
}
