using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day12
{
    public enum CharType : byte { Dot, Hash, QuestionMark }
    public record Line(string LineInput, int[] Streaks, CharType[] Chars);
    public class Part2
    {
        public List<Line> Lines { get; set; }
        public Part2(string[] input)
        {
            Lines = input.Select(line =>
            {
                var split = line.Split(" ");

                var streaks = split[1].Split(",").Select(int.Parse).ToArray();

                var charTypes = split[0].Select(y => y switch
                {
                    '.' => CharType.Dot,
                    '#' => CharType.Hash,
                    '?' => CharType.QuestionMark,
                    _ => throw new Exception()
                }).ToList();

                //return new Line(streaks, charTypes.ToArray());
                var streaksx5 = Enumerable.Repeat(streaks, 5).SelectMany(x => x).ToArray();

                charTypes.Add(CharType.QuestionMark);
                var charTypesx5 = Enumerable.Repeat(charTypes, 5).SelectMany(x => x).ToList();
                charTypesx5.RemoveAt(charTypesx5.Count - 1);

                return new Line(line, streaksx5, charTypesx5.ToArray());
            }).ToList();

        }

        long Sum = 0;
        public long Main()
        {
            long localSum = 0;
            var stopWatch = new Stopwatch();
            foreach (var line in Lines)
            {
                stopWatch.Start();
                CountMe(line.Streaks, 0, line.Chars.AsSpan(), new());
                stopWatch.Stop();


                localSum += Sum;

                Console.WriteLine(line.LineInput);
                Console.WriteLine(stopWatch.ElapsedMilliseconds);
                Console.WriteLine("Sum " + Sum);
                Console.WriteLine();
                Sum = 0;
            }

            Console.WriteLine("Result " + localSum);
            return Sum;
        }

        private void CountMe(Span<int> streaks, int inputCurrIdx, Span<CharType> span, List<bool> soFar)
        {
            var currIdx = inputCurrIdx;
            if (streaks.Length == 0)
            {
                if (None(currIdx, span, CharType.Hash))
                {
                    Console.WriteLine(string.Join(" ", soFar.Select(x => x ? "#" : ".")));
                    Sum++;
                    return;
                }

                return;
            }

            while (currIdx < span.Length && span[currIdx] == CharType.Dot)
            {
                soFar.Add(false);
                currIdx++;
            }

            if (currIdx >= span.Length)
                return;

            if (MinCharactersNeededLeft(streaks) + currIdx > span.Length)
            {
                return;
            }

            int amountOfHashAndQuest = 1;

            while (currIdx + amountOfHashAndQuest < span.Length && span[currIdx + amountOfHashAndQuest] != CharType.Dot)
            {
                amountOfHashAndQuest++;
            }


            int nextIdxWithHashOrQuest = currIdx + amountOfHashAndQuest + 1;
            while (nextIdxWithHashOrQuest < span.Length && span[nextIdxWithHashOrQuest] == CharType.Dot)
            {
                nextIdxWithHashOrQuest++;
            }


            if (streaks[0] > amountOfHashAndQuest)
            {
                if (Any(span, inputCurrIdx, Math.Min(span.Length, nextIdxWithHashOrQuest), CharType.Hash))
                    return;

                for (int i = currIdx; i < nextIdxWithHashOrQuest; i++)
                {
                    soFar.Add(false);
                }

                CountMe(streaks, nextIdxWithHashOrQuest, span, soFar.ToList());
                return;
            }

            var streaks0 = streaks[0];
            var streaksWithoutStreak0 = streaks.Slice(1);
            var amountOfOptions = amountOfHashAndQuest - streaks0 + 1;


            for (int i = 0; i < amountOfOptions; i++)
            {
                if (currIdx + streaks0 + i > span.Length)
                    throw new Exception("Shouldnt happen");

                //if(Any(span, inputCurrIdx + streaks0 + i, inputCurrIdx + amountOfHashAndQuest, CharType.Hash))
                //    continue;

                if (currIdx + streaks0 + i < span.Length && span[currIdx + streaks0 + i] == CharType.Hash)
                {
                    if (span[currIdx + i] == CharType.Hash)
                        break;

                    continue;
                }

                var localSoFar = soFar.ToList();
                for (int j = 0; j < i; j++)
                {
                    localSoFar.Add(false);
                }
                for (int j = 0; j < streaks0; j++)
                {
                    localSoFar.Add(true);
                }

                localSoFar.Add(false);

                CountMe(streaksWithoutStreak0, currIdx + streaks0 + i + 1, span, localSoFar.ToList());

                if (span[currIdx + i] == CharType.Hash)
                    break;
            }

            for (int i = currIdx; i < nextIdxWithHashOrQuest; i++)
            {
                soFar.Add(false);
            }

            if (Any(span, inputCurrIdx, Math.Min(span.Length, nextIdxWithHashOrQuest), CharType.Hash))
                return;

            CountMe(streaks, nextIdxWithHashOrQuest, span, soFar.ToList());
        }

        private bool Any(Span<CharType> span, int startIdx_Incl, int endIdx_Excl, CharType hash)
        {
            for (int i = startIdx_Incl; i < endIdx_Excl; i++)
            {
                if (span[i] == hash)
                    return true;
            }
            return false;
        }

        public int MinCharactersNeededLeft(Span<int> streaks)
        {
            int sum = 0;
            for (int i = 0; i < streaks.Length; i++)
            {
                sum += streaks[i];
            }
            return sum + streaks.Length - 1;
        }

        bool None(int startIdx, Span<CharType> span, CharType chType)
        {
            for (int i = startIdx; i < span.Length; i++)
            {
                if (span[i] == chType)
                    return false;
            }
            return true;
        }

    }
}
