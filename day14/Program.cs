using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

string ParseMask(string inputLine) => inputLine[7..];

(long addr, long num) ParseWrite(string inputLine)
{
    int startBracket = inputLine.IndexOf('['), endBracket = inputLine.IndexOf(']');
    long addr = long.Parse(inputLine[(startBracket+1)..endBracket]);
    long num = long.Parse(inputLine[(endBracket+4)..]);
    return (addr, num);
}

var input = File.ReadAllLines("input.txt");
var mem = new Dictionary<long, long>();

// Part 1
long zerosMask = 0, onesMask = 0;

foreach (string line in input)
{
    if (line.StartsWith("mask"))
    {
        string maskStr = ParseMask(line);
        zerosMask = Convert.ToInt64(maskStr.Replace('1', '0').Replace('X', '1'), 2);
        onesMask = Convert.ToInt64(maskStr.Replace('X', '0'), 2);
    }
    else
    {
        var w = ParseWrite(line);
        mem[w.addr] = (w.num & zerosMask) | onesMask;
    }
}

long sum = mem.Values.Sum();
Console.WriteLine(sum);

// Part 2
IEnumerable<long> GetAddressPermutations(long baseAddr, IEnumerable<int> floatingBits)
{
    int bit = floatingBits.First();
    long mask = 1L << bit;
    if (floatingBits.Count() == 1)
    {
        yield return baseAddr & ~mask;
        yield return baseAddr | mask;
    }
    else
    {
        foreach (long addr in GetAddressPermutations(baseAddr, floatingBits.Skip(1)))
        {
            yield return addr & ~mask;
            yield return addr | mask;
        }
    }
}

mem = new Dictionary<long, long>();
IEnumerable<int> floatingBitPositions = null;
onesMask = 0;

foreach (string line in input)
{
    if (line.StartsWith("mask"))
    {
        string maskStr = ParseMask(line);
        onesMask = Convert.ToInt64(maskStr.Replace('X', '0'), 2);
        floatingBitPositions = maskStr.Reverse().Select((bit, idx) => (bit, idx)).Where(t => t.bit == 'X').Select(t => t.idx);
    }
    else
    {
        (long addr, long num) = ParseWrite(line);        
        foreach (var address in GetAddressPermutations(addr | onesMask, floatingBitPositions))
        {
            mem[address] = num;
        }
    }
}

sum = mem.Values.Sum();
Console.WriteLine(sum);
