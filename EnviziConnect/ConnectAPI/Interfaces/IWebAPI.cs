using System.Collections.Generic;

namespace ConnectAPI
{
    public interface IWebAPI
    {
        bool IsLoggedIn { get; }

        void Close();
        void Initialize(string url, string username, string password);

        void SetOrganization(string name);

        List<Models.GroupData> GetGroups();
        List<Models.LocationData> GetLocations();
        List<EnviziCoreLibrary.AccountData> GetAccounts(string accountReference = null);
        List<Models.MeterData> GetMeters();

        string UploadAccountData(EnviziCoreLibrary.CaptureData captureData);
    }
}