﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Dynamsoft.Barcode;

namespace ReadBarcodeFromRegion
{
    class Program
    {
        private static TextResult[] DecodeFile(string strImagePath, int mode, int iLeft, int iTop, int iRight, int iBottom)
        {

            BarcodeReader _br = new BarcodeReader();
            _br.ProductKeys = "t0068MgAAAEUWFzAvIFjWdsOhURov3SljTtFakKFsHemq+2NKnvb5tEihIDmWlZsFpCWpVOnWr1Uw1NzIQ2EcnLj9Hxxvjfs=";

            string strErrorMSG = "";
            //Best coverage settings
            _br.InitRuntimeSettingsWithString("{\"ImageParameter\":{\"Name\":\"BestCoverage\",\"DeblurLevel\":9,\"ExpectedBarcodesCount\":512,\"ScaleDownThreshold\":100000,\"LocalizationModes\":[{\"Mode\":\"LM_CONNECTED_BLOCKS\"},{\"Mode\":\"LM_SCAN_DIRECTLY\"},{\"Mode\":\"LM_STATISTICS\"},{\"Mode\":\"LM_LINES\"},{\"Mode\":\"LM_STATISTICS_MARKS\"}],\"GrayscaleTransformationModes\":[{\"Mode\":\"GTM_ORIGINAL\"},{\"Mode\":\"GTM_INVERTED\"}]}}", EnumConflictMode.CM_OVERWRITE, out strErrorMSG);
            //Best speed settings
            //_br.InitRuntimeSettingsWithString("{\"ImageParameter\":{\"Name\":\"BestSpeed\",\"DeblurLevel\":3,\"ExpectedBarcodesCount\":512,\"LocalizationModes\":[{\"Mode\":\"LM_SCAN_DIRECTLY\"}],\"TextFilterModes\":[{\"MinImageDimension\":262144,\"Mode\":\"TFM_GENERAL_CONTOUR\"}]}}", EnumConflictMode.CM_OVERWRITE, out strErrorMSG);
            //Balance settings
            //_br.InitRuntimeSettingsWithString("{\"ImageParameter\":{\"Name\":\"Balance\",\"DeblurLevel\":5,\"ExpectedBarcodesCount\":512,\"LocalizationModes\":[{\"Mode\":\"LM_CONNECTED_BLOCKS\"},{\"Mode\":\"LM_STATISTICS\"}]}}", EnumConflictMode.CM_OVERWRITE, out strErrorMSG);


            string[] strTemplateNameArray = { "All_DEFAULT", "All_DEFAULT_WITHREGION" };
            string tempDefaultTemplateJson = "{\"Version\": \"2.0\",\"ImageParameter\": {\"Name\": \"" + strTemplateNameArray[0] + "\",\"BarcodeFormatIds\": [\"BF_ALL\"],\"RegionPredetectionModes\": [{\"Mode\":\"RPM_GENERAL_RGB_CONTRAST\"}]}}";
            string tempTemplateJsonWithRegion = "{\"Version\": \"2.0\",\"ImageParameter\": {\"Name\": \"" + strTemplateNameArray[1] + "\",\"BarcodeFormatIds\": [\"BF_ALL\"],\"RegionPredetectionModes\": [{\"Mode\":\"RPM_GENERAL\"}],\"RegionDefinitionNameArray\": [\"Region\"]},\"RegionDefinitionArray\": [{\"Name\": \"Region\",\"MeasuredByPercentage\": 1" + ",\"Left\":" + iLeft.ToString() + ",\"Top\":" + iTop.ToString() + ",\"Right\":" + iRight.ToString() + ",\"Bottom\":" + iBottom.ToString() + "}]}";

            if (mode == 0)
            {
                // load template json as a string.
                string errorstring = "";
                EnumErrorCode temperrorcode = _br.AppendTplStringToRuntimeSettings(tempDefaultTemplateJson, EnumConflictMode.CM_OVERWRITE, out errorstring);
            }
            else
            {
                // load template json as a string.
                string errorstring = "";
                EnumErrorCode temperrorcode = _br.AppendTplStringToRuntimeSettings(tempTemplateJsonWithRegion, EnumConflictMode.CM_OVERWRITE, out errorstring);

            }

            TextResult[] result = _br.DecodeFile(strImagePath, "");

            return result;

        }

