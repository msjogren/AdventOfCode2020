using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

const int InputLinesPerTile = Tile.TileSize + 2;

var input = File.ReadAllLines("input.txt");
bool doneParsing = false;
var tiles = new List<Tile>();

while (!doneParsing) {
    var tileLines = input.Skip(tiles.Count * InputLinesPerTile).Take(InputLinesPerTile);
    doneParsing = tileLines.Count() < InputLinesPerTile;
    int tileNumber = int.Parse(tileLines.First().Substring(5).TrimEnd(':'));
    tiles.Add(new Tile(tileNumber, tileLines.Skip(1).Take(Tile.TileSize)));
}

//
// Part 1
//
var borderMatchGroups = tiles
    .SelectMany(t => t.MaxBorders.Select(bits => (bits, tile: t.ID)))
    .GroupBy(tpl => tpl.bits, tpl => tpl.tile);

// Sanity check: Verify that there are no border patterns that match more than two tiles,   
// as that would make solving the puzzle much harder.
var bitBorderOnMoreThanTwoTiles = borderMatchGroups.Where(grp => grp.Count() > 2);
if (bitBorderOnMoreThanTwoTiles.Any()) throw new InvalidOperationException();

// Find tile border patterns that are not found on any other tile, i.e. those towards the outer edge of the puzzle
var unmatchedBorders = borderMatchGroups.Where(grp => grp.Count() == 1);

// Find tiles with two unmatched borders, i.e. those in the corners
var cornerTiles = unmatchedBorders
    .Select(grp => (tile: grp.First(), bits: grp.Key))
    .GroupBy(tpl => tpl.tile)
    .Where(grp => grp.Count() == 2)
    .Select(grp => grp.Key);

long product = cornerTiles.Aggregate(1L, (prd, tile) => prd * tile);
Console.WriteLine(product);

//
// Part 2
//
int gridSize = (int)Math.Sqrt(tiles.Count);
var puzzleTiles = new TileProjection[gridSize, gridSize];
var topLeftTile = cornerTiles.First();
var adjacentTiles = new Dictionary<int, HashSet<int>>();

// Create a lookup dictionary from each tile to its neigbor tiles.
foreach (var grp in borderMatchGroups.Where(grp => grp.Count() == 2))
{
    foreach ((int key, int other) in new[] {(grp.First(), grp.ElementAt(1)), (grp.ElementAt(1), grp.First())})
    {
        if (!adjacentTiles.TryGetValue(key, out var set))
        {
            set = new HashSet<int>();
            adjacentTiles.Add(key, set);
        }
        set.Add(other);
    }
}

// Fill the puzzle with tiles one by one, row by row, starting with one of the identified corner tiles.
for (int tileY = 0; tileY < gridSize; tileY++)
{
    for (int tileX = 0; tileX < gridSize; tileX++)
    {
        if (tileX == 0 && tileY == 0)
        {
            var outerBorders = unmatchedBorders
                .Where(grp => grp.First() == topLeftTile)
                .Select(grp => grp.Key);
            puzzleTiles[0,0] = tiles
                .Where(t => t.ID == topLeftTile)
                .SelectMany(t => t.Projections)
                .First(p =>
                        (outerBorders.Contains(p.LeftBorder) || outerBorders.Contains(Tile.ReverseBits(p.LeftBorder))) &&
                        (outerBorders.Contains(p.TopBorder) || outerBorders.Contains(Tile.ReverseBits(p.TopBorder))));
        }
        else if (tileX == 0)
        {
            if (tileY == 0) continue;
            int bottomBorderToMatch = puzzleTiles[tileX, tileY - 1].BottomBorder;
            puzzleTiles[tileX, tileY] = tiles
                .Where(t => adjacentTiles[puzzleTiles[tileX, tileY - 1].ID].Contains(t.ID))
                .SelectMany(t => t.Projections)
                .First(p => p.TopBorder == bottomBorderToMatch);
        }
        else
        {
            int rightBorderToMatch = puzzleTiles[tileX - 1 , tileY].RightBorder;
            puzzleTiles[tileX, tileY] = tiles
                .Where(t => adjacentTiles[puzzleTiles[tileX - 1, tileY].ID].Contains(t.ID))
                .SelectMany(t => t.Projections)
                .First(p => p.LeftBorder == rightBorderToMatch);
        }
    }
}

// Create merged image from tiles, excluding their borders. Count number of #.
int imageSize = (Tile.TileSize - 2) * gridSize;
int totalPounds = 0;
var image = new char[imageSize, imageSize];
for (int tileY = 0; tileY < gridSize; tileY++)
    for (int tileX = 0; tileX < gridSize; tileX++)
        for (int tileImageY = 1; tileImageY < Tile.TileSize - 1; tileImageY++)
            for (int tileImageX = 1; tileImageX < Tile.TileSize - 1; tileImageX++) {
                char ch = image[tileX * (Tile.TileSize - 2) + (tileImageX - 1), tileY * (Tile.TileSize - 2) + (tileImageY - 1)] = 
                    puzzleTiles[tileX, tileY].ImageData[tileImageX, tileImageY];
                if (ch == '#') totalPounds++;
            }

