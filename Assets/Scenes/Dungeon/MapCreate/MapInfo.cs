using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInfo
{
    public enum MapState
    {
        Wall,       // 壁
        Road,       // 道
    }

    class MapCell
    {
        public MapState State { get; set; }
    }

    int width;      // マップ幅
    int height;     // マップ高さ


    // マップ情報
    MapCell[,] mapCell;

    public MapInfo(int width, int height)
    {
        this.width = width;
        this.height = height;

        // マップ生成
        CreateMap(width, height);
    }

    /// <summary>
    ///  マップ生成
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    private void CreateMap(int width, int height)
    {
        // メモリ確保
        mapCell = new MapCell[height, width];

        // 初期化
        for(int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                mapCell[y, x] = new MapCell();
            }
        }
    }

    /// <summary>
    /// セルの状態取得
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public MapState GetState(int x, int y)
    {
        return mapCell[y, x].State;
    }

    /// <summary>
    /// 全て同じ状態にする
    /// </summary>
    /// <param name="state"></param>
    public void SetMapStateAll(MapState state)
    {
        for(int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                mapCell[y, x].State = state;
            }
        }
    }

    /// <summary>
    /// 範囲内の状態を変更
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="state"></param>
    public void SetMapStateRange(Rect rect, MapState state)
    {
        for(int y = rect.Top; y < rect.Bottom; y++)
        {
            for(int x = rect.Left; x < rect.Right; x++)
            {
                mapCell[y, x].State = state;
            }
        }
    }

    /// <summary>
    /// 縦の通路を作成
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="width"></param>
    public void SetMapRoadVertical(int from, int to, int x)
    {
        if(to < from)
        {
            // 値入れ替え
            (from, to) = (to, from);
        }

        // 通路作成(道幅で+1)
        SetMapStateRange(new Rect(from, x, to + 1, x + 1), MapState.Road);
    }

    /// <summary>
    /// 横の通路を作成
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="width"></param>
    public void SetMapRoadHorizontal(int from, int to, int y)
    {
        if (to < from)
        {
            // 値入れ替え
            (from, to) = (to, from);
        }

        // 通路作成
        SetMapStateRange(new Rect(y, from, y + 1, to + 1), MapState.Road);
    }
}
