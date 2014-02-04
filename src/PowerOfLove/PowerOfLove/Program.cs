namespace PowerOfLove
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var game = new MainGame())
                game.Run();
        }
    }
}
