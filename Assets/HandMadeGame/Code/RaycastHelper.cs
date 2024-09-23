using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastHelper : MonoBehaviour
{
    public Camera cam;

    public Material mainMat;
    public Material mainVar;
    public Material highlightMat;
    public Material highlightVar;

    private Material[] mainMats = new Material[2];
    private Material[] highlightMats = new Material[2];

    public HighlightController bottle;
    public HighlightController can;
    public HighlightController candyMan;
    public HighlightController childsToy;
    public HighlightController christmasTree;
    public HighlightController bunny;
    public HighlightController frog;
    public HighlightController present;
    public HighlightController sock;
    public HighlightController star;
    public HighlightController trash;

    private Ray ray;
    private RaycastHit hit;
    
    // Start is called before the first frame update
    void Start()
    {
        mainMats[0] = mainMat;
        mainMats[1] = mainVar;
        highlightMats[0] = highlightMat;
        highlightMats[1] = highlightVar;
    }

    // Update is called once per frame
    void Update()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) {
            Debug.Log(hit.collider.name);
            if (hit.collider.name == "bottle") {
                bottle.ChangeMats(highlightMats);
                can.ChangeMats(mainMats);
                candyMan.ChangeMats(mainMats);
                childsToy.ChangeMats(mainMats);
                christmasTree.ChangeMats(mainMats);
                bunny.ChangeMats(mainMats);
                frog.ChangeMats(mainMats);
                present.ChangeMats(mainMats);
                sock.ChangeMats(mainMats);
                star.ChangeMats(mainMats);
                trash.ChangeMats(mainMats);
            } else if (hit.collider.name == "can") {
                bottle.ChangeMats(mainMats);
                can.ChangeMats(highlightMats);
                candyMan.ChangeMats(mainMats);
                childsToy.ChangeMats(mainMats);
                christmasTree.ChangeMats(mainMats);
                bunny.ChangeMats(mainMats);
                frog.ChangeMats(mainMats);
                present.ChangeMats(mainMats);
                sock.ChangeMats(mainMats);
                star.ChangeMats(mainMats);
                trash.ChangeMats(mainMats);
            } else if (hit.collider.name == "candy man") {
                bottle.ChangeMats(mainMats);
                can.ChangeMats(mainMats);
                candyMan.ChangeMats(highlightMats);
                childsToy.ChangeMats(mainMats);
                christmasTree.ChangeMats(mainMats);
                bunny.ChangeMats(mainMats);
                frog.ChangeMats(mainMats);
                present.ChangeMats(mainMats);
                sock.ChangeMats(mainMats);
                star.ChangeMats(mainMats);
                trash.ChangeMats(mainMats);
            } else if (hit.collider.name == "child's toy") {
                bottle.ChangeMats(mainMats);
                can.ChangeMats(mainMats);
                candyMan.ChangeMats(mainMats);
                childsToy.ChangeMats(mainMats);
                christmasTree.ChangeMats(mainMats);
                bunny.ChangeMats(mainMats);
                frog.ChangeMats(mainMats);
                present.ChangeMats(mainMats);
                sock.ChangeMats(mainMats);
                star.ChangeMats(mainMats);
                trash.ChangeMats(mainMats);
            } else if (hit.collider.name == "Christmas tree") {
                bottle.ChangeMats(mainMats);
                can.ChangeMats(mainMats);
                candyMan.ChangeMats(mainMats);
                childsToy.ChangeMats(mainMats);
                christmasTree.ChangeMats(highlightMats);
                bunny.ChangeMats(mainMats);
                frog.ChangeMats(mainMats);
                present.ChangeMats(mainMats);
                sock.ChangeMats(mainMats);
                star.ChangeMats(mainMats);
                trash.ChangeMats(mainMats);
            } else if (hit.collider.name == "plush bunny") {
                bottle.ChangeMats(mainMats);
                can.ChangeMats(mainMats);
                candyMan.ChangeMats(mainMats);
                childsToy.ChangeMats(mainMats);
                christmasTree.ChangeMats(mainMats);
                bunny.ChangeMats(highlightMats);
                frog.ChangeMats(mainMats);
                present.ChangeMats(mainMats);
                sock.ChangeMats(mainMats);
                star.ChangeMats(mainMats);
                trash.ChangeMats(mainMats);
            } else if (hit.collider.name == "plush frog") {
                bottle.ChangeMats(mainMats);
                can.ChangeMats(mainMats);
                candyMan.ChangeMats(mainMats);
                childsToy.ChangeMats(mainMats);
                christmasTree.ChangeMats(mainMats);
                bunny.ChangeMats(mainMats);
                frog.ChangeMats(highlightMats);
                present.ChangeMats(mainMats);
                sock.ChangeMats(mainMats);
                star.ChangeMats(mainMats);
                trash.ChangeMats(mainMats);
            } else if (hit.collider.name == "present") {
                bottle.ChangeMats(mainMats);
                can.ChangeMats(mainMats);
                candyMan.ChangeMats(mainMats);
                childsToy.ChangeMats(mainMats);
                christmasTree.ChangeMats(mainMats);
                bunny.ChangeMats(mainMats);
                frog.ChangeMats(mainMats);
                present.ChangeMats(highlightMats);
                sock.ChangeMats(mainMats);
                star.ChangeMats(mainMats);
                trash.ChangeMats(mainMats);
            } else if (hit.collider.name == "sock") {
                bottle.ChangeMats(mainMats);
                can.ChangeMats(mainMats);
                candyMan.ChangeMats(mainMats);
                childsToy.ChangeMats(mainMats);
                christmasTree.ChangeMats(mainMats);
                bunny.ChangeMats(mainMats);
                frog.ChangeMats(mainMats);
                present.ChangeMats(mainMats);
                sock.ChangeMats(highlightMats);
                star.ChangeMats(mainMats);
                trash.ChangeMats(mainMats);
            } else if (hit.collider.name == "star") {
                bottle.ChangeMats(mainMats);
                can.ChangeMats(mainMats);
                candyMan.ChangeMats(mainMats);
                childsToy.ChangeMats(mainMats);
                christmasTree.ChangeMats(mainMats);
                bunny.ChangeMats(mainMats);
                frog.ChangeMats(mainMats);
                present.ChangeMats(mainMats);
                sock.ChangeMats(mainMats);
                star.ChangeMats(highlightMats);
                trash.ChangeMats(mainMats);
            } else if (hit.collider.name == "trash") {
                bottle.ChangeMats(mainMats);
                can.ChangeMats(mainMats);
                candyMan.ChangeMats(mainMats);
                childsToy.ChangeMats(mainMats);
                christmasTree.ChangeMats(mainMats);
                bunny.ChangeMats(mainMats);
                frog.ChangeMats(mainMats);
                present.ChangeMats(mainMats);
                sock.ChangeMats(mainMats);
                star.ChangeMats(mainMats);
                trash.ChangeMats(highlightMats);
            } else {
                bottle.ChangeMats(mainMats);
                can.ChangeMats(mainMats);
                candyMan.ChangeMats(mainMats);
                childsToy.ChangeMats(mainMats);
                christmasTree.ChangeMats(mainMats);
                bunny.ChangeMats(mainMats);
                frog.ChangeMats(mainMats);
                present.ChangeMats(mainMats);
                sock.ChangeMats(mainMats);
                star.ChangeMats(mainMats);
                trash.ChangeMats(mainMats);
            }
        }
    }
}
