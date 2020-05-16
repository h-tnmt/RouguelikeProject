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

    /// <summary>
    /// コンストラクタ
    /// </summary>
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
        Top = top;
        Left = left;
        Bottom = bottom;
        Right = right;
    }

    /// <summary>
    /// 矩形情報の登録
    /// </summary>
    /// <param name="top"></param>
    /// <param name="left"></param>
    /// <param name="height"></param>
    /// <param name="width"></param>
    public void SetRect(int top, int left, int height, int width)
    {
        this.Top = top;
        this.Left = left;
        this.Bottom = top + height - 1;
        this.Right = left + width - 1;
    }

    /// <summary>
    /// 区画幅
    /// </summary>
    /// <returns></returns>
    public int GetWidth()
    {
        // 0スタートなので+1
        return Right - Left + 1;
    }

    /// <summary>
    /// 区画高さ
    /// </summary>
    /// <returns></returns>
    public int GetHeight()
    {
        // 0スタートなので+1
        return Bottom - Top + 1;
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
    // 部屋
    public Rect room;

    public Division()
    {
        outer = new Rect();
        room = new Rect();
    }
}
