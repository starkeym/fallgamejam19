using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class FieldOfView : MonoBehaviour {

	public float viewRadius;
	[Range(0,360)]
	public float viewAngle;

	public LayerMask targetMask;
	public LayerMask obstacleMask;

	[HideInInspector]
	public List<Transform> visibleTargets = new List<Transform>();

	public float meshResolution;
	public int edgeResolveIterations;
	public float edgeDstThreshold;

	public float maskCutawayDst = .1f;

	public MeshFilter viewMeshFilter;
	Mesh viewMesh;

    //atakan eklentileri
    NavMeshAgent enemy;

    GameObject player;

    Vector3 direction1;

    Vector3 direction2;

    public GameObject RobotSignal;

    

    int isDetected = 0;

    bool isInside = false;

    bool PlayerEnteredTheAngle;

    public static bool canbeKilled = false;

    public bool aRobotDied = false;

    bool isStandingEnemy;

    Vector3 enemypos;

    float targetTime = 1.8f;

    public float enemyRadius;

    public float escapeTime = 3f;

    int SurroundingMembers = 0;

    Vector3 destination;



   

   







    void Start() {
		viewMesh = new Mesh ();
		viewMesh.name = "View Mesh";
		viewMeshFilter.mesh = viewMesh;
        StartCoroutine("FindTargetsWithDelay", .2f);

        enemy = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");

        foreach (Transform item in gameObject.transform)
        {
            if (item.gameObject.name=="Direction1")
            {
                direction1 = item.position;
                Destroy(item.gameObject);
            }
            if (item.gameObject.name=="Direction2")
            {
                direction2 = item.position;
                Destroy(item.gameObject);
            }
        }
        destination = direction1;
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }


    void LateUpdate() {
        if(this.gameObject.tag == "Standingenemy" || this.gameObject.tag =="Standingrobot" ) { isStandingEnemy = true; }   
        else { isStandingEnemy = false; }
		DrawFieldOfView ();
        
        FindVisibleTargets();
        EnemyBehaviour();
        Patrolafterdetection();
        

	}

	void FindVisibleTargets() {
        Debug.Log(targetTime);
		visibleTargets.Clear ();
		Collider[] targetsInViewRadius = Physics.OverlapSphere (transform.position, viewRadius, targetMask);
        
		for (int i = 0; i < targetsInViewRadius.Length; i++) {
			Transform target = targetsInViewRadius [i].transform;
			Vector3 dirToTarget = (target.position - transform.position).normalized;
			if (Vector3.Angle (transform.forward, dirToTarget) < viewAngle / 2) {
				float dstToTarget = Vector3.Distance (transform.position, target.position);
				if (!Physics.Raycast (transform.position, dirToTarget, dstToTarget, obstacleMask)) {
                    Debug.Log("hi1");
                    PlayerEnteredTheAngle = true;
					visibleTargets.Add (target);
                    canbeKilled = false;
                    isDetected = 1;
                    escapeTime = 3;
                    
                    targetTime -= Time.deltaTime;
                    if (targetTime<0)
                    {
                        SceneManager.LoadScene("GameScene");
                    }
                   
                   
                }
                
			}
            else
            {
                canbeKilled = true;
                targetTime = 1.8f;
                if(isDetected==1)
                {
                    escapeTime -= Time.deltaTime;
                    if (escapeTime < 0)
                    {
                        isDetected = 2;
                        StartCoroutine(PatrolRandomizer2());
                    }
                }
                
               
                
            }
		}

	}

	void DrawFieldOfView() {
		int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
		float stepAngleSize = viewAngle / stepCount;
		List<Vector3> viewPoints = new List<Vector3> ();
		ViewCastInfo oldViewCast = new ViewCastInfo ();
		for (int i = 0; i <= stepCount; i++) {
			float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
			ViewCastInfo newViewCast = ViewCast (angle);

			if (i > 0) {
				bool edgeDstThresholdExceeded = Mathf.Abs (oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
				if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded)) {
					EdgeInfo edge = FindEdge (oldViewCast, newViewCast);
					if (edge.pointA != Vector3.zero) {
						viewPoints.Add (edge.pointA);
					}
					if (edge.pointB != Vector3.zero) {
						viewPoints.Add (edge.pointB);
					}
				}

			}


			viewPoints.Add (newViewCast.point);
			oldViewCast = newViewCast;
		}

		int vertexCount = viewPoints.Count + 1;
		Vector3[] vertices = new Vector3[vertexCount];
		int[] triangles = new int[(vertexCount-2) * 3];

		vertices [0] = Vector3.zero;
		for (int i = 0; i < vertexCount - 1; i++) {
			vertices [i + 1] = transform.InverseTransformPoint(viewPoints [i]) + Vector3.forward * maskCutawayDst;

			if (i < vertexCount - 2) {
				triangles [i * 3] = 0;
				triangles [i * 3 + 1] = i + 1;
				triangles [i * 3 + 2] = i + 2;
			}
		}

		viewMesh.Clear ();

		viewMesh.vertices = vertices;
		viewMesh.triangles = triangles;
		viewMesh.RecalculateNormals ();
	}


	EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast) {
		float minAngle = minViewCast.angle;
		float maxAngle = maxViewCast.angle;
		Vector3 minPoint = Vector3.zero;
		Vector3 maxPoint = Vector3.zero;

		for (int i = 0; i < edgeResolveIterations; i++) {
			float angle = (minAngle + maxAngle) / 2;
			ViewCastInfo newViewCast = ViewCast (angle);

			bool edgeDstThresholdExceeded = Mathf.Abs (minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
			if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded) {
				minAngle = angle;
				minPoint = newViewCast.point;
			} else {
				maxAngle = angle;
				maxPoint = newViewCast.point;
			}
		}

		return new EdgeInfo (minPoint, maxPoint);
	}


	ViewCastInfo ViewCast(float globalAngle) {
		Vector3 dir = DirFromAngle (globalAngle, true);
		RaycastHit hit;

		if (Physics.Raycast (transform.position, dir, out hit, viewRadius, obstacleMask)) {
			return new ViewCastInfo (true, hit.point, hit.distance, globalAngle);
		} else {
			return new ViewCastInfo (false, transform.position + dir * viewRadius, viewRadius, globalAngle);
		}
	}

	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal) {
		if (!angleIsGlobal) {
			angleInDegrees += transform.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),0,Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}

	public struct ViewCastInfo {
		public bool hit;
		public Vector3 point;
		public float dst;
		public float angle;

		public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle) {
			hit = _hit;
			point = _point;
			dst = _dst;
			angle = _angle;
		}
	}

	public struct EdgeInfo {
		public Vector3 pointA;
		public Vector3 pointB;

		public EdgeInfo(Vector3 _pointA, Vector3 _pointB) {
			pointA = _pointA;
			pointB = _pointB;
		}
	}

    //atakan ekleme part
    void EnemyBehaviour()
    {
        RegularPatrol();
        if(aRobotDied==true)
        {
            RobotSignal = GameObject.FindGameObjectWithTag("Robotsignal");
            enemy.SetDestination(RobotSignal.transform.position);
            StartCoroutine(ComingtoSignal());

        }
        if (isDetected == 1)
        {
            enemy.SetDestination(player.transform.position);
            

        }
    }
    void RegularPatrol()
    {
        if (this.gameObject.tag !="Standingenemy" || this.gameObject.tag != "Standingrobot")
        {
            enemy.SetDestination(destination);
            if (Mathf.Abs(enemy.transform.position.x-direction2.x)<0.5f && Mathf.Abs(enemy.transform.position.z - direction2.z) < 0.5f && isDetected==0)
            {
                destination = direction1;
            }
            else if (Mathf.Abs(enemy.transform.position.x - direction1.x) < 0.5f && Mathf.Abs(enemy.transform.position.z - direction1.z) < 0.5f && isDetected == 0)
            {
                destination = direction2;
            }

        }
       


    }
    void Patrolafterdetection()
    {
        if(isDetected ==2)
        {
            StartCoroutine(PatrolRandomizer2());
        }
        
        

    }

  
    
    IEnumerator PatrolRandomizer2()
    {
        if(isDetected ==2)
        {
            enemy.SetDestination(new Vector3(Random.Range(player.transform.position.x + enemyRadius, player.transform.position.x - enemyRadius), transform.position.y, Random.Range(player.transform.position.z + enemyRadius, player.transform.position.z - enemyRadius)));
            yield return new WaitForSeconds(1);
            enemy.SetDestination(new Vector3(Random.Range(player.transform.position.x + enemyRadius-1, player.transform.position.x - enemyRadius-1), transform.position.y, Random.Range(player.transform.position.z + enemyRadius-1, player.transform.position.z - enemyRadius-1)));
            yield return new WaitForSeconds(2);
            enemy.SetDestination(new Vector3(Random.Range(player.transform.position.x + enemyRadius-2, player.transform.position.x - enemyRadius-2), transform.position.y, Random.Range(player.transform.position.z + enemyRadius-2, player.transform.position.z - enemyRadius-2)));
            yield return new WaitForSeconds(3);
            isDetected = 0;
            


        }
       




    }
    IEnumerator ComingtoSignal()
    {
        yield return new WaitForSeconds(2);
        aRobotDied = false;
        isDetected = 2;
        
    }

   

   
    
    

}