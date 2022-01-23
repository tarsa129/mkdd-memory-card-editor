using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class MemCard
    {
        public byte[] PreservedBlock { get; set; }
        public byte[] NewBlock { get; set; }
        public byte[] HeaderLine { get; set; }
        public byte[] PreservedEnd { get; set; }
        public byte[] NewEnd { get; set; }
        public bool firstispres { get; set; }
        public uint NewIndex { get; set; }

        public MemCard(byte[] Part1, byte[] Part2, bool preserve, byte[] Head){
            if (preserve)
            {
                this.PreservedBlock = Part1;

            } else
            {
                this.PreservedBlock = Part2;
            }
            NewBlock = new byte[0x2010];
            HeaderLine = Head;
        }

        public MemCard()
        {
            PreservedBlock = new byte[0x2010];
            NewBlock = new byte[0x2010];
            HeaderLine = new byte[0x10];
            PreservedEnd= new byte[0x10];
            NewEnd = new byte[0x10];
            firstispres = false;
        }

    }
}
