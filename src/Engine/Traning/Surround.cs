using System;
using System.Collections.Generic;
using System.Linq;
using FDDC;
using JiebaNet.Segmenter;
using JiebaNet.Segmenter.PosSeg;

public class Surround
{

    Dictionary<string, int> LeadingWordDict = new Dictionary<String, int>();
    Dictionary<string, int> LeadingVerbWordDict = new Dictionary<String, int>();

    Dictionary<string, int> TrailingWordDict = new Dictionary<String, int>();


    public void AnlayzeEntitySurroundWords(HTMLEngine.MyRootHtmlNode root, string KeyWord)
    {
        Program.Training.WriteLine("关键字：[" + KeyWord + "]");
        JiebaSegmenter segmenter = new JiebaSegmenter();
        segmenter.AddWord(KeyWord);
        PosSegmenter posSeg = new PosSegmenter(segmenter);
        foreach (var paragrah in root.Children)
        {
            foreach (var sentence in paragrah.Children)
            {
                var segments = posSeg.Cut(sentence.Content).ToList();  // 默认为精确模式,寻找关键字的位置
                for (int i = 0; i < segments.Count; i++)
                {
                    if (segments[i].Word.Equals(KeyWord))
                    {
                        //前5个词语和后五个词语
                        var startInx = Math.Max(0, i - 5);
                        var EndInx = Math.Min(i + 5, segments.Count);
                        for (int s = startInx; s < i; s++)
                        {
                            if (segments[s].Flag == WordUtility.标点) continue;
                            if (LeadingWordDict.ContainsKey(segments[s].Word))
                            {
                                LeadingWordDict[segments[s].Word]++;
                            }
                            else
                            {
                                LeadingWordDict.Add(segments[s].Word, 1);
                            }
                            Program.Training.WriteLine("前导关键字：[" + segments[s] + "]");

                            //特别关注动词和冒号的情况
                            if (segments[s].Flag == WordUtility.动词)
                            {
                                if (LeadingVerbWordDict.ContainsKey(segments[s].Word))
                                {
                                    LeadingVerbWordDict[segments[s].Word]++;
                                }
                                else
                                {
                                    LeadingVerbWordDict.Add(segments[s].Word, 1);
                                }
                                Program.Training.WriteLine("前导动词:" + segments[s].Word);
                            }
                            if (segments[s].Word == "：")
                            {
                                var leading = "";
                                for (int l = startInx; l < s; l++)
                                {
                                    leading += segments[l];
                                }
                                Program.Training.WriteLine("冒号前导词：" + leading);
                            }
                        }
                        Program.Training.WriteLine("关键字：[" + KeyWord + "]");
                        for (int s = i + 1; s < EndInx; s++)
                        {
                            if (segments[s].Flag == WordUtility.标点) continue;
                            if (TrailingWordDict.ContainsKey(segments[s].Word))
                            {
                                TrailingWordDict[segments[s].Word]++;
                            }
                            else
                            {
                                TrailingWordDict.Add(segments[s].Word, 1);
                            }
                            Program.Training.WriteLine("后续关键字：[" + segments[s] + "]");
                        }
                        return;     //仅统计第一次出现
                    }
                }
            }
        }
    }

    public void WriteTop(int top)
    {
        Program.Training.WriteLine("前导词语");
        Utility.FindTop(top, LeadingWordDict);
        Program.Training.WriteLine("前导动词");
        Utility.FindTop(top, LeadingVerbWordDict);
        Program.Training.WriteLine("后续词语");
        Utility.FindTop(top, TrailingWordDict);
    }

}