using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var input = File.ReadAllLines("input.txt");
var rawRules = new SortedDictionary<int, string>();
var messages = new List<string>();
foreach (string line in input)
{
    if (line.Length == 0)
        continue;
    else if (Char.IsDigit(line[0]))
        rawRules.Add(int.Parse(line.Substring(0, line.IndexOf(':'))), line.Substring(line.IndexOf(' ') + 1));
    else
        messages.Add(line);
}

var rules = rawRules.Values.Select(line => new Rule(line)).ToList();

// Part 1
var rule0Matches = new HashSet<string>(rules[0].GetMatches(rules));
Console.WriteLine(messages.Count(m => rule0Matches.Contains(m))); 

// Part 2

// Observation #1:
// After rule rewrite to recursive, we have
// 0: 8 11
// 8: 42 | 42 8
// 11: 42 31 | 42 11 31
//
// Rules 8 and 11 are only ever referenced from rule 0.
// The combined rule 0 becomes
// (42)*(m) (31)*(n) where m > n
//
// Observation #2:
// All matches for rules 31 and 42 have the same length (8).
var rule42Matches = new HashSet<string>(rules[42].GetMatches(rules));
var rule31Matches = new HashSet<string>(rules[31].GetMatches(rules));
int rule42Length = rule42Matches.First().Length;
int rule31Length = rule31Matches.First().Length;

int part2Matches = 0;
foreach (string msg in messages)
{
    int rule42Count = 0, rule31Count = 0;
    string remaining = msg;

    while (remaining.Length >= rule31Length && rule31Matches.Contains(remaining[^rule31Length..]))
    {
        remaining = remaining[..^rule31Length];
        rule31Count++;
    }

    if (rule31Count == 0) continue;

    while (remaining.Length >= rule42Length && rule42Matches.Contains(remaining[..rule42Length]))
    {
        remaining = remaining[rule42Length..];
        rule42Count++;
    }

    if (remaining.Length == 0 && rule42Count > rule31Count) part2Matches++;
}

Console.WriteLine(part2Matches);

class Rule
{
    private string _def;
    private IEnumerable<string> _matches;
    public Rule(string def)
    {
        _def = def;
    }

    public IEnumerable<string> GetMatches(IList<Rule> rules, int rule = 0) => _matches ??= EnumerateMatches(rules, rule).ToList();

    private IEnumerable<string> EnumerateMatches(IList<Rule> rules, int rule)
    {
        if (_def[0] == '\"')
        {
            yield return _def.Substring(1, 1);
            yield break;
        }
        else
        {
            foreach (string subrule in _def.Split(" | "))
            {
                var ruleRefs = subrule.Split(' ').Select(n => int.Parse(n)).ToArray();
                var permutations = new List<string>(rules[ruleRefs[0]].GetMatches(rules, ruleRefs[0]));
                foreach (var rr in ruleRefs.Skip(1))
                {
                    var newpermutations = new List<string>();
                    foreach (string lhs in permutations)
                    {
                        foreach (string rhs in rules[rr].GetMatches(rules, rr))
                        {
                            newpermutations.Add(lhs + rhs);
                        }
                    }
                    permutations = newpermutations;
                }

                foreach (string match in permutations)
                {
                    yield return match;
                }
            }
        }
    }
}
