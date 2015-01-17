﻿using System;
using System.Collections.Generic;
using Microsoft.Expression.Encoder;
using Microsoft.Expression.Encoder.Live;
using eeDevices = Microsoft.Expression.Encoder.Devices;
using OGV.Cache;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Expression.Encoder.Devices;

namespace OGV.Streaming.Models
{

    public delegate void LoadCompleteDelegate(object sender, EventArgs e);

    public class LiveEncodingSource : EncodingSourceBase, IEncoderInterface
    {
        #region Fields - Properties - Events

        public IList<eeDevices.EncoderDevice> VideoDevices { get; set; }

        public eeDevices.EncoderDevice VideoDevice { get; set; }

        public IList<eeDevices.EncoderDevice> AudioDevices { get; set; }

        public eeDevices.EncoderDevice AudioDevice { get; set; }

        public bool UseLocalArchive { get; set; }

        public string ArchiveFileName { get; set; }

        private string settingsCacheFileName = "live_settings.xml";

        public event LoadCompleteDelegate LoadCompletedEvent;

        #endregion

        #region Constructors
        public LiveEncodingSource()
            : base()
        {
            VideoDevices = eeDevices.EncoderDevices.FindDevices(eeDevices.EncoderDeviceType.Video);
            AudioDevices = eeDevices.EncoderDevices.FindDevices(eeDevices.EncoderDeviceType.Audio);

            SetDefaultDevices();

           
        }

        #endregion

        #region IEncoderInterface
        public override void PreconnectPublishingPoint()
        {
            try
            {
                //reset the publishing point and track each step
                if (StreamLive)
                {

                }
              

                if (_job == null)
                {

                    _job = new LiveJob();
                    _job.Status += new EventHandler<EncodeStatusEventArgs>(job_Status);
                    // remove all existing output formats it will have
                    //to add them new each time
                    _job.PublishFormats.Clear();

                }
                

                if (StreamLive)
                {
                    PushBroadcastPublishFormat pushformat = new PushBroadcastPublishFormat();
                    if( ! string.IsNullOrEmpty(PublishingPoint))
                    {
                        pushformat.PublishingPoint = new Uri(PublishingPoint);
                        pushformat.EventId = GenerateEventID();
                        Job.PublishFormats.Add(pushformat);
                        Job.PreConnectPublishingPoint();

                        int x = 0;
                        if (State != "Connected")
                        {
                            State = "Disconnected";
                            throw new Exception(Message);
                        }
                    }
                   
                }



                //create the base file structure if it does not exist
                string myVideos = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                myVideos = System.IO.Path.Combine(myVideos, "OV Videos");
                if (!System.IO.Directory.Exists(myVideos))
                    System.IO.Directory.CreateDirectory(myVideos);

                //add a folder for todays segements
                string todaysFolder = string.Format("{0}_{1}_{2}", DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Year);
                myVideos = System.IO.Path.Combine(myVideos, todaysFolder);
                if (!System.IO.Directory.Exists(myVideos))
                    System.IO.Directory.CreateDirectory(myVideos);

                //add the file archive output format by choosing
                //a segment file name that has not been used today
                string fileName = string.Format("{0}_{1}.ismv", SessionName, 1);
                int i = 1;
                string fullPath = System.IO.Path.Combine(myVideos, fileName);
                while (System.IO.File.Exists(fullPath))
                {
                    fileName = string.Format("{0}_{1}.ismv", SessionName, i++);
                    fullPath = System.IO.Path.Combine(myVideos, fileName);

                }

                FileArchivePublishFormat archiveFormat = new FileArchivePublishFormat(fullPath);
                Job.PublishFormats.Add(archiveFormat);

                if(_liveSource == null)
                {
                    ActivateSource(VideoDevice,AudioDevice);

                }

                if (LoadCompletedEvent != null)
                    LoadCompletedEvent(this, new EventArgs());

            }
            catch (Exception ex)
            {

                Exception exf = new Exception(ex.Message, ex);
                throw exf;
            }


        }

        private void timerFrameTrack_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //report the sample statistics
            NumberOfDroppedSamples = Job.NumberOfDroppedSamples;
            NumberOfSamples = Job.NumberOfEncodedSamples;

            //report the time
            SessionTime = string.Format("{0} : {1} : {2}",
                (DateTime.Now - _startTime).Hours,
                (DateTime.Now - _startTime).Minutes,
                (DateTime.Now - _startTime).Seconds);

