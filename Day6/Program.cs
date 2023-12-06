using System.Runtime.InteropServices;

internal class Program
{
    const StringSplitOptions SplitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
    private static void Main(string[] args)
    {
        var list = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt"));

        var parsed = Parse(list);

        var sum = Challenge1(parsed);
        var sumCh2 = Challenge2(list);

        Console.WriteLine("Ch1: " + sum);
        Console.WriteLine("Ch2: " + sumCh2);
        Console.ReadKey();
    }

    private static long Challenge2(string[] list)
    {
        var splitTime = long.Parse(list[0].Split(":", SplitOptions)[1].Replace(" ", ""));
        var splitDistance = long.Parse(list[1].Split(":", SplitOptions)[1].Replace(" ", ""));
        var parsed = new Run() { Time = splitTime, Record_Distance = splitDistance };

        return parsed.GetAmountOfTimesWhichAreBetterThanRecordDistance();
    }

    private static long Challenge1(List<Run> parsed)
    {
        return parsed.Aggregate(1L, (curr, el) => curr * el.GetAmountOfTimesWhichAreBetterThanRecordDistance());
    }

    private static List<Run> Parse(string[] list)
    {
        var listTime = list[0].Split(" ", SplitOptions).Skip(1).ToArray();
        var listDistance = list[1].Split(" ", SplitOptions).Skip(1).ToArray();

        List<Run> runs = new();
        for (int i = 0; i < listTime.Length; i++)
        {
            runs.Add(new Run() { Time = int.Parse(listTime[i]), Record_Distance = int.Parse(listDistance[i]) });
        }
        return runs;
    }
}

public class Run
{
    public long Time { get; set; }
    public long Record_Distance { get; set; }

    public long GetAmountOfTimesWhichAreBetterThanRecordDistance()
    {
        long amount = 0;
        for (long i = 0; i < Time; i++)
        {
            if (i * (Time - i) > Record_Distance)
                amount++;
        }

        return amount;
    }
}