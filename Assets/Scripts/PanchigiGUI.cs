using UnityEngine;

public class PanchigiGUI : MonoBehaviour
{
    [SerializeField] private Panchigi panchigi;
    [SerializeField] private Camera targetCamera;

    void OnGUI()
    {
        if (GUI.Button(new Rect(20, 20, 150, 50), "Reset 4x1"))
        {
            panchigi.ResetGame();
            panchigi.SpawnCoins(CoinFormation.FourInLine);
        }

        if (GUI.Button(new Rect(20, 80, 150, 50), "Reset 6x1"))
        {
            panchigi.ResetGame();
            panchigi.SpawnCoins(CoinFormation.SixInLine);
        }

        if (GUI.Button(new Rect(180, 20, 150, 50), "Reset 4x2"))
        {
            panchigi.ResetGame();
            panchigi.SpawnCoins(CoinFormation.FourByTwo);
        }

        if (GUI.Button(new Rect(180, 80, 150, 50), "Reset 6x2"))
        {
            panchigi.ResetGame();
            panchigi.SpawnCoins(CoinFormation.SixByTwo);
        }

        // Arrow buttons on the top right
        float buttonWidth = 50;
        float buttonHeight = 50;
        float padding = 10;
        float rightEdge = Screen.width - padding - 10;
        float topEdge = padding + 10;

        if (GUI.Button(new Rect(rightEdge - buttonWidth - padding - buttonWidth - padding - buttonWidth, topEdge, buttonWidth, buttonHeight), "-"))
        {
            targetCamera.transform.position += Vector3.up;
        }
        if (GUI.Button(new Rect(rightEdge - buttonWidth - padding - buttonWidth, topEdge, buttonWidth, buttonHeight), "ก่"))
        {
            targetCamera.transform.position += new Vector3(0, 0, 1);
        }
        if (GUI.Button(new Rect(rightEdge - buttonWidth, topEdge, buttonWidth, buttonHeight), "+"))
        {
            targetCamera.transform.position += Vector3.down;
        }
        if (GUI.Button(new Rect(rightEdge - buttonWidth - padding - buttonWidth - padding - buttonWidth, topEdge + buttonHeight + padding, buttonWidth, buttonHeight), "ก็"))
        {
            targetCamera.transform.position += new Vector3(-1, 0, 0);
        }
        if (GUI.Button(new Rect(rightEdge - buttonWidth - padding - buttonWidth, topEdge + buttonHeight + padding, buttonWidth, buttonHeight), "ก้"))
        {
            targetCamera.transform.position += new Vector3(0, 0, -1);
        }
        if (GUI.Button(new Rect(rightEdge - buttonWidth, topEdge + buttonHeight + padding, buttonWidth, buttonHeight), "กๆ"))
        {
            targetCamera.transform.position += new Vector3(1, 0, 0);
        }
    }
}
