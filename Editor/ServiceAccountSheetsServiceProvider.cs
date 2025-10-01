using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using UnityEditor.Localization.Plugins.Google;
using UnityEngine;

namespace Biswajit.Unity.Localization.GoogleSheets.Authentication.Editor
{
    /// <summary>
    /// Custom Google Sheets Service Provider class that performs authentication to Google services via ServiceAccounts which is not supported at the moment
    /// by the <see cref="UnityEditor.Localization.Plugins.Google.SheetsServiceProvider"/> and keeps track of the authentication tokens for avoiding the need
    /// to re-authenticate each time.
    /// <p>
    /// NOTE: If you need OAuth or API Key authentication, please use the Unity's <see cref="UnityEditor.Localization.Plugins.Google.SheetsServiceProvider"/>.
    /// </p>
    /// </summary>
    [CreateAssetMenu(fileName = "Service Account Google Sheets Service", menuName = "Localization/ServiceAccountGoogleSheetsService")]
    [HelpURL("https://developers.google.com/sheets/api/guides/authorizing#AboutAuthorization")]
    public class ServiceAccountSheetsServiceProvider : SheetsServiceProvider
    {
        // ===================================================================
        // Properties
        // ===================================================================

        /// <inheritdoc cref="SheetsServiceProvider.Service"/>
        /// <p>
        /// Overrides the default property to return a new connection using the Service Account.
        /// </p>
        public override SheetsService Service
        {
            get
            {
                if (this.sheetsService == null)
                    this.sheetsService = ConnectWithServiceAccount();

                return this.sheetsService;
            }
        }

        /// <summary>
        /// Returns the spreadsheet identifier for the google sheets file present on Google Drive.
        /// </summary>
        public string SpreadsheetIdentifier => this.spreadsheetId;


        // ===================================================================
        // Variables
        // ===================================================================

        [SerializeField] private AuthenticationType authenticationType;
        [SerializeField] private string jsonKeyFilePath;
        [SerializeField] private string spreadsheetId;
        [SerializeField] private NewSheetProperties newSheetProperties = new NewSheetProperties();

        private SheetsService sheetsService;

        /// <summary>
        /// Defines the scope of the service account.
        /// </summary>
        private static readonly string[] s_serviceAccountScopes = new[] { SheetsService.Scope.Spreadsheets };


        // ===================================================================
        // Unity Callbacks
        // ===================================================================

        protected virtual void OnValidate()
        {
            if (string.IsNullOrEmpty(this.jsonKeyFilePath))
            {
                Debug.LogWarning($"{nameof(jsonKeyFilePath)} is empty. Please assign a JSON key file path.", this);
                return;
            }

            if (!System.IO.File.Exists(this.jsonKeyFilePath))
            {
                Debug.LogWarning($"JSON key file does not exist at path: {this.jsonKeyFilePath}", this);
                return;
            }

            if (string.IsNullOrEmpty(this.spreadsheetId))
            {
                Debug.LogWarning($"{nameof(spreadsheetId)} is empty. Please assign a Spreadsheet ID.", this);
                return;
            }
        }

        // ===================================================================
        // Methods: private, protected
        // ===================================================================

        /// <summary>
        /// Creates a google account connection using the provided service account.
        /// </summary>
        /// <returns></returns>
        protected virtual SheetsService ConnectWithServiceAccount()
        {
            GoogleCredential credential = GoogleCredential.FromFile(this.jsonKeyFilePath).CreateScoped(s_serviceAccountScopes);

            return new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = base.ApplicationName,
            });
        }
    }
}
