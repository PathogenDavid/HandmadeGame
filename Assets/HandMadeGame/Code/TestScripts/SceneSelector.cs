using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class SceneSelector : MonoBehaviour
{
    public void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Scene selector");
        if (GUILayout.Button("X"))
            gameObject.SetActive(false);
        GUILayout.EndHorizontal();

        void SceneButton(string sceneName)
        {
            if (GUILayout.Button(sceneName))
                SceneManager.LoadScene(sceneName);
        }

        SceneButton("GameplayFlowTestScene");
        SceneButton("BirdController");
        SceneButton("Main");
    }
}
