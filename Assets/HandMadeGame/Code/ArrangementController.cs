using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrangementController : MonoBehaviour
{
    public GameObject inventory;
    public GameObject grid;

    public int[,] Board = new int[3,3];
    public List<GameObject> internalDisplay = new();
    public readonly int BoardWidth = 3;
    public readonly int BoardHeight = 3;
    
    public List<int> Inventory = new List<int>(9);
    public List<GameObject> hotbar = new List<GameObject>(9);

    private Quest currentQuest;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartArrangementMode(Quest quest) {
        currentQuest = quest;
    }

    private void ShowInventory() {
        inventory.SetActive(true);
    }

    private void HideInventory() {
        inventory.SetActive(false);
    }

    private void ShowGrid() {
        grid.SetActive(true);
    }
    
    private void HideGrid() {
        grid.SetActive(false);
    }

    public void UpdateBoard(Vector2 pos, int invPos, Image img) {
        Debug.Log(Board[(int)pos[0], (int)pos[1]]);
        Board[(int)pos[0], (int)pos[1]] = Inventory[invPos];
        Inventory[invPos] = -1;
        int internalPos = (int)pos[1] + (3 * (int)pos[0]);
        internalDisplay[internalPos].SetActive(true);
        internalDisplay[internalPos].GetComponent<Image>().color = img.color;
    }
}
