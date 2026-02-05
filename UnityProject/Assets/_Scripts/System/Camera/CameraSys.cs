using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSys : SystemBase<CameraSys>
{
    private Camera _mainCamera;
    public Camera MainCamera => _mainCamera;

    protected override void OnEnter()
    {
        base.OnEnter();

        _mainCamera = Camera.main;

        Object.DontDestroyOnLoad(_mainCamera.gameObject);
    }
}
