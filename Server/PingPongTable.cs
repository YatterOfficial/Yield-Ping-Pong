public class PingPongTable
{
    public static string OpponentUrl { get; set; }
    public static string MyName { get; set; }

    public static string Server { get; set; }

    public static bool HaveResponse { get; set; }

    public static string Response { get; set; }

    public static int ReactionTime { get; set; }

   public async static void PlayPingPong()
   {
      if(Server.Equals(MyName))
      {
         Console.WriteLine("Starting Game!");

         var bam = GetBam().GetAsyncEnumerator();

         await foreach(var bim in GetBim())
         {
            Console.WriteLine($"{bim}");
            await bam.MoveNextAsync();
            Console.WriteLine($"{bam.Current}");
         }
      }
   }

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
            yield return MyName;
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
               yield return returnResponse;
            }
         }
         await Task.Delay(ReactionTime);
      }
   }


}