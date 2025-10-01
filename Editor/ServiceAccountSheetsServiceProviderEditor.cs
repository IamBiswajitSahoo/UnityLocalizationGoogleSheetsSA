using System;
using UnityEditor;
using UnityEditor.Localization.Plugins.Google;
using UnityEngine;

namespace Biswajit.Unity.Localization.GoogleSheets.Authentication.Editor
{
    /// <summary>
    /// Custom editor drawer for <see cref="ServiceAccountSheetsServiceProvider"/>, that displays GUI fields for providing
    /// the Application Name, Authentication Type, Sheet Properties, Sheet ID & JSON Key file path for the service account.
    /// And a button to validate the access to the spreadsheet, using the service account credentials.
    /// </summary>
    [CustomEditor(typeof(ServiceAccountSheetsServiceProvider))]
    public class ServiceAccountSheetsServiceProviderEditor : UnityEditor.Editor
    {
        // ===================================================================
        // Variables
        // ===================================================================

        private SerializedProperty applicationName;
        private SerializedProperty authenticationType;
        private SerializedProperty jsonKeyFilePath;
        private SerializedProperty spreadsheetId;
        private SerializedProperty newSheetProperties;


        // ===================================================================
        // Unity Callbacks
        // ===================================================================

        private void OnEnable()
        {
            // Links to the GoogleSheetsServiceProvider properties.
            this.applicationName = serializedObject.FindProperty("m_ApplicationName");
            this.authenticationType = serializedObject.FindProperty("m_AuthenticationType");
            this.newSheetProperties = serializedObject.FindProperty("m_NewSheetProperties");

            // Links to the custom ServiceAccountGoogleSheetsServiceProvider properties.
            this.jsonKeyFilePath = serializedObject.FindProperty("jsonKeyFilePath");
            this.spreadsheetId = serializedObject.FindProperty("spreadsheetId");
        }

        public override void OnInspectorGUI()
        {
            // Fetch the latest serialized data
            base.serializedObject.Update();

            // 1. Set the Application (Sheets Name) and the Authentication Type (Expected to be None, and not OAuth or API Key)
            EditorGUILayout.LabelField("Application Name & Authentication", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(this.applicationName);
            EditorGUILayout.PropertyField(this.authenticationType, EditorGUIUtility.TrTextContent("Authentication"));

            var auth = (AuthenticationType)this.authenticationType.intValue;
            if (auth != AuthenticationType.OAuth)
            {
                EditorGUILayout.HelpBox(
                    message: "This is a custom service provider which doesn't user API Key or OAuth Authentication, and uses service account to authenticate internally. " +
                             "But for the GoogleSheetsExtension to allow modifications to the sheet we need to treat this service provider as an OAuth service. " +
                             "Please select OAuth to use the GoogleSheetsExtension.",
                    type: MessageType.Info
                );
            }

            // Disable the below controls if the AuthenticationType is not set to None.
            EditorGUI.BeginDisabledGroup(auth != AuthenticationType.OAuth);
            EditorGUILayout.Space();

            // 2. Spreadsheet ID field
            EditorGUILayout.LabelField("Google Sheet Spreadsheet ID", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(this.spreadsheetId, new GUIContent("Spreadsheet ID"));
            EditorGUILayout.Space();

            // 3. JSON key field
            EditorGUILayout.LabelField("Service Account Key File", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(this.jsonKeyFilePath, new GUIContent("JSON Key File"));
            if (GUILayout.Button("Load JSON Key", GUILayout.MaxWidth(120)))
            {
                var path = EditorUtility.OpenFilePanel(title: "Select Service Account JSON Key", directory: Application.dataPath, extension: "json");

                if (!string.IsNullOrEmpty(path))
                {
                    this.jsonKeyFilePath.stringValue = path;
                }
                else
                {
                    Debug.LogError("Path to JSON Key is empty");
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            // 4. Sheet Properties
            EditorGUILayout.PropertyField(this.newSheetProperties, true);
            EditorGUILayout.Space();

            // 5. Test Authentication button
            if (GUILayout.Button("Test Authentication"))
            {
                try
                {
                    var provider = (ServiceAccountSheetsServiceProvider)target;
                    var service = provider.Service;

                    if (string.IsNullOrEmpty(this.spreadsheetId.stringValue))
                        throw new Exception("Spreadsheet ID cannot be empty.");

                    var request = service.Spreadsheets.Get(this.spreadsheetId.stringValue);
                    var response = request.Execute();

                    Debug.Log($"[SUCCESS] Service account authorized. Spreadsheet title: {response.Properties.Title}");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[FAILED] Service account auth failed: {ex.Message}");
                }
            }

            EditorGUI.EndDisabledGroup();

            // 5. Push the modified properties back into the serialized fields
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}
