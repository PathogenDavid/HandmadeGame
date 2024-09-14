using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RL
{
    public class MainMenu : MonoBehaviour
    {
        public void StartGame(string scn)
        {
            GameSceneManager.instance.ChangeScene(scn);
        }
        public void QuitGame()
        {
            GameSceneManager.instance.QuitGame();
        }
    }
}
