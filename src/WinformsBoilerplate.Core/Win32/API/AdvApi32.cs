namespace WinformsBoilerplate.Core.Win32.API;

public static unsafe class AdvApi32
{
    public static bool LogonUser(string username, string domain, string password)
    {
        return PInvoke.LogonUser(
            lpszUsername: username,
            lpszDomain: domain,
            lpszPassword: password,
            dwLogonType: LOGON32_LOGON.LOGON32_LOGON_NETWORK,
            dwLogonProvider: LOGON32_PROVIDER.LOGON32_PROVIDER_DEFAULT,
            phToken: out _
        );
    }
}
