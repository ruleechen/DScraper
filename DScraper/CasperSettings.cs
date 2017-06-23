﻿using System;
using System.Collections.Generic;
using System.IO;

namespace DScraper
{
    public class CasperSettings
    {
        public string CasperjsExePath { get; set; }

        private static CasperSettings _fromDetection;
        public static CasperSettings FromDetection
        {
            get
            {
                if (_fromDetection == null)
                {
                    _fromDetection = new CasperSettings
                    {
                        CasperjsExePath = GetCasperjsExePath()
                    };
                }

                return _fromDetection;
            }
        }

        private static string GetCasperjsExePath()
        {
            var element = ScraperExtensions.Configuration.AppSettings.Settings["DScraper.CasperjsExePath"];

            if (element != null)
            {
                return element.Value;
            }
            else
            {
                return ScraperExtensions.GetExecutableFullPath("casperjs.exe") ??
                       ScraperExtensions.GetExecutableFullPath("bin\\casperjs.exe");
            }
        }

        private const string VerificationCacheKey = "DSCRAPER_ENVIRONMENT_VERIFICATION";

        public static void VerifyEnvironment()
        {
            if ((string)AppDomain.CurrentDomain.GetData(VerificationCacheKey) == "1")
            {
                return;
            }

            var executables = new List<string>
            {
                "python.exe",
                "phantomjs.exe",
                "casperjs.exe"
            };

            foreach (var file in executables)
            {
                var path = ScraperExtensions.GetExecutableFullPath(file);

                if (string.IsNullOrEmpty(path))
                {
                    throw new FileNotFoundException("Environment can not found executable file: " + file);
                }
            }

            AppDomain.CurrentDomain.SetData(VerificationCacheKey, "1");
        }
    }
}