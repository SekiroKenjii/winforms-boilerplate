using System.IO.Pipes;
using System.Text.Json;
using WinformsBoilerplate.Core.Abstractions;
using WinformsBoilerplate.Core.Extensions;
using WinformsBoilerplate.Core.Wrappers;

namespace WinformsBoilerplate.Core.Pipes;

/// <summary>
/// Represents a server that uses named pipes for interprocess communication (IPC).
/// </summary>
/// <remarks>The <see cref="NamedPipeServer"/> class provides functionality to create a named pipe server for
/// handling requests from clients. It supports asynchronous communication and allows customization of request handling
/// through the <see cref="OnHandleRequest"/> delegate.</remarks>
/// <param name="pipeName"></param>
public class NamedPipeServer(string pipeName) : Disposable
{
    const int MAX_SERVER_INSTANCE_COUNT = 1;

    private NamedPipeServerStream? _pipeServerStream;

    private NamedPipeServer() : this(DEFAULT_SERVER_NAME) { }

    public const string DEFAULT_SERVER_NAME = "Global\\WinformsBoilerplatePipeServer";

    public static readonly string DefaultPipeName = typeof(NamedPipeServer).FullName ?? "Global\\WinformsBoilerplatePipeName";

    /// <summary>
    /// Gets or sets the name of the pipe.
    /// </summary>
    public string Name { get; set; } = pipeName;

    /// <summary>
    /// Gets or sets the delegate to be invoked when a request is handled.
    /// </summary>
    public Delegate? OnHandleRequest { get; set; }

    /// <summary>
    /// Sends a request to the default server using a named pipe connection.
    /// </summary>
    /// <remarks>This method establishes a connection to the default server, sends the serialized request
    /// data,  and waits for the server to process the request before closing the connection.  Ensure the server is
    /// running and configured to handle named pipe communication.</remarks>
    /// <param name="request">The request data to send, serialized as JSON. Cannot be null or empty.</param>
    public static void SendRequest(string request)
    {
        using NamedPipeClientStream pipeClient = new(DEFAULT_SERVER_NAME);
        pipeClient.Connect();
        JsonSerializer.Serialize(pipeClient, request);
        pipeClient.Flush();
        pipeClient.WaitForPipeDrain();
        pipeClient.Close();
    }

    /// <summary>
    /// Initializes and starts the named pipe server to listen for incoming connections.
    /// </summary>
    /// <remarks>This method sets up a named pipe server stream with the default pipe name and configuration, 
    /// and begins asynchronously waiting for client connections. The server operates in message  transmission mode and
    /// supports multiple instances.</remarks>
    private void Start()
    {
        _pipeServerStream ??= new NamedPipeServerStream(
            pipeName: DefaultPipeName,
            direction: PipeDirection.In,
            maxNumberOfServerInstances: MAX_SERVER_INSTANCE_COUNT,
            transmissionMode: PipeTransmissionMode.Message,
            options: PipeOptions.Asynchronous
        );

        _ = _pipeServerStream.BeginWaitForConnection(StreamServerConnectionCallback, null);
    }

    /// <summary>
    /// Stops the current operation and performs necessary cleanup.
    /// </summary>
    /// <returns><see langword="true"/> if the operation was successfully stopped; otherwise, <see langword="false"/>.</returns>
    private bool Stop()
    {
        return ThrowableFunction<bool>
            .Run(() => {
                CleanupServerStream();

                return true;
            })
            .Catch();
    }

    /// <summary>
    /// Handles the completion of an asynchronous server connection operation for the named pipe server.
    /// </summary>
    /// <remarks>This method processes the incoming connection, deserializes the request, and invokes the
    /// appropriate handler if a valid request is received. If the server is stopped after handling the request, it will
    /// automatically restart.</remarks>
    /// <param name="result">The result of the asynchronous operation initiated by the server.</param>
    private void StreamServerConnectionCallback(IAsyncResult result)
    {
        if (Disposed || _pipeServerStream == null || !StreamServerEndWaitForConnection(result))
        {
            return;
        }

        string? request = JsonSerializer.Deserialize<string>(_pipeServerStream);

        if (string.IsNullOrEmpty(request))
        {
            return;
        }

        _ = OnHandleRequest?.DynamicInvoke(request);

        if (Stop())
        {
            Start();
        }
    }

    /// <summary>
    /// Completes the asynchronous operation of waiting for a connection on the server stream.
    /// </summary>
    /// <remarks>This method finalizes the asynchronous wait for a connection initiated by the server stream. 
    /// If an exception occurs during the operation, it is caught and the method returns <see
    /// langword="false"/>.</remarks>
    /// <param name="result">The <see cref="IAsyncResult"/> representing the status of the asynchronous operation.</param>
    /// <returns><see langword="true"/> if the operation completes successfully; otherwise, <see langword="false"/> if an
    /// exception is caught during execution.</returns>
    private bool StreamServerEndWaitForConnection(IAsyncResult result)
    {
        return ThrowableFunction<bool>.Run(() => {
            _pipeServerStream?.EndWaitForConnection(result);

            return true;
        }).Catch();
    }

    /// <inheritdoc cref="Disposable.Dispose(bool)" />
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (Disposed || _pipeServerStream == null)
        {
            return;
        }

        CleanupServerStream();
    }

    /// <summary>
    /// Cleans up the server stream by disconnecting, closing, and disposing of the underlying pipe server stream.
    /// </summary>
    /// <remarks>This method ensures that the server stream is properly released and set to null.  It should
    /// be called when the server stream is no longer needed to free resources.</remarks>
    private void CleanupServerStream()
    {
        _pipeServerStream?.Disconnect();
        _pipeServerStream?.Close();
        _pipeServerStream?.Dispose();
        _pipeServerStream = null;
    }
}
