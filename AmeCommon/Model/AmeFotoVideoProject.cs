﻿using System;
using System.Collections.Generic;

namespace AmeCommon.Model
{
    public class AmeFotoVideoProject
    {
        public int Id { get; set; }
        public string UniqueName => $"{EventDate:yyyy-MM-dd} {Name}";
        public DateTime EventDate { get; set; }
        public string Name { get; set; }
        public List<MediaFile> MediaFiles { get; set; } = new List<MediaFile>();
        public string LocalPathRoot { get; set; }

        public void AppendMediaFiles(IEnumerable<MediaFile> newFiles)
        {
            foreach (var newFile in newFiles)
            {
                MediaFiles.RemoveAll(mf => mf.RelativePath == newFile.RelativePath);
                MediaFiles.Add(newFile);
            }
        }
    }
}