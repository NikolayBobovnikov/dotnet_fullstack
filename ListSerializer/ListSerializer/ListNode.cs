
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

namespace ListSerializer
{
    public static class Extensions
    {
        public static void Write<T>(this Stream stream, T value)
            where T : unmanaged
        {
            int size = Marshal.SizeOf(value);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            try
            {
                Marshal.StructureToPtr(value, ptr, false);
                byte[] managedArray = new byte[size];
                Marshal.Copy(ptr, managedArray, 0, size);
                stream.Write(managedArray, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public static T? Read<T>(this Stream stream)
            where T : unmanaged
        {
            T? result = default;
            int size = Marshal.SizeOf(result);
            byte[] managedArray = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            stream.Read(managedArray, 0, size);
            Marshal.Copy(managedArray, 0, ptr, size);
            result = Marshal.PtrToStructure<T>(ptr);

            return result;
        }
    }

    public class ListNode
    {
        public ListNode Previous;
        public ListNode Next;
        public ListNode Random;
        public string Data;


        public static void Serialize(ListNode? node, Stream s)
        {
            if (node != null)
            {
                s.Write(node);

                if (node.Previous != null)
                {
                    Serialize(node.Previous, s);
                }
                if (node.Next != null)
                {
                    Serialize(node.Next, s);
                }
                if (node.Random != null)
                {
                    Serialize(node.Random, s);
                }
            }
        }

        public static ListNode? Deserialize(ListNode? node, Stream s)
        {
            var n = s.Read<ListNode>();

            if (node != null)
            {
                if (node.Previous != null)
                {
                    Serialize(node.Previous, s);
                }
                if (node.Next != null)
                {
                    Serialize(node.Next, s);
                }
                if (node.Random != null)
                {
                    Serialize(node.Random, s);
                }
            }

            return node;
        }
    }

    public class ListRandom
    {
        public ListNode Head;
        public ListNode Tail;
        public int Count;

        public void Serialize(Stream s)
        {
            s.Write(Count);

            if (Head != null)
            {
                ListNode.Serialize(Head, s);
            }

            if (Tail != null)
            {
                ListNode.Serialize(Tail, s);
            }
        }

        public void Deserialize(Stream s)
        {
            if (s.Read<int>() is int count)
            {
                Count = count;
            }

            ListNode.Deserialize(Head, s);
            ListNode.Deserialize(Tail, s);
        }
    }

}
