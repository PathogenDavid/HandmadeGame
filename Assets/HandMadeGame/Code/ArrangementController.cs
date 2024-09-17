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

    public bool UpdateBoardFromInv(Vector2Int pos, int invPos, Image img) {
        if (pos[0] == -1 || pos[1] == -1) return false; // invalid tile replacement
        // check if board already has something in that slot
        // if it does, move it to inventory
        if (Board[pos[0], pos[1]] != 0) {
            // find first empty inventory slot
            int emptyInvIndex = -1;
            for (int i = 0; i < Inventory.Count; i++) {
                if (i == invPos || Inventory[i] == -1) {
                    emptyInvIndex = i;
                    break;
                }
            }
            if (emptyInvIndex == invPos) {
                int intPos = pos[1] + (3 * pos[0]);
                Image temp = Image.Instantiate(hotbar[emptyInvIndex].GetComponent<Image>());
                hotbar[emptyInvIndex].GetComponent<Image>().color = internalDisplay[intPos].GetComponent<Image>().color;
                int temp2 = Inventory[emptyInvIndex];
                Inventory[emptyInvIndex] = Board[pos[0], pos[1]];
                Inventory[invPos] = Board[pos[0], pos[1]];
                Board[pos[0], pos[1]] = temp2;
                internalDisplay[intPos].SetActive(true);
                internalDisplay[intPos].GetComponent<Image>().color = temp.color;
                return true;
            } else {
                hotbar[emptyInvIndex].SetActive(true);
                int internalPos = pos[1] + (3 * pos[0]);
                hotbar[emptyInvIndex].GetComponent<Image>().color = internalDisplay[internalPos].GetComponent<Image>().color;
                Inventory[emptyInvIndex] = Board[pos[0], pos[1]];
                Board[pos[0], pos[1]] = Inventory[invPos];
                Inventory[invPos] = -1;
                internalDisplay[internalPos].SetActive(true);
                internalDisplay[internalPos].GetComponent<Image>().color = img.color;
                hotbar[invPos].SetActive(false);
                return true;
            }
        } else { // otherwise, just move it
            Board[pos[0], pos[1]] = Inventory[invPos];
            Inventory[invPos] = -1;
            int internalPosAgain = pos[1] + (3 * pos[0]);
            internalDisplay[internalPosAgain].SetActive(true);
            internalDisplay[internalPosAgain].GetComponent<Image>().color = img.color;
            hotbar[invPos].SetActive(false);
            return true;
        }
    }

    public bool UpdateBoardFromBoard(Vector2Int newPos, int invPos, Vector2Int curPos, Image img) {
        if (newPos[0] == -1 || newPos[1] == -1) return false; // invalid tile replacement
        // check if board already has something in that slot
        // if it does, move it to inventory
        if (Board[newPos[0], newPos[1]] != 0) {
            // find first empty inventory slot
            int emptyInvIndex = -1;
            for (int i = 0; i < Inventory.Count; i++) {
                if (i == invPos || Inventory[i] == -1) {
                    emptyInvIndex = i;
                    break;
                }
            }
            hotbar[emptyInvIndex].SetActive(true);
            int newInternalPos = newPos[1] + (3 * newPos[0]);
            int curInternalPos = curPos[1] + (3 * curPos[0]);
            hotbar[emptyInvIndex].GetComponent<Image>().color = internalDisplay[newInternalPos].GetComponent<Image>().color;
            Inventory[emptyInvIndex] = Board[newPos[0], newPos[1]];
            Board[newPos[0], newPos[1]] = Board[curPos[0], curPos[1]];
            Board[curPos[0], curPos[1]] = 0;
            internalDisplay[newInternalPos].GetComponent<Image>().color = img.color;
            internalDisplay[curInternalPos].SetActive(false);
            return true;
        } else { // otherwise, just move it
            Board[newPos[0], newPos[1]] = Board[curPos[0], curPos[1]];
            Board[curPos[0], curPos[1]] = 0;
            int newInternalPos = newPos[1] + (3 * newPos[0]);
            int curInternalPos = curPos[1] + (3 * curPos[0]);
            internalDisplay[newInternalPos].SetActive(true);
            internalDisplay[newInternalPos].GetComponent<Image>().color = img.color;
            internalDisplay[curInternalPos].SetActive(false);
            return true;
        }
    }

    public bool UpdateInvFromBoard(Vector2Int curPos, int invPos, Image img) {
        // check if inventory space is full
        if (Inventory[invPos] != -1) return false;
        // otherwise, move it to the inventory
        Inventory[invPos] = Board[curPos[0], curPos[1]];
        hotbar[invPos].SetActive(true);
        hotbar[invPos].GetComponent<Image>().color = img.color;
        int internalPos = curPos[1] + (3 * curPos[0]);
        Board[curPos[0], curPos[1]] = 0;
        internalDisplay[internalPos].SetActive(false);
        return true;
    }

    public bool UpdateInvFromInv(int curInvPos, int newInvPos, Image img) {
        // check if inventory space is full. if so, swap items
        if (Inventory[newInvPos] != -1) {
            //Image temp = Image.Instantiate(hotbar[emptyInvIndex].GetComponent<Image>());
            int tempInt = Inventory[newInvPos];
            Inventory[newInvPos] = Inventory[curInvPos];
            Inventory[newInvPos] = tempInt;
            Image tempCurImage = Image.Instantiate(hotbar[curInvPos].GetComponent<Image>());
            Image tempNewImage = Image.Instantiate(hotbar[newInvPos].GetComponent<Image>());
            hotbar[newInvPos].GetComponent<Image>().color = tempCurImage.color;
            hotbar[curInvPos].GetComponent<Image>().color = tempNewImage.color;
            return true;
        }
        // otherwise,  just place it
        Inventory[newInvPos] = Inventory[curInvPos];
        hotbar[newInvPos].SetActive(true);
        hotbar[newInvPos].GetComponent<Image>().color = img.color;
        Inventory[curInvPos] = -1;
        hotbar[curInvPos].SetActive(false);
        return true;
    }

    internal static int FindInventoryLocation(Vector2 mouseCoords) {
        int boardPos = -1;
        //Debug.Log(new Vector2(mouseCoords[0], Screen.width));
        // first, make sure on inventory
        if (mouseCoords[0] < .15 * Screen.width || mouseCoords[0] > .85 * Screen.width || mouseCoords[1] < .08 * Screen.height || mouseCoords[1] > .18 * Screen.height)
            return -1;
        if (mouseCoords[0] < .22 * Screen.width) {
            boardPos = 0;
        } else if (mouseCoords[0] < .3 * Screen.width) {
            boardPos = 1;
        } else if (mouseCoords[0] < .38 * Screen.width) {
            boardPos = 2;
        } else if (mouseCoords[0] < .46 * Screen.width) {
            boardPos = 3;
        } else if (mouseCoords[0] < .54 * Screen.width) {
            boardPos = 4;
        } else if (mouseCoords[0] < .62 * Screen.width) {
            boardPos = 5;
        } else if (mouseCoords[0] < .7 * Screen.width) {
            boardPos = 6;
        } else if (mouseCoords[0] < .78 * Screen.width) {
            boardPos = 7;
        } else if (mouseCoords[0] < .85 * Screen.width) {
            boardPos = 8;
        } else {
            boardPos = -1;
            Debug.Log("stop breaking my code >:(");
        }
        return boardPos;
    }

    internal static Vector2Int FindTileLocation(Vector2 mouseCoords) {
        float relativeX = mouseCoords[0];
        float relativeY = mouseCoords[1];
        Vector2Int boardPos = new(-1, -1);
        // first, make sure on board
        if (relativeX < Screen.width / 3 || relativeX > 2 * Screen.width / 3 || relativeY < .314 * Screen.height || relativeY > .87 * Screen.height) {
            return boardPos;
        }
        // find col
        if (relativeX < .45 * Screen.width) {
            boardPos = new Vector2Int(boardPos[0], 0);
        } else if (relativeX < .55 * Screen.width) {
            boardPos = new Vector2Int(boardPos[0], 1);
        } else if (relativeX < .65 * Screen.width) {
            boardPos = new Vector2Int(boardPos[0], 2);
        } else {
            boardPos = new Vector2Int(boardPos[0], -1);
            Debug.Log("you broke it. good job.");
        }
        // find row
        if (relativeY < Screen.height / 2) {
            boardPos = new Vector2Int(2, boardPos[1]);
        } else if (relativeY < .68 * Screen.height) {
            boardPos = new Vector2Int(1, boardPos[1]);
        } else if (relativeY < .86 * Screen.height) {
            boardPos = new Vector2Int(0, boardPos[1]);
        } else {
            boardPos = new Vector2Int(-1, boardPos[1]);
            Debug.Log("wow. you still broke it. i feel attacked.");
        }
        return boardPos;
    }
}
