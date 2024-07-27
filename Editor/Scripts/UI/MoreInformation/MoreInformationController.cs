using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
{
    public class MoreInformationController
    {
        private readonly MoreInformationView _view;
        private readonly MoreInformationModel _model;

        public MoreInformationController(VisualElement root)
        {
            _view = new MoreInformationView(root);
            _model = new MoreInformationModel();
        }

        public void OpenInformationPanel()
        {
            
        }
        
        public void OpenCompanyWebsite()
        {
            
        }
        
        public void OpenDiscordJoinLink()
        {
            
        }
        
        public void OpenCompanyAssetStorePage()
        {
            
        }
        
        public void OpenProductAssetStorePage()
        {
            
        }
    }
}
