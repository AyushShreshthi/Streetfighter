using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    WaitForSeconds oneSec;
    public Transform[] spawnPositions;

    CameraManager camM;
    public CharacterManager charM;
    LevelUI levelUI;

    public int maxTurns = 2;
    int currentTurn = 1;

    public bool countdown;
    public int maxTurnTimer = 30;
    int currentTimer;
    float internalTImer;
    public AudioSource ad;
    public AudioClip clip;

    private void Start()
    {
        
        charM = CharacterManager.GetInstance();
        levelUI = LevelUI.GetInstance();
        camM = CameraManager.GetInstance();

        oneSec = new WaitForSeconds(1);

        levelUI.AnnouncerTextLine1.gameObject.SetActive(false);
        levelUI.AnnouncerTextLine2.gameObject.SetActive(false);

        StartCoroutine("StartGame");
    }

    IEnumerator StartGame()
    {
        yield return CreatePlayers();

        yield return InitTurn();
    }
    IEnumerator CreatePlayers()
    {
        for(int i = 0; i < charM.players.Count; i++)
        {
            GameObject go = Instantiate(charM.players[i].playerPrefab,
                spawnPositions[i].position, Quaternion.identity)
                as GameObject;

            charM.players[i].playerStates = go.GetComponent<StateManager>();

            charM.players[i].playerStates.healthSlider = levelUI.healthSliders[i];

            camM.players.Add(go.transform);
        }

        yield return null;
    }
    IEnumerator InitTurn()
    {
        levelUI.AnnouncerTextLine1.gameObject.SetActive(false);
        levelUI.AnnouncerTextLine2.gameObject.SetActive(false);

        currentTimer = maxTurnTimer;
        countdown = false;

        yield return InitPlayers();

        yield return EnableControl();
    }
    IEnumerator InitPlayers()
    {
        for(int i = 0; i < charM.players.Count; i++)
        {
            charM.players[i].playerStates.health = 100;
           // charM.players[i].playerStates.handleAnim.anim.Play("Locomotion");
            charM.players[i].playerStates.transform.position = spawnPositions[i].position;

        }
        yield return null;
    }
    IEnumerator EnableControl()
    {
        for(int i = 0; i < charM.players.Count; i++)
        {

            charM.players[i].playerStates.gameObject.GetComponentInChildren<Animator>().SetBool("DontMove", false);
            charM.players[i].playerStates.gameObject.GetComponentInChildren<Animator>().Play("Locomotion");
        }
        levelUI.AnnouncerTextLine1.gameObject.SetActive(true);
        levelUI.AnnouncerTextLine1.text = "Turn " + currentTurn;
        levelUI.AnnouncerTextLine1.color = Color.white;
        yield return oneSec;
        yield return oneSec;
        ad.clip = clip;
        ad.loop = false;
        ad.Play();
        levelUI.AnnouncerTextLine1.text = "3";
        levelUI.AnnouncerTextLine1.color = Color.green;
        yield return oneSec;
        levelUI.AnnouncerTextLine1.text = "2";
        levelUI.AnnouncerTextLine1.color = Color.yellow;
        yield return oneSec;
        levelUI.AnnouncerTextLine1.text = "1";
        levelUI.AnnouncerTextLine1.color = Color.red;
        yield return oneSec;
        levelUI.AnnouncerTextLine1.color = Color.red;
        levelUI.AnnouncerTextLine1.text = "FIGHT";

        for(int i = 0; i < charM.players.Count; i++)
        {
            if (charM.players[i].playerType == PlayerBase.PlayerType.user)
            {
                InputHandler ih = charM.players[i].playerStates.gameObject.GetComponent<InputHandler>();

                ih.playerInput = charM.players[i].inputId;
                ih.enabled = true;
            }
            if (charM.players[i].playerType == PlayerBase.PlayerType.ai)
            {
                AIcharacter ai = charM.players[i].playerStates.gameObject.GetComponent<AIcharacter>();
                ai.enabled = true;


                ai.enStates = charM.returnOppositePlayer(charM.players[i]).playerStates;
            }
        }
        
        yield return oneSec;
        levelUI.AnnouncerTextLine1.gameObject.SetActive(false);
        countdown = true;
    }

    void DisableControl()
    {
        for(int i = 0; i < charM.players.Count; i++)
        {
            charM.players[i].playerStates.ResetStateInputs();

            if (charM.players[i].playerType == PlayerBase.PlayerType.user)
            {
                charM.players[i].playerStates.GetComponent<InputHandler>().enabled = false;
            }
            else
            {
                charM.players[i].playerStates.GetComponent<AIcharacter>().enabled = false;
            }

        }
    }
    private void FixedUpdate()
    {
        if(charM.players[0].playerStates.transform.position.x<
            charM.players[1].playerStates.transform.position.x)
        {
            charM.players[0].playerStates.lookRight = true;
            charM.players[1].playerStates.lookRight = false;
        }
        else
        {
            charM.players[0].playerStates.lookRight = false;
            charM.players[1].playerStates.lookRight = true;

        }
    }
    private void Update()
    {
        if (countdown)
        {
            HandleTurnTimer();
        }
        
    }

    void HandleTurnTimer()
    {
        levelUI.LevelTimer.text = currentTimer.ToString();

        internalTImer += Time.deltaTime;

        if (internalTImer > 1)
        {
            currentTimer--;
            internalTImer = 0;
        }
        if (currentTimer <= 0)
        {
            EndTurnFunction(true);
            countdown = false;
        }
    }

    public void EndTurnFunction(bool timeOut=false)
    {
        countdown = false;

        levelUI.LevelTimer.text = maxTurnTimer.ToString();

        if (timeOut)
        {
            levelUI.AnnouncerTextLine1.gameObject.SetActive(true);
            levelUI.AnnouncerTextLine1.text = "TIME OUT !!! ";
            levelUI.AnnouncerTextLine1.color = Color.cyan;
        }
        else
        {
            levelUI.AnnouncerTextLine1.gameObject.SetActive(true);
            levelUI.AnnouncerTextLine1.text = "K.O.";
            levelUI.AnnouncerTextLine1.color = Color.red;

        }

        DisableControl();

        StartCoroutine("EndTurn");
    }

    IEnumerator EndTurn()
    {
        yield return oneSec;
        yield return oneSec;
        yield return oneSec;

        PlayerBase vPlayer = FindWinningPlayer();

        if (vPlayer == null)
        {
            levelUI.AnnouncerTextLine1.text = "DRAW";
            levelUI.AnnouncerTextLine1.color = Color.blue;

        }
        else
        {
            levelUI.AnnouncerTextLine1.text = vPlayer.playerId + " Wins !!! ";
            levelUI.AnnouncerTextLine1.color = Color.red;
        }
        yield return oneSec;
        yield return oneSec;
        yield return oneSec;

        if (vPlayer != null)
        {
            if (vPlayer.playerStates.health == 100)
            {
                levelUI.AnnouncerTextLine2.gameObject.SetActive(true);
                levelUI.AnnouncerTextLine2.text="FLAWLESS VICTORY";

            }
        }
        yield return oneSec;
        yield return oneSec;
        yield return oneSec;

        bool matchOver = IsMatchOver();

        if (!matchOver)
        {
            StartCoroutine("InitTurn");
        }
        else
        {
            for(int i = 0; i < charM.players.Count; i++)
            {
                charM.players[i].score = 0;
                charM.players[i].hasCharacter = false;
            }
            //SceneManager.LoadSceneAsync("select");
            if (charM.solo)
            {
                if(vPlayer==charM.players[0])
                    MySceneManager.GetInstance().LoadNextOnProgresssion();
                else
                    MySceneManager.GetInstance().RequestLevelLoad(SceneType.main, "game_over");

            }
            else
            {
                MySceneManager.GetInstance().RequestLevelLoad(SceneType.main, "select");
            }
        }
    }

    bool IsMatchOver()
    {
        bool retVal = false;

        for(int i = 0; i < charM.players.Count; i++)
        {
            if (charM.players[i].score >= maxTurns)
            {
                retVal = true;
                break;
            }
        }

        return retVal;
    }
    private PlayerBase FindWinningPlayer()
    {
        PlayerBase retVal = null;

        StateManager targetPlayer = null;

        if (charM.players[0].playerStates.health != charM.players[1].playerStates.health)
        {
            if (charM.players[0].playerStates.health < charM.players[1].playerStates.health)
            {
                charM.players[1].score++;
                targetPlayer = charM.players[1].playerStates;
                levelUI.AddWinIndicator(1);
            }
            else
            {
                charM.players[0].score++;
                targetPlayer = charM.players[0].playerStates;
                levelUI.AddWinIndicator(0);
            }

            retVal = charM.returnPlayerFormStates(targetPlayer);
        }

        return retVal;
    }

    public static LevelManager instance;
    public static LevelManager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }


}
