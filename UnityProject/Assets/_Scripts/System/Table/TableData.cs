using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Tup.Tars;

public class TableIndex
{
    public int ID;
    public int Index;
    public int Length;
}

public class TableIndexList
{
    public Dictionary<int, TableIndex> _tableIndices;
    public int tableType;
    public int tableId;
    public int tableIndex;
    public int tableOffset;
    public bool supportSilentUpdate;
    public bool supportSpecRegion;
    public FileStream tableRef;
    public byte[] bytes;
}

public static class SharedBuffer
{
    private static byte[] kBuffer = new byte[128];
    public static object locker = new object();

    public static byte[] GetBuffer(int len)
    {
        if (len > kBuffer.Length)
        {
            kBuffer = new byte[len];
        }
        else
        {
            Array.Clear(kBuffer, 0, kBuffer.Length);
        }

        return kBuffer;
    }
}

public class TableData<T> : IEnumerable<T>, ITableData where T : TarsStruct, new()
{
    private Dictionary<int, T> _tableDic;

    private List<int> _iDs = new List<int>();

    private TableIndexList _tableIndexList;

    public TableData()
    {
    }

    public TableData(TableIndexList tableIndexList)
    {
        this._tableIndexList = tableIndexList;
        foreach (var kvp in tableIndexList._tableIndices)
        {
            _iDs.Add(kvp.Key);
        }
        _iDs.Sort();
        _tableDic = new Dictionary<int, T>();
    }


    public List<int> IDs
    {
        get
        {
            return _iDs;
        }
    }

    public void InitData(Dictionary<int, T> sourceTableDic)
    {
        _tableDic = sourceTableDic;
        var keys = _tableDic.Keys;

        foreach (int id in keys)
        {
            _iDs.Add(id);
        }

        _iDs.Sort();
    }

    public T this[int index]
    {
        get
        {
            if (index >= _iDs.Count)
            {
                UnityEngine.Debug.LogErrorFormat("error index:{0}", index);
                return null;
            }
            int id = _iDs[index];
            return GetRecord(id);
        }
    }

    public int Length
    {
        get
        {
            if (_iDs == null)
            {
                return 0;
            }
            return _iDs.Count;
        }
    }

    public T GetRecord(int id)
    {
        if (id == 0)
        {
            return default;
        }

        T val = null;
        if (_tableDic.TryGetValue(id, out val))
        {
            return val;
        }
        try
        {
            return GetDataFromIndex(id);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public bool TryGetRecord(int id, out T data)
    {
        if (_tableDic.TryGetValue(id, out data))
        {
            return true;
        }

        data = GetDataFromIndex(id);

        return data != default;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Length; i++)
        {
            yield return this[i];
        }
    }

    // 实现非泛型版本的 GetEnumerator
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private T GetDataFromIndex(int id)
    {
        if (_tableIndexList != null && _tableIndexList._tableIndices.TryGetValue(id, out var list))
        {
            lock (SharedBuffer.locker)
            {
                var fs = _tableIndexList.tableRef;
                var index = _tableIndexList.tableIndex + list.Index + _tableIndexList.tableOffset;
                var len = list.Length;

                var temp = SharedBuffer.GetBuffer(len);
                if (_tableIndexList.bytes != null)
                {
                    Array.Copy(_tableIndexList.bytes, index, temp, 0, len);
                }
                else if (fs != null)
                {

                    fs.Seek(index, SeekOrigin.Begin);
                    var _ = fs.Read(temp, 0, len);
                }

                var t = new T();
                //ProtoUtil.BytesToJceStruct(temp, len, t);
                _tableDic[id] = t;
                return t;
            }
        }
        return default;
    }
}