using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var ingredientSetsForAllergen = new Dictionary<string, List<HashSet<string>>>();
var allergenToIngredientMap = new Dictionary<string, string>();
var ingredientCounts = new Dictionary<string, int>();

var input = File.ReadAllLines("input.txt");
foreach (string line in input)
{
    var parts = line.Split(" (contains ");
    var lineIngredients = parts[0].Split(' ');
    var lineAllergens = parts.Length > 1 ? parts[1].TrimEnd(')').Split(", ") : new string[0];

    foreach (string ingredient in lineIngredients)
    {
        if (!ingredientCounts.TryGetValue(ingredient, out int count)) count = 0;
        ingredientCounts[ingredient] = ++count;
    }

    foreach (string allergen in lineAllergens)
    {
        if (!allergenToIngredientMap.ContainsKey(allergen)) allergenToIngredientMap.Add(allergen, null);

        if (!ingredientSetsForAllergen.TryGetValue(allergen, out var list))
        {
            list = new List<HashSet<string>>();
            ingredientSetsForAllergen.Add(allergen, list);
        }
        list.Add(new HashSet<string>(lineIngredients));
    }
}


bool changes;
do
{
    changes = false;
    foreach (var kvp in ingredientSetsForAllergen)
    {
        if (allergenToIngredientMap.TryGetValue(kvp.Key, out string knownIngredient) && knownIngredient != null) continue;

        var intersection = new HashSet<string>(kvp.Value.First());
        foreach (var otherset in kvp.Value.Skip(1)) intersection.IntersectWith(otherset);

        if (intersection.Count == 1)
        {
            string foundIngredient = intersection.First();
            allergenToIngredientMap[kvp.Key] = foundIngredient;
            foreach (var kvp2 in ingredientSetsForAllergen)
            {
                if (kvp.Key == kvp2.Key) continue;
                foreach (var set in kvp2.Value) set.Remove(foundIngredient);
            }
            changes = true;
        }
    }
} while (changes);

int part1sum = ingredientCounts.Where(kvp => !allergenToIngredientMap.ContainsValue(kvp.Key)).Sum(kvp => kvp.Value);
Console.WriteLine(part1sum);

string part2list = String.Join(",", allergenToIngredientMap.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value));
Console.WriteLine(part2list);
