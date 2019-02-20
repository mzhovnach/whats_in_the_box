using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.Collections.Generic;
using System;
#if UNITY_IOS || UNITY_TVOS
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.PBX;
#endif
using System.IO;

public class ZagravaPostBuilder : MonoBehaviour
{
    internal static void CopyAndReplaceDirectory(string srcPath, string dstPath)
    {
        if (Directory.Exists(dstPath))
        {
            Directory.Delete(dstPath, true);
        }

        if (File.Exists(dstPath))
        {
            File.Delete(dstPath);
        }

        Directory.CreateDirectory(dstPath);

        foreach (var file in Directory.GetFiles(srcPath))
        {
            if (!file.EndsWith("meta"))
            {
                File.Copy(file, Path.Combine(dstPath, Path.GetFileName(file)));
            }
        }

        foreach (var dir in Directory.GetDirectories(srcPath))
        {
            CopyAndReplaceDirectory(dir, Path.Combine(dstPath, Path.GetFileName(dir)));
        }
    }

    static void CopyAndReplaceFile(string srcPath, string dstPath)
    {
        string dirName = Path.GetDirectoryName(dstPath);
        if (!Directory.Exists(dirName))
        {
            Directory.CreateDirectory(dirName);
        }

        if (File.Exists(dstPath))
        {
            File.Delete(dstPath);
        }

        File.Copy(srcPath, dstPath);
    }

    [PostProcessBuild(2000)]
    private static void onPostProcessBuildPlayer(BuildTarget buildTarget, string path)
    {
#if UNITY_IOS || UNITY_TVOS
        if( buildTarget == BuildTarget.iOS )
		{
            //string entitlementFileName = "greedubunnies_dev_pushnotifications.entitlement";
            //string entitlementPath = "Apple Certificates/" + entitlementFileName;
            //CopyAndReplaceFile(entitlementPath, Path.Combine(path , entitlementFileName));

            //string xcodeprojPath = path + "/Unity-iPhone.xcodeproj";
			//string projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";

			//PBXProject proj = new PBXProject();
			//proj.ReadFromString(File.ReadAllText(projPath));

			//string target = proj.TargetGuidByName("Unity-iPhone");

            //proj.AddFrameworkToProject(target, "UserNotifications.framework", false);
			//proj.AddFrameworkToProject(target, "StoreKit.framework", false);
            //proj.AddCapability(target, PBXCapabilityType.PushNotifications, entitlementFileName);

			//File.WriteAllText(projPath, proj.WriteToString());

            // Update info.plist
            // Get plist
            string plistPath = path + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));
            // Get plist root
            PlistElementDict rootDict = plist.root;

            //rootDict.SetString("NSLocationWhenInUseUsageDescription", "Location used to inform you when doctor/client is here.");
            rootDict.SetString("NSCameraUsageDescription", "Camera is used to scan QR code.");

            // enablde background mode for remote notifications
            //var buildKey = "UIBackgroundModes";
            //rootDict.CreateArray(buildKey).AddString("remote-notification");

            // Write to file
            File.WriteAllText(plistPath, plist.WriteToString());
		}
#endif
	}
}
