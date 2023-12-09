using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

internal class Program
{
    private static void Main(string[] args)
    {
        var list = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt"));

        var parsed = Parse(list);

        //var sum = Challenge1(parsed);
        var sumCh2 = Challenge2(parsed);

        //Console.WriteLine("Ch1: " + sum);
        Console.WriteLine("Ch2: " + sumCh2);
        Console.ReadKey();
    }

    private static long Challenge2(MapWithInstructions parsed)
    {
        string[] loopStarts = parsed.Nodes.Where(x => x.Key.EndsWith("A")).Select(x => x.Key).ToArray();
        LoopDiLoop[] loops = loopStarts.Select(x => new LoopDiLoop() { LoopStart = x }).ToArray();


        foreach (var item in loops)
        {
            int idx = 0;
            int currIdx = 0;
            string curr = item.LoopStart;

            item.Visited.Add((0, curr));

            do
            {
                curr = parsed.Instructions[currIdx] ? parsed.Nodes[curr].L : parsed.Nodes[curr].R;

                currIdx++;
                idx++;

                if (currIdx >= parsed.Instructions.Length)
                    currIdx = 0;

                if (curr[^1] == 'Z')
                    item.FoundZ = (currIdx, curr);
            }
            while (!item.HasAlreadyVisited((currIdx, curr)));
        }

        var maxZOffsetLoop = loops.MaxBy(x => x.Offset)!;
        var diffsOfLoops = loops.Select(x => (maxZOffsetLoop.Offset + maxZOffsetLoop.IdxOfZInLoop - x.Offset - x.IdxOfZInLoop) % x.LoopLength).ToArray();

        var startVars = loops.Select(x => (maxZOffsetLoop.ZIdxGlobal - x.ZIdxGlobal) % x.LoopLength).ToArray();

        long runs = 0;
        bool isDone = false;
        while (!isDone)
        {
            isDone = true;
            for (int i = 0; i < startVars.Length; i++)
            {
                startVars[i] = (startVars[i] + diffsOfLoops[i]) % loops[i].LoopLength;

                if (startVars[i] != 0)
                    isDone = false;
            }
            runs++;
        }

        return runs * maxZOffsetLoop.LoopLength + maxZOffsetLoop.ZIdxGlobal;
    }

    private static MapWithInstructions Parse(string[] list)
    {
        return new MapWithInstructions(list);
    }
}

public class LoopDiLoop
{
    public int Offset { get; set; }
    public int LoopLength { get; set; }
    public int IdxOfZInLoop { get; set; }

    public List<(int idx, string currEl)> Visited { get; set; } = new();
    public (int idx, string newVal) FoundZ { get; set; } = new();
    public required string LoopStart { get; set; }
    public int ZIdxGlobal { get; set; }

    public bool HasAlreadyVisited((int idx, string currEl) el)
    {
        if (Visited.Contains(el))
        {
            Visited.Add(el);
            SubmitLoopPoint(el);
            return true;
        }

        Visited.Add(el);
        return false;
    }

    private void SubmitLoopPoint((int idx, string currEl) el)
    {
        Offset = Visited.IndexOf(el);
        LoopLength = Visited.Count - Offset - 1;
        ZIdxGlobal = Visited.IndexOf(FoundZ);
        IdxOfZInLoop = Visited.IndexOf(FoundZ) - Offset;
    }
}

public class MapWithInstructions
{
    const StringSplitOptions SplitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
    public bool[] Instructions { get; set; }
    public Dictionary<string, (string L, string R)> Nodes { get; set; } = new();

    public MapWithInstructions(string[] input)
    {
        Instructions = input[0].Select(x => x == 'L').ToArray();

        foreach (var x in input.Skip(2))
        {
            var split = x.Split('=', SplitOptions);
            var key = split[0];
            var values = split[1].Split(',', SplitOptions);
            values[0] = values[0].Substring(1);
            values[1] = values[1].Substring(0, values[1].Length - 1);

            Nodes.Add(key, (values[0], values[1]));
        }
    }

}