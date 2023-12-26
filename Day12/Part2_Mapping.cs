using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day12
{

    public class Part2_Mapping
    {
        public Dictionary<int, Dictionary<int, long>> Idx_StreaksDone_Multiply { get; set; } = new();
        public Line Line { get; set; }
        public Part2_Mapping(Line line)
        {
            Line = line;
            Idx_StreaksDone_Multiply.Add(0, new Dictionary<int, long>() { { 0, 1 } });
        }

        public long GetAmount()
        {
            int idx = 0;
            while (idx < Line.Chars.Length && Idx_StreaksDone_Multiply.Any())
            {
                if (!Idx_StreaksDone_Multiply.TryGetValue(idx, out var streaksDone_Multiplys))
                {
                    idx++;
                    continue;
                }

                foreach (var streaksDone_Multiply in streaksDone_Multiplys)
                {
                    //check remove
                    if (streaksDone_Multiply.Key == Line.Streaks.Length)
                    {
                        if (idx < Line.EarliestDoneIdx)
                        {
                            streaksDone_Multiplys.Remove(streaksDone_Multiply.Key);
                            continue;
                        }

                        continue;
                    }
                    else
                    {
                        streaksDone_Multiplys.Remove(streaksDone_Multiply.Key);
                    }


                    //on dot go next
                    if (idx < Line.Chars.Length && Line.Chars[idx] == CharType.Dot)
                    {
                        //Debug.WriteLine(idx);
                        Commit_Idx_StreaksDone_Add(idx + 1, streaksDone_Multiply.Key, streaksDone_Multiply.Value);
                        continue;
                    }

                    //try to get one more streak done for ? and #
                    foreach (var nextIdx in GetNextIdxWithNextStreak(idx, streaksDone_Multiply.Key))
                    {
                        //Debug.WriteLine(nextIdx);
                        Commit_Idx_StreaksDone_Add(nextIdx, streaksDone_Multiply.Key + 1, streaksDone_Multiply.Value);
                    }

                    //if current is ? -> also skip
                    if (idx < Line.Chars.Length && Line.Chars[idx] == CharType.QuestionMark)
                    {
                        int questionMarkIdx = idx;
                        while (questionMarkIdx < Line.Chars.Length && Line.Chars[questionMarkIdx] == CharType.QuestionMark)
                        {
                            questionMarkIdx++;
                        }

                        //Debug.WriteLine(idx);
                        if (questionMarkIdx < Line.Chars.Length && Line.Chars[questionMarkIdx] == CharType.Dot)
                            Commit_Idx_StreaksDone_Add(questionMarkIdx, streaksDone_Multiply.Key, streaksDone_Multiply.Value);
                    }

                }

                //Idx_StreaksDone_Multiply.Remove(idx);
                idx++;
            }

            foreach (var item in Idx_StreaksDone_Multiply)
            {
                if (!item.Value.Any())
                    Idx_StreaksDone_Multiply.Remove(item.Key);

                foreach (var resuultItem in item.Value)
                {
                    if (resuultItem.Key != Line.Streaks.Length)
                        item.Value.Remove(resuultItem.Key);
                }
            }
            return Idx_StreaksDone_Multiply.Sum(x => x.Value.Sum(y => y.Value));
        }

        private void Commit_Idx_StreaksDone_Add(int nextIdx, int key, long value)
        {
            if (Idx_StreaksDone_Multiply.TryGetValue(nextIdx, out var nextStreaksDone_Multiplys))
            {
                if (nextStreaksDone_Multiplys.TryGetValue(key, out var multiply))
                {
                    nextStreaksDone_Multiplys[key] = multiply + value;
                }
                else
                {
                    nextStreaksDone_Multiplys.Add(key, value);
                }
            }
            else
            {
                Idx_StreaksDone_Multiply.Add(nextIdx, new Dictionary<int, long>() { { key, value } });
            }
        }
                
        private IEnumerable<int> GetNextIdxWithNextStreak(int baseIdx, int streaksDone)
        {
            if (streaksDone >= Line.Streaks.Length)
                yield break;

            var nextStreak = Line.Streaks[streaksDone];

            int idxLimit = Line.Chars.Length + 1;
            int i = 0;
            while (i + baseIdx < idxLimit)
            {
                int iIdx = baseIdx + i;
                if (iIdx >= Line.Chars.Length || Line.Chars[iIdx] == CharType.Dot)
                {
                    if (i >= nextStreak)
                        yield return iIdx + 1;
                    yield break;
                }

                if (i >= nextStreak
                    && (iIdx >= Line.Chars.Length || Line.Chars[iIdx] != CharType.Hash))
                    yield return iIdx + 1;

                if (Line.Chars[iIdx] == CharType.Hash)
                {
                    var potentialLimit = iIdx + nextStreak + 1;
                    if (idxLimit > potentialLimit)
                        idxLimit = potentialLimit;
                }

                i++;
            }

        }
    }
}
