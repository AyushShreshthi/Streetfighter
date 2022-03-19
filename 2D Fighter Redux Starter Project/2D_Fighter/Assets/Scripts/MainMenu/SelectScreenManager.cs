using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectScreenManager : MonoBehaviour
{
    public int numberOfPlayers = 1;
    public List<PlayerInterfaces> p1Interface = new List<PlayerInterfaces>();
    //public PotraitInfo[] potraitPrefabs;
    public int maxX;
    public int maxY;
    PotraitInfo[,] charGrid;

    int maxRow;
    int maxColumn;
    List<PotraitInfo> potraitList = new List<PotraitInfo>();

    public GameObject potraitCanvas;

    bool loadLevel;
    public bool bothPlayerSelected;

    CharacterManager charManager;

    GameObject potraitPrefab;
    public Text player2;
    public AudioSource ad;
    public AudioClip updown;
    public Text highscoreTxt;
    #region Singleton

    public static SelectScreenManager instance;
    public static SelectScreenManager GetInstance()
    {
        return instance;
    }
    private void Awake()
    {
        instance = this;
    }
    #endregion

    private void Start()
    {
        charManager = CharacterManager.GetInstance();
        numberOfPlayers = charManager.numberOfUsers;

        potraitPrefab = Resources.Load("PotraitPrefab") as GameObject;
        CreatePotraits();

        charManager.solo = (numberOfPlayers == 1);
        if (charManager.solo)
            player2.gameObject.SetActive(false);

        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerBase data = JsonUtility.FromJson<PlayerBase>(json);

            highscoreTxt.text = data.score.ToString();
        }
    }
    void CreatePotraits()
    {
        GridLayoutGroup group = potraitCanvas.GetComponent<GridLayoutGroup>();

        maxRow = group.constraintCount;

        int x = 0;
        int y = 0;

        for(int i = 0; i < charManager.characterList.Count; i++)
        {
            CharacterBase c = charManager.characterList[i];

            GameObject go = Instantiate(potraitPrefab) as GameObject;
            go.transform.SetParent(potraitCanvas.transform);

            PotraitInfo p = go.GetComponent<PotraitInfo>();
            p.img.sprite = c.icon;
            p.characterId = c.charId;
            p.posX = x;
            p.posY = y;
            potraitList.Add(p);

            if (x < maxRow - 1)
            {
                x++;
            }
            else
            {
                x = 0;
                y++;
            }
            maxColumn = y;
        }
    }
    private void Update()
    {
        if (!loadLevel)
        {
            for(int i = 0; i < p1Interface.Count; i++)
            {
                if (i < numberOfPlayers)
                {

                    if (Input.GetButtonUp("Fire2" + charManager.players[i].inputId))
                    {

                        p1Interface[i].playerBase.hasCharacter = false;

                    }

                    if (!charManager.players[i].hasCharacter)
                    {
                        p1Interface[i].playerBase = charManager.players[i];

                        HandleSelectorPosition(p1Interface[i]);
                        HandleSelectScreenInput(p1Interface[i],charManager.players[i].inputId);
                        HandleCharacterPreview(p1Interface[i]);
                    }
                }
                else
                {
                    charManager.players[i].hasCharacter = true;
                }
            }
        }
        if (bothPlayerSelected)
        {
            //Debug.Log("loading");
            StartCoroutine("LoadLevel");
            loadLevel = true;

        }
        else
        {
            if(charManager.players[0].hasCharacter
                && charManager.players[1].hasCharacter)
            {
                bothPlayerSelected = true;
            }
        }
    }

    IEnumerator LoadLevel()
    {
        for(int i = 0; i < charManager.players.Count; i++)
        {
            if (charManager.players[i].playerType == PlayerBase.PlayerType.ai)
            {
                if (charManager.players[i].playerPrefab == null)
                {
                    int ranValue = UnityEngine.Random.Range(0, potraitList.Count);

                    charManager.players[i].playerPrefab =
                        charManager.returnCharacterWithID(potraitList[ranValue].characterId).prefab;

                }
            }
        }

        yield return new WaitForSeconds(2);

        if (charManager.solo)
        {
            MySceneManager.GetInstance().CreateProgression();
            MySceneManager.GetInstance().LoadNextOnProgresssion();
            //SceneManager.LoadScene(2);
        }
        else
        {
            MySceneManager.GetInstance().RequestLevelLoad(SceneType.prog, "level_1");
        }
        //SceneManager.LoadSceneAsync("level", LoadSceneMode.Single);

    }
    private void HandleCharacterPreview(PlayerInterfaces pl)
    {
        if (pl.previewPotrait != pl.activePotrait)
        {
            if (pl.createdCharacter != null)
            {
                Destroy(pl.createdCharacter);
            }

            GameObject go = Instantiate(
                CharacterManager.GetInstance().returnCharacterWithID(pl.activePotrait.characterId).prefab,
                pl.charVisPos.position,
                Quaternion.identity) as GameObject;

            pl.createdCharacter = go;

            pl.previewPotrait = pl.activePotrait;

            if (!string.Equals(pl.playerBase.playerId, charManager.players[0].playerId))
            {
                pl.createdCharacter.GetComponent<StateManager>().lookRight = false;
            }
        }
    }

    private void HandleSelectScreenInput(PlayerInterfaces pl, string playerId)
    {
        #region Grid Navigation

        float vertical = Input.GetAxis("Vertical" + playerId);

        if (vertical != 0)
        {
            if (!pl.hitInputOnce)
            {
                if (vertical > 0)
                {
                    pl.activeY = (pl.activeY > 0) ? pl.activeY - 1 : maxColumn;

                }
                else
                {
                    pl.activeY = (pl.activeY < maxColumn) ? pl.activeY + 1 : 0;

                }

                ad.clip = updown;
                ad.PlayOneShot(ad.clip);
                pl.hitInputOnce = true;
            }
        }

        float horizontal = Input.GetAxis("Horizontal" + playerId);

        if (horizontal != 0)
        {
            if (!pl.hitInputOnce)
            {
                if (horizontal > 0)
                {
                    pl.activeX = (pl.activeX > 0) ? pl.activeX - 1 : maxRow-1;

                }
                else
                {
                    pl.activeX = (pl.activeX < maxRow-1) ? pl.activeX + 1 : 0;

                }
                pl.timerToReset = 0;
                ad.clip = updown;
                ad.PlayOneShot(ad.clip);
                pl.hitInputOnce = true;
            }
        }
        if (vertical == 0 && horizontal == 0)
        {
            pl.hitInputOnce = false;
        }
        if (pl.hitInputOnce)
        {
            pl.timerToReset += Time.deltaTime;
            if (pl.timerToReset > 0.8f)
            {
                pl.hitInputOnce = false;
                pl.timerToReset = 0;
            }
        }
        #endregion

        if (Input.GetButtonUp("Fire1" + playerId))
        {
            pl.createdCharacter.GetComponentInChildren<Animator>().Play("Kick");
            
            pl.playerBase.playerPrefab =
                charManager.returnCharacterWithID(pl.activePotrait.characterId).prefab;

            pl.playerBase.hasCharacter = true;

            AudioManager.GetInstance().PlaySound("kick");
        }

    }

    private void HandleSelectorPosition(PlayerInterfaces pl)
    {
        pl.selector.SetActive(true);

        PotraitInfo pi = ReturnPotrait(pl.activeX, pl.activeY);

        if (pi != null)
        {
            pl.activePotrait = pi;

            Vector2 selectorPosition = pl.activePotrait.transform.localPosition;

            selectorPosition = selectorPosition + new Vector2(potraitCanvas.transform.localPosition.x,
                potraitCanvas.transform.localPosition.y);


            //pl.activePotrait = charGrid[pl.activeX, pl.activeY];

            pl.selector.transform.localPosition = selectorPosition;

        }

    }
    PotraitInfo ReturnPotrait(int x, int y)
    {
        PotraitInfo r = null;

        for(int i = 0; i < potraitList.Count; i++)
        {
            if (potraitList[i].posX == x && potraitList[i].posY == y)
            {
                r = potraitList[i];
            }
        }

        return r;
    }
}
[System.Serializable]
public class PlayerInterfaces
{
    public PotraitInfo activePotrait;
    public PotraitInfo previewPotrait;
    public GameObject selector;
    public Transform charVisPos;
    public GameObject createdCharacter;

    public int activeX;
    public int activeY;

    public bool hitInputOnce;
    public float timerToReset;

    public PlayerBase playerBase;

}
