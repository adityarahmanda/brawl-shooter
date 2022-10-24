using UnityEngine;

namespace BrawlShooter
{
    /// <summary>
    /// App entry point and main UI flow management.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance { get; private set; }

        public Panel.Type activePanel = Panel.Type.Intro;
        
        private Panel[] _uiPanels;

        private void Awake()
        {
            if (instance == null) 
                instance = this;
            else if (instance != this) 
                Destroy(this);

            _uiPanels = GetComponentsInChildren<Panel>(true);
        }

        private void Start()
        {
            SwitchPanel(activePanel, true);
        }

        public void SwitchPanelToIntro()
        {
            SwitchPanel(Panel.Type.Intro);
        }

        public void SwitchPanel(Panel.Type panelType, bool forceSwitch = false)
        {
            if (activePanel == panelType && !forceSwitch)
                return;

            for(int i = 0; i < _uiPanels.Length; i++)
            {
                if (_uiPanels[i].type == panelType && _uiPanels[i].type != Panel.Type.None)
                {
                    _uiPanels[i].SetVisible(true);
                    activePanel = _uiPanels[i].type;
                }
                else
                {
                    _uiPanels[i].SetVisible(false);
                }
            } 
        }

        public void CloseAllPanels()
        {
            activePanel = Panel.Type.None;

            for (int i = 0; i < _uiPanels.Length; i++)
            {
                _uiPanels[i].SetVisible(false);
            }
        }
    }
}