using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Expression.Encoder;
using Microsoft.Expression.Encoder.Live;
using eeDevices = Microsoft.Expression.Encoder.Devices;
using System.Drawing;
using System.Windows.Forms;

namespace OGV.Streaming.Models
{
    public delegate void StatusDelegate(object sender, EncodeStatusEventArgs e);

    public delegate void MessageDelegate(string message);

    public interface IEncoderInterface
    {
        

        LiveSource AddRootSource();

        void RemoveRootSource();

        void StopEncoding();

        void CacheSettings();

        void ReadAndApplySettings();

        PreviewWindow SetInputPreviewWindow(Size windowSize, Panel pnlInputPreview);

        event StatusDelegate StatusEvent;

        event MessageDelegate MessageEvent;


    }
}


