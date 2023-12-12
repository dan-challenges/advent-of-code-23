using System.Diagnostics;
using System.Drawing;
using System.Security.Principal;
using System.Text;

internal class Program
{
    public const StringSplitOptions SplitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

    private static void Main(string[] args)
    {
        var list = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt"));
        var resAlready = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ResultsAlready.txt"));

        //var parsed = Parse(list);

        //var sum = Challenge1(parsed);
        var sumCh2 = Challenge2(list, resAlready);

        //Console.WriteLine("Ch1: " + sum);
        Console.WriteLine("Ch2: " + sumCh2);
        Console.ReadKey();
    }

    private static string Challenge2(string[] list, string[] resAlready)
    {
        var resDic = resAlready.Select(x => x.Split(' ')).ToDictionary(x => x[0], x => long.Parse(x[1]));
        var parsed = list.Select(x => new Row(x, 5)).ToList();

        long sum = parsed.AsParallel().Sum(row =>
        {
            if (resDic.TryGetValue(row.Line, out var val))
            {
                return val;
            }

            return row.CountPossibilities();
        });

        return sum.ToString();
    }

    private static long Challenge1(List<Row> parsed)
    {
        return parsed.Sum(x => x.CountPossibilities());
    }

    private static List<Row> Parse(string[] list)
    {
        return list.Select(x => new Row(x)).ToList();
    }
}

public class Row
{
    public string Line { get; set; }
    public int[] FlexibleIdxs { get; set; }

    public int[] Expected { get; set; }
    public int[] ExpectedCommulatedFromBehind { get; set; }

    public Row(string input, int copyAmount = 1)
    {
        var split = input.Split(' ');
        Expected = string.Join(",", Enumerable.Repeat(split[1], copyAmount)).Split(',').Select(int.Parse).ToArray();
        Line = string.Join("?", Enumerable.Repeat(split[0], copyAmount));

        List<int> flexibleIdxs = new();

        for (int i = 0; i < Line.Length; i++)
        {
            if (Line[i] == '?')
                flexibleIdxs.Add(i);
        }
        FlexibleIdxs = flexibleIdxs.ToArray();

        ExpectedCommulatedFromBehind = new int[Expected.Length];

        int sum = 0;
        for (int i = Expected.Length - 1; i > -1; i--)
        {
            sum += Expected[i];
            ExpectedCommulatedFromBehind[i] = sum;
        }
    }

    public long CountPossibilities()
    {
        if (FlexibleIdxs.Length == 0)
            return IsValid(Line) ? 1 : 0;

        var allPossibilities = GetAllPossibilities(Line.Substring(0, FlexibleIdxs[0]), FlexibleIdxs);
        long amount = allPossibilities.Count(IsValid);
        Console.WriteLine(Line + " " + amount);
        return amount;
    }

    private bool IsValid(string str)
    {
        int compareIdx = 0;
        int hashtagCount = 0;
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] == '#')
            {
                hashtagCount++;
                continue;
            }
            else
            {
                if (hashtagCount != 0)
                {
                    if (compareIdx >= Expected.Length || Expected[compareIdx] != hashtagCount)
                        return false;

                    hashtagCount = 0;
                    compareIdx++;
                }
            }
        }

        if (hashtagCount != 0)
        {
            if (compareIdx >= Expected.Length || Expected[compareIdx] != hashtagCount)
                return false;

            hashtagCount = 0;
            compareIdx++;
        }

        return compareIdx == Expected.Length;
    }

    private IEnumerable<string> GetAllPossibilities(string str, Memory<int> flexibles)
    {
        int span0 = flexibles.Span[0];

        if (flexibles.Length == 1)
        {
            string substring = Line.Substring(span0 + 1);
            yield return str + "." + substring;
            yield return str + "#" + substring;
            yield break;
        }

        var possibleSoFar = IsPossibleSoFar(str, Line.Length - str.Length);

        switch (possibleSoFar)
        {
            case Possibility.Not:
                yield break;
            case Possibility.Done:
                yield return str + "." + Line.Substring(span0 + 1);
                yield break;
                //case Possibility.KeepGoing:
                //    break;
        }

        string rest = Line.Substring(span0 + 1, flexibles.Span[1] - span0 - 1);
        var nextSpan = flexibles.Slice(1);

        foreach (var item in GetAllPossibilities(str + "." + rest, nextSpan))
        {
            yield return item;
        }


        foreach (var item in GetAllPossibilities(str + "#" + rest, nextSpan))
        {
            yield return item;
        }
    }

    private Possibility IsPossibleSoFar(string str, int remainingChars)
    {
        int compareIdx = 0;
        int hashtagCount = 0;
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] == '#')
            {
                hashtagCount++;
                continue;
            }
            else
            {
                if (hashtagCount != 0)
                {
                    if (compareIdx >= Expected.Length || Expected[compareIdx] != hashtagCount)
                        return Possibility.Not;

                    hashtagCount = 0;
                    compareIdx++;
                }
            }
        }

        if (hashtagCount != 0)
        {
            if (compareIdx >= Expected.Length)
                return Possibility.Not;

            hashtagCount = 0;
            compareIdx++;
        }


        if (compareIdx > Expected.Length)
            return Possibility.Not;

        if (compareIdx < ExpectedCommulatedFromBehind.Length && remainingChars < (ExpectedCommulatedFromBehind[compareIdx] + Expected.Length - compareIdx - 1))
            return Possibility.Not;


        return Possibility.KeepGoing;
    }
}

public enum Possibility : byte { Not, Done, KeepGoing };