internal class Program
{
    private static void Main(string[] args)
    {
        var list = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt"));

        var parsed = Parse(list);

        long sum = Challenge1(parsed);
        long sumCh2 = Challenge2(parsed);

        Console.WriteLine("Ch1: " + sum);
        Console.WriteLine("Ch2: " + sumCh2);
        Console.ReadKey();
    }

    private static long Challenge2(List<Card> parsed)
    {
        Dictionary<int, int> cardAmounts = Enumerable.Range(0, parsed.Count).ToDictionary(x => x, x => 1);

        for (int i = 0; i < parsed.Count; i++)
        {
            int amount = parsed[i].AmountOfWinningNumbers;

            for (int j = 0; j < amount; j++)
            {
                cardAmounts[i + j + 1] += cardAmounts[i];
            }
        }


        long res = 0;
        foreach (var item in cardAmounts)
        {
            res += item.Value;
        }

        return res;
    }

    private static long Challenge1(List<Card> parsed)
    {
        return parsed.Sum(x => x.CardWorth);
    }

    private static List<Card> Parse(string[] list)
    {
        return list.Select(x => new Card(x)).ToList();
    }
}

public class Card
{
    const StringSplitOptions SplitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
    public Card(string line)
    {
        var split = line.Split(':', SplitOptions)[1].Split('|', SplitOptions);

        CorrectNumbers = split[0].Split(' ', SplitOptions).Select(int.Parse).ToArray();
        GuessedNumbers = split[1].Split(' ', SplitOptions).Select(int.Parse).ToArray();
    }

    public int[] CorrectNumbers { get; set; }
    public int[] GuessedNumbers { get; set; }
    public IEnumerable<int> WinningNumbers => CorrectNumbers.Intersect(GuessedNumbers);

    public int AmountOfWinningNumbers => WinningNumbers.Count();
    public int CardWorth => (int)Math.Pow(2, AmountOfWinningNumbers - 1);
}