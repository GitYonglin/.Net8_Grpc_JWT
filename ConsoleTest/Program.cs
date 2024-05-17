// See https://aka.ms/new-console-template for more information
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using services;

Console.WriteLine("Hello, World!");

string _token = "";
var services = new ServiceCollection();

// 依赖注入方式使用GRPC
services.AddGrpcClient<Greeter.GreeterClient>(o =>
  {
    o.Address = new Uri("https://localhost:7228");
  })
  // 自动添加 依赖 _token
  //  { "Authorization", $"Bearer {token}" }
  .AddCallCredentials((context, metadata) =>
  {
    if (!string.IsNullOrEmpty(_token))
    {
      metadata.Add("Authorization", $"Bearer {_token}");
    }
    return Task.CompletedTask;
  });
using (var serviceProvider = services.BuildServiceProvider())
{
  var client = serviceProvider.GetRequiredService<Greeter.GreeterClient>();

  var reply = await client.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });
  Console.WriteLine(reply.Message);
  _token = reply.Message;

  var reply2 = await client.SayHelloAuthorizedAsync(new HelloRequest { Name = "GreeterClient" });
  Console.WriteLine(reply2.Message);
}

// 直接使用GRPC
static async Task ssAsync()
{
  var channel = GrpcChannel.ForAddress("https://localhost:7228");
  var client = new Greeter.GreeterClient(channel);
  var reply = await client.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });
  Console.WriteLine(reply.Message);

  var token = reply.Message;
  var headers = new Metadata
  {
    { "Authorization", $"Bearer {token}" }
  };

  var reply2 = await client.SayHelloAuthorizedAsync(
    new HelloRequest { Name = "GreeterClient" },
    headers
    );
  Console.WriteLine(reply2.Message);
}

Console.ReadLine();

