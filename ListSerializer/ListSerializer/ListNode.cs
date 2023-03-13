
using System;
using System.Drawing;
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

    public struct ListNodeData
    {
        public string Data;
    }

    public class ListRandom
    {
        public ListNode Head;
        public ListNode Tail;
        public int Count;

        public void Serialize(Stream s)
        {
            //var v = default(ListNodeData);
            var v = new ListNodeData() { Data = "Hello" };
            int size = Marshal.SizeOf(v);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            try
            {
                Marshal.StructureToPtr(v, ptr, false);
                byte[] managedArray = new byte[size];
                Marshal.Copy(ptr, managedArray, 0, size);
                s.Write(managedArray, 0, size);
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(ptr);
            }
        }

        public void Deserialize(Stream s)
        {
            var v = default(ListNodeData);
            int size = Marshal.SizeOf(v);
            byte[] managedArray = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            s.Read(managedArray, 0, size);
            Marshal.Copy(managedArray, 0, ptr, size);
            v = Marshal.PtrToStructure<ListNodeData>(ptr);
        }
    }

}
