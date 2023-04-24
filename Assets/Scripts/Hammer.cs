using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MonoBehaviour
{
    [SerializeField]
    private float maxY; // 망치의 최대 y 위치

    [SerializeField]
    private float minY; // 망치의 최소 y 위치

    [SerializeField]
    private GameObject MoleHitEffectPrefab; // 두더지 타격 효과 프리팹

    [SerializeField]
    private AudioClip[] audioClips; // 두더지를 타격했을 때 재생되는 사운드

    [SerializeField]
    private MoleHitTextViewer[] moleHitTextViewers; // 타격한 두더지 위치에 타격 정보 텍스트 출

    [SerializeField]
    private GameController gameController; // 점수 증가를 위한  GameController

    [SerializeField]
    private objectDetector objectDetector; // 마우스 클릭으로 오브젝트 선택을 위한 objectDetector
    private Movement3D movement3D; // 망치  오브젝트 이동을 위한 Movement
    private AudioSource audioSource; // 두더지를 타격했을 때 소리를 재생하는 AudioSource

    public SkinnedMeshRenderer catMeshRenderer;
    public Material[] materials;

    private void Awake()
    {
        movement3D = GetComponent<Movement3D>();
        audioSource = GetComponent<AudioSource>();

        // onHit 메소드를 objectDetector Class의 raycastEvent에 이벤트로 등록
        // objectDetector의 raycastEvent.Invoke(hit.transform); 메소드가
        // 호출될 때마다 onHit(Trnasform Target)메소드가 호출된다.
        objectDetector.raycastEvent.AddListener(OnHit);
    }

    private void OnHit(Transform target)
    {
        if (target.CompareTag("Mole"))
        {
            MoleFSM mole = target.GetComponent<MoleFSM>();

            // 두더지가 홀 안에 있을때는 공격 불가
            if (mole.MoleState == MoleState.UnderGround) return;

            // 망치의 위치 설정
            transform.position = new Vector3(target.position.x, minY, target.position.z);

            //망치에 맞우면 얼굴 변경 ?
//            mole.ChangeState(catMeshRenderer.material = materials[1]);

            //망치에 맞았기 때문에 두더지의 상태를 바로 "UnderGround"로 설정
            mole.ChangeState(MoleState.UnderGround);

            // 카메라 흔들기
            ShakeCamera.Instance.OnShakeCamera(0.1f, 0.01f);

            // 두더지 타격 효과 생성 (particle의 색상을 두더지 색상과 동일하게 설정)
            GameObject clone = Instantiate(MoleHitEffectPrefab, transform.position, Quaternion.identity);
            ParticleSystem.MainModule main = clone.GetComponent<ParticleSystem>().main;
            main.startColor = mole.GetComponent<MeshRenderer>().material.color;

            // 점수 증가 (+50)
            // gameController.Score += 50;
            // 두더지의 색상에 따라 처리 (점수, 시간, 사운드 재생)
            MoleHitProcess(mole);

            // 망치를 위로 이동시키는 코루틴 재생
            StartCoroutine("MoveUp");
        }
    }

    private void MoleHitProcess(MoleFSM mole)
    {
        if ( mole.MoleType == MoleType.Normal )
        {
            // gameController.NormalMoleHitCount++;
            gameController.Combo++;
            //gameController.Score += 50;
            // 기본 x1에 10콤보당 0.5씩 더한다
            float scoreMultiple = 1 + gameController.Combo / 10 * 0.5f;
            int getScore = (int)(scoreMultiple * 50);
            // 계산된 점수 getScore를 Score에 더해준다
            gameController.Score += getScore;

            // MoleIndex로 순번을 설정해 두었기 때문에 같은 자리에 있는 TextGetScore 텍스트 출력 
            // 하얀색 텍스트로 점수 증가 표현
            // moleHitTextViewers[mole.MoleIndex].OnHit("Score =50", Color.white);
            moleHitTextViewers[mole.MoleIndex].OnHit("Score +" + getScore, Color.white);
        }
        else if (mole.MoleType == MoleType.Red)
        {
            // gameController.RedMoleHitCount++;
            gameController.Combo = 0; // 빨간색 두더지 터뜨릴시 콤보 0
            gameController.Score -= 300;
            // 빨간색 텍스트로 점수 증가 표현
            moleHitTextViewers[mole.MoleIndex].OnHit("Score -300", Color.red);
        }
        else if (mole.MoleType == MoleType.Blue)
        {
            // gameController.BlueMoleHitCount++;
            gameController.Combo++;
            gameController.CurrentTime += 3;
            // 파란색 텍스트로 점수 증가 표현
            moleHitTextViewers[mole.MoleIndex].OnHit("Score +3", Color.blue);
        }

        // 사운드 재생 (Normal=0. Red=1, Blue=2)
        PlaySound((int)mole.MoleType);
    }

    private void PlaySound(int index)
    {
        audioSource.Stop();
        audioSource.clip = audioClips[index];
        audioSource.Play();
    }

    private IEnumerator MoveUp()
    {
        // 이동방향 (0,1,0) [위]
        movement3D.MoveTO(Vector3.up);

        while (true)
        {
            if(transform.position.y >= maxY)
            {
                movement3D.MoveTO(Vector3.zero);

                break;
            }
            yield return null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
