using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleSpawner : MonoBehaviour
{
    [SerializeField]
    private MoleFSM[] Moles; // 맵에 존재하는 두더지들
    [SerializeField]
    private float spawnTime; // 두더지 등장 주기

    // 두더지 등장 확률 (Normal : 85%, Red : 10%, Blue : 5%)
    private int[] spawnPercents = new int[3] { 85, 10, 5 };
    // 한번에 등장하는 최대 두더지 수
    public int MaxSpawnMole { set; get; } = 1;

    // Start is called before the first frame update
    // public void Start()
    public void Setup()
    {
        StartCoroutine("SpawnMole");
    }

    private IEnumerator SpawnMole()
    {
        while (true)
        {
            // 0 ~ Moles.Length-1 중 임의의 숫자 선택
            // int index = Random.Range(0, Moles.Length);

            // 선택된 두더지의 속성 설정
            // Moles[index].MoleType = SpawnMoleType();

            // index번째 두더지의 상태를 "MoveUp"으로 변경
            // Moles[index].ChangeState(MoleState.MoveUp); 

            // MaxSpawnMole 숫자만큼 두더지 등장
            StartCoroutine("SpawnMultiMoles");

            // spawnTIme 시간동안 대기
            yield return new WaitForSeconds(spawnTime);
        }
    }

    private MoleType SpawnMoleType()
    {
        int percent = Random.Range(0, 100);
        float cumulative = 0;

        for( int i = 0; i < spawnPercents.Length; ++i )
        {
            cumulative += spawnPercents[i];

            if ( percent < cumulative)
            {
                return (MoleType)i;
            }
        }

        return MoleType.Normal;
    }

    private IEnumerator SpawnMultiMoles()
    {
        // 0 ~ moles.Length-1 사이의 겹치지 않는 난수를 모두 생성
        int[] indexs = RandomNumerics(Moles.Length, Moles.Length);
        int currentSpawnMole = 0; // 현재 등장한 두더지 숫자
        int currentIndex = 0; // indexs 배열 인덱스

        // 현재 등장해야할 두더지 숫자만큼 두더지 등장
        while ( currentIndex < indexs.Length)
        {
            // 두더지가 바닥에 있을 때만 등장 가능 (현재 등장한 두더지를 사용하지 않도록)
            if (Moles[indexs[currentIndex]].MoleState == MoleState.UnderGround)
            {
                // 선택된 두더지의 속성 설정
                Moles[indexs[currentIndex]].MoleType = SpawnMoleType();
                Moles[indexs[currentIndex]].ChangeState(MoleState.MoveUp);
                //등장한 두더지 숫자 1 증가
                currentSpawnMole++;

                yield return new WaitForSeconds(0.1f);
            }

            // 최대 등장 숫자만큼 등장 했으면 SpawnMultiMoles() 코루틴 함수 종료
            if ( currentSpawnMole == MaxSpawnMole )
            {
                break;
            }

            currentIndex ++;

            yield return null;
        }
    }

    private int[] RandomNumerics(int maxCount, int n)
    {
        // 0 ~ maxCount까지의 숫자 중 겹치지 않는 n개의 난수가 필요할 때 사용
        int[] defaults = new int[maxCount]; // 0 ~ maxCount까지 순서대로 저장하는 배열
        int[] results = new int[n]; // 결과 값들을 저장하는 배열

        // 배열 전체에 0부터 maxCount의 값을 순서대로 저장
        for ( int i = 0; i < maxCount; ++i)
        {
            defaults[i] = i;
        }

        for ( int i = 0; i < n; ++i)
        {
            int index = Random.Range(0, maxCount); // 임의의 숫자를 하나 뽑아서

            results[i] = defaults[index];
            defaults[index] = defaults[maxCount - 1];

            maxCount--;
        }

        return results;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}