using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RL
{
    public class DialoguePanel : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI DialogText;
        public Button YesBtn;
        public Button NoBtn;

        protected int _dialogID;

        public void Init()
        {
            
        }
        public void Yes()
        {

        }
        public void No()
        {

        }
        public void Cancel()
        {
            DialogueManager.instance.CloseDialogue();
        }
    }
}