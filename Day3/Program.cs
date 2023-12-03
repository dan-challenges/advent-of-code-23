internal class Program
{
    private static void Main(string[] args)
    {

        var list = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt"));

        int sum = Challenge1(list);
        int sumCh2 = Challenge2(list);

        Console.WriteLine("Ch1: " + sum);
        Console.WriteLine("Ch2: " + sumCh2);
        Console.ReadKey();
    }

    public record FoundNumber(string Number, int Start_X, int End_X, int Y)
    {
        internal bool IsCoordinateInNumber(int x, int y)
        {
            return x >= Start_X && x <= End_X && y == Y;
        }
    }

    private static int Challenge2(string[] list)
    {
        var allNumbers = GetAllNumbers(list);

        var sum = 0;

        for (int y = 0; y < list.Length; y++)
        {
            var row = list[y];
            for (int x = 0; x < row.Length; x++)
            {
                if (row[x] == '*')
                {
                    if (HasExactly2NeighbourNumbers(x, y, list, allNumbers, out var foundNumbers))
                    {
                        sum += foundNumbers.Aggregate(1, (curr, x) => curr * int.Parse(x.Number));
                    }
                }
            }
        }

        return sum;
    }

    private static List<FoundNumber> GetAllNumbers(string[] list)
    {
        List<FoundNumber> numbersToSumUp = new();

        for (int y = 0; y < list.Length; y++)
        {
            var row = list[y];
            for (int x = 0; x < row.Length; x++)
            {
                if (char.IsDigit(row[x]))
                {
                    (var startIdx, var endIdx) = GetFullNumberBounds(x, row);
                    numbersToSumUp.Add(new(row.Substring(startIdx, endIdx - startIdx + 1), startIdx, endIdx, y));
                    x = endIdx;
                }
            }
        }

        return numbersToSumUp;
    }

    private static bool HasExactly2NeighbourNumbers(int inputX, int inputY, string[] list, List<FoundNumber> allNumbers, out List<FoundNumber> foundNeighbourNumbers)
    {
        int startIdxY = Math.Max(0, inputY - 1);
        int endIdxY_P1 = Math.Min(list.Length, inputY + 2);

        int startIdxX = Math.Max(0, inputX - 1);
        int endIdxX_P1 = Math.Min(list[0].Length, inputX + 2);

        List<FoundNumber> tempNeighbourNumbers = new();

        for (int x = startIdxX; x < endIdxX_P1; x++)
        {
            for (int y = startIdxY; y < endIdxY_P1; y++)
            {
                tempNeighbourNumbers.AddRange(allNumbers.Where(num => num.IsCoordinateInNumber(x, y)));
            }
        }

        var distinctNeighbours = tempNeighbourNumbers.Distinct().ToList();
        foundNeighbourNumbers = distinctNeighbours;
        return distinctNeighbours.Count == 2;
    }

    private static int Challenge1(string[] list)
    {
        List<string> numbersToSumUp = new();

        for (int y = 0; y < list.Length; y++)
        {
            var row = list[y];
            for (int x = 0; x < row.Length; x++)
            {
                if (char.IsDigit(row[x]))
                {
                    (var startIdx, var endIdx) = GetFullNumberBounds(x, row);
                    bool hasCharacterNextToNumber = HasCharacterNextToNumber(startIdx, endIdx, y, list);

                    if (hasCharacterNextToNumber)
                        numbersToSumUp.Add(row.Substring(startIdx, endIdx - startIdx + 1));

                    x = endIdx;
                }
            }
        }

        return numbersToSumUp.Sum(x => int.Parse(x));
    }

    private static bool HasCharacterNextToNumber(int startIdx, int endIdx, int inputY, string[] list)
    {
        int startIdxY = Math.Max(0, inputY - 1);
        int endIdxY_P1 = Math.Min(list.Length, inputY + 2);

        int startIdxX = Math.Max(0, startIdx - 1);
        int endIdxX_P1 = Math.Min(list[0].Length, endIdx + 2);

        for (int x = startIdxX; x < endIdxX_P1; x++)
        {
            for (int y = startIdxY; y < endIdxY_P1; y++)
            {
                if (char.IsDigit(list[y][x]) || list[y][x] == '.')
                    continue;

                return true;
            }
        }

        return false;
    }

    private static (int startIdx, int endIdx) GetFullNumberBounds(int x, string row)
    {
        int startIdx = x;
        int endIdx = x;

        while (endIdx + 1 < row.Length && char.IsDigit(row[endIdx + 1]))
        {
            endIdx++;
        }

        return (startIdx, endIdx);
    }

}