using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public Door doorToOpen;

    public float timeLimit = 8f;

    public PuzzleEnemy[] puzzleEnemies;

    public PuzzleEnemy.PuzzleType[] correctOrder;

    float timer;
    bool puzzleActive = false;
    int currentIndex;

    void Update()
    {
        if (!puzzleActive) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            FailPuzzle();
        }
    }

    public void StartPuzzle()
    {
        if (puzzleActive) return;

        puzzleActive = true;
        timer = timeLimit;
        currentIndex = 0;

        foreach (var e in puzzleEnemies)
        {
            if (e != null)
                e.gameObject.SetActive(true);
        }
    }

    public void OnPuzzleEnemyKilled(PuzzleEnemy enemy)
    {
        if (!puzzleActive) return;

        if (currentIndex >= correctOrder.Length)
        {
            FailPuzzle();
            return;
        }

        if (enemy.type != correctOrder[currentIndex])
        {
            FailPuzzle();
            return;
        }

        currentIndex++;

        if (currentIndex >= correctOrder.Length)
        {
            SuccessPuzzle();
        }
    }

    void FailPuzzle()
    {
        puzzleActive = false;

        foreach (var e in puzzleEnemies)
        {
            if (e != null)
            {
                if (e.gameObject != null)
                    e.gameObject.SetActive(false);
            }
        }
    }

    void SuccessPuzzle()
    {
        puzzleActive = false;

        foreach (var e in puzzleEnemies)
        {
            if (e != null && e.gameObject != null)
                e.gameObject.SetActive(false);
        }

        if (doorToOpen != null)
            doorToOpen.Open();
    }
}
