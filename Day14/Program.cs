using System.Diagnostics;

internal class Program
{
    public const StringSplitOptions SplitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

    private static void Main(string[] args)
    {
        var list = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt"));

        var parsed = Parse(list);

        //var sum = Challenge1(parsed);

        var sumCh2 = Challenge2(parsed);

        //Console.WriteLine("Ch1: " + sum);
        Console.WriteLine("Ch2: " + sumCh2);
        Console.ReadLine();
        Console.ReadLine();
    }

    private static long Challenge2(List<List<FloorEl>> parsed)
    {
        List<long> results = new();
        int looping = 0;
        int loopStartIdx = -1;

        for (int i = 0; i < 1_000_000; i++)
        {
            MoveUp(parsed);
            MoveLeft(parsed);
            MoveDown(parsed);
            MoveRight(parsed);

            var currRes = Calc(parsed);

            if (results.Any(x => x == currRes))
            {
                if (loopStartIdx == -1)
                    loopStartIdx = i;

                looping++;

                if (looping == 100)
                {
                    break;
                }
            }
            else
            {
                looping = 0;
                loopStartIdx = -1;
            }

            results.Add(currRes);
        }

        var lastIdx = results.Count - 1;

        var lastElValue = results.Last();
        results.RemoveAt(results.Count - 1);
        var secondFromLastIdx = results.LastIndexOf(lastElValue);

        int loopSize = lastIdx - secondFromLastIdx;

        var idx = (1_000_000_000 - 1 - loopStartIdx) % loopSize;

        return results[loopStartIdx + idx];
    }

    private static void MoveRight(List<List<FloorEl>> parsed)
    {
        var height = parsed.Count;
        var width = parsed[0].Count;
        Queue<int> rightMostIdxToMoveTo = new();
        for (int y = 0; y < height; y++)
        {
            for (int x = width - 1; x > -1; x--)
            {
                switch (parsed[y][x])
                {
                    case FloorEl.Empty:
                        rightMostIdxToMoveTo.Enqueue(x);
                        break;
                    case FloorEl.Moving:
                        if (rightMostIdxToMoveTo.TryDequeue(out var dequeued))
                        {
                            parsed[y][x] = FloorEl.Empty;
                            parsed[y][dequeued] = FloorEl.Moving;
                            rightMostIdxToMoveTo.Enqueue(x);
                        }
                        break;
                    case FloorEl.Static:
                        rightMostIdxToMoveTo.Clear();
                        break;
                    default:
                        break;
                }
            }
            rightMostIdxToMoveTo.Clear();
        }
    }

    private static void MoveDown(List<List<FloorEl>> parsed)
    {
        var height = parsed.Count;
        var width = parsed[0].Count;
        Queue<int> downMostIdxToMoveTo = new();
        for (int x = 0; x < width; x++)
        {
            for (int y = height - 1; y > -1; y--)
            {
                switch (parsed[y][x])
                {
                    case FloorEl.Empty:
                        downMostIdxToMoveTo.Enqueue(y);
                        break;
                    case FloorEl.Moving:
                        if (downMostIdxToMoveTo.TryDequeue(out var dequeued))
                        {
                            parsed[y][x] = FloorEl.Empty;
                            parsed[dequeued][x] = FloorEl.Moving;
                            downMostIdxToMoveTo.Enqueue(y);
                        }
                        break;
                    case FloorEl.Static:
                        downMostIdxToMoveTo.Clear();
                        break;
                    default:
                        break;
                }
            }
            downMostIdxToMoveTo.Clear();
        }
    }

    private static void MoveLeft(List<List<FloorEl>> parsed)
    {
        var height = parsed.Count;
        var width = parsed[0].Count;
        Queue<int> leftMostIdxToMoveTo = new();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                switch (parsed[y][x])
                {
                    case FloorEl.Empty:
                        leftMostIdxToMoveTo.Enqueue(x);
                        break;
                    case FloorEl.Moving:
                        if (leftMostIdxToMoveTo.TryDequeue(out var dequeued))
                        {
                            parsed[y][x] = FloorEl.Empty;
                            parsed[y][dequeued] = FloorEl.Moving;
                            leftMostIdxToMoveTo.Enqueue(x);
                        }
                        break;
                    case FloorEl.Static:
                        leftMostIdxToMoveTo.Clear();
                        break;
                    default:
                        break;
                }
            }
            leftMostIdxToMoveTo.Clear();
        }
    }

    private static long Challenge1(List<List<FloorEl>> parsed)
    {
        MoveUp(parsed);
        //PrintFloor(parsed);
        return Calc(parsed);
    }

    private static long Calc(List<List<FloorEl>> parsed)
    {
        var len = parsed[0].Count;
        long res = 0;
        for (int x = 0; x < len; x++)
        {
            for (int y = 0; y < parsed.Count; y++)
            {
                if (parsed[y][x] == FloorEl.Moving)
                {
                    res += len - y;
                }
            }
        }
        return res;
    }

    private static void PrintFloor(List<List<FloorEl>> parsed)
    {
        foreach (var line in parsed)
        {
            foreach (var el in line)
            {
                switch (el)
                {
                    case FloorEl.Empty:
                        Console.Write('.');
                        break;
                    case FloorEl.Moving:
                        Console.Write('O');
                        break;
                    case FloorEl.Static:
                        Console.Write('#');
                        break;
                    default:
                        break;
                }
            }
            Console.WriteLine();
        }
    }

    private static void MoveUp(List<List<FloorEl>> parsed)
    {
        var len = parsed[0].Count;
        Queue<int> uppestIdxToMoveTo = new();
        for (int x = 0; x < len; x++)
        {
            for (int y = 0; y < parsed.Count; y++)
            {
                switch (parsed[y][x])
                {
                    case FloorEl.Empty:
                        uppestIdxToMoveTo.Enqueue(y);
                        break;
                    case FloorEl.Moving:
                        if (uppestIdxToMoveTo.TryDequeue(out var dequeued))
                        {
                            parsed[y][x] = FloorEl.Empty;
                            parsed[dequeued][x] = FloorEl.Moving;
                            uppestIdxToMoveTo.Enqueue(y);
                        }
                        break;
                    case FloorEl.Static:
                        uppestIdxToMoveTo.Clear();
                        break;
                    default:
                        break;
                }
            }
            uppestIdxToMoveTo.Clear();
        }
    }

    public enum FloorEl : byte { Empty, Moving, Static }
    private static List<List<FloorEl>> Parse(string[] list)
    {
        List<List<FloorEl>> els = new List<List<FloorEl>>();

        foreach (var line in list)
        {
            var floor = new List<FloorEl>();
            foreach (var c in line)
            {
                switch (c)
                {
                    case '.':
                        floor.Add(FloorEl.Empty);
                        break;
                    case 'O':
                        floor.Add(FloorEl.Moving);
                        break;
                    case '#':
                        floor.Add(FloorEl.Static);
                        break;
                }
            }
            els.Add(floor);
        }

        return els;
    }
}