using UnityEngine;
using System;
using System.Collections.Generic;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> _executionQueue = new Queue<Action>();
    private static UnityMainThreadDispatcher _instance = null;

    // ★ Awake() はUnityのメインスレッドで、Start()よりも先に呼ばれることが保証されています。
    void Awake()
    {
        // シーンに既にインスタンスが存在するかチェック
        if (_instance == null)
        {
            _instance = this;
            // シーンをまたいでも破棄されないようにする（任意）
            DontDestroyOnLoad(this.gameObject);
        }
        else if (_instance != this)
        {
            // 既に存在する場合は、重複しないように自身を破棄
            Destroy(gameObject);
        }
    }

    // ★ Instance() メソッドをシンプルで安全な形に変更
    // このメソッドは、Awake()で準備されたインスタンスを返すだけなので、
    // どのスレッドから呼ばれても安全です。
    public static UnityMainThreadDispatcher Instance()
    {
        return _instance;
    }

    public void Enqueue(Action action)
    {
        // lockステートメントで、複数のスレッドから同時にアクセスされても
        // _executionQueueが壊れないように保護します。
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }

    void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                _executionQueue.Dequeue().Invoke();
            }
        }
    }
}