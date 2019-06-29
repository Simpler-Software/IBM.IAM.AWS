﻿using Amazon.SecurityToken.SAML;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Http;
using Amazon.SecurityToken.Model;
using Amazon.Runtime.CredentialManagement;
using Amazon;
using Amazon.Runtime;
using Amazon.SecurityToken;

namespace IBM.IAM.AWS.SecurityToken.SAML
{
    internal class IBMSAMAuthenticationController : IAuthenticationController
    {
        const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36 Edge/15.15063";
        private static Regex SAMLResponseField = new Regex("SAMLResponse\\W+value\\=\\\"([^\\\"]+)\\\"");

        PSCmdlet _cmdlet;
        private string _region;
        private IWebProxy _lastProxy;
        internal string _lastAssertion;

        public SecurityProtocolType SecurityProtocol { get; set; }
        public string ErrorElement { get; set; } = "p";
        public string ErrorClass { get; set; } = "error";
        public Action<string, LogType> Logger { get; set; } = null;

        public IBMSAMAuthenticationController(PSCmdlet cmdlet, string region)
        {
            _cmdlet = cmdlet;
            _region = region;
        }

        public string Authenticate(Uri identityProvider, ICredentials credentials, string authenticationType, IWebProxy proxySettings)
        {
            string result = null;
            //ImpersonationState impersonationState = null;
            try
            {
                CookieContainer cookies = new CookieContainer();
                _lastProxy = proxySettings;
                //if (credentials != null)
                //{
                //    impersonationState = ImpersonationState.Impersonate(credentials.GetCredential(identityProvider, authenticationType));
                //}
                using (HttpWebResponse httpWebResponse = this.QueryProvider(identityProvider, cookies, null, proxySettings))
                {
                    using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                        result = streamReader.ReadToEnd();
                    _cmdlet.WriteVerbose($"Retrieved response from identity provider.");
                    Dictionary<string, string> values = new Dictionary<string, string>();
                    if (credentials != null)
                    {
                        var creds = credentials as NetworkCredential;
                        values.Add("username", creds.UserName);
                        values.Add("password", creds.Password);
                    }
                    var formResponse = GetFormData(result, values);

                    bool stopRequest = false;
                    int tryCount = 1;
                    do
                    {
                        Uri postTo = new Uri(httpWebResponse.ResponseUri, formResponse.Action);
                        using (HttpWebResponse httpWebResponsePost = this.PostProvider(postTo, cookies, httpWebResponse.ResponseUri, formResponse.FormData, proxySettings))
                        {
                            if (httpWebResponsePost.StatusCode == HttpStatusCode.Found)
                            {
                                string location = httpWebResponsePost.Headers[HttpResponseHeader.Location];
                                if (!string.IsNullOrWhiteSpace(location))
                                {
                                    Uri uLocation = new Uri(location);
                                    var qry = new UrlEncodingParser(uLocation);
                                    if (qry.AllKeys.Contains("TAM_OP", StringComparer.CurrentCultureIgnoreCase))
                                    {
                                        string TAM_OP = qry["TAM_OP"];
                                        //https://www.ibm.com/support/knowledgecenter/en/SSPREK_9.0.0/com.ibm.isam.doc/wrp_config/concept/con_op_redir.html
                                        switch (TAM_OP.ToLower())
                                        {
                                            case "acct_inactivated":
                                                throw new IbmIamException("User has provided correct authentication details, but nsAccountLock is set to true for the user in Sun Java™ System Directory Server.");
                                            case "acct_locked":
                                                throw new IbmIamException("User authentication failed due to a locked(invalid) account.");
                                            case "cert_login":
                                                throw new IbmIamException("User must login with a certificate when accept-client-certs=prompt_as_needed.");
                                            case "cert_stepup_http":
                                                throw new IbmIamException("User tried to step-up to certificate authentication over HTTP, which is not allowed (HTTPS is required).");
                                            case "eai_auth_error":
                                                throw new IbmIamException("External authentication interface information returned to WebSEAL is invalid.");
                                            case "error":
                                                {
                                                    string ERROR_CODE = qry["ERROR_CODE"];
                                                    switch (ERROR_CODE.ToLower())
                                                    {
                                                        case "0xpwdexprd":
                                                            string url = qry["URL"];
                                                            throw new IbmIamPasswordExpiredException("You're password has expired.") { HelpLink = url };
                                                        default:
                                                            throw new IbmIamErrorException($"An unknown error occurred. Code: {ERROR_CODE}", ERROR_CODE);
                                                    }
                                                }
                                            case "failed_cert":
                                                throw new IbmIamException("An attempt to authenticate with a client certificate failed. Client failed to authenticate with a certificate when accept-client-certs=required. A valid client certificate is required to make this connection. User's certificate is invalid.");
                                            case "help":
                                                throw new IbmIamException("User performed an action that makes no sense, such as requesting /pkmslogout while logged in using basic authentication.");
                                            case "login":
                                                throw new IbmIamException("User needs to authenticate.");
                                            case "login_success":
                                                throw new IbmIamException("User successfully authenticated, but there is no last cached URL to redirect to.");
                                            case "logout":
                                                throw new IbmIamException("User has logged out.");
                                            case "passwd":
                                                throw new IbmIamException("User requests password change.");
                                            case "passwd_exp":
                                                {
                                                    string url = qry["URL"];
                                                    throw new IbmIamPasswordExpiredException("User's password has expired.") { HelpLink = url };
                                                }
                                            case "passwd_rep_failure":
                                                throw new IbmIamException("Password change request failed.");
                                            case "passwd_rep_success":
                                                throw new IbmIamException("Password change request succeeded.");
                                            case "passwd_warn":
                                                throw new IbmIamException("Password is soon to expire.");
                                            case "passwd_warn_failure":
                                                throw new IbmIamException("Password change not performed after notification that the password is soon to expire.");
                                            case "stepup":
                                                throw new IbmIamException("User must step-up to another authentication level.Check the AUTHNLEVEL macro for the required authentication level.");
                                            case "switch_user":
                                                throw new IbmIamException("User requested the switch user login page.");
                                            case "too_many_sessions":
                                                throw new IbmIamException("User has reached or exceeded the maximum number of allowed sessions.");
                                            default:
                                                throw new IbmIamException($"Unknown operation response {TAM_OP}");
                                        }
                                    }
                                    else
                                    {
                                        using (HttpWebResponse httpRedirectResponse = this.QueryProvider(uLocation, cookies, httpWebResponse.ResponseUri, proxySettings, true))
                                        {
                                            using (StreamReader streamReader = new StreamReader(httpRedirectResponse.GetResponseStream()))
                                            {
                                                result = streamReader.ReadToEnd();
                                                if (!SAMLResponseField.IsMatch(result))
                                                {
                                                    // This should be asking for the MFA now
                                                    formResponse = GetFormData(result, values);
                                                }
                                                else
                                                {
                                                    stopRequest = true;
                                                    MatchCollection resposne = SAMLResponseField.Matches(result);
                                                    foreach (Match data in resposne)
                                                    {
                                                        return _lastAssertion = data.Groups[1].Value;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader streamReader = new StreamReader(httpWebResponsePost.GetResponseStream()))
                                {
                                    result = streamReader.ReadToEnd();
                                    string errMsg = CheckErrorMessage(result);
                                    if (!string.IsNullOrWhiteSpace(errMsg))
                                    {
                                        throw new IbmIamException(errMsg);
                                    }
                                    else if (!SAMLResponseField.IsMatch(result))
                                    {
                                        // This should be asking for the MFA now
                                        formResponse = GetFormData(result, values);
                                    }
                                    else
                                    {
                                        stopRequest = true;
                                        MatchCollection resposne = SAMLResponseField.Matches(result);
                                        foreach (Match data in resposne)
                                        {
                                            return _lastAssertion = data.Groups[1].Value;
                                        }
                                    }
                                }
                            }
                        }
                        tryCount++;
                    }
                    while (!stopRequest && tryCount < 5);
                }
            }
            catch (IbmIamException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AdfsAuthenticationControllerException(ex.ToString(), ex);
            }
            finally
            {
                //if (impersonationState != null)
                //{
                //    impersonationState.Dispose();
                //}
            }
            throw new Amazon.Runtime.FederatedAuthenticationFailureException("Invalid credentials or an error occurred on server. No SAML response found from server's response.");
        }

        private HttpWebResponse QueryProvider(Uri identityProvider, CookieContainer cookies, Uri referer, IWebProxy proxySettings, bool autoRedirect = true)
        {
            _cmdlet.WriteVerbose($"Querying identity provider on host '{identityProvider.Host}' via {identityProvider.Scheme}.");
            ServicePointManager.SecurityProtocol = this.SecurityProtocol;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(identityProvider);
            httpWebRequest.UserAgent = IBMSAMAuthenticationController.UserAgent;
            httpWebRequest.Referer = referer?.ToString();
            httpWebRequest.KeepAlive = true;
            httpWebRequest.PreAuthenticate = true;
            httpWebRequest.AllowAutoRedirect = autoRedirect;
            httpWebRequest.CookieContainer = cookies;
            if (proxySettings != null)
            {
                _cmdlet.WriteVerbose($"Proxy settings applied for call.");
                httpWebRequest.Proxy = proxySettings;
            }
            httpWebRequest.UseDefaultCredentials = true;
            var rspns = (HttpWebResponse)httpWebRequest.GetResponse();
            _cmdlet.WriteVerbose($"Query returned status '{(int)rspns.StatusCode}' '{rspns.StatusDescription}'.");
            return rspns;
        }

        private HttpWebResponse PostProvider(Uri identityProvider, CookieContainer cookies, Uri referer, Dictionary<string, string> formData, IWebProxy proxySettings)
        {
            // Create POST data and convert it to a byte array.  
            StringBuilder postData = new StringBuilder();
            foreach (string key in formData.Keys)
            {
                if (postData.Length == 0)
                    postData.Append($"{key}={WebUtility.UrlEncode(formData[key])}");
                else
                    postData.Append($"&{key}={WebUtility.UrlEncode(formData[key])}");
            }
            byte[] byteArray = Encoding.UTF8.GetBytes(postData.ToString());

            _cmdlet.WriteVerbose($"Posting to identity provider on host '{identityProvider.Host}' via {identityProvider.Scheme}.");
            ServicePointManager.SecurityProtocol = this.SecurityProtocol;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(identityProvider);
            httpWebRequest.UserAgent = IBMSAMAuthenticationController.UserAgent;
            httpWebRequest.Referer = referer?.ToString();
            httpWebRequest.KeepAlive = true;
            httpWebRequest.PreAuthenticate = true;
            httpWebRequest.AllowAutoRedirect = false;
            httpWebRequest.CookieContainer = cookies;
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.ContentLength = byteArray.Length;
            if (proxySettings != null)
            {
                _cmdlet.WriteVerbose($"Proxy settings applied for call.");
                httpWebRequest.Proxy = proxySettings;
            }

            Stream dataStream = httpWebRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            var rspns = (HttpWebResponse)httpWebRequest.GetResponse();
            _cmdlet.WriteVerbose($"Post returned status '{(int)rspns.StatusCode}' '{rspns.StatusDescription}'.");
            return rspns;
        }

        private string SecureStringToString(SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }

        private FormResponse GetFormData(string htmlResults, Dictionary<string,string> predefinedValues)
        {
            // <form class="form-signin" role="form" method="post" action="Login">
            Regex rgxForm = new Regex(@"<form\s(?<Attr>[^>]*)/?>(?<Content>[\s\w\S\W]*)</form>");
            Regex rgxAtrAction = new Regex(@"(?<=\baction=""|')[^ ""']*");
            var forms = rgxForm.Matches(htmlResults);
            if (forms.Count == 1)
            {
                _cmdlet.WriteVerbose($"Found one form in response.");
                FormResponse formResponse = new FormResponse();
                string formAttributes = forms[0].Groups["Attr"].Value;
                string formContent = forms[0].Groups["Content"].Value;

                Match mAtt = null;
                if ((mAtt = rgxAtrAction.Match(formAttributes)).Success)
                    formResponse.Action = mAtt.Value;

                //  <input type="username" class="form-control nv-text-input" name="username" required autofocus></input>
                Regex rgxInputs = new Regex(@"<input\s([^>]*)/?>");
                Regex rgxAtrType =  new Regex(@"(?<=\stype=[""']+)[^""']*|(?<=\stype=)[\w]+");
                Regex rgxAtrName =  new Regex(@"(?<=\sname=[""']+)[^""']*|(?<=\sname=)[\w]+");
                Regex rgxAtrValue = new Regex(@"(?<=\svalue=[""']+)[^""']*|(?<=\svalue=)[\w]+");
                Regex rgxLabels = new Regex(@"<label\s([^>]*)>([^<]+)</label>");
                Regex rgxAtrFor = new Regex(@"(?<=\sfor=[""']+)[^""']*|(?<=\sfor=)[\w]+");
                Hashtable inputs = new Hashtable();

                var fLabels = rgxLabels.Matches(formContent);
                _cmdlet.WriteVerbose($"Found {fLabels.Count} label(s) in form. (Some may be hidden fields.)");
                foreach (Match match in fLabels)
                {
                    string name = null;
                    string value = null;
                    if (match.Success)
                    {
                        if ((mAtt = rgxAtrFor.Match(match.Groups[1].Value)).Success)
                            name = mAtt.Value;
                        value = WebUtility.HtmlDecode(match.Groups[2].Value);
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            if (formResponse.LabelData.ContainsKey(name))
                                formResponse.LabelData[name] = value;
                            else
                                formResponse.LabelData.Add(name, value);
                        }
                    }
                }

                var fInputs = rgxInputs.Matches(formContent);
                _cmdlet.WriteVerbose($"Found {fInputs.Count} input(s) in form. (Some may be hidden fields.)");
                foreach (Match match in fInputs)
                {
                    string name = null;
                    string type = null;
                    string value = null;
                    if (match.Success)
                    {
                        if ((mAtt = rgxAtrName.Match(match.Value)).Success)
                            name = mAtt.Value;
                        if ((mAtt = rgxAtrType.Match(match.Value)).Success)
                            type = mAtt.Value;
                        if ((mAtt = rgxAtrValue.Match(match.Value)).Success)
                            value = WebUtility.HtmlDecode(mAtt.Value);
                        if (!type.Equals("hidden") && !type.Equals("submit"))
                        {
                            string displayName = name;
                            if (formResponse.LabelData.ContainsKey(name))
                                displayName = $"{formResponse.LabelData[name]}".Trim();
                            if (!displayName.EndsWith(":"))
                                displayName += ":";
                            _cmdlet.Host.UI.Write($"{displayName} ");
                            if (predefinedValues != null && predefinedValues.ContainsKey(name.ToLower()))
                            {
                                _cmdlet.Host.UI.WriteLine($"(using predefined {name})");
                                value = predefinedValues[name.ToLower()];
                            }
                            else if (type.Equals("password"))
                            {
                                using (SecureString secStr = _cmdlet.Host.UI.ReadLineAsSecureString())
                                {
                                    value = this.SecureStringToString(secStr);
                                }
                            }
                            else
                                value = _cmdlet.Host.UI.ReadLine();
                        }
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            if (formResponse.FormData.ContainsKey(name))
                                formResponse.FormData[name] = value;
                            else
                                formResponse.FormData.Add(name, value);
                        }
                    }
                }

                return formResponse;
            }
            else
            {
                throw new NotSupportedException("Found multiple forms in response, this is not currently supported.");
            }
        }

        private string CheckErrorMessage(string htmlResults)
        {
            // (?<=<p\s[\s\w\S\W]*class=["']*error["']*[^>]*>)(?<Content>.+?)(?=</p>)
            if (string.IsNullOrWhiteSpace(this.ErrorClass))
            {
                Regex rgxElmErr = new Regex($@"(?<=<{this.ErrorElement}[^>]*>)(?<Content>.+?)(?=</{this.ErrorElement}>)", RegexOptions.IgnoreCase);
                var mtch = rgxElmErr.Match(htmlResults);
                if (mtch.Success)
                {
                    return mtch.Groups["Content"].Value;
                }
            }
            else
            {
                Regex rgxElmErr = new Regex($@"(?<=<{this.ErrorElement}\s[\s\w\S\W]*class=[""']*{this.ErrorClass}[""']*[^>]*>)(?<Content>.+?)(?=</{this.ErrorElement}>)", RegexOptions.IgnoreCase);
                var mtch = rgxElmErr.Match(htmlResults);
                if (mtch.Success)
                {
                    return mtch.Groups["Content"].Value;
                }
            }
            return null;
        }

        private void LoggerInternal(string message, LogType type = LogType.Info)
        {
            Logger?.Invoke(message, type);
        }

        private Credentials AssumeRole(ICredentialProfileStore config, SAMLCredential role, Func<SAMLCredential, RegionEndpoint> DecodeRegionFromRole = null)
        {
            return AssumeRole(config, null, role, DecodeRegionFromRole);
        }
        private Credentials AssumeRole(ICredentialProfileStore config, string profileName, SAMLCredential role, Func<SAMLCredential, RegionEndpoint> DecodeRegionFromRole = null)
        {
            var credential = AssumeRole(role, DecodeRegionFromRole);
            AddRoleToConfig(config ?? throw new ArgumentNullException(nameof(config)),
                profileName,
                role ?? throw new ArgumentNullException(nameof(role)),
                credential,
                DecodeRegionFromRole);
            return credential;
        }
        private Credentials AssumeRole(SAMLCredential role, Func<SAMLCredential, RegionEndpoint> DecodeRegionFromRole = null)
        {
            DecodeRegionFromRole = DecodeRegionFromRole ?? (x => null);

            if (!role.PrincipalArn.ResourceType.Equals("saml-provider", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException($"Principal ARN Resource Type is [{role.PrincipalArn.ResourceType}], should be [saml-provider]");

            LoggerInternal($"Found SAML Provider in {role.PrincipalArn}", LogType.Debug);

            AnonymousAWSCredentials anonCred = new AnonymousAWSCredentials();
            AmazonSecurityTokenServiceConfig cfg = new AmazonSecurityTokenServiceConfig();
            cfg.RegionEndpoint = DecodeRegionFromRole(role) ?? RegionEndpoint.GetBySystemName(this._region ?? cfg.RegionEndpoint.SystemName);
            if (this._lastProxy != null)
                cfg.SetWebProxy(this._lastProxy as WebProxy);

            AmazonSecurityTokenServiceClient sts = new AmazonSecurityTokenServiceClient(anonCred, cfg);

            LoggerInternal($"Calling AssumeRoleWithSAML at the {cfg.RegionEndpoint.SystemName} region to retrieve Access and Secret Keys for {role.RoleArn.Resource}.", LogType.Debug);
            AssumeRoleWithSAMLResponse response = AsyncHelpers.RunSync(async () => await sts.AssumeRoleWithSAMLAsync(new AssumeRoleWithSAMLRequest()
            {
                PrincipalArn = role.PrincipalArn.OriginalString,
                RoleArn = role.RoleArn.OriginalString,
                SAMLAssertion = this._lastAssertion,
                DurationSeconds = 3600
            }) ?? throw new Exception("No Credentials returned from AWS"));

            return response.Credentials;
        }

        private void AddRoleToConfig(ICredentialProfileStore config, string profileName, SAMLCredential role, Credentials t, Func<SAMLCredential, RegionEndpoint> DecodeRegionFromRole = null)
        {
            DecodeRegionFromRole = DecodeRegionFromRole ?? (x => null);

            if (!config.TryGetProfile(profileName ?? role.RoleArn.Resource, out CredentialProfile profile))
            {
                var options = new CredentialProfileOptions();
                profile = new CredentialProfile(role.RoleArn.Resource, options);
            }
            profile.Options.AccessKey = t.AccessKeyId;
            profile.Options.SecretKey = t.SecretAccessKey;
            profile.Options.Token = t.SessionToken;

            var role_region = DecodeRegionFromRole(role);
            if (role_region != null)
            {
                LoggerInternal($"Adding auto-magic region option to {role.RoleArn.Resource}", LogType.Debug);
                profile.Region = role_region;
            }
            else if (!string.IsNullOrWhiteSpace(this._region))
            {
                LoggerInternal($"Adding region option to {role.RoleArn.Resource}", LogType.Debug);
                profile.Region = RegionEndpoint.GetBySystemName(this._region);
            }
            config.RegisterProfile(profile);
        }

    }

    class FormResponse
    {
        public string Action { get; set; }
        public Dictionary<string, string> FormData { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> LabelData { get; set; } = new Dictionary<string, string>();
    }
}