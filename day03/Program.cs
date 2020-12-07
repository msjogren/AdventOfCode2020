using System;
using System.IO;


int GetTreesForSlope(string[] lines, int dx, int dy)
{
    int trees = 0, w = lines[0].Length;
    for (int y = 0, x = 0; y < lines.Length; y += dy, x += dx)
    {
        char ch = lines[y][x % w];
        if (ch == '#') trees++;
    }

    return trees;
}


string[] lines = File.ReadAllLines("input.txt");

// Part 1
Console.WriteLine(GetTreesForSlope(lines, 3, 1));

// Part 2
long p2Result = 1;
foreach (var slope in new [] {(1, 1), (3, 1), (5, 1), (7, 1), (1, 2)})
{
    p2Result *= GetTreesForSlope(lines, slope.Item1, slope.Item2);
}
Console.WriteLine(p2Result);
