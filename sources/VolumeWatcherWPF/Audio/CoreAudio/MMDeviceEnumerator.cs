using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Audio.CoreAudio.Interfaces;

namespace Audio.CoreAudio
{
    [ComImport, Guid(ComIIds.IID_DEVICE_ENUMERATOR_CID)]
    internal class _MMDeviceEnumerator
    {
    }

    public class MMDeviceEnumerator
    {
        private readonly IMMDeviceEnumerator _realEnumerator = new _MMDeviceEnumerator() as IMMDeviceEnumerator;
        private MMNotificationClient innerListener;
        private bool IsRegistered = false;

        private event MMNotificationClientDeviceStateChangedDelegate     _OnDeviceStateChanged;
        private event MMNotificationClientDeviceAddedDelegate            _OnDeviceAdded;
        private event MMNotificationClientDeviceRemovedDelegate          _OnDeviceRemoved;
        private event MMNotificationClientDefaultDeviceChangedDelegate   _OnDefaultDeviceChanged;
        private event MMNotificationClientPropertyValueChangedDelegate   _OnPropertyValueChanged;

        public MMDeviceEnumerator()
        {
            innerListener = new MMNotificationClient(this);
        }

        public MMDeviceCollection EnumAudioEndpoints(EDataFlow dataFlow, EDeviceState dwStateMask)
        {
            IMMDeviceCollection result;
            Marshal.ThrowExceptionForHR(_realEnumerator.EnumAudioEndpoints(dataFlow, dwStateMask, out result));
            return new MMDeviceCollection(result);
            //return result;
        }

        public MMDevice GetDefaultAudioEndpoint(EDataFlow dataFlow, ERole role)
        {
            IMMDevice result;
            Marshal.ThrowExceptionForHR(_realEnumerator.GetDefaultAudioEndpoint(dataFlow, role, out result));
            return new MMDevice(result);
            //return result;
        }

        public MMDevice GetDevice(string deviceId)
        {
            IMMDevice result;
            Marshal.ThrowExceptionForHR(_realEnumerator.GetDevice(deviceId, out result));
            return new MMDevice(result);
            //return result;
        }

        /// <summary>
        /// Registers a client's notification callback interface.
        /// </summary>
        /// <param name="client">The <see cref="IMMNotificationClient"/> interface that the client is registering for notification callbacks.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        internal void RegisterEndpointNotificationCallback(IMMNotificationClient client)
        {
            Marshal.ThrowExceptionForHR(_realEnumerator.RegisterEndpointNotificationCallback(client));
        }

        /// <summary>
        /// Deletes the registration of a notification interface that the client registered in a previous call
        /// to the <see cref="IMMDeviceEnumerator.RegisterEndpointNotificationCallback"/> method.
        /// </summary>
        /// <param name="client">A <see cref="IMMNotificationClient"/> interface that was previously registered for notification callbacks.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        internal void UnregisterEndpointNotificationCallback(IMMNotificationClient client)
        {
            Marshal.ThrowExceptionForHR(_realEnumerator.UnregisterEndpointNotificationCallback(client));
        }

        internal void FireOnDeviceStateChanged([MarshalAs(UnmanagedType.LPWStr)] string deviceId, EDeviceState newState)
        {
            if (_OnDeviceStateChanged == null) return;

            _OnDeviceStateChanged(deviceId, newState);
        }

        internal void FireOnDeviceAdded([MarshalAs(UnmanagedType.LPWStr)] string deviceId)
        {
            if (_OnDeviceAdded == null) return;

            _OnDeviceAdded(deviceId);
        }

        internal void FireOnDeviceRemoved([MarshalAs(UnmanagedType.LPWStr)] string deviceId)
        {
            if (_OnDeviceRemoved == null) return;

            _OnDeviceRemoved(deviceId);
        }

        internal void FireOnDefaultDeviceChanged(
            [MarshalAs(UnmanagedType.I4)] EDataFlow dataFlow,
            [MarshalAs(UnmanagedType.I4)] ERole deviceRole,
            [MarshalAs(UnmanagedType.LPWStr)] string defaultDeviceId)
        {
            if (_OnDefaultDeviceChanged == null) return;

            _OnDefaultDeviceChanged(dataFlow, deviceRole, defaultDeviceId);
        }

        internal void FireOnPropertyValueChanged(
            [MarshalAs(UnmanagedType.LPWStr)] string deviceId, PropertyKey propertyKey)
        {
            if (_OnPropertyValueChanged == null) return;

            _OnPropertyValueChanged(deviceId, propertyKey);
        }

