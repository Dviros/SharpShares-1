# SharpShares

`SharpShares` is a .NET utility to, well, enumerate shares:

```

            ~SharpShares~
         />_________________________________
[########[]_________________________________|
         \>
                        ~mez0~

```

The goal of `SharpShares` is to be able to parse different input types and run across a network(s) to find SMB services, authenticate, and pull the ACLs for each share.

## Building

[ILMerge](https://github.com/dotnet/ILMerge):

```
.\ILMerge.exe /out:C:\SharpShares.exe .\SharpShares.exe .\LukeSkywalker.IPNetwork.dll
```

## Usage

The available options:

```
OPTIONS:
   -targets       Targets to check
   -username      Username to authenticate with
   -domain        Domain to authenticate with
   -password      Password to authenticate with
   -jitter        Random sleep between 0 and X
   -tcp           Enable/Disable the default TCP check
   -grep          Print in a grepable format
   -acl           Attempt to get ACL for the shares
   -randomise     Loop through targets randomly
```

Targets can take in any of the following:

- File path

```
-targets="c:\users\admin\desktop\computers.txt"
```

- Comma seperated list of IPs or subnets, example:

```
-targets="192.168.0.2,192.168.0.3"
-targets="192.168.0.0/24,192.168.1.0/24"
```

- Dash separated:

```
-targets="192.168.0.12-192.168.0.252"
```

The authentication is handled by the `SharpShares.Helpers.Network.Authenticate()` method, which uses [LogonUser](https://docs.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-logonusera):

```csharp
public static IntPtr Authenticate(String username, String domain, String password)
{
    IntPtr hToken = IntPtr.Zero;
    bool bLogonUser = NativeMethods.LogonUser(username, domain, password, Enums.DwLogonType.LOGON32_LOGON_NEW_CREDENTIALS, Enums.DwLogonProvider.LOGON32_PROVIDER_WINNT50, ref hToken);
    if (!bLogonUser)
    {
        Logger.Print(Logger.STATUS.ERROR, "LogonUser(): ", String.Format("FAILURE ({0})", Logger.GetLastWindowsError()));
    }
    Logger.Print(Logger.STATUS.VERBOSE, "LogonUser() == ", Logger.GetLastWindowsError());
    return hToken;
}
```

The two important things here are:

- **LOGON32_LOGON_NEW_CREDENTIALS**:

> This logon type allows the caller to clone its current token and specify new credentials for outbound connections. The new logon session has the same local identifier but uses different credentials for other network connections. This logon type is supported only by the LOGON32_PROVIDER_WINNT50 logon provider.

- **LOGON32_PROVIDER_WINNT50**:

> Use the negotiate logon provider.

The flags are all booleans and alter the behaviour slightly:

- **Jitter**: Specify an Int32 which will be converted into milliseconds and used to generate the upper value of a random Int32. This will then be used as a sleep between each request made to the network. Note, originally a paralleled for loop was used here, but it was removed because it didn't respect the jitter. This can be reimplemented in the `SharpShares.Helpers.Network.GetTargetsWithOpenPorts()` method by replacing the `foreach` with:

```csharp
Parallel.ForEach(targets, target =>
{
    
});
```

And again in `SharpShares.Helpers.Execution.Execute()`. To be fair, there should probably be a `parallel` flag.

- **TCP**: Enable or disable the `TcpClient()` connection to 445 (saves from listing shares on servers with SMB listening).
- **Grep**: Shares will be printed on online, like so:

```
[16:20:38] |_ 10.10.11.38: ADMIN$ (0), C$ (0), D$ (0), E$ (0), F$ (0), G$ (0), H$ (0), IPC$ (0)
```

- **ACL**:  Use [Directory.GetAccessControl](https://docs.microsoft.com/en-us/dotnet/api/system.io.directory.getaccesscontrol?view=netframework-4.0) to get the total ACLs for the share. [This](https://stackoverflow.com/questions/1410127/c-sharp-test-if-user-has-write-access-to-a-folder) was originally going to be used, but there was too much parsing. The total number of ACLs should be enough of an indicator of which to go for. Higher the amount, the more permissions (AFAIK).
- **Randomise**: Re-jig the `List<String>` to be random, self-explanatory.

## Example

**Without credentials**:

```
PS C:\> .\SharpShares.exe -targets="10.10.11.140" -grep=true -tcp=false -acl=true

            ~SharpShares~
         />_________________________________
[########[]_________________________________|
         \>
                        ~mez0~

[16:44:23] ==> Total targets: 1
[16:44:23] ==> TCP: False
[16:44:23] ==> Verbose: False
[16:44:23] ==> Grep: True
[16:44:23] ==> List: True
[16:44:23] ==> Randomise: False

[16:44:23] ==> Credentials:
[16:44:23] |_ Username: NULL
[16:44:23] |_ Password: NULL

[16:44:23] ==> Request shares on 1 host(s) as NULL

[16:44:23] |_ No shares!

[16:44:23] ==> Done!
[16:44:23] |_ Execution time: 32ms
```

No credentials and `dir`:

```
C:\>dir \\10.10.11.140\TestShare
The user name or password is incorrect.
```

**With credentials**:

```
PS C:\> .\SharpShares.exe -targets="10.10.11.140" -username="testuser" -password="Password123!" -domain="test.local" -grep=true -tcp=false -acl=true

            ~SharpShares~
         />_________________________________
[########[]_________________________________|
         \>
                        ~mez0~

[16:45:05] ==> Total targets: 1
[16:45:05] ==> TCP: False
[16:45:05] ==> Verbose: False
[16:45:05] ==> Grep: True
[16:45:05] ==> List: True
[16:45:05] ==> Randomise: False

[16:45:05] ==> Credentials:
[16:45:05] |_ Username: test.local\testuser
[16:45:05] |_ Password: Password123!

[16:45:05] ==> Request shares on 1 host(s) as test.local\testuser

[16:45:05] |_ 10.10.11.140: ADMIN$ (13), C$ (6), IPC$ (0), NETLOGON (0), Secure (6), SYSVOL (0), TestShare (6)

[16:45:05] ==> Done!
[16:45:05] |_ Execution time: 128ms
```

credentials and `dir`:

```
c:\>dir \\10.10.11.140\TestShare
 Volume in drive \\10.10.11.140\TestShare has no label.
 Volume Serial Number is 5629-F29E

 Directory of \\10.10.11.140\TestShare

08/01/2021  16:46    <DIR>          .
08/01/2021  16:46    <DIR>          ..
08/01/2021  16:46    <DIR>          A Folder
               0 File(s)              0 bytes
               3 Dir(s)  33,982,828,544 bytes free
```

> Happy mappin'.