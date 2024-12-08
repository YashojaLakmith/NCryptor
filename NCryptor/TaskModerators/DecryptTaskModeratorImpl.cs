using NCryptor.Crypto;
using NCryptor.Events;
using NCryptor.Helpers;
using NCryptor.Metadata;
using NCryptor.Streams;

namespace NCryptor.TaskModerators;

public class DecryptTaskModeratorImpl : IDecryptTaskModerator
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

    public DecryptTaskModeratorImpl(ISymmetricCryptoService cryptoService,
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

    public async Task ModerateFileDecryptionAsync()
    {
        try
        {
            for (int i = 0; i < _fileList.Count; i++)
            {
                _cancellationToken.ThrowIfCancellationRequested();
                await DecryptCurrentFileAsync(i);
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

    private async Task DecryptCurrentFileAsync(int indexOfFile)
    {
        string inputFilePath = _fileList[indexOfFile];
        string outputFilePath = _fileServices.CreateDecryptedFilePath(inputFilePath, _outputDirectory);

        try
        {
            if (!_fileServices.CheckFileExistance(inputFilePath))
            {
                _eventServices.FileNotFoundEvent(inputFilePath);
                return;
            }

            _eventServices.BeginOfFileDecryptionEvent(inputFilePath, indexOfFile, _fileList.Count);
            await TryDecryptInputFile(inputFilePath, outputFilePath);
            _eventServices.SuccessfulSingleFileCompletionEvent();
        }
        catch (OperationCanceledException)
        {
            _fileServices.DeleteFileIfExists(outputFilePath);
            throw;
        }
        catch (Exception ex)
        {
            _eventServices.ErrorEvent(ex.Message);
            _fileServices.DeleteFileIfExists(outputFilePath);
        }
    }

    private async Task TryDecryptInputFile(string inputFilePath, string outputFilePath)
    {
        await using FileStream fsIn = _streamFactory.CreateReadFileStream(inputFilePath);
        using NcryptorMetadata metadata = await _metadataHandler.ReadMetadataAsync(fsIn, _cancellationToken);
        (byte[] decryptionKey, byte[] calculatedVerificationTag) = _keyServices.DeriveKeyAndVerificationTag(_userKey, metadata.Salt);

        if (!metadata.AreVerificationTagsEqual(calculatedVerificationTag))
        {
            _eventServices.ErrorEvent(@"Incorrect key");
            return;
        }

        await using FileStream fsOut = _streamFactory.CreateWriteFileStream(outputFilePath);
        await _cryptoService.DecryptAsync(fsIn, fsOut, decryptionKey, metadata.IV, _cancellationToken);

        _eventServices.SuccessfulSingleFileCompletionEvent();
        Array.Clear(decryptionKey);
    }
}
