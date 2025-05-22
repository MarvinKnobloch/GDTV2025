using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialog", menuName = "ScriptableObjects/DialogMessage")]
public class DialogObj : ScriptableObject
{
    public bool cantSkipDialog;
    public bool disableInputs;
    public bool pauseGame;
    public Dialog[] dialogs;

    public VoidEventChannel[] eventAfterDialogEnd;
}
[Serializable]
public struct Dialog
{
    public Sprite characterSprite;
    public string characterName;
    [TextArea]
    public string dialogText;
    public VoidEventChannel[] dialogEvent;
    public float autoPlayInterval;
}
