using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

List<int> adapters = File.ReadAllLines("input.txt").Select(line => int.Parse(line)).ToList();
adapters.Sort();
adapters.Insert(0, 0);
adapters.Add(adapters.Last() + 3);

// Part 1
var diffs = adapters.Skip(1).Zip(adapters).Select(pair => pair.First - pair.Second);
int ones = diffs.Count(i => i == 1), threes = diffs.Count(i => i == 3);
Console.WriteLine($"{ones} * {threes} = {ones * threes}");

// Part 2
long combinations = 1;
for (int i = 0; i < adapters.Count; i++)
{
    if (i + 4 < adapters.Count && (adapters[i] + 4) == adapters[i + 4])
    {
        // Longest sequence in input. 5 in a row with diff == 1, eg 10, 11, 12, 13, 14. 
        // 7 ways:
        // 10, 11, 12, 13, 14
        // 10, 11, 12,     14
        // 10, 11,     13, 14
        // 10,     12, 13, 14
        // 10, 11,         14
        // 10,     12,     14
        // 10,         13, 14
        combinations *= 7;
        i += 4;
    }
    else if (i + 3 < adapters.Count && (adapters[i] + 3) == adapters[i + 3])
    {
        // 4 in a row, eg 10, 11, 12, 13
        // 4 ways:
        // 10, 11, 12, 13
        // 10, 11,     13  
        // 10,     12, 13
        // 10,         13
        combinations *= 4;
        i += 3;
    }
    else if (i + 2 < adapters.Count && (adapters[i] + 2) == adapters[i + 2])
    {
        // 3 in a row, eg 10, 11, 12
        // 2 ways:
        // 10, 11, 12
        // 10,     12 
        combinations *= 2;
        i += 2;
    }
}

Console.WriteLine(combinations);
