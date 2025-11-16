using UnityEngine;
using FMODUnity;

public class AmbientMusic : MonoBehaviour
{
    [EventRef]
    public string eventoAmbiente;

    private FMOD.Studio.EventInstance instancia;

    void Start()
    {
        instancia = RuntimeManager.CreateInstance(eventoAmbiente);
        instancia.start();
    }

    void OnDestroy()
    {
        instancia.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        instancia.release();
    }
}
