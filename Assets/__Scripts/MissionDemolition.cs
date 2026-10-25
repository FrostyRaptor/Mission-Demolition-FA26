using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameMode
{
  idle,
  playing,
  levelEnd
}

public class MissionDemolition : MonoBehaviour
{
  static private MissionDemolition S;
  static public MissionDemolition Instance { get; private set; }

  [Header("Inscribed")]
  public Text uitLevel;
  public Text uitShots;
  public Vector3 castlePos;
  public GameObject[] castles;

  [Header("Dynamic")]
  public int level;
  public int levelMax;
  public int shotsTaken;
  public GameObject castle;
  public Goal activeGoal;
  public GameMode mode = GameMode.idle;
  public string showing = "Show Slingshot";

  void Start()
  {
    S = this;
    Instance = this;

    level = 0;
    shotsTaken = 0;
    levelMax = castles.Length;

    StartLevel();
  }

  void StartLevel()
  {
    if (castle != null)
    {
      activeGoal.ResetConfetti();
      Destroy(castle);
    }

    Projectile.DESTROY_PROJECTILES();

    castle = Instantiate<GameObject>(castles[level]);
    castle.transform.position = castlePos;

    Goal.goalMet = false;

    UpdateGUI();

    mode = GameMode.playing;

    FollowCam.SWITCH_VIEW(FollowCam.eView.both);
  }

  void UpdateGUI()
  {
    uitLevel.text = "Level: " + (level + 1) + " of " + levelMax;
    uitShots.text = "Shot Taken: " + shotsTaken;
  }

  void Update()
  {
    UpdateGUI();

    if ((mode == GameMode.playing) && Goal.goalMet)
    {
      mode = GameMode.levelEnd;

      if (activeGoal != null)
      {
        activeGoal.TriggerConfetti();
      }

      FollowCam.SWITCH_VIEW(FollowCam.eView.both);

      Invoke("NextLevel", 3f);
    }
  }

  void NextLevel()
  {
    level++;
    if (level == levelMax)
    {
      level = 0;
      shotsTaken = 0;

      SceneManager.LoadScene("_Game_Over");
    }

    StartLevel();
  }

  static public void SHOT_FIRED()
  {
    S.shotsTaken++;
  }

  static public GameObject GET_CASTLE()
  {
    return S.castle;
  }

  public void RegisterCurrentGoal(Goal newGoal)
  {
    activeGoal = newGoal;
  }
}
