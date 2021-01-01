using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

(int start, int end) ParseRange(string rangeStr)
{
    int dashPos = rangeStr.IndexOf('-');
    return (int.Parse(rangeStr[..dashPos]), int.Parse(rangeStr[(dashPos+1)..]));
}

var rules = new List<Rule>();
var tickets = new List<int[]>();
int[] myTicket = null;
int parseSection = 0;
foreach (string line in File.ReadAllLines("input.txt"))
{
    if (line == "")
    {
        parseSection++;
        continue;
    }

    switch (parseSection)
    {
        case 0:
            int colonPos = line.IndexOf(':');
            var ranges = line[(colonPos+1)..].Split(" or ");
            rules.Add(new Rule() {
                Name = line[..colonPos],
                FirstRange = ParseRange(ranges[0]), 
                SecondRange = ParseRange(ranges[1])
            });
            break;

        case 1:
            if (line.StartsWith("your")) continue;
            myTicket = line.Split(',').Select(n => int.Parse(n)).ToArray();
            break;

        case 2:
            if (line.StartsWith("nearby")) continue;
            tickets.Add(line.Split(',').Select(n => int.Parse(n)).ToArray());
            break;
    }
}

// Part 1
int scanningErrorRate = tickets.SelectMany(t => t).Where(n => !rules.Any(r => r.InRange(n))).Sum();
Console.WriteLine(scanningErrorRate);

// Part 2
tickets.RemoveAll(t => t.Any(n => !rules.Any(r => r.InRange(n))));

var columnToRuleMap = new Dictionary<int, Rule>();
var availableRules = new List<Rule>(rules);
var numbersByColumn = tickets.SelectMany(t => t.Select((n, idx) => (idx, n))).GroupBy(tpl => tpl.idx, tpl => tpl.n).OrderBy(grp => grp.Key);
var unassignedColumns = Enumerable.Range(0, rules.Count).ToList();

while (unassignedColumns.Any())
{
    foreach (int col in unassignedColumns)
    {
        var possibleRules = availableRules.Where(rule => numbersByColumn.Single(g => g.Key == col).All(n => rule.InRange(n)));
        if (possibleRules.Count() == 1)
        {
            Rule rule = possibleRules.First();
            columnToRuleMap.Add(col, rule);
            availableRules.Remove(rule);
            unassignedColumns.Remove(col);
            break;
        } 
    }
}

long departureFields = columnToRuleMap.Where(kvp => kvp.Value.Name.Contains("departure")).Aggregate(1L, (product, kvp) => product * myTicket[kvp.Key]);
Console.WriteLine(departureFields);


class Rule
{
    public string Name;
    public (int start, int end) FirstRange, SecondRange;

    public bool InRange(int n) => (n >= FirstRange.start && n <= FirstRange.end) || (n >= SecondRange.start && n <= SecondRange.end);
}