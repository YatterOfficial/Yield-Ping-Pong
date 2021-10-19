# The 'Yield-Ping-Pong' Pattern

- _Read this as a blog before pulling the code!_

Here we demonstrate the advantages of the C# keyword 'yield' in a game of ping-pong between two servers

- The basic concept is demonstrated in the Console app
- A fully-fledged example occurs using two instances of the same Server
- This 'Yield-Ping-Pong' pattern can be used to sequentially retrieve remotely distributed items, for years, using just a single method call, **once**.
- 'Yield-Ping-Pong' pattern is easily transformed into 'Yield-House-Wife' pattern, whose name is not intended to be sexist but is a reflection upon the 1960s, 70s, and 80s, acclaimed method of solving the Conundrum of Intersteller Communication, in which radio operators are light-years apart, hence messages that are sent, take light years to be received, and then take further light years for the answer to return to the original operator who asked the question.
- Towards the end, we point out that where the oscillation between ping and pong happens remotely, it's Yield-Ping-Pong pattern, and where the oscillation happens locally, it's called Yield-Flip-Flop pattern.
- We then apply this to a SignalR abstraction, demonstrating how each of these patterns have their uses.

## Quickstart (either read-about-it, or do-it)

- Use git to clone the repo as follows: ```git clone https://github.com/YatterOfficial/Yield-Ping-Pong.git``` or download the [Release zip](https://github.com/YatterOfficial/Yield-Ping-Pong/releases/) and expand it.
- Familiarise yourself with how yield works, using the console app.
- Run the console app in Visual Studio Code or Visual Studio, first without the illustrated break-points, and then with the illustrated break-points.
- Notice, in particular how yield works in the methods GetPing() and GetPong(), and from where they are called - these have a parallel pattern in the Server App.
- Close the Console App (as it runs on the same port as one of the following servers), and open two command line consoles, side by side, and, changing to the directory that contains the Server's ```Program.cs``` file, start a server in each, by running  one of the following respective commands, in each console:
  - ```dotnet run --urls "http://localhost:5100;https://localhost:5101" --remoteurl "http://localhost:5000/pingpong?name=ping" --whoami "ping" --server ping --reactiontime 500```
  - ```dotnet run --urls "http://localhost:5000;https://localhost:5001" --remoteurl "http://localhost:5100/pingpong?name=pong" --whoami "pong" --server ping --reactiontime 500```
- You will see that the game runs in the console that belongs to whichever is designated as the --server, ping or pong, feel free to modify --server and --reactiontime (the latter which is in milliseconds)
- The methods that correspond to GetPing() and GetPong() in the console app, are called GetBim() and GetBam(), respectively, in the server app. The reason why they cannot be called GetPing() and GetPong() in the server app, is because it switches around, depending upon who is the --server. _By --server, we mean 'who serves, when starting a game of ping-pong'!_
- You will, however, see that the yields work just the same as in the Console app, the big difference here is that the --server makes an http call to his opponent, and the yielding response is cleared after each service and response.
- Although you can't set the same break-points, watch the output but compare it with the parallel break-point marks to the Console app, in the Server's code.

## Discussion

This fully asynchronous, remotely distributed system, illustrates that instead of creating a List&lt;CustomObject&gt; and managing 'the collective that' which is typically returned as a single list all at once, that instead, by creating a 'list' of IEnumerableAsync&lt;CustomObject&gt; in the asynchronous version of the familiar IEnumerable&lt;CustomObject&gt; pattern - and using the keyword  ```yield``` in the method-call in front of where the individual 'list-items' are created - then the remote management of single items that ultimately constitute a list, yet are yielded sequentially, one by one, instead of being returned corporately as 'the singular, collective, that', prevents the overhead of having to retrieve 'that' whole list first, before being able to operate on individual items.
  
Here, the remote system happens to be a server, however it could just as easily have been retrieving a series of items on a queue in the cloud, or merely scraping a series of webpages.

In essence a call is made to retrieve a list, and yet each item is retrieved individually but acted upon sequentially.

The following extract shows that although GetBim() is called once, _ping_ and _pong_ continue to be exposed using the variables _bim_ and _bam_, one by one, and can continue to be exposed one-by-one for weeks, even years, without GetBim() having to be called again.

- That's one hell-of-a big 'get list' coming in sequentially, one by one, and if you put a break-point on GetBim(), you will see that it is only called once, and yet it will run for years, loosely coupled from the remote server!

```
         var bam = GetBam().GetAsyncEnumerator();
         
         await foreach(var bim in GetBim()) /* breakpoint */
         {
            Console.WriteLine($"{bim}");
            await bam.MoveNextAsync();
            Console.WriteLine($"{bam.Current}");
         } /* breakpoint */
```
  
This is because of how yield works in each of GetBim() and GetBam() in the server's [PingPongTable.cs](https://github.com/YatterOfficial/Yield-Ping-Pong/blob/master/Server/PingPongTable.cs) class.

Although tangential, the responses to the HttpClient's url calls (the pings and pongs, whether bims or bams), are handled in the [PingPongController](https://github.com/YatterOfficial/Yield-Ping-Pong/blob/master/Server/Controllers/PingPongController.cs).

```
   async static IAsyncEnumerable<string> GetBim()
   {
      while(true)
      {
         using(var client = new HttpClient())
         {
            try
            {
               var response = await client.GetStringAsync(OpponentUrl);
               HaveResponse = true;
               Response = response;
            }
            catch(Exception ex){} // wait for other side to boot up
         }
         if(HaveResponse)
         {
            yield return MyName; /* breakpoint */
         }

         await Task.Delay(ReactionTime);
      }
   }

   async static IAsyncEnumerable<string> GetBam()
   {
      while(true)
      {
         if(HaveResponse)
         {
            string returnResponse = Response;
            HaveResponse = false;
            Response = string.Empty;
            if(returnResponse.Equals("ping")||returnResponse.Equals("pong"))
            {
               yield return returnResponse; /* breakpoint */
            }
         }
         await Task.Delay(ReactionTime);
      }
   }
```

A simplified example is illiustrated in the [console app](https://github.com/YatterOfficial/Yield-Ping-Pong/blob/master/ConsoleApp/Program.cs):

```
Console.WriteLine("Starting Game!");

var pong = GetPong().GetAsyncEnumerator();

await foreach(var ping in GetPing()) /* breakpoint */
{
    Console.WriteLine(ping);
    await pong.MoveNextAsync();
    Console.WriteLine(pong.Current);
} /* breakpoint */

Console.ReadLine();

async IAsyncEnumerable<string> GetPing()
{
    while(true)
    {
        yield return "ping"; /* breakpoint */
        await Task.Delay(1000);
    }
}

async IAsyncEnumerable<string> GetPong()
{
    while(true)
    {
        yield return "pong"; /* breakpoint */
        await Task.Delay(1000);
    }
}
```

## Break-Point Heaven => 'Yield-Ping-Pong' Pattern Name

To truly understand what is happening, put a breakpoint on the four lines of code marked as break-points in either the Console app or the Server app, and you will see 'ping' and 'pong' in action!

- GetBim() only gets called once
- The end of GetBim()'s foreach loop gets called every time
- bim and bam oscillate

So now, remove the first two break-points and just leave the break-points on bim and bam:

- bim and bam oscillate, forever returning ping and pong.
- GetBim()'s foreach loop is merely the table upon which both ping and pong bounce

_Which is where this pattern gets it's name!_

Could'a called it Yield-Flip-Flop, but what would have been the fun in that! 

But seriously:

- If the oscillation has a remote end-point, it's ping-pong
- If the oscillation occurs locally, it's flip-flop
- Which shows that the console app demo is actually Yield-Flip-Flop pattern, and the Server demo is actually Yield-Ping-Pong pattern
- This is not just a whim, it came about because I have to design the architecture of how to update list-items in a local Yatter page, from a remote source: the remote-sourcing of an unknown quantity of new list-items will be done using Yield-Ping-Pong pattern (or more-likely the new pattern that is mentioned in the next section), and yet once a single item is received locally in the midst of an unknown stream-length of items, it will be placed in the local Yatter page using Yield-Flip-Flop pattern
- It was a bit of a brain-freeze until this became clear, such are the demands of solution architecture

## The Historical 'House-Wife' Pattern => 'Yield-House-Wife' Pattern

This terminology is not intended to be sexist, but is a reflection upon the 'House-Wife' pattern of intersteller communication that was discussed in the 1960s, 70s, and 80s, in which the conundrum about how to best communicate over inter-steller distances was discussed.

The difficulty was that when a radio operator sends a message, he finishes it, says 'over', and it takes light-years before the receiver, who is light-years away, receives it. This, of course, is because radio-communication travels at the speed of light, and this problem occurs when communicating distances are light-years apart.

Then the receiver replies, finishing with 'over', and it takes years for the reply to get back.

- Sender: 'Hello Red Leader, Alpha-Centauri Spooning-Satellite-One here, do you receive me? Over!'
- The message takes 4.367 years to arrive at planet earth
- Receiver: 'Hello Alpha-Centauri, reading you loud and clear, how can I help you? Over!'
- The message takes 4.367 years to arrive back at Alpha-Centauri Spooning-Satellite-One
- _... and nothing of substance has been said!_

_This delay queues up all the messages, because the sending operator waits for a reply before continuing._

The solution was called ```The Housewife Principle```, in which both sides "just keep talking"! 

Although an answer to any particular question still takes the same time to be answered, from when asked, the interim silent periods are productively filled with anticipative, tangential agenda, so that in the extreme, there aren't any silent periods. _Once again, the name 'The House-Wife Principle' isn't meant to be sexist, but is a historical reflection upon the 1960s, 70s, and 80s._ It still makes me laugh, though!

How this applies to the Yield-Ping-Pong pattern, is that in the PingPongController, there are two filters, and both are designed so that any game of ping-pong is only conducted under the auspices of the server.

To transform Yield-Ping-Pong into Yield-House-Wife, just comment out the following filters and run both servers again - you will see two games of ping-pong being conducted simultaneously, each under the auspices of two single originators.

Comment out these filters in both of the opposing contexts:

```
            if(PingPongTable.Server.Equals("ping"))
            {
                return "not a game";
            }
```

and

```
            if(PingPongTable.Server.Equals("pong"))
            {
                return "not a game";
            }
```

- To abstract Yield-House-Wife pattern into a metaphor of usefulness, imagine a client with a SignalR connection to a remote server.
- If the client has a need, it sends it to the remote server, and waits for the response before sending it's next need; in such a case, the client is interacting with the server using Yield-Ping-Pong pattern.
- But who knows how long it will take the remote server to return it's answer? Instead, why not use Yield-House-Wife pattern to fill the intervening spaces with tangential communication!

**So it would be better for a client to continually stream it's requests over SignalR using Yield-House-Wife pattern, and to use Yield-Flip-Flop pattern locally to distribute the answers!**

- This, of course, only demonstrates the client's needs ... but Yield-House-Wife pattern also allows the server to drop any new need of it's own into the intervening spaces - for example, it might want to tell the user about a pending service disruption, so Yield-House-Wife pattern allows the client to respond to the server, without the client having to request such information from the server.

_Inter-Stella!_, just keep talking, honey!

Copyright (C) Anthony Harrison 2021
