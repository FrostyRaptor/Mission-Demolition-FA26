using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
  [Header("Inscribed")]
  public GameObject projectilePrefab;
  public float velocityMult = 10f;
  public GameObject projLinePrefab;
  public Material matBand;
  public float bandWidth = 0.15f;
  public Color bandColor = new Color(0f, 0f, 0f);

  [Header("Dynamic")]
  public GameObject launchPoint;
  public Vector3 launchPos;
  public GameObject projectile;
  public bool aimingMode;

  private Transform leftArm;
  private Transform rightArm;
  private LineRenderer leftLine;
  private LineRenderer rightLine;

  void Awake()
  {
    Transform launchPointTrans = transform.Find("LaunchPoint");
    launchPoint = launchPointTrans.gameObject;
    launchPoint.SetActive(false);
    launchPos = launchPointTrans.position;

    leftArm = transform.Find("LeftArm");
    rightArm = transform.Find("RightArm");

    leftLine = MakeBandSegment("LeftLine", matBand);
    rightLine = MakeBandSegment("RightLine", matBand);
    UpdateBand(launchPos);
  }

  LineRenderer MakeBandSegment(string name, Material mat)
  {
    GameObject go = new GameObject(name);
    go.transform.SetParent(transform, false);

    LineRenderer lr = go.AddComponent<LineRenderer>();
    lr.material = mat;
    lr.startWidth = lr.endWidth = bandWidth;

    lr.numCapVertices = 4;
    lr.useWorldSpace = true;
    lr.positionCount = 2;
    return lr;
  }

  Vector3 ArmTip(Transform arm)
  {
    return arm.position + arm.up * arm.lossyScale.y;
  }

  void UpdateBand(Vector3 projPos)
  {
    leftLine.SetPosition(0, ArmTip(leftArm));
    leftLine.SetPosition(1, projPos);

    rightLine.SetPosition(0, ArmTip(rightArm));
    rightLine.SetPosition(1, projPos);
  }

  void Update()
  {
    if (!aimingMode) return;

    Vector3 mousePos2D = Input.mousePosition;
    mousePos2D.z = -Camera.main.transform.position.z;
    Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

    Vector3 mouseDelta = mousePos3D - launchPos;

    float maxMagnitude = this.GetComponent<SphereCollider>().radius;
    if (mouseDelta.magnitude > maxMagnitude)
    {
      mouseDelta.Normalize();
      mouseDelta *= maxMagnitude;
    }

    Vector3 projPos = launchPos + mouseDelta;
    projectile.transform.position = projPos;
    UpdateBand(projPos);

    if (Input.GetMouseButtonUp(0))
    {
      aimingMode = false;
      Rigidbody projRB = projectile.GetComponent<Rigidbody>();
      projRB.isKinematic = false;
      projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
      projRB.velocity = -mouseDelta * velocityMult;

      FollowCam.SWITCH_VIEW(FollowCam.eView.slingshot);

      FollowCam.POI = projectile;
      Instantiate<GameObject>(projLinePrefab, projectile.transform);
      projectile = null;
      UpdateBand(launchPos);
      MissionDemolition.SHOT_FIRED();
    }
  }

  void OnMouseEnter()
  {
    //print("Slingshot:OnMouseEnter()");
    launchPoint.SetActive(true);
  }

  void OnMouseExit()
  {
    //print("Slingshot:OnMouseExit()");
    launchPoint.SetActive(false);
  }

  void OnMouseDown()
  {
    aimingMode = true;
    projectile = Instantiate(projectilePrefab) as GameObject;
    projectile.transform.position = launchPos;
    projectile.GetComponent<Rigidbody>().isKinematic = true;
  }
}
