using UnityEngine;
using System;

[Serializable]
public class LabItemInstance
{
    // 玩家在操作过程中改变了的实验状态记录（比如瓶子里装了水）
    public string instanceID;       // 如 "bottle1"
    public float volumeML;          // 当前体积（如 800）
    public string content;          // 内容物（如 "deionised water"）
    public bool isMarked;           // 是否已标记
}
