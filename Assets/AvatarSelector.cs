using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class AvatarSelector : MonoBehaviour
{
    [SerializeField] Image avatarImage;
    [SerializeField] Sprite[] avatarSprites;
    private int selectedIndex;

    private void Start()
    {
        selectedIndex = PlayerPrefs.GetInt("AvatarIndex", 0);
        avatarImage.sprite = avatarSprites[selectedIndex];
        SaveSelectedIndex();
    }

    // shifting index milih ke kiri atau kanan dari Sprite[]
    public void shiftSelectedIndex(int shift)
    {
        selectedIndex += shift;

        while (selectedIndex >= avatarSprites.Length)
        {
            selectedIndex -= avatarSprites.Length;
        }

        while (selectedIndex < 0)
        {
            selectedIndex += avatarSprites.Length;
        }

        avatarImage.sprite = avatarSprites[selectedIndex];

        SaveSelectedIndex();

    }

    private void SaveSelectedIndex()
    {
                //simpan di local storage
        PlayerPrefs.SetInt("AvatarIndex", selectedIndex);

        // simpan di network
        var property = new Hashtable();
        property.Add("AvatarIndex", selectedIndex);
        PhotonNetwork.LocalPlayer.SetCustomProperties(property);
    }
}