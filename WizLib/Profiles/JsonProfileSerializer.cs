﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace WizLib.Profiles
{
    public class JsonProfileSerializer : IProfileSerializer
    {
        public string Filename { get; set; }

        public static string DefaultExtension { get; set; } = ".wizj";

        public static string LastDirectory { get; set; }

        public string[] EnumProfiles()
        {
            return EnumProfiles(LastDirectory);
        }

        public static string[] EnumProfiles(string path, string ext = null, bool setLastDir = true)
        {
            if (ext == null)
                ext = DefaultExtension;

            if (!Directory.Exists(path)) throw new DirectoryNotFoundException(path);
            if (setLastDir) LastDirectory = path;

            if (string.IsNullOrEmpty(ext))
            {
                return Directory.GetFiles(path);
            }
            else
            {
                if (ext[0] != '.') ext = '.' + ext;
                ext = "*" + ext;

                return Directory.GetFiles(path, ext);
            }
        }

        public JsonProfileSerializer(string fileName)
        {
            Filename = fileName;
        }

        public IProfile Deserialize()
        {
            if (!File.Exists(Filename)) throw new FileNotFoundException(nameof(Filename));
            string json = File.ReadAllText(Filename);

            IProfile p = new Profile();

            JsonConvert.PopulateObject(json, p, BulbCommand.DefaultJsonSettings);

            foreach (var bulb in p.Bulbs)
            {
                bulb.GetBulb().Wait();
            }

            return p;
        }

        public void Deserialize(IProfile obj)
        {
            if (!File.Exists(Filename)) throw new FileNotFoundException(nameof(Filename));
            string json = File.ReadAllText(Filename);

            JsonConvert.PopulateObject(json, obj, BulbCommand.DefaultJsonSettings);

            foreach (var bulb in obj.Bulbs)
            {
                bulb.GetBulb().Wait();
            }
        }

        public void Serialize(IProfile profile)
        {
            string json = JsonConvert.SerializeObject(profile, BulbCommand.DefaultJsonSettings);

            if (string.IsNullOrEmpty(Filename)) throw new NullReferenceException(nameof(Filename));
            File.WriteAllText(Filename, json);
        }

    }
}