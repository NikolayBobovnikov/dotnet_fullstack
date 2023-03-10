
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ListSerializer
{
    public class ListNode
    {
        public ListNode Previous;
        public ListNode Next;
        public ListNode Random;
        public string Data;
    }

    public class ListRandom
    {
        public ListNode Head;
        public ListNode Tail;
        public int Count;

        public void Serialize(Stream s)
        {
            var tSpan = MemoryMarshal.CreateSpan(ref Count, 1);
            var span = MemoryMarshal.AsBytes(tSpan);
            s.Write(span);
        }

        public void Deserialize(Stream s)
        {

        }
    }

    public static class Extensions
    {
        public static void Serialize(this ListNode node, Stream s)
        {
            var dataBytes = Encoding.UTF8.GetBytes(node.Data);
            var len = Encoding.UTF8.GetByteCount(node.Data);
        }

        public static void Deserialize(this Stream s)
        {

        }
    }
}
