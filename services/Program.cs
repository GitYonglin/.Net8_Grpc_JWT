using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using services;
using services.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddGrpc();
builder.Services.AddGrpc().AddJsonTranscoding();

// GrpcUI 测试工具
builder.Services.AddGrpcReflection();

#region 添加Swagger
builder.Services.AddGrpcSwagger();
builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1",
      new OpenApiInfo { Title = "gRPC transcoding", Version = "v1" });
  //添加Jwt验证设置,添加请求头信息
  c.AddSecurityRequirement(new OpenApiSecurityRequirement
  {
    {
      new OpenApiSecurityScheme
      {
        Reference = new OpenApiReference
        {
          Id = "Bearer",
          Type = ReferenceType.SecurityScheme
        }
      },
      new List<string>()
    }
  });

  //放置接口Auth授权按钮 格式为 Bearer 空格 Token
  c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
  {
    Description = "Bearer {token}",
    Name = "Authorization",//jwt默认的参数名称
    In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
    Type = SecuritySchemeType.ApiKey
  });
});
#endregion

//builder.Services.AddSingleton<TicketRepository>();
builder.Services.AddAuthorization(options =>
{
  options.AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
  {
    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
    policy.RequireClaim(ClaimTypes.Name);
  });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
    options.TokenValidationParameters =
      new TokenValidationParameters
      {
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateActor = false,
        ValidateLifetime = true,
        IssuerSigningKey = JwtToken.SecurityKey
      };
    options.Events = new JwtBearerEvents() // 添加Query token 参数认证
    {
      OnMessageReceived = (context) =>
      {
        if (context.Request.Query.ContainsKey("token"))
        {
          context.Token = context.Request.Query["token"];
        }
        var ts = context.HttpContext.Response.Headers.Authorization;
        return Task.CompletedTask;
      }
    };
  });



var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger(); // 启用Swagger

// 判断当前环境是否为开发环境
if (app.Environment.IsDevelopment())
{
  // Grpcui 测试工具
  app.MapGrpcReflectionService();
  // swagger ui
  app.UseSwaggerUI(c =>
    {
      c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGet(
    "/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909"
);

app.Run();
