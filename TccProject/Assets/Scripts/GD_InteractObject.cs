using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class GD_InteractObject : MonoBehaviour
{

    public string interactionText = "Interact";
    public UnityEvent onInteract;

    [SerializeField] DialogoSo dialogData;

    public string GetInteractionText()
    {
        return interactionText;
    }

    public void Interact()
    {
        onInteract.Invoke();

    }
    public void GetStartDialog()
    {
            GameEvents.Instance.StartDialog(dialogData);
     }
   
}