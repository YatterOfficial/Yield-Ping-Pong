# Yield-Ping-Pong
Demonstrating the advantages of the C# keyword 'yield' in a game of ping-pong between two servers

- The basic concept is demonstrated in the Console app
- A fully-fledged example occurs using two instances of the same Server

## Quickstart

- Familiarise yourself with how yield works, using the console app.
- Notice, in particular the methods GetPing() and GetPong(), these have a parallel in the Server App.
- Open two command line boxes, side by side, and in the directory containing the Server Program.cs file, run one of the following commands in each console:
  - ```dotnet run --urls "http://localhost:5100;https://localhost:5101" --remoteurl "http://localhost:5000/pingpong?name=ping" --whoami "ping" --server ping --reactiontime 500```
  - ```dotnet run --urls "http://localhost:5000;https://localhost:5001" --remoteurl "http://localhost:5100/pingpong?name=pong" --whoami "pong" --server ping --reactiontime 500```
- You will see that the game runs in the console that belongs to whichever is designated as the --server, ping or pong
- The methods that correspond to GetPing() and GetPong() in the console app, are called GetBim() and GetBam(), respectively. The reason why they cannot be called GetPing() and GetPong(), is that it switches around, depending upon who is server.
- You will, however, see that the yields work just the same as in the Console app, the big difference here is that the --server makes an http call to his opponent, and the yielding response is cleared after each service and response.
