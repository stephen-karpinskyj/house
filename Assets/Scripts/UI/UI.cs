using UnityEngine.SceneManagement;

public class UI : BaseMonoBehaviour
{
    public void HandleResetButtonClick()
    {
        SceneManager.LoadScene(0);
    }
}
