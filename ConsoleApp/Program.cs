Console.WriteLine("Starting Game!");

var pong = GetPong().GetAsyncEnumerator();

await foreach(var ping in GetPing())
{
    Console.WriteLine(ping);
    await pong.MoveNextAsync();
    Console.WriteLine(pong.Current);
}

Console.ReadLine();

async IAsyncEnumerable<string> GetPing()
{
    while(true)
    {
        yield return "ping";
        await Task.Delay(1000);
    }
}

async IAsyncEnumerable<string> GetPong()
{
    while(true)
    {
        yield return "pong";
        await Task.Delay(1000);
    }
}
