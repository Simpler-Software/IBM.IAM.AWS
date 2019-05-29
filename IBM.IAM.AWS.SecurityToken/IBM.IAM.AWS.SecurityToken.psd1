#
# Module manifest for module 'IBM.IAM.AWS.SecurityToken'
#
# Generated by: John W Carew
#
# Generated on: 05/20/2018
#
@{
	RootModule = 'IBM.IAM.AWS.SecurityToken.dll'
	ModuleVersion = '1.0.1905'
	GUID = '79cd19d7-c349-4bae-be9a-43f3cc325876'
	Author = 'John W Carew'
	CompanyName = 'Simpler Software'
	Copyright = '(c) 2019 Simpler Software. All rights reserved.'
	Description = 'PowerShell cmdlet to allow SAML authentication with AWS using IBM Identity and Access Management.'
	PowerShellVersion = '5.0'
	DotNetFrameworkVersion = '4.0'
	CLRVersion = '4.0'
	RequiredModules = @()
	RequiredAssemblies = @(
		'AWSSDK.Core.dll',
		'AWSSDK.SecurityToken.dll',
		'Newtonsoft.Json.dll',
		'System.Net.Http.Formatting.dll'
	)
	FileList = @(
		'IBM.IAM.AWS.SecurityToken.dll',
		'IBM.IAM.AWS.SecurityToken.dll-Help.xml',
		'AWSSDK.Core.dll',
		'AWSSDK.SecurityToken.dll',
		'Newtonsoft.Json.dll',
		'System.Net.Http.Formatting.dll',
		'IBM.IAM.AWS.SecurityToken.psd1',
		'LICENSE.txt'
	)
	FormatsToProcess = @('IBM.IAM.AWS.SecurityToken.Format.ps1xml')
	CmdletsToExport = @('Set-AwsIbmSamlCredentials','Get-AwsIbmSamlRoles')
	FunctionsToExport = @()
	AliasesToExport = @()

	PrivateData = @{
		PSData = @{
			Tags = @('AWS', 'SAML', 'IBM', 'IAM')
			Prerelease = 'preview'
			ReleaseNotes = '* Add full proxy support for SAML client and STS client.'
			LicenseUri = 'https://github.com/Simpler-Software/IBM.IAM.AWS/blob/master/LICENSE.md'
			ProjectUri = 'https://github.com/Simpler-Software/IBM.IAM.AWS'
			IconUri = 'https://sdk-for-net.amazonwebservices.com/images/AWSLogo128x128.png'
		} 
	}
}

