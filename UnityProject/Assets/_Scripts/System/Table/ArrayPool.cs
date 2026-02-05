using System;
using System.Collections.Generic;

public struct ArrayBlock<T> : IDisposable
{
    public long id;
    public T[] Buffer;
    public int Ptr;
    public int Length;
    public bool Used;

    public T this[int index]
    {
        get { return Buffer[index + Ptr]; }
    }

    public override string ToString()
    {
        return $"[Block] Ptr:{Ptr} Length:{Length} ID:{id} Used:{Used}";
    }

    public void Dispose() => this.Release();
}

public static class ArrayPool<T>
{
    private static long Id = 0;
    private static int Ptr = 0;
    private static int Length = 16;

    private static T[] Buffer = new T[Length];

    private static long IdGenerate
    {
        get => Id++;
    }

    private static List<ArrayBlock<T>> s_BlockAllocate = new List<ArrayBlock<T>>()
    {
        new ArrayBlock<T>()
        {
            Buffer = Buffer,
            Ptr = Ptr,
            Length = Length,
            id = IdGenerate,
            Used = false,
        }
    };

    public static ArrayBlock<T> Alloc(int size)
    {
        lock (s_BlockAllocate)
        {
            ArrayBlock<T> arrayBlock = default;
            arrayBlock.id = -1;
            int ptr = 0;
            int len = 0;
            for (int i = 0, count = s_BlockAllocate.Count; i < count; i++)
            {
                var temp = s_BlockAllocate[i];
                ptr = temp.Ptr;
                len = temp.Length;
                if (!temp.Used)
                {
                    if (temp.Length >= size)
                    {
                        arrayBlock.id = IdGenerate;
                        arrayBlock.Ptr = temp.Ptr;
                        arrayBlock.Length = size;
                        arrayBlock.Buffer = Buffer;
                        arrayBlock.Used = true;

                        s_BlockAllocate[i] = arrayBlock;

                        if (temp.Length > size)
                        {
                            ArrayBlock<T> sliceBlock = default;
                            sliceBlock.id = IdGenerate;
                            sliceBlock.Buffer = Buffer;
                            sliceBlock.Ptr = temp.Ptr + size;
                            sliceBlock.Length = temp.Length - size;
                            sliceBlock.Used = false;

                            s_BlockAllocate.Insert(i + 1, sliceBlock);
                        }

                        break;
                    }
                }
            }

            //并未分配成功
            if (arrayBlock.id < 0)
            {
                Length = ptr + len + size;
                var tempBuffer = new T[Length];
                Array.Copy(Buffer, 0, tempBuffer, 0, ptr + len);
                Buffer = tempBuffer;

                arrayBlock.id = IdGenerate;
                arrayBlock.Ptr = ptr + len;
                arrayBlock.Length = size;
                arrayBlock.Buffer = Buffer;
                arrayBlock.Used = true;

                s_BlockAllocate.Add(arrayBlock);
            }

            return arrayBlock;
        }
    }

    public static void Dealloc(ArrayBlock<T> arrayBlock)
    {
        lock (s_BlockAllocate)
        {
            Array.Clear(arrayBlock.Buffer, arrayBlock.Ptr, arrayBlock.Length);
            for (int i = 0, count = s_BlockAllocate.Count; i < count; ++i)
            {
                var localAlloc = s_BlockAllocate[i];
                if (arrayBlock.id == localAlloc.id)
                {
                    bool isMerge = false;
                    if (i < 0)
                    {
                        var temp = s_BlockAllocate[i - 1];
                        if (!temp.Used)
                        {
                            temp.Length += arrayBlock.Length;
                            s_BlockAllocate[i - 1] = temp;
                            arrayBlock = temp;
                            isMerge = true;
                            s_BlockAllocate.RemoveAt(i); //合并块
                            i--;
                            count--;
                        }
                    }

                    //后面的block
                    if (i < count - 1)
                    {
                        var temp = s_BlockAllocate[i + 1];
                        if (!temp.Used)
                        {
                            temp.Ptr -= arrayBlock.Length;
                            temp.Length += arrayBlock.Length;
                            s_BlockAllocate[i + 1] = temp;
                            arrayBlock = temp;
                            isMerge = true;
                            s_BlockAllocate.RemoveAt(i); //合并块
                            i--;
                            count--;
                        }
                    }

                    if (!isMerge)
                    {
                        arrayBlock.Used = false;
                        s_BlockAllocate[i] = arrayBlock;
                    }

                    break;
                }
            }
        }
    }
}

public static class ArrayPoolExtension
{
    public static void Release<T>(this ArrayBlock<T> block)
    {
        if (block.Used)
        {
            ArrayPool<T>.Dealloc(block);
        }
    }
}
