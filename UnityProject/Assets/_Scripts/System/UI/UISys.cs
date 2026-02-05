using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UISys : SystemBase<UISys>
{
    private GameObject _rootNode;

    protected override UniTask OnPreload()
    {
        return LoadUIRoot();
    }

    private async UniTask LoadUIRoot()
    {
        _rootNode = await ResSys.Instance.Instantiate("Default_UIRoot");

        UIManager.Instance.Init(_rootNode);
    }
}
