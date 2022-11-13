  using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField newRoomInputField; 
    [SerializeField] TMP_Text feedbackText;
    [SerializeField] Button StartGameButton;
    [SerializeField] GameObject roomPanel;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] GameObject RoomListObject;
    [SerializeField] GameObject playerListObject; //
    [SerializeField] RoomItem roomItemPrefab;
    [SerializeField] PlayerItem playerItemPrefab; //
    List<RoomItem> roomItemList = new List<RoomItem>();
    List<PlayerItem> playerItemList = new List<PlayerItem>(); //
    Dictionary<string, RoomInfo> roomInfoCache = new Dictionary<string, RoomInfo>();

    private void Start()
    {
        PhotonNetwork.JoinLobby();
        roomPanel.SetActive(false);
    }

    //membuat room baru denga max jumlah player 2
    public void clickCreateRoom()
    {
        feedbackText.text = "";
        if (newRoomInputField.text.Length < 3)
        {
            feedbackText.text = "Username min 3 characters";
            return;
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 5;
        PhotonNetwork.CreateRoom(newRoomInputField.text, roomOptions);
    }

    public void ClickStartGame(string levelName)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(levelName);
        }
    }

    //join room
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Create room: " + PhotonNetwork.CurrentRoom);
        feedbackText.text = "Create room: " + PhotonNetwork.CurrentRoom.Name;
    }

    //create dan join room
    public override void OnJoinedRoom()
    {
        Debug.Log("Join room: " + PhotonNetwork.CurrentRoom);
        feedbackText.text = "Create room: " + PhotonNetwork.CurrentRoom.Name;
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        roomPanel.SetActive(true);

        //update player list
        UpdatePlayerList();

        //atur start game button
        SetStartGameButton();
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        //atur start game button
        SetStartGameButton();
    }

    private void SetStartGameButton()
    {
        //tampilkan hanya di master client
        StartGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);

        // bisa diklik/interectable hanya ketika jumlah player >=2
        StartGameButton.interactable = PhotonNetwork.CurrentRoom.PlayerCount >= 2; 
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        //update player list
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        //update player list
        UpdatePlayerList();
    }

    private void UpdatePlayerList()
    {
        //destroy semua player item yang sudah ada
        foreach (var item in playerItemList)
        {
            Destroy(item.gameObject);
        }

        playerItemList.Clear();

        // PhotonNetwork.PlayerList
        // foreach (Photon.Realtime.Player in PhotonNetwork.PlayerList)
        //bikin ulang player list
        foreach (var (id, player) in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newPlayerItem = Instantiate(playerItemPrefab, playerListObject.transform);
            newPlayerItem.Set(player);
            playerItemList.Add(newPlayerItem);

            if (player == PhotonNetwork.LocalPlayer)
            {
                newPlayerItem.transform.SetAsFirstSibling(); 
            }
        }

        //start game hanya bisa diklik ketika jumlah pemaintertentu
        SetStartGameButton();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log(returnCode + ", " + message);
        feedbackText.text = returnCode.ToString() + ": " + message;
    }

    // update daftar room
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {

        foreach (var roomInfo in roomList)
        {
            roomInfoCache[roomInfo.Name] = roomInfo;
        }
        
        foreach (var item in this.roomItemList)
        {
            Destroy(item.gameObject);
        }

        this.roomItemList.Clear();
        
        var roomInfoList = new List<RoomInfo>(roomInfoCache.Count);

        //Short. yang open di add duluan  
        foreach (var roomInfo in roomInfoCache.Values)
        {
            if (roomInfo.IsOpen)
            {
                roomInfoList.Add(roomInfo);
            }
        }

        //kemudian yang close
        foreach (var roomInfo in roomInfoCache.Values)
        {
            if (roomInfo.IsOpen == false)
            {
                roomInfoList.Add(roomInfo);
            }
        }

        foreach (var roomInfo in roomInfoList)
        {
            if (roomInfo.IsVisible == false || roomInfo.MaxPlayers == 0)
            {
                continue;    
            }
            RoomItem newRoomItem = Instantiate(roomItemPrefab, RoomListObject.transform);
            newRoomItem.Set(this, roomInfo);
            this.roomItemList.Add(newRoomItem);
        }
    }
}