using UnityEngine;
using Unity.Profiling;
using System.Text;

namespace HTH.Tests
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 디버깅용 UI
    /// 수정자  : 권병석
    /// 수정일  : 2023_01_26
    /// 설명    : 안드로이드에서 로그를 찍기위해 싱글톤화 및 log1~3 추가
    /// </summary>
    public class StatisticsDebuggingUI : MonoBehaviour
    {
        public static StatisticsDebuggingUI instance;
        string statsText;
        string _logText;
        string _logText2;
        string _logText3;
        ProfilerRecorder systemMemoryRecorder;
        ProfilerRecorder gcMemoryRecorder;
        ProfilerRecorder mainThreadTimeRecorder;

        
        public void Log(string txt)
        {
            _logText = txt;
        }

        public void Log2(string txt)
        {
            _logText2 = txt;
        }

        public void Log3(string txt)
        {
            _logText3 = txt;
        }

        //===========================================================================
        //                             Private Methods
        //===========================================================================


        private static double GetRecorderFrameAverage(ProfilerRecorder recorder)
        {
            var samplesCount = recorder.Capacity;
            if (samplesCount == 0)
                return 0;

            double r = 0;
            unsafe
            {
                var samples = stackalloc ProfilerRecorderSample[samplesCount];
                recorder.CopyTo(samples, samplesCount);
                for (var i = 0; i < samplesCount; ++i)
                    r += samples[i].Value;
                r /= samplesCount;
            }

            return r;
        }

        private void OnEnable()
        {
            systemMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "System Used Memory");
            gcMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Reserved Memory");
            mainThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 15);
        }

        private void OnDisable()
        {
            systemMemoryRecorder.Dispose();
            gcMemoryRecorder.Dispose();
            mainThreadTimeRecorder.Dispose();
        }

        private void Update()
        {
            var sb = new StringBuilder(500);
            sb.AppendLine($"Frame Time: {GetRecorderFrameAverage(mainThreadTimeRecorder) * (1e-6f):F1} ms");
            sb.AppendLine($"GC Memory: {gcMemoryRecorder.LastValue / (1024 * 1024)} MB");
            sb.AppendLine($"System Memory: {systemMemoryRecorder.LastValue / (1024 * 1024)} MB");
            sb.AppendLine($"Log: {_logText}");
            sb.AppendLine($"Log2: {_logText2}");
            sb.AppendLine($"Log3: {_logText3}");
            statsText = sb.ToString();
        }

        private void OnGUI()
        {
            GUI.TextArea(new Rect(10, 30, 250, 200), statsText);
        }

        private void Awake()
        {
            instance = this;
        }
    }
}