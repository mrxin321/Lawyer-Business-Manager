using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace Network
{
    class DownloadCertificateHandler : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }

    public enum DownloadState
    {
        None,
        Start,
        Update,
        Success,
        Error,
        WaitToRemove,
    }

    public class DownloadStatus
    {
        public DownloadState State = DownloadState.None;
        public string Info;
    }

    public class DownloadManagerInfo
    {
        public int FlushSizeKB;
        public int TimeoutSecond;
    }

    public class DownloadItemBase
    {
        protected string Url;

        protected DownloadStatus Status;

        protected Action<DownloadStatus> Callback;

        protected Action<DownloadStatus> ManagerHandler;

        protected DownloadItemBase(string url, Action<DownloadStatus> callback)
        {
            Url = url;
            Callback = callback;
            Status = new DownloadStatus();
        }

        public virtual void InitManagerInfo(DownloadManagerInfo info)
        {

        }

        public virtual void InitManagerHandler(Action<DownloadStatus> handler)
        {
            ManagerHandler = handler;
        }

        protected virtual void ManagerHandle()
        {
            if (ManagerHandler != null)
            {
                ManagerHandler(Status);
            }
        }

        public virtual void SendStatusToUser()
        {
            var callback = Callback;
            var status = Status;
            Utility.ActionOnLateUpdate(this, o =>
            {
                if (callback != null)
                {
                    callback(status);
                }
            }, "SendStatusToUser");
        }

        public virtual void Start(bool reset)
        {
            Status.State = DownloadState.Start;
            Status.Info = "";
        }

        public virtual void Stop()
        {
            Status.State = DownloadState.Error;
            Status.Info = "Stop";
        }
    }

    public class DownloadComponent : MonoBehaviour
    {
        private UnityWebRequest _req;
        private Action<DownloadStatus> _callback;

        private void Init(UnityWebRequest req, Action<DownloadStatus> callback)
        {
            _req = req;
            _callback = callback;
        }

        public static DownloadComponent Create(UnityWebRequest req, Action<DownloadStatus> callback)
        {
            var obj = new GameObject("DownloadComponent");
            DontDestroyOnLoad(obj);
            var com = obj.AddComponent<DownloadComponent>();
            com.Init(req, callback);

            return com;
        }

        public void Release()
        {
            StopAllCoroutines();
            if (_req != null)
            {
                _req.Dispose();
                _req = null;
            }
            Destroy(gameObject);
        }

        void Start()
        {
            StartCoroutine(Download());
        }

        IEnumerator Download()
        {
            if (_req != null)
            {
                yield return _req.SendWebRequest();

                if (_callback != null)
                {
                    var status = new DownloadStatus();

                    if(_req.isNetworkError || _req.isHttpError) {
                        status.State = DownloadState.Error;
                        status.Info = _req.error;
                    }
                    else
                    {
                        status.State = DownloadState.Success;
                        status.Info = _req.downloadHandler.text;
                    }

                    _callback(status);
                }
            }
        }
    }

    public class DownloadGet : DownloadItemBase
    {
        protected DownloadComponent Component;

        protected int Timeout;

        public DownloadGet(string url, Action<DownloadStatus> callback) : base(url, callback)
        {
        }

        public override void Start(bool reset)
        {
            base.Start(reset);

            ReleaseComponent();
            CreateComponent(CreateRequest());
        }

        public override void Stop()
        {
            ReleaseComponent();

            base.Stop();
        }

        public override void InitManagerInfo(DownloadManagerInfo info)
        {
            Timeout = info.TimeoutSecond;
        }

        protected virtual UnityWebRequest CreateRequest()
        {
            var req = UnityWebRequest.Get(Url);
            req.certificateHandler = new DownloadCertificateHandler();
            req.timeout = Timeout;
            return req;
        }

        protected virtual void CreateComponent(UnityWebRequest req)
        {
            Component = DownloadComponent.Create(req, status =>
            {
                Status = status;

                ManagerHandle();

                ReleaseComponent();
            });
        }

        protected virtual void ReleaseComponent()
        {
            if (Component != null)
            {
                Component.Release();
                Component = null;
            }
        }
    }

    public class DownloadPost : DownloadGet
    {
        private WWWForm _form;

        public DownloadPost(string url, WWWForm form, Action<DownloadStatus> callback) : base(url, callback)
        {
            _form = form;
        }

        protected override UnityWebRequest CreateRequest()
        {
            var req = UnityWebRequest.Post(Url, _form);
            req.certificateHandler = new DownloadCertificateHandler();
            req.timeout = Timeout;
            return req;
        }
    }

    public class DownloadHTTP : DownloadItemBase
    {
        private const string DownloadSuffix = ".down";
        private const int DownloadBufferSize = 128 * 1024;

        private string _savePath;

        private int _flushSizeKB;
        private int _timeoutSecond;

        private byte[] _buffer;
        private HttpWebRequest _req;
        private HttpWebResponse _res;
        private Stream _resStream;
        private FileStream _saveFile;

        private int _startLength;
        private long _downloadedLength;
        private long _savedLength;
        private long _targetFileSize;

        private bool _start;

        public DownloadHTTP(string url, string path, Action<DownloadStatus> callback) : base(url, callback)
        {
            _savePath = path;
        }

        public override void InitManagerInfo(DownloadManagerInfo info)
        {
            _flushSizeKB = info.FlushSizeKB;
            _timeoutSecond = info.TimeoutSecond;
        }

        public override void Start(bool reset)
        {
            base.Start(reset);

            Reset();

            _start = true;
            _buffer = new byte[DownloadBufferSize];

            try
            {
                PrepareSaveFile(reset);
                StartAsyncDownload();
            }
            catch (Exception e)
            {
                HandleException(e);
            }
        }

        public override void Stop()
        {
            Reset();

            base.Stop();
        }

        private void HandleException(Exception e)
        {
            Reset();

            Status.State = DownloadState.Error;
            Status.Info = e.Message;
            ManagerHandle();
        }

        private void HandleUpdate()
        {
            Status.State = DownloadState.Update;
            if (_targetFileSize > 0)
            {
                float percent = (float) _savedLength / _targetFileSize;
                Status.Info = percent.ToString("0.00");
            }
            else
            {
                Status.Info = string.Format("Download Size : {0}", _savedLength);
            }
            ManagerHandle();
        }

        private void HandleFinish()
        {
            Reset();

            Status.State = DownloadState.Success;
            Status.Info = _savePath;
            ManagerHandle();
        }

        private int GetFlushSize()
        {
            return _flushSizeKB * 1024;
        }

        private void Reset()
        {
            _start = false;

            if (_resStream != null)
            {
                _resStream.Close();
                _resStream = null;
            }
            if (_saveFile != null)
            {
                _saveFile.Close();
                _saveFile = null;
            }
            if (_res != null)
            {
                _res.Close();
                _res = null;
            }
            if (_req != null)
            {
                _req.Abort();
                _req = null;
            }

            if (_buffer != null)
            {
                Array.Clear(_buffer, 0, _buffer.Length);
                _buffer = null;
            }
        }

        private void PrepareSaveFile(bool reset)
        {
            string downloadFile = _savePath + DownloadSuffix;

            if (reset)
            {
                if (File.Exists(downloadFile))
                {
                    File.Delete(downloadFile);
                }
            }

            if (File.Exists(downloadFile))
            {
                _saveFile = File.OpenWrite(downloadFile);
                _saveFile.Seek(0, SeekOrigin.End);
                _startLength = (int) _saveFile.Length;
                _savedLength = _saveFile.Length;
                _downloadedLength = 0;
            }
            else
            {
                var directory = Path.GetDirectoryName(_savePath);

                if (directory == null) throw new Exception("download save file path error");

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                _saveFile = new FileStream(downloadFile, FileMode.Create, FileAccess.Write);
                _startLength = 0;
                _savedLength = 0;
                _downloadedLength = 0;
            }
        }

        private void StartAsyncDownload()
        {
            ServicePointManager.ServerCertificateValidationCallback =
                (s, certificate, chain, sslPolicyErrors) => true;

            _req = (HttpWebRequest) WebRequest.Create(Url);
            _req.AddRange(_startLength);
            var result = _req.BeginGetResponse(ResponseCallback, this);

            ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle, TimeoutCallback, this, _timeoutSecond * 1000, true);
        }

        private void SaveBytesToFile(byte[] bytes)
        {
            if (bytes != null)
            {
                try
                {
                    int length = bytes.Length;
                    _saveFile.Write(bytes, 0, length);
                    _downloadedLength += length;
                    _savedLength += length;

                    if (_downloadedLength >= GetFlushSize())
                    {
                        _saveFile.Flush();
                        _downloadedLength = 0;
                    }
                }
                catch(Exception e)
                {
                    HandleException(e);
                }
            }
        }

        private static void TimeoutCallback(object state, bool timeout)
        {
            if (timeout)
            {
                var download = (DownloadHTTP) state;
                if (download != null)
                {
                    download.HandleException(new Exception("Timeout"));
                }
            }
        }

        private static void ResponseCallback(IAsyncResult asyncResult)
        {
            var state = (DownloadHTTP) asyncResult.AsyncState;

            if (state == null || !state._start)
            {
                return;
            }

            try
            {
                state._res = (HttpWebResponse) state._req.EndGetResponse(asyncResult);
                var targetSizeStr = state._res.GetResponseHeader("Content-Length");
                if (!string.IsNullOrEmpty(targetSizeStr))
                {
                    state._targetFileSize = long.Parse(targetSizeStr);
                }
                state._resStream = state._res.GetResponseStream();
                if (state._resStream != null)
                    state._resStream.BeginRead(state._buffer, 0, DownloadBufferSize, ReadCallBack, state);
            }
            catch (Exception e)
            {
                state.HandleException(e);
            }
        }

        private static void ReadCallBack(IAsyncResult asyncResult)
        {
            var state = (DownloadHTTP) asyncResult.AsyncState;

            if (state == null || !state._start)
            {
                return;
            }

            try
            {
                int bytesRead = state._resStream.EndRead(asyncResult);
                if (bytesRead > 0)
                {
                    byte[] saveBytes;
                    if (bytesRead == DownloadBufferSize)
                    {
                        saveBytes = state._buffer;
                    }
                    else
                    {
                        byte[] bytes = new byte[bytesRead];
                        Buffer.BlockCopy(state._buffer, 0, bytes, 0, bytesRead);
                        saveBytes = bytes;
                    }

                    state.SaveBytesToFile(saveBytes);
                    state.HandleUpdate();
                    state._resStream.BeginRead(state._buffer, 0, DownloadBufferSize, ReadCallBack, state);
                }
                else
                {
                    var tempFile = state._savePath + DownloadSuffix;
                    File.Move(tempFile, state._savePath);

                    state.HandleFinish();
                }
            }
            catch (Exception e)
            {
                state.HandleException(e);
            }
        }
    }

    public class DownloadingInfo
    {
        public DownloadState State;
        public int CountDelayFrame;
    }

    public class NetworkManager : Singleton<NetworkManager>
    {
        public static Action<bool> NetworkConnectionChange;

        //private bool _lazyInit;

        #region Unity
        [SerializeField] public int ParallelCount = 4;
        [SerializeField] public int FlushSizeKB = 1024;
        [SerializeField] public int TimeoutSecond = 10;
        [SerializeField] public int WaitDelayFrame = 3;

        protected override void Awake()
        {
            base.Awake();
            Init();
        }
        protected override void Reset()
        {
            LazyInit = false;
            _lastConnected = false;
            _downloadList.Clear();
            _downloadingList.Clear();
        }
        void Update()
        {
            if (!LazyInit)
            {
                return;
            }

            if (CheckNetworkChange())
            {
                return;
            }

            ScheduleDownload();
        }

        void LateUpdate()
        {
            if (!LazyInit)
            {
                return;
            }

            DelayCheckError();
        }
        #endregion

        private static object _downloadListLock = new object();

        private bool _lastConnected;

        private Queue<DownloadItemBase> _downloadList;
        private Dictionary<DownloadItemBase, DownloadingInfo> _downloadingList;

        public bool IsConnected
        {
            get
            {
                return true;
            }
        }

        public void Clear()
        {
            Reset();
        }

        private void Init()
        {
            _downloadList = new Queue<DownloadItemBase>();
            _downloadingList = new Dictionary<DownloadItemBase, DownloadingInfo>();
        }
        public bool LazyInit { get; set; }
        public void Download(DownloadItemBase item)
        {
            if (!LazyInit)
            {
                LazyInit = true;
                _lastConnected = true;
            }

            lock (_downloadListLock)
            {
                _downloadList.Enqueue(item);
            }

            var info = new DownloadManagerInfo
            {
                FlushSizeKB = FlushSizeKB,
                TimeoutSecond = TimeoutSecond,
            };

            item.InitManagerInfo(info);

            item.InitManagerHandler(status =>
            {
                if (_downloadingList.ContainsKey(item))
                {
                    var download = _downloadingList[item];
                    var state = DownloadState.None;
                    switch (status.State)
                    {
                        case DownloadState.Update:
                            item.SendStatusToUser();
                            state = DownloadState.Update;
                            break;
                        case DownloadState.Success:
                            item.SendStatusToUser();
                            state = DownloadState.WaitToRemove;
                            break;
                        case DownloadState.Error:
                            item.SendStatusToUser();
                            // block error to wait reconnection
                            state = DownloadState.Error;
                            break;
                    }

                    download.State = state;
                }
            });
        }

        private void StartDownloadingItem()
        {
            DownloadItemBase item;
            lock (_downloadListLock)
            {
                item = _downloadList.Dequeue();
            }

            if (item != null)
            {
                item.Start(true);
                var info = new DownloadingInfo
                {
                    State = DownloadState.Start,
                    CountDelayFrame = WaitDelayFrame,
                };
                _downloadingList.Add(item, info);
            }
        }

        private void NetworkDisconnected()
        {
            foreach (var item in _downloadingList)
            {
                if (item.Value.State == DownloadState.Start || item.Value.State == DownloadState.Update)
                {
                    item.Key.Stop();
                } 
            }

            Utility.SafePostEvent(NetworkConnectionChange, false);
        }

        private void NetworkReconnected()
        {
            foreach (var item in _downloadingList)
            {
                if (item.Value.State == DownloadState.Error)
                {
                    item.Key.Start(false);

                    // reset delay
                    item.Value.CountDelayFrame = WaitDelayFrame;
                    item.Value.State = DownloadState.Start;
                } 
            }

            Utility.SafePostEvent(NetworkConnectionChange, true);
        }

        private void DelayCheckError()
        {
            if (_lastConnected)
            {
                foreach (var item in _downloadingList)
                {
                    if (item.Value.State == DownloadState.Error)
                    {
                        if (--item.Value.CountDelayFrame <= 0)
                        {
                            item.Key.SendStatusToUser();
                            item.Value.State = DownloadState.WaitToRemove;
                        }
                    } 
                }
            }
        }

        private void ScheduleDownload()
        {
            RemoveFinished();
            if (_downloadList.Count > 0)
            {
                while (_downloadingList.Count < ParallelCount && _downloadList.Count > 0)
                {
                    StartDownloadingItem();
                }
            }
        }

        private bool CheckNetworkChange()
        {
            var lastConnected = _lastConnected;
            _lastConnected = IsConnected;

            if (!IsConnected && lastConnected)
            {    
                NetworkDisconnected();
                return true;
            } 
            
            if (!lastConnected && IsConnected)
            {
                NetworkReconnected();
                return true;
            }

            return false;
        } 

        private void RemoveFinished()
        {
            List<DownloadItemBase> remove = null;
            foreach (var item in _downloadingList)
            {
                if (item.Value.State == DownloadState.WaitToRemove)
                {
                    if (remove == null)
                    {
                        remove = new List<DownloadItemBase>();
                    }
                    remove.Add(item.Key);
                }
            }

            if (remove != null)
            {
                foreach (var item in remove)
                {
                    _downloadingList.Remove(item);
                }
            }
        }
    }
}
