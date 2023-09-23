using UnityEngine;
using UnityEngine.SceneManagement;

namespace SapperChain
{
    internal class Options : MonoBehaviour
    {
        public void Restart()
        {
            SceneManager.LoadScene(1);
        }

        public void ToMenu()
        {
            SceneManager.LoadScene(0);
        }

    }
}