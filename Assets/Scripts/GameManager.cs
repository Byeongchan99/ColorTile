using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        
    }

    // 1. 게임 시작
    public void GameStart()
    {
        // 1. 스테이지 생성
        // 2. 타이머 시작
        // 3. 메인화면 닫기
    }

    // 2. 게임 종료
    public void GameOver()
    {
        // 1. 타이머 정지
        // 2. 게임 결과 표시
    }

    // 3. 일시정지
    public void Pause()
    {
        // 1. 타이머 정지
        // 2. 일시정지 화면 표시
    }

    // 4. 게임 재시작
    public void GameRestart()
    {
        // 1. 타이머 초기화 및 재시작
        // 2. 스테이지 초기화 및 재생성
        // 3. 일시정지 화면 닫기
    }

    // 5. 메인 화면으로 이동
    public void GoToMain()
    {
        // 1. 타이머 초기화
        // 2. 스테이지 초기화
        // 3. 게임 결과 화면 / 일시정지 화면 닫기
        // 4. 메인 화면 표시
    }
}
