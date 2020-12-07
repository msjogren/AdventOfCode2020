using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

string[] RequiredFields = new[] {"byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid"};
string[] EyeColors = new[] {"amb", "blu", "brn", "gry", "grn", "hzl", "oth"};


IEnumerable<string> GetPassports(string[] input)
{
    string buffer = "";
    foreach (string line in input)
    {
        if (line == "")
        {
            yield return buffer;
            buffer = "";
        }
        else
        {
            if (buffer.Length > 0) buffer += " ";
            buffer += line;
        }
    }

    if (buffer.Length > 0) yield return buffer;
}
 
bool ValidateData(string passport)
{
    foreach (string field in passport.Split(' '))
    {
        var keyval = field.Split(':');
        string key = keyval[0], data = keyval[1];

        switch (key)
        {
            case "byr":
                if (data.Length != 4 || !int.TryParse(data, out int byr) || byr < 1920 || byr > 2002) return false;
                break;
            
            case "iyr":
                if (data.Length != 4 || !int.TryParse(data, out int iyr) || iyr < 2010 || iyr > 2020) return false;
                break;

            case "eyr":
                if (data.Length != 4 || !int.TryParse(data, out int eyr) || eyr < 2020 || eyr > 2030) return false;
                break;

            case "hgt":
                if (data.Length < 4 || data.Length > 5) return false;
                if (!int.TryParse(data.Substring(0, data.Length - 2), out int hgt)) return false;
                if (data.EndsWith("in"))
                {
                    if (hgt < 59 || hgt > 76) return false;
                }
                else if (data.EndsWith("cm"))
                {
                    if (hgt < 150 || hgt > 193) return false;
                }
                else
                {
                    return false;
                }
                break;
            
            case "hcl":
                if (data.Length != 7 || data[0] != '#' || !int.TryParse(data.Substring(1), NumberStyles.AllowHexSpecifier, null, out int hcl)) return false;
                break;
            
            case "ecl":
                if (!EyeColors.Contains(data)) return false;
                break; 
            
            case "pid":
                if (data.Length != 9 || !long.TryParse(data, out long pid)) return false;
                break;
        }
    }

    return true;
}


var input = File.ReadAllLines("input.txt");
var passports = GetPassports(input);

int validPart1 = 0, validPart2 = 0;
foreach (var passport in passports)
{
    if (RequiredFields.All(f => passport.Contains($"{f}:")))
    {
        validPart1++;
        if (ValidateData(passport)) validPart2++;
    }
}

Console.WriteLine(validPart1);
Console.WriteLine(validPart2);
