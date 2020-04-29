using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ダンジョン管理クラス
/// </summary>
public class DungeonControl : MonoBehaviour
{
    [SerializeField]
    DungeonGenerator dgGenerator;

    private void Start()
    {
        dgGenerator.CreateDungeon();   
    }

    void Update()
    {
        
    }
}
