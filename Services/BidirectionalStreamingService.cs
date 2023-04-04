using Grpc.Core;
using GrpcExample.Protos;

namespace GrpcExample.Services
{
    public class BidirectionalStreamingService : BidirectionalStreaming.BidirectionalStreamingBase
    {
        private readonly ILogger<BidirectionalStreamingService> _logger;
        public BidirectionalStreamingService(ILogger<BidirectionalStreamingService> logger)
        {
            _logger = logger;
        }

        public override async Task SendMessage(
            IAsyncStreamReader<ClientToServerMsg> requestStream,
            IServerStreamWriter<ServerToClientMsg> responseStream,
            ServerCallContext context)
        {
            var clientToServerTask = ClientToServerMsgHandlingAsync(requestStream, context);
            var serverToClientTask = ServerToClientMsgSendingAsync(responseStream, context);

            await Task.WhenAll(clientToServerTask, serverToClientTask);
        }

        private async Task ServerToClientMsgSendingAsync(IServerStreamWriter<ServerToClientMsg> responseStream, ServerCallContext context)
        {
            var count = 0;
            while (!context.CancellationToken.IsCancellationRequested)
            {
                count++;
                await responseStream.WriteAsync(new ServerToClientMsg
                {
                    Content = $"This is a {count} message from server.",
                    Info = "ServerToClient"
                }
                );
                _logger.LogInformation("Server to Client message sent.");
                await Task.Delay(1000);
            }
        }

        private async Task ClientToServerMsgHandlingAsync(IAsyncStreamReader<ClientToServerMsg> requestStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext() && !context.CancellationToken.IsCancellationRequested)
            {
                var message = requestStream.Current;
                _logger.LogWarning("The client sent: {Message}. Info: {Info}", message.Content, message.Info);
            }
        }
    }
}
