using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var nodes = new Dictionary<string, List<(string color, int count)>>();

long CountContainedBags(string color) => 1 + nodes[color].Sum(kvp => kvp.count * CountContainedBags(kvp.color));


foreach (string line in File.ReadAllLines("input.txt"))
{
    int bagsContainPos = line.IndexOf(" bags contain ");
    string key = line.Substring(0, bagsContainPos);
    string contained = line.Substring(bagsContainPos + 14);
    var containedList = new List<(string color, int count)>();
    nodes.Add(key, containedList);

    if (contained == "no other bags.") continue;

    foreach (string containedBag in contained.Split(", "))
    {
        int firstSpacePos = containedBag.IndexOf(' ');
        int bagPos = containedBag.IndexOf(" bag");
        int count = int.Parse(containedBag.Substring(0, firstSpacePos));
        string containedKey = containedBag.Substring(firstSpacePos + 1, bagPos - firstSpacePos - 1);
        containedList.Add((containedKey, count));
    }
}

var knownShinyGoldContainers = new HashSet<string>();
var colorQueue = new Queue<string>();
colorQueue.Enqueue("shiny gold");
while (colorQueue.TryDequeue(out string currentColor))
{
    foreach (var kvp in nodes)
    {
        if (knownShinyGoldContainers.Contains(kvp.Key)) continue;

        if (kvp.Value.Any(t => t.color == currentColor))
        {
            knownShinyGoldContainers.Add(kvp.Key);
            colorQueue.Enqueue(kvp.Key);
        }
    }
}

// Part 1
Console.WriteLine(knownShinyGoldContainers.Count);

// Part 2
long containedInShinyGold = CountContainedBags("shiny gold") - 1;
Console.WriteLine(containedInShinyGold);
