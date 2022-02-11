using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroSceneManager : MonoBehaviour
{
    public GameObject startText;
    float timer;
    bool loadingLevel;
    bool init;

    public int activeElement;
    public GameObject menuObj;
    public ButtonRef[] menuOptions;

    public GameObject controlPanel;
    public AudioSource ad;
    public AudioClip click;
    public AudioClip updown;
    private void Start()
    {
        menuObj.SetActive(false);
        StartCoroutine(DisableControls());
    }
    IEnumerator DisableControls()
    {
        yield return new WaitForSeconds(5f);

        controlPanel.SetActive(false);
    }
    private void Update()
    {
        if (!init)
        {
            timer += Time.deltaTime;
            if (timer > 0.6f)
            {
                timer = 0;
                startText.SetActive(!startText.activeInHierarchy);
            }

            if (Input.GetKeyUp(KeyCode.Space) )
            {
                init = true;
                ad.clip = click;
                ad.PlayOneShot(ad.clip);
                startText.SetActive(false);
                menuObj.SetActive(true);
            }
        }
        else
        {
            if (!loadingLevel)
            {
                menuOptions[activeElement].selected = true;

                if (Input.GetKeyUp(KeyCode.UpArrow))
                {
                    menuOptions[activeElement].selected = false;
                    if (activeElement > 0)
                    {
                        activeElement--;
                        ad.clip = updown;
                        ad.PlayOneShot(ad.clip);
                    }
                    else
                    {
                        activeElement = menuOptions.Length - 1;
                    }
                }


                if (Input.GetKeyUp(KeyCode.DownArrow))
                {
                    menuOptions[activeElement].selected = false;

                    if (activeElement < menuOptions.Length - 1)
                    {
                        activeElement++;
                        ad.clip = updown;
                        ad.PlayOneShot(ad.clip);
                    }
                    else
                    {
                        activeElement = 0;
                    }
                }

                if (Input.GetKeyUp(KeyCode.Space))
                {
                   // Debug.Log("load");
                    loadingLevel = true;
                    ad.clip = click;
                    ad.PlayOneShot(ad.clip);
                    StartCoroutine("LoadLevel");
                    menuOptions[activeElement].transform.localScale *= 1.2f;
                }
            }
        }
    }
    void HandleSelectedOption()
    {
        switch (activeElement)
        {
            case 0:
                CharacterManager.GetInstance().numberOfUsers = 1;
                break;
            case 1:
                CharacterManager.GetInstance().numberOfUsers = 2;
                CharacterManager.GetInstance().players[1].playerType = PlayerBase.PlayerType.user;
                break;
        }
    }

    IEnumerator LoadLevel()
    {
        HandleSelectedOption();
        yield return new WaitForSeconds(0.6f);

        MySceneManager.GetInstance().RequestLevelLoad(SceneType.main, "select");
        //SceneManager.LoadSceneAsync("select", LoadSceneMode.Single);
    }
}

