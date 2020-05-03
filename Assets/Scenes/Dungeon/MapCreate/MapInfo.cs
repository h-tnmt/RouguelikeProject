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

    // マップ生成
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

    // セルの状態取得
    public MapState GetState(int x, int y)
    {
        return mapCell[y, x].State;
    }

    // 全て同じ状態にする
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

    // 範囲内の状態を変更
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
}
