// **********************************************************************
// 此代码自动生成，请勿修改。
// **********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using Tup.Tars;
public static partial class NormalTableInit
{
    #region 通过index方案，后面流式加载数据
    public static ITableData ResolveDictDataIndex(ETableType tableType, TableIndexList tableIndexList)
    {
        switch (tableType)
        {
            default:
                return null;
        }
    }
    #endregion


    #region 从bytes直接解析出表数据
    [System.Obsolete("不推荐这种解析数据的方式，建议用索引内存和GC更优")]
    public static ITableData ResolveDictDataBytes(ETableType tableType, byte[] bytes)
    {
        const int kIndexHead  = 4; //这个头部数据表示这个bytes数据的index数据区有多长
        const int kIndexBlock = 12;//这个表示index数据一个块有多少字节
        switch (tableType)
        {
            default:
                return null;
        }
    }
    #endregion

}
