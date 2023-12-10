using System.Text;

internal class Program
{
    public const StringSplitOptions SplitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

    private static void Main(string[] args)
    {
        var list = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt"));

        var parsed = Parse(list, out var mainLoop);

        var sum = Challenge1(parsed);
        var sumCh2 = Challenge2(list, parsed, mainLoop);

        Console.WriteLine("Ch1: " + sum);
        Console.WriteLine("Ch2: " + sumCh2);
        Console.ReadKey();
    }

    private static string Challenge2(string[] input, Pipe startPipe, Dictionary<(int x, int y), Pipe> pipes)
    {
        SetStartChar(input, startPipe);
        var allPipes = startPipe;

        var length = input[0].Length * 3;
        var height = input.Length * 3;
        var grid = Getx3ResoultionGrid(input, length, height, pipes);

        FillFromOutside(grid);

        var count = CountFullTiles(grid);

        PrintGrid(grid);

        return count.ToString(); ;
    }

    private static int CountFullTiles(int[,] grid)
    {
        int count = 0;
        for (int y = 0; y < grid.GetLength(1); y += 3)
        {
            for (int x = 0; x < grid.GetLength(0); x += 3)
            {
                if (IsFullTileInside(grid, x, y))
                {
                    grid[x, y] = 100 + count;
                    count++;
                }
            }
        }

        return count;
    }

    private static bool IsFullTileInside(int[,] grid, int x, int y)
    {
        for (int xOff = 0; xOff < 3; xOff++)
        {
            for (int yOff = 0; yOff < 3; yOff++)
            {
                if (grid[x + xOff, y + yOff] != 0)
                    return false;
            }
        }

        return true;
    }

