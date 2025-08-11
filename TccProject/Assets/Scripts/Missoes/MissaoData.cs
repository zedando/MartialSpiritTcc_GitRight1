using UnityEngine;

[CreateAssetMenu(fileName = "NovaMissao", menuName = "Missao/Nova Missao")]
public class MissaoData : ScriptableObject
{
    [Header("Informações da Missão")]
    public string titulo;
    [TextArea] public string descricao;
}
