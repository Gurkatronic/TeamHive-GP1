using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrototypeStartWindow : MonoBehaviour
{
    [SerializeField] PrototypeControllerBox[] prototypeControllers;
    [SerializeField] Button startButton;

    private void Update() {
        PlayerSlot[] players = ControllerDetector.Instance.slots;

        for(int i = 0; i < players.Length; i++) {
            if (players[i].isConnected && !prototypeControllers[i].showing) {
                startButton.interactable = true;
                if (players[i].controllerGlyphs != null) {
                    ControllerGlyphsScriptableObject glyphLibrary = players[i].controllerGlyphs;
                    prototypeControllers[i].FillImages(glyphLibrary.GetControllerGlyph(ControllerButtons.ButtonWest),
                                                        glyphLibrary.GetControllerGlyph(ControllerButtons.ButtonEast), 
                                                        glyphLibrary.GetControllerGlyph(ControllerButtons.ControllerSymbol));
                }
                else {
                    prototypeControllers[i].FillImages(null, null, null);
                }
                
            }
        }
    }

    public void StartGame() {
        //save on controller
        ControllerDetector.Instance.SaveControllers();
        //Load on gamemanager
        GameManager.Instance.TestSessionPackage();
        //start on gamemanger
        GameManager.Instance.TestSpawnPlayers();
        gameObject.SetActive(false);
    }
}
