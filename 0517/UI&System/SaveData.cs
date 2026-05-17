using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public Vector3 playerPosition;
    public string mapBounddary;

    public string currentSceneName;

    // 【新增】將需要跨遊戲進度保存的資料寫入存檔中
    public bool[] unlockedAreas;
    public string respawnSceneName;
    public Vector2 respawnPosition;
    public bool hasCheckpoint;

    public bool hasBossKey;
    public bool isBossDefeated;
}
