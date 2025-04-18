using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums
{
    // Ÿ�� ����
    public enum TileColor
    {
        None,      // �� ĭ
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

    // ���簢���� ũ�⸦ ��Ÿ���� ����ü
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
        TimeOver, // �ð� ����
        NoRemovableTiles, // �� �̻� ���� ������ Ÿ���� ���� ���� ����
        Cleared // ���� Ŭ����
    }
}
