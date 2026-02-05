using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

public interface ITweenPlayer
{
    UniTask PlayOpen();
    UniTask PlayClose();
}
