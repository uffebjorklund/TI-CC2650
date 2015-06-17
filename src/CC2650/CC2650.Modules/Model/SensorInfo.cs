using System;

namespace CC2650.Modules.Model
{
    public class SensorInfo
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public TempModel lastValue { get; set; }

        public SensorInfo(string n)
        {
            this.lastValue = new TempModel{amb = 0, obj = 0};
            this.name = n;
        }
    }
}