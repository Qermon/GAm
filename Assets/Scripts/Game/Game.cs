using System;

public class Game
{
    private GameManager gameManager;

    public Game()
    {
        gameManager = new GameManager();
    }

    public void Start()
    {
        Console.WriteLine("��� ��������!");
        gameManager.StartGame();
    }
}
