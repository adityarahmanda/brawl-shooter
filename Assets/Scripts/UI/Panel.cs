using UnityEngine;

namespace BrawlShooter
{
    public class Panel : MonoBehaviour
    {
        public enum Type
        {
            None,
            Intro,
            EnterRoom,
            Progress,
            Lobby,
            WinnerUI,
            Error
        }

        public Type type;

        public void SetVisible(bool v)
        {
            gameObject.SetActive(v);
        }
    }
}