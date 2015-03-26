using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace RefundMod
{
    public class RefundModData
    {
        private const string Path = "refund.settings.xml";

        public bool Shrunk;

        public bool OnlyWhenPaused;
        public bool RemoveTimeLimit;

        private float _refundModifier;
        public float RefundModifier
        {
            get { return _refundModifier; }
            set { _refundModifier = (float)Math.Round(Mathf.Clamp(value, -1, 1), 1); }
        }

        private float _relocateModifier;
        public float RelocateModifier
        {
            get { return _relocateModifier; }
            set { _relocateModifier = (float)Math.Round(Mathf.Clamp01(value), 1); }
        }

        public float X;
        public float Y;

        public RefundModData(bool shrunk, bool onlyWhenPaused, bool removeTimeLimit, float refundModifier, float relocateModifier, float x, float y)
        {
            Shrunk = shrunk;
            OnlyWhenPaused = onlyWhenPaused;
            RemoveTimeLimit = removeTimeLimit;
            RefundModifier = refundModifier;
            RelocateModifier = relocateModifier;
            X = x;
            Y = y;
        }

        public RefundModData() : this(false, false, false, 0.75f, 0.2f, 60, 20) { }

        public void Serialize()
        {
            using (var stream = File.CreateText(Path))
            {
                var serializer = new XmlSerializer(typeof(RefundModData));
                serializer.Serialize(stream, this);
            }
        }

        public static RefundModData Deserialize()
        {
            RefundModData data;

            using (var stream = File.OpenRead(Path))
            {
                var serializer = new XmlSerializer(typeof(RefundModData));
                data = (RefundModData)serializer.Deserialize(stream);
            }

            return data;
        }
    }
}
