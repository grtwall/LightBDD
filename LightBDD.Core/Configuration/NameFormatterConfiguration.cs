﻿using System;
using LightBDD.Core.Formatting;

namespace LightBDD.Configuration
{
    public class NameFormatterConfiguration : IFeatureConfiguration
    {
        public NameFormatterConfiguration()
        {
            Formatter = new DefaultNameFormatter();
        }

        public INameFormatter Formatter { get; private set; }

        public NameFormatterConfiguration UpdateFormatter(INameFormatter formatter)
        {
            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));
            Formatter = formatter;
            return this;
        }
    }
}