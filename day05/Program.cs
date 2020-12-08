using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


int SeatSpecToSeatID(string seatSpec)
{
    int seatID = 0;
    foreach (char c in seatSpec)
    {
        seatID = (seatID << 1) | ((c == 'B' || c == 'R') ? 1 : 0);
    }
    return seatID;
}


// Part 1
int highestSeatID = -1;
var takenSeats = new List<int>();

foreach (string line in File.ReadAllLines("input.txt"))
{
    int seatID = SeatSpecToSeatID(line);
    if (seatID > highestSeatID) highestSeatID = seatID;
    takenSeats.Add(seatID);
}

Console.WriteLine(highestSeatID);

// Part 2
takenSeats.Sort();

int firstRow = takenSeats.First() >> 3;
int lastRow = takenSeats.Last() >> 3;

for (int row = firstRow + 1; row < lastRow; row++)
{
    for (int col = 0; col < 8; col++)
    {
        int seatID = row * 8 + col;
        if (takenSeats.BinarySearch(seatID) < 0) Console.WriteLine($"Row {row} col {col} seat ID {seatID}");
    }
}
