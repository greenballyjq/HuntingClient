using cfg;
using Cysharp.Threading.Tasks;
using Luban;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class AsyncAwaitTest1 : MonoBehaviour
{
    //private void Start()
    //{
    //    Init();
    //}

    //public virtual void Init()
    //{
    //    Debug.Log("1Init开始1");
    //    Debug.Log("2Init开始2");
    //    Debug.Log("3Init开始3");
    //    InitializeAsync(); 
    //    Debug.Log("7Init结束1");
    //    Debug.Log("8Init结束2");
    //    Debug.Log("9Init结束3");
    //}

    //protected virtual async UniTask InitializeAsync()
    //{
    //    Debug.Log("4InitializeAsync开始1");
    //    LoadTablesAsync().Forget();
    //    Debug.Log("6InitializeAsync完成1");
    //}

    //public virtual async UniTask LoadTablesAsync()
    //{
    //    Debug.Log("5LoadTablesAsync开始1");
    //    List<int> byteBuffs = new List<int>();
    //    foreach (var tableName in byteBuffs)
    //    {
    //        TextAsset textAsset = Resources.Load<TextAsset>($"...");
    //    }

    //    await UniTask.Yield(); 
    //    Debug.Log("10LoadTablesAsync完成1");
    //}
}
