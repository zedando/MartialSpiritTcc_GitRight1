using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class GD_PlayerInteract : MonoBehaviour
{
    public float InteractionDistance = 2f;
    public GameObject interactionText;
    [SerializeField] GD_InteractObject currentInteractable;
    private List<GD_InteractObject> interactablesInRange = new List<GD_InteractObject>();

    

    void Start()
    {
        if (interactionText != null)
        {
            interactionText.SetActive(false); // Esconde o texto de interação no início
        }
    }

    void Update()
    {
        // Chama OnInteract em Update para verificar a interação
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            Interact();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GD_InteractObject interactableObject = other.GetComponent<GD_InteractObject>();
        
        
        if (interactableObject != null)
        {
            interactablesInRange.Add(interactableObject);
            if (currentInteractable == null)
            {
                SetCurrentInteractable(interactableObject);
                if (other.gameObject.CompareTag("NPC"))
                {
                    GD_InteractObject objectInteract = currentInteractable.GetComponent<GD_InteractObject>();
                    objectInteract.GetStartDialog();
                }
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        GD_InteractObject interactableObject = other.GetComponent<GD_InteractObject>();
        if (interactableObject != null)
        {
            interactablesInRange.Remove(interactableObject);
            if (interactableObject == currentInteractable)
            {
                RemoveCurrentInteractable();
            }
        }
    }

    private void SetCurrentInteractable(GD_InteractObject interactable)
    {
        currentInteractable = interactable;
        ShowInteractionText();
        GD_InteractObject objectInteract = currentInteractable.GetComponent<GD_InteractObject>();
        
    }

    private void RemoveCurrentInteractable()
    {
        currentInteractable = null;
        interactionText.SetActive(false);
    }

    private void ShowInteractionText()
    {
        if (interactionText != null && currentInteractable != null)
        {
            interactionText.SetActive(true);
            TextMeshProUGUI textComponent = interactionText.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = currentInteractable.GetInteractionText();
            }
        }
        else if (interactionText == null)
        {
            interactionText.SetActive(false);
        }
    }

    public void Interact()
    {
        
        if (currentInteractable != null)
        {
            // Envia uma mensagem para o script GD_ObjectInteract
            GD_InteractObject objectInteract = currentInteractable.GetComponent<GD_InteractObject>();
            if (objectInteract != null)
            {
                objectInteract.Interact(); // Chama o método Interact diretamente
            } 

            // Descomente a linha a seguir se quiser destruir o objeto após a interação
            // if (currentInteractable.GetInteractionText() == "porta de sair") Destroy(currentInteractable.gameObject, 2);
            RemoveCurrentInteractable();
        }
    }
}
