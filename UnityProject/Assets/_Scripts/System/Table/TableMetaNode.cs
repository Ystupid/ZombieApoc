using System;
using System.Collections.Generic;

public class TableMetaNode
{
    /// <summary>
    /// 和TableType的字符串形式一样,由表名和分表名一起压制而成,也就是对应本表的key值
    /// </summary>
    public string TableTypeStr;

    /// <summary>
    /// 中文名
    /// </summary>
    public string SheetCName;

    /// <summary>
    /// 从Res目录下到该表的路径.
    /// </summary>
    public string XlsxName;

    /// <summary>
    /// 是否是静默更新表,由策划指定
    /// </summary>
    public bool IsSilentTable;

    /// <summary>
    /// 是否是系统表(Res/系统)目录下的表,如果是,该表就支持失去特化功能
    /// </summary>
    public bool IsSupportSpecRegion;

    /// <summary>
    /// 当前有哪些时区数据.key是时区,value是数据产物的名字(可以直接用作资源加载的键值).cn对应国区的资源.
    /// </summary>
    public Dictionary<string, string> TimeZoneBytesName;

    /// <summary>
    /// 产物的实际生成目标
    /// </summary>
    public string Target;

    public ETableType TableType
    {
        get
        {
            return (ETableType)Enum.Parse(typeof(ETableType), TableTypeStr);
        }
        set
        {
            TableTypeStr = value.ToString();
        }
    }
}
