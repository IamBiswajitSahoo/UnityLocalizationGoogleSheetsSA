## [1.0.0] - 2025-10-01
### First Release
- Adds a custom service provider to allow connecting to Google Sheets using a Service Account.
- Adds a custom editor for the service provider for providing the 
  - Application Name
  - Authentication Type
  - Spreadsheet ID
  - Service Account Key Json
  - Sheet Properties
  - A button to test authentication of the service account for the provided spreadsheet id
- A pre-made asset is placed under `Integrations/GoogleSheetsService/ServiceAccountGoogleSheetsService.asset` to help quickly test out authentication first before you create your own asset.
- An asset can be created by `Create > Localization > ServiceAccountGoogleSheetsService` in your project and can be used as direct replacement to a Google Sheet Service asset.