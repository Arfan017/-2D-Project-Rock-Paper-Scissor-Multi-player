using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Hastable = ExitGames.Client.Photon.Hashtable;

public class PilihAvatar : MonoBehaviour
{
    [SerializeField] Image avatarImage;
    [SerializeField] Sprite[] avatarSprites;
    private int selectedIndex;

    private void Start()
    {
        selectedIndex = PlayerPrefs.GetInt("AvatarIndex", 0);
        avatarImage.sprite = avatarSprites[selectedIndex];
    }

    public void ShiftSelectedIndex(int shift)
    {
        selectedIndex += shift;

        while (selectedIndex >= avatarSprites.Length)
            selectedIndex -= avatarSprites.Length;

        while (selectedIndex < 0)

            selectedIndex += avatarSprites.Length;

        avatarImage.sprite = avatarSprites[selectedIndex];

        PlayerPrefs.SetInt("AvatarIndex", selectedIndex);
        var property = new Hashtable();
        property.Add("AvatarIndex", selectedIndex);
        PhotonNetwork.LocalPlayer.SetCustomProperties(property);
    } 
}