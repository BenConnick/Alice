#if UNITY_EDITOR
using UnityEditor;
#endif

public static class NativePopupHelper
{
    public static void ShowTestPopup()
    {
        /*Process process = new Process();
        ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        startInfo.FileName = "Terminal";
        startInfo.Arguments = "osascript -e 'display dialog \"Hello\" with title \"Hello\"'";
        process.StartInfo = startInfo;
        process.Start();*/
        UnityEngine.Debug.Log("Starting process");

        //using (Process myProcess = new Process())
        //{
        //    myProcess.StartInfo = new ProcessStartInfo("/Applications/Utilities/Terminal.app/Contents/MacOS/Terminal");
        //    myProcess.StartInfo.UseShellExecute = false;
        //    // You can start any process, HelloWorld is a do-nothing example.
        //    myProcess.StartInfo.Arguments = " -c \"osascript -e 'display dialog \"Hello\" with title \'Hello\''\"";
        //    myProcess.StartInfo.CreateNoWindow = true;
        //    myProcess.Start();
        //    // This code assumes the process you are starting will terminate itself.
        //    // Given that it is started without a window so you cannot terminate it
        //    // on the desktop, it must terminate itself or you can do it programmatically
        //    // from this application using the Kill method.
        //}
#if UNITY_EDITOR
        EditorUtility.DisplayDialog(" ", "♥️", "ACCEPT", "REJECT");
#elif UNITY_WEBGL
        
#endif
    }
}