    private static void PrintGrid(int[,] grid)
    {
        for (int y = 0; y < grid.GetLength(1); y++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                if (grid[x, y] == 0)
                    Console.Write("~");
                else if (grid[x, y] == 1)
                    Console.Write("#");
                else if (grid[x, y] == 2)
                    Console.Write(" ");
                else if (grid[x, y] == 5)
                    Console.Write("+");
                else if (grid[x, y] >= 100)
                    Console.Write(grid[x, y] % 10);
                else
                    throw new Exception();
            }
            Console.WriteLine();
        }
    }

    private static void FillFromOutside(int[,] grid)
    {
        while (TryGetOuterPosition(grid, out var pos))
        {
            var res = FlowThrough(grid, pos);

            Queue<(int x, int y)> toDo = new(res);
            //grid[pos.x, pos.y] = 5;
            while (toDo.TryDequeue(out var next))
            {
                foreach (var item in FlowThrough(grid, next))
                {
                    toDo.Enqueue(item);
                }
            }
        }
    }

    private static IEnumerable<(int x, int y)> FlowThrough(int[,] grid, (int x, int y) pos)
    {
        if (pos.x >= 0 && pos.x < grid.GetLength(0) && pos.y >= 0 && pos.y < grid.GetLength(1))
        {
            if (grid[pos.x, pos.y] == 0)
            {
                grid[pos.x, pos.y] = 2;
                for (int xOff = -1; xOff < 2; xOff++)
                {
                    for (int yOff = -1; yOff < 2; yOff++)
                    {
                        if (xOff == 0 && yOff == 0)
                            continue;

                        //FlowThrough(grid, (pos.x + xOff, pos.y + yOff));
                        yield return (pos.x + xOff, pos.y + yOff);
                    }
                }
            }
        }
    }

    private static bool TryGetOuterPosition(int[,] grid, out (int x, int y) pos)
    {
        int lastY = grid.GetLength(1) - 1;
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            if (grid[x, 0] == 0)
            {
                pos = (x, 0);
                return true;
            }

            if (grid[x, lastY] == 0)
            {
                pos = (x, lastY);
                return true;
            }
        }

        int lastX = grid.GetLength(0) - 1;
        for (int y = 0; y < grid.GetLength(1); y++)
        {
            if (grid[0, y] == 0)
            {
                pos = (0, y);
                return true;
            }

            if (grid[lastX, y] == 0)
            {
                pos = (lastX, y);
                return true;
            }
        }

        pos = (-1, -1);
        return false;
    }

    private static int[,] Getx3ResoultionGrid(string[] input, int length, int height, Dictionary<(int x, int y), Pipe> pipes)
    {
        int[,] grid = new int[length, height];

        for (int y = 0; y < input.Length; y++)
        {
            string line = input[y];
            for (int x = 0; x < line.Length; x++)
            {
                char c = line[x];
                InitGrid(ref grid, c, x, y, pipes);
            }
        }

        return grid;
    }

    private static void InitGrid(ref int[,] grid, char c, int x, int y, Dictionary<(int x, int y), Pipe> pipes)
    {
        if (!pipes.ContainsKey((x, y)))
        {
            return;
        }

        int offsetX = x * 3;
        int offsetY = y * 3;

        switch (c)
        {
            case '|':
                grid[offsetX + 1, offsetY + 0] = 1;
                grid[offsetX + 1, offsetY + 1] = 1;
                grid[offsetX + 1, offsetY + 2] = 1;
                break;
            case '-':
                grid[offsetX + 0, offsetY + 1] = 1;
                grid[offsetX + 1, offsetY + 1] = 1;
                grid[offsetX + 2, offsetY + 1] = 1;
                break;
            case 'L':
                grid[offsetX + 1, offsetY + 0] = 1;
                grid[offsetX + 1, offsetY + 1] = 1;
                grid[offsetX + 2, offsetY + 1] = 1;

                //grid[offsetX + 0, offsetY + 0] = 1;
                //grid[offsetX + 0, offsetY + 1] = 1;
                //grid[offsetX + 0, offsetY + 2] = 1;
                //grid[offsetX + 1, offsetY + 2] = 1;
                //grid[offsetX + 2, offsetY + 2] = 1;
                break;
            case 'J':
                grid[offsetX + 1, offsetY + 0] = 1;
                grid[offsetX + 1, offsetY + 1] = 1;
                grid[offsetX + 0, offsetY + 1] = 1;

                //grid[offsetX + 2, offsetY + 0] = 1;
                //grid[offsetX + 2, offsetY + 1] = 1;
                //grid[offsetX + 2, offsetY + 2] = 1;
                //grid[offsetX + 1, offsetY + 2] = 1;
                //grid[offsetX + 0, offsetY + 2] = 1;
                break;
            case '7':
                grid[offsetX + 0, offsetY + 1] = 1;
                grid[offsetX + 1, offsetY + 1] = 1;
                grid[offsetX + 1, offsetY + 2] = 1;


                //grid[offsetX + 0, offsetY + 0] = 1;
                //grid[offsetX + 1, offsetY + 0] = 1;
                //grid[offsetX + 2, offsetY + 0] = 1;
                //grid[offsetX + 2, offsetY + 1] = 1;
                //grid[offsetX + 2, offsetY + 2] = 1;
                break;
            case 'F':
                grid[offsetX + 2, offsetY + 1] = 1;
                grid[offsetX + 1, offsetY + 1] = 1;
                grid[offsetX + 1, offsetY + 2] = 1;

                //grid[offsetX + 2, offsetY + 2] = 1;
                //grid[offsetX + 1, offsetY + 2] = 1;
                //grid[offsetX + 0, offsetY + 2] = 1;
                //grid[offsetX + 0, offsetY + 1] = 1;
                //grid[offsetX + 0, offsetY + 0] = 1;

                break;
            case '.':
                break;
            default:
                throw new Exception();
        }
    }

    private static void SetStartChar(string[] input, Pipe startPipe)
    {
        var startPipeChar = GetTypeFromStartPipe(startPipe);

        var builder = new StringBuilder(input[startPipe.Pos.y]);
        builder[startPipe.Pos.x] = startPipeChar;

        input[startPipe.Pos.y] = builder.ToString();
    }

    private static char GetTypeFromStartPipe(Pipe startPipe)
    {
        var fromStartConnections = new List<PipeConnections>();
        foreach (var item in startPipe.ConnectedPipes)
        {
            if (item.Pos.x == startPipe.Pos.x && item.Pos.y == startPipe.Pos.y - 1)
                fromStartConnections.Add(PipeConnections.Top);
            else if (item.Pos.x == startPipe.Pos.x && item.Pos.y == startPipe.Pos.y + 1)
                fromStartConnections.Add(PipeConnections.Bottom);
            else if (item.Pos.x == startPipe.Pos.x - 1 && item.Pos.y == startPipe.Pos.y)
                fromStartConnections.Add(PipeConnections.Left);
            else if (item.Pos.x == startPipe.Pos.x + 1 && item.Pos.y == startPipe.Pos.y)
                fromStartConnections.Add(PipeConnections.Right);
            else
                throw new Exception();
        }

        if (fromStartConnections.Contains(PipeConnections.Top) && fromStartConnections.Contains(PipeConnections.Bottom))
            return '|';
        else if (fromStartConnections.Contains(PipeConnections.Left) && fromStartConnections.Contains(PipeConnections.Right))
            return '-';
        else if (fromStartConnections.Contains(PipeConnections.Top) && fromStartConnections.Contains(PipeConnections.Right))
            return 'L';
        else if (fromStartConnections.Contains(PipeConnections.Top) && fromStartConnections.Contains(PipeConnections.Left))
            return 'J';
        else if (fromStartConnections.Contains(PipeConnections.Bottom) && fromStartConnections.Contains(PipeConnections.Left))
            return '7';
        else if (fromStartConnections.Contains(PipeConnections.Bottom) && fromStartConnections.Contains(PipeConnections.Right))
            return 'F';
        else
            throw new Exception();
    }




    #region Ch1
    private static string Challenge1(Pipe startPipe)
    {
        return startPipe.GetFarthestPipe(ref startPipe).WaterLevel.ToString();
    }

    private static Pipe Parse(string[] list, out Dictionary<(int x, int y), Pipe> pipes)
    {
        (int x, int y) startPos = GetStartPos(list);

        var discovered = new Dictionary<(int x, int y), Pipe>();
        var toDo = new Queue<(Pipe pipe, int waterLevel)>();
        var startPipe = new Pipe(list, startPos);

        toDo.Enqueue((startPipe, 0));

        while (toDo.TryDequeue(out var el))
        {
            if (discovered.ContainsKey(el.pipe.Pos))
                continue;
            discovered.Add(el.pipe.Pos, el.pipe);

            var nexts = el.pipe.FlowThroughDiscoverNexts(list, el.waterLevel);
            el.pipe.ConnectedPipes.AddRange(nexts);

            foreach (var item in nexts)
            {
                toDo.Enqueue((item, el.waterLevel + 1));
            }
        }

        pipes = discovered;
        return startPipe;
    }

    private static (int x, int y) GetStartPos(string[] list)
    {
        //find S in list
        for (int y = 0; y < list.Length; y++)
        {
            for (int x = 0; x < list[y].Length; x++)
            {
                if (list[y][x] == 'S')
                    return (x, y);
            }
        }
        throw new Exception();
    }
    #endregion
}

