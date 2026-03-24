using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnSpace : MonoBehaviour
{
#if UNITY_EDITOR
    public UnityEditor.SceneAsset sceneAsset; // Drag your scene here in the Inspector
#endif

    [HideInInspector]
    public string sceneName;

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (sceneAsset != null)
            sceneName = sceneAsset.name; // Automatically get the scene name
#endif
    }

    void Update()
    {
        // Check if Space is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!string.IsNullOrEmpty(sceneName))
            {
                SceneManager.LoadScene(sceneName); // Load the specific scene
            }
            else
            {
                Debug.LogWarning("Scene not assigned in the Inspector!");
            }
        }
    }
}