        private static string GeneratorOutputTextResult(TextResult[] result)
        {
            string builder = null;
            builder += "\r\nBarcode Results:\r\n----------------------------------------------------------\r\n";
            if (result == null)
            {
                string outputInfo = "Total barcode(s) found: 0.\r\n\r\n";
                builder += outputInfo;
            }
            else
            {
                string outputInfo = "Total barcode(s) found: " + result.Length + ".\r\n\r\n";
                builder += outputInfo;
                for (int iIndex = 0; iIndex < result.Length; iIndex++)
                {

                    int iBarcodeIndex = iIndex + 1;
                    builder += "Barcode " + iBarcodeIndex.ToString() + ":\r\n";
                    if (result[iIndex].BarcodeFormat != 0)
                    {
                        builder += "    Type: " + result[iIndex].BarcodeFormatString + "\r\n";
                    }
                    else
                    {
                        builder += "    Type: " + result[iIndex].BarcodeFormatString_2 + "\r\n";
                    }
                    builder += "    Value: " + result[iIndex].BarcodeText + "\r\n";
                    builder += "    Hex Data: " + ToHexString(result[iIndex].BarcodeBytes) + "\r\n";
                }
            }
            return builder.ToString();
        }

        private static bool GetImagePath(ref string strImagePath)
        {
            while (true)
            {
                Console.WriteLine("\r\n>> Step 1: Input your image file's full path:\r\n");
                string tempInput = Console.ReadLine();
                if (tempInput.Length > 0)
                {
                    if (tempInput == "q" || tempInput == "Q")
                    {
                        strImagePath = null;
                        return true;
                    }
                }
                strImagePath = tempInput.Replace("\\", "\\\\");
                strImagePath = strImagePath.Replace("\"", "");
                if (!File.Exists(strImagePath))
                {
                    Console.WriteLine("Please input a valid path.\r\n");
                    continue;
                }
                return false;
            }
        }

        private static bool SelectMode(ref int mode, ref int iLeft, ref int iTop, ref int iRight, ref int iBottom)
        {
            bool bExitFlag = false;
            iLeft = -1;
            iTop = -1;
            iRight = -1;
            iBottom = -1;
            while (true)
            {
                Console.WriteLine("Whether to set the detection area(Y:Yes/N:NO)?");
                string tempInput = Console.ReadLine();
                if (tempInput.Length > 0)
                {
                    if (tempInput == "q" || tempInput == "Q")
                    {
                        bExitFlag = true;
                        return bExitFlag;
                    }
                    else if (tempInput == "N" || tempInput == "n")
                    {
                        mode = 0;
                        return bExitFlag;
                    }
                    else if (tempInput == "Y" || tempInput == "y")
                    {
			            mode = 1;
                        while (true)
                        {
                            Console.WriteLine("Set left, top, right, bottom value (in percentage) of your region rectangle (e.g:10,10,90,90):");
                            string tempRectString = Console.ReadLine();
                            if (tempInput == "q" || tempInput == "Q")
                            {
                                bExitFlag = true;
                                break;
                            }
                            string[] arrayRect = tempRectString.Split(',');
                            if (arrayRect.Length != 4)
                            {
                                Console.WriteLine("Please input a vaild rect.");
                            }
                            else
                            {
                                try
                                {
                                    iLeft = int.Parse(arrayRect[0]);
                                    iTop = int.Parse(arrayRect[1]);
                                    iRight = int.Parse(arrayRect[2]);
                                    iBottom = int.Parse(arrayRect[3]);
                                }
                                catch (Exception exp)
                                {
                                    Console.WriteLine(exp.Message);
                                    continue;
                                }
                                break;
                            }
                        }
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Please input a vaild value.");
                    }
                }
            }
            return bExitFlag;
        }

        private static string ToHexString(byte[] bytes)
        {
            string hexString = string.Empty;

            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    strB.Append(bytes[i].ToString("X2") + " ");
                }

                hexString = strB.ToString();

            }

            return hexString;
        }

        static void Main(string[] args)
        {

            Console.WriteLine("*************************************************\r\n");
            Console.WriteLine("Welcome to Dynamsoft Barcode Reader Demo\r\n");
            Console.WriteLine("*************************************************\r\n");
            Console.WriteLine("Hints: Please input 'Q'or 'q' to quit the application.\r\n");
            bool bExitFlag = false;
            while (true)
            {
                string strImagePath = null;
                bExitFlag = GetImagePath(ref strImagePath);
                if (bExitFlag)
                    break;
                int mode = 0, iLeft = -1, iTop = -1, iRight = -1, iBottom = -1;
                bExitFlag = SelectMode(ref mode, ref iLeft, ref iTop, ref iRight, ref iBottom);
                if (bExitFlag)
                    break;
                TextResult[] result = DecodeFile(strImagePath, mode, iLeft, iTop, iRight, iBottom);
                string outputTextResult = GeneratorOutputTextResult(result);
                Console.WriteLine(outputTextResult);

            }
        }
    }
}
