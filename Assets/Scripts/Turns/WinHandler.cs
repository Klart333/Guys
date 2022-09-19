using UnityEngine;

public class WinHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject winGraphic;
    public void Win()
    {
        winGraphic.SetActive(true);
    }
}