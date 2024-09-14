using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RL
{
    public class GameSceneManager : MonoBehaviour
    {
        public static GameSceneManager instance;

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
        /// <summary>
        /// Static method to Spawn a GameScene Manager
        /// </summary>
        public static void CreateManager()
        {
            var go = new GameObject();
            go.AddComponent<GameSceneManager>();
            Instantiate(go);
        }

        // Scene Management
        /// <summary>
        /// Handles Scene Changing with Loading Screen
        /// </summary>
        /// <param name="scn"></param>
        public void ChangeScene(string scn)
        {
            StartCoroutine(ChangeSceneIEnumerator(scn));
        }
        private IEnumerator ChangeSceneIEnumerator(string scn)
        {
            const string loadingscene = "LoadingScreen";
            SceneManager.LoadSceneAsync(loadingscene);
            //TODO: Maybe implement fade in and out down the line

            var op = SceneManager.LoadSceneAsync(scn);
            while(!op.isDone)
            {
                yield return null;
            }
            yield break;
        }
        public void QuitGame()
        {
            Application.Quit();
        }
    }
}