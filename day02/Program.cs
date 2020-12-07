using System;
using System.IO;
using System.Linq;

bool IsValidPasswordPart1(string input)
{
    string[] parts = input.Split(' ');
    int dashPos = parts[0].IndexOf('-');
    int min = int.Parse(parts[0].Substring(0, dashPos));
    int max = int.Parse(parts[0].Substring(dashPos + 1));
    int count = parts[2].Count(c => c == parts[1][0]);
    return count >= min && count <= max;
}

bool IsValidPasswordPart2(string input)
{
    string[] parts = input.Split(' ');
    int dashPos = parts[0].IndexOf('-');
    int idx1 = int.Parse(parts[0].Substring(0, dashPos)) - 1;
    int idx2 = int.Parse(parts[0].Substring(dashPos + 1)) - 1;
    char c = parts[1][0];
    return parts[2][idx1] == c ^ parts[2][idx2] == c;
}

var lines = File.ReadAllLines("input.txt");
int validPasswordsP1 = lines.Where(l => IsValidPasswordPart1(l)).Count();
Console.WriteLine(validPasswordsP1);
int validPasswordsP2 = lines.Where(l => IsValidPasswordPart2(l)).Count();
Console.WriteLine(validPasswordsP2);
