using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Tup.Tars;
using UnityEngine;

public class TableSys : SystemBase<TableSys>
{
    public const string MetaInfoKey = "_Table_TableMetaInfo";

    private Dictionary<ETableType, ITableData> _tableMap;
    private Dictionary<string, TableMetaNode> _metaMap;

    protected override void OnInitial()
    {
        base.OnInitial();

        _tableMap = new Dictionary<ETableType, ITableData>();
    }

    public TableData<T> GetTableData<T>(ETableType tableType) where T : TarsStruct, new()
    {
        if (_tableMap.TryGetValue(tableType, out var tableData))
        {
            return tableData as TableData<T>;
        }

        return null;
    }

    public T GetRecord<T>(ETableType tableType, int id) where T : TarsStruct, new()
    {
        return GetTableData<T>(tableType).GetRecord(id);
    }

    protected override async UniTask OnPreload()
    {
        await LoadMetaInfo();

        await LoadTableData();
    }

    private async UniTask LoadMetaInfo()
    {
        var metaAsset = await ResSys.Instance.LoadAsset<TextAsset>(MetaInfoKey);

        _metaMap = JsonConvert.DeserializeObject<Dictionary<string, TableMetaNode>>(metaAsset.text);
    }

    private async UniTask LoadTableData()
    {
        var taskList = new List<UniTask>();

        foreach (var keyValue in _metaMap)
        {
            var key = $"_Table_{keyValue.Key}";

            var task = ResSys.Instance.LoadAsset<TextAsset>(key);

            var tableType = keyValue.Value.TableType;

            taskList.Add(task.AddCallback(tableAsset =>
            {
                var tableData = NormalTableInit.ResolveDictDataBytes(tableType, tableAsset.bytes);

                _tableMap[tableType] = tableData;
            }));
        }

        await UniTask.WhenAll(taskList);
    }
}
