using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class tiaozhuan : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Jump()
    {
        SceneManager.LoadScene(1);
    }
    public void tui()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
