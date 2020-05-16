using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// ダンジョン生成
/// </summary>
public class DungeonGenerator : MonoBehaviour
{
    const int MaxWidth = 30;         // ダンジョン最大幅
    const int MaxHeight = 30;        // ダンジョン最大高さ
    const int RoomMinWidth = 3;      // 部屋最小幅
    const int RoomMinHeight = 3;     // 部屋最小高さ
    const int RoomMinMergin = 1;     // 区画と部屋の最小余白    
    const int RoomTotalMargin = RoomMinMergin * 2 + 1;  // 区画と部屋の余白合計(最小余白*2 + 壁)

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
        // 1. 全てを壁にする
        // 2. 最初の区画を生成
        // 3. 区画を分割
        // 4. 区画内に部屋生成
        // 5. 部屋と部屋をつなげる
        // 6. 行き止まりを削除


        // 1. マップをすべて壁に
        mapInfo.SetMapStateAll(MapInfo.MapState.Wall);

        // 2. 最初の区画登録
        CreateDivision();

        // 3. 区画分割
        SplitDivision(divList.Last());

        // 4. 部屋の生成
        CreateRoom();

        // 5.部屋と部屋をつなげる
        CreateRoad();

        // 6. 行き止まりを削除
        DeleteDeadEnd();

        // (デバッグ用)マップ表示
        DebugShowDungeonMap();
    }

    /// <summary>
    /// 区画生成
    /// </summary>
    void CreateDivision()
    {
        var div = new Division();
        div.outer.SetRect(0, 0, MaxHeight, MaxWidth);
        divList.Add(div);
    }

    /// <summary>
    /// 区画生成
    /// </summary>
    void SplitDivision(Division div)
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

            // (部屋最小サイズ+部屋の上下余白)
            int min_range = RoomMinHeight + RoomTotalMargin;
            if (div.outer.GetHeight() >= min_range * 2)
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

            // (部屋最小サイズ+部屋の左右余白以上か
            int min_range = RoomMinWidth + RoomTotalMargin;
            if (div.outer.GetWidth() >= min_range * 2)
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

        divList.Add(div_add);

        // 再帰呼び出しでさらに分割
        SplitDivision(div);
        SplitDivision(div_add);
    }

    /// <summary>
    /// 部屋生成
    /// </summary>
    private void CreateRoom()
    {
        foreach (var div in divList)
        {
            // 部屋の最大範囲を計算
            int maxWidth = div.outer.GetWidth() - RoomTotalMargin;
            int maxHeight = div.outer.GetHeight() - RoomTotalMargin;

            // 部屋の範囲をランダムに決定
            int width = Random.Range(RoomMinWidth, maxWidth + 1);
            int height = Random.Range(RoomMinHeight, maxHeight + 1);

            // 部屋の左上座標をランダムに決定(外周左上座標 + 通路幅)
            int top = Random.Range(div.outer.Top + RoomMinMergin, div.outer.Bottom - (RoomTotalMargin - RoomMinMergin + height) + 1);
            int left = Random.Range(div.outer.Left + RoomMinMergin, div.outer.Right - (RoomTotalMargin - RoomMinMergin + width) + 1);

            // 部屋サイズを設定
            div.room.SetRect(top, left, height, width);
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
            var outer = div.outer;
            var room = div.room;
            // 外周の右側と下側を通路作成(マップ端は除く)
            if(outer.Right < MaxWidth - 1)
            {
                mapInfo.SetMapRoadVertical(outer.Top, outer.Bottom, outer.Right);
            }
            if(outer.Bottom < MaxHeight - 1)
            {
                mapInfo.SetMapRoadHorizontal(outer.Left, outer.Right, outer.Bottom);
            }

            // 0. 上
            // 1. 左
            // 2. 下
            // 3. 右
            var roadList = new List<int> { 0, 1, 2, 3 };

            if (outer.Top == 0) roadList.Remove(0);
            if (outer.Left == 0) roadList.Remove(1);
            if (outer.Bottom == MaxHeight - 1) roadList.Remove(2);
            if (outer.Right == MaxWidth - 1) roadList.Remove(3);

            if (roadList.Count == 0)
            {
                // 通路を作成できない
                return;
            }

            var hasRoadList = new List<int> ();
            if(outer.Top == 0 && outer.Bottom == MaxHeight - 1)
            {
                // 上下に通路を作れないので左右に通路を作る
                hasRoadList.Add(1);
                hasRoadList.Add(3);
            }
            if(outer.Left == 0 && outer.Right == MaxWidth - 1)
            {
                // 左右に通路を作れないので上下に道を作る
                hasRoadList.Add(0);
                hasRoadList.Add(2);
            }

            // １つは通路を作る
            var isRoad = roadList[Random.Range(0, roadList.Count)];
            if (!hasRoadList.Contains(isRoad))
            {
                hasRoadList.Add(isRoad);
            }

            int pos;

            // 上側通路
            if(hasRoadList.Contains(0) || Random.Range(0, 2) == 0)
            {
                pos = Random.Range(room.Left, room.Right + 1);
                mapInfo.SetMapRoadVertical(outer.Top, room.Top, pos);
            }

            // 左側通路
            if (hasRoadList.Contains(1) || Random.Range(0, 2) == 0)
            {
                pos = Random.Range(room.Top, room.Bottom + 1);
                mapInfo.SetMapRoadHorizontal(outer.Left, room.Left, pos);
            }

            // 下側通路
            if (hasRoadList.Contains(2) || Random.Range(0, 2) == 0)
            {
                pos = Random.Range(room.Left, room.Right + 1);
                mapInfo.SetMapRoadVertical(room.Bottom, outer.Bottom, pos);
            }

            // 右側通路
            if (hasRoadList.Contains(3) || Random.Range(0, 2) == 0)
            {
                pos = Random.Range(room.Top, room.Bottom + 1);
                mapInfo.SetMapRoadHorizontal(room.Right, outer.Right, pos);
            }
            
        }
    }

    /// <summary>
    /// 行き止まり削除
    /// </summary>
    private void DeleteDeadEnd()
    {
        var Dir = new int[,]{ { 0, -1 }, { -1, 0 }, { 1, 0 }, { 0, 1 } };
        int count = 0;
        bool deadEndFlg = false;
        foreach (var div in divList)
        {
            while (true)
            {
                for (int i = div.outer.Top; i <= div.outer.Bottom; i++)
                {
                    for (int j = div.outer.Left; j <= div.outer.Right; j++)
                    {
                        count = 0;
                        deadEndFlg = false;

                        if (mapInfo.GetState(j, i) == MapInfo.MapState.Wall)
                        {
                            // すでに壁
                            continue;
                        }

                        // 周りが壁かチェック
                        for (int k = 0; k < Dir.GetLength(0); k++)
                        {
                            int x = j + Dir[k, 0];
                            int y = i + Dir[k, 1];

                            if (x < 0 || MaxWidth <= x ||
                                y < 0 || MaxHeight <= y)
                            {
                                // 範囲外
                                count++;
                                continue;
                            }

                            if (mapInfo.GetState(x, y) == MapInfo.MapState.Wall)
                            {
                                count++;
                            }
                        }

                        if (count >= Dir.GetLength(0) - 1)
                        {
                            // 一方向以外は壁（行き止まり）
                            // 壁に変更
                            mapInfo.SetState(j, i, MapInfo.MapState.Wall);
                            deadEndFlg = true;
                            break;
                        }
                    }
                    if (deadEndFlg)
                    {
                        break;
                    }
                }

                if (deadEndFlg)
                {
                    // 行き止まりがあれば最初からチェック
                    continue;
                }

                // 次の区画へ
                break;

            }
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
