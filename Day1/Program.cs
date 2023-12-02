using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Threading;

internal class Program
{
    private static void Main(string[] args)
    {
        var list = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt"));
        var list2 = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input2.txt"));

        int sum = Challenge1(list);
        int sumCh2 = Challenge2(list2);

        Console.WriteLine("Ch1: " + sum);
        Console.WriteLine("Ch2: " + sumCh2);
        Console.ReadKey();
    }

    private static int Challenge2(string[] list)
    {
        Dictionary<string, int> numbers = new()
        {
            { "one" , 1 },
            { "two" , 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 },
            { "seven", 7 },
            { "eight", 8 },
            { "nine", 9 },
            { "1", 1 },
            { "2", 2 },
            { "3", 3 },
            { "4", 4 },
            { "5", 5 },
            { "6", 6 },
            { "7", 7 },
            { "8", 8 },
            { "9", 9 },
        };

        var dic = numbers.ToDictionary(x => x.Key, x => x.Key.ToCharArray());

        int sum = 0;
        foreach (var item in list)
        {
            sum += GetNumberOfLineCh2(item, numbers);
        }


        return sum;
    }

    private static int GetNumberOfLineCh2(string item, Dictionary<string, int> numbers)
    {
        var firstIdxs = numbers.ToDictionary(x => x.Key, x => item.IndexOf(x.Key));
        var lastIdxs = numbers.ToDictionary(x => x.Key, x => item.LastIndexOf(x.Key));

        var firstIdx = firstIdxs.Where(x => x.Value != -1).MinBy(x => x.Value);
        var lastIdx = lastIdxs.MaxBy(x => x.Value);

        var firstIdxChar = numbers[firstIdx.Key].ToString()[0];
        var lastIdxChar = numbers[lastIdx.Key].ToString()[0];

        return int.Parse(new string(new char[] { firstIdxChar, lastIdxChar }));
    }


    private static int Challenge1(string[] list)
    {
        int sum = 0;
        foreach (var item in list)
        {
            char firstDig = item.First(char.IsDigit);
            char lastDig = item.Last(char.IsDigit);

            string number = new string(new char[] { firstDig, lastDig });

            sum += int.Parse(number);
        }

        return sum;
    }
}