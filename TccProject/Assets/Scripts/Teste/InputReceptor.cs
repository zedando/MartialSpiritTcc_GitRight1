using UnityEngine;

public class InputReceptor : MonoBehaviour
{
    // Métodos que simulam as teclas (chamados pelo teclado E pelos botões)
    public void PressionarJ()
    {
        Debug.Log("Tecla J (ou botão J) pressionada!");
        // coloque aqui o código de quando J é apertado
    }

    public void PressionarK()
    {
        Debug.Log("Tecla K pressionada!");
    }

    public void PressionarU()
    {
        Debug.Log("Tecla U pressionada!");
    }

    public void PressionarL()
    {
        Debug.Log("Tecla L pressionada!");
    }

    public void PressionarS()
    {
        Debug.Log("Tecla S pressionada!");
    }

    private void Update()
    {
        // → Se apertar pelo teclado, chama os MESMOS métodos
        if (Input.GetKeyDown(KeyCode.J)) PressionarJ();
        if (Input.GetKeyDown(KeyCode.K)) PressionarK();
        if (Input.GetKeyDown(KeyCode.U)) PressionarU();
        if (Input.GetKeyDown(KeyCode.L)) PressionarL();
        if (Input.GetKeyDown(KeyCode.S)) PressionarS();
    }
}
