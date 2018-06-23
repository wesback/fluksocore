using System;
using System.Runtime.Serialization;
using System.Globalization;

namespace FluksoCore
{
    [DataContract(Name="sensor")]
    public class SensorData
    {
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private string _name;
        private string _topicName;

        [DataMember(Name="topicname")]
        public string TopicName 
        {
            get
            {
                return _topicName;
            }
            set
            {
                _topicName = value;
                deriveNameFromTopicName();
            }
        }

        [DataMember(Name="name")]
        public string Name 
        { 
            get
            {
                return _name;
            } 
            set
            {
                _name = value;
            }
        }

        [DataMember(Name="type")]
        public string type { get; set; }

        [DataMember(Name="value")]
        public double value { get; set; }

        [DataMember(Name="epochTime")]
        public long epochTime { get; set; }

        [DataMember(Name="timestamp")]
        public DateTime timeStamp
        {
            get
            {
                DateTime utcDate = epoch.AddSeconds(epochTime);
                utcDate = DateTime.SpecifyKind(utcDate, DateTimeKind.Utc);

                return utcDate.ToLocalTime();
            }
        }

        private void deriveNameFromTopicName()
        {
            if (_topicName.IndexOf(FluksoConfig.getConfig("Sensors:Electricity")) > 0)
            {
                _name = "Electricity";
            }
            else if (_topicName.IndexOf(FluksoConfig.getConfig("Sensors:Water")) > 0)
            {
                _name = "Water";
            }
             else if (_topicName.IndexOf(FluksoConfig.getConfig("Sensors:Gas")) > 0)
            {
                _name = "Gas";
            }
            else
                _name = "Unknown";
        }
    }
}