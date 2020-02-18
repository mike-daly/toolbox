using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace host_ironpython
{
    class SampleWatermarkProvider
    {
        private int lastValue;

        public SampleWatermarkProvider()
        {
            this.lastValue = 0;
        }
        public void UpdateWatermark(int nextWatermark)
        {
            Console.WriteLine("UpdateWatermark():  {0},  previous:  {1}", nextWatermark, this.lastValue);
            this.lastValue = nextWatermark;
        }


    }

    class SampleMessage
    {
        public string message;
        public int offset;

        public SampleMessage()
        {
            this.message = string.Empty;
            this.offset = new Random().Next();
        }

        public void SampleFunction()
        {
            Console.WriteLine("sample function to make this more than a simple struct.");
        }
    }
}
