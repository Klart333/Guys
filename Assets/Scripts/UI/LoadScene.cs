using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    [SerializeField]
    private int _sceneIndex;

    public async void Load(float delay)
    {
        await Task.Delay(Mathf.RoundToInt(delay * 1000));
        SceneManager.LoadScene(_sceneIndex);
    }
}
