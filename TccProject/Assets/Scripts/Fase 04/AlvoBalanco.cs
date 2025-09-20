using UnityEngine;
using System.Collections;

public class AlvoBalanco : MonoBehaviour
{
  public float velocidade = 5f; 
    private Quaternion rotacaoInicial;

    void Start()
    {
        rotacaoInicial = transform.localRotation;
    }

    public void Balancar(float angulo)
    {
        StopAllCoroutines();
        StartCoroutine(BalancarRoutine(angulo));
    }
    public void BalancarComEixo(float anguloX, float anguloY, float anguloZ)
{
    StopAllCoroutines();
    StartCoroutine(BalancarRoutineEixo(anguloX, anguloY, anguloZ));
}

IEnumerator BalancarRoutineEixo(float anguloX, float anguloY, float anguloZ)
{
    Quaternion rotacaoInicial = transform.localRotation;
    Quaternion rotacaoFinal = Quaternion.Euler(anguloX, anguloY, anguloZ) * rotacaoInicial;

    float t = 0;
    while (t < 1)
    {
        t += Time.deltaTime * velocidade;
        transform.localRotation = Quaternion.Slerp(rotacaoInicial, rotacaoFinal, t);
        yield return null;
    }

    t = 0;
    while (t < 1)
    {
        t += Time.deltaTime * velocidade;
        transform.localRotation = Quaternion.Slerp(rotacaoFinal, rotacaoInicial, t);
        yield return null;
    }
}


    IEnumerator BalancarRoutine(float angulo)
    {
        Quaternion rotacaoFrente = Quaternion.Euler(angulo, 0, 0) * rotacaoInicial;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * velocidade;
            transform.localRotation = Quaternion.Slerp(rotacaoInicial, rotacaoFrente, t);
            yield return null;
        }

        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * velocidade;
            transform.localRotation = Quaternion.Slerp(rotacaoFrente, rotacaoInicial, t);
            yield return null;
        }
    }
}