        /// <summary>
        /// delegateに登録されている関数数を返す。対象がnullの場合は0を返す。
        /// </summary>
        /// <param name="d">評価対象のdelegate</param>
        /// <returns></returns>
        private int NumInvokeDelegates( [In]Delegate d)
        {
            if(d == null)
            {
                return 0;
            }
            return d.GetInvocationList().Length;
        }

        /// <summary>
        /// イベント通知に関するdelegateに登録された関数が存在するかどうか
        /// </summary>
        /// <returns></returns>
        private bool HasAllInvokeDelegates()
        {
            Delegate[] dels = {
                _OnDeviceStateChanged,
                _OnDeviceAdded,
                _OnDeviceRemoved,
                _OnDefaultDeviceChanged,
                _OnPropertyValueChanged
            };

            foreach(var d in dels)
            {
                if (NumInvokeDelegates(d) > 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 登録されているdelegateを評価し、Callbackの設定・解除を行う
        /// </summary>
        private void UpdateRegisterNotification()
        {
            //
            // 登録されているdelegateを評価し、Callbackの設定・解除を行う
            //
            if (HasAllInvokeDelegates())
            {
                if (!IsRegistered)
                {
                    // delegateは登録されている、Callbackは未設定
                    // ⇒Callbackを登録する
                    RegisterEndpointNotificationCallback(innerListener);
                    IsRegistered = true;
                }
            }
            else
            {
                if (IsRegistered)
                {
                    // delegateは登録されていない、Callbackは設定済
                    // ⇒Callbackを解除する
                    UnregisterEndpointNotificationCallback(innerListener);
                    IsRegistered = false;
                }
            }
        }

        public event MMNotificationClientDeviceStateChangedDelegate OnDeviceStateChanged
        {
            add
            {
                _OnDeviceStateChanged += value;
                UpdateRegisterNotification();
            }
            remove
            {
                _OnDeviceStateChanged -= value;
                UpdateRegisterNotification();
            }
        }

        public event MMNotificationClientDeviceAddedDelegate OnDeviceAdded
        {
            add
            {
                _OnDeviceAdded += value;
                UpdateRegisterNotification();
            }
            remove
            {
                _OnDeviceAdded -= value;
                UpdateRegisterNotification();
            }
        }
        public event MMNotificationClientDeviceRemovedDelegate OnDeviceRemoved
        {
            add
            {
                _OnDeviceRemoved += value;
                UpdateRegisterNotification();
            }
            remove
            {
                _OnDeviceRemoved -= value;
                UpdateRegisterNotification();
            }
        }
        public event MMNotificationClientDefaultDeviceChangedDelegate OnDefaultDeviceChanged
        {
            add
            {
                _OnDefaultDeviceChanged += value;
                UpdateRegisterNotification();
            }
            remove
            {
                _OnDefaultDeviceChanged -= value;
                UpdateRegisterNotification();
            }
        }
        public event MMNotificationClientPropertyValueChangedDelegate OnPropertyValueChanged
        {
            add
            {
                _OnPropertyValueChanged += value;
                UpdateRegisterNotification();
            }
            remove
            {
                _OnPropertyValueChanged -= value;
                UpdateRegisterNotification();
            }
        }
    }


    class MMNotificationClient : IMMNotificationClient
    {
        private MMDeviceEnumerator parent;

        internal MMNotificationClient(MMDeviceEnumerator _parent)
        {
            parent = _parent;
        }

        public void OnDeviceStateChanged(string deviceId, EDeviceState newState)
        {
            parent.FireOnDeviceStateChanged(deviceId, newState);
        }

        public void OnDeviceAdded(
            [MarshalAs(UnmanagedType.LPWStr)] string deviceId)
        {
            parent.FireOnDeviceAdded(deviceId);
        }

        public void OnDeviceRemoved(
            [MarshalAs(UnmanagedType.LPWStr)] string deviceId)
        {
            parent.FireOnDeviceRemoved(deviceId);
        }

        public void OnDefaultDeviceChanged(
            [MarshalAs(UnmanagedType.I4)] EDataFlow dataFlow,
            [MarshalAs(UnmanagedType.I4)] ERole deviceRole,
            [MarshalAs(UnmanagedType.LPWStr)] string defaultDeviceId)
        {
            parent.FireOnDefaultDeviceChanged(dataFlow, deviceRole, defaultDeviceId);
        }

        public void OnPropertyValueChanged(
            [MarshalAs(UnmanagedType.LPWStr)] string deviceId, PropertyKey propertyKey)
        {
            parent.FireOnPropertyValueChanged(deviceId, propertyKey);
        }
    }
}
