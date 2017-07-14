using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ResetCore.Asset;

namespace ResetCore.Util
{
    public class BlockWordChecker
    {

        private static string textFileName = "BlockWord/BlockWord";

        private static BlockWordChecker _instance;
        public static BlockWordChecker Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BlockWordChecker();
                }
                return _instance;
            }
        }

        private BlockWordChecker()
        {
            ReadFromFile();
            AddBlockWordToDict();
        }

        private List<string> blockWordList;
        Dictionary<string, object> DFATable;

        public enum MatchType
        {
            MIN_MATCH_TYPE,
            MAX_MATCH_TYPE
        }

        #region 初始化函数
        //读取文件
        private void ReadFromFile()
        {
            string content = Resources.Load<TextAsset>(textFileName).text;
            string[] blockWords = content.Split('#');
            blockWordList = new List<string>(blockWords);

        }
        //建立DFA树
        private void AddBlockWordToDict()
        {
            DFATable = new Dictionary<string, object>();
            Dictionary<string, object> nowTable;
            Dictionary<string, object> newTable;
            foreach (string word in blockWordList)
            {
                nowTable = DFATable;
                for (int i = 0; i < word.Length; i++)
                {
                    string keyChar = word[i].ToString();
                    object tempTable;
                    if (nowTable.ContainsKey(keyChar))
                    {
                        tempTable = nowTable[keyChar];
                        nowTable = (Dictionary<string, object>)tempTable;
                    }
                    else
                    {
                        //设置标志位
                        newTable = new Dictionary<string, object>();

                        if (i == word.Length - 1)
                        {
                            newTable.Add("isEnd", "1");
                        }
                        else
                        {
                            newTable.Add("isEnd", "0");
                        }

                        nowTable.Add(keyChar, newTable);
                        nowTable = newTable;
                    }
                }
            }
        }
        #endregion

        #region 检查函数
        //是否包含敏感词
        public bool IsContaintBlockWord(string txt, MatchType matchType)
        {
            bool flag = false;
            for (int i = 0; i < txt.Length; i++)
            {
                int matchFlag = CheckBlockWord(txt, i, matchType);
                if (matchFlag > 0)
                {
                    Debug.Log("含有屏蔽词！" + matchFlag + "个");
                    flag = true;
                }
            }

            return flag;
        }
        //获取敏感词
        public HashSet<string> GetBlockWord(string txt, MatchType matchType)
        {
            HashSet<string> sensitiveWordSet = new HashSet<string>();
            for (int i = 0; i < txt.Length; i++)
            {
                //判断是否包含敏感字符
                int lenth = CheckBlockWord(txt, i, matchType);

                if (lenth > 0)
                {
                    sensitiveWordSet.Add(txt.Substring(i, i + lenth));
                    i = i + lenth - 1;
                }
            }
            return sensitiveWordSet;
        }
        //替换敏感字符
        public string ReplaceBlockWord(string txt, MatchType matchType, string replaceChar)
        {
            string resultTxt = txt;
            HashSet<string> set = GetBlockWord(txt, matchType);
            string replaceWord = "";
            foreach (string word in set)
            {
                replaceWord = GetReplaceChars(replaceChar, word.Length);
                resultTxt = resultTxt.Replace(word, replaceWord);
            }
            return resultTxt;
        }
        //获取替换字符串
        private string GetReplaceChars(string replaceChar, int lenth)
        {
            string resultReplace = replaceChar;
            for (int i = 1; i < lenth; i++)
            {
                resultReplace += replaceChar;
            }
            return resultReplace;
        }
        //查找是否包含铭感字符，如果存在返回铭感字符长度否则返回0
        public int CheckBlockWord(string txt, int beginIndex, MatchType matchType)
        {
            bool flag = false;
            int matchFlag = 0;
            Dictionary<string, object> nowTable = DFATable;
            for (int i = beginIndex; i < txt.Length; i++)
            {
                string word = txt[i].ToString();
                //获取指定key

                if (nowTable.ContainsKey(word))
                {
                    nowTable = (Dictionary<string, object>)nowTable[word];
                    //找到相应key
                    matchFlag++;

                    //如果为最后一个匹配规则，结束循环，返回匹配表指数
                    if ("1".Equals(nowTable["isEnd"]))
                    {
                        //结束标志位为true
                        flag = true;

                        //最小规则，直接返回
                        if (MatchType.MIN_MATCH_TYPE == matchType)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
            if (matchFlag < 2 || !flag)
            {
                matchFlag = 0;
            }
            return matchFlag;
        }
        #endregion

    }

}
