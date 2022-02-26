using System;
using System.Collections.Generic;

/// <summary>
/// string 型の拡張メソッドを管理するクラス
/// </summary>

//コガネブログ  2014-06-26 
//http://baba-s.hatenablog.com/entry/2014/06/26/192817

public static partial class StringExtensions
{

    /// <summary>
    /// 指定した文字列がいくつあるか
    /// </summary>
    public static int CountOf(this string self, params string[] strArray)
    {
        int count = 0;

        foreach (string str in strArray)
        {
            int index = self.IndexOf(str, 0);
            while (index != -1)
            {
                count++;
                index = self.IndexOf(str, index + str.Length);
            }
        }

        return count;
    }

}