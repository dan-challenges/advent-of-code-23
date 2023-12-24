using Day12;
using System.Collections.Concurrent;
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
        //var sumCh2 = Challenge2(list, resAlready);
        var sumCh2 = new Part2(list).Main();

        //Console.WriteLine("Ch1: " + sum);
        Console.WriteLine("Ch2: " + sumCh2);
        Console.ReadKey();
    }

    private static string Challenge2(string[] list, string[] resAlready)
    {
        var resDic = resAlready.Select(x => x.Split(' ')).ToDictionary(x => x[0], x => long.Parse(x[1]));
        var parsed = list.Select(x => new Row(x, 5)).ToList();

        long sum = parsed.AsParallel().Sum(row =>
        //long sum = 0;
        //foreach (var row in parsed)
        {
            if (resDic.TryGetValue(row.Line, out var val))
            {
                return val;
            }

            row.Reverse();

            if (resDic.TryGetValue(row.Line, out var val1))
            {
                return val1;
            }

            row.Reverse();

            Console.WriteLine("on it");


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

    public static ConcurrentDictionary<string, long> Submissions = new();
    internal static void Submit(string v)
    {
        Submissions.AddOrUpdate(v, 1, (x, y) => y + 1);
    }
}

public class Row
{
    StringBuilder b = new StringBuilder();
    public string Line { get; set; }
    public int[] FlexibleIdxs { get; set; }

    public int[] Expected { get; set; }
    public int[] ExpectedCommulatedFromBehind { get; set; }

    public Row(string input, int copyAmount = 1)
    {
        var split = input.Split(' ');
        Expected = string.Join(",", Enumerable.Repeat(split[1], copyAmount)).Split(',').Select(int.Parse).ToArray();
        Line = string.Join("?", Enumerable.Repeat(split[0], copyAmount));

        CreateFlexibleIds();
        CreateExpectedCommulatedFromBehind();
    }

    private void CreateFlexibleIds()
    {
        List<int> flexibleIdxs = new();

        for (int i = 0; i < Line.Length; i++)
        {
            if (Line[i] == '?')
                flexibleIdxs.Add(i);
        }
        FlexibleIdxs = flexibleIdxs.ToArray();
    }

    private void CreateExpectedCommulatedFromBehind()
    {
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

        GetAllPossibilities(Line.Substring(0, FlexibleIdxs[0]), FlexibleIdxs);

        //long amount = allPossibilities.Count(IsValid);
        if (Program.Submissions.TryGetValue(Line, out var amount))
        {
            Console.WriteLine(Line + " " + amount);
            return amount;
        }

        Console.WriteLine("ERRORR");
        return -1;
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

    private void GetAllPossibilities(string str, Memory<int> flexibles)
    {
        int currIdx = flexibles.Span[0];

        if (flexibles.Length == 1)
        {
            ReadOnlyMemory<char> substring = Line.AsMemory(currIdx + 1);
            b.Clear();
            b.Append(str);
            b.Append(".");
            b.Append(substring);
            SubmitIfValid(b.ToString());

            b.Clear();
            b.Append(str);
            b.Append("#");
            b.Append(substring);
            SubmitIfValid(b.ToString());
            return;
        }

        var skipHashtags = IsPossibleSoFar(str, Line.Length - str.Length);

        if (skipHashtags == -1)
            return;
        else if (skipHashtags > 0 && skipHashtags < 100)
        {
            int skipFlexibles = 1;
            while (skipFlexibles + 1 < flexibles.Length && flexibles.Span[skipFlexibles] < currIdx + skipHashtags)
            {
                skipFlexibles++;
            }

            ReadOnlyMemory<char> restAfterHashtags = currIdx + skipHashtags < Line.Length ? Line.AsMemory(currIdx + skipHashtags, flexibles.Span[skipFlexibles] - currIdx - skipHashtags) : ReadOnlyMemory<char>.Empty;
            b.Clear();
            b.Append(str);
            b.Append('#', skipHashtags);
            b.Append(restAfterHashtags);

            GetAllPossibilities(b.ToString(), flexibles.Slice(skipFlexibles));
            return;
        }


        ReadOnlyMemory<char> rest = Line.AsMemory(currIdx + 1, flexibles.Span[1] - currIdx - 1);
        var nextSpan = flexibles.Slice(1);

        b.Clear();
        b.Append(str);
        b.Append(".");
        b.Append(rest);

        GetAllPossibilities(b.ToString(), nextSpan);

        b.Clear();
        b.Append(str);
        b.Append("#");
        b.Append(rest);
        GetAllPossibilities(b.ToString(), nextSpan);
    }

    private void SubmitIfValid(string v)
    {
        if (IsValid(v))
            Program.Submit(Line);
    }

    //-1 not
    //0 keep going
    //1-10 next #
    //100-110 next .
    private int IsPossibleSoFar(string str, int remainingChars)
    {
        int compareIdx = 0;
        int hashtagCount = 0;
        int i = 0;
        for (i = 0; i < str.Length; i++)
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
                        return -1;

                    hashtagCount = 0;
                    compareIdx++;
                }
            }
        }

        if (hashtagCount != 0)
        {
            if (compareIdx >= Expected.Length)
                return -1;

            if (hashtagCount < Expected[compareIdx])
            {
                var nextHashtags = Expected[compareIdx] - hashtagCount;

                if (i + nextHashtags > Line.Length)
                    return -1;

                for (int j = 0; j < nextHashtags; j++)
                {
                    if (Line[i + j] == '.')
                        return -1;
                }

                return nextHashtags;
            }

            hashtagCount = 0;
            compareIdx++;
        }

        if (compareIdx > Expected.Length)
            return -1;

        if (compareIdx < ExpectedCommulatedFromBehind.Length && remainingChars < (ExpectedCommulatedFromBehind[compareIdx] + Expected.Length - compareIdx - 1))
            return -1;


        return 0;
    }

    internal void Reverse()
    {
        Line = new string(Line.Reverse().ToArray());
        CreateFlexibleIds();
        Expected = Expected.Reverse().ToArray();
        CreateExpectedCommulatedFromBehind();
    }
}

//public enum Possibility : int { Not, Done, KeepGoing, NextHashtag };