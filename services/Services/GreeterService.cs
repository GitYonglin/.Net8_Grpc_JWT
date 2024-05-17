using Grpc.Core;
using Microsoft.AspNetCore.Authorization;


namespace services.Services
{
  public class GreeterService(ILogger<GreeterService> logger) : Greeter.GreeterBase
  {
    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
      return Task.FromResult(new HelloReply
      {
        Message = JwtToken.GenerateJwtToken(request.Name)
      });
    }

    [Authorize]
    public override Task<HelloReply> SayHelloAuthorized(HelloRequest request, ServerCallContext context)
    {

      return Task.FromResult(new HelloReply
      {
        Message = "当前用户：" + GetIdentity(context)?.Name
      });
    }
    [Authorize(Roles = "Admin")]
    public override Task<HelloReply> SayHelloAdmin(HelloRequest request, ServerCallContext context)
    {
      return Task.FromResult(new HelloReply
      {
        Message = "当前用户：" + GetIdentity(context)?.Name
      });
    }
    [Authorize(Roles = "Admin,User")]
    public override Task<HelloReply> SayHelloUser(HelloRequest request, ServerCallContext context)
    {
      return Task.FromResult(new HelloReply
      {
        Message = "当前用户：" + GetIdentity(context)?.Name
      });
    }

    static System.Security.Principal.IIdentity? GetIdentity(ServerCallContext context)
    {
      var httpctx = context.GetHttpContext();
      return httpctx.User.Identity;
    }
  }
}
