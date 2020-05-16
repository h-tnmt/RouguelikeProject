using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ダンジョン管理クラス
/// </summary>
public class DungeonControl : MonoBehaviour
{
    [SerializeField]
    DungeonGenerator dgGenerator = default;

    private void Start()
    {
        // ダンジョン生成
        dgGenerator.CreateDungeon();   
    }

    void Update()
    {
        
    }
}
