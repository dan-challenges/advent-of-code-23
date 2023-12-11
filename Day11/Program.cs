using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

internal class Program
{
    public const StringSplitOptions SplitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

    private static void Main(string[] args)
    {
        var list = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt"));

        //PrintUniverse(list);

        //var sum = Challenge1(list);

        var sumCh2 = Challenge2(list);

        //Console.WriteLine("Ch1: " + sum);
        Console.WriteLine("Ch2: " + sumCh2);
        Console.ReadKey();
    }

    #region Ch2

    private static string Challenge2(string[] list)
    {
        var expandedUniverse = ExpandUniverseCh2(list);
        PrintUniverse(expandedUniverse);

        var parsed = GetUniversePos(expandedUniverse);

        

        ulong res = GetDistancesCh2(parsed, expandedUniverse);

        return res.ToString();
    }

    private static ulong GetDistancesCh2(List<Universe> parsed, List<string> expandedUniverse)
    {
        ulong res = 0;
        for (int i = 0; i < parsed.Count; i++)
        {
            var a = parsed[i];
            for (int j = i + 1; j < parsed.Count; j++)
            {
                var b = parsed[j];
                res += GetDistanceBetweenTwo(a, b, expandedUniverse);
            }
        }
        return res;
    }

    private static ulong GetDistanceBetweenTwo(Universe a, Universe b, List<string> expandedUniverse)
    {
        ulong increase = 1_000_000;
        ulong distance = 0;
        var x1 = Math.Min(a.X, b.X);
        var x2 = Math.Max(a.X, b.X);

        var y1 = Math.Min(a.Y, b.Y);
        var y2 = Math.Max(a.Y, b.Y);

        for (int x = x1; x < x2; x++)
        {
            if (expandedUniverse[b.Y][x] == 'e')
                distance += increase;
            else
                distance += 1;
        }
        for (int y = y1; y < y2; y++)
        {
            if (expandedUniverse[y][b.X] == 'e')
                distance += increase;
            else
                distance += 1;
        }

        return distance;
    }

    private static List<string> ExpandUniverseCh2(string[] list)
    {
        var copy = list.ToList();

        for (int i = 0; i < copy.Count; i++)
        {
            if (!copy[i].Contains('#'))
            {
                copy[i] = copy[i].Replace('.', 'e');
            }
        }

        for (int x = 0; x < copy[0].Length; x++)
        {
            bool allDots = true;
            for (int y = 0; y < copy.Count; y++)
            {
                if (copy[y][x] == '#')
                {
                    allDots = false;
                    break;
                }
            }

            if (allDots)
            {
                for (int y = 0; y < copy.Count; y++)
                {
                    var builder = new StringBuilder(copy[y]);
                    builder[x] = 'e';
                    copy[y] = builder.ToString();
                }
            }

        }

        return copy;
    }
    #endregion


    #region Ch1
    private static string Challenge1(string[] list)
    {
        var expandedUniverse = ExpandUniverseCh1(list);

        var parsed = GetUniversePos(expandedUniverse);

        int sum = 0;
        for (int i = 0; i < parsed.Count; i++)
        {
            var a = parsed[i];
            for (int j = i + 1; j < parsed.Count; j++)
            {
                var b = parsed[j];
                sum += Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
            }
        }
        return sum.ToString();
    }

    private static List<Universe> GetUniversePos(List<string> list)
    {
        List<Universe> universes = new();

        for (int y = 0; y < list.Count; y++)
        {
            for (int x = 0; x < list[y].Length; x++)
            {
                if (list[y][x] == '#')
                {
                    universes.Add(new Universe { X = x, Y = y });
                }
            }
        }

        return universes;
    }
    private static List<string> ExpandUniverseCh1(string[] list)
    {
        var copy = list.ToList();

        for (int i = 0; i < copy.Count; i++)
        {
            if (!copy[i].Contains('#'))
            {
                copy.Insert(i, copy[i]);
                i++;
            }
        }

        for (int x = 0; x < copy[0].Length; x++)
        {
            bool allDots = true;
            for (int y = 0; y < copy.Count; y++)
            {
                if (copy[y][x] == '#')
                {
                    allDots = false;
                    break;
                }
            }

            if (allDots)
            {
                for (int y = 0; y < copy.Count; y++)
                {
                    copy[y] = copy[y].Insert(x, ".");
                }
                x++;
            }

        }

        return copy;
    }
    #endregion



    private static void PrintUniverse(IEnumerable<string> list)
    {
        foreach (var line in list)
        {
            Console.WriteLine(line);
        }
    }

}

public class Universe
{
    public int X { get; set; }
    public int Y { get; set; }
}