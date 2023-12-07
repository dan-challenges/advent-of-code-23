using System.ComponentModel.DataAnnotations;

internal class Program
{
    public static Dictionary<char, long> CharsWithValues = new()
    {
        { '2', 2 },
        { '3', 3 },
        { '4', 4 },
        { '5', 5 },
        { '6', 6 },
        { '7', 7 },
        { '8', 8 },
        { '9', 9 },
        { 'T', 10 },
        { 'J', 11 },
        { 'Q', 12 },
        { 'K', 13 },
        { 'A', 14 },
    };
    public static Dictionary<char, long> CharsWithValuesCh2 = new()
    {
        { 'J', 1 },
        { '2', 2 },
        { '3', 3 },
        { '4', 4 },
        { '5', 5 },
        { '6', 6 },
        { '7', 7 },
        { '8', 8 },
        { '9', 9 },
        { 'T', 10 },
        { 'Q', 11 },
        { 'K', 12 },
        { 'A', 13 },
    };

    const StringSplitOptions SplitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
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

    private static long Challenge2(List<Hand> parsed)
    {
        parsed = parsed.OrderBy(x => x.OrderNumberCh2).ToList();
        long sum = 0;
        for (int i = 0; i < parsed.Count; i++)
        {
            sum += parsed[i].Bid * (i + 1);
        }
        return sum;
    }

    private static long Challenge1(List<Hand> parsed)
    {
        parsed = parsed.OrderBy(x => x.OrderNumber).ToList();
        long sum = 0;
        for (int i = 0; i < parsed.Count; i++)
        {
            sum += parsed[i].Bid * (i + 1);
        }
        return sum;
    }

    private static List<Hand> Parse(string[] list)
    {
        return list.Select(x => new Hand(x)).ToList();
    }
}

public class Hand
{
    public Hand(string x)
    {
        var split = x.Split(" ");
        Cards = split[0];
        Bid = long.Parse(split[1]);

        CombinationValue = CalculateCombinationValue();
        CombinationValueCh2 = CalculateCombinationValueCh2();

        HighCardOrderNumber = CalculateHighcardOrderNumber(Program.CharsWithValues);
        HighCardOrderNumberCh2 = CalculateHighcardOrderNumber(Program.CharsWithValuesCh2);

        if (CombinationValue * 10000000 <= HighCardOrderNumber)
            throw new Exception();

        if (CombinationValueCh2 * 10000000 <= HighCardOrderNumberCh2)
            throw new Exception();

        OrderNumber = CombinationValue * 10000000 + HighCardOrderNumber;
        OrderNumberCh2 = CombinationValueCh2 * 10000000 + HighCardOrderNumberCh2;
    }


    public string Cards { get; set; }
    public long Bid { get; set; }

    public long CombinationValue { get; set; }
    public long CombinationValueCh2 { get; private set; }
    public long HighCardOrderNumber { get; set; }
    public long HighCardOrderNumberCh2 { get; private set; }
    public long OrderNumber { get; set; }
    public long OrderNumberCh2 { get; private set; }

    public long CalculateHighcardOrderNumber(Dictionary<char, long> charsWithValuesCh2)
    {
        long sum = 0;
        for (int i = 0; i < Cards.Length; i++)
        {
            sum += charsWithValuesCh2[Cards[i]] * (long)(Math.Pow(17, ((Cards.Length - i) - 1)));
        }
        return sum;
    }


    private long CalculateCombinationValueCh2()
    {
        Dictionary<char, int> CardAmount = new();

        int jokerAmount = 0;
        foreach (var card in Cards)
        {
            if (card == 'J')
            {
                jokerAmount++;
                continue;
            }

            if (!CardAmount.ContainsKey(card))
                CardAmount.Add(card, 1);
            else
                CardAmount[card]++;
        }


        var count = CardAmount.Count;

        if (count == 0)
            return 7;

        CardAmount[CardAmount.MaxBy(x => x.Value).Key] += jokerAmount;

        if (count == 1) //5 same
            return 7;

        if (count == 2) //full house or 4 same
        {
            if (CardAmount.First().Value is 1 or 4)
                return 6; //4 same

            return 5; //full house
        }

        if (count == 3) //3 same or two pair
        {
            if (CardAmount.Any(x => x.Value == 3))
                return 4; //3 same

            return 3; //two pair
        }

        if (count == 4) //one pair
            return 2;

        return 1;
    }

    private int CalculateCombinationValue()
    {
        Dictionary<char, int> CardAmount = new();
        foreach (var card in Cards)
        {
            if (!CardAmount.ContainsKey(card))
                CardAmount.Add(card, 1);
            else
                CardAmount[card]++;
        }

        var count = CardAmount.Count;
        if (count == 1) //5 same
            return 7;

        if (count == 2) //full house or 4 same
        {
            if (CardAmount.First().Value is 1 or 4)
                return 6; //4 same

            return 5; //full house
        }

        if (count == 3) //3 same or two pair
        {
            if (CardAmount.Any(x => x.Value == 3))
                return 4; //3 same

            return 3; //two pair
        }

        if (count == 4) //one pair
            return 2;

        return 1;
    }
}