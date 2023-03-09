using DaiwaRentalGD.Gui.ViewModels.Model;
using DaiwaRentalGD.Gui.ViewModels.Model.SiteDesign;
using DaiwaRentalGD.Model.Samples;

namespace DaiwaRentalGD.Gui.Utilities.WorkingJsonFile
{
    public class GDSettingJsonSite
    {
        public GDSettingJsonSite(GDModelSceneMainViewModel modelSceneMainViewModel)
        {
            _modelSceneMainViewModel = modelSceneMainViewModel;
        }

        public void Process()
        {
            var siteDesignerViewModel = _modelSceneMainViewModel.GDModelSceneInputsViewModel.SiteInputsViewModel.SiteDesignerViewModel;
            if (siteDesignerViewModel is SampleSiteDesignerViewModel)
            {
                var sampleJsonSiteCreatorComponent = ((SampleSiteDesignerViewModel)siteDesignerViewModel)._allSiteDesigners.FindAll(t =>
                {
                    if (t.SiteCreatorComponent is SampleJsonSiteCreatorComponent)
                        return true;
                    else return false;
                });
                if (sampleJsonSiteCreatorComponent.Count > 0)
                    ((SampleSiteDesignerViewModel)siteDesignerViewModel).SiteDesigner = sampleJsonSiteCreatorComponent[0];
                
                
            } 
        }
        private GDModelSceneMainViewModel _modelSceneMainViewModel;
    }
}
