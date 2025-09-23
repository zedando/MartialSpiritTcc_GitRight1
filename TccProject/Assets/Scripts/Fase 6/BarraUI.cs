using UnityEngine;
using UnityEngine.UI;

public class BarraUI : MonoBehaviour
{
    public Slider slider;

    public void AtualizarBarra(int valorAtual, int valorMax)
    {
        if (slider != null)
            slider.value = (float)valorAtual / valorMax;
    }
}
