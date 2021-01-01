using System;
using System.Linq;
using System.Numerics;

const int DepartureTime = 1001287;
const string Input = "13,x,x,x,x,x,x,37,x,x,x,x,x,461,x,x,x,x,x,x,x,x,x,x,x,x,x,17,x,x,x,x,19,x,x,x,x,x,x,x,x,x,29,x,739,x,x,x,x,x,x,x,x,x,41,x,x,x,x,x,x,x,x,x,x,x,x,23";

// Part 1
int[] schedules = Input.Split(',').Where(s => s != "x").Select(s => int.Parse(s)).ToArray();
int earliestTime = int.MaxValue, earliestBusId = -1;

foreach (int bus in schedules)
{
    var before = DepartureTime / bus;
    var next = (before + 1) * bus;
    if (next < earliestTime)
    {
        earliestTime = next;
        earliestBusId = bus;
    }
}

Console.WriteLine($"{earliestBusId} * {earliestTime - DepartureTime} = {earliestBusId * (earliestTime - DepartureTime)}");


// Part 2

var schedules2 = Input
    .Split(',')
    .Select((busstr, idx) => (busstr, idx))
    .Where(tpl => tpl.busstr != "x")
    .Select(tpl => (busid: int.Parse(tpl.busstr), offset: tpl.idx))
    .OrderByDescending(t => t.busid)
    .ToArray();

// For each bus with id Bn that leaves Mn minutes after t we get an equation
//   t % Bn == Bn - Mn
// This gives an equation system that can be solved according to Chinese Remainder Theorem
// since the IDs happen to be coprime.
// Thanks to https://brilliant.org/wiki/chinese-remainder-theorem/

// First equation 
// t ≡ (B1 - M1)    (mod B1)        
// rewrites to
// t = a*j + b     for some integer j where a = B1 and b = (B1 - M1)
var first = schedules2[0];
BigInteger a = first.busid;
BigInteger b = first.busid - first.offset;

for (int i = 1; i < schedules2.Length; i++)
{
    var equation = schedules2[i];

    // Next equation
    // t ≡ (Bn - Mn)    (mod Bn)
    // and substitute the previous equation for t
    // a*j + b ≡ (Bn - Mn)    (mod Bn)
    // or
    // a*j + b ≡ c            (mod d) where c = (Bn - Mn) and d = Bn

    BigInteger c = equation.busid - equation.offset;
    BigInteger d = equation.busid;
    
    // Solve for j
    // (a*j + b) mod d = c mod d
    //   =>
    // a*j mod d = (c - b) mod d
    BigInteger rhs = (c - b) % d;

    // j ≡ a^-1 * (c - b)           (mod d)
    // j ≡ e                        (mod d)
    // rewrites to
    // j = d*k + e                  for some integer k
    // Note: a^-1 ≡ a^(m-2)  (mod m) when m is prime (Euler's theorem)
    BigInteger e = (BigInteger.ModPow(a, d-2, d) * (c - b)) % d;
    if (e < 0) e = (e + d) % d;

    if (i == schedules2.Length - 1)
    {
        // Last equation
        // j = d*k + e
        // into last equation
        // t = a*j + b
        // t = a*(d*k + e) + b = a*d*k + a*e + b
        // t = (a*d)*k + (a*e + b)
        // or
        // t ≡ (a*e + b)    (mod (a*d))
        Console.WriteLine((a * e + b) % (a * d));
    }
    else
    {
        // back into equation above
        // t = a*j + b
        // t = a*(d*k + e) + b
        // t = (a*d)*k + (a*e + b)

        b = a * e + b;
        a = a * d;
    }
}