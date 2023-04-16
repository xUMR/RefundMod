using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace RefundMod
{
    public sealed class Data
    {
        public static readonly Data Default = new Data();

        private bool _needsValidation;

        private bool _removeTimeLimit;

        public bool RemoveTimeLimit
        {
            get => _removeTimeLimit;
            set
            {
                _removeTimeLimit = value;
                if (value) _onlyWhenPaused = false;
            }
        }

        private bool _onlyWhenPaused;

        public bool OnlyWhenPaused
        {
            get => _onlyWhenPaused;
            set
            {
                _onlyWhenPaused = value;
                if (value) _removeTimeLimit = false;
            }
        }

        public event Action<bool> OnDisableOtherEconomyModsSet;
        private bool _disableOtherEconomyMods;

        public bool DisableOtherEconomyMods
        {
            get => _disableOtherEconomyMods;
            set
            {
                if (_disableOtherEconomyMods == value) return;

                _disableOtherEconomyMods = value;

                OnDisableOtherEconomyModsSet?.Invoke(value);
            }
        }

        private float _refundModifier;

        public float RefundModifier
        {
            get => _refundModifier;
            set => _refundModifier = (float)Math.Round(Mathf.Clamp(value, -1, 1), 2);
        }

        private float _relocateModifier;

        public float RelocateModifier
        {
            get => _relocateModifier;
            set => _relocateModifier = (float)Math.Round(Mathf.Clamp01(value), 2);
        }

        public Data(
            bool removeTimeLimit,
            bool onlyWhenPaused,
            bool disableOtherEconomyMods,
            float refundModifier,
            float relocateModifier)
        {
            RemoveTimeLimit = removeTimeLimit;
            OnlyWhenPaused = onlyWhenPaused;
            DisableOtherEconomyMods = disableOtherEconomyMods;
            RefundModifier = refundModifier;
            RelocateModifier = relocateModifier;
        }

        public Data() : this(
            false,
            false,
            false,
            0.75f,
            0.2f) { }


        public void Invalidate()
        {
            _needsValidation = true;
        }

        public bool Validate()
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
                hash = (hash * 16777619) ^ DisableOtherEconomyMods.GetHashCode();
                hash = (hash * 16777619) ^ RefundModifier.GetHashCode();
                hash = (hash * 16777619) ^ RelocateModifier.GetHashCode();

                return hash;
            }
        }

        public sealed class Persistence
        {
            private const string PATH = "refund.settings.xml";
            private readonly XmlSerializer _serializer = new XmlSerializer(typeof(Data));

            public Data Data { get; private set; }

            public Persistence()
            {
                Load();
            }

            public void Save()
            {
                using var stream = File.CreateText(PATH);
                _serializer.Serialize(stream, Data);
            }

            public void Load()
            {
                if (!File.Exists(PATH))
                {
                    Data = new Data();
                    Save();
                    return;
                }

                using var stream = File.OpenRead(PATH);
                Data = (Data)_serializer.Deserialize(stream);
            }
        }
    }
}