public enum PipeConnections : int { Top, Left, Right, Bottom };
public class Pipe
{
    public PipeConnections[] Connections { get; set; }
    public char CurrentSymbol { get; set; }
    public (int x, int y) Pos { get; set; }

    public List<Pipe> ConnectedPipes { get; set; } = new();

    public int WaterLevel { get; set; } = -1;

    public Pipe(string[] input, (int x, int y) pos)
    {
        Pos = pos;
        CurrentSymbol = input[pos.y][pos.x];
        Connections = GetConnectionsFromSymbol(CurrentSymbol);
    }

    public List<Pipe> FlowThroughDiscoverNexts(string[] input, int waterLevel)
    {
        WaterLevel = waterLevel;
        var connectedPipes = new List<Pipe>();
        foreach (var dir in Connections)
        {
            var nextPos = GetNextPositions(Pos, dir);

            if (nextPos.x < 0 || nextPos.x > input[0].Length || nextPos.y < 0 || nextPos.y > input.Length)
                continue;

            var nextPipe = new Pipe(input, nextPos);

            if (!IsConnected(dir, nextPipe))
                continue;

            connectedPipes.Add(nextPipe);
        }
        return connectedPipes;
    }


    private bool IsConnected(PipeConnections dir, Pipe nextPipe)
    {
        switch (dir)
        {
            case PipeConnections.Top:
                return nextPipe.Connections.Contains(PipeConnections.Bottom);
            case PipeConnections.Left:
                return nextPipe.Connections.Contains(PipeConnections.Right);
            case PipeConnections.Right:
                return nextPipe.Connections.Contains(PipeConnections.Left);
            case PipeConnections.Bottom:
                return nextPipe.Connections.Contains(PipeConnections.Top);
        }
        throw new NotImplementedException();
    }

    private (int x, int y) GetNextPositions((int x, int y) pos, PipeConnections dir)
    {
        switch (dir)
        {
            case PipeConnections.Top:
                return (pos.x, pos.y - 1);
            case PipeConnections.Left:
                return (pos.x - 1, pos.y);
            case PipeConnections.Right:
                return (pos.x + 1, pos.y);
            case PipeConnections.Bottom:
                return (pos.x, pos.y + 1);
        }
        throw new NotImplementedException();
    }

    private PipeConnections[] GetConnectionsFromSymbol(char symbol)
    {
        switch (symbol)
        {
            case '|':
                return new PipeConnections[] { PipeConnections.Top, PipeConnections.Bottom };
            case '-':
                return new PipeConnections[] { PipeConnections.Left, PipeConnections.Right };
            case 'L':
                return new PipeConnections[] { PipeConnections.Top, PipeConnections.Right };
            case 'J':
                return new PipeConnections[] { PipeConnections.Top, PipeConnections.Left };
            case '7':
                return new PipeConnections[] { PipeConnections.Bottom, PipeConnections.Left };
            case 'F':
                return new PipeConnections[] { PipeConnections.Bottom, PipeConnections.Right };
            case '.':
                return new PipeConnections[] { };
            case 'S':
                return new PipeConnections[] { PipeConnections.Top, PipeConnections.Bottom, PipeConnections.Left, PipeConnections.Right };
        }

        throw new NotImplementedException();
    }

    internal Pipe GetFarthestPipe(ref Pipe farthest)
    {
        if (this.WaterLevel > farthest.WaterLevel)
            farthest = this;

        foreach (var item in ConnectedPipes)
        {
            if (item.WaterLevel > this.WaterLevel)
                farthest = item.GetFarthestPipe(ref farthest);
        }

        return farthest;
    }

}