﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IBM.IAM.AWS.SecurityToken {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Lang {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Lang() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("IBM.IAM.AWS.SecurityToken.Lang", typeof(Lang).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Found one form in response..
        /// </summary>
        internal static string DebugFoundForm {
            get {
                return ResourceManager.GetString("DebugFoundForm", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Retrieved response from identity provider..
        /// </summary>
        internal static string DebugIdpResponded {
            get {
                return ResourceManager.GetString("DebugIdpResponded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Proxy settings applied for call..
        /// </summary>
        internal static string DebugProxyUsed {
            get {
                return ResourceManager.GetString("DebugProxyUsed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You&apos;re password has expired..
        /// </summary>
        internal static string Error_0XPWDEXPRD {
            get {
                return ResourceManager.GetString("Error_0XPWDEXPRD", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User has provided correct authentication details, but nsAccountLock is set to true for the user in Sun Java™ System Directory Server..
        /// </summary>
        internal static string Error_ACCT_INACTIVATED {
            get {
                return ResourceManager.GetString("Error_ACCT_INACTIVATED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User authentication failed due to a locked(invalid) account..
        /// </summary>
        internal static string Error_ACCT_LOCKED {
            get {
                return ResourceManager.GetString("Error_ACCT_LOCKED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User must login with a certificate when accept-client-certs=prompt_as_needed..
        /// </summary>
        internal static string Error_CERT_LOGIN {
            get {
                return ResourceManager.GetString("Error_CERT_LOGIN", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User tried to step-up to certificate authentication over HTTP, which is not allowed (HTTPS is required)..
        /// </summary>
        internal static string Error_CERT_STEPUP_HTTP {
            get {
                return ResourceManager.GetString("Error_CERT_STEPUP_HTTP", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to External authentication interface information returned to WebSEAL is invalid..
        /// </summary>
        internal static string Error_EAI_AUTH_ERROR {
            get {
                return ResourceManager.GetString("Error_EAI_AUTH_ERROR", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An attempt to authenticate with a client certificate failed. Client failed to authenticate with a certificate when accept-client-certs=required. A valid client certificate is required to make this connection. User&apos;s certificate is invalid..
        /// </summary>
        internal static string Error_FAILED_CERT {
            get {
                return ResourceManager.GetString("Error_FAILED_CERT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User performed an action that makes no sense, such as requesting /pkmslogout while logged in using basic authentication..
        /// </summary>
        internal static string Error_HELP {
            get {
                return ResourceManager.GetString("Error_HELP", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User needs to authenticate..
        /// </summary>
        internal static string Error_LOGIN {
            get {
                return ResourceManager.GetString("Error_LOGIN", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User successfully authenticated, but there is no last cached URL to redirect to..
        /// </summary>
        internal static string Error_LOGIN_SUCCESS {
            get {
                return ResourceManager.GetString("Error_LOGIN_SUCCESS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User has logged out..
        /// </summary>
        internal static string Error_LOGOUT {
            get {
                return ResourceManager.GetString("Error_LOGOUT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User requests password change..
        /// </summary>
        internal static string Error_PASSWD {
            get {
                return ResourceManager.GetString("Error_PASSWD", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User&apos;s password has expired..
        /// </summary>
        internal static string Error_PASSWD_EXP {
            get {
                return ResourceManager.GetString("Error_PASSWD_EXP", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Password change request failed..
        /// </summary>
        internal static string Error_PASSWD_REP_FAILURE {
            get {
                return ResourceManager.GetString("Error_PASSWD_REP_FAILURE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Password change request succeeded..
        /// </summary>
        internal static string Error_PASSWD_REP_SUCCESS {
            get {
                return ResourceManager.GetString("Error_PASSWD_REP_SUCCESS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Password is soon to expire..
        /// </summary>
        internal static string Error_PASSWD_WARN {
            get {
                return ResourceManager.GetString("Error_PASSWD_WARN", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Password change not performed after notification that the password is soon to expire..
        /// </summary>
        internal static string Error_PASSWD_WARN_FAILURE {
            get {
                return ResourceManager.GetString("Error_PASSWD_WARN_FAILURE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User must step-up to another authentication level. Check the AUTHNLEVEL macro for the required authentication level..
        /// </summary>
        internal static string Error_STEPUP {
            get {
                return ResourceManager.GetString("Error_STEPUP", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User requested the switch user login page..
        /// </summary>
        internal static string Error_SWITCH_USER {
            get {
                return ResourceManager.GetString("Error_SWITCH_USER", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User has reached or exceeded the maximum number of allowed sessions..
        /// </summary>
        internal static string Error_TOO_MANY_SESSIONS {
            get {
                return ResourceManager.GetString("Error_TOO_MANY_SESSIONS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An unknown error occurred. Code: {0}.
        /// </summary>
        internal static string Error_Unknown_Error_Code {
            get {
                return ResourceManager.GetString("Error_Unknown_Error_Code", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown operation response {0}.
        /// </summary>
        internal static string Error_Unknown_Operation_Response {
            get {
                return ResourceManager.GetString("Error_Unknown_Operation_Response", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Found multiple forms in response, this is not currently supported..
        /// </summary>
        internal static string ErrorFoundMultiForms {
            get {
                return ResourceManager.GetString("ErrorFoundMultiForms", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid credentials or an error occurred on server. No SAML response found from server&apos;s response..
        /// </summary>
        internal static string ErrorInvalidCredentials {
            get {
                return ResourceManager.GetString("ErrorInvalidCredentials", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A role is required before the profile can be stored..
        /// </summary>
        internal static string ErrorNoRoleSelected {
            get {
                return ResourceManager.GetString("ErrorNoRoleSelected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to set credentials: {0}.
        /// </summary>
        internal static string ErrorUnableSetCredentials {
            get {
                return ResourceManager.GetString("ErrorUnableSetCredentials", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The passed string value is not in a well formatted ARN format. View the help link for more info..
        /// </summary>
        internal static string ErrorUnrecognizedArn {
            get {
                return ResourceManager.GetString("ErrorUnrecognizedArn", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to RoleARN must be specified with PrincipalARN..
        /// </summary>
        internal static string PrincipalRequiredWithRole {
            get {
                return ResourceManager.GetString("PrincipalRequiredWithRole", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Select Role.
        /// </summary>
        internal static string SelectRoleCaption {
            get {
                return ResourceManager.GetString("SelectRoleCaption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Select the role to be assumed when this profile is active..
        /// </summary>
        internal static string SelectRoleMessage {
            get {
                return ResourceManager.GetString("SelectRoleMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Network Credentials given, will attempt to use them..
        /// </summary>
        internal static string UseGivenNetworkCredentials {
            get {
                return ResourceManager.GetString("UseGivenNetworkCredentials", resourceCulture);
            }
        }
    }
}
