// **********************************************************************
// 此代码自动生成，请勿修改。
// **********************************************************************
using System.Collections.Generic;

public enum ETableType
{
    AssetConfig,  // 物品表
    AssetWay,  // 物品合成表
    CookingConfig,  // 烹饪表
    FarmCropConfig,  // 作物表
    NewbiePackage,  // 新手礼包表
    OrderConfig,  // 订单表
    StoreConfig,  // 商城表
    SystemParam,  // 系统参数表
    Count,
}
public static class ETableTypeExtraInfo
{
/// <summary>标识了所有可静默更新的表,这里的表都是在[静默更新xlsx]名单里的定义. </summary>
    public static readonly ISet<ETableType> ETableTypeSilentUpdateTable = new HashSet<ETableType>{
    };
}

