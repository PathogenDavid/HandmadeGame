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

    public bool UpdateBoardFromInv(Vector2 pos, int invPos, Image img) {
        if (pos[0] == -1 || pos[1] == -1) return false; // invalid tile replacement
        // check if board already has something in that slot
        // if it does, move it to inventory
        if (Board[(int)pos[0], (int)pos[1]] != 0) {
            // find first empty inventory slot
            int emptyInvIndex = -1;
            for (int i = 0; i < Inventory.Count; i++) {
                if (i == invPos || Inventory[i] == -1) {
                    emptyInvIndex = i;
                    break;
                }
            }
            if (emptyInvIndex == invPos) {
                int intPos = (int)pos[1] + (3 * (int)pos[0]);
                Image temp = Image.Instantiate(hotbar[emptyInvIndex].GetComponent<Image>());
                hotbar[emptyInvIndex].GetComponent<Image>().color = internalDisplay[intPos].GetComponent<Image>().color;
                int temp2 = Inventory[emptyInvIndex];
                Inventory[emptyInvIndex] = Board[(int)pos[0], (int)pos[1]];
                Inventory[invPos] = Board[(int)pos[0], (int)pos[1]];
                Board[(int)pos[0], (int)pos[1]] = temp2;
                internalDisplay[intPos].SetActive(true);
                internalDisplay[intPos].GetComponent<Image>().color = temp.color;
                return true;
            } else {
                hotbar[emptyInvIndex].SetActive(true);
                int internalPos = (int)pos[1] + (3 * (int)pos[0]);
                hotbar[emptyInvIndex].GetComponent<Image>().color = internalDisplay[internalPos].GetComponent<Image>().color;
                Inventory[emptyInvIndex] = Board[(int)pos[0], (int)pos[1]];
                Board[(int)pos[0], (int)pos[1]] = Inventory[invPos];
                Inventory[invPos] = -1;
                internalDisplay[internalPos].SetActive(true);
                internalDisplay[internalPos].GetComponent<Image>().color = img.color;
                hotbar[invPos].SetActive(false);
                return true;
            }
        } else { // otherwise, just move it
            Board[(int)pos[0], (int)pos[1]] = Inventory[invPos];
            Inventory[invPos] = -1;
            int internalPosAgain = (int)pos[1] + (3 * (int)pos[0]);
            internalDisplay[internalPosAgain].SetActive(true);
            internalDisplay[internalPosAgain].GetComponent<Image>().color = img.color;
            hotbar[invPos].SetActive(false);
            return true;
        }
    }

    public bool UpdateBoardFromBoard(Vector2 newPos, int invPos, Vector2 curPos, Image img) {
        if (newPos[0] == -1 || newPos[1] == -1) return false; // invalid tile replacement
        // check if board already has something in that slot
        // if it does, move it to inventory
        if (Board[(int)newPos[0], (int)newPos[1]] != 0) {
            // find first empty inventory slot
            int emptyInvIndex = -1;
            for (int i = 0; i < Inventory.Count; i++) {
                if (i == invPos || Inventory[i] == -1) {
                    emptyInvIndex = i;
                    break;
                }
            }
            hotbar[emptyInvIndex].SetActive(true);
            int newInternalPos = (int)newPos[1] + (3 * (int)newPos[0]);
            int curInternalPos = (int)curPos[1] + (3 * (int)curPos[0]);
            hotbar[emptyInvIndex].GetComponent<Image>().color = internalDisplay[newInternalPos].GetComponent<Image>().color;
            Inventory[emptyInvIndex] = Board[(int)newPos[0], (int)newPos[1]];
            Board[(int)newPos[0], (int)newPos[1]] = Board[(int)curPos[0], (int)curPos[1]];
            Board[(int)curPos[0], (int)curPos[1]] = 0;
            internalDisplay[newInternalPos].GetComponent<Image>().color = img.color;
            internalDisplay[curInternalPos].SetActive(false);
            return true;
        } else { // otherwise, just move it
            Board[(int)newPos[0], (int)newPos[1]] = Board[(int)curPos[0], (int)curPos[1]];
            Board[(int)curPos[0], (int)curPos[1]] = 0;
            int newInternalPos = (int)newPos[1] + (3 * (int)newPos[0]);
            int curInternalPos = (int)curPos[1] + (3 * (int)curPos[0]);
            internalDisplay[newInternalPos].SetActive(true);
            internalDisplay[newInternalPos].GetComponent<Image>().color = img.color;
            internalDisplay[curInternalPos].SetActive(false);
            return true;
        }
    }
}
