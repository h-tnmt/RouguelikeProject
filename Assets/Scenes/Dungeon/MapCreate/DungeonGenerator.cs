using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// ダンジョン生成
/// </summary>
public class DungeonGenerator : MonoBehaviour
{
    const int MAX_WIDTH = 30;           // ダンジョン最大幅
    const int MAX_HEIGHT = 30;          // ダンジョン最大高さ
    const int ROOM_MIN_WIDTH = 3;       // 部屋最小幅
    const int ROOM_MIN_HEIGHT = 3;      // 部屋最小高さ
    const int ROOM_MIN_MERGIN = 1;      // 区画と部屋の最小余白（通路用）



    // 区画リスト
    List<Division> div_list;

    void Awake()
    {
        div_list = new List<Division>();
    }

    /// <summary>
    /// ダンジョン生成
    /// </summary>
    public void CreateDungeon()
    {
        // 作成手順
        // 1. 区画生成
        // 2. 区画内に部屋生成
        // 3. 部屋から区画の端に道をつなげる
        // 4. 行き止まりを消す

        // 1. 区画生成

        // 1.1 最初の区画登録
        var div = new Division();
        div.outer.SetRect(0, 0, MAX_HEIGHT - 1, MAX_WIDTH - 1);
        div_list.Add(div);
        //1.2 区画分割
        CreateDivision(div);


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
            int min_range = ROOM_MIN_HEIGHT + ROOM_MIN_MERGIN * 2;
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
            int min_range = ROOM_MIN_WIDTH + ROOM_MIN_MERGIN * 2;
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
        div_list.Add(div_add);

        // 再帰呼び出しでさらに分割
        CreateDivision(div);
        CreateDivision(div_add);
    }

    /// <summary>
    /// マップコンソール表示
    /// </summary>
    void DebugShowDungeonMap()
    {
        var map_data = new int[MAX_HEIGHT, MAX_WIDTH];

        int count = 1;
        foreach (var div in div_list)
        {
            // 区画範囲を登録
            for(int i = div.outer.Top; i <= div.outer.Bottom; i++)
            {
                for (int j = div.outer.Left; j <= div.outer.Right; j++)
                {
                    if(map_data[i, j] == 0)
                    {
                        map_data[i, j] = count;
                    }
                    else
                    {
                        map_data[i, j] = -1;
                    }
                }
            }
            count++;
        }

        string msg = "ダンジョンマップ\n";
        for (int i = 0; i < MAX_HEIGHT; i++)
        {
            msg += $"({i:00}) ";
            for (int j = 0; j < MAX_WIDTH; j++)
            {
                if(j % 5 == 0 && j != 0)
                {
                    msg += $"[{j}] ";
                }
                msg += map_data[i, j].ToString("00") + " ";
            }
            msg += "\n";
        }
        Debug.Log(msg);
    }
}
