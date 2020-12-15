using UnityEngine;
using System.Text;

public class DebugInformation : MonoBehaviour
{
    StringBuilder stringBuilder = new StringBuilder();
    void OnGUI ()
    {
        float fps = 1f / Time.deltaTime;
        stringBuilder.Clear();
        stringBuilder.AppendFormat("FPS : {0:f1}", fps);

        GUI.TextArea(new Rect(16, 16, 84, 24), stringBuilder.ToString());
    }
}
