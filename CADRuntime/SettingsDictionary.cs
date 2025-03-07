﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CADRuntime
{
    /// <summary>
    /// A subclass of Dictionary<string, string>, that writes changes back to a settings xml file.
    /// </summary>
    public class SettingsDictionary : IDictionary<string, string>
    {
        private readonly IDictionary<string, string> _dict;
        private readonly string _settingsPath;
        private XDocument _settings;
        private string nameVariable = "StringVariable";
        public SettingsDictionary(string settingsPath)
        {
            _settingsPath = settingsPath;
            _settings = XDocument.Load(_settingsPath);

            _dict = _settings.Root.Descendants(nameVariable).ToDictionary(
                v => v.Attribute("name").Value,
                v => v.Attribute("value").Value);
        }

        private void SetVariable(string name, string value)
        {
            var variable = _settings.Root.Descendants(nameVariable).Where(x => x.Attribute("name").Value == name).FirstOrDefault();
            if (variable != null)
            {
                variable.Attribute("value").Value = value.ToString();
            }
            else
            {
                _settings.Root.Descendants("Variables").First().Add(
                    new XElement(nameVariable, new XAttribute("name", name), new XAttribute("value", value)));
            }
            _settings.Save(_settingsPath);
        }

        private void RemoveVariable(string name)
        {
            var variable = _settings.Root.Descendants(nameVariable).Where(x => x.Attribute("name").Value == name).FirstOrDefault();
            if (variable != null)
            {
                variable.Remove();
                _settings.Save(_settingsPath);
            }
        }

        private void ClearVariables()
        {
            var variables = _settings.Root.Descendants(nameVariable);
            foreach (var variable in variables)
            {
                variable.Remove();
            }
            _settings.Save(_settingsPath);
        }

        public void Add(string key, string value)
        {
            _dict.Add(key, value);
            SetVariable(key, value);
        }

        public bool ContainsKey(string key)
        {
            return _dict.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return _dict.Keys; }
        }

        public bool Remove(string key)
        {
            RemoveVariable(key);
            return _dict.Remove(key);
        }

        public bool TryGetValue(string key, out string value)
        {
            return _dict.TryGetValue(key, out value);
        }

        public ICollection<string> Values
        {
            get { return _dict.Values; }
        }

        public string this[string key]
        {
            get
            {
                return _dict[key];
            }
            set
            {
                _dict[key] = value;
                SetVariable(key, value);
            }
        }

        public void Add(KeyValuePair<string, string> item)
        {
            _dict.Add(item);
            SetVariable(item.Key, item.Value);
        }

        public void Clear()
        {
            ClearVariables();
            _dict.Clear();
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            return _dict.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            _dict.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _dict.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            RemoveVariable(item.Key);
            return _dict.Remove(item);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _dict.GetEnumerator();
        }
    }
}