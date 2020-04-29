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

        // 1. 区画分割
        CreateDivision();


        // (デバッグ用)マップ表示
        DebugShowDungeonMap();
    }

    /// <summary>
    /// 区画生成
    /// </summary>
    void CreateDivision()
    {
        // 区画が一つもなければ最初の区画生成
        if(div_list.Count == 0)
        {
            // 上側と左側はこの時点で壁として登録
            div_list.Add(new Division());
            div_list.Last().outer.SetRect(1, 1, MAX_HEIGHT - 1, MAX_WIDTH - 1);
        }

        // リストの最後を取得    
        var div_last = div_list.Last();
        div_list.Remove(div_last);
        // 追加区画
        var div_add = new Division();
        div_add.outer.Copy(div_last.outer);

        // 分割方向をランダムに決める
        Random.InitState((int)System.DateTime.Now.Ticks);
        //bool isVertical = (Random.value == 0);
        bool isVertical = true;
        if (isVertical)
        {
            // 縦に分割

            // (部屋最小サイズ+部屋の下側余白) * 2 以上か
            int min_range = ROOM_MIN_HEIGHT + ROOM_MIN_MERGIN;
            if (div_last.outer.GetHeight() > min_range * 2)
            {
                // 分割位置を決める

                int max_range = div_last.outer.GetHeight() - min_range * 2;
                if(max_range == 0)
                {
                    // 中間で分割
                    div_last.outer.Bottom -= div_last.outer.GetHeight() / 2;
                }
                else
                {
                    // 分割位置をランダムに決定
                    div_last.outer.Bottom -= (min_range + Random.Range(0, max_range));
                }

                // 追加区画の範囲登録
                div_add.outer.Top = div_last.outer.Bottom + 1;
            }
            else
            {
                // 分割できないので終了
                return;
            }
        }
        else
        {

        }

        // 分割する区画をランダムに登録
        if (Random.value == 0)
        {
            div_list.Add(div_last);
            div_list.Add(div_add);

            //CreateDivision();
        }
        else
        {
            div_list.Add(div_add);
            div_list.Add(div_last);

            //CreateDivision();
        }
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
            for(int i = div.outer.Top; i < div.outer.Bottom; i++)
            {
                for (int j = div.outer.Left; j < div.outer.Right; j++)
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
