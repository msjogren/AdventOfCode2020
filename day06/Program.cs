using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


void Part1(string[] input)
{
    var yesQuestions = new HashSet<char>();
    var sum = 0;

    foreach (string line in input)
    {
        if (line == "")
        {
            sum += yesQuestions.Count;
            yesQuestions.Clear();
        }
        else
        {
            foreach (char c in line) yesQuestions.Add(c);
        }
    }

    sum += yesQuestions.Count;

    Console.WriteLine(sum);
}

void Part2(string[] input)
{
    var linesInGroup = new List<string>();
    var sum = 0;

    void ProcessGroup()
    {   
        if (linesInGroup.Count == 1)
        {
            sum += linesInGroup[0].Length;
        }
        else
        {
            int allYes = 0;
            for (char c = 'a'; c <= 'z'; c++)
            {
                if (linesInGroup.All(s => s.Contains(c))) allYes++;
            }
            sum += allYes;
        }
    }

    foreach (string line in input)
    {
        if (line == "")
        {
            ProcessGroup();
            linesInGroup.Clear();
        }
        else
        {
            linesInGroup.Add(line);
        }
    }

    ProcessGroup();

    Console.WriteLine(sum);
}

var input = File.ReadAllLines("input.txt");
Part1(input);
Part2(input);
