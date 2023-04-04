using Grpc.Core;
using GrpcExample.Protos;

namespace GrpcExample.Services
{
    public class UnaryService: Unary.UnaryBase
    {
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }
    }
}
