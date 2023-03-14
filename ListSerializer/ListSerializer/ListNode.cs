
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

namespace ListSerializer
{
    /// <summary>
    /// Extensions for stream IO.
    /// </summary>
    public static class Extensions
    {
        public static void WriteMarshal<T>(this Stream stream, T value)
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

        public static T ReadMarshal<T>(this Stream stream)
            where T : unmanaged
        {
            T result = default;
            int size = Marshal.SizeOf(result);
            byte[] managedArray = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            stream.Read(managedArray, 0, size);
            Marshal.Copy(managedArray, 0, ptr, size);
            result = Marshal.PtrToStructure<T>(ptr);

            return result;
        }

        public static void Write<T>(this Stream stream, T value)
        where T : unmanaged
            {
                var tSpan = MemoryMarshal.CreateSpan(ref value, 1);
                var span = MemoryMarshal.AsBytes(tSpan);
                stream.Write(span);
            }

        public static T Read<T>(this Stream stream)
        where T : unmanaged
            {
                var result = default(T);
                var tSpan = MemoryMarshal.CreateSpan(ref result, 1);
                var span = MemoryMarshal.AsBytes(tSpan);
                stream.Read(span);
                return result;
            }
    }

    /// <summary>
    /// Class represents node of the ListRandom linked list.
    /// </summary>
    public class ListNode
    {
        public ListNode Previous;
        public ListNode Next;
        public ListNode Random;
        public string Data;
    }

    /// <summary>
    /// Class represents double linked list with Count elements, containing Head and Tail nodes on the left/right sides correspondently.
    /// </summary>
    public class ListRandom
    {
        public ListNode Head;
        public ListNode Tail;
        public int Count;

        #region Public methods

        #endregion
        public void Serialize(Stream s)
        {
            // Go through all nodes and get their positions
            // Map pointers to positions to resolve Random
            // Map ListNode class to ListNodeFlat structure containing node positions instead of pointers
            // Write flatten list of ListNodeFlat's into stream
        }

        public void Deserialize(Stream s)
        {
            // Read flatten list of ListNodeFlat's from stream
            // Restore pointers to Random nodes from positions in flatten structure
        }

        #region Private types

        /// <summary>
        /// Intermediary class for storing instead of ListNode.
        /// </summary>
        private struct ListNodeFlat
        {
            public int PreviousPosition;
            public int NextPosition;
            public int Random;
            public string Data;
        }

        #endregion

        #region Private methods

        private static ListRandom _RestoreFromFlatten(IList<ListNodeFlat> flatten)
        {
            // create ListNodes from corresponding flatten elements and store them into list
            var nodes = flatten.Select(x => new ListNode() { Data = x.Data }).ToList();

            // restore node correspondence
            foreach (var (node, index) in nodes.Select((value, i) => (value, i)))
            {
                node.Random = nodes[index];
            }

            // construct result structure
            return new ListRandom()
            {
                Head = nodes.First(),
                Tail = nodes.Last(),
                Count = nodes.Count()
            };
        }

        #endregion
    }

}
