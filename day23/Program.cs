using System;
using System.Collections.Generic;
using System.Linq;

const string Input = "792845136";

void SolvePart1()
{
    var cupCircle = Input.Select(c => c - '0').ToList();
    int maxCupValue = cupCircle.Max(), minCupValue = cupCircle.Min();
    var pickedUpCups = new int[3];
    var inputLength = Input.Length;
    int currentPos = 0;

    for (int move = 0; move < 100; move++)
    {
        for (int i = 0; i < pickedUpCups.Length; i++)
        {
            pickedUpCups[i] = cupCircle[(currentPos + i + 1) % inputLength];
        }

        int destinationCupValue = cupCircle[currentPos];
        do 
        {
            destinationCupValue = destinationCupValue - 1;
            if (destinationCupValue < minCupValue) {
                destinationCupValue = maxCupValue;
            }
        } while (pickedUpCups.Contains(destinationCupValue));

        int currentVal = cupCircle[currentPos];
        foreach (int n in pickedUpCups) cupCircle.Remove(n);
        int destinationCupPos = cupCircle.IndexOf(destinationCupValue);
        foreach (int n in pickedUpCups.Reverse()) cupCircle.Insert(destinationCupPos + 1, n);
        currentPos = (cupCircle.IndexOf(currentVal) + 1) % inputLength;
    }

    int cup1Pos = cupCircle.IndexOf(1);
    for (int i = 1; i < inputLength; i++)
    {
        Console.Write(cupCircle[(cup1Pos + i) % inputLength]);
    }
    Console.WriteLine();
}

void SolvePart2()
{
    var cupCircle = new LinkedList<int>();
    var valueToCupMap = new Dictionary<int, LinkedListNode<int>>();

    for (int i = 0; i < Input.Length; i++)
    {
        int cupValue = Input[i] - '0';
        var node = cupCircle.AddLast(cupValue);
        valueToCupMap.Add(cupValue, node);
    }

    for (int n = 10; n <= 1_000_000; n++) 
    {
        var node = cupCircle.AddLast(n);
        valueToCupMap.Add(n, node);
    }

    const int MaxCupValue = 1_000_000, MinCupValue = 1;
    var pickedUpCups = new LinkedListNode<int>[3];
    var currentNode = cupCircle.First;

    for (int move = 0; move < 10_000_000; move++)
    {
        var nextPickup = currentNode.Next ?? cupCircle.First;
        for (int i = 0; i < pickedUpCups.Length; i++)
        {
            var currentPickup = nextPickup;
            pickedUpCups[i] = currentPickup;
            nextPickup = currentPickup.Next ?? cupCircle.First;
            cupCircle.Remove(currentPickup);
        }

        int destinationCupValue = currentNode.Value;
        LinkedListNode<int> destinationCupNode;
        do 
        {
            destinationCupValue = destinationCupValue - 1;
            if (destinationCupValue < MinCupValue) destinationCupValue = MaxCupValue;
            destinationCupNode = valueToCupMap[destinationCupValue];
        } while (pickedUpCups.Contains(destinationCupNode));

        foreach (var node in pickedUpCups.Reverse()) cupCircle.AddAfter(destinationCupNode, node);

        currentNode = currentNode.Next ?? cupCircle.First;
    }

    var factor1Node = valueToCupMap[1].Next ?? cupCircle.First;
    var factor2Node = factor1Node.Next ?? cupCircle.First;
    Console.WriteLine($"{(long)factor1Node.Value * factor2Node.Value}");
}

SolvePart1();
SolvePart2();