﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AssetConverter
{
    public class DLG : IEAsset
    {
        private string _dPath;
        private string _traPath;
        public DLG(string preConversionPath, string postConversionPath, IEResRef resRef) : base(preConversionPath, postConversionPath, resRef)
        {
            Weidu.DecompileDialog(this);
            ResourceManager.RegisterDialog(_owningReference.OldReferenceID.ToLower(), _owningReference.NewReferenceID.ToLower());
        }

        public void SetComponentPaths(string dpath, string traPath)
        {
            _dPath = dpath;
            _traPath = traPath;
        }

        public void ReplaceDReferences()
        {
            string beginFlag = "BEGIN ~";
            string externFlag = "EXTERN ~";
            bool changeMade = false;
            string[] lineContents = File.ReadAllLines(_dPath);
            bool beginFlagFound = false;
            for (int i = 0; i < lineContents.Length; i++)
            {
                string currentLine = lineContents[i];
                bool lineFlagFound = false;
                if (currentLine.Length > 8)
                {
                    string reference = "";
                    if (!beginFlagFound)
                    {
                        if (currentLine.StartsWith(beginFlag, StringComparison.Ordinal))
                        {
                            reference = currentLine.Split(beginFlag)[1].ToLower();
                            beginFlagFound = true;
                            lineFlagFound = true;
                        }
                    }
                    if (!lineFlagFound)
                    {
                        if (currentLine.Contains(externFlag))
                        {
                            reference = currentLine.Split(externFlag)[1].ToLower();
                            lineFlagFound = true;
                        }
                    }
                    if (lineFlagFound)
                    {
                        reference = reference.Split("~")[0];
                        string newReference = "";
                        if (ResourceManager.GetNewDialogReference(reference, ref newReference))
                        {
                            lineContents[i] = currentLine.Replace(reference.ToUpper(), newReference.ToUpper());
                            changeMade = true;
                        }
                    }
                }

            }
            if (changeMade)
            {
                File.WriteAllLines(_dPath, lineContents);
            }
        }
    }
}
