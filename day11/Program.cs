using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var input = File.ReadAllLines("input.txt");
int height = input.Length, width = input[0].Length;

char[,] grid = new char[width, height],
        grid2 = new char[width, height],
        newgrid = new char[width, height];

for (int y = 0; y < height; y++)
    for (int x = 0; x < width; x++)
        grid[x, y] = grid2[x, y] = input[y][x];

// Part 1

while (true)
{
    int changed = 0;
    int occupied = 0;

    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            newgrid[x, y] = grid[x, y];
            if (grid[x, y] == 'L')
            {
                if (!GetAdjacentSeats(x, y).Any(pos => grid[pos.x, pos.y] == '#'))
                {
                    changed++;
                    newgrid[x, y] = '#';
                }
            }
            else if (grid[x, y] == '#')
            {
                occupied++;

                if (GetAdjacentSeats(x, y).Count(pos => grid[pos.x, pos.y] == '#') >= 4)
                {
                    changed++;
                    newgrid[x, y] = 'L';
                }
            }
        }
    }

    if (changed == 0)
    {
        Console.WriteLine(occupied);
        break;
    }

    (grid, newgrid) = (newgrid, grid);
}

// Part 2
while (true)
{
    int changed = 0;
    int occupied = 0;

    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            newgrid[x, y] = grid2[x, y];

            if (grid2[x, y] == 'L')
            {
                if (!GetSeatsInSight(grid2, x, y).Any(pos => grid2[pos.x, pos.y] == '#'))
                {
                    changed++;
                    newgrid[x, y] = '#';
                }
            }
            else if (grid2[x, y] == '#')
            {
                occupied++;

                if (GetSeatsInSight(grid2, x, y).Count(pos => grid2[pos.x, pos.y] == '#') >= 5)
                {
                    changed++;
                    newgrid[x, y] = 'L';
                }
            }
        }
    }

    if (changed == 0)
    {
        Console.WriteLine(occupied);
        break;
    }

    (grid2, newgrid) = (newgrid, grid2);
}


IEnumerable<(int x, int y)> GetAdjacentSeats(int x, int y)
{
    var offsets = new (int x, int y)[] {(-1, -1), (0, -1), (1, -1),
                                        (-1, 0),           (1, 0),
                                        (-1, 1),  (0, 1),  (1, 1)};

    foreach (var o in offsets)
    {
        if (x + o.x >= 0 && x + o.x < width && y + o.y >= 0 && y + o.y < height)
            yield return (x + o.x, y + o.y);
    }
}

IEnumerable<(int x, int y)> GetSeatsInSight(char[,] grd, int x, int y)
{
    var offsets = new (int x, int y)[] {(-1, -1), (0, -1), (1, -1),
                                        (-1, 0),           (1, 0),
                                        (-1, 1),  (0, 1),  (1, 1)};

    foreach (var o in offsets)
    {
        bool directionDone = false;
        for (int mult = 1; !directionDone; mult++)
        {
            var target = (x: x + mult * o.x, y: y + mult * o.y);
            if (target.x >= 0 && target.x < width && target.y >= 0 && target.y < height)
            {
                if (grd[target.x, target.y] == 'L' || grd[target.x, target.y] == '#')
                {
                    directionDone = true;
                    yield return target;
                }
            }
            else
            {
                directionDone = true;
                continue;
            }
        }
    }
}