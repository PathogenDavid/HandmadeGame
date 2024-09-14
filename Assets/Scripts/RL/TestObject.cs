using RL;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObject : MonoBehaviour, IInteractable
{
    public GameObject SelectIcon;
    public void Interact()
    {
        Debug.Log("Test");
    }

    public void Selected()
    {
        SelectIcon.SetActive(true);
    }

    public void Unselected()
    {
        SelectIcon.SetActive(false);
    }
}
