namespace ListSerializer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var head = new ListNode() { Data = "HeadData" };
            var tail = new ListNode() { Data = "TailData" };
            ListRandom s = new ListRandom() { Count = 123, Head = head, Tail = tail };

            using (Stream stream = new FileStream("log", FileMode.Create))
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