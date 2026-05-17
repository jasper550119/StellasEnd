using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 新增一個類別來儲存「單句對話」的所有資訊
[System.Serializable]
public class DialogueLine
{
    public string speakerName;         // 這句話的說話者名字
    public Sprite speakerPortrait;     // 這句話的說話者頭像
    [TextArea(2, 5)]
    public string sentence;            // 對話內容
    
    public bool autoProgress;          // 是否自動播放下一句
    public AudioClip voiceSound;       // (可選) 獨立的語音音效
}

[CreateAssetMenu(fileName ="NewMultiDialog", menuName ="NPC Dialogue (Multi)")]
public class NPCDialogue : ScriptableObject
{
    [Header("對話內容")]
    public DialogueLine[] lines;       // 儲存所有對話句子的陣列

    [Header("全域對話設定")]
    public float autoProgressDelay = 1.5f;
    public float typingSpeed = 0.05f;
    public float voicePitch = 1f;
}