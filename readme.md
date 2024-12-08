# NCryptor

NCryptor is a file encryption and decryption tool for Windows built using C# and WinForms which targets .NET 8 and .NET 9 It provides an easy-to-use and straightforward graphical interface for encrypting and decrypting personal files using symmetric encryption algorithms.

### Table of Contents
- [Features](#features)
- [Building](#building)
- [Usage](#usage)
- [Build Dependencies](#build-dependencies)
- [Runtime Dependencies](#runtime-dependencies)
- [Backwards Compatibility](#backwards-compatibility)
- [Special Notes](#notes)
- [Change Encryption Algorithm](#modifying-algorithm)
- [License](#license)

<a name="features"></a>
### Features
- Securely encrypt and decrypt files using symmetric encryption algorithms.
- Simple and user-friendly interface with real-time logs.
- Supports batch encryption and decryption of multiple files..
- Detailed error logging for troubleshooting.
- Can be easily modified to use any algorithm that derives from `SymmetricAlgorithm` class.

<a name="building"></a>
## Building

- Clone the repository: `https://github.com/YashojaLakmith/NCryptor.git`.
- Build the solution with `dotnet` as an either framework dependent or self-hosted application.

<a name="usage"></a>
### Usage
- Launch the NCryptor application.
- Choose between encryption and decryption modes.
- Select one or more files to process.
- Select the directory to save completed files.
- Enter a valid encryption/decryption key.
- A valid key:
	- must be between 6 and 14 characters
	- may contain alphanumeric charcters as well as _!@#$%^&*_
- Click the _"Start"_ button to initiate the operation.

<a name="build-dependencies"></a>
### Build Dependencies
- The application requires .NET 8 or .NET 9 SDK to be present on the platform for build process.

<a name="runtime-dependencies"></a>
### Runtime Dependencies
- The application requires .NET 8 or .NET 9 Runtime to be present on the platform if it was built as a framework dependent application.

<a name="backwards-compatibility"></a>
### Backwards Compatibility
- It is not guaranteed that the application would be backwards compatible with the previous .NET versions.

<a name="notes"></a>
### Special Notes
- By default, the application uses __Advanced Encryption Standard with 256bit key size, CBC mode and PKCS7 padding mode.__ However, this can be [replaced](#modifying-algorithm) with any algorithm which derives from `SymmetricAlgorithm` of your choice.

<a name="modifying-algorithm"></a>
### Change Encryption Algorithm
Inject any algorithm that derives from `SymmetricAlgorithm` class using the `AddSymmetricAlgorithm` extension method in the `ServiceCollectionExtensions` class.

_Example:_
```csharp
public static IServiceCollection AddSymmetricAlgorithm(this IServiceCollection services)
{
    return services.AddTransient<SymmetricAlgorithm>(
        _ =>
        {
            var aes = Aes.Create();
            aes.KeySize = 256;
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;
            return aes;
        });
}
```

<a name="license"></a>
### License
NCryptor is open-source software licensed under the [MIT License](license.txt).