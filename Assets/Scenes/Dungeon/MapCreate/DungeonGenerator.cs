using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// ダンジョン生成
/// </summary>
public class DungeonGenerator : MonoBehaviour
{
    readonly int MaxWidth = 30;                     // ダンジョン最大幅
    readonly int MaxHeight = 30;                    // ダンジョン最大高さ
    readonly int RoomMinWidth = 3;                  // 部屋最小幅
    readonly int RoomMinHeight = 3;                 // 部屋最小高さ
    readonly int RoomMinMarginBottomRight = 2;      // 区画と部屋の最小余白(右下)
    readonly int RoomMinMarginTopLeft = 1;          // 区画と部屋の最小余白(左上)

    // 区画リスト
    List<Division> divList;
    // マップの情報
    MapInfo mapInfo;

    /// <summary>
    /// ダンジョン生成
    /// </summary>
    public void CreateDungeon()
    {
        divList = new List<Division>();
        mapInfo = new MapInfo(MaxWidth, MaxHeight);

        // 作成手順
        // 1. 区画生成
        // 2. 区画内に部屋生成
        // 3. 部屋から区画の端に道をつなげる
        // 4. 行き止まりを消す

        // 1. 区画生成

        // 1-1. 最初の区画登録
        var div = new Division();
        div.outer.SetRect(0, 0, MaxHeight - 1, MaxWidth - 1);
        divList.Add(div);
        // 1-2. 区画分割
        CreateDivision(div);

        // 2. 区画内に部屋生成

        // 2-1. マップをすべて壁に
        mapInfo.SetMapStateAll(MapInfo.MapState.Wall);

        // 2-2. 部屋の生成
        CreateRoom();

        // 3. 部屋から区画の端に道をつなげる
        CreateRoad();

        // (デバッグ用)マップ表示
        DebugShowDungeonMap();
    }

    /// <summary>
    /// 区画生成
    /// </summary>
    void CreateDivision(Division div)
    {
        // 追加区画
        var div_add = new Division();
        div_add.outer.Copy(div.outer);

        // 分割方向をランダムに決める
        //Random.InitState((int)System.DateTime.Now.Ticks);
        bool isVertical = (Random.Range(0, 2) == 0);
        if (isVertical)
        {
            // 縦に分割

            // (部屋最小サイズ+部屋の上下余白) * 2 以上か
            int min_range = RoomMinHeight + RoomMinMarginTopLeft + RoomMinMarginBottomRight;
            if (div.outer.GetHeight() > min_range * 2)
            {
                // 分割位置を決める
                int max_range = div.outer.GetHeight() - min_range * 2;
                if(max_range == 0)
                {
                    // 中間で分割
                    div.outer.Bottom -= div.outer.GetHeight() / 2;
                }
                else
                {
                    // 分割位置をランダムに決定
                    div.outer.Bottom -= (min_range + Random.Range(0, max_range));
                }

                // 追加区画の範囲登録
                div_add.outer.Top = div.outer.Bottom + 1;
            }
            else
            {
                // 分割できないので終了
                return;
            }
        }
        else
        {
            // 横に分割

            // (部屋最小サイズ+部屋の左右余白) * 2 以上か
            int min_range = RoomMinWidth + RoomMinMarginTopLeft + RoomMinMarginBottomRight;
            if (div.outer.GetWidth() > min_range * 2)
            {
                // 分割位置を決める
                int max_range = div.outer.GetWidth() - min_range * 2;
                if (max_range == 0)
                {
                    // 中間で分割
                    div.outer.Right -= div.outer.GetWidth() / 2;
                }
                else
                {
                    // 分割位置をランダムに決定
                    div.outer.Right -= (min_range + Random.Range(0, max_range));
                }

                // 追加区画の範囲登録
                div_add.outer.Left = div.outer.Right + 1;
            }
            else
            {
                // 分割できないので終了
                return;
            }
        }

        // 区画を追加
        divList.Add(div_add);

        // 再帰呼び出しでさらに分割
        CreateDivision(div);
        CreateDivision(div_add);
    }

    /// <summary>
    /// 部屋生成
    /// </summary>
    private void CreateRoom()
    {
        foreach (var div in divList)
        {
            // 部屋の最大範囲を計算
            int maxWidth = div.outer.GetWidth() - (RoomMinMarginTopLeft + RoomMinMarginBottomRight);
            int maxHeight = div.outer.GetHeight() - (RoomMinMarginTopLeft + RoomMinMarginBottomRight);

            // 部屋の範囲をランダムに決定
            int width = Random.Range(RoomMinWidth, maxWidth + 1);
            int height = Random.Range(RoomMinHeight, maxHeight + 1);

            // 部屋の左上座標をランダムに決定
            int top = Random.Range(div.outer.Top + RoomMinMarginTopLeft, div.outer.Bottom - (RoomMinMarginBottomRight + height) + 1);
            int left = Random.Range(div.outer.Left + RoomMinMarginTopLeft, div.outer.Right - (RoomMinMarginBottomRight + width) + 1);

            // 部屋サイズを設定
            div.room.SetRect(top, left, top + height, left + width);
            // 部屋を通路として登録
            mapInfo.SetMapStateRange(div.room, MapInfo.MapState.Road);
        }
    }

    /// <summary>
    /// 通路を作成
    /// </summary>
    private void CreateRoad()
    {
        foreach(var div in divList)
        {
            // 区画の右側をすべて壁にする
            mapInfo.SetMapRoadVertical(div.outer.Top, div.outer.Bottom, div.outer.Right);
            // 区画の下側をすべて壁にする
            mapInfo.SetMapRoadHorizontal(div.outer.Left, div.outer.Right, div.outer.Bottom);

            // 上側通路
            int top = Random.Range(div.room.Left, div.room.Right);
            mapInfo.SetMapRoadVertical(div.room.Top, div.outer.Top, top);

            // 下側通路
            int bottom = Random.Range(div.room.Left, div.room.Right);
            mapInfo.SetMapRoadVertical(div.room.Bottom, div.outer.Bottom, bottom);

            // 左側通路
            int left = Random.Range(div.room.Top, div.room.Bottom);
            mapInfo.SetMapRoadHorizontal(div.room.Left, div.outer.Left, left);

            // 右下通路
            int right = Random.Range(div.room.Top, div.room.Bottom);
            mapInfo.SetMapRoadHorizontal(div.room.Right, div.outer.Right, right);
        }
    }

    /// <summary>
    /// マップコンソール表示
    /// </summary>
    void DebugShowDungeonMap()
    {
        string msg = "ダンジョンマップ\n";

        for(int i = 0; i < MaxHeight; i++)
        {
            for(int j = 0; j < MaxWidth; j++)
            {
                switch(mapInfo.GetState(j, i))
                {
                    case MapInfo.MapState.Wall:
                        msg += "□";
                        break;
                    case MapInfo.MapState.Road:
                        msg += "■";
                        break;
                }
            }
            msg += "\n";
        }

        Debug.Log(msg);
    }
}
