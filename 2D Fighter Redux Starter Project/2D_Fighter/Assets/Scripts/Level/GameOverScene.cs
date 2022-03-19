using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameOverScene : MonoBehaviour
{
    public float timer = 3;
    private void Start()
    {
        PlayerBase pb = new PlayerBase();
        pb.score = LevelManager.instance.charM.players[0].score;
        string json = JsonUtility.ToJson(pb);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);

        StartCoroutine(LoadScene());
    }
    IEnumerator LoadScene()
    {
        

        yield return new WaitForSeconds(timer);

        MySceneManager.GetInstance().RequestLevelLoad(SceneType.main, "intro");
    }
}
