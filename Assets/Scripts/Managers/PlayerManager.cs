using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static event Action OnPlayerRespawn;
    public static event Action OnPlayerDeath;

    private LevelSplitScreenSetup splitScreenSetup;

    public PlayerInputManager playerInputManager { get; private set; }
    public static PlayerManager instance;

    public List<GameObject> objectsToDisable;

    public int lifePoints;
    public int maxPlayerCount = 1;
    public int playerCountWinCondition;

    [Header("Player")]
    [SerializeField] private List<Player> playerList = new List<Player>();
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private string[] playerDevice;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        playerInputManager = GetComponent<PlayerInputManager>();
    }

    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += AddPlayer;
        playerInputManager.onPlayerLeft += RemovePlayer;
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= AddPlayer;
        playerInputManager.onPlayerLeft -= RemovePlayer;
    }

    public void SetupMaxPlayerCount(int newPlayerCount)
    {
        maxPlayerCount = newPlayerCount;
        playerInputManager.SetMaximumPlayerCount(maxPlayerCount);
    }

    public void EnableJoinAndUpdateLifePoints()
    {
        splitScreenSetup = FindAnyObjectByType<LevelSplitScreenSetup>();

        playerInputManager.EnableJoining();
        playerCountWinCondition = maxPlayerCount;
        lifePoints = maxPlayerCount;
        UI_InGame.instance.UpdateLifePointsUI(lifePoints, maxPlayerCount);
    }

    private void AddPlayer(PlayerInput newPlayer)
    {
        Player playerScript = newPlayer.GetComponent<Player>();

        playerList.Add(playerScript);

        OnPlayerRespawn?.Invoke();
        PlaceNewPlayerAtRepawnPoint(newPlayer.transform);

        int newPlayerNumber = GetPlayerNumber(newPlayer);
        int newPlayerSkinId = SkinManager.instance.GetSkinId(newPlayerNumber);

        playerScript.UpdateSkin(newPlayerSkinId);

        foreach (GameObject gameObject in objectsToDisable)
        {
            if (gameObject != null)
                gameObject.SetActive(false);
        }

        if (playerInputManager.splitScreen == true)
        {
            newPlayer.camera = splitScreenSetup.mainCamera[newPlayerNumber];
            splitScreenSetup.cinemachineCamera[newPlayerNumber].Follow = newPlayer.transform;
        }
    }

    private void RemovePlayer(PlayerInput player)
    {
        Player playerScript = player.GetComponent<Player>();
        playerList.Remove(playerScript);

        if (CanRemoveLifePoints() && lifePoints > 0)
            lifePoints--;

        if (lifePoints <= 0)
        {
            playerCountWinCondition--;
            playerInputManager.DisableJoining();

            if (playerList.Count <= 0)
                GameManager.instance.RestartLevel();
        }

        UI_InGame.instance.UpdateLifePointsUI(lifePoints, maxPlayerCount);

        OnPlayerDeath?.Invoke();
    }

    private bool CanRemoveLifePoints()
    {
        if (DifficultyManager.instance.difficulty == DifficultyType.Hard)
            return true;

        if (GameManager.instance.fruitsCollected <= 0 && DifficultyManager.instance.difficulty == DifficultyType.Normal)
            return true;

        return false;
    }

    private int GetPlayerNumber(PlayerInput newPlayer)
    {
        int newPlayerNumber = 0;

        foreach (var device in newPlayer.devices)
        {
            for (int i = 0; i < playerDevice.Length; i++)
            {
                if (playerDevice[i] == "Empty")
                {
                    newPlayerNumber = i;
                    playerDevice[i] = device.name;
                    break;
                }
                else if (playerDevice[i] == device.name)
                {
                    newPlayerNumber = i;
                    break;
                }
            }
        }

        return newPlayerNumber;
    }

    public List<Player> GetPlayerList() => playerList;

    public void UpdateRespawnPosition(Transform newRespawnPoint) => respawnPoint = newRespawnPoint;

    private void PlaceNewPlayerAtRepawnPoint(Transform newPlayer)
    {
        if (respawnPoint == null)
            respawnPoint = FindAnyObjectByType<StartPoint>().transform;

        newPlayer.position = respawnPoint.position;
    }
}
