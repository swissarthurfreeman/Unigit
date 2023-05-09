using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public GameObject GameOverText;

    public Text HighScoreText;
    public int highScore;
    public static MainManager Instance;
    
    private bool m_Started = false;
    private int m_Points;
    
    private bool m_GameOver = false;

    
    void Awake() {
        if(Instance == null) {
            Instance = this;
        }
        LoadHighScore();
    }

    [System.Serializable]
    class Score {
        public int score;
    }

    void LoadHighScore() {
        string path = Application.persistentDataPath + "/highScoreUnigit.json";
        if (File.Exists(path)) {
            Debug.Log("Loading High Score...");
            string json = File.ReadAllText(path);
            Score s = JsonUtility.FromJson<Score>(json);
            highScore = s.score;
            HighScoreText.text = "High Score : " + highScore.ToString();
        } else {
            highScore = 0;
        }
    }

    void SaveHighScore(int newHighScore) {
        if (highScore < newHighScore) {
            Debug.Log("Saving High Score...");
            string path = Application.persistentDataPath + "/highScoreUnigit.json";
            Debug.Log(path);
            Score s = new Score();
            s.score = newHighScore;
            string json = JsonUtility.ToJson(s);
            File.WriteAllText(path, json);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point) {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
        SaveHighScore(m_Points);
    }
}
