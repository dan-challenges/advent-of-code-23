using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day12
{
    public enum CharType : byte { Dot, Hash, QuestionMark }
    public class Line
    {
        public Line(string lineInput, int[] streaks, CharType[] chars)
        {
            LineInput = lineInput;
            Streaks = streaks;
            Chars = chars;
            InitStreakMinCharactersNeeded(Streaks);
            EarliestDoneIdx = Array.LastIndexOf(chars, CharType.Hash) + 1;
        }

        private void InitStreakMinCharactersNeeded(int[] streaks)
        {
            for (int i = 0; i < streaks.Length; i++)
            {
                StreakMinCharactersNeeded.Add(streaks.Length - i, GetStreakMinCharactersNeeded(Streaks.AsSpan().Slice(i)));
            }
        }

        private int GetStreakMinCharactersNeeded(Span<int> streaks)
        {
            int sum = 0;
            for (int i = 0; i < streaks.Length; i++)
            {
                sum += streaks[i];
            }
            return sum + streaks.Length - 1;
        }

        public string LineInput { get; }
        public int[] Streaks { get; }
        public CharType[] Chars { get; }
        public Dictionary<int, int> StreakMinCharactersNeeded { get; set; } = new();
        public int EarliestDoneIdx { get; set; }
    }

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

        int amountDone = 0;
        public long Main()
        {
            long localSum = 0;
            //foreach (var line in Lines)
            Parallel.ForEach(Lines, line =>
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                //var res = CountMe(line, line.Streaks, 0, line.Chars.AsSpan());
                var res = new Part2_Mapping(line).GetAmount();
                stopWatch.Stop();


                localSum += res;
                amountDone++;

                Console.WriteLine("Nr: " + amountDone);
                Console.WriteLine(line.LineInput);
                Console.WriteLine(stopWatch.ElapsedMilliseconds);
                Console.WriteLine("Res " + res);
                Console.WriteLine();
            });

            Console.WriteLine("Result " + localSum);
            return localSum;
        }

        private int CountMe(Line line, Span<int> streaks, int inputCurrIdx, Span<CharType> span)
        {
            int currSum = 0;
            var currIdx = inputCurrIdx;
            if (streaks.Length == 0)
            {
                if (None(currIdx, span, CharType.Hash))
                {
                    return 1;
                }

                return currSum;
            }

            while (currIdx < span.Length && span[currIdx] == CharType.Dot)
            {
                currIdx++;
            }

            if (currIdx >= span.Length)
                return currSum;

            if (line.StreakMinCharactersNeeded[streaks.Length] + currIdx > span.Length)
            {
                return currSum;
            }

            int amountOfHashAndQuest = 0;
            int amountOfQuest = -1;

            while (currIdx + amountOfHashAndQuest < span.Length)
            {
                var el = span[currIdx + amountOfHashAndQuest];
                if (el == CharType.Dot)
                    break;

                amountOfHashAndQuest++;

                if (el == CharType.Hash && amountOfQuest == -1)
                    amountOfQuest = amountOfHashAndQuest - 1;
            }

            int nextIdxWithHashOrQuest = currIdx + amountOfHashAndQuest + 1;
            while (nextIdxWithHashOrQuest < span.Length && span[nextIdxWithHashOrQuest] == CharType.Dot)
            {
                nextIdxWithHashOrQuest++;
            }

            var streaks0 = streaks[0];
            if (streaks0 > amountOfHashAndQuest)
            {
                if (Any(span, inputCurrIdx, Math.Min(span.Length, nextIdxWithHashOrQuest), CharType.Hash))
                    return currSum;

                currSum += CountMe(line, streaks, nextIdxWithHashOrQuest, span);
                return currSum;
            }



            var streaksWithoutStreak0 = streaks.Slice(1);
            var amountOfOptions = amountOfHashAndQuest - streaks0 + 1;

            //search for all patterns with the same streaks amount left and next idx -> then multiple the next countMe with the amount of found patterns
            var streaksLeft = streaks;
            var currStreakToCheck = streaksLeft[0];
            var accumulated = 0;


            while (currStreakToCheck <= amountOfQuest)
            {
                var amountOfOptionsThisLoop = amountOfQuest - currStreakToCheck + 1;
                var nextStreaksLeft = streaksLeft.Slice(1);
                accumulated += currStreakToCheck + 1;


            }


            for (int i = 0; i < amountOfOptions; i++)
            {
                if (currIdx + streaks0 + i > span.Length)
                    throw new Exception("Shouldnt happen");

                if (currIdx + streaks0 + i < span.Length && span[currIdx + streaks0 + i] == CharType.Hash)
                {
                    if (span[currIdx + i] == CharType.Hash)
                        break;

                    continue;
                }

                currSum += CountMe(line, streaksWithoutStreak0, currIdx + streaks0 + i + 1, span);

                if (span[currIdx + i] == CharType.Hash)
                    break;
            }


            if (Any(span, inputCurrIdx, Math.Min(span.Length, nextIdxWithHashOrQuest), CharType.Hash))
                return currSum;

            currSum += CountMe(line, streaks, nextIdxWithHashOrQuest, span);
            return currSum;
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
