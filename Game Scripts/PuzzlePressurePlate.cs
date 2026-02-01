using UnityEngine;

public class PuzzlePressurePlate : MonoBehaviour
{
    public PuzzleManager puzzleManager;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")){
            print("not Player");
            return;
        }

        if (puzzleManager != null){
            print("puzzle Start");
            puzzleManager.StartPuzzle();
        }
    }
}
