using System;
using System.Collections;
using UnityEngine;

//지하에서 대기, 지상에서 대기, 지하 -> 지상 이동, 지상 -> 지하 이동
public enum MoleState { UnderGround = 0, OnGround, MoveUp, MoveDown }
// 두더지 종류 (기본, 점수 -, 시간 +)
public enum MoleType { Normal = 0, Red, Blue}

public class MoleFSM : MonoBehaviour
{
    [SerializeField]
    private GameController gameController; // 콤보 초기화를 위한 GameController
    [SerializeField]
    private float waitTimeOnGround; // 지면에 올라와서 내려가기까지 기다리는 시간
    [SerializeField]
    private float limitMinY; // 내려갈 수 있는 최소 y위치 
    [SerializeField]
    private float limitMaxY; // 올라올 수 있는 최대 y위치

    private Movement3D movement3D; // 위/아래 이동을 위한 Movement3D
    private MeshRenderer meshRenderer; // 두더지 색상 설정을 위한 MeshRenderer
    // private SkinnedMeshRenderer catMeshRenderer;

    private MoleType moleType; // 두더지의 종류
    private Color defaultColor; // 기본 두더지의 색(173, 135, 24)

    public SkinnedMeshRenderer catMeshRenderer;
    public Material[] materials;

    // 두더지의 현재 상태 (set은 moleFSM 클래스 내부에서만)
    public MoleState MoleState { private set; get; }

    public MoleType MoleType
    {
        set
        {
            moleType = value;

            switch (moleType)
            {
                case MoleType.Normal:
                    meshRenderer.material.color = defaultColor;
                    catMeshRenderer.material = materials[0];
                    break;
                case MoleType.Red:
                    meshRenderer.material.color = Color.red;
                    catMeshRenderer.material = materials[2];
                    break;
                case MoleType.Blue:
                    meshRenderer.material.color = Color.blue;
                    catMeshRenderer.material = materials[3];
                    break;

            }
        }
        get => moleType;
    }

    // 두더지가 배치되어 있는 순번 (왼쪽 상단부터 0)
    [field:SerializeField]
    public int MoleIndex { private set; get; }

    private void Awake()
    {
        movement3D = GetComponent<Movement3D>();
        meshRenderer = GetComponent<MeshRenderer>();

        defaultColor = meshRenderer.material.color; // 두더지의 최초 색상 저장

        ChangeState(MoleState.UnderGround);

        //catMeshRenderer.material = materials[0];
    }

    public void ChangeState(MoleState newState)
    {
        // 열거형 변수를 ToString() 메서드를 이용해 문자열로 변환하면
        // "UnderGround"와 같이 열겨형 요소 이름 반환
       

        // 이전에 재생중이던 상태 종료
        StopCoroutine(MoleState.ToString());
        // 상태 변경
        MoleState = newState;
        // 새로운 상태 재생
        StartCoroutine(MoleState.ToString());
    }

    /// <summary>
    /// 두더지가 바닥에서 대기하는 상태로 최초에 바닥 위치로 두더지 위치 설정 
    /// </summary>
    private IEnumerator UnderGround()
    {
        // 이동방향을 : (0,0,0) [정지]
        movement3D.MoveTO(Vector3.zero);

        // 애니메이션 정지
        // "Animator" 컴포넌트에 접근
        Animator animator = GetComponent<Animator>();

        // 현재 실행 중인 애니메이션의 State를 exit 시켜줌
        animator.Play("Base Layer.stop", 0, 0f);

        // 두더지의 y 위치를 홀에 숨어있는 limitMinY 위치로 설정
        transform.position = new Vector3(transform.position.x, limitMinY, transform.position.z);

        yield return null;
    }

    /// <summary>
    /// 두더지가 홀 밖으로 나와있는 상태로 waitTimeOnGround동안 대기 
    /// </summary>
    private IEnumerator OnGround()
    {
        // 이동방향을 : (0,0,0) [정지]
        movement3D.MoveTO(Vector3.zero);

        // "Animator" 컴포넌트에 접근
        Animator animator = GetComponent<Animator>();

        // 현재 실행 중인 애니메이션의 State를 exit 시켜줌
        animator.Play("Base Layer.open", 0, 0f);

        // 두더지의 y 위치를 홀 밖으로 나와있는 limitMaxY 위치로 설정
        transform.position = new Vector3(transform.position.x, limitMaxY, transform.position.z);

        // waitTimeOnGround 시간 동안 대기
        yield return new WaitForSeconds(waitTimeOnGround);

        // 두더지의 상태를 MoveDown으로 변경
        ChangeState(MoleState.MoveDown);
    }

    /// <summary>
    /// 두더지가 홀 밖으로 나오는 상태 (maxYPosUnderGround 위치까지 위로 이동)
    /// </summary>
     private IEnumerator MoveUp()
    {
        // 이동방향 : (0,1,0) [위]
        movement3D.MoveTO(Vector3.up);

        // "Animator" 컴포넌트에 접근
        Animator animator = GetComponent<Animator>();

        // 현재 실행 중인 애니메이션의 State를 exit 시켜줌
        animator.Play("Base Layer.idle", 0, 0f);

        while (true)
        {
            // 두더지의 y위치가 limitMaxY에 도달하면 상태 변경
            if (transform.position.y >= limitMaxY)
            {
                // On Ground 상태로 변경
                ChangeState(MoleState.OnGround);
            }
            yield return null;
        }
    }

    /// <summary>
    /// 두더지가 홀로 들어가는 상태 (minYPosUnderGround 위치까지 아래로 이동)
    /// </summary>
    private IEnumerator MoveDown()
    {
        // 이동방향 : (0,-1,0) [위]
        // movement3D.MoveTO(Vector3.down);

        while (true)
        {
            // 두더지의 y위치가 limitMinY에 도착하면 반복문 중지
            if (transform.position.y <= limitMinY)
            {
                // UnderGround 상태로 변경 
                // ChangeState(MoleState.UnderGround);
                break; // while() 아래쪽 실행을 위해 이동 완료시 break;
            }
            yield return null;
        }

        // 망치에 타격 당하지 않고 자연스럽게 구멍으로 들어갈 때 호출
        // MoveDown -> UnderGround

        // 망치로 때리지 못하고 땅속으로 들어간 두더지의 속성이 Normal이면 콤보 초기화
        if ( moleType == MoleType.Normal)
        {
            gameController.Combo = 0;
        }

        // "Animator" 컴포넌트에 접근
        Animator animator = GetComponent<Animator>();

        // 현재 실행 중인 애니메이션의 State를 exit 시켜줌
        animator.Play("Base Layer.close", 0, 0f);


        // UnderGround 상태로 변경
        ChangeState(MoleState.UnderGround);
    }

    
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void ChangeState(Material material)
    {
        throw new NotImplementedException();
    }
}
