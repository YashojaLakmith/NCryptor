using NCryptor.Crypto;
using NCryptor.Events;
using NCryptor.Helpers;
using NCryptor.Metadata;
using NCryptor.Streams;

namespace NCryptor.TaskModerators;

public class EncryptTaskModeratorImpl : IEncryptTaskModerator
{
    private readonly ISymmetricCryptoService _cryptoService;
    private readonly IMetadataHandler _metadataHandler;
    private readonly IFileStreamFactory _streamFactory;
    private readonly IFileServices _fileServices;
    private readonly IKeyDerivationServices _keyServices;
    private readonly ITaskModeratorEventService _eventServices;
    private readonly List<string> _fileList;
    private readonly string _outputDirectory;
    private readonly byte[] _userKey;
    private readonly CancellationToken _cancellationToken;

    public EncryptTaskModeratorImpl(ISymmetricCryptoService cryptoService,
    IMetadataHandler metadataHandler,
    IFileStreamFactory streamFactory,
    IFileServices fileServices,
    IKeyDerivationServices keyDerivationServices,
    ITaskModeratorEventService eventService,
    ManualModeratorParameters moderatorParameters)
    {
        _cryptoService = cryptoService;
        _metadataHandler = metadataHandler;
        _streamFactory = streamFactory;
        _fileServices = fileServices;
        _keyServices = keyDerivationServices;
        _eventServices = eventService;
        _fileList = moderatorParameters.FilePathCollection.ToList();
        _outputDirectory = moderatorParameters.OutputDirectory;
        _userKey = moderatorParameters.UserKey;
        _cancellationToken = moderatorParameters.CancellationToken;
    }

    public async Task ModerateFileEncryptionAsync()
    {
        try
        {
            for (int i = 0; i < _fileList.Count; i++)
            {
                _cancellationToken.ThrowIfCancellationRequested();
                await EncryptCurrentFileAsync(i);
            }
        }
        catch (OperationCanceledException)
        {
            _eventServices.CancellationEvent();
            return;
        }
        catch (Exception ex)
        {
            _eventServices.CompletionDueToErrorEvent(ex.Message);
            return;
        }

        _eventServices.CompletionDueToSuccessEvent();
    }

    private async Task EncryptCurrentFileAsync(int indexOfFile)
    {
        string inputFilePath = _fileList[indexOfFile];
        string fullOutputPath = _fileServices.CreateEncryptedFilePath(inputFilePath, _outputDirectory);

        try
        {
            if (!_fileServices.CheckFileExistance(inputFilePath))
            {
                _eventServices.FileNotFoundEvent(inputFilePath);
                return;
            }

            _eventServices.BeginOfFileEncryptionEvent(inputFilePath, indexOfFile, _fileList.Count);
            await TryEncryptInputFileAsync(inputFilePath, fullOutputPath);
            _eventServices.SuccessfulSingleFileCompletionEvent();
        }
        catch (OperationCanceledException)
        {
            _fileServices.DeleteFileIfExists(fullOutputPath);
            throw;
        }
        catch (Exception ex)
        {
            _fileServices.DeleteFileIfExists(fullOutputPath);
            _eventServices.ErrorEvent(ex.Message);
        }
    }

    private async Task TryEncryptInputFileAsync(string inputFilePath, string outputFilePath)
    {
        await using FileStream fsIn = _streamFactory.CreateReadFileStream(inputFilePath);
        await using FileStream fsOut = _streamFactory.CreateWriteFileStream(outputFilePath);

        byte[] salt = _keyServices.GenerateRandomSalt();
        byte[] iv = _keyServices.GenerateRandomIv();
        (byte[] encryptionKey, byte[] tag) = _keyServices.DeriveKeyAndVerificationTag(_userKey, salt);

        using NcryptorMetadata metadata = NcryptorMetadata.Create(salt, tag, iv);
        await _metadataHandler.WriteMetadataAsync(metadata, fsOut, _cancellationToken);

        await _cryptoService.EncryptAsync(fsIn, fsOut, encryptionKey, iv, _cancellationToken);

        Array.Clear(encryptionKey);
    }
}
