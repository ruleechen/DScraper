﻿using System.Configuration;
using System.Web;
using System.Web.Configuration;

namespace DScraper
{
    public static class DScraperExtensions
    {
        private static Configuration _current;
        public static Configuration Current
        {
            get
            {
                if (_current == null)
                {
                    if (HttpContext.Current.IsAvailable())
                    {
                        _current = WebConfigurationManager.OpenWebConfiguration("~");
                    }
                    else
                    {
                        _current = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    }
                }

                return _current;
            }
            set
            {
                _current = value;
            }
        }

        public static bool IsAvailable(this HttpContext context)
        {
            return context != null && context.Handler != null;
        }
    }
}