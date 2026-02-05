using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameCore : MonoBehaviour
{
    private static GameCore _instance;
    public static GameCore Instance
    {
        get => _instance ??= GameObject.Find("GameCore").GetComponent<GameCore>();
    }

    public ESystemState FirstState;

    private SystemStateMachine _stateMachine;
    public SystemStateMachine StateMachine => _stateMachine;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // 设置加载线程调度优先级
        Application.backgroundLoadingPriority = ThreadPriority.High;

        // 内存相关设置
        QualitySettings.asyncUploadBufferSize = 4;

        TaskPool.SetMaxPoolSize(64);

        // 游戏的表现帧率不设置上限
        Application.targetFrameRate = 1000;

        // 设置屏幕不息屏
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        UniTask.SwitchToMainThread();

        _stateMachine = new SystemStateMachine(SystemTreeConfig.GameTree);
    }

    private void Start()
    {
        _stateMachine.Start();
        _stateMachine.ChangeState(FirstState);
    }

    public static void ChangeState(ESystemState stateType)
    {
        _instance.StateMachine.ChangeState(stateType);
    }

    private void Update()
    {
        _stateMachine.Update();
    }

    private void OnDestroy()
    {
        _stateMachine.Destroy();
    }
}
