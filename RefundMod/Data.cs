using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace RefundMod
{
    public class Data
    {
        private const string Path = "refund.settings.xml";
        private bool _needsValidation;
        private static Data _instance;
        public static Data ModData => (_instance == null) ? (_instance = Load()) : _instance;

        private bool _removeTimeLimit;
        public bool RemoveTimeLimit
        {
            get { return _removeTimeLimit; }
            set
            {
                _removeTimeLimit = value;
                if (value) _onlyWhenPaused = false;
            }
        }

        private bool _onlyWhenPaused;
        public bool OnlyWhenPaused
        {
            get { return _onlyWhenPaused; }
            set
            {
                _onlyWhenPaused = value;
                if (value) _removeTimeLimit = false;
            }
        }

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

        public Data(bool removeTimeLimit, bool onlyWhenPaused, float refundModifier, float relocateModifier)
        {
            RemoveTimeLimit = removeTimeLimit;
            OnlyWhenPaused = onlyWhenPaused;
            RefundModifier = refundModifier;
            RelocateModifier = relocateModifier;
        }

        public Data() : this(false, false, 0.75f, 0.2f) { }

        public void Invalidate()
        {
            _needsValidation = true;
        }

        public bool Validated()
        {
            if (_needsValidation)
            {
                _needsValidation = false;
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)2166136261;

                hash = (hash * 16777619) ^ RemoveTimeLimit.GetHashCode();
                hash = (hash * 16777619) ^ OnlyWhenPaused.GetHashCode();
                hash = (hash * 16777619) ^ RefundModifier.GetHashCode();
                hash = (hash * 16777619) ^ RelocateModifier.GetHashCode();

                return hash;
            }
        }

        public void Save()
        {
            using (var stream = File.CreateText(Path))
            {
                var serializer = new XmlSerializer(typeof(Data));
                serializer.Serialize(stream, this);
            }
        }

        static Data Load()
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
