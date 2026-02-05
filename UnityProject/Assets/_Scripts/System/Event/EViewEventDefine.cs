using System;
using System.Collections.Generic;

public enum EUIStore
{
    OpenStore,

    OnRecipeStoreSelectedChange,

    OnFoodStoreSelectedChange,

    Count,
}

public enum EUIBag
{
    OnAssetChange = EUIStore.Count,

    OnMoneyChange,

    Count,
}

public enum EUISaved
{
    OnSelectedItemChange = EUIBag.Count,

    Count,
}

public enum EUIBuild
{
    OpenBuildPanel = EUISaved.Count,

    OnKitchenSelectedChange,

    Count,
}

public enum EUIDrag
{
    BindElement = EUIBuild.Count,
    UnBindElement,

    RegisterContainer,
    UnRegisterContainer,

    Count,
}

public enum EUIOrder
{
    OnOrderCreated = EUIDrag.Count,

    OnOrderStateChange,

    OnOrderFinish,

    Count,
}