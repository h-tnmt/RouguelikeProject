using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        // 乱数シードを現在時刻で初期化
        Random.InitState((int)System.DateTime.Now.Ticks);
    }
}
