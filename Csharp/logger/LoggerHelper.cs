// ---------------------------------------------------------------
// 
// Copyright © Microsoft. All rights reserved.
// 
// ---------------------------------------------------------------

namespace Microsoft.Msn.Utilities.Logging
{
    using Windows.Foundation.Diagnostics;

    public static class LoggerHelper
    {
        public static void RegisterLogSettings()
        {
            #if !(PROD_BUILD)
                #if (DEBUG)
                        var debugLogger = Logger.Instance;
                        debugLogger.IsEnabled = true;
                        debugLogger.AddListener(new FileLogListener(LoggingConstants.LogFileName, "Threshold_DebugSession"));
                        debugLogger.AddListener(new DebugListener());
                        debugLogger.AddListener(new ETWListener());
                        debugLogger.SetMinLogLevel(LoggingLevel.Verbose);
                #endif

                #if (DEV_BUILD)
                        var filelogger = Logger.Instance;
                        filelogger.IsEnabled = true;
                        filelogger.AddListener(new FileLogListener("ThresholdLog.etl", "Threshold_DevSession"));
                        filelogger.AddListener(new ETWListener());
                        filelogger.SetMinLogLevel(LoggingLevel.Warning);
                #endif

                #if (TEST_BUILD)
                        var logger = Logger.Instance;
                        logger.IsEnabled = true;
                        logger.AddListener(new FileLogListener("ThresholdLog.etl", "Threshold_TestSession"));
                        logger.AddListener(new ETWListener());
                        logger.SetMinLogLevel(LoggingLevel.Error);  
                #endif
                #if (DOGFOOD_BUILD)
                        var logger = Logger.Instance;
                        logger.IsEnabled = true;
                        logger.AddListener(new FileLogListener("ThresholdLog.etl", "Threshold_DogfoodSession"));
                        logger.AddListener(new ETWListener());
                        logger.SetMinLogLevel(LoggingLevel.Critical);   
                #endif
        #else
                var logger = Logger.Instance;
                logger.IsEnabled = false;
        #endif
        }
    }
}
