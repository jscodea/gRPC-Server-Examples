using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcExample.Protos;

namespace GrpcExample.Services
{
    public class StreamingService: Streaming.StreamingBase
    {
        private readonly ILogger<StreamingService> _logger;
        public StreamingService(ILogger<StreamingService> logger)
        {
            _logger = logger;
        }

        public override async Task SayHelloPeriodically(
            HelloStreamingRequest request,
            IServerStreamWriter<HelloStreamingReply> responseStream,
            ServerCallContext context)
        {
            while(true)
            {
                if (context.CancellationToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Request SayHelloPeriodically cancelled");
                    break;
                }
                await responseStream.WriteAsync(new HelloStreamingReply
                {
                    Message = $"Hello {request.Name} {request.Surname}",
                    Timestamp = Timestamp.FromDateTime(DateTime.UtcNow)
                });
                await Task.Delay(1000);
            }
        }

        public override async Task<MultiHelloStreamingReply> ComposeHellos(
            IAsyncStreamReader<HelloStreamingRequest> requestStream,
            ServerCallContext context)
        {
            var response = new MultiHelloStreamingReply
            {
                Hello = { }
            };
            await foreach(var request in requestStream.ReadAllAsync())
            {
                response.Hello.Add(
                    new HelloStreamingReply
                    {
                        Message = "Hello " + request.Name + " " + request.Surname,
                        Timestamp = Timestamp.FromDateTime(DateTime.UtcNow)
                    }
                 );
                _logger.LogInformation("Request ComposeHellos received");
            }

            return response;
        }

        public override async Task<Empty> CollectLocations(
            IAsyncStreamReader<SendLocationRequest> requestStream,
            ServerCallContext context)
        {
            await foreach (var request in requestStream.ReadAllAsync())
            {
                _logger.LogInformation("Request CollectLocations received");
                _logger.LogInformation("Location received {Location}", request.Location);
            }

            return new();
        }
    }
}
