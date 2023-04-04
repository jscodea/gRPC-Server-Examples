using Grpc.Core;
using GrpcExample.Protos;

namespace GrpcExample.Services
{
    public class SomeService: Some.SomeBase
    {
        public override Task<SmtReply> SaySomething(SmtRequest request, ServerCallContext context)
        {
            return Task.FromResult(new SmtReply
            {
                MessageAAAA = "Hello " + request.Name
            });
        }
    }
}