// Find the sea monsters

//               1111111111
//     01234567890123456789
// -1 "                  # "
//  0 "#    ##    ##    ###"
//  1 " #  #  #  #  #  #   "
IEnumerable<(int x, int y)> seaMonsterOffsets = new[] {
    (18,-1),
    (5,0), (6,0), (11,0), (12,0), (17,0), (18,0), (19,0),
    (1,1), (4,1), (7,1), (10,1), (13,1), (16,1)
};

int SeaMonsterCount(char[,] img)
{
    int monsters = 0;

    for (int imageY = 1; imageY < imageSize - 2; imageY++)
        for (int imageX = 0; imageX <= imageSize - 20; imageX++)
            if (img[imageX, imageY] == '#' && seaMonsterOffsets.All(o => img[imageX + o.x, imageY + o.y] == '#')) monsters++;

    return monsters;
}

int rotations = 0;
var currentImage = image;
do {
    int monsters = SeaMonsterCount(currentImage);
    if (monsters == 0) monsters = SeaMonsterCount(Tile.FlipImageHorizontally(currentImage, imageSize));
    if (monsters > 0)
    {
        Console.WriteLine(totalPounds - monsters * (seaMonsterOffsets.Count() + 1));
        break;
    }
    currentImage = Tile.RotateImageClockwise(currentImage, imageSize);
} while (++rotations < 4);

class TileProjection
{
    public int ID { get; private set; }
    public int LeftBorder { get; private set; }
    public int TopBorder { get; private set; }
    public int RightBorder { get; private set; }
    public int BottomBorder { get; private set; }

    public IEnumerable<int> Borders => new[] { LeftBorder, TopBorder, RightBorder, BottomBorder }; 

    public char[,] ImageData { get; }

    public TileProjection(int id, char[,] imageData)
    {
        ID = id;
        ImageData = imageData;

        int gridSize = imageData.GetLength(0);
        var borderEnumerator = Enumerable.Range(0, gridSize);
        LeftBorder = CharsToBits(gridSize, borderEnumerator.Select(y => imageData[0, y]));
        RightBorder = CharsToBits(gridSize, borderEnumerator.Select(y => imageData[gridSize - 1, y]));
        TopBorder = CharsToBits(gridSize, borderEnumerator.Select(x => imageData[x, 0]));
        BottomBorder = CharsToBits(gridSize, borderEnumerator.Select(x => imageData[x, gridSize - 1]));
    }

    int CharsToBits(int tileSize, IEnumerable<char> chars)
    {
        int bits = 0;
        foreach (var chpos in chars.Select((c, i) => (c, i)).Where(t => t.c == '#'))
        {
            bits |= 1 << (tileSize - 1 - chpos.i);
        }

        return bits;
    }
}

class Tile
{
    public const int TileSize = 10;

    public IEnumerable<TileProjection> Projections { get; }

    public IEnumerable<int> MaxBorders => Projections.First().Borders.Select(b => Math.Max(b, ReverseBits(b)));

    public int ID { get; private set; }

    public Tile(int id, IEnumerable<string> lines)
    {
        ID = id;

        var data = new char[TileSize, TileSize];
        int y = 0;
        foreach (string line in lines)
        {
            int x = 0;
            foreach (char c in line) data[x++, y] = c;
            y++;
        }

        var projections = new List<TileProjection>();
        projections.Add(new TileProjection(id, data));
        projections.Add(new TileProjection(id, FlipImageHorizontally(data, TileSize)));

        var currentImage = data;
        for (int rotations = 0; rotations < 3; rotations++)
        {
            currentImage = RotateImageClockwise(currentImage, TileSize);
            projections.Add(new TileProjection(id, currentImage));
            projections.Add(new TileProjection(id, FlipImageHorizontally(currentImage, TileSize)));
        }

        Projections = projections;
    }

    public static int ReverseBits(int bits)
    {
        int result = 0;
        for (int bit = 0; bit < TileSize; bit++)
        {
            if ((bits & (1 << bit)) != 0)
            {
                result |= 1 << (TileSize - bit - 1);
            }
        }
        return result;
    }

    public static char[,] FlipImageHorizontally(char[,] imageData, int size)
    {
        var result = new char[size, size];
        for (int fromY = 0, toY = size - 1; fromY < size; fromY++, toY--)
        {
            for (int x = 0; x < size; x++) result[x, toY] = imageData[x, fromY];
        }

        return result;
    }

    public static char[,] RotateImageClockwise(char[,] imageData, int size)
    {
        var result = new char[size, size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                result[size - y - 1, x] = imageData[x, y];
            }
        }

        return result;
    }
}
