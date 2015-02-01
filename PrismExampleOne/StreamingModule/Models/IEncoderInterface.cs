using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Expression.Encoder;
using Microsoft.Expression.Encoder.Live;
using eeDevices = Microsoft.Expression.Encoder.Devices;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Practices.Prism.Commands;

namespace OGV.Streaming.Models
{
       public interface IEncoderInterface
    {

        LiveSource AddRootSource();

        void RemoveRootSource();

        void CacheSettings();

        void ReadAndApplySettings();

        PreviewWindow SetInputPreviewWindow(Size windowSize, Panel pnlInputPreview);

    }
}


