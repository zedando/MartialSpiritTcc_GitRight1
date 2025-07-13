using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[DefaultExecutionOrder(-1)]
public class GameEvents : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameEvents Instance { get; private set; }

    public void Awake() => Instance = this;
    
    public event Action<DialogoSo> OnStartDialog;
    public void StartDialog(DialogoSo dialogData) => OnStartDialog?.Invoke(dialogData);

    public event Action OnfinishDialog;

    public void FinishDialog() => OnfinishDialog?.Invoke();
}