            if (NumberOfDroppedSamples > 100)
            {
                MessageBox.Show(string.Format("The number of dropped samples is excessive.  Dropped Samples: {0} " +
                    "- This is an indication of poor network performance. Live streaming will be stopped." +
                    " Disable live streaming and turn on archiving.", NumberOfDroppedSamples));
                timerFrameTrack.Stop();
                Job.StopEncoding();

            }
        }

        public LiveSource AddRootSource()
        {
            _liveSource = Job.AddDeviceSource(VideoDevice, AudioDevice);
            return _liveSource;
        }

        public void RemoveRootSource()
        {
            if (Job == null)
                return;

            if (Job.DeviceSources.Count > 0)
                Job.RemoveDeviceSource(Job.DeviceSources[0]);
        }

        public void StopEncoding()
        {
            base.Message = "Shutting down the live stream...";
            timerFrameTrack.Stop();
            base.Job.StopEncoding();
            base.State = "Disconnected";
            NumberOfDroppedSamples = 0;
            NumberOfSamples = 0;
            SessionTime = "00 : 00 : 00";
        }

        public PreviewWindow SetInputPreviewWindow(System.Drawing.Size windowSize, System.Windows.Forms.Panel pnlInputPreview)
        {

            HandleRef h = new HandleRef(pnlInputPreview, pnlInputPreview.Handle);
            PreviewWindow prev = new PreviewWindow(h);
            prev.SetSize(windowSize);
            return prev;
        }

        /// <summary>
        /// Saves to disk, the users chosen runtime settings and use them on the next run
        /// sort of like default settings.
        /// </summary>
        public void CacheSettings()
        {
            List<string> names = new List<string>();
            List<string> values = new List<string>();

            string audioDevicePath = Job.DeviceSources[0].AudioDevice.DevicePath;
            string videoDevicePath = Job.DeviceSources[0].VideoDevice.DevicePath;
            string presetName = SelectedPreset.Name;

            names.Add("AudioDevice");
            values.Add(audioDevicePath);

            names.Add("VideoDevice");
            values.Add(videoDevicePath);

            names.Add("Preset");
            values.Add(presetName);

            CacheManager cm = new CacheManager();
            cm.WriteCacheSettings(names, values, settingsCacheFileName);

        }


        /// <summary>
        /// Reads and apply the users last run time settngs.
        /// </summary>
        public void ReadAndApplySettings()
        {
            try
            {

                //read cache settings
                CacheManager cm = new CacheManager();
                XDocument doc = new XDocument();
                string settingsContent = cm.ReadCacheFile(settingsCacheFileName);
                if (settingsContent == string.Empty)
                    return;

                doc = XDocument.Parse(settingsContent);

                string audioDeviceName = doc.Element("Settings").Element(@"AudioDevice").Value;
                string videoDeviceName = doc.Element("Settings").Element(@"VideoDevice").Value;
                string presetName = doc.Element("Settings").Element(@"Preset").Value;

                //set the video device
                foreach (eeDevices.EncoderDevice x in VideoDevices)
                {
                    if (x.DevicePath == videoDeviceName)
                        VideoDevice = x;
                }

               
                

                //set the video devices
                foreach (eeDevices.EncoderDevice y in AudioDevices)
                {
                    if (y.DevicePath == videoDeviceName)
                    {
                        AudioDevice = y;
                    }
                }

                //apply the preset
                foreach (Preset p in Presets)
                {
                    if (p.Name == presetName)
                        SelectedPreset = p;
                }

            }
            catch (Exception ex)
            {

                throw new Exception("Error reading cached settings.", ex);
            }
        }

        event StatusDelegate IEncoderInterface.StatusEvent
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        event MessageDelegate IEncoderInterface.MessageEvent
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        #endregion

        #region Event Handlers



        #endregion

        #region Methods and Functions

        /// <summary>
        /// Sets the active devices to the first devices in the list
        /// </summary>
        private void SetDefaultDevices()
        {
            if (VideoDevices.Count > 0)
            {
                //set to the first device that can capture live video
                VideoDevice = VideoDevices.Where(dev => dev.Name =="Integrated Webcam").First();
            }
                

            if (AudioDevices.Count > 0)
                AudioDevice = AudioDevices[0];
        }

        public void ActivateSource( EncoderDevice video, EncoderDevice audio )
        {
            foreach (LiveDeviceSource ds in _job.DeviceSources)
            {
                _job.RemoveDeviceSource(ds);
            }
            
            VideoDevice = video;
            AudioDevice = audio;
            _liveSource = _job.AddDeviceSource(VideoDevice, AudioDevice);
            _job.ActivateSource(_liveSource);


        }
        #endregion
    }
}
