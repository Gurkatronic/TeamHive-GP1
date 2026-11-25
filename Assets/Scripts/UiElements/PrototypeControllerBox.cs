using UnityEngine;
using UnityEngine.UI;

public class PrototypeControllerBox : MonoBehaviour
{
    [SerializeField] Image attackButton;
    [SerializeField] Image dropButton;
    [SerializeField] Image controllerImage;
    public bool showing = false;

    public void FillImages(Sprite attack, Sprite drop, Sprite controller) {
        attackButton.sprite = attack;
        dropButton.sprite = drop;
        controllerImage.sprite = controller;
        gameObject.SetActive(true);
    }
}
