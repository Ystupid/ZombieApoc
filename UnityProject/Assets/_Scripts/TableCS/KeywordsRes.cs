namespace KGameRes
{
    public enum EAssetType  //物品类型
    {
        ///<summary> 营业用具 </summary>
        [KeywordAdditionalData(CName = "营业用具", Category = "")]
        E_ASSET_CURRENCY = 1 ,

        ///<summary> 食物 </summary>
        [KeywordAdditionalData(CName = "食物", Category = "")]
        E_ASSET_FOOD = 2 ,

        ///<summary> 菜谱 </summary>
        [KeywordAdditionalData(CName = "菜谱", Category = "")]
        E_ASSET_Menu = 3 ,

        ///<summary> 生产工具 </summary>
        [KeywordAdditionalData(CName = "生产工具", Category = "")]
        E_ASSET_Production = 4 ,

        ///<summary> 农作物 </summary>
        [KeywordAdditionalData(CName = "农作物", Category = "")]
        E_ASSET_Crop = 5 ,

    }

    public enum EKitchenWareType  //厨具类型
    {
        ///<summary> 煮锅 </summary>
        [KeywordAdditionalData(CName = "煮锅", Category = "")]
        Pot = 1 ,

        ///<summary> 平底锅 </summary>
        [KeywordAdditionalData(CName = "平底锅", Category = "")]
        Pan = 2 ,

        ///<summary> 搅拌碗 </summary>
        [KeywordAdditionalData(CName = "搅拌碗", Category = "")]
        MixingBowl = 3 ,

        ///<summary> 砧板 </summary>
        [KeywordAdditionalData(CName = "砧板", Category = "")]
        Board = 4 ,

        ///<summary> 蒸笼 </summary>
        [KeywordAdditionalData(CName = "蒸笼", Category = "")]
        Steamer = 5 ,

    }

    public enum ESystemParam  //系统参数
    {
        ///<summary> 商城食谱刷新价格 </summary>
        [KeywordAdditionalData(CName = "商城食谱刷新价格", Category = "")]
        StoreRecipeRefreshPrice = 1 ,

    }

    public enum EStoreUpCondition  //商城上架条件
    {
        ///<summary> 无 </summary>
        [KeywordAdditionalData(CName = "无", Category = "")]
        None = 1 ,

        ///<summary> 货架刷新 </summary>
        [KeywordAdditionalData(CName = "货架刷新", Category = "")]
        Refresh = 2 ,

    }
}
