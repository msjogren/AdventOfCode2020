using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

List<int> player1StartingDeck = new(), player2StartingDeck = new(), currentDeck = player1StartingDeck;
var input = File.ReadAllLines("input.txt");

foreach (string line in input)
{
    if (line == "Player 2:")
    {
        currentDeck = player2StartingDeck;
    }
    else if (line.Length > 0 && Char.IsDigit(line[0]))
    {
        currentDeck.Add(int.Parse(line));
    }
}

long DeckScore(IEnumerable<int> deck) => 
    deck.Reverse<int>().Select((card, idx) => (card, idx)).Aggregate(0L, (sum, card) => sum + card.card * (1 + card.idx));

// Part 1
List<int> part1player1Deck = new(player1StartingDeck), part1player2Deck = new(player2StartingDeck);
while (part1player1Deck.Count > 0  && part1player2Deck.Count > 0)
{
    int player1Card = part1player1Deck[0];
    int player2Card = part1player2Deck[0];
    part1player1Deck.RemoveAt(0);
    part1player2Deck.RemoveAt(0);
    var winningDeck = player1Card > player2Card ? part1player1Deck : part1player2Deck;
    winningDeck.Add(Math.Max(player1Card, player2Card));
    winningDeck.Add(Math.Min(player1Card, player2Card));
}

var part1Winner = part1player1Deck.Count == 0 ? part1player2Deck : part1player1Deck;
Console.WriteLine(DeckScore(part1Winner));


// Part 2
(int winner, List<int> winnerDeck) PlayPart2Game(List<int> currentPlayer1Deck, List<int> currentPlayer2Deck)
{
    List<List<int>> player1History = new(), player2History = new();

    while (true)
    {
        if (player1History.Any(deck => deck.SequenceEqual(currentPlayer1Deck)) ||
            player2History.Any(deck => deck.SequenceEqual(currentPlayer2Deck)))
        {
            return (1, currentPlayer1Deck);
        }

        player1History.Add(currentPlayer1Deck);
        player2History.Add(currentPlayer2Deck);
        int player1Card = currentPlayer1Deck[0];
        int player2Card = currentPlayer2Deck[0];
        int roundWinner;

        currentPlayer1Deck = currentPlayer1Deck.Skip(1).ToList();
        currentPlayer2Deck = currentPlayer2Deck.Skip(1).ToList();

        if (currentPlayer1Deck.Count >= player1Card && currentPlayer2Deck.Count >= player2Card)
        {
            List<int> nextPlayer1Deck = new(currentPlayer1Deck.Take(player1Card)), nextPlayer2Deck = new(currentPlayer2Deck.Take(player2Card));
            roundWinner = PlayPart2Game(nextPlayer1Deck, nextPlayer2Deck).winner;
        }
        else
        {
            roundWinner = player1Card > player2Card ? 1 : 2;
        }

        if (roundWinner == 1)
        {
            currentPlayer1Deck.Add(player1Card);
            currentPlayer1Deck.Add(player2Card);
            if (currentPlayer2Deck.Count == 0) return (1, currentPlayer1Deck);
        }
        else
        {
            currentPlayer2Deck.Add(player2Card);
            currentPlayer2Deck.Add(player1Card);
            if (currentPlayer1Deck.Count == 0) return (2, currentPlayer2Deck);
        }
    }
}

var part2Winner = PlayPart2Game(player1StartingDeck, player2StartingDeck).winnerDeck;
Console.WriteLine(DeckScore(part2Winner));