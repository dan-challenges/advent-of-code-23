using Xunit;

internal class Program
{
    private static void Main(string[] args)
    {
        var list = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt"));

        var parsed = Parse(list);

        long sum = Challenge1(parsed);
        long sumCh2 = Challenge2(parsed);

        //Console.WriteLine("Ch1: " + sum);
        Console.WriteLine("Ch2: " + sumCh2);
        Console.ReadKey();
    }

    private static long Challenge2(Maps maps)
    {
        return maps.GetLowestFinalDestinationForRangeSeedInterpretation();
    }

    private static long Challenge1(Maps maps)
    {
        return maps.GetLowestFinalDestination();
    }

    private static Maps Parse(string[] list)
    {
        return new Maps(list);
    }
}

public class Maps
{
    public List<long> Seeds { get; set; }
    public ComplexMap ComplexMapHead { get; set; }
    public ComplexMap ReversedComplexHead { get; set; }

    public Maps(string[] input)
    {
        Seeds = input[0].Split(' ').Skip(1).Select(long.Parse).ToList();
        ComplexMapHead = new ComplexMap(null, input.Skip(2).ToArray());

        ReversedComplexHead = ComplexMapHead.GetLast();
    }

    public long GetLowestFinalDestination()
    {
        var lowest = long.MaxValue;

        foreach (var seed in Seeds)
        {
            var finalDestination = ComplexMapHead.GetFinalDestination(seed);
            if (finalDestination < lowest)
                lowest = finalDestination;
        }

        return lowest;
    }
    public long GetLowestFinalDestinationForRangeSeedInterpretation()
    {
        List<SeedRange> Start_Range_Pair = new();
        for (int i = 0; i < Seeds.Count; i += 2)
        {
            Start_Range_Pair.Add(new() { Start = Seeds[i], Length = Seeds[i + 1] });
        }

        long lookFor = 0;

        while (true)
        {
            var origins = ReversedComplexHead.GetFinalOrigins(lookFor);

            foreach (var origin in origins)
            {
                foreach (var rangePair in Start_Range_Pair)
                {
                    if (rangePair.IsInRange(origin))
                    {
                        return lookFor;
                    }
                }
            }
            lookFor++;
        }

        throw new Exception();
    }

}

public class SeedRange
{
    public long Start { get; set; }
    public long Length { get; set; }
    public bool IsInRange(long val)
    {
        return val >= Start && val <= Start + Length;
    }
}

public class ComplexMap
{
    public string MapName { get; set; }
    public List<ComplexMapRange> ComplexMapRanges { get; set; } = new();

    public ComplexMap? NextMap { get; set; }
    public ComplexMap? Previous { get; set; }

    public ComplexMap(ComplexMap? prev, string[] remainingInput)
    {
        Assert.Contains(':', remainingInput[0]);

        Previous = prev;

        MapName = remainingInput[0];

        int i = 1;
        while (i < remainingInput.Length && !string.IsNullOrWhiteSpace(remainingInput[i]))
        {
            var line = remainingInput[i];

            var split = line.Split(' ');
            var destinationStart = long.Parse(split[0]);
            var originStart = long.Parse(split[1]);
            var range = long.Parse(split[2]);

            ComplexMapRanges.Add(new ComplexMapRange()
            {
                Destination_Start = destinationStart,
                Origin_Start = originStart,
                Range = range
            });
            i++;
        }

        if (i < remainingInput.Length)
            NextMap = new ComplexMap(this, remainingInput.Skip(i + 1).ToArray());
    }

    public long GetFinalDestination(long origin)
    {
        foreach (var item in ComplexMapRanges)
        {
            if (item.TryGetDestination(origin, out var destination))
                return NextMap?.GetFinalDestination(destination) ?? destination;
        }

        return NextMap?.GetFinalDestination(origin) ?? origin;
    }

    internal ComplexMap GetLast()
    {
        if (NextMap != null)
            return NextMap.GetLast();

        return this;
    }

    internal IEnumerable<long> GetFinalOrigins(long destinationToLookFor)
    {
        bool anyInRange = false;

        foreach (var rangeWhereDestinationIsInRange in ComplexMapRanges)
        {
            if (rangeWhereDestinationIsInRange.IsDestinationInRange(destinationToLookFor))
            {
                anyInRange = true;

                var origin = rangeWhereDestinationIsInRange.GetOrigin(destinationToLookFor);

                if (Previous != null)
                    foreach (var originResult in Previous.GetFinalOrigins(origin))
                    {
                        yield return originResult;
                    }
                else
                    yield return origin;
            }
        }

        if (anyInRange == false)
        {
            var origin = destinationToLookFor;
            if (Previous != null)
                foreach (var originResult in Previous.GetFinalOrigins(origin))
                {
                    yield return originResult;
                }
            else
                yield return origin;
        }
    }
}

public class ComplexMapRange
{
    public long Origin_Start { get; set; }
    public long Destination_Start { get; set; }
    public long Range { get; set; }

    public bool IsOriginInRange(long origin)
    {
        return origin >= Origin_Start && origin <= Origin_Start + Range;
    }

    public bool TryGetDestination(long origin, out long destination)
    {
        destination = (origin - Origin_Start) + Destination_Start;
        return IsOriginInRange(origin);
    }

    internal long GetOrigin(long destinationToLookFor)
    {
        return (destinationToLookFor - Destination_Start) + Origin_Start;
    }

    internal bool IsDestinationInRange(long destinationToLookFor)
    {
        return destinationToLookFor >= Destination_Start && destinationToLookFor <= Destination_Start + Range;
    }
}