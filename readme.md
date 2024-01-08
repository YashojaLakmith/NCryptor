# NCryptor

NCryptor is a file encryption and decryption tool for Windows built using C# and WinForms which runs on .NET Framework 4.8. It provides an easy-to-use and straightforward graphical interface for encrypting and decrypting personal files using symmetric encryption algorithms.

### Table of Contents
- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
- [Runtime Dependencies](#dependencies)
- [Special Notes](#notes)
- [License](#license)

<a name="features"></a>
### Features
- Securely encrypt and decrypt files using symmetric encryption algorithms.
- Simple and user-friendly interface with real-time logs.
- Supports batch encryption and decryption of multiple files..
- Detailed error logging for troubleshooting.
- Can be easily modified to use any symmetric algorithm in the .NET Standard.

<a name="installation"></a>
## Installation

- Clone the repository: `https://github.com/YashojaLakmith/NCryptor.git`.
- Build the solution with `dotnet build` to generate the executable file.

<a name="usage"></a>
### Usage
- Launch the NCryptor application using the .exe file.
- Choose between encryption and decryption modes.
- Select one or more files to process.
- Select the directory to save completed files.
- Enter a valid encryption/decryption key.
- A valid key:
	- must be between 6 and 14 characters
	- may contain alphanumeric charcters as well as _!@#$%^&*_
- Click the _"Start"_ button to initiate the operation.

<a name="dependencies"></a>
### Runtime Dependencies
- The application requires _.NET Framework 4.8_ to be available on the host.
- The application requires _Microsoft C Runtime Library (msvcrt.dll)_ to be present on the host.

<a name="notes"></a>
### Special Notes
- By default, the application implemets __Advanced Encryption Standard with 256bit key size, CBC mode and PKCS7 padding mode.__ However, this can be replaced with any algorithm which derives from `SymmetricAlgorithm` of your choice.

<a name="license"></a>
### License
NCryptor is open-source software licensed under the [MIT License](license.md).