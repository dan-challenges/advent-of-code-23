using System.Collections.Concurrent;

internal class Program
{
    private static void Main(string[] args)
    {
        var list = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt"));

        var gameRuns = list.Select(x => new GameRun(x)).ToArray();

        int sum = Challenge1(gameRuns);
        int sumCh2 = Challenge2(gameRuns);

        Console.WriteLine("Ch1: " + sum);
        Console.WriteLine("Ch2: " + sumCh2);
        Console.ReadKey();
    }

    private static int Challenge2(GameRun[] gameRuns)
    {
        return gameRuns.Sum(x => x.GetMultiplyOfAllColorMax);
    }

    private static int Challenge1(GameRun[] gameRuns)
    {

        Dictionary<string, int> toCheck = new() { { "red", 12 }, { "green", 13 }, { "blue", 14 } };

        var sum = gameRuns.Where(run => toCheck.All(check => check.Value >= run.ColorAmounts[check.Key])).Sum(x => x.RunNumber);

        return sum;
    }
}

public class GameRun
{
    const StringSplitOptions SplitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
    public int RunNumber { get; set; }
    public ConcurrentDictionary<string, int> ColorAmounts { get; set; } = new();
    public int GetMultiplyOfAllColorMax => ColorAmounts.Aggregate(1, (acc, cur) => acc * cur.Value);

    public GameRun(string line)
    {
        var split = line.Split(":", SplitOptions);
        RunNumber = int.Parse(split[0].Split(" ")[1]);

        var colors = split[1].Split([",", ";"], SplitOptions);

        foreach (var item in colors)
        {
            var splitColor = item.Split(" ", SplitOptions);
            var amount = int.Parse(splitColor[0]);
            var color = splitColor[1];

            ColorAmounts.AddOrUpdate(color, amount, (key, oldValue) => Math.Max(oldValue, amount));
        }
    }
}