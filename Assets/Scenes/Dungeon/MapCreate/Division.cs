using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 矩形情報
/// </summary>
public class Rect
{
    public int Top { get; set; }        // 上
    public int Bottom { get; set; }     // 下
    public int Left { get; set; }       // 左
    public int Right { get; set; }      // 右

    public Rect() { }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="top"></param>
    /// <param name="left"></param>
    /// <param name="bottom"></param>
    /// <param name="right"></param>
    public Rect(int top, int left, int bottom, int right) 
    {
        SetRect(top, left, bottom, right);
    }

    /// <summary>
    /// 矩形情報の登録
    /// </summary>
    /// <param name="top"></param>
    /// <param name="left"></param>
    /// <param name="bottom"></param>
    /// <param name="right"></param>
    public void SetRect(int top, int left, int bottom, int right)
    {
        this.Top = top;
        this.Left = left;
        this.Bottom = bottom;
        this.Right = right;
    }

    /// <summary>
    /// 区画幅
    /// </summary>
    /// <returns></returns>
    public int GetWidth()
    {
        return Right - Left;
    }

    /// <summary>
    /// 区画高さ
    /// </summary>
    /// <returns></returns>
    public int GetHeight()
    {
        return Bottom - Top;
    }

    /// <summary>
    /// コピー
    /// </summary>
    /// <param name="rect"></param>
    public void Copy(Rect rect)
    {
        Top = rect.Top;
        Left = rect.Left;
        Bottom = rect.Bottom;
        Right = rect.Right;
    }
}

/// <summary>
/// マップの区画を管理
/// </summary>
public class Division
{
    // 外周
    public Rect outer;
    
    public Division()
    {
        outer = new Rect();
    }
}
