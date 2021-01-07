using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

IEnumerable<string> ParseMoves(string line)
{
    for (int i = 0, j = 1; i < line.Length; i++, j++)
    {
        char ch1 = line[i], ch2 = j < line.Length ? line[j] : ' ';

        string move = ch1 switch {
            's' when ch2 == 'e' => "se",
            's' when ch2 == 'w' => "sw",
            'n' when ch2 == 'e' => "ne",
            'n' when ch2 == 'w' => "nw",
            'e' => "e",
            'w' => "w",
            _ => ""
        };

        if (move.Length == 2) { i++; j++; };
        yield return move;
    }
}

(int x, int y) CoordinateForMoves(IEnumerable<string> moves)
{
    var result = (x: 0, y: 0);

    foreach (string move in moves)
    {
        result = move switch {
            "e"  => (result.x + 2, result.y),
            "w"  => (result.x - 2, result.y),
            "nw" => (result.x - 1, result.y - 2),
            "ne" => (result.x + 1, result.y - 2),
            "sw" => (result.x - 1, result.y + 2),
            "se" => (result.x + 1, result.y + 2),
            _ => throw new InvalidOperationException()
        };
    }

    return result;
}

IEnumerable<(int x, int y)> GetAdjacentTiles(int x, int y)
{
    yield return (x - 1, y - 2);
    yield return (x + 1, y - 2);
    yield return (x - 2, y);
    yield return (x + 2, y);
    yield return (x - 1, y + 2);
    yield return (x + 1, y + 2);
}

var input = File.ReadAllLines("input.txt");

// Part 1
var flips = new Dictionary<(int x, int y), int>();

foreach (string line in input)
{
    var coords = CoordinateForMoves(ParseMoves(line));
    if (!flips.TryGetValue(coords, out int flipped)) flips[coords] = 0;
    flips[coords]++;
}

Console.WriteLine(flips.Values.Count(v => v % 2 == 1));

// Part 2
var blackTiles = new HashSet<(int x, int y)>(flips.Where(kvp => (kvp.Value % 2) == 1).Select(kvp => kvp.Key));
for (int day = 0; day < 100; day++)
{
    var newBlackTiles = new HashSet<(int x, int y)>(blackTiles);
    var tilesToEvaluate = blackTiles.Union(blackTiles.SelectMany(tile => GetAdjacentTiles(tile.x, tile.y))).Distinct();
    foreach (var tile in tilesToEvaluate)
    {
        bool wasBlack = blackTiles.Contains(tile);
        int adjacentBlackTiles = GetAdjacentTiles(tile.x, tile.y).Count(a => blackTiles.Contains(a));
        if (wasBlack && (adjacentBlackTiles == 0 || adjacentBlackTiles > 2))
        {
            newBlackTiles.Remove(tile);
        }
        else if (!wasBlack && adjacentBlackTiles == 2)
        {
            newBlackTiles.Add(tile);
        }
    }

    blackTiles = newBlackTiles;
}

Console.WriteLine(blackTiles.Count);