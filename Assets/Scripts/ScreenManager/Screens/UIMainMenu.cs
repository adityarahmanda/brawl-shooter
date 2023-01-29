using BrawlShooter;
using Fusion;

namespace BrawlShooter
{
    public class UIMainMenu : TweenScreen
    {
        public void HostGame()
        {
            ScreenManager.Instance.Show<UIRoomMenu>().gameMode = GameMode.Host;
        }

        public void JoinGame()
        {
            ScreenManager.Instance.Show<UIRoomMenu>().gameMode = GameMode.Client;
        }
    }
}
