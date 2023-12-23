using System.Diagnostics.CodeAnalysis;

internal class Program
{
    public const StringSplitOptions SplitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

    private static void Main(string[] args)
    {
        var list = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt"));

        var parsed = Parse(list);

        var sum = Challenge1(parsed); //31956
        var sumCh2 = Challenge2(parsed);

        Console.WriteLine("Ch1: " + sum);
        Console.WriteLine("Ch2: " + sumCh2);
        Console.ReadKey();
    }

    private static int Challenge2(List<Ground> parsed)
    {
        foreach (var ground in parsed)
        {
            (int? hor, int? vert) expected = (ground.HorizontalIdx, ground.VerticalIdx);

            ground.NewIdx = MakeOneChange_And_GetNewMirrorIdx(ground, expected.hor, expected.vert);
        }

        int sum = 0;
        foreach (var ground in parsed)
        {
            if (ground.NewIdx.hor != null)
            {
                sum += 100 * (ground.NewIdx.hor.Value + 1);
            }

            if (ground.NewIdx.ver != null)
            {
                sum += (ground.NewIdx.ver.Value + 1);
            }
        }

        return sum;
    }

    private static (int? hor, int? ver) MakeOneChange_And_GetNewMirrorIdx(Ground ground, int? oldHorIdx, int? oldVertIdx)
    {
        for (int lineIdx = 0; lineIdx < ground.Floor.Count; lineIdx++)
        {
            var line = ground.Floor[lineIdx];

            for (int i = 0; i < line.Count; i++)
            {
                line[i] = !line[i];
                ground.FlippedFloor_ForVertical[i][lineIdx] = !ground.FlippedFloor_ForVertical[i][lineIdx];
                (int? hor, int? vert) newIdx = GetMirrorLineIdx(ground, oldHorIdx, oldVertIdx);
                line[i] = !line[i];
                ground.FlippedFloor_ForVertical[i][lineIdx] = !ground.FlippedFloor_ForVertical[i][lineIdx];

                if (newIdx.hor != null && newIdx.hor != oldHorIdx)
                {
                    return (newIdx.hor, null);
                }

                if (newIdx.vert != null && newIdx.vert != oldVertIdx)
                {
                    return (null, newIdx.vert);
                }
            }

        }

        throw new Exception("Nothing found");
    }

    public static (int? hor, int? vert) GetMirrorLineIdx(Ground ground, int? oldHorIdx, int? oldVertIdx)
    {
        int? hor = GetHorizontalMirrorLineIdx(ground, oldHorIdx);
        int? vert = GetVerticalMirrorLineIdx(ground, oldVertIdx);

        return (hor, vert);
    }

    private static object Challenge1(List<Ground> parsed)
    {
        int sum = 0;
        foreach (var ground in parsed)
        {
            int? horizontalMirrorLineIdx = GetHorizontalMirrorLineIdx(ground, null);
            if (horizontalMirrorLineIdx == null)
            {
                int? verticalMirrorLineIdx = GetVerticalMirrorLineIdx(ground, null);
                if (verticalMirrorLineIdx == null)
                    throw new Exception("Nothing found");

                sum += (verticalMirrorLineIdx.Value + 1);
                ground.VerticalIdx = verticalMirrorLineIdx;

                continue;
            }

            sum += 100 * (horizontalMirrorLineIdx.Value + 1);
            ground.HorizontalIdx = horizontalMirrorLineIdx;
        }

        return sum;
    }

    private static int? GetVerticalMirrorLineIdx(Ground ground, int? cantBeThisNumber)
    {
        for (int i = 0; i < ground.FlippedFloor_ForVertical.Count - 1; i++)
        {
            var line = ground.FlippedFloor_ForVertical[i];
            var nextLine = ground.FlippedFloor_ForVertical[i + 1];

            if (line.SequenceEqual(nextLine))
            {
                if (i != cantBeThisNumber && CheckFullSymmetryVertical(ground, i))
                    return i;
            }

        }

        return null;
    }

    private static bool CheckFullSymmetryVertical(Ground ground, int i)
    {
        int lower = i;
        int higher = i + 1;

        while (lower > -1 && higher < ground.FlippedFloor_ForVertical.Count)
        {
            var lowerLine = ground.FlippedFloor_ForVertical[lower];
            var higherLine = ground.FlippedFloor_ForVertical[higher];

            if (!lowerLine.SequenceEqual(higherLine))
            {
                return false;
            }

            lower--;
            higher++;
        }

        return true;
    }

    private static int? GetHorizontalMirrorLineIdx(Ground ground, int? cantBeThisNumber)
    {
        for (int i = 0; i < ground.Floor.Count - 1; i++)
        {
            var line = ground.Floor[i];
            var nextLine = ground.Floor[i + 1];

            if (line.SequenceEqual(nextLine))
            {
                if (i != cantBeThisNumber && CheckFullSymmetry(ground, i))
                    return i;
            }

        }

        return null;
    }

    private static bool CheckFullSymmetry(Ground ground, int i)
    {
        int lower = i;
        int higher = i + 1;

        while (lower > -1 && higher < ground.Floor.Count)
        {
            var lowerLine = ground.Floor[lower];
            var higherLine = ground.Floor[higher];

            if (!lowerLine.SequenceEqual(higherLine))
            {
                return false;
            }

            lower--;
            higher++;
        }

        return true;
    }

    private static List<Ground> Parse(string[] list)
    {
        List<Ground> grounds = new();

        List<List<bool>> currFloor = new();

        foreach (var line in list)
        {
            if (line == "")
            {
                grounds.Add(new Ground(currFloor));
                currFloor = new();
                continue;
            }

            currFloor.Add(line.Select(x => x == '#').ToList());
        }


        grounds.Add(new Ground(currFloor));

        return grounds;
    }
}

public class Ground
{
    public List<List<bool>> Floor { get; set; }
    public List<List<bool>> FlippedFloor_ForVertical { get; set; }
    public int? VerticalIdx { get; set; }
    public int? HorizontalIdx { get; set; }
    public (int? hor, int? ver) NewIdx { get; set; }

    public Ground(List<List<bool>> floor)
    {
        Floor = floor;
        FlippedFloor_ForVertical = FlipFloorForVertical(Floor);
    }

    private List<List<bool>> FlipFloorForVertical(List<List<bool>> floor)
    {
        List<List<bool>> flipped = new();

        for (int i = 0; i < floor[0].Count; i++)
        {
            flipped.Add(floor.Select(x => x[i]).ToList());
        }

        return flipped;
    }
}
