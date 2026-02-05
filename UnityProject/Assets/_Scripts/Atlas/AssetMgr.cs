using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class AtlasMgr : Singleton<AtlasMgr>
{
    private class SpriteState
    {
        public Sprite Sprite;
        public EAssetLoadState State;
    }

    private Dictionary<string, SpriteAtlas> _atlasMap;
    private Dictionary<ulong, SpriteState> _spriteMap;

    public AtlasMgr()
    {
        _atlasMap = new Dictionary<string, SpriteAtlas>();
        _spriteMap = new Dictionary<ulong, SpriteState>();
    }

    public async UniTask<Sprite> LoadSprite(string atlasName, string spriteName)
    {
        var hash = GetHash(atlasName, spriteName);

        if (!_spriteMap.TryGetValue(hash, out var state))
        {
            state = new SpriteState();
            _spriteMap.Add(hash, state);
        }

        if (state.State == EAssetLoadState.None)
        {
            await LoadAndCacheSprite(atlasName, spriteName);
        }
        else if (state.State == EAssetLoadState.Loading)
        {
            await UniTask.WaitUntil(() => state.State == EAssetLoadState.Finish);
        }

        return state.Sprite;
    }

    private async UniTask<SpriteAtlas> LoadAtlas(string atlasName)
    {
        if (!_atlasMap.TryGetValue(atlasName, out var atlas))
        {
            try
            {
                atlas = await ResSys.Instance.LoadAsset<SpriteAtlas>(atlasName);
            }
            catch (Exception exception)
            {
                Debug.LogError(exception);
            }

            if (atlas != null)
            {
                _atlasMap[atlasName] = atlas;
            }
        }

        return atlas;
    }

    private async UniTask<Sprite> LoadAndCacheSprite(string atlasName, string spriteName)
    {
        var hash = GetHash(atlasName, spriteName);

        var state = _spriteMap[hash];

        state.State = EAssetLoadState.Loading;

        var atlas = await LoadAtlas(atlasName);

        if (atlas == null)
        {
            state.State = EAssetLoadState.Failure;
            return null;
        }

        var sprite = atlas.GetSprite(spriteName);

        if (sprite == null)
        {
            state.State = EAssetLoadState.Failure;
            return null;
        }

        state.Sprite = sprite;
        state.State = EAssetLoadState.Finish;

        return sprite;
    }

    private ulong GetHash(string atlasName, string spriteName)
    {
        ulong atlasHash = (uint)atlasName.GetHashCode();
        ulong keyHash = (uint)spriteName.GetHashCode();
        ulong hash = atlasHash << 32 | keyHash;

        return hash;
    }
}
