using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RL
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager instance;

        public UnityAction OnDialogueStart;
        public UnityAction OnDialogueEnd;
        public DialoguePanel DialogPrefab;

        protected bool _isDialogueInProgress;

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        public void StartDialogue(GameObject actor)
        {
            _isDialogueInProgress = true;
            OnDialogueStart();

            var dia = Instantiate(DialogPrefab); //Spawn Dialog Prefab
            dia.Init();
        }
        public void CloseDialogue()
        {
            _isDialogueInProgress = false;
            OnDialogueEnd();
        }
    }

}