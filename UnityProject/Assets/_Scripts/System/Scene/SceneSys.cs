using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneSys : SystemBase<SceneSys>
{
    private SceneInstance _currentScene;

    public UniTask ChangeScene(string key)
    {
        return LoadScene(key);
    }

    private async UniTask LoadScene(string sceneName, LoadSceneMode loadMode = LoadSceneMode.Single)
    {
        if(_currentScene.Scene.isLoaded)
        {
            await Addressables.UnloadSceneAsync(_currentScene);
        }

        var exist = !string.IsNullOrEmpty(sceneName) && await ResSys.Instance.IsExist(sceneName);

        if(exist)
        {
            _currentScene = await Addressables.LoadSceneAsync(sceneName, loadMode).ToUniTask(Progress.Create<float>(progress =>
            {
            }));
        }
    }
}
