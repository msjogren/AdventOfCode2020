using System;
using System.IO;
using System.Linq;

const int PreambleLength = 25;
var numbers = File.ReadAllLines("input.txt").Select(line => long.Parse(line)).ToArray();

bool IsValid(long n, int i)
{
    for (int j = i - PreambleLength; j < i - 1; j++)
    {
        for (int k = j + 1; k < i; k++)
        {
            if (numbers[j] + numbers[k] == n) return true;
        }
    }

    return false;
}

(int start, int end) FindRange(long n, int i)
{
    for (int j = i - 1; j >=1; j--)
    {
        long remainder = n - numbers[j];
        for (int k = j - 1; k >= 0 && remainder > 0; k--)
        {
            remainder -= numbers[k];
            if (remainder == 0) return (k, j);
        }
    }

    return (-1, -1);
}

for (int i = PreambleLength; ; i++)
{
    long n = numbers[i];
    if (!IsValid(n, i))
    {
        Console.WriteLine(n);

        var range = FindRange(n, i);
        var subset = numbers.Skip(range.start).Take(range.end - range.start + 1);
        Console.WriteLine(subset.Min() + subset.Max());
        break;
    }
}
