using Microsoft.OpenApi.Models;
using Server.Controllers;

var builder = WebApplication.CreateBuilder(args);

string OpponentUrl = string.Empty;
string MyName = string.Empty;
string Server = string.Empty;
int ReactionTime = 0;

try
{
    for(int x = 0; x < args.Length; x++)
    {
        Console.WriteLine($"Arg {x}: {args[x]}");

        if(args[x].Equals("--remoteurl"))
        {
            OpponentUrl = args[x+1];
        }

        if(args[x].Equals("--whoami"))
        {
            MyName = args[x+1];
        }

        if(args[x].Equals("--server"))
        {
            Server = args[x+1];
        }

                
        if(args[x].Equals("--reactiontime"))
        {
            try
            {
                ReactionTime = Convert.ToInt32(args[x+1]);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Arguments Exception: --reactiontime did not cast from {args[x+1]} to int");
            }
        }

    }
}
catch(Exception ex)
{
    Console.WriteLine($"Arguments Exception: {ex.Message}");
}

bool commandLineError = false;
if(string.IsNullOrEmpty(OpponentUrl))
{
    commandLineError = true;
    Console.WriteLine("Argument --remoteurl is missing from command line");
}

if(string.IsNullOrEmpty(MyName))
{
    commandLineError = true;
    Console.WriteLine("Argument --whoami is missing from command line");
}

if(string.IsNullOrEmpty(Server))
{
    commandLineError = true;
    Console.WriteLine("Argument --server is missing from command line");
}

if(!string.IsNullOrEmpty(Server)&&!Server.Equals("ping")&&!Server.Equals("pong"))
{
    commandLineError = true;
    Console.WriteLine("Argument --server must be ping or pong");
}

if(ReactionTime==0)
{
    commandLineError = true;
    Console.WriteLine("Argument --reactiontime must be in milliseconds, eg --reactiontime 1000");    
}

if(commandLineError)
{
    Console.WriteLine("Exiting because of incorrect command line argument");
    return;
}

PingPongController.Name = MyName;
PingPongTable.MyName = MyName;
PingPongTable.OpponentUrl = OpponentUrl;
PingPongTable.Server = Server;
PingPongTable.ReactionTime = ReactionTime;

Thread pingPongGame = new Thread(PingPongTable.PlayPingPong);
pingPongGame.Start();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Server", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Server v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();




