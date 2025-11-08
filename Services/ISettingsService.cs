namespace GoatSilencerArchitecture.Services
{
    public interface ISettingsService
    {
        string GetProjectsMainPageLayoutType();
        void SetProjectsMainPageLayoutType(string layoutType);
    }
}
