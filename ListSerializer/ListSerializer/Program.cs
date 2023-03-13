namespace ListSerializer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ListRandom s = new ListRandom();
            using (Stream stream = new FileStream("log", FileMode.CreateNew))
            {
                s.Serialize(stream);
            }

            using (Stream stream = new FileStream("log", FileMode.Open))
            {
                s.Deserialize(stream);
            }
            
        }
    }
}