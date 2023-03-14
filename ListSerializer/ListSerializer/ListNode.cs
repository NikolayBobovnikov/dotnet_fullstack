
using System;
using System.Diagnostics;
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
            where T : struct
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
            where T : struct
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

            var flatten = Flatten(Head);
            foreach(var flat in flatten)
            {
                s.WriteMarshal(flat);
            }
        }

        public void Deserialize(Stream s)
        {
            // Read flatten list of ListNodeFlat's from stream
            // Restore pointers to Random nodes from positions in flatten structure
        }

        #region Public types

        /// <summary>
        /// Intermediary class for storing instead of ListNode.
        /// </summary>
        public struct ListNodeFlat
        {
            public ListNodeFlat(ListNodeFlat other)
            {
                PrevPosition = other.PrevPosition;
                NextPosition = other.NextPosition;
                Random = other.Random;
                Data = other.Data;
            }

            public int PrevPosition = NULL_POS;
            public int NextPosition = NULL_POS;
            public int Random = NULL_POS;
            public string Data;
        }

        #endregion

        #region Public methods

        public static ListRandom RestoreFromFlatten(IList<ListNodeFlat> flatten)
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

        public static IList<ListNodeFlat> Flatten(ListNode linkedList)
        {
            // result flatten list
            var flatten = new List<ListNodeFlat>();

            // aux data to get node by position
            var nodes = new List<ListNode>();

            // mapping between nodes and positions
            var nodeToPositionDict = new Dictionary<ListNode, int>();


            // prepare aux data (contiguous list of nodes and mapping to node positions)
            var currentNode = linkedList;
            int index = 0;
            while (currentNode != null)
            {
                // add flatten node to list (head and tail are corrected in the end)
                nodes.Add(currentNode);

                // store map between node and its position
                nodeToPositionDict.Add(currentNode, index);

                // go to next node
                currentNode = currentNode.Next;
                index++;
            }


            // fill flatten list
            index = 0;
            foreach (var node in nodes)
            {
                // get position of that node in the list corresponding to node's ptr
                var pos = node.Random == null ? NULL_POS : nodeToPositionDict[node.Random];

                // save position in flatten structure
                flatten.Add(new ListNodeFlat() 
                { 
                    Data = node.Data, 
                    PrevPosition = (index == 0 ? NULL_POS : index - 1), 
                    NextPosition = (index == nodes.Count - 1 ? NULL_POS : index + 1), 
                    Random = pos }
                );

                index++;
            }

            return flatten;
        }

        public static IList<ListNodeFlat> Flatten(ListRandom randomList)
        {
            // flatten list starting from head node
            var flatten = Flatten(randomList.Head);

            // verify
            Debug.Assert(flatten.Count == randomList.Count);

            return flatten;
        }

        #endregion

        #region Properties

        public static int NULL_POS => int.MinValue;

        #endregion
    }

}
