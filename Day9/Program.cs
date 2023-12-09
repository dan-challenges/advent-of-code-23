internal class Program
{
    public const StringSplitOptions SplitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

    private static void Main(string[] args)
    {
        var list = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt"));

        var parsed = Parse(list);

        var sum = Challenge1(parsed);
        var sumCh2 = Challenge2(parsed);

        Console.WriteLine("Ch1: " + sum);
        Console.WriteLine("Ch2: " + sumCh2);
        Console.ReadKey();
    }

    private static string Challenge2(Recording[] parsed)
    {
        long sum = 0;
        foreach (var rec in parsed)
        {
            sum += rec.ExtrapolateBefore();
        }
        return sum.ToString();
    }

    private static string Challenge1(Recording[] parsed)
    {
        long sum = 0;
        foreach (var rec in parsed)
        {
            sum += rec.Extrapolate();
        }
        return sum.ToString();
    }

    private static Recording[] Parse(string[] list)
    {
        return list.Select(x => new Recording(x)).ToArray();
    }
}

public class Recording
{
    public List<int> InputNumbers { get; set; }

    public Recording(string line)
    {
        InputNumbers = line.Split(' ', Program.SplitOptions).Select(int.Parse).ToList();
    }

    internal long Extrapolate()
    {
        var triangle = GetTriangle();

        for (int i = triangle.Count - 2; i > -1; i--)
        {
            var preValue = triangle[i].Last();
            var increaseBy = (i + 1) < triangle.Count - 1 ? triangle[i + 1].Last() : 0;
            triangle[i].Add(preValue + increaseBy);
        }

        return triangle[0].Last();
    }

    private List<List<int>> GetTriangle()
    {
        List<List<int>> triangle = new() { InputNumbers };

        var latest = InputNumbers.ToArray();

        while (!AllZero(latest))
        {
            int[] nextLine = new int[latest.Length - 1];
            for (int i = 1; i < latest.Length; i++)
            {
                nextLine[i - 1] = latest[i] - latest[i - 1];
            }
            triangle.Add(nextLine.ToList());
            latest = nextLine;
        }
        return triangle;
    }

    private bool AllZero(int[] inputNumbers)
    {
        foreach (var num in inputNumbers)
        {
            if (num != 0)
            {
                return false;
            }
        }

        return true;
    }

    internal long ExtrapolateBefore()
    {
        var triangle = GetTriangle();

        for (int i = triangle.Count - 2; i > -1; i--)
        {
            var preValue = triangle[i].First();
            var decreaseBy = (i + 1) < triangle.Count - 1 ? triangle[i + 1].First() : 0;
            triangle[i].Insert(0, preValue - decreaseBy);
        }

        return triangle[0].First();
    }
}