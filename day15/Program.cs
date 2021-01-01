using System;
using System.Collections.Generic;
using System.Linq;

const string Input = "13,0,10,12,1,5,8";

IEnumerable<int> GameNumberSequence()
{
    var turnLastSeen = new Dictionary<int, int>();
    int prevNum = -1;
    int turn = 0;

    foreach (int startingNum in Input.Split(',').Select(s => int.Parse(s)))
    {
        turnLastSeen.Add(startingNum, ++turn);
        prevNum = startingNum;
        yield return startingNum;
    }

    while (true)
    {
        int prevTurn = turn++;
        bool seenBefore = turnLastSeen.TryGetValue(prevNum, out int lastTurn);
        turnLastSeen[prevNum] = prevTurn;
        prevNum = seenBefore ? prevTurn - lastTurn : 0;
        yield return prevNum;
    }
}

var gameSequence = GameNumberSequence();
Console.WriteLine(gameSequence.ElementAt(2019));
Console.WriteLine(gameSequence.ElementAt(29999999));


