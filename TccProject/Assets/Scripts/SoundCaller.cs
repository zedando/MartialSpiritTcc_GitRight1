using UnityEngine;
using FMODUnity;

public class SoundCaller : MonoBehaviour
{
    [Header("FMOD Events")]
    public string clickEvent = "event:/ui/click"; 
    public string doorEvent = "event:/ambiente/porta";

   
    
    public void PlayClick()
    {
        RuntimeManager.PlayOneShot(clickEvent);
    }


    public void PlayDoor()
    {
        RuntimeManager.PlayOneShot(doorEvent);
    }
}
