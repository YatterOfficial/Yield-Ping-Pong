# Yield-Ping-Pong
Demonstrating the advantages of the C# keyword 'yield' in a game of ping-pong between two servers

- The basic concept is demonstrated in the Console app
- A fully-fledged example occurs using two instances of the same Server

## Quickstart

- Familiarise yourself with how yield works, using the console app.
- Notice, in particular how yield works in the methods GetPing() and GetPong(), and from where they are called - these have a parallel pattern in the Server App.
- Open two command line consoles, side by side, and, changing to the directory that contains the Server's ```Program.cs``` file, start a server in each, by running  one of the following respective commands, in each console:
  - ```dotnet run --urls "http://localhost:5100;https://localhost:5101" --remoteurl "http://localhost:5000/pingpong?name=ping" --whoami "ping" --server ping --reactiontime 500```
  - ```dotnet run --urls "http://localhost:5000;https://localhost:5001" --remoteurl "http://localhost:5100/pingpong?name=pong" --whoami "pong" --server ping --reactiontime 500```
- You will see that the game runs in the console that belongs to whichever is designated as the --server, ping or pong
- The methods that correspond to GetPing() and GetPong() in the console app, are called GetBim() and GetBam(), respectively, in the server app. The reason why they cannot be called GetPing() and GetPong() in the server app, is because it switches around, depending upon who is server.
- You will, however, see that the yields work just the same as in the Console app, the big difference here is that the --server makes an http call to his opponent, and the yielding response is cleared after each service and response.

## Benefits

This fully asynchronous, remotely distributed system, illustrates that instead of creating a List&lt;CustomObject&gt; and managing that, that instead, by creating an  IEnumerableAsync&lt;CustomObject&gt; allows the remote management of single items that ultimately constitute a list, instead of having to retrieve the whole list first.
  
Here, the remote system happens to be a server, however it could just as easily have been retrieving a series of items on a queue in the cloud, or merely scraping a series of webpages.

In essence a call is made to retrieve a list, and yet each item is retrieved individually but acted upon sequentially.

The following extract shows that although GetBin() is called once, _ping_ and _pong_ continue to be exposed using the variables _bim_ and _bam_, one by one, and can continue to be exposed one-by-one for weeks, even years, without GetBin() having to be called again.

That's one hell-of-a big 'get list' coming in sequentially, one by one!

```
         await foreach(var bim in GetBim())
         {
            Console.WriteLine($"{bim}");
            await bam.MoveNextAsync();
            Console.WriteLine($"{bam.Current}");
         }
```
  

