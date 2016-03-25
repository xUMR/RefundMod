using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace RefundMod
{
    public class Data
    {
        private const string Path = "refund.settings.xml";

        public bool RemoveTimeLimit { get; set; }

        private float _refundModifier;
        public float RefundModifier
        {
            get { return _refundModifier; }
            set { _refundModifier = (float)Math.Round(Mathf.Clamp(value, -1, 1), 2); }
        }

        private float _relocateModifier;
        public float RelocateModifier
        {
            get { return _relocateModifier; }
            set { _relocateModifier = (float)Math.Round(Mathf.Clamp01(value), 2); }
        }

        public Data(bool removeTimeLimit, float refundModifier, float relocateModifier)
        {
            RemoveTimeLimit = removeTimeLimit;
            RefundModifier = refundModifier;
            RelocateModifier = relocateModifier;
        }

        public Data() : this(false, 0.75f, 0.2f) { }

        public void Serialize()
        {
            using (var stream = File.CreateText(Path))
            {
                var serializer = new XmlSerializer(typeof(Data));
                serializer.Serialize(stream, this);
            }
        }

        public static Data Deserialize()
        {
            Data data;

            using (var stream = File.OpenRead(Path))
            {
                var serializer = new XmlSerializer(typeof(Data));
                data = (Data)serializer.Deserialize(stream);
            }

            return data;
        }
    }
}
