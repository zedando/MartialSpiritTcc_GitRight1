using UnityEngine;

public class BotaoVirtual : MonoBehaviour
{
    public InputReceptor receptor;
    public string tecla; // J, K, U, L, S

    public void Acionar()
    {
        switch (tecla.ToUpper())
        {
            case "J": receptor.PressionarJ(); break;
            case "K": receptor.PressionarK(); break;
            case "U": receptor.PressionarU(); break;
            case "L": receptor.PressionarL(); break;
            case "S": receptor.PressionarS(); break;
        }
    }
}
