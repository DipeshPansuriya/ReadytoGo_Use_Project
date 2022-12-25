using MediatR;
using User_Command;
using User_Database;
using User_Database.Domain;
using User_Infrastructure;
using User_Infrastructure.Startup_Proj;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

StartupProj.AddStartup(builder);
StartupProj.AddSerilog(builder);
StartupProj.AddSwagger(builder);
StartupProj.AddToken(builder);
StartupProj.AddHangfire(builder);
StartupProj.AddCORS(builder);

builder.Services.AddUserCommand();
builder.Services.AddDatabase(APISetting.UserDBConnection);
builder.Services.AddInfrastructure();

StartupProj.AddGenricAppBuilder(builder);

/// <summary>
/// Configure the HTTP request pipeline.
/// </summary>
WebApplication app = builder.Build();

app.UseHttpLogging();

StartupProj.AddException(app);

if (builder.Environment.IsDevelopment())
{
    //app.UseDeveloperExceptionPage();
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

StartupProj.AddGenricApp(app);

using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider services = scope.ServiceProvider;
    IMediator mediator = services.GetRequiredService<IMediator>();
}

app.Run();