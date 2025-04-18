using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums
{
    // 타일 색상
    public enum TileColor
    {
        None,      // 빈 칸
        Red,
        Orange,
        Brown,
        Cream,
        Yellow,
        Green,
        LightBlue,
        Blue,
        Purple,
        Pink
    }

    // 직사각형의 크기를 나타내는 구조체
    public struct Square
    {
        public int width;
        public int height;
        public Square(int w, int h)
        {
            width = w;
            height = h;
        }
    }

    public enum GameMode
    {
        Normal,
        Infinite
    }

    public enum GameResult
    {
        TimeOver, // 시간 종료
        NoRemovableTiles, // 더 이상 제거 가능한 타일이 없어 게임 종료
        Cleared // 게임 클리어
    }
}